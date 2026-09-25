using System.IO.Compression;

namespace Carbon.Core;

/// <summary>
/// Hosts every <see cref="PluginSource"/> and drives their ticking and batch loading.
/// Replaces the old per-format script processors.
/// </summary>
public class PluginSources : FacepunchBehaviour
{
	public ScriptFileSource Scripts { get; private set; }
	public ZipFileSource Zips { get; private set; }
#if DEBUG
	public DevFolderSource DevFolders { get; private set; }
#endif

	public List<PluginSource> All { get; } = new();

	private readonly Dictionary<PluginSource, Coroutine> _routines = new();

	public void Init()
	{
		Register(Scripts = new ScriptFileSource());
		Register(Zips = new ZipFileSource());
#if DEBUG
		Register(DevFolders = new DevFolderSource());
#endif

		InvokeRepeating(nameof(RefreshConsoleInfo), 1f, 1f);
	}

	private void RefreshConsoleInfo() => Community.Runtime?.RefreshConsoleInfo();

	/// <summary>Adds and starts watching a custom plugin source.</summary>
	public void Register(PluginSource source)
	{
		if (All.Contains(source))
		{
			return;
		}

		All.Add(source);
		source.Start();
		_routines[source] = StartCoroutine(Run(source));

		if (!Community.Runtime.Config.Logging.ReducedLogging)
		{
			Logger.Log($" Initialized {source.Name} source");
		}
	}

	public void Unregister(PluginSource source)
	{
		if (!All.Remove(source))
		{
			return;
		}

		if (_routines.TryGetValue(source, out var routine))
		{
			StopCoroutine(routine);
			_routines.Remove(source);
		}

		source.Dispose();
	}

	private static IEnumerator Run(PluginSource source)
	{
		var rate = source.Rate;
		var wait = new WaitForSeconds(rate);

		while (true)
		{
			// Rates are configurable at runtime
			if (!Mathf.Approximately(rate, source.Rate))
			{
				rate = source.Rate;
				wait = new WaitForSeconds(rate);
			}

			yield return wait;
			yield return source.Tick();
		}
	}

	#region Queries

	/// <summary>Finds the entry that owns the file (or key) across all sources.</summary>
	public PluginSource.Entry Find(string keyOrFile)
	{
		foreach (var source in All)
		{
			var entry = source.Get(keyOrFile) ?? source.Get(source.GetKey(keyOrFile));
			if (entry != null) return entry;
		}

		return null;
	}

	public int Count => All.Sum(x => x.Entries.Count);

	/// <summary>No compilation is running in any source.</summary>
	public bool AllComplete => All.All(x => x.AllComplete());

	/// <summary>Every plugin without "// Requires:" has finished compiling.</summary>
	public bool AllNonRequiresComplete => All.All(x => x.AllComplete(loader => !loader.HasRequires));

	/// <summary>Every extension plugin has finished compiling.</summary>
	public bool AllExtensionsComplete => All.All(x => x.AllComplete(loader => loader.IsExtension));

	#endregion

	#region Batching

	/// <summary>Unloads and recompiles everything on disk, skipping paths that contain any of <paramref name="except"/>.</summary>
	public void LoadAll(IEnumerable<string> except = null)
	{
		var exceptList = except?.ToArray();
		var count = 0;

		foreach (var source in All)
		{
			source.Clear();

			foreach (var file in source.Discover())
			{
				if (source.IsBlacklisted(file) || (exceptList != null && exceptList.Any(file.Contains)))
				{
					continue;
				}

				if (source.Queue(file) != null)
				{
					count++;
				}
			}
		}

		if (count == 0)
		{
			ModLoader.IsBatchComplete = true;
			Community.Runtime.Events.Trigger(CarbonEvent.AllPluginsLoaded, EventArgs.Empty);
			Community.Runtime.Events.Trigger(CarbonEvent.AllPluginsInitialized, EventArgs.Empty);
		}
	}

	/// <summary>(Re)compiles a file in whichever source handles it.</summary>
	public void Prepare(string file)
	{
		foreach (var source in All)
		{
			source.ClearIgnore(file);
			source.Prepare(file);
		}
	}

	/// <summary>Retries a unit in every source, used after a required plugin shows up.</summary>
	public void Retry(string file)
	{
		var key = Path.GetFileNameWithoutExtension(file);

		foreach (var source in All)
		{
			source.ClearIgnore(key);
			source.Prepare(key, file);
		}
	}

	public void ClearAll()
	{
		foreach (var source in All)
		{
			source.Clear();
		}
	}

	#endregion

	public void Shutdown()
	{
		StopAllCoroutines();
		CancelInvoke();
		_routines.Clear();

		foreach (var source in All)
		{
			try
			{
				source.Dispose();
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed disposing {source.Name} source", ex);
			}
		}

		All.Clear();
	}
}

/// <summary>Loose .cs plugins (and extension plugins) in the plugins folder.</summary>
public class ScriptFileSource : PluginSource
{
	public override string Name => "Scripts";
	public override string Folder => Defines.GetScriptsFolder();
	public override string Extension => ".cs";
	public override bool WatcherEnabled => !Community.IsConfigReady || Community.Runtime.Config.Watchers.ScriptWatchers;
	public override float Rate => Community.Runtime.Config.Processors.ScriptProcessingRate;

	public ScriptFileSource()
	{
		Blacklist = ["backups", "debug", "cszip_dev"];
		IncludeSubdirectories = Community.Runtime.Config.Watchers.ScriptWatcherOption == SearchOption.AllDirectories;
	}

	public override IEnumerable<string> Discover()
	{
		return OsEx.Folder.GetFilesWithExtension(Defines.GetExtensionsFolder(), "cs").Concat(base.Discover());
	}

	protected override void Collect(Entry entry, ScriptLoader loader)
	{
		loader.Sources.Add(SourceFile.FromFile(entry.File));
	}
}

/// <summary>.cszip archives, where every entry of the archive is part of one plugin.</summary>
public class ZipFileSource : PluginSource
{
	public override string Name => "Zip Scripts";
	public override string Folder => Defines.GetScriptsFolder();
	public override string Extension => ".cszip";
	public override ModLoader.Package Package => Community.Runtime.ZipPlugins;
	public override bool WatcherEnabled => !Community.IsConfigReady || Community.Runtime.Config.Watchers.ZipScriptWatchers;
	public override float Rate => Community.Runtime.Config.Processors.ZipScriptProcessingRate;
	public override bool BypassFileNameChecks => true;

	public ZipFileSource()
	{
		IncludeSubdirectories = Community.Runtime.Config.Watchers.ScriptWatcherOption == SearchOption.AllDirectories;
	}

	protected override void Collect(Entry entry, ScriptLoader loader)
	{
		if (!OsEx.File.Exists(entry.File))
		{
			return;
		}

		using var zipFile = ZipFile.OpenRead(entry.File);

		foreach (var zipEntry in zipFile.Entries)
		{
			using var stream = new StreamReader(zipEntry.Open());

			loader.Sources.Add(new SourceFile
			{
				ContextFilePath = entry.File,
				ContextFileName = Path.GetFileName(entry.File),
				FilePath = zipEntry.FullName,
				FileName = zipEntry.Name,
				Content = stream.ReadToEnd()
			});
		}
	}
}

#if DEBUG
/// <summary>Unpacked .cszip plugins for development: every folder in cszip_dev is one plugin.</summary>
public class DevFolderSource : PluginSource
{
	private static readonly char[] DirectorySeparators = [Path.DirectorySeparatorChar];

	public override string Name => "Dev Folders";
	public override string Folder => Defines.GetZipDevFolder();
	public override string Extension => ".cs";
	public override ModLoader.Package Package => Community.Runtime.ZipPlugins;
	public override bool WatcherEnabled => !Community.IsConfigReady || Community.Runtime.Config.Watchers.ZipScriptWatchers;
	public override float Rate => Community.Runtime.Config.Processors.ZipScriptProcessingRate;
	public override bool BypassFileNameChecks => true;

	public DevFolderSource()
	{
		IncludeSubdirectories = true;
	}

	public override string GetKey(string path) => Path.GetFileName(path);
	protected override bool Exists(string path) => OsEx.Folder.Exists(path);

	// Any file change maps to the top-level folder it lives in
	protected override string ResolvePath(string eventPath)
	{
		var folder = Defines.GetZipDevFolder();
		return Path.Combine(folder, eventPath.Replace(folder, string.Empty).Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries)[0]);
	}

	public override IEnumerable<string> Discover()
	{
		return Directory.GetDirectories(Folder, "*", SearchOption.TopDirectoryOnly);
	}

	protected override void Collect(Entry entry, ScriptLoader loader)
	{
		if (!OsEx.Folder.Exists(entry.File))
		{
			return;
		}

		foreach (var file in OsEx.Folder.GetFilesWithExtension(entry.File, "cs"))
		{
			loader.Sources.Add(SourceFile.FromFile(file, entry.File, OsEx.File.ReadText(file)));
		}
	}
}
#endif
