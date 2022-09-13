using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VehicleModuleEngine ), "RefreshPerformanceStats" )]
    public class OnEngineStatsRefresh
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEngineStatsRefresh" );
        }
    }
}