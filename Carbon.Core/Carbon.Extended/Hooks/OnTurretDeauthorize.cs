using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( AutoTurret ), "RemoveSelfAuthorize" )]
    public class OnTurretDeauthorize
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTurretDeauthorize" );
        }
    }
}