using API.Events;
using Cysharp.Threading.Tasks;
using Application = UnityEngine.Application;

namespace Carbon;

public partial class Community
{
	public static Community Runtime { get; set; }

	public Community()
	{
		try
		{
			Events.Subscribe(CarbonEvent.CarbonStartup, args =>
			{
				Logger.Log($"Carbon fingerprint: {Analytics.ClientID}");
				Logger.Log($"System fingerprint: {Analytics.SystemID}");
				Analytics.SessionStart();
			});

			Events.Subscribe(CarbonEvent.CarbonStartupComplete, args =>
			{
				Components.Analytics.on_server_startup();
			});

			var newlineSplit = new[] { '\n' };

			Application.logMessageReceived += (string condition, string stackTrace, LogType type) =>
			{
				switch (type)
				{
					case LogType.Error:
					case LogType.Exception:
					case LogType.Assert:
						if (!string.IsNullOrEmpty(condition) &&
						(condition.StartsWith("Null") || condition.StartsWith("Index")))
						{
							var trace = stackTrace.Split(newlineSplit, StringSplitOptions.RemoveEmptyEntries);
							var resultTrace = string.Empty;

							for (int i = 0; i < trace.Length; i++)
							{
								var t = trace[i];
								if (string.IsNullOrEmpty(t)) continue;

								resultTrace += $"  at {t}\n";
							}

							Array.Clear(trace, 0, trace.Length);

							resultTrace = resultTrace.TrimEnd();
							Logger.Write(API.Logger.Severity.Error, $"Unhandled error occurred ({condition})\n{resultTrace}", nativeLog: false);
							Console.WriteLine(resultTrace);
						}
						break;
				}
			};
		}
		catch (Exception ex)
		{
			Logger.Error("Critical error", ex);
		}
	}

	public void MarkServerInitialized(bool wants)
	{
		IsServerInitialized = wants;
	}
	public void ClearCommands(bool all = false)
	{
		CommandManager.ClearCommands(command => all || command.Reference is RustPlugin plugin && !plugin.IsCorePlugin);
	}

	public void RefreshConsoleInfo()
	{
#if WIN
		if (!IsConfigReady || !Config.Misc.ShowConsoleInfo) return;

		if (!IsServerInitialized || ServerConsole.Instance == null) return;
		if (ServerConsole.Instance.input.statusText.Length != 4) ServerConsole.Instance.input.statusText = new string[4];

		var version =
#if DEBUG
			Analytics.InformationalVersion;
#else
            Analytics.Version;
#endif

		ServerConsole.Instance.input.statusText[3] = $" Carbon" +
#if MINIMAL
			$" Minimal" +
#endif
			$" v{version}, {ModLoader.Packages.Count:n0} mods, {ModLoader.Packages.Sum(x => x.Plugins.Count):n0} plgs, {ModuleProcessor.Modules.Count(x => x is BaseModule module && module.IsEnabled()):n0}/{ModuleProcessor.Modules.Count:n0} mdls, {AssemblyEx.Extensions.Loaded.Count:n0} exts, {StoredModifiers.Entities?.Count:n0} mdfs";
#endif
	}

	public virtual void Initialize()
	{
		StoredModifiers.Init();
		UniTaskInjector.Inject(SynchronizationContext.Current, Thread.CurrentThread.ManagedThreadId, injectTimings: InjectPlayerLoopTimings.Minimum);
	}
	public virtual void Uninitialize()
	{
		Runtime = null;
	}
}
