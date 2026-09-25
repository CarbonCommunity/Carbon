using System;
using System.IO;
using System.Reflection;
using Carbon.Compat;
using Carbon.Core;
using Carbon.Events;
using Carbon.Managers;
using Patches;

namespace Carbon;

/// <summary>
/// First Carbon code to run, injected into Rust's Bootstrap.Init_Tier0 by the publicizer.
/// Installs the core <see cref="Services"/> and waits for the shared startup to boot Carbon.dll.
/// </summary>
public sealed class Bootstrap
{
	private static readonly string identifier;
	private static readonly string assemblyName;
	private static HarmonyLib.Harmony _harmonyInstance;

	public static string Name => assemblyName;

	internal static HarmonyLib.Harmony Harmony => _harmonyInstance;

	static Bootstrap()
	{
		Carbon.Components.ConVarSnapshots.TakeSnapshot();

		identifier = $"{Guid.NewGuid():N}";
		assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
	}

	public static void Initialize()
	{
		Logger.Log($"{assemblyName} loaded.");
		_harmonyInstance = new HarmonyLib.Harmony(identifier);

		var logPath = Path.Combine(Defines.GetLogsFolder(), "Carbon.Harmony.log");

		Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", logPath);
		typeof(HarmonyLib.FileLog).GetField("_logPathInited", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
#if DEBUG
		HarmonyLib.Harmony.DEBUG = true;
#elif RELEASE
		HarmonyLib.Harmony.DEBUG = false;
#endif

		if (File.Exists(logPath))
		{
			File.Delete(logPath);
		}

		Services.Install();

		Services.Events.Subscribe(CarbonEvent.StartupShared, _ =>
		{
			Services.InstallAssemblies();

			// Harmony mod compatibility is always present
			Services.GameObject.AddComponent<CompatManager>();

			// Optional packages load first so they can register into Carbon's extension points
			Services.Assemblies.Components.LoadPackages();
			Services.Assemblies.Components.Load("Carbon.dll", "CarbonEvent.StartupShared");
		});

		try
		{
			Logger.Log("Applying Harmony patches");
			Harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
		catch (Exception e)
		{
			Logger.Error("Unable to apply all patches", e);
		}

		Services.Events.Subscribe(CarbonEvent.HooksInstalled, _ =>
		{
			FileSystem_WarmupHalt.IsReady = true;
		});
	}
}
