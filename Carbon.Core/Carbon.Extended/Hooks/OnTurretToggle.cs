using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( AutoTurret ), "SetIsOnline" )]
    public class OnTurretToggle
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTurretToggle" );
        }
    }
}