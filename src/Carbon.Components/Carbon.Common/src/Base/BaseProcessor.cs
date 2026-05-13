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
	public bool IncludeSubdirectories { get; set; }
	public FileSystemWatcher Watcher { get; private set; }

	internal WaitForSeconds _wfsInstance;
	internal Dictionary<string, IBaseProcessor.IProcess> _runtimeCache = new(128);
	internal string _normalizedFolder;

	private Func<Process> _processFactory;
	private readonly ConcurrentQueue<WatchFileEvent> _events = new();

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

			if (_runtimeCache.Count == 0)
			{
				yield return null;
				continue;
			}

			foreach (var element in _runtimeCache)
			{
				var value = element.Value;

				if (value == null)
				{
					var instance = CreateProcess();

					if (instance != null)
					{
						instance.File = element.Key;
						instance.Execute(this);

						var id = Path.GetFileNameWithoutExtension(element.Key);
						InstanceBuffer.Remove(element.Key);
						InstanceBuffer[id] = instance;
					}

					continue;
				}

				if (value.IsRemoved)
				{
					Clear(element.Key, value);
					yield return null;
					continue;
				}

				if (value.IsDirty)
				{
					Execute(element.Key, value);
					yield return null;
				}
			}

			_runtimeCache.Clear();

			yield return null;
		}
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
			Prepare(Path.GetFileNameWithoutExtension(file), file);
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

		Remove(id);

		var instance = CreateProcess();
		if (instance == null) return;

		InstanceBuffer.Add(id, instance);

		instance.File = file;
		instance.Execute(this);
	}
	public virtual void Remove(string id)
	{
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
		if (!EnableWatcher || IsBlacklisted(e.Path)) return;

		if (InstanceBuffer.TryGetValue(e.Path, out var instance1))
		{
			instance1?.MarkDirty();
			return;
		}

		if (InstanceBuffer.TryGetValue(Path.GetFileNameWithoutExtension(e.Path), out var instance2))
		{
			instance2?.MarkDirty();
			return;
		}

		InstanceBuffer.Add(e.Path, null);
	}
	public virtual void OnChanged(WatchFileEvent e)
	{
		if (!EnableWatcher || IsBlacklisted(e.Path)) return;

		var name = Path.GetFileNameWithoutExtension(e.Path);

		if (InstanceBuffer.TryGetValue(name, out var mod))
		{
			mod.MarkDirty();
		}
	}
	public virtual void OnRenamed(WatchFileEvent e)
	{
		if (!EnableWatcher) return;

		if (!string.IsNullOrEmpty(e.OldPath))
		{
			var oldName = Path.GetFileNameWithoutExtension(e.OldPath);
			if (InstanceBuffer.TryGetValue(oldName, out var oldMod))
			{
				oldMod?.MarkDeleted();
			}
		}

		if (IsBlacklisted(e.Path)) return;

		var newName = Path.GetFileNameWithoutExtension(e.Path);
		InstanceBuffer[newName] = null;
	}
	public virtual void OnRemoved(WatchFileEvent e)
	{
		if (!EnableWatcher || IsBlacklisted(e.Path)) return;

		var name = Path.GetFileNameWithoutExtension(e.Path);

		if (InstanceBuffer.TryGetValue(name, out var mod))
		{
			mod.MarkDeleted();
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