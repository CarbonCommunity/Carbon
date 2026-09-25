using Carbon.Jobs;
using Carbon.Commands;
using Command = Carbon.Commands.Command;

namespace Carbon;

#if !(WIN || UNIX)
#error Target architecture not defined
#endif

public partial class Community
{
	public bool IsInitialized { get; private set; }

	/// <summary>Holds every runtime component Carbon attaches to Unity.</summary>
	public GameObject RuntimeObject { get; private set; }

	public virtual void Initialize()
	{
		StoredModifiers.Init();

		if (IsInitialized)
		{
			return;
		}

		HookCaller.Caller = new HookCallerInternal();

		LoadConfig();
		LoadMonoProfilerConfig();

		RefreshConsoleInfo();

		if (!Config.Logging.ReducedLogging)
		{
			Logger.Log(Environment.NewLine +
			           @"                                               " + Environment.NewLine +
			           @"  ______ _______ ______ ______ _______ _______ " + Environment.NewLine +
			           @" |      |   _   |   __ \   __ \       |    |  |" + Environment.NewLine +
			           @" |   ---|       |      <   __ <   -   |       |" + Environment.NewLine +
			           @" |______|___|___|___|__|______/_______|__|____|" + Environment.NewLine +
			           @"                          discord.gg/carbonmod " + Environment.NewLine +
			           @"                                               " + Environment.NewLine
			);

#if MINIMAL
			Logger.Log("Initializing minimal build...");
#else
			Logger.Log("Initializing...");
#endif
		}

		Events.Trigger(CarbonEvent.CarbonStartup, EventArgs.Empty);

		Logger.InitTaskExceptions();
		Defines.Initialize();
		Vault.Load();

		ThreadEx.MainThread.Name = "Main";

		InstallRuntime();
		Test.Integrations.Logger = new Logger();

		Events.Subscribe(CarbonEvent.HooksInstalled, _ =>
		{
			ClearCommands();
			InstallCore();
			Modules.Init();

			Events.Trigger(CarbonEvent.HookValidatorRefreshed, EventArgs.Empty);
		});

		Events.Subscribe(CarbonEvent.HookValidatorRefreshed, _ =>
		{
			CommandLine.ExecuteCommands("+carbon.onboot", "Carbon boot");

			var serverConfigPath = Path.Combine(ConVar.Server.GetServerFolder("cfg"), "server.cfg");
			var lines = OsEx.File.Exists(serverConfigPath) ? OsEx.File.ReadTextLines(serverConfigPath) : null;

			if (lines != null)
			{
				CommandLine.ExecuteCommands("+carbon.onboot", "cfg/server.cfg", lines);
				CommandLine.ExecuteCommands(lines);
				Array.Clear(lines, 0, lines.Length);
			}

			ReloadPlugins();
		});

		Logger.Log($"Carbon {Analytics.Version} [{Analytics.Protocol}] {Build.Git.HashShort} on {Analytics.Platform.ToCamelCase()}");
		Logger.Log($"       {Build.Git.Author} on {Build.Git.Branch} ({Build.Git.Date})");
		Logger.Log($"Rust   {Facepunch.BuildInfo.Current.Build.Number}/{Rust.Protocol.printable} on {Facepunch.BuildInfo.Current.Scm.Branch} ({Facepunch.BuildInfo.Current.Scm.Date}) {Facepunch.BuildInfo.Current.Scm.ChangeId}");

		InstallLibraries();

		IsInitialized = true;

		Events.Trigger(CarbonEvent.CarbonStartupComplete, EventArgs.Empty);

		WebControlPanel.Init();
	}

	public virtual void Uninitialize()
	{
		try
		{
			Events.Trigger(CarbonEvent.CarbonShutdown, EventArgs.Empty);

			UninstallRuntime();
			ClearCommands(all: true);
			ClearPlugins(all: true);
			ModLoader.Packages.Clear();
			HookSubscriberIndex.Invalidate();
			Debug.Log("Unloaded Carbon.");

#if WIN
			try
			{
				if (IsConfigReady && Config.Misc.ShowConsoleInfo && ServerConsole.Instance != null && ServerConsole.Instance.input != null)
				{
					ServerConsole.Instance.input.statusText = new string[3];
				}
			}
			catch { }
#endif

			Logger.Dispose();

			Events.Trigger(CarbonEvent.CarbonShutdownComplete, EventArgs.Empty);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed Carbon uninitialization.", ex);
			Events.Trigger(CarbonEvent.CarbonShutdownFailed, EventArgs.Empty);
		}

		Runtime = null;
	}

	/// <summary>Creates the runtime systems: plugin sources, hook manager, scheduler and module registry.</summary>
	protected virtual void InstallRuntime()
	{
		UninstallRuntime();

		RuntimeObject = new GameObject("Carbon.Runtime");
		UnityEngine.Object.DontDestroyOnLoad(RuntimeObject);

		Scheduler = RuntimeObject.AddComponent<Scheduler>();
		HookManager = RuntimeObject.AddComponent<PatchManager>();
		PluginSources = RuntimeObject.AddComponent<PluginSources>();
		PluginSources.Init();
		Modules = new ModuleRegistry();

		ScriptCompilationThread._injectPatchedReferences();
		RegisterBuiltinCommands();

		if (!Config.Logging.ReducedLogging)
		{
			Logger.Log("Installed runtime");
		}
	}

	protected virtual void UninstallRuntime()
	{
		try
		{
			PluginSources?.Shutdown();
			Modules?.Dispose();
		}
		catch (Exception ex)
		{
			Logger.Error("Failed uninstalling runtime", ex);
		}

		if (RuntimeObject != null)
		{
			UnityEngine.Object.Destroy(RuntimeObject);
			RuntimeObject = null;
		}
	}

	/// <summary>Creates the Core plugin and the default plugin packages.</summary>
	protected virtual void InstallCore()
	{
		Core = new CorePlugin();
		Core.Setup("Core", "Carbon Community", new VersionNumber(1, 0, 0), string.Empty);
		ModLoader.ProcessPrecompiledType(Core);
		Core.IsCorePlugin = Core.IsPrecompiled = true;
		Core.IInit();
		Core.ILoadDefaultMessages();

		ModLoader.RegisterPackage(Core.Package = ModLoader.Package.Get("Carbon Community", true).AddPlugin(Core));
		ModLoader.RegisterPackage(Plugins = ModLoader.Package.Get("Scripts", false));
		ModLoader.RegisterPackage(ZipPlugins = ModLoader.Package.Get("Zip Scripts", false));

		ModLoader.ProcessCommands(typeof(CorePlugin), Core, prefix: "c");
		ModLoader.ProcessCommands(typeof(CorePlugin), Core, prefix: "carbon", hidden: true);

		var commandCount = CommandManager.Chat.Count(x => x.Reference == Core && !x.HasFlag(CommandFlags.Hidden)) +
		                   CommandManager.ClientConsole.Count(x => x.Reference == Core && !x.HasFlag(CommandFlags.Hidden));

		if (!Config.Logging.ReducedLogging)
		{
			Logger.Log($"Initialized Carbon Core plugin ({Core.Hooks.Count:n0} {Core.Hooks.Count.Plural("hook", "hooks")}, {commandCount:n0} {commandCount.Plural("command", "commands")})");
		}

#if !MINIMAL
		CarbonAuto.Init();
		CarbonAuto.Singleton.Load();
#endif
	}

	private void RegisterBuiltinCommands()
	{
		static void AverageFps(Command.Args arg) => arg.ReplyWith($"{Performance.report.frameRateAverage:0}");

		CommandManager.RegisterCommand(new Command.RCon
		{
			Name = "avgfps",
			Help = "Displays the server's average FPS.",
			Callback = AverageFps
		}, out _);
		CommandManager.RegisterCommand(new Command.ClientConsole
		{
			Name = "avgfps",
			Help = "Displays the server's average FPS.",
			Callback = AverageFps,
			Auth = new Command.Authentication { AuthLevel = 2 }
		}, out _);
	}
}
