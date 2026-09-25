﻿using Carbon.Jobs;
using Facepunch;

namespace Carbon.Core;

/// <summary>
/// Compiles the sources of a single <see cref="PluginSource.Entry"/> and initializes the plugins inside.
/// Runs as a coroutine on <see cref="PluginSources"/>, the heavy lifting happens on <see cref="ScriptCompilationThread"/>.
/// </summary>
public class ScriptLoader : IDisposable
{
	public const int BusyFileAttempts = 10;

	public SourceFile InitialSource => Sources?.Count > 0 ? Sources[0] : null;

	public bool BypassFileNameChecks { get; set; }

	public List<Script> Scripts { get; set; } = [];
	public List<SourceFile> Sources { get; set; } = [];

	public bool IsCore { get; set; }
	public bool IsExtension { get; set; }

	public bool HasFinished { get; set; }
	public bool HasRequires { get; set; }

	/// <summary>The source entry that owns this job.</summary>
	public PluginSource.Entry Entry { get; set; }
	public ModLoader.Package Package { get; set; }
	public ScriptCompilationThread AsyncLoader { get; set; } = new();

	private IEnumerator _compileRoutine;

	public void Load()
	{
		if (InitialSource == null || string.IsNullOrEmpty(InitialSource.FilePath))
		{
			Clear();
			return;
		}

		try
		{
			var directory = Path.GetDirectoryName(InitialSource.FilePath);
			IsExtension = directory.EndsWith("extensions");

			_compileRoutine = Compile();
			Community.Runtime.PluginSources.StartCoroutine(_compileRoutine);
		}
		catch (Exception exception)
		{
			Logger.Error($"Failed loading script '{InitialSource.FilePath}':", exception);
		}
	}

	public void Clear()
	{
		if (Scripts != null)
		{
			for (int i = 0; i < Scripts.Count; i++)
			{
				var plugin = Scripts[i];

				if (plugin.IsCore || plugin.Instance == null)
				{
					continue;
				}

				plugin.Instance.Package.RemovePlugin(plugin.Instance);

				if (plugin.Instance.IsExtension)
				{
					ScriptCompilationThread._clearExtensionPlugin(plugin.Instance.FilePath);
				}

				try
				{
					ModLoader.UninitializePlugin(plugin.Instance);
				}
				catch (Exception ex)
				{
					Logger.Error($"Failed unloading '{plugin.Instance}'", ex);
				}
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
			var fileInfo = new FileInfo(filePath);
			var inUse = true;
			var success = true;
			var attempts = 0;

			while (inUse)
			{
				inUse = !RunFileUseChecks();

				if (!inUse)
				{
					break;
				}

				attempts++;
				await AsyncEx.WaitForSeconds(0.2f);

				if (attempts < BusyFileAttempts)
				{
					continue;
				}

				inUse = false;
				success = false;
				Logger.Warn($"Failed compiling '{InitialSource.ContextFileName}' due to it being in use.");
			}

			if (success && !inUse)
			{
				using var reader = new StreamReader(filePath, detectEncodingFromByteOrderMarks: true);
				return await reader.ReadToEndAsync();
			}
			else
			{
				return null;
			}

			bool RunFileUseChecks()
			{
				try
				{
					using var stream = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
					stream.Close();
					return true;
				}
				catch (IOException)
				{
					return false;
				}
			}
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
			yield return ReadFileAsync(InitialSource.FilePath, content =>
			{
				if(InitialSource == null || string.IsNullOrEmpty(content))
				{
					return;
				}

				InitialSource.Content = content;
			});
		}

		if (Sources == null || Sources.Count == 0)
		{
			HasFinished = true;
			yield break;
		}

		var lines = Sources.Where(x => !string.IsNullOrEmpty(x.Content)).SelectMany(x => x.Content.Split('\n'));
		var resultReferences = Pool.Get<List<string>>();
		var resultRequires = Pool.Get<List<string>>();

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
					}
				}
				catch { }
				try
				{
					if (line.StartsWith("// Requires:") || line.StartsWith("//Requires:"))
					{

						var @ref = $"{line.Replace("// Requires:", "").Replace("//Requires:", "")}".Trim();
						resultRequires.Add(@ref);
					}
				}
				catch { }
			}
		}

		yield return null;

		lines = null;

		if (AsyncLoader != null)
		{
			AsyncLoader.Sources = new List<SourceFile>(Sources);
			AsyncLoader.References = resultReferences?.ToArray();
			AsyncLoader.Requires = resultRequires?.ToArray();
			AsyncLoader.IsExtension = IsExtension;
		}
		Pool.FreeUnmanaged(ref resultReferences);
		Pool.FreeUnmanaged(ref resultRequires);

		if (AsyncLoader != null) HasRequires = AsyncLoader.Requires.Length > 0;

		yield return null;

		// Give everything we might depend on a chance to compile first
		while (HasRequires && !Community.Runtime.PluginSources.AllNonRequiresComplete && !IsExtension && !Community.Runtime.PluginSources.AllExtensionsComplete)
		{
			yield return null;
		}

		var requires = Pool.Get<List<Plugin>>();
		var missingRequires = Pool.Get<List<string>>();
		if (AsyncLoader != null)
		{
			foreach (var require in AsyncLoader.Requires)
			{
				var plugin = ModLoader.FindPlugin(require);
				if (plugin == null)
				{
					missingRequires.Add(require);
				}
				else
				{
					requires.Add(plugin);
				}
			}
		}

		yield return null;

		if (missingRequires.Count > 0)
		{
			foreach (var require in missingRequires)
			{
				Logger.Warn($" Couldn't find required plugin '{require}' for '{(!string.IsNullOrEmpty(InitialSource.ContextFilePath) ? Path.GetFileNameWithoutExtension(InitialSource.ContextFilePath) : "<unknown>")}', retrying..");
			}

			ModLoader.AddPostBatchFailedRequiree(InitialSource.ContextFilePath);
			HasFinished = true;
			Pool.FreeUnmanaged(ref requires);
			Pool.FreeUnmanaged(ref missingRequires);

			if (Community.Runtime.PluginSources.AllComplete)
			{
				ModLoader.IsBatchComplete = true;
			}
			yield break;
		}

		Pool.FreeUnmanaged(ref missingRequires);

		yield return null;

		var requiresResult = requires.ToArray();

		AsyncLoader?.Start();

		while (AsyncLoader != null && !AsyncLoader.IsDone)
		{
			yield return null;
		}

		if (AsyncLoader == null)
		{
			HasFinished = true;
			Pool.FreeUnmanaged(ref requires);
			yield break;
		}

		yield return null;

		// In compile-test mode the assembly is intentionally never loaded, so bail out before anything
		// would reflect over, instantiate or otherwise execute the compiled plugin.
		if (AsyncLoader != null && (AsyncLoader.IsCompileTestMode || AsyncLoader.Assembly == null))
		{
			if (AsyncLoader.Exceptions != null && AsyncLoader.Exceptions.Count > 0)
			{
				Logger.Error($"Failed compiling '{AsyncLoader.InitialSource.ContextFileName}':");
				for (int i = 0; i < AsyncLoader.Exceptions.Count; i++)
				{
					var error = AsyncLoader.Exceptions[i];
					var print = $"{error.Error.ErrorText} [{error.Error.ErrorNumber}]\n     ({error.Error.FileName} {error.Error.Column} line {error.Error.Line})";
					Logger.Error($"  {i + 1:n0}. {print}");
				}

				var compilationFailure = ModLoader.GetCompilationResult(InitialSource.ContextFilePath);
				compilationFailure.Clear();

				compilationFailure.RollbackType = ModLoader.GetRegisteredType(InitialSource.ContextFilePath);
				compilationFailure.AppendErrors(AsyncLoader.Exceptions.Select(x => new ModLoader.Trace
				{
					Message = x.Error.ErrorText,
					Number = x.Error.ErrorNumber,
					Column = x.Error.Column,
					Line = x.Error.Line
				}));

#if DEBUG
				compilationFailure.AppendWarnings(AsyncLoader.Warnings.Select(x => new ModLoader.Trace
				{
					Message = x.Error.ErrorText,
					Number = x.Error.ErrorNumber,
					Column = x.Error.Column,
					Line = x.Error.Line
				}));
#endif

				// OnCompilationFail
				HookCaller.CallStaticHook(2719094727, InitialSource.ContextFilePath, compilationFailure);

				if (Community.Runtime.Config.Compiler.UnloadOnFailure)
				{
					var rollbackTypeName = compilationFailure.GetRollbackTypeName();

					if (!string.IsNullOrEmpty(rollbackTypeName))
					{
						var existentPlugin = ModLoader.FindPlugin(rollbackTypeName);

						if (existentPlugin != null)
						{
							ModLoader.UninitializePlugin(existentPlugin);
						}
					}
				}

#if DEBUG
				if (Community.Runtime.Config.Compiler.GenerateInternalCallHookSourceOnFailure)
				{
					OsEx.File.Create(Path.Combine(Defines.GetScriptDebugFolder(), $"{Path.GetFileNameWithoutExtension(AsyncLoader.InitialSource.ContextFilePath)}.Internal.cs"), AsyncLoader.InternalCallHookSource);
				}
#endif
			}
			else if (AsyncLoader.IsCompileTestMode)
			{
				var name = AsyncLoader.InitialSource?.ContextFileName ?? "<unknown>";

				if (AsyncLoader.IsCompileSuccess)
				{
					var warnings = AsyncLoader.Warnings != null ? AsyncLoader.Warnings.Count : 0;

					Logger.Log($"Compilation of '{name}' complete [{AsyncLoader.CompileTime.TotalMilliseconds:0}ms] (compile-test mode)");
				}
				else
				{
					Logger.Error($"Compilation of '{name}' failed (compile-test mode)");
				}
			}

			AsyncLoader.Exceptions?.Clear();
			AsyncLoader.Warnings?.Clear();
			AsyncLoader.Exceptions = AsyncLoader.Warnings = null;
			HasFinished = true;
			Pool.FreeUnmanaged(ref requires);

			if (Community.Runtime.PluginSources.AllComplete)
			{
				ModLoader.OnPluginProcessFinished();
			}
			yield break;
		}

		if (AsyncLoader == null)
		{
			Pool.FreeUnmanaged(ref requires);
			yield break;
		}

		var assembly = AsyncLoader.Assembly;
		var firstPlugin = true;

		yield return null;

		foreach (var type in assembly.GetTypes())
		{
			try
			{
				if (string.IsNullOrEmpty(type.Namespace) || !ModLoader.PluginNamespaces.Contains(type.Namespace)) continue;

				if (type.GetCustomAttribute(typeof(InfoAttribute), true) is not InfoAttribute info) continue;

				if (!IsExtension && firstPlugin && !BypassFileNameChecks)
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

				if (ModLoader.InitializePlugin(type, out Plugin rustPlugin, Package, preInit: p =>
					{
						Scripts.Add(plugin);
						p.HasConditionals = Sources.Any(x => x.Content.Contains("#if "));
						p.IsExtension = IsExtension;
#if DEBUG
						p.CompileWarnings = AsyncLoader.Warnings.Select(x => new ModLoader.Trace
						{
							Message = x.Error.ErrorText,
							Number = x.Error.ErrorNumber,
							Column = x.Error.Column,
							Line = x.Error.Line
						}).ToArray();
#endif
						plugin.IsCore = IsCore;

						p.Hooks = AsyncLoader.Hooks[type];
						p.HookMethods = AsyncLoader.HookMethods[type];
						p.PluginReferences = AsyncLoader.PluginReferences[type];

						p.Requires = requiresResult;
						p.Source = Entry;
						p.CompileTime = AsyncLoader.CompileTime;
						p.InternalCallHookGenTime = AsyncLoader.InternalCallHookGenTime;
						p.InternalCallHookSource = AsyncLoader.InternalCallHookSource;

						p.FilePath = AsyncLoader.InitialSource.ContextFilePath;
						p.FileName = AsyncLoader.InitialSource.ContextFileName;
					}))
				{
					plugin.Instance = rustPlugin;

					var arg = Pool.Get<CarbonEventArgs>();
					arg.Init(rustPlugin);
					Community.Runtime.Events.Trigger(CarbonEvent.PluginPreload, arg);
					Pool.Free(ref arg);

					ModLoader.RegisterType(AsyncLoader.InitialSource.ContextFilePath, type);

					Plugin.InternalApplyAllPluginReferences();

					// OnPluginLoaded
					HookCaller.CallStaticHook(3051933177, rustPlugin);
				}
			}
			catch (Exception exception)
			{
				HasFinished = true;
				if (InitialSource != null)
				{
					// OnPluginCompileFailure
					HookCaller.CallStaticHook(1298319061, !string.IsNullOrEmpty(InitialSource.ContextFilePath) ? Path.GetFileNameWithoutExtension(InitialSource.ContextFilePath) : "<unknown>", exception);
					Logger.Error($"Failed to compile '{(!string.IsNullOrEmpty(InitialSource.ContextFilePath) ? Path.GetFileNameWithoutExtension(InitialSource.ContextFilePath) : "<unknown>")}': ", exception);
				}
			}

			yield return null;
		}

		if (firstPlugin)
		{
			Logger.Error($"Invalid plugin format in '{AsyncLoader.InitialSource.ContextFileName}'. Namespace must be one of [{string.Join(", ", ModLoader.PluginNamespaces)}] and the class must inherit from one of [{string.Join(", ", ModLoader.PluginBaseTypes.Select(x => x.Name))}].");
		}

		AsyncLoader?.Dispose();

		HasFinished = true;

		if (Community.Runtime.PluginSources.AllComplete)
		{
			ModLoader.OnPluginProcessFinished();
		}

		Pool.FreeUnmanaged(ref requires);
		yield return null;
	}

	public void Dispose()
	{
		if (_compileRoutine != null)
		{
			Community.Runtime.PluginSources.StopCoroutine(_compileRoutine);
			_compileRoutine = null;
		}

		HasFinished = true;

		AsyncLoader?.Abort();
		AsyncLoader = null;

		if (Scripts != null)
		{
			foreach (var script in Scripts)
			{
				script.Dispose();
			}
		}

		Scripts?.Clear();
		Sources = null;
		Scripts = null;
	}

	[Serializable]
	public class Script : IDisposable
	{
		public Assembly Assembly { get; set; }
		public Type Type { get; set; }

		public string Name { get; set; }
		public string Author { get; set; }
		public VersionNumber Version { get; set; }
		public string Description { get; set; }
		public ScriptLoader Loader { get; set; }
		public Plugin Instance { get; set; }
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
