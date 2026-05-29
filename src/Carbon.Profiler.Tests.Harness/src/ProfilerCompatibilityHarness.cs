using JetBrains.Annotations;

namespace Carbon.Profiler.Tests.Harness;

[UsedImplicitly]
public sealed class ProfilerCompatibilityHarness : IHarmonyModHooks
{
	public void OnLoaded(OnHarmonyModLoadedArgs args)
	{
		RunnerLog.Info("Harness loaded. Waiting for Bootstrap.DedicatedServerStartup");
	}

	public void OnUnloaded(OnHarmonyModUnloadedArgs args)
	{
	}
}
