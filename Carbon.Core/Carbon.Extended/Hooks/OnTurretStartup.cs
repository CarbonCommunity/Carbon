using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( AutoTurret ), "InitiateStartup" )]
    public class OnTurretStartup
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTurretStartup" );
        }
    }
}