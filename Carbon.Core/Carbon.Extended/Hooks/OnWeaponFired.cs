using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseProjectile ), "CLProject" )]
    public class OnWeaponFired
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnWeaponFired" );
        }
    }
}