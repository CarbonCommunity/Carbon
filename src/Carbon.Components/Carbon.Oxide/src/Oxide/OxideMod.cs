using System.Collections.Concurrent;
using Newtonsoft.Json;
using Oxide.Core.Extensions;
using Oxide.Core.Logging;

namespace Oxide.Core;

/// <summary>
/// The "Interface.Oxide" singleton. Most of it forwards to Carbon's native systems.
/// </summary>
public class OxideMod
{
	public DataFileSystem DataFileSystem => Community.Runtime.DataFileSystem;
	public PluginManager RootPluginManager { get; private set; } = new();

	public Permission Permission => Community.Runtime.Permission;

	public string RootDirectory { get; private set; }
	public string InstanceDirectory { get; private set; }
	public string PluginDirectory { get; private set; }
	public string ConfigDirectory { get; private set; }
	public string DataDirectory { get; private set; }
	public string LangDirectory { get; private set; }
	public string LogDirectory { get; private set; }
	public string TempDirectory { get; private set; }
	public string ExtensionDirectory { get; private set; }

	public bool IsShuttingDown { get; private set; }

	private Oxide.Core.Extensions.ExtensionManager extensionManager = new();
	public IEnumerable<PluginLoader> GetPluginLoaders() => extensionManager.GetPluginLoaders();
	public OxideConfig Config { get; private set; } = new(Path.Combine(Defines.GetRootFolder(), "oxide.config.json"));

	internal static readonly Version _assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
	internal List<Extension> _extensions = new();

	public float Now => UnityEngine.Time.realtimeSinceStartup;

	public void Load()
	{
		InstanceDirectory = Defines.GetRootFolder();
		RootDirectory = Environment.CurrentDirectory;
		if (RootDirectory.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)))
			RootDirectory = AppDomain.CurrentDomain.BaseDirectory;

		ConfigDirectory = Defines.GetConfigsFolder();
		DataDirectory = Defines.GetDataFolder();
		LangDirectory = Defines.GetLangFolder();
		LogDirectory = Defines.GetLogsFolder();
		PluginDirectory = Defines.GetScriptsFolder();
		TempDirectory = Defines.GetTempFolder();
		ExtensionDirectory = Defines.GetExtensionsFolder();

		_extensions.Add(new Extension { Name = "Rust", Author = "Carbon Community LTD", Branch = "none", Filename = "Carbon.dll", Version = new VersionNumber(1, 0, 0) });

		CovalencePlugin.PlayerManager.RefreshDatabase(Permission.userdata);
	}

	public void NextTick(Action callback) => Community.Runtime.Scheduler.NextFrame(callback);

	public void NextFrame(Action callback) => Community.Runtime.Scheduler.NextFrame(callback);

	public bool LoadPlugin(string name)
	{
		CorePlugin.ProcessableFilesLookup();

		var path = CorePlugin.GetPluginFile(name);

		if (!path.IsValid)
		{
			return false;
		}

		path.Source.Prepare(path.Id, path.Path);
		return true;
	}

	public bool ReloadPlugin(string name)
	{
		return LoadPlugin(name);
	}

	public bool UnloadPlugin(string name)
	{
		var file = CorePlugin.GetPluginFile(name);

		if (!file.IsValid)
		{
			return false;
		}

		if (ModLoader.FindPlugin(name) is Plugin plugin && !plugin.IsCorePlugin)
		{
			ModLoader.UninitializePlugin(plugin);
		}

		file.Source.Remove(file.Id);
		return true;
	}

	public void ReloadAllPlugins(IList<string> skip = null)
	{
		foreach (var entry in Community.Runtime.PluginSources.All.SelectMany(x => x.Entries.Values))
		{
			if (skip != null && skip.Any(x => entry.Key.Contains(x)))
			{
				continue;
			}

			entry.MarkDirty();
		}
	}

	public void UnloadAllPlugins(IList<string> skip = null)
	{
		foreach (var source in Community.Runtime.PluginSources.All)
		{
			source.Clear(skip);
		}
	}

	public void OnSave()
	{

	}

	public void OnShutdown()
	{
		// Carbon saves permissions itself on shutdown
		IsShuttingDown = true;
	}

	public IEnumerable<Extension> GetAllExtensions()
	{
		return _extensions;
	}

	public object CallHook(string hookName)
	{
		var hookId = HookStringPool.GetOrAdd(hookName);
		return HookCaller.CallStaticHook(hookId);
	}

	public object CallHook(string hookName, object arg1)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1);
	}

	public object CallHook(string hookName, object arg1, object arg2)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1, arg2);
	}

	public object CallHook(string hookName, object arg1, object arg2, object arg3)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1, arg2, arg3);
	}

	public object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1, arg2, arg3, arg4);
	}

	public object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1, arg2, arg3, arg4, arg5);
	}

	public object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1, arg2, arg3, arg4, arg5, arg6);
	}

	public object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}

	public object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	}

	public object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
	}

	public object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
	}

	public object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
	}

	public object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
	}

	public object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
	{
	    var hookId = HookStringPool.GetOrAdd(hookName);
	    return HookCaller.CallStaticHook(hookId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
	}

	public object CallHook(string hookName, params object[] args)
	{
		var hookId = HookStringPool.GetOrAdd(hookName);
		return HookCaller.CallStaticHook(hookId, args);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1, arg2);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1, arg2, arg3);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1, arg2, arg3, arg4);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1, arg2, arg3, arg4, arg5);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1, arg2, arg3, arg4, arg5, arg6);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
	{
	    var oldHookId = HookStringPool.GetOrAdd(oldHook);
	    var newHookId = HookStringPool.GetOrAdd(newHook);
	    return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, params object[] args)
	{
		var oldHookId = HookStringPool.GetOrAdd(oldHook);
		var newHookId = HookStringPool.GetOrAdd(newHook);
		return HookCaller.CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, args);
	}

	internal static ConcurrentDictionary<string, object> _libraryCache = new();

	public T GetLibrary<T>(string name = null) where T : class
	{
		var type = typeof(T);

		// Shared libraries map to the native instances Carbon's core plugin uses
		if (type == typeof(Permission)) return Community.Runtime.Permission as T;
		if (type == typeof(Lang)) return Community.Runtime.Core.lang as T;
		if (type == typeof(CommandLibrary)) return Community.Runtime.Core.cmd as T;
		if (type == typeof(WebRequests)) return Community.Runtime.Core.webrequest as T;
		if (type == typeof(TimerLibrary)) return Community.Runtime.Core.timer?.Library as T;

		name ??= type.Name;

		if (!_libraryCache.TryGetValue(name, out var instance))
		{
			try { instance = Activator.CreateInstance<T>(); }
			catch
			{
				try { instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T)) as T; }
				catch { }
			}

			_libraryCache.TryAdd(name, instance);
		}

		return instance as T;
	}

	public Extension GetExtension(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}

		foreach (var extension in Oxide.Core.Extensions.ExtensionManager.extensionCache)
		{
			if (extension.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
			{
				return extension;
			}
		}

		return null;
	}

	public void LoadExtension(string name)
	{
		var path = Path.Combine(Defines.GetExtensionsFolder(), name + ".dll");
		Logger.Log("Loading extension: " + path);
		Community.Runtime.AssemblyEx.Extensions.Load(path, "OxideMod.LoadExtension");
	}

	public void LoadAllPlugins(bool init = false)
	{

	}

	public static readonly VersionNumber Version = new(_assemblyVersion.Major, _assemblyVersion.Minor, _assemblyVersion.Build);

	#region Logging

	public CompoundLogger RootLogger { get; set; } = new();

	/// <summary>
	/// Outputs to the game's console a message with severity level 'DEBUG'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	public void LogDebug(string message, params object[] args)
		=> Logger.Debug(args != null && args.Length > 0 ? string.Format(message, args) : message);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="args"></param>
	public void LogError(string message, params object[] args)
		=> Logger.Error(args != null && args.Length > 0 ? string.Format(message, args) : message);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="ex"></param>
	public void LogException(string message, Exception ex)
		=> Logger.Error(message, ex);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'NOTICE'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="args"></param>
	public void LogInfo(string message, params object[] args)
		=> Logger.Log(args != null && args.Length > 0 ? string.Format(message, args) : message);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'WARNING'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="args"></param>
	public void LogWarning(string message, params object[] args)
		=> Logger.Warn(args != null && args.Length > 0 ? string.Format(message, args) : message);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'WARNING'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="args"></param>
	public void PrintWarning(string message, params object[] args)
		=> Logger.Warn(args != null && args.Length > 0 ? string.Format(message, args) : message);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="args"></param>
	public void PrintError(string message, params object[] args)
		=> Logger.Error(args != null && args.Length > 0 ? string.Format(message, args) : message);

	#endregion
}
