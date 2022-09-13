using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "ItemCmd" )]
    public class OnItemAction
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemAction" );
        }
    }
}