using Carbon.Commands;
using Carbon.Events;
using Carbon.Hooks;
using Facepunch;

namespace Carbon;

public partial class Community
{
	/// <summary>The persistent GameObject hosting the core <see cref="Services"/>.</summary>
	public static GameObject GameObject => Services.GameObject;

	public AnalyticsManager Analytics => Services.Analytics;
	public AssemblyManager AssemblyEx => Services.Assemblies;
	public CommandManager CommandManager => Services.Commands;
	public DownloadManager Downloader => Services.Downloads;
	public EventManager Events => Services.Events;

	/// <summary>Installs and tracks Harmony-based hooks.</summary>
	public PatchManager HookManager { get; set; }

	/// <summary>Watches and compiles plugin sources (.cs, .cszip, dev folders).</summary>
	public PluginSources PluginSources { get; set; }

	/// <summary>Every loaded module.</summary>
	public ModuleRegistry Modules { get; set; }

	/// <summary>Main-thread dispatcher (next-frame callbacks, timer ticks).</summary>
	public Scheduler Scheduler { get; set; }

	public static bool IsServerInitialized { get; internal set; }
	public static bool IsConfigReady => Runtime != null && Runtime.Config != null;

	internal static string _runtimeId;

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
		{
			return string.Empty;
		}

		var str = new StringView(name);
		var spaceIndex = str.IndexOf(' ');
		if (spaceIndex < 0)
		{
			return Vault.Pool.Get(str + RuntimeId).ToString();
		}

		var command = str.Substring(0, spaceIndex);
		var args = str.Substring(spaceIndex + 1);
		return Vault.Pool.Get(command + RuntimeId) + " " + args;
	}
}
