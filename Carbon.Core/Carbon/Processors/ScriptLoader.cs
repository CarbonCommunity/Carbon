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

		public string File { get; set; }
		public string Source { get; set; }
		public bool IsCore { get; set; }

		public bool HasFinished { get; set; }

		public BaseProcessor.Parser Parser { get; set; }
		public AsyncPluginLoader AsyncLoader { get; set; } = new AsyncPluginLoader();

		internal WaitForSeconds _serverExhale = new WaitForSeconds(0.1f);

		public void Load()
		{
			try
			{
				if (!string.IsNullOrEmpty(File)) Source = OsEx.File.ReadText(File);

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
				Carbon.Logger.Error($"Failed loading script;", exception);
			}
		}

		public static void LoadAll()
		{
			var files = OsEx.Folder.GetFilesWithExtension(CarbonDefines.GetPluginsFolder(), "cs");

			CarbonCore.Instance.ScriptProcessor.Clear();
			CarbonCore.Instance.ScriptProcessor.IgnoreList.Clear();

			foreach (var file in files)
			{
				var plugin = new ScriptProcessor.Script { File = file };
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
						Carbon.Logger.Log($"Unloaded plugin {plugin.Instance.ToString()}");
					}
					catch (Exception ex) { Carbon.Logger.Error($"Failed unloading '{plugin.Instance}'", ex); }
				}

				plugin.Dispose();
			}

			if (Scripts.Count > 0)
			{
				Scripts.RemoveAll(x => !x.IsCore);
			}
		}

		public IEnumerator Compile()
		{
			if (string.IsNullOrEmpty(Source))
			{
				HasFinished = true;
				Carbon.Logger.Warn("Attempted to compile an empty string of source code.");
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
						Carbon.Logger.Log($" Added reference: {@ref}");
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
						Carbon.Logger.Log($" Added required plugin: {@ref}");
					}
				}
				catch { }
			}

			Pool.Free(ref lines);
			AsyncLoader.FilePath = File;
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
					Carbon.Logger.Warn($"Couldn't find required plugin '{require}' for '{(!string.IsNullOrEmpty(File) ? Path.GetFileNameWithoutExtension(File) : "<unknown>")}'");
					noRequiresFound = true;
				}
				else requires.Add(plugin);
			}

			if (noRequiresFound)
			{
				HasFinished = true;
				Pool.FreeList(ref requires);
				yield break;
			}

			Report.OnPluginAdded?.Invoke(AsyncLoader.FilePath);

			var requiresResult = requires.ToArray();

			AsyncLoader.Start();

			while (AsyncLoader != null && !AsyncLoader.IsDone) { yield return null; }

			if (AsyncLoader == null)
			{
				HasFinished = true;
				yield break;
			}

			if (AsyncLoader.Assembly == null || AsyncLoader.Exceptions.Count != 0)
			{
				Carbon.Logger.Error($"Failed compiling '{AsyncLoader.FilePath}':");
				for (int i = 0; i < AsyncLoader.Exceptions.Count; i++)
				{
					var error = AsyncLoader.Exceptions[i];
					Carbon.Logger.Error($"  {i + 1:n0}. {error.Error.ErrorText}\n     ({error.Error.FileName} {error.Error.Column} line {error.Error.Line})");
				}

				HasFinished = true;
				yield break;
			}

			Carbon.Logger.Warn($" Compiling '{(!string.IsNullOrEmpty(File) ? Path.GetFileNameWithoutExtension(File) : "<unknown>")}' took {AsyncLoader.CompileTime * 1000:0}ms...");

			CarbonLoader.AssemblyCache.Add(AsyncLoader.Assembly);

			var assembly = AsyncLoader.Assembly;

			foreach (var type in assembly.GetTypes())
			{
				try
				{
					if (string.IsNullOrEmpty(type.Namespace) ||
						!(type.Namespace.Equals("Oxide.Plugins") ||
						type.Namespace.Equals("Carbon.Plugins"))) continue;

					if (CarbonCore.Instance.Config.HookValidation)
					{
						var counter = 0;
						foreach (var hook in AsyncLoader.UnsupportedHooks[type])
						{
							Carbon.Logger.Warn($" Hook '{hook}' is not supported.");
							counter++;
						}

						if (counter > 0)
						{
							Carbon.Logger.Warn($" Plugin '{type.Name}' uses {counter:n0} Oxide hooks that Carbon doesn't support yet.");
							Carbon.Logger.Warn("The plugin will not work as expected.");
						}
					}

					var info = type.GetCustomAttribute(typeof(InfoAttribute), true) as InfoAttribute;
					if (info == null) continue;

					if (requires.Any(x => x.Name == info.Title)) continue;

					var description = type.GetCustomAttribute(typeof(DescriptionAttribute), true) as DescriptionAttribute;
					var plugin = Script.Create(Source, assembly, type);

					plugin.Name = info.Title;
					plugin.Author = info.Author;
					plugin.Version = info.Version;
					plugin.Description = description?.Description;

					plugin.Instance = Activator.CreateInstance(type) as RustPlugin;
					{
						plugin.Instance.Hooks = AsyncLoader.Hooks[type];
						plugin.Instance.HookMethods = AsyncLoader.HookMethods[type];
						plugin.Instance.PluginReferences = AsyncLoader.PluginReferences[type];

					}
					plugin.IsCore = IsCore;
					plugin.Instance.Requires = requiresResult;
					plugin.Instance.SetProcessor(CarbonCore.Instance.ScriptProcessor);
					plugin.Instance.CompileTime = AsyncLoader.CompileTime;

					plugin.Instance.FilePath = AsyncLoader.FilePath;
					plugin.Instance.FileName = AsyncLoader.FileName;

					plugin.Instance.CallHook("SetupMod", null, info.Title, info.Author, info.Version, plugin.Description);
					HookExecutor.CallStaticHook("OnPluginLoaded", plugin);
					plugin.Instance.IInit();
					try { plugin.Instance.DoLoadConfig(); } catch (Exception loadException) { plugin.Instance.LogError($"Failed loading config.", loadException); }
					plugin.Instance.Load();
					plugin.Loader = this;

					CarbonLoader.AppendAssembly(plugin.Name, AsyncLoader.Assembly);
					CarbonCore.Instance.Plugins.Plugins.Add(plugin.Instance);

					if (info != null)
					{
						Carbon.Logger.Log(
							$"Loaded plugin {info.Title} v{info.Version} by {info.Author}"
						);
					}

					CarbonLoader.ProcessCommands(type, plugin.Instance);

					Scripts.Add(plugin);

					Report.OnPluginCompiled?.Invoke(plugin.Instance, AsyncLoader.UnsupportedHooks[type]);
				}
				catch (Exception exception)
				{
					HasFinished = true;
					Carbon.Logger.Error($"Failed to compile: ", exception);
				}

				yield return _serverExhale;
			}

			foreach (var uhList in AsyncLoader.UnsupportedHooks)
			{
				uhList.Value.Clear();
			}

			foreach (var plugin in CarbonCore.Instance.Plugins.Plugins)
			{
				plugin.InternalApplyPluginReferences();
			}

			AsyncLoader.Hooks.Clear();
			AsyncLoader.UnsupportedHooks.Clear();
			AsyncLoader.HookMethods.Clear();
			AsyncLoader.PluginReferences.Clear();

			AsyncLoader.Hooks = null;
			AsyncLoader.UnsupportedHooks = null;
			AsyncLoader.HookMethods = null;
			AsyncLoader.PluginReferences = null;

			HasFinished = true;

			if (CarbonCore.Instance.ScriptProcessor.AllPendingScriptsComplete())
			{
				OnFinished();
			}

			Pool.FreeList(ref requires);
			yield return null;
		}

		public static void OnFinished()
		{
			if (CarbonCore.IsServerFullyInitialized)
			{
				var counter = 0;
				foreach (var plugin in CarbonCore.Instance.Plugins.Plugins)
				{
					if (plugin.HasInitialized) continue;
					counter++;

					try
					{
						plugin.CallHook("OnServerInitialized");
						plugin.CallHook("OnServerInitialized", CarbonCore.IsServerFullyInitialized);
					}
					catch (Exception initException)
					{
						plugin.LogError($"Failed OnServerInitialized.", initException);
					}

					plugin.HasInitialized = true;
				}

				foreach (var plugin in CarbonCore.Instance.ModuleProcessor.Modules)
				{
					if (plugin.HasInitialized) continue;

					try
					{
						HookExecutor.CallHook(plugin, "OnServerInitialized");
						HookExecutor.CallHook(plugin, "OnServerInitialized", CarbonCore.IsServerFullyInitialized);
					}
					catch (Exception initException)
					{
						Logger.Error($"[{plugin.Name}] Failed OnServerInitialized.", initException);
					}

					plugin.HasInitialized = true;
				}

				if (counter > 1) Carbon.Logger.Log($" Batch completed! OSI on {counter:n0} {counter.Plural("plugin", "plugins")}.");

				Report.OnProcessEnded?.Invoke();
			}
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
