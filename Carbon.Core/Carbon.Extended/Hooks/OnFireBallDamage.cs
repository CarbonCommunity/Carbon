using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( FireBall ), "DoRadialDamage" )]
    public class OnFireBallDamage
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnFireBallDamage" );
        }
    }
}