using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ShopFront ), "CancelClicked" )]
    public class OnShopCancelClick
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnShopCancelClick" );
        }
    }
}