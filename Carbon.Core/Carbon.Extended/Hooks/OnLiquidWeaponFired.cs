using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( LiquidWeapon ), "StartFiring" )]
    public class OnLiquidWeaponFired
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnLiquidWeaponFired" );
        }
    }
}