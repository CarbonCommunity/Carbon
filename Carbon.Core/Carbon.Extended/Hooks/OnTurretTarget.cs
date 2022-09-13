using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( AutoTurret ), "SetTarget" )]
    public class OnTurretTarget
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTurretTarget" );
        }
    }
}