using System.Collections.Concurrent;
using System.Linq.Expressions;
using API.Assembly;
using Facepunch;

namespace Carbon.Base;

public abstract class BaseProcessor : FacepunchBehaviour, IDisposable, IBaseProcessor
{
	public virtual string Name { get; }

	public Dictionary<string, IBaseProcessor.IProcess> InstanceBuffer { get; set; }
	public List<string> IgnoreList { get; set; }

	public virtual bool EnableWatcher => true;
	public virtual string Folder => string.Empty;
	public virtual string Extension => string.Empty;
	public string[] BlacklistPattern { get; set; }
	public virtual float Rate => 0.2f;
	public virtual Type IndexedType => null;

	public bool IncludeSubdirectories
	{
		get;
		set
		{
			field = value;
			Watcher?.IncludeSubdirectories = value;
		}
	}

	public FileSystemWatcher Watcher { get; private set; }

	internal WaitForSeconds _wfsInstance;
	internal readonly Dictionary<string, IBaseProcessor.IProcess> _runtimeCache = new(128);
	internal string _normalizedFolder;

	private Func<Process> _processFactory;
	private readonly ConcurrentQueue<WatchFileEvent> _events = new();
	private readonly List<string> _sourceChanges = new(32);
	private readonly HashSet<string> _sourceChangeSet = [];
	private readonly List<string> _pendingSources = new(16);
	private readonly List<string> _drainedSources = new(16);

	public bool IsInitialized { get; set; }

	public void Awake()
	{
		if (!Community.Runtime.Config.Logging.ReducedLogging)
		{
			Logger.Log($"- Installed {Name}");
		}
	}
	public virtual void Start()
	{
		if (IsInitialized) return;

		InstanceBuffer = new Dictionary<string, IBaseProcessor.IProcess>();
		IgnoreList = new List<string>();
		_sourceChanges.Clear();
		_sourceChangeSet.Clear();
		_pendingSources.Clear();
		_drainedSources.Clear();

		DontDestroyOnLoad(gameObject);

		IsInitialized = true;

		RefreshRate();

		StopAllCoroutines();
		StartCoroutine(Run());

		DisposeWatcher();

		_normalizedFolder = PathEx.NormalizePath(Folder);
		_processFactory = BuildProcessFactory(IndexedType);

		if (!string.IsNullOrEmpty(Extension) && !string.IsNullOrEmpty(Folder))
		{
			Watcher = new FileSystemWatcher(Folder)
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
			Watcher.Created += OnCreatedRaw;
			Watcher.Changed += OnChangedRaw;
			Watcher.Renamed += OnRenamedRaw;
			Watcher.Deleted += OnDeletedRaw;
			Watcher.Error += OnWatcherError;
			Watcher.EnableRaisingEvents = true;
		}

		if (!Community.Runtime.Config.Logging.ReducedLogging)
		{
			Logger.Log($" Initialized {(IndexedType?.Name ?? Name)} processor...");
		}
	}
	public virtual void OnDestroy()
	{
		DisposeWatcher();

		IsInitialized = false;

		Logger.Log($"{IndexedType?.Name} processor has been unloaded.");
	}

	private void DisposeWatcher()
	{
		if (Watcher == null) return;

		Watcher.EnableRaisingEvents = false;
		Watcher.Created -= OnCreatedRaw;
		Watcher.Changed -= OnChangedRaw;
		Watcher.Renamed -= OnRenamedRaw;
		Watcher.Deleted -= OnDeletedRaw;
		Watcher.Error -= OnWatcherError;
		Watcher.Dispose();
		Watcher = null;
	}
	public virtual void Dispose()
	{
		Clear();
	}

	private void OnCreatedRaw(object sender, FileSystemEventArgs e)
		=> _events.Enqueue(new WatchFileEvent(WatcherChangeTypes.Created, e.FullPath, null, isInitial: false));

	private void OnChangedRaw(object sender, FileSystemEventArgs e)
		=> _events.Enqueue(new WatchFileEvent(WatcherChangeTypes.Changed, e.FullPath, null, isInitial: false));

	private void OnRenamedRaw(object sender, RenamedEventArgs e)
		=> _events.Enqueue(new WatchFileEvent(WatcherChangeTypes.Renamed, e.FullPath, e.OldFullPath, isInitial: false));

	private void OnDeletedRaw(object sender, FileSystemEventArgs e)
		=> _events.Enqueue(new WatchFileEvent(WatcherChangeTypes.Deleted, e.FullPath, null, isInitial: false));

	private void OnWatcherError(object sender, ErrorEventArgs e)
	{
		var ex = e.GetException();
		Logger.Error($"FileSystemWatcher error in '{Folder}': {ex?.Message}", ex);
	}

	private static Func<Process> BuildProcessFactory(Type type)
	{
		if (type == null) return null;

		var ctor = type.GetConstructor(Type.EmptyTypes);
		if (ctor == null) return null;

		return Expression.Lambda<Func<Process>>(Expression.New(ctor)).Compile();
	}

	private Process CreateProcess()
	{
		if (_processFactory != null) return _processFactory();
		if (IndexedType == null) return null;
		return Activator.CreateInstance(IndexedType) as Process;
	}

	public virtual string GetInstanceKey(string sourcePath)
	{
		return Path.GetFileNameWithoutExtension(sourcePath);
	}
	protected virtual string GetSourcePath(string eventPath)
	{
		return eventPath;
	}
	protected virtual bool SourceExists(string sourcePath)
	{
		return OsEx.File.Exists(sourcePath);
	}

	private void DrainEventQueue()
	{
		while (_events.TryDequeue(out var evt))
		{
			try
			{
				switch (evt.Type)
				{
					case WatcherChangeTypes.Created: OnCreated(evt); break;
					case WatcherChangeTypes.Changed: OnChanged(evt); break;
					case WatcherChangeTypes.Renamed: OnRenamed(evt); break;
					case WatcherChangeTypes.Deleted: OnRemoved(evt); break;
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"Watcher dispatch error for '{evt.Path}' ({evt.Type})", ex);
			}
		}

		ReconcileSourceChanges();
	}

	public virtual IEnumerator Run()
	{
		while (true)
		{
			yield return _wfsInstance;

			DrainEventQueue();

			foreach (var element in InstanceBuffer)
			{
				var value = element.Value;
				if (value == null || value.IsRemoved || value.IsDirty)
				{
					_runtimeCache.Add(element.Key, value);
				}
			}

			foreach (var element in _runtimeCache)
			{
				var yieldAfter = false;

				try
				{
					yieldAfter = ProcessRuntimeEntry(element.Key, element.Value);
				}
				catch (Exception ex)
				{
					Logger.Error($"Processor run error for '{element.Key}'", ex);
				}

				if (yieldAfter)
				{
					yield return null;
				}
			}

			_runtimeCache.Clear();

			ProcessPendingSources();

			yield return null;
		}
	}

	private bool ProcessRuntimeEntry(string key, IBaseProcessor.IProcess value)
	{
		if (value == null)
		{
			InstanceBuffer.Remove(key);
			return false;
		}

		if (value.IsRemoved)
		{
			Clear(key, value);
			return true;
		}

		if (value.IsDirty)
		{
			Execute(key, value);
			return true;
		}

		return false;
	}

	public virtual bool Exists(string path)
	{
		foreach (var entry in InstanceBuffer)
		{
			if (entry.Value != null && entry.Value.File == path) return true;
		}
		return false;
	}
	public virtual void Prepare(string file)
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
			Prepare(GetInstanceKey(file), file);
		}
	}
	public virtual void Prepare(string id, string file)
	{
		if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(file) || IgnoreList.Contains(file))
		{
			return;
		}

		if (!string.IsNullOrEmpty(Extension) && OsEx.File.Exists(file) && !PathEx.HasExtension(file, Extension))
		{
			return;
		}

		InstallProcess(id, file);
	}
	private void InstallProcess(string id, string file)
	{
		Remove(id);

		var instance = CreateProcess();
		if (instance == null) return;

		InstanceBuffer.Add(id, instance);

		instance.File = file;
		instance.Execute(this);
	}
	public virtual void Remove(string id)
	{
		CancelPendingSource(id);

		if (InstanceBuffer.TryGetValue(id, out var existent))
		{
			existent?.Clear();
			existent?.Dispose();
			InstanceBuffer.Remove(id);
		}
	}
	public virtual void Clear(IEnumerable<string> except = null)
	{
		List<string> exceptList = null;
		if (except != null)
		{
			exceptList = Pool.Get<List<string>>();
			foreach (var s in except) exceptList.Add(s);
			if (exceptList.Count == 0)
			{
				Pool.FreeUnmanaged(ref exceptList);
				exceptList = null;
			}
		}

		if (exceptList == null)
		{
			_pendingSources.Clear();
		}
		else
		{
			for (int i = _pendingSources.Count - 1; i >= 0; i--)
			{
				if (!FileMatchesAny(_pendingSources[i], exceptList)) _pendingSources.RemoveAt(i);
			}
		}

		var toRemove = Pool.Get<List<string>>();

		foreach (var item in InstanceBuffer)
		{
			if (exceptList != null && FileMatchesAny(item.Value?.File, exceptList))
			{
				continue;
			}

			try
			{
				item.Value?.Clear();
				item.Value?.Dispose();
			}
			catch (Exception ex) { Logger.Error($" Processor error: '{item.Key}'", ex); }

			toRemove.Add(item.Key);
		}

		for (int i = 0; i < toRemove.Count; i++)
		{
			InstanceBuffer.Remove(toRemove[i]);
		}

		Pool.FreeUnmanaged(ref toRemove);
		if (exceptList != null) Pool.FreeUnmanaged(ref exceptList);
	}
	public virtual void Ignore(string file)
	{
		if (!IgnoreList.Contains(file)) IgnoreList.Add(file);
	}
	public virtual void ClearIgnore(string file)
	{
		IgnoreList.Remove(file);
	}
	public T Get<T>(string id) where T : IBaseProcessor.IProcess
	{
		if (InstanceBuffer.TryGetValue(id, out var instance))
		{
			return (T)instance;
		}

		return default;
	}

	public virtual void Clear(string id, IBaseProcessor.IProcess process)
	{
		process?.Clear();
		process?.Dispose();

		Remove(id);
	}
	public virtual void Execute(string id, IBaseProcessor.IProcess process)
	{
		Prepare(id, process.File);
	}

	public virtual void OnCreated(WatchFileEvent e)
	{
		if (!EnableWatcher) return;

		QueueSourceChange(e.Path);
	}
	public virtual void OnChanged(WatchFileEvent e)
	{
		if (!EnableWatcher) return;

		QueueSourceChange(e.Path);
	}
	public virtual void OnRenamed(WatchFileEvent e)
	{
		if (!EnableWatcher) return;

		QueueSourceChange(e.OldPath);
		QueueSourceChange(e.Path);
	}
	public virtual void OnRemoved(WatchFileEvent e)
	{
		if (!EnableWatcher) return;

		QueueSourceChange(e.Path);
	}

	private void QueueSourceChange(string path)
	{
		if (string.IsNullOrEmpty(path) || IsBlacklisted(path)) return;
		if (!string.IsNullOrEmpty(Extension) && !PathEx.HasExtension(path, Extension)) return;

		var sourcePath = GetSourcePath(path);
		if (string.IsNullOrEmpty(sourcePath) || !_sourceChangeSet.Add(sourcePath)) return;

		_sourceChanges.Add(sourcePath);
	}

	private void ReconcileSourceChanges()
	{
		for (int i = 0; i < _sourceChanges.Count; i++)
		{
			var sourcePath = _sourceChanges[i];

			try
			{
				ReconcileSource(sourcePath);
			}
			catch (Exception ex)
			{
				Logger.Error($"Watcher source error for '{sourcePath}'", ex);
			}
		}

		_sourceChanges.Clear();
		_sourceChangeSet.Clear();
	}

	private void ReconcileSource(string sourcePath)
	{
		var key = GetInstanceKey(sourcePath);
		if (string.IsNullOrEmpty(key)) return;

		var exists = SourceExists(sourcePath);

		if (InstanceBuffer.TryGetValue(key, out var process) && process != null)
		{
			if (PathEx.Equals(process.File, sourcePath))
			{
				if (exists) process.MarkDirty();
				else process.MarkDeleted();
				return;
			}

			if (!exists) return;

			if (!SourceExists(process.File))
			{
				process.File = sourcePath;
				process.MarkDirty();
				return;
			}

			WarnDuplicateSource(sourcePath, process.File);
			return;
		}

		if (exists)
		{
			_pendingSources.Add(sourcePath);
		}
	}

	private void ProcessPendingSources()
	{
		if (_pendingSources.Count == 0) return;

		_drainedSources.AddRange(_pendingSources);
		_pendingSources.Clear();

		for (int i = 0; i < _drainedSources.Count; i++)
		{
			var sourcePath = _drainedSources[i];
			if (!SourceExists(sourcePath)) continue;

			try
			{
				var key = GetInstanceKey(sourcePath);

				if (InstanceBuffer.TryGetValue(key, out var existing) && existing != null)
				{
					if (!PathEx.Equals(existing.File, sourcePath)) WarnDuplicateSource(sourcePath, existing.File);
					continue;
				}

				InstallProcess(key, sourcePath);
			}
			catch (Exception ex)
			{
				Logger.Error($"Processor run error for '{sourcePath}'", ex);
			}
		}

		_drainedSources.Clear();
	}

	private static void WarnDuplicateSource(string sourcePath, string existingFile)
	{
		Logger.Warn($"Skipping '{sourcePath}': '{existingFile}' is already loaded under the same name.");
	}

	private void CancelPendingSource(string key)
	{
		for (int i = _pendingSources.Count - 1; i >= 0; i--)
		{
			if (GetInstanceKey(_pendingSources[i]) == key)
			{
				_pendingSources.RemoveAt(i);
			}
		}
	}

	public void RefreshRate()
	{
		_wfsInstance = new WaitForSeconds(Rate);
	}

	public bool IsBlacklisted(string path)
	{
		if (!IncludeSubdirectories && !string.IsNullOrEmpty(_normalizedFolder))
		{
			var dir = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(dir))
			{
				var fullDir = PathEx.NormalizePath(dir);
				if (!PathEx.Equals(fullDir, _normalizedFolder))
				{
					return true;
				}
			}
		}

		if (BlacklistPattern == null) return false;

		for (int i = 0; i < BlacklistPattern.Length; i++)
		{
			if (path.Contains(BlacklistPattern[i])) return true;
		}

		return false;
	}

	private static bool FileMatchesAny(string file, List<string> patterns)
	{
		if (file == null) return false;
		for (int i = 0; i < patterns.Count; i++)
		{
			if (file.Contains(patterns[i])) return true;
		}
		return false;
	}

	public abstract class Process : IBaseProcessor.IProcess, IDisposable
	{
		public IBaseProcessor Processor { get; internal set; }
		public virtual IBaseProcessor.IParser Parser { get; }

		public string File { get; set; }

		internal bool _hasChanged;
		internal bool _hasRemoved;

		public abstract void Clear();
		public abstract void Dispose();
		public virtual void Execute(IBaseProcessor processor)
		{
			Processor = processor;
		}

		public bool HasSucceeded { get; set; }
		public bool IsDirty => _hasChanged;
		public bool IsRemoved => _hasRemoved;

		public void MarkDirty()
		{
			_hasRemoved = false;
			_hasChanged = true;
		}
		public void MarkDeleted()
		{
			_hasRemoved = true;
		}
	}
	public class Parser
	{
		public virtual void Process(string file, string input, out string output)
		{
			output = null;
		}
	}
}
