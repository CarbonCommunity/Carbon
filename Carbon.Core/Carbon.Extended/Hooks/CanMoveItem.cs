using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "MoveItem" )]
    public class CanMoveItem
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanMoveItem" );
        }
    }
}