using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( AutoTurret ), "InitiateShutdown" )]
    public class OnTurretShutdown
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTurretShutdown" );
        }
    }
}