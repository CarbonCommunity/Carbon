using HarmonyLib;
using JetBrains.Annotations;

namespace Carbon.Profiler.Tests.Harness;

[HarmonyPatch(typeof(Bootstrap), "DedicatedServerStartup")]
internal static class BootstrapDedicatedServerStartupPatch
{
	[UsedImplicitly]
	[HarmonyPrefix]
	public static bool Prefix()
	{
		RunnerLog.Info("Bootstrap.DedicatedServerStartup intercepted. Starting harness before server startup");
		HarnessBootstrap.Start();
		return false;
	}
}
