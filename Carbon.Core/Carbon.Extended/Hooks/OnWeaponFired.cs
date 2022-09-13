using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseProjectile ), "CLProject" )]
    public class OnWeaponFired
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnWeaponFired" );
        }
    }
}