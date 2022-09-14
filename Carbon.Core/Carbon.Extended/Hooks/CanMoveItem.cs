using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "MoveItem" )]
    public class CanMoveItem
    {
        public static bool Prefix ( BaseEntity.RPCMessage msg )
        {
            return HookExecutor.CallStaticHook ( "CanMoveItem" ) == null;
        }
    }
}