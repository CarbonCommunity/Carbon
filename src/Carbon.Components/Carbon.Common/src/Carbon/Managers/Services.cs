namespace Carbon.Managers;

/// <summary>
/// Process-wide managers created during early boot, before <see cref="Community"/> exists.
/// They live on the persistent "Carbon" GameObject and survive Carbon reloads.
/// </summary>
public static class Services
{
	public static GameObject GameObject { get; private set; }

	public static EventManager Events { get; private set; }
	public static CommandManager Commands { get; private set; }
	public static FileWatcherManager FileWatcher { get; private set; }
	public static AnalyticsManager Analytics { get; private set; }
	public static DownloadManager Downloads { get; private set; }

	/// <summary>Loads components, extensions, hooks and modules. Available once the server shared startup ran.</summary>
	public static AssemblyManager Assemblies { get; private set; }

	public static bool IsInstalled => GameObject != null;

	/// <summary>Creates the core managers. Called once by Carbon.Bootstrap.</summary>
	public static void Install()
	{
		if (IsInstalled)
		{
			return;
		}

		GameObject = new GameObject("Carbon");
		UnityEngine.Object.DontDestroyOnLoad(GameObject);

		// Order matters, later managers rely on events and commands during Awake
		Commands = GameObject.AddComponent<CommandManager>();
		Events = GameObject.AddComponent<EventManager>();
		FileWatcher = GameObject.AddComponent<FileWatcherManager>();
		Analytics = GameObject.AddComponent<AnalyticsManager>();
		Downloads = GameObject.AddComponent<DownloadManager>();

		Events.Subscribe(CarbonEvent.CarbonStartupComplete, _ => FileWatcher.enabled = true);
	}

	/// <summary>Creates the assembly manager, which boots Carbon.dll and every addon.</summary>
	public static void InstallAssemblies()
	{
		if (Assemblies != null)
		{
			return;
		}

		Assemblies = GameObject.AddComponent<AssemblyManager>();
	}
}
