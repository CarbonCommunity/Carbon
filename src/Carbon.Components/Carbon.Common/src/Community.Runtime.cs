using API.Analytics;
using API.Assembly;
using API.Commands;
using API.Contracts;
using API.Events;
using API.Hooks;
using Facepunch;

namespace Carbon;

public partial class Community
{
	public static GameObject GameObject => _gameObject.Value;

	private static readonly Lazy<GameObject> _gameObject = new(() =>
	{
		var gameObject = GameObject.Find("Carbon");
		return gameObject == null ? throw new Exception("Carbon GameObject not found") : gameObject;
	});

	public IAnalyticsManager Analytics => _analyticsManager.Value;
	public IAssemblyManager AssemblyEx => _assemblyEx.Value;
	public ICommandManager CommandManager => _commandManager.Value;
	public IDownloadManager Downloader => _downloadManager.Value;
	public IEventManager Events => _eventManager.Value;
	public ICompatManager Compat => _compatManager.Value;

	public IPatchManager HookManager { get; set; }
	public IScriptProcessor ScriptProcessor { get; set; }
	public IModuleProcessor ModuleProcessor { get; set; }
	public IZipScriptProcessor ZipScriptProcessor { get; set; }

#if DEBUG
	public IZipDevScriptProcessor ZipDevScriptProcessor { get; set; }
#endif

	public ICarbonProcessor CarbonProcessor { get; set; }

	public static bool IsServerInitialized { get; internal set; }
	public static bool IsConfigReady => Runtime != null && Runtime.Config != null;
	public static bool AllProcessorsFinalized => Runtime.ScriptProcessor.AllPendingScriptsComplete() &&
												 Runtime.ZipScriptProcessor.AllPendingScriptsComplete()
#if !MINIMAL && DEBUG
												 && Runtime.ZipDevScriptProcessor.AllPendingScriptsComplete()
#endif
		;
	public static int AllProcesses => Runtime.ScriptProcessor.InstanceBuffer.Count
		+ Runtime.ZipScriptProcessor.InstanceBuffer.Count
#if !MINIMAL && DEBUG
		+ Runtime.ZipDevScriptProcessor.InstanceBuffer.Count
#endif
		;

	internal static string _runtimeId;
	internal static Dictionary<StringView, StringView> _protect = [];

	public static string RuntimeId
	{
		get
		{
			if (string.IsNullOrEmpty(_runtimeId))
			{
				var date = DateTime.Now;
				_runtimeId = date.Year.ToString() + date.Month + date.Day +
							 date.Hour + date.Minute + date.Second + date.Millisecond;

			}

			return _runtimeId;
		}
	}

	/// <summary>
	/// Returns a unique string for the provided value. The results will be different each time the server reboots.
	/// </summary>
	/// <param name="name">Value string input.</param>
	/// <returns></returns>
	public static string Protect(string name)
	{
		if (string.IsNullOrEmpty(name))
			return string.Empty;

		var str = new StringView(name);
		var spaceIndex = str.IndexOf(' ');
		if (_protect.TryGetValue(str, out var cached))
			return cached.ToString();

		var command = spaceIndex < 0 ? str : str.Substring(0, spaceIndex);
		var args = spaceIndex < 0 ? string.Empty : str.Substring(spaceIndex + 1);
		return (_protect[str] = new StringView(Vault.Pool.Get(command + RuntimeId).ToString() + " " + args)).ToString();
	}
}
