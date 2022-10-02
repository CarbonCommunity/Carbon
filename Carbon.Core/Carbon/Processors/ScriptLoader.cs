///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Carbon.Core.Processors;
using Facepunch;
using Humanlights.Extensions;
using Oxide.Core;
using Oxide.Plugins;
using UnityEngine;

namespace Carbon.Core
{
	public class ScriptLoader : IDisposable
	{
		public List<Script> Scripts { get; set; } = new List<Script>();

		public List<string> Files { get; set; } = new List<string>();
		public List<string> Sources { get; set; } = new List<string>();
		public List<string> Namespaces { get; set; } = new List<string>();
		public string Source { get; set; }
		public bool IsCore { get; set; }

		public BaseProcessor.Parser Parser { get; set; }

		public AsyncPluginLoader AsyncLoader { get; set; } = new AsyncPluginLoader();

		public void Load(bool customFiles = false, bool customSources = false, GameObject target = null)
		{
			try
			{
				if (!customFiles)
				{
					if (Files.Count == 0) return;
				}

				if (!customSources) GetSources();

				if (Sources.Count > 0) Source = Sources[0];

				if (Parser != null)
				{
					Parser.Process(Source, out var newSource);

					if (!string.IsNullOrEmpty(newSource))
					{
						Source = newSource;
					}
				}

				CarbonCore.Instance.ScriptProcessor.StartCoroutine(Compile());
			}
			catch (Exception exception)
			{
				CarbonCore.Error($"Failed loading script;", exception);
			}
		}

		public static void LoadAll()
		{
			var files = OsEx.Folder.GetFilesWithExtension(CarbonCore.GetPluginsFolder(), "cs");

			CarbonCore.Instance.ScriptProcessor.Clear();
			CarbonCore.Instance.ScriptProcessor.IgnoreList.Clear();

			foreach (var file in files)
			{
				var plugin = new ScriptProcessor.Script();
				plugin.File = file;
				CarbonCore.Instance.ScriptProcessor.InstanceBuffer.Add(Path.GetFileNameWithoutExtension(file), plugin);
			}

			foreach (var plugin in CarbonCore.Instance.ScriptProcessor.InstanceBuffer)
			{
				plugin.Value.SetDirty();
			}
		}

		public void Clear()
		{
			AsyncLoader?.Abort();
			AsyncLoader = null;

			for (int i = 0; i < Scripts.Count; i++)
			{
				var plugin = Scripts[i];
				if (plugin.IsCore) continue;

				CarbonCore.Instance.Plugins.Plugins.Remove(plugin.Instance);

				if (plugin.Instance != null)
				{
					try
					{
						HookExecutor.CallStaticHook("OnPluginUnloaded", plugin.Instance);
						plugin.Instance.CallHook("Unload");
						plugin.Instance.IUnload();
						CarbonLoader.RemoveCommands(plugin.Instance);
						plugin.Instance.Dispose();
						CarbonCore.Log($"Unloaded plugin {plugin.Instance.ToString()}");
					}
					catch (Exception ex) { CarbonCore.Error($"Failed unloading '{plugin.Instance}'", ex); }
				}

				plugin.Dispose();
			}

			if (Scripts.Count > 0)
			{
				Scripts.RemoveAll(x => !x.IsCore);
			}
		}
		protected void GetSources()
		{
			Sources.Clear();

			foreach (var file in Files)
			{
				if (!OsEx.File.Exists(file))
				{
					CarbonCore.Warn($"Plugin \"{file}\" does not exist or the path is misspelled.");
					continue;
				}

				var source = OsEx.File.ReadText(file);
				Sources.Add(source);
			}
		}

		public IEnumerator Compile()
		{
			if (string.IsNullOrEmpty(Source))
			{
				CarbonCore.Warn("Attempted to compile an empty string of source code.");
				yield break;
			}

			var lines = Source.Split('\n');
			var resultReferences = Pool.GetList<string>();
			foreach (var reference in lines)
			{
				try
				{
					if (reference.StartsWith("// Reference:") || reference.StartsWith("//Reference:"))
					{
						var @ref = $"{reference.Replace("// Reference:", "").Replace("//Reference:", "")}".Trim();
						resultReferences.Add(@ref);
						CarbonCore.Log($" Added reference: {@ref}");
					}
				}
				catch { }
			}

			var resultRequires = Pool.GetList<string>();
			foreach (var require in lines)
			{
				try
				{
					if (require.StartsWith("// Requires:") || require.StartsWith("//Requires:"))
					{

						var @ref = $"{require.Replace("// Requires:", "").Replace("//Requires:", "")}".Trim();
						resultRequires.Add(@ref);
						CarbonCore.Log($" Added required plugin: {@ref}");
					}
				}
				catch { }
			}

			Pool.Free(ref lines);
			if (Files.Count > 0) AsyncLoader.FilePath = Files[0];
			AsyncLoader.Source = Source;
			AsyncLoader.References = resultReferences?.ToArray();
			AsyncLoader.Requires = resultRequires?.ToArray();
			Pool.FreeList(ref resultReferences);
			Pool.FreeList(ref resultRequires);

			var requires = Pool.GetList<Plugin>();
			var noRequiresFound = false;
			foreach (var require in AsyncLoader.Requires)
			{
				var plugin = CarbonCore.Instance.CorePlugin.plugins.Find(require);
				if (plugin == null)
				{
					CarbonCore.Warn($"Couldn't find required plugin '{require}' for '{(Files.Count > 0 ? Path.GetFileNameWithoutExtension(Files[0]) : "<unknown>")}'");
					noRequiresFound = true;
				}
				else requires.Add(plugin);
			}

			if (noRequiresFound)
			{
				Pool.FreeList(ref requires);
				yield break;
			}

			AsyncLoader.Start();

			while (AsyncLoader != null && !AsyncLoader.IsDone) { yield return null; }

			if (AsyncLoader == null) yield break;

			if (AsyncLoader.Assembly == null || AsyncLoader.Exceptions.Count != 0)
			{
				CarbonCore.Error($"Failed compiling '{AsyncLoader.FilePath}':");
				for (int i = 0; i < AsyncLoader.Exceptions.Count; i++)
				{
					var error = AsyncLoader.Exceptions[i];
					CarbonCore.Error($"  {i + 1:n0}. {error.Error.ErrorText}\n     ({error.Error.FileName} {error.Error.Column} line {error.Error.Line})");
				}
				yield break;
			}

			CarbonCore.Warn($" Compiling '{(Files.Count > 0 ? Path.GetFileNameWithoutExtension(Files[0]) : "<unknown>")}' took {AsyncLoader.CompileTime * 1000:0}ms...");

			try
			{
				CarbonLoader.AssemblyCache.Add(AsyncLoader.Assembly);

				var assembly = AsyncLoader.Assembly;
				var pluginIndex = 0;

				foreach (var type in assembly.GetTypes())
				{
					if (string.IsNullOrEmpty(type.Namespace) ||
						!(type.Namespace.Equals("Oxide.Plugins") ||
						type.Namespace.Equals("Carbon.Plugins"))) continue;

					if (CarbonCore.Instance.Config.HookValidation)
					{
						var counter = 0;
						foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
						{
							if (CarbonHookValidator.IsIncompatibleOxideHook(method.Name))
							{
								CarbonCore.Warn($" Hook '{method.Name}' is not supported.");
								counter++;
							}
						}

						if (counter > 0)
						{
							CarbonCore.Warn($" Plugin '{type.Name}' uses {counter:n0} Oxide hooks that Carbon doesn't support yet.\n The plugin will not work as expected.");
						}
					}

					var info = type.GetCustomAttribute(typeof(InfoAttribute), true) as InfoAttribute;
					if (info == null) continue;

					if (requires.Any(x => x.Name == info.Title)) continue;

					var description = type.GetCustomAttribute(typeof(DescriptionAttribute), true) as DescriptionAttribute;
					var plugin = Script.Create(Sources[pluginIndex], assembly, type);

					plugin.Name = info.Title;
					plugin.Author = info.Author;
					plugin.Version = info.Version;
					plugin.Description = description?.Description;

					plugin.Instance = Activator.CreateInstance(type) as RustPlugin;
					plugin.IsCore = IsCore;
					plugin.Instance.Requires = requires.ToArray();
					plugin.Instance.SetProcessor(CarbonCore.Instance.ScriptProcessor);
					plugin.Instance.CompileTime = AsyncLoader.CompileTime;

					plugin.Instance.CallHook("SetupMod", null, info.Title, info.Author, info.Version, plugin.Description);
					HookExecutor.CallStaticHook("OnPluginLoaded", plugin);
					plugin.Instance.IInit();
					try { plugin.Instance.DoLoadConfig(); } catch (Exception loadException) { plugin.Instance.LogError($"Failed loading config.", loadException); }
					plugin.Instance.Load();

					if (CarbonCore.IsServerFullyInitialized)
					{
						try
						{
							plugin.Instance.CallHook("OnServerInitialized");
							plugin.Instance.CallHook("OnServerInitialized", CarbonCore.IsServerFullyInitialized);
						}
						catch (Exception initException)
						{
							plugin.Instance.LogError($"Failed OnServerInitialized.", initException);
						}
					}

					plugin.Loader = this;

					CarbonLoader.AppendAssembly(plugin.Name, AsyncLoader.Assembly);
					CarbonCore.Instance.Plugins.Plugins.Add(plugin.Instance);

					if (info != null)
					{
						DebugEx.Log($"Loaded plugin {info.Title} v{info.Version} by {info.Author}");
					}

					CarbonLoader.ProcessCommands(type, plugin.Instance);

					Scripts.Add(plugin);
				}
			}
			catch (Exception exception)
			{
				CarbonCore.Error($"Failed to compile: ", exception);
			}

			Pool.FreeList(ref requires);

			yield break;
		}

		public void Dispose()
		{

		}

		[Serializable]
		public class Script : IDisposable
		{
			public Assembly Assembly { get; set; }
			public Type Type { get; set; }

			public string Name;
			public string Author;
			public VersionNumber Version;
			public string Description;
			public string Source;
			public ScriptLoader Loader;
			public RustPlugin Instance;
			public bool IsCore;

			public static Script Create(string source, Assembly assembly, Type type)
			{
				return new Script
				{
					Source = source,
					Assembly = assembly,
					Type = type,

					Name = null,
					Author = null,
					Version = new VersionNumber(1, 0, 0),
					Description = null,
				};
			}

			public void Dispose()
			{
				Assembly = null;
				Type = null;
			}

			public override string ToString()
			{
				return $"{Name} v{Version}";
			}
		}
	}
}
