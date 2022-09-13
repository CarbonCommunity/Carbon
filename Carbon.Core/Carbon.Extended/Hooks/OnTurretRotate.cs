using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( AutoTurret ), "FlipAim" )]
    public class OnTurretRotate
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTurretRotate" );
        }
    }
}