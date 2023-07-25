/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Base;

public abstract class BaseProcessor : FacepunchBehaviour, IDisposable, IBaseProcessor
{
	public virtual string Name { get; }

	public Dictionary<string, IBaseProcessor.IInstance> InstanceBuffer { get; set; }
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
	internal Dictionary<string, IBaseProcessor.IInstance> _runtimeCache = new(1000);

	public bool IsInitialized { get; set; }

	public void Awake()
	{
		Logger.Log($"- Installed {Name}");
	}
	public virtual void Start()
	{
		if (IsInitialized) return;

		InstanceBuffer = new Dictionary<string, IBaseProcessor.IInstance>();
		IgnoreList = new List<string>();

		DontDestroyOnLoad(gameObject);

		IsInitialized = true;

		_wfsInstance = new WaitForSeconds(Rate);

		StopAllCoroutines();
		StartCoroutine(Run());

		Watcher?.Dispose();
		Watcher = null;

		if (!string.IsNullOrEmpty(Extension) && !string.IsNullOrEmpty(Folder))
		{
			Watcher = new FileSystemWatcher(Folder)
			{
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.FileName,
				Filter = $"*{Extension}"
			};
			Watcher.Created += _onCreated;
			Watcher.Changed += _onChanged;
			Watcher.Renamed += _onRenamed;
			Watcher.Deleted += _onRemoved;
			Watcher.IncludeSubdirectories = true;
			Watcher.EnableRaisingEvents = true;
		}

		Logger.Log($" Initialized {IndexedType?.Name} processor...");
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

			foreach (var element in InstanceBuffer) _runtimeCache.Add(element.Key, element.Value);

			foreach (var element in _runtimeCache)
			{
				var id = Path.GetFileNameWithoutExtension(element.Key);

				if (element.Value == null)
				{
					var instance = Activator.CreateInstance(IndexedType) as Instance;
					instance.File = element.Key;
					instance.Execute();

					InstanceBuffer.Remove(element.Key);
					InstanceBuffer[id] = instance;
					continue;
				}

				if (element.Value.IsDirty)
				{
					Process(element.Key, element.Value);
					yield return null;
					continue;
				}

				if (element.Value.IsRemoved)
				{
					Clear(id, element.Value);
					yield return null;
					continue;
				}
			}

			_runtimeCache.Clear();

			yield return null;
		}
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
		if (IgnoreList.Contains(file)) return;

		if (!string.IsNullOrEmpty(file))
		{
			var extension = Path.GetExtension(file);

			if (!string.IsNullOrEmpty(Extension) && OsEx.File.Exists(file) && extension != Extension)
			{
				return;
			}
		}

		Logger.Debug($" Loading plugin '{id}'...", 1);

		Remove(id);

		var instance = Activator.CreateInstance(IndexedType) as Instance;
		InstanceBuffer.Add(id, instance);

		instance.File = file;
		instance.Execute();
	}
	public virtual void Remove(string id)
	{
		var existent = !InstanceBuffer.ContainsKey(id) ? null : InstanceBuffer[id];
		existent?.Dispose();

		if (InstanceBuffer.ContainsKey(id)) InstanceBuffer.Remove(id);
	}
	public virtual void Clear()
	{
		foreach (var item in InstanceBuffer)
		{
			try
			{
				item.Value?.Dispose();
			}
			catch (Exception ex) { Logger.Error($" Processor error: '{item.Key}'", ex); }
		}

		InstanceBuffer.Clear();
	}
	public virtual void Ignore(string file)
	{
		if (!IgnoreList.Contains(file)) IgnoreList.Add(file);
	}
	public virtual void ClearIgnore(string file)
	{
		IgnoreList.RemoveAll(x => x == file);
	}
	public T Get<T>(string id) where T : IBaseProcessor.IInstance
	{
		if (InstanceBuffer.TryGetValue(id, out var instance))
		{
			return (T)instance;
		}

		return default;
	}

	public virtual void Clear(string id, IBaseProcessor.IInstance instance)
	{
		instance?.Dispose();
		instance = null;
		Remove(id);
	}
	public virtual void Process(string id, IBaseProcessor.IInstance instance)
	{
		var file = instance.File;

		Clear(id, instance);
		Prepare(id, file);
	}

	internal void _onCreated(object sender, FileSystemEventArgs e)
	{
		if (!EnableWatcher || IsBlacklisted(e.FullPath)) return;

		if (InstanceBuffer.TryGetValue(e.FullPath, out var instance1))
		{
			instance1?.SetDirty();
			return;
		}

		if (InstanceBuffer.TryGetValue(Path.GetFileNameWithoutExtension(e.FullPath), out var instance2))
		{
			instance2?.SetDirty();
			return;
		}

		InstanceBuffer.Add(e.FullPath, null);
	}
	internal void _onChanged(object sender, FileSystemEventArgs e)
	{
		var path = e.FullPath;
		var name = Path.GetFileNameWithoutExtension(path);

		if (!EnableWatcher || IsBlacklisted(path)) return;

		if (InstanceBuffer.TryGetValue(name, out var mod)) mod.SetDirty();
	}
	internal void _onRenamed(object sender, RenamedEventArgs e)
	{
		var path = e.FullPath;
		var name = Path.GetFileNameWithoutExtension(path);

		if (!EnableWatcher || IsBlacklisted(path)) return;

		if (InstanceBuffer.TryGetValue(name, out var mod)) mod.MarkDeleted();
		InstanceBuffer.Add(name, null);
	}
	internal void _onRemoved(object sender, FileSystemEventArgs e)
	{
		var path = e.FullPath;
		var name = Path.GetFileNameWithoutExtension(path);

		if (!EnableWatcher || IsBlacklisted(path)) return;

		if (InstanceBuffer.TryGetValue(name, out var mod)) mod.MarkDeleted();
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

	public class Instance : IBaseProcessor.IInstance, IDisposable
	{
		public virtual IBaseProcessor.IParser Parser { get; }

		public string File { get; set; }

		internal bool _hasChanged;
		internal bool _hasRemoved;

		public virtual void Dispose() { }
		public virtual void Execute() { }

		public bool HasSucceeded { get; set; }
		public bool IsDirty => _hasChanged;
		public bool IsRemoved => _hasRemoved;

		public void SetDirty()
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
