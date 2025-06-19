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
	internal Dictionary<string, IBaseProcessor.IProcess> _runtimeCache = new(1000);

	public bool IsInitialized { get; set; }

	public void Awake()
	{
		Logger.Log($"- Installed {Name}");
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

		Watcher?.Dispose();
		Watcher = null;

		if (!string.IsNullOrEmpty(Extension) && !string.IsNullOrEmpty(Folder))
		{
			Watcher = new FileSystemWatcher(Folder)
			{
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
				#if WIN
				| NotifyFilters.LastAccess
				#endif
				,
				Filter = $"*{Extension}"
			};
			Watcher.Created += OnCreated;
			Watcher.Changed += OnChanged;
			Watcher.Renamed += OnRenamed;
			Watcher.Deleted += OnRemoved;
			Watcher.IncludeSubdirectories = true;
			Watcher.EnableRaisingEvents = true;
		}

		Logger.Log($" Initialized {(IndexedType?.Name ?? Name)} processor...");
	}
	public virtual void OnDestroy()
	{
		IsInitialized = false;

		Logger.Log($"{IndexedType?.Name} processor has been unloaded.");
	}
	public virtual void Dispose()
	{
		Clear();
	}

	public virtual IEnumerator Run()
	{
		while (true)
		{
			yield return _wfsInstance;

			foreach (var element in InstanceBuffer)
			{
				_runtimeCache.Add(element.Key, element.Value);
			}

			foreach (var element in _runtimeCache)
			{
				var id = Path.GetFileNameWithoutExtension(element.Key);

				if (element.Value == null)
				{
					var instance = Activator.CreateInstance(IndexedType) as Process;

					if (instance != null)
					{
						instance.File = element.Key;
						instance.Execute(this);

						InstanceBuffer.Remove(element.Key);
						InstanceBuffer[id] = instance;
					}

					continue;
				}

				if (element.Value.IsRemoved)
				{
					Clear(id, element.Value);
					yield return null;
					continue;
				}

				if (element.Value.IsDirty)
				{
					Execute(element.Key, element.Value);
					yield return null;
				}
			}

			_runtimeCache.Clear();

			yield return null;
		}
	}

	public virtual bool Exists(string path)
	{
		return InstanceBuffer.Any(x => x.Value.File == path);
	}
	public virtual void Prepare(string file)
	{
		if (file.StartsWith("http"))
		{
			var filename = Path.GetFileName(file.Replace("http://", string.Empty).Replace("https://", string.Empty));
			Prepare(filename, file);
		}
		else Prepare(Path.GetFileNameWithoutExtension(file), file);
	}
	public virtual void Prepare(string id, string file)
	{
		if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(file) || IgnoreList.Contains(file))
		{
			return;
		}

		if (!string.IsNullOrEmpty(file))
		{
			var extension = Path.GetExtension(file);

			if (!string.IsNullOrEmpty(Extension) && OsEx.File.Exists(file) && extension != Extension)
			{
				return;
			}
		}

		Remove(id);

		var instance = Activator.CreateInstance(IndexedType) as Process;
		InstanceBuffer.Add(id, instance);

		instance.File = file;
		instance.Execute(this);
	}
	public virtual void Remove(string id)
	{
		var existent = !InstanceBuffer.ContainsKey(id) ? null : InstanceBuffer[id];
		existent?.Dispose();

		if (InstanceBuffer.ContainsKey(id)) InstanceBuffer.Remove(id);
	}
	public virtual void Clear(IEnumerable<string> except = null)
	{
		foreach (var item in InstanceBuffer)
		{
			if (except != null && except.Any(x => item.Value.File.Contains(x)))
			{
				continue;
			}

			try
			{
				item.Value?.Clear();
				item.Value?.Dispose();
			}
			catch (Exception ex) { Logger.Error($" Processor error: '{item.Key}'", ex); }
		}

		var temp = Pool.Get<Dictionary<string, IBaseProcessor.IProcess>>();

		foreach (var instance in InstanceBuffer)
		{
			temp.Add(instance.Key, instance.Value);
		}

		if (except == null || !except.Any())
		{
			InstanceBuffer.Clear();
		}
		else
		{
			foreach (var instance in temp)
			{
				if (!except.Any(instance.Value.File.Contains))
				{
					InstanceBuffer.Remove(instance.Key);
				}
			}
		}

		Pool.FreeUnmanaged(ref temp);
	}
	public virtual void Ignore(string file)
	{
		if (!IgnoreList.Contains(file)) IgnoreList.Add(file);
	}
	public virtual void ClearIgnore(string file)
	{
		IgnoreList.RemoveAll(x => x == file);
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

	public virtual void OnCreated(object sender, FileSystemEventArgs e)
	{
		if (!EnableWatcher || IsBlacklisted(e.FullPath)) return;

		if (InstanceBuffer.TryGetValue(e.FullPath, out var instance1))
		{
			instance1?.MarkDirty();
			return;
		}

		if (InstanceBuffer.TryGetValue(Path.GetFileNameWithoutExtension(e.FullPath), out var instance2))
		{
			instance2?.MarkDirty();
			return;
		}

		InstanceBuffer.Add(e.FullPath, null);
	}
	public virtual void OnChanged(object sender, FileSystemEventArgs e)
	{
		var path = e.FullPath;
		var name = Path.GetFileNameWithoutExtension(path);

		if (!EnableWatcher || IsBlacklisted(path)) return;

		if (InstanceBuffer.TryGetValue(name, out var mod))
		{
			mod.MarkDirty();
		}
	}
	public virtual void OnRenamed(object sender, RenamedEventArgs e)
	{
		var path = e.FullPath;
		var name = Path.GetFileNameWithoutExtension(path);

		if (!EnableWatcher || IsBlacklisted(path)) return;

		if (InstanceBuffer.TryGetValue(name, out var mod))
		{
			mod.MarkDeleted();
		}
		InstanceBuffer.Add(name, null);
	}
	public virtual void OnRemoved(object sender, FileSystemEventArgs e)
	{
		var path = e.FullPath;
		var name = Path.GetFileNameWithoutExtension(path);

		if (!EnableWatcher || IsBlacklisted(path)) return;

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
		if (!IncludeSubdirectories && Path.GetFullPath(Path.GetDirectoryName(path)) != Path.GetFullPath(Folder))
		{
			return true;
		}

		if (BlacklistPattern == null) return false;

		for (int i = 0; i < BlacklistPattern.Length; i++)
		{
			if (path.Contains(BlacklistPattern[i])) return true;
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
