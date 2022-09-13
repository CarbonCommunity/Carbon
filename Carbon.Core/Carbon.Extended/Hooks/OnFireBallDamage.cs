using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( FireBall ), "DoRadialDamage" )]
    public class OnFireBallDamage
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnFireBallDamage" );
        }
    }
}