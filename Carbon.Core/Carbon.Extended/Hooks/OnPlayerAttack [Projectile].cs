using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnProjectileAttack" )]
    public class OnPlayerAttack [Projectile]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerAttack [Projectile]" );
        }
    }
}