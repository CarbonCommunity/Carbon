// #define DISABLE_ASYNC_LOADING

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Carbon.Base;
using Carbon.Contracts;
using Carbon.Core;
using Carbon.Extensions;
using Carbon.Jobs;
using Facepunch;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Plugins;
using SharpCompress.Common;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Managers;

public class ScriptLoader : IScriptLoader
{
	public List<IScript> Scripts { get; set; } = new List<IScript>();

	public string File { get; set; }
	public string Source { get; set; }
	public bool IsCore { get; set; }
	public bool IsExtension { get; set; }

	public bool HasFinished { get; set; }
	public bool HasRequires { get; set; }

	public IBaseProcessor.IInstance Instance { get; set; }
	public Loader.CarbonMod Mod { get; set; }
	public IBaseProcessor.IParser Parser { get; set; }
	public ScriptCompilationThread AsyncLoader { get; set; } = new ScriptCompilationThread();

	internal const int ReaderBufferSize = 8 * 1024;

	public void Load()
	{
		try
		{
			var directory = Path.GetDirectoryName(File);
			IsExtension = directory.EndsWith("extensions");

			Community.Runtime.ScriptProcessor.StartCoroutine(Compile());
		}
		catch (Exception exception)
		{
			Logger.Error($"Failed loading script;", exception);
		}
	}

	public static void LoadAll()
	{
		var extensionPlugins = OsEx.Folder.GetFilesWithExtension(Defines.GetExtensionsFolder(), "cs");
		var plugins = OsEx.Folder.GetFilesWithExtension(Defines.GetScriptFolder(), "cs", option: SearchOption.TopDirectoryOnly);

		Community.Runtime.ScriptProcessor.Clear();

		foreach (var file in extensionPlugins)
		{
			var plugin = new ScriptProcessor.Script { File = file };
			Community.Runtime.ScriptProcessor.InstanceBuffer.Add(Path.GetFileNameWithoutExtension(file), plugin);
		}

		foreach (var file in plugins)
		{
			var plugin = new ScriptProcessor.Script { File = file };
			Community.Runtime.ScriptProcessor.InstanceBuffer.Add(Path.GetFileNameWithoutExtension(file), plugin);
		}

		foreach (var plugin in Community.Runtime.ScriptProcessor.InstanceBuffer)
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

			Community.Runtime.Plugins.Plugins.Remove(plugin.Instance);

			if (plugin.Instance.IsExtension) ScriptCompilationThread._clearExtensionPlugin(plugin.Instance.FilePath);

			if (plugin.Instance != null)
			{
				try
				{
					Loader.UninitializePlugin(plugin.Instance);
				}
				catch (Exception ex) { Logger.Error($"Failed unloading '{plugin.Instance}'", ex); }
			}

			plugin?.Dispose();
		}

		if (Scripts.Count > 0)
		{
			Scripts.RemoveAll(x => !x.IsCore);
		}

		Dispose();
	}

	IEnumerator ReadFileAsync(string filePath, Action<string> onRead)
	{
		var task = Task.Run(async () =>
		{
			using var reader = new StreamReader(filePath, Encoding.UTF8, true, ReaderBufferSize);
			return await reader.ReadToEndAsync();
		});

		while (!task.IsCompleted)
		{
			yield return null;
		}

		onRead?.Invoke(task.Result);
	}

	public IEnumerator Compile()
	{
		if (string.IsNullOrEmpty(Source) && !string.IsNullOrEmpty(File) && OsEx.File.Exists(File))
			yield return ReadFileAsync(File, content => Source = content);

		if (Parser != null)
		{
			Parser.Process(Source, out var newSource);

			yield return null;

			if (!string.IsNullOrEmpty(newSource))
			{
				Source = newSource;
			}
		}

		if (string.IsNullOrEmpty(Source))
		{
			HasFinished = true;
			Logger.Warn("Attempted to compile an empty string of source code.");
			yield break;
		}

		var lines = Source?.Split('\n');
		var resultReferences = Pool.GetList<string>();
		var resultRequires = Pool.GetList<string>();

		if (lines != null)
		{
			foreach (var line in lines)
			{
				try
				{
					if (line.StartsWith("// Reference:") || line.StartsWith("//Reference:"))
					{
						var @ref = $"{line.Replace("// Reference:", "").Replace("//Reference:", "")}".Trim();
						resultReferences.Add(@ref);
						Logger.Log($" Added reference: {@ref}");
					}
				}
				catch { }
				try
				{
					if (line.StartsWith("// Requires:") || line.StartsWith("//Requires:"))
					{

						var @ref = $"{line.Replace("// Requires:", "").Replace("//Requires:", "")}".Trim();
						resultRequires.Add(@ref);
						Logger.Log($" Added required plugin: {@ref}");
					}
				}
				catch { }
			}
		}

		yield return null;

		Array.Clear(lines, 0, lines.Length);
		if (AsyncLoader != null)
		{
			AsyncLoader.FilePath = File;
			AsyncLoader.Source = Source;
			AsyncLoader.References = resultReferences?.ToArray();
			AsyncLoader.Requires = resultRequires?.ToArray();
			AsyncLoader.IsExtension = IsExtension;
		}
		Pool.FreeList(ref resultReferences);
		Pool.FreeList(ref resultRequires);

		if (AsyncLoader != null) HasRequires = AsyncLoader.Requires.Length > 0;

		yield return null;

		while (HasRequires && !Community.Runtime.ScriptProcessor.AllNonRequiresScriptsComplete() && !IsExtension && !Community.Runtime.ScriptProcessor.AllExtensionsComplete())
		{
			yield return null;
		}

		var requires = Pool.GetList<Plugin>();
		var noRequiresFound = false;
		if (AsyncLoader != null)
		{
			foreach (var require in AsyncLoader.Requires)
			{
				var plugin = Community.Runtime.CorePlugin.plugins.Find(require);
				if (plugin == null)
				{
					Logger.Warn($"Couldn't find required plugin '{require}' for '{(!string.IsNullOrEmpty(File) ? Path.GetFileNameWithoutExtension(File) : "<unknown>")}'");
					noRequiresFound = true;
				}
				else requires.Add(plugin);
			}
		}

		yield return null;

		if (noRequiresFound)
		{
			Loader.PostBatchFailedRequirees.Add(File);
			HasFinished = true;
			Pool.FreeList(ref requires);
			yield break;
		}

		yield return null;

		if (AsyncLoader != null) Carbon.Components.Report.OnPluginAdded?.Invoke(AsyncLoader.FilePath);

		var requiresResult = requires.ToArray();

#if DISABLE_ASYNC_LOADING
		AsyncLoader.ThreadFunction();
		AsyncLoader.IsDone = true;
#else
		AsyncLoader?.Start();
#endif

		while (AsyncLoader != null && !AsyncLoader.IsDone)
		{
			yield return null;
		}

		if (AsyncLoader == null)
		{
			HasFinished = true;
			yield break;
		}

		yield return null;

		if (AsyncLoader.Assembly == null)
		{
			var errors = Pool.GetList<string>();
			Logger.Error($"Failed compiling '{AsyncLoader.FilePath}':");
			for (int i = 0; i < AsyncLoader.Exceptions.Count; i++)
			{
				var error = AsyncLoader.Exceptions[i];
				var print = $"{error.Error.ErrorText}\n     ({error.Error.FileName} {error.Error.Column} line {error.Error.Line})";
				Logger.Error($"  {i + 1:n0}. {print}");
				errors.Add(print);
			}

			Loader.FailedMods.Add(new Loader.FailedMod
			{
				File = File,
				Errors = AsyncLoader.Exceptions.Select(x => new Loader.FailedMod.Error
				{
					Message = x.Error.ErrorText,
					Number = x.Error.ErrorNumber,
					Column = x.Error.Column,
					Line = x.Error.Line
				}).ToArray(),
#if DEBUG
				Warnings = AsyncLoader.Warnings.Select(x => new Loader.FailedMod.Error
				{
					Message = x.Error.ErrorText,
					Number = x.Error.ErrorNumber,
					Column = x.Error.Column,
					Line = x.Error.Line
				}).ToArray()
#endif
			});

			Pool.FreeList(ref errors);
			AsyncLoader.Exceptions.Clear();
			AsyncLoader.Warnings.Clear();
			AsyncLoader.Exceptions = AsyncLoader.Warnings = null;
			HasFinished = true;

			if (Community.Runtime.ScriptProcessor.AllPendingScriptsComplete())
			{
				Loader.OnPluginProcessFinished();
			}
			yield break;
		}

		Logger.Debug($" Compiling '{(!string.IsNullOrEmpty(File) ? Path.GetFileNameWithoutExtension(File) : "<unknown>")}' took {AsyncLoader.CompileTime:0}ms...", 1);

		Loader.AssemblyCache.Add(AsyncLoader.Assembly);

		var assembly = AsyncLoader.Assembly;
		var firstPlugin = true;

		yield return null;

		foreach (var type in assembly.GetTypes())
		{
			try
			{
				if (string.IsNullOrEmpty(type.Namespace) ||
					!(type.Namespace.Equals("Oxide.Plugins") || type.Namespace.Equals("Carbon.Plugins"))) continue;

				if (Community.Runtime.Config.HookValidation)
				{
					var unsupportedHooksString = new StringBuilder();
					var counter = 0;
					foreach (var hook in AsyncLoader.UnsupportedHooks[type])
					{
						unsupportedHooksString.Append($"{hook}, ");
						counter++;
					}

					if (counter > 0)
					{
						Logger.Warn($"Plugin '{type.Name}' uses {counter:n0} hooks that are not supported: {unsupportedHooksString}and will not work as expected.");
					}

					unsupportedHooksString.Clear();
					unsupportedHooksString = null;
				}

				if (type.GetCustomAttribute(typeof(InfoAttribute), true) is not InfoAttribute info) continue;

				if (!IsExtension && firstPlugin && Community.Runtime.Config.FileNameCheck)
				{
					var name = Path.GetFileNameWithoutExtension(File).ToLower().Replace(" ", "").Replace(".", "").Replace("-", "");

					if (type.Name.ToLower().Replace(" ", "").Replace(".", "").Replace("-", "") != name)
					{
						Logger.Warn($"Plugin '{type.Name}' does not match with its file-name '{name}'.");
						break;
					}
				}

				firstPlugin = false;

				if (requires.Any(x => x.Name == info.Title)) continue;

				var description = type.GetCustomAttribute(typeof(DescriptionAttribute), true) as DescriptionAttribute;
				var plugin = Script.Create(Source, assembly, type);

				plugin.Name = info.Title;
				plugin.Author = info.Author;
				plugin.Version = info.Version;
				plugin.Description = description?.Description;

				if (Loader.InitializePlugin(type, out RustPlugin rustPlugin, Mod, preInit: p =>
					{
						p._processor_instance = Instance;

						p.Hooks = AsyncLoader.Hooks[type];
						p.HookMethods = AsyncLoader.HookMethods[type];
						p.PluginReferences = AsyncLoader.PluginReferences[type];

						p.Requires = requiresResult;
						p.SetProcessor(Community.Runtime.ScriptProcessor);
						p.CompileTime = AsyncLoader.CompileTime;

						p.FilePath = AsyncLoader.FilePath;
						p.FileName = AsyncLoader.FileName;
					}))
				{
					rustPlugin.HasConditionals = Source.Contains("#if ");
					rustPlugin.IsExtension = IsExtension;
#if DEBUG
					rustPlugin.CompileWarnings = AsyncLoader.Warnings.Select(x => new Loader.FailedMod.Error
					{
						Message = x.Error.ErrorText,
						Number = x.Error.ErrorNumber,
						Column = x.Error.Column,
						Line = x.Error.Line
					}).ToArray();
#endif

					plugin.Instance = rustPlugin;
					plugin.IsCore = IsCore;

					Loader.AppendAssembly(plugin.Name, AsyncLoader.Assembly);
					Scripts.Add(plugin);

					Carbon.Components.Report.OnPluginCompiled?.Invoke(plugin.Instance, AsyncLoader.UnsupportedHooks[type]);

					Plugin.InternalApplyAllPluginReferences();
					HookCaller.CallStaticHook("OnPluginLoaded", rustPlugin);
				}
			}
			catch (Exception exception)
			{
				HasFinished = true;
				Logger.Error($"Failed to compile '{(!string.IsNullOrEmpty(File) ? Path.GetFileNameWithoutExtension(File) : "<unknown>")}': ", exception);
			}

			yield return null;
		}

		foreach (var uhList in AsyncLoader.UnsupportedHooks)
		{
			uhList.Value.Clear();
		}

		AsyncLoader.Dispose();

		HasFinished = true;

		if (Community.Runtime.ScriptProcessor.AllPendingScriptsComplete())
		{
			Loader.OnPluginProcessFinished();
		}

		Pool.FreeList(ref requires);
		yield return null;
	}

	public void Dispose()
	{
		Community.Runtime.ScriptProcessor.StopCoroutine(Compile());
	}

	[Serializable]
	public class Script : IDisposable, IScript
	{
		public Assembly Assembly { get; set; }
		public Type Type { get; set; }

		public string Name { get; set; }
		public string Author { get; set; }
		public VersionNumber Version { get; set; }
		public string Description { get; set; }
		public string Source { get; set; }
		public IScriptLoader Loader { get; set; }
		public RustPlugin Instance { get; set; }
		public bool IsCore { get; set; }

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
