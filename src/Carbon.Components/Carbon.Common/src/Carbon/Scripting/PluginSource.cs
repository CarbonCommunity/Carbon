using System.Collections.Concurrent;
using Facepunch;

namespace Carbon.Core;

/// <summary>
/// A place plugins are loaded from (loose .cs files, .cszip archives, dev folders..).
/// Tracks one <see cref="Entry"/> per loadable unit and keeps them in sync with the file system.
/// Subclass and register it through <see cref="PluginSources.Register"/> to support custom plugin formats.
/// </summary>
public abstract class PluginSource : IDisposable
{
	public abstract string Name { get; }
	public abstract string Folder { get; }

	/// <summary>File extension watched for changes, including the dot.</summary>
	public abstract string Extension { get; }

	/// <summary>Package newly loaded plugins get registered under.</summary>
	public virtual ModLoader.Package Package => Community.Runtime.Plugins;

	public virtual bool WatcherEnabled => true;
	public virtual float Rate => 0.2f;
	public virtual bool BypassFileNameChecks => false;

	/// <summary>Path fragments that are never picked up.</summary>
	public string[] Blacklist { get; set; } = ["backups", "debug"];

	public Dictionary<string, Entry> Entries { get; } = new();
	public List<string> IgnoreList { get; } = new();

	public bool IncludeSubdirectories
	{
		get;
		set
		{
			field = value;
			_watcher?.IncludeSubdirectories = value;
		}
	}

	private FileSystemWatcher _watcher;
	private string _normalizedFolder;
	private readonly ConcurrentQueue<string> _events = new();
	private readonly HashSet<string> _changeSet = new();
	private readonly List<string> _changes = new(32);
	private readonly List<string> _pending = new(16);
	private readonly List<string> _drained = new(16);
	private readonly List<Entry> _runtimeCache = new(32);

	#region Overridables

	/// <summary>Unique key of the unit a path belongs to. Defaults to the file name without extension.</summary>
	public virtual string GetKey(string path) => Path.GetFileNameWithoutExtension(path);

	/// <summary>Maps a raw file system event path to the path of its loadable unit.</summary>
	protected virtual string ResolvePath(string eventPath) => eventPath;

	protected virtual bool Exists(string path) => OsEx.File.Exists(path);

	/// <summary>Every unit currently present on disk, used for full (re)loads.</summary>
	public virtual IEnumerable<string> Discover()
	{
		var option = IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
		return OsEx.Folder.GetFilesWithExtension(Folder, Extension.TrimStart('.'), option);
	}

	/// <summary>Adds all source files of <paramref name="entry"/> to the loader.</summary>
	protected abstract void Collect(Entry entry, ScriptLoader loader);

	#endregion

	#region Lifecycle

	public virtual void Start()
	{
		Stop();

		_normalizedFolder = PathEx.NormalizePath(Folder);

		if (string.IsNullOrEmpty(Extension) || string.IsNullOrEmpty(Folder))
		{
			return;
		}

		OsEx.Folder.Create(Folder);

		_watcher = new FileSystemWatcher(Folder)
		{
			NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
#if WIN
				| NotifyFilters.LastAccess
#endif
			,
			Filter = $"*{Extension}",
			IncludeSubdirectories = IncludeSubdirectories,
			InternalBufferSize = 65536
		};
		_watcher.Created += OnFileEvent;
		_watcher.Changed += OnFileEvent;
		_watcher.Deleted += OnFileEvent;
		_watcher.Renamed += OnFileRenamed;
		_watcher.Error += OnWatcherError;
		_watcher.EnableRaisingEvents = true;
	}

	public virtual void Stop()
	{
		if (_watcher == null)
		{
			return;
		}

		_watcher.EnableRaisingEvents = false;
		_watcher.Created -= OnFileEvent;
		_watcher.Changed -= OnFileEvent;
		_watcher.Deleted -= OnFileEvent;
		_watcher.Renamed -= OnFileRenamed;
		_watcher.Error -= OnWatcherError;
		_watcher.Dispose();
		_watcher = null;
	}

	public virtual void Dispose()
	{
		Stop();
		Clear();
	}

	// Watcher callbacks run on a worker thread, only queue here
	private void OnFileEvent(object sender, FileSystemEventArgs e) => _events.Enqueue(e.FullPath);
	private void OnFileRenamed(object sender, RenamedEventArgs e)
	{
		_events.Enqueue(e.OldFullPath);
		_events.Enqueue(e.FullPath);
	}
	private void OnWatcherError(object sender, ErrorEventArgs e)
	{
		var ex = e.GetException();
		Logger.Error($"[{Name}] File watcher error in '{Folder}': {ex?.Message}", ex);
	}

	/// <summary>
	/// Main-thread tick: applies file changes and (re)compiles or unloads dirty entries.
	/// Yields between heavy entries so a mass reload doesn't stall a single frame.
	/// </summary>
	public IEnumerator Tick()
	{
		DrainEvents();

		foreach (var entry in Entries.Values)
		{
			if (entry.IsRemoved || entry.IsDirty)
			{
				_runtimeCache.Add(entry);
			}
		}

		foreach (var entry in _runtimeCache)
		{
			var yieldAfter = false;

			try
			{
				if (entry.IsRemoved)
				{
					Remove(entry.Key);
					yieldAfter = true;
				}
				else if (entry.IsDirty)
				{
					Prepare(entry.Key, entry.File);
					yieldAfter = true;
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"[{Name}] Failed processing '{entry.Key}'", ex);
			}

			if (yieldAfter)
			{
				yield return null;
			}
		}

		_runtimeCache.Clear();

		ProcessPending();
	}

	#endregion

	#region Entries

	public Entry Get(string key) => key != null && Entries.TryGetValue(key, out var entry) ? entry : null;

	public bool Exists(string path, bool byFile)
	{
		if (!byFile)
		{
			return Exists(path);
		}

		foreach (var entry in Entries.Values)
		{
			if (entry.File == path) return true;
		}
		return false;
	}

	/// <summary>Queues a unit for compilation without starting it (used by batch loads).</summary>
	public Entry Queue(string file)
	{
		var key = GetKey(file);
		if (string.IsNullOrEmpty(key) || Entries.ContainsKey(key))
		{
			return null;
		}

		var entry = new Entry(this, key, file);
		Entries.Add(key, entry);
		entry.MarkDirty();
		return entry;
	}

	/// <summary>(Re)compiles the unit at <paramref name="file"/> right away.</summary>
	public void Prepare(string file)
	{
		if (file.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
		{
			Prepare(Path.GetFileName(file.Substring(8)), file);
		}
		else if (file.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
		{
			Prepare(Path.GetFileName(file.Substring(7)), file);
		}
		else
		{
			Prepare(GetKey(file), file);
		}
	}

	public virtual void Prepare(string key, string file)
	{
		if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(file) || IgnoreList.Contains(file))
		{
			return;
		}

		if (!string.IsNullOrEmpty(Extension) && OsEx.File.Exists(file) && !PathEx.HasExtension(file, Extension))
		{
			return;
		}

		Remove(key);

		var entry = new Entry(this, key, file);
		Entries.Add(key, entry);
		entry.Execute();
	}

	/// <summary>Unloads a unit and forgets about it.</summary>
	public virtual void Remove(string key)
	{
		_pending.RemoveAll(x => GetKey(x) == key);

		if (Entries.TryGetValue(key, out var existent))
		{
			existent.Unload();
			Entries.Remove(key);
		}
	}

	/// <summary>Unloads everything, except units whose file matches any of <paramref name="except"/>.</summary>
	public virtual void Clear(IEnumerable<string> except = null)
	{
		var exceptList = except?.ToList();
		if (exceptList is { Count: 0 }) exceptList = null;

		if (exceptList == null)
		{
			_pending.Clear();
		}
		else
		{
			_pending.RemoveAll(x => !FileMatchesAny(x, exceptList));
		}

		var toRemove = Pool.Get<List<string>>();

		foreach (var entry in Entries.Values)
		{
			if (exceptList != null && FileMatchesAny(entry.File, exceptList))
			{
				continue;
			}

			try
			{
				entry.Unload();
			}
			catch (Exception ex)
			{
				Logger.Error($"[{Name}] Failed unloading '{entry.Key}'", ex);
			}

			toRemove.Add(entry.Key);
		}

		foreach (var key in toRemove)
		{
			Entries.Remove(key);
		}

		Pool.FreeUnmanaged(ref toRemove);
	}

	public void Ignore(string file)
	{
		if (!IgnoreList.Contains(file)) IgnoreList.Add(file);
	}
	public void ClearIgnore(string file)
	{
		IgnoreList.Remove(file);
	}

	public bool IsBlacklisted(string path)
	{
		if (!IncludeSubdirectories && !string.IsNullOrEmpty(_normalizedFolder))
		{
			var dir = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(dir) && !PathEx.Equals(PathEx.NormalizePath(dir), _normalizedFolder))
			{
				return true;
			}
		}

		if (Blacklist == null) return false;

		foreach (var pattern in Blacklist)
		{
			if (path.Contains(pattern)) return true;
		}

		return false;
	}

	public bool AllComplete(Func<ScriptLoader, bool> filter = null)
	{
		foreach (var entry in Entries.Values)
		{
			var loader = entry.Loader;
			if (loader != null && !loader.HasFinished && (filter == null || filter(loader)))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>Builds the compiler job for an entry. Override to customize loaders.</summary>
	protected internal virtual ScriptLoader CreateLoader(Entry entry)
	{
		var loader = new ScriptLoader
		{
			Package = Package,
			Entry = entry,
			BypassFileNameChecks = BypassFileNameChecks
		};

		Collect(entry, loader);
		return loader;
	}

	#endregion

	#region File changes

	private void DrainEvents()
	{
		while (_events.TryDequeue(out var path))
		{
			if (!WatcherEnabled || string.IsNullOrEmpty(path) || IsBlacklisted(path)) continue;
			if (!string.IsNullOrEmpty(Extension) && !PathEx.HasExtension(path, Extension)) continue;

			var sourcePath = ResolvePath(path);
			if (!string.IsNullOrEmpty(sourcePath) && _changeSet.Add(sourcePath))
			{
				_changes.Add(sourcePath);
			}
		}

		foreach (var sourcePath in _changes)
		{
			try
			{
				Reconcile(sourcePath);
			}
			catch (Exception ex)
			{
				Logger.Error($"[{Name}] Failed handling change of '{sourcePath}'", ex);
			}
		}

		_changes.Clear();
		_changeSet.Clear();
	}

	private void Reconcile(string sourcePath)
	{
		var key = GetKey(sourcePath);
		if (string.IsNullOrEmpty(key)) return;

		var exists = Exists(sourcePath);

		if (Entries.TryGetValue(key, out var entry))
		{
			if (PathEx.Equals(entry.File, sourcePath))
			{
				if (exists) entry.MarkDirty();
				else entry.MarkDeleted();
				return;
			}

			if (!exists) return;

			// The previous file got moved, follow it
			if (!Exists(entry.File))
			{
				entry.File = sourcePath;
				entry.MarkDirty();
				return;
			}

			WarnDuplicate(sourcePath, entry.File);
			return;
		}

		if (exists)
		{
			_pending.Add(sourcePath);
		}
	}

	private void ProcessPending()
	{
		if (_pending.Count == 0) return;

		_drained.AddRange(_pending);
		_pending.Clear();

		foreach (var sourcePath in _drained)
		{
			if (!Exists(sourcePath)) continue;

			try
			{
				var key = GetKey(sourcePath);

				if (Entries.TryGetValue(key, out var existing))
				{
					if (!PathEx.Equals(existing.File, sourcePath)) WarnDuplicate(sourcePath, existing.File);
					continue;
				}

				Prepare(key, sourcePath);
			}
			catch (Exception ex)
			{
				Logger.Error($"[{Name}] Failed loading '{sourcePath}'", ex);
			}
		}

		_drained.Clear();
	}

	private static void WarnDuplicate(string sourcePath, string existingFile)
	{
		Logger.Warn($"Skipping '{sourcePath}': '{existingFile}' is already loaded under the same name.");
	}

	private static bool FileMatchesAny(string file, List<string> patterns)
	{
		return file != null && patterns.Any(file.Contains);
	}

	#endregion

	/// <summary>
	/// A single loadable unit, and the compiler job currently working on it.
	/// </summary>
	public class Entry
	{
		public PluginSource Source { get; }
		public string Key { get; }
		public string File { get; set; }
		public ScriptLoader Loader { get; private set; }

		public bool IsDirty { get; private set; }
		public bool IsRemoved { get; private set; }

		public Entry(PluginSource source, string key, string file)
		{
			Source = source;
			Key = key;
			File = file;
		}

		public void MarkDirty()
		{
			IsRemoved = false;
			IsDirty = true;
		}
		public void MarkDeleted()
		{
			IsRemoved = true;
		}

		/// <summary>Kicks off compilation.</summary>
		public void Execute()
		{
			IsDirty = false;

			try
			{
				ModLoader.GetCompilationResult(File, true);
				Loader = Source.CreateLoader(this);
				Loader.Load();
			}
			catch (Exception ex)
			{
				Logger.Warn($"Failed processing {Path.GetFileNameWithoutExtension(File)}:\n{ex}");
			}
		}

		/// <summary>Recompiles in place. Loaded plugins get swapped once the new build initializes.</summary>
		public void Recompile()
		{
			Loader?.Dispose();
			Execute();
		}

		/// <summary>Unloads plugins compiled by this entry and stops any pending compilation.</summary>
		public void Unload()
		{
			try
			{
				Loader?.Clear();
			}
			catch (Exception ex)
			{
				Logger.Error($"Error unloading {File}", ex);
			}

			Loader = null;
		}
	}
}
