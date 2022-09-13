using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "GiveDefaultItems" )]
    public class OnDefaultItemsReceive
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnDefaultItemsReceive" );
        }
    }
}