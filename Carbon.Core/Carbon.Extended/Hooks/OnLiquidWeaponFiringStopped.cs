using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( LiquidWeapon ), "StopFiring" )]
    public class OnLiquidWeaponFiringStopped
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnLiquidWeaponFiringStopped" );
        }
    }
}