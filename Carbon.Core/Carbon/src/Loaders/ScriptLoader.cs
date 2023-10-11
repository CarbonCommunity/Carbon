using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using API.Events;
using Carbon.Base;
using Carbon.Contracts;
using Carbon.Core;
using Carbon.Extensions;
using Carbon.Jobs;
using Facepunch;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Plugins;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community
 * All rights reserved.
 *
 */

namespace Carbon.Managers;

public class ScriptLoader : IScriptLoader
{
	public ISource InitialSource => Sources.Count > 0 ? Sources[0] : null;

	public List<IScript> Scripts { get; set; } = new();
	public List<ISource> Sources { get; set; } = new();

	public bool IsCore { get; set; }
	public bool IsExtension { get; set; }

	public bool HasFinished { get; set; }
	public bool HasRequires { get; set; }

	public IBaseProcessor.IProcess Process { get; set; }
	public ModLoader.ModPackage Mod { get; set; }
	public IBaseProcessor.IParser Parser { get; set; }
	public ScriptCompilationThread AsyncLoader { get; set; } = new ScriptCompilationThread();

	public void Load()
	{
		if (InitialSource == null)
		{
			Clear();
			return;
		}

		try
		{
			var directory = Path.GetDirectoryName(InitialSource.FilePath);
			IsExtension = directory.EndsWith("extensions");

			Community.Runtime.ScriptProcessor.StartCoroutine(Compile());
		}
		catch (Exception exception)
		{
			Logger.Error($"Failed loading script '{InitialSource.FilePath}':", exception);
		}
	}

	public static void LoadAll()
	{
		var config = Community.Runtime.Config;
		var extensionPlugins = OsEx.Folder.GetFilesWithExtension(Defines.GetExtensionsFolder(), "cs");
		var plugins = OsEx.Folder.GetFilesWithExtension(Defines.GetScriptFolder(), "cs", option: config.ScriptWatcherOption);
		var zipPlugins = OsEx.Folder.GetFilesWithExtension(Defines.GetScriptFolder(), "cszip", option: config.ScriptWatcherOption);
		var zipDevPlugins = OsEx.Folder.GetFilesWithExtension(Defines.GetZipDevFolder(), "cs", option: SearchOption.AllDirectories);
		ExecuteProcess(Community.Runtime.ScriptProcessor, false, extensionPlugins, plugins);
		ExecuteProcess(Community.Runtime.ZipScriptProcessor, false, zipPlugins);
		ExecuteProcess(Community.Runtime.ZipDevScriptProcessor, true, zipDevPlugins);

		void ExecuteProcess(IScriptProcessor processor, bool folderMode, params string[][] folders)
		{
			processor.Clear();

			foreach (var files in folders)
			{
				foreach (var file in files)
				{
					if (processor.IsBlacklisted(file)) continue;

					var folder = Path.GetDirectoryName(file);

					var id = folderMode ? folder : Path.GetFileNameWithoutExtension(file);
					if (processor.InstanceBuffer.ContainsKey(id)) continue;

					var plugin = new ScriptProcessor.Script { File = folderMode ? folder : file };
					processor.InstanceBuffer.Add(id, plugin);
				}
			}

			foreach (var plugin in processor.InstanceBuffer)
			{
				plugin.Value.SetDirty();
			}

			Array.Clear(folders, 0, folders.Length);
			folders = null;
		}
	}

	public void Clear()
	{
		AsyncLoader?.Abort();
		AsyncLoader = null;

		if (Scripts != null)
		{
			for (int i = 0; i < Scripts.Count; i++)
			{
				var plugin = Scripts[i];
				if (plugin.IsCore || plugin.Instance == null) continue;

				plugin.Instance.Package?.Plugins?.Remove(plugin.Instance);

				if (plugin.Instance.IsExtension) ScriptCompilationThread._clearExtensionPlugin(plugin.Instance.FilePath);

				try
				{
					ModLoader.UninitializePlugin(plugin.Instance);
				}
				catch (Exception ex) { Logger.Error($"Failed unloading '{plugin.Instance}'", ex); }

				plugin.Dispose();
			}

			if (Scripts.Count > 0)
			{
				Scripts.RemoveAll(x => !x.IsCore);
			}
		}

		Dispose();
	}

	IEnumerator ReadFileAsync(string filePath, Action<string> onRead)
	{
		var task = Task.Run(async () =>
		{
			using var reader = new StreamReader(filePath, Encoding.UTF8, true);
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
		if (string.IsNullOrEmpty(InitialSource.Content) && !string.IsNullOrEmpty(InitialSource.FilePath) && OsEx.File.Exists(InitialSource.FilePath))
		{
			yield return ReadFileAsync(InitialSource.FilePath, content => InitialSource.Content = content);
		}

		if (Parser != null)
		{
			for(int i = 0; i < Sources.Count; i++)
			{
				var source = Sources[i];
				Parser.Process(source.FilePath, source.Content, out var content);

				yield return null;

				if (!string.IsNullOrEmpty(content))
				{
					Sources[i] = new BaseSource
					{
						ContextFilePath = source.ContextFilePath,
						ContextFileName = source.ContextFileName,
						FilePath = source.FilePath,
						FileName = source.FileName,
						Content = content
					};
				}
			}
		}

		if (Sources == null || Sources.Count == 0)
		{
			HasFinished = true;
			// Logger.Warn("Attempted to compile an empty string of source code.");
			yield break;
		}

		var lines = Sources.Where(x => !string.IsNullOrEmpty(x.Content)).SelectMany(x => x.Content.Split('\n'));
		var resultReferences = Facepunch.Pool.GetList<string>();
		var resultRequires = Facepunch.Pool.GetList<string>();

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

		lines = null;

		if (AsyncLoader != null)
		{
			AsyncLoader.Sources = Sources;
			AsyncLoader.References = resultReferences?.ToArray();
			AsyncLoader.Requires = resultRequires?.ToArray();
			AsyncLoader.IsExtension = IsExtension;
		}
		Facepunch.Pool.FreeList(ref resultReferences);
		Facepunch.Pool.FreeList(ref resultRequires);

		if (AsyncLoader != null) HasRequires = AsyncLoader.Requires.Length > 0;

		yield return null;

		while (HasRequires && !Community.Runtime.ScriptProcessor.AllNonRequiresScriptsComplete() && !IsExtension && !Community.Runtime.ScriptProcessor.AllExtensionsComplete())
		{
			yield return null;
		}

		var requires = Facepunch.Pool.GetList<Plugin>();
		var noRequiresFound = false;
		if (AsyncLoader != null)
		{
			foreach (var require in AsyncLoader.Requires)
			{
				var plugin = Community.Runtime.CorePlugin.plugins.Find(require);
				if (plugin == null)
				{
					Logger.Warn($"Couldn't find required plugin '{require}' for '{(!string.IsNullOrEmpty(InitialSource.ContextFilePath) ? Path.GetFileNameWithoutExtension(InitialSource.ContextFilePath) : "<unknown>")}'");
					noRequiresFound = true;
				}
				else requires.Add(plugin);
			}
		}

		yield return null;

		if (noRequiresFound)
		{
			ModLoader.PostBatchFailedRequirees.Add(InitialSource.ContextFilePath);
			HasFinished = true;
			Facepunch.Pool.FreeList(ref requires);
			yield break;
		}

		yield return null;

		if (AsyncLoader != null) Carbon.Components.Report.OnPluginAdded?.Invoke(InitialSource.ContextFilePath);

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
			if (AsyncLoader.Exceptions.Count > 0)
			{
				Logger.Error($"Failed compiling '{AsyncLoader.InitialSource.ContextFilePath}':");
				for (int i = 0; i < AsyncLoader.Exceptions.Count; i++)
				{
					var error = AsyncLoader.Exceptions[i];
					var print = $"{error.Error.ErrorText} [{error.Error.ErrorNumber}]\n     ({error.Error.FileName} {error.Error.Column} line {error.Error.Line})";
					Logger.Error($"  {i + 1:n0}. {print}");
				}

				ModLoader.FailedMods.Add(new ModLoader.FailedMod
				{
					File = InitialSource.ContextFilePath,
					Errors = AsyncLoader.Exceptions.Select(x => new ModLoader.FailedMod.Error
					{
						Message = x.Error.ErrorText,
						Number = x.Error.ErrorNumber,
						Column = x.Error.Column,
						Line = x.Error.Line
					}).ToArray(),
#if DEBUG
					Warnings = AsyncLoader.Warnings.Select(x => new ModLoader.FailedMod.Error
					{
						Message = x.Error.ErrorText,
						Number = x.Error.ErrorNumber,
						Column = x.Error.Column,
						Line = x.Error.Line
					}).ToArray()
#endif
				});
			}

			AsyncLoader.Exceptions.Clear();
			AsyncLoader.Warnings.Clear();
			AsyncLoader.Exceptions = AsyncLoader.Warnings = null;
			HasFinished = true;

			if (Community.Runtime.ScriptProcessor.AllPendingScriptsComplete())
			{
				ModLoader.OnPluginProcessFinished();
			}
			yield break;
		}

		Logger.Debug($" Compiling '{(!string.IsNullOrEmpty(InitialSource.FilePath) ? Path.GetFileNameWithoutExtension(InitialSource.FilePath) : "<unknown>")}' took {AsyncLoader.CompileTime:0}ms...", 1);

		ModLoader.AssemblyCache.Add(AsyncLoader.Assembly);

		var assembly = AsyncLoader.Assembly;
		var firstPlugin = true;

		yield return null;

		foreach (var type in assembly.GetTypes())
		{
			try
			{
				if (string.IsNullOrEmpty(type.Namespace) ||
					!(type.Namespace.Equals("Oxide.Plugins") || type.Namespace.Equals("Carbon.Plugins"))) continue;

				if (type.GetCustomAttribute(typeof(InfoAttribute), true) is not InfoAttribute info) continue;

				if (!IsExtension && firstPlugin && Community.Runtime.Config.FileNameCheck)
				{
					var name = Path.GetFileNameWithoutExtension(InitialSource.FilePath).ToLower().Replace(" ", "").Replace(".", "").Replace("-", "");

					if (type.Name.ToLower().Replace(" ", "").Replace(".", "").Replace("-", "") != name)
					{
						Logger.Warn($"Plugin '{type.Name}' does not match with its file-name '{name}'.");
						break;
					}
				}

				firstPlugin = false;

				if (requires.Any(x => x.Name == info.Title)) continue;

				var description = type.GetCustomAttribute(typeof(DescriptionAttribute), true) as DescriptionAttribute;
				var plugin = Script.Create(assembly, type);

				plugin.Name = info.Title;
				plugin.Author = info.Author;
				plugin.Version = info.Version;
				plugin.Description = description?.Description;

				if (ModLoader.InitializePlugin(type, out RustPlugin rustPlugin, Mod, preInit: p =>
					{
						Scripts.Add(plugin);
						p.HasConditionals = Sources.Any(x => x.Content.Contains("#if "));
						p.IsExtension = IsExtension;
#if DEBUG
						p.CompileWarnings = AsyncLoader.Warnings.Select(x => new ModLoader.FailedMod.Error
						{
							Message = x.Error.ErrorText,
							Number = x.Error.ErrorNumber,
							Column = x.Error.Column,
							Line = x.Error.Line
						}).ToArray();
#endif

						p.ProcessorProcess = Process;
						plugin.IsCore = IsCore;

						p.Hooks = AsyncLoader.Hooks[type];
						p.HookMethods = AsyncLoader.HookMethods[type];
						p.PluginReferences = AsyncLoader.PluginReferences[type];

						p.Requires = requiresResult;
						p.SetProcessor(Community.Runtime.ScriptProcessor);
						p.CompileTime = AsyncLoader.CompileTime;

						p.FilePath = AsyncLoader.InitialSource.ContextFilePath;
						p.FileName = AsyncLoader.InitialSource.ContextFileName;
					}))
				{
					plugin.Instance = rustPlugin;

					Community.Runtime.Events.Trigger(CarbonEvent.PluginPreload, new CarbonEventArgs(rustPlugin));

					ModLoader.AppendAssembly(plugin.Name, AsyncLoader.Assembly);

					Carbon.Components.Report.OnPluginCompiled?.Invoke(plugin.Instance);

					Plugin.InternalApplyAllPluginReferences();

					// OnPluginLoaded
					HookCaller.CallStaticHook(4143864509, rustPlugin);
				}
			}
			catch (Exception exception)
			{
				HasFinished = true;
				Logger.Error($"Failed to compile '{(!string.IsNullOrEmpty(InitialSource.ContextFilePath) ? Path.GetFileNameWithoutExtension(InitialSource.ContextFilePath) : "<unknown>")}': ", exception);
			}

			yield return null;
		}

		AsyncLoader.Dispose();

		HasFinished = true;

		if (Community.Runtime.ScriptProcessor.AllPendingScriptsComplete())
		{
			ModLoader.OnPluginProcessFinished();
		}

		Facepunch.Pool.FreeList(ref requires);
		yield return null;
	}

	public void Dispose()
	{
		HasFinished = true;

		foreach (var script in Scripts)
		{
			script.Dispose();
		}

		foreach (var source in Sources)
		{
			source.Dispose();
		}

		Sources.Clear();
		Sources = null;
		Scripts.Clear();
		Scripts = null;

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
		public IScriptLoader Loader { get; set; }
		public RustPlugin Instance { get; set; }
		public bool IsCore { get; set; }

		public static Script Create(Assembly assembly, Type type)
		{
			return new Script
			{
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

			Name = null;
			Author = null;
			Version = default;
			Description = null;
			Loader = null;
			Instance = null;
			IsCore = default;
		}

		public override string ToString()
		{
			return $"{Name} v{Version}";
		}
	}
}
