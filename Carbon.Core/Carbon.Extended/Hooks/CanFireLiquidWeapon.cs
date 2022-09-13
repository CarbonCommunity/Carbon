using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( LiquidWeapon ), "CanFire" )]
    public class CanFireLiquidWeapon
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanFireLiquidWeapon" );
        }
    }
}