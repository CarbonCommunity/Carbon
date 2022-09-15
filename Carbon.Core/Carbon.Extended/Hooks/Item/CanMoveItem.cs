using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [Hook ( "CanMoveItem" ), Hook.Category ( Hook.Category.Enum.Item )]
    [Hook.Parameter ( "item", typeof ( Item ) )]
    [Hook.Parameter ( "this", typeof ( PlayerInventory ) )]
    [Hook.Parameter ( "targetContainer", typeof ( uint ) )]
    [Hook.Parameter ( "targetSlot", typeof ( int ) )]
    [Hook.Parameter ( "amount", typeof ( int ) )]
    [Hook.Info ( "Called when moving an item from one inventory slot to another." )]
    [HarmonyPatch ( typeof ( PlayerInventory ), "MoveItem" )]
    public class CanMoveItem
    {
        public static bool Prefix ( BaseEntity.RPCMessage msg , ref PlayerInventory __instance )
        {
            var oldPosition = msg.read.Position;
            var num = msg.read.UInt32 ();
            var num2 = msg.read.UInt32 ();
            var num3 = ( int )msg.read.Int8 ();
            var num4 = ( int )msg.read.UInt32 ();
            var item = __instance.FindItemUID ( num );
            if ( item == null )
            {
                msg.player.ChatMessage ( "Invalid item (" + num + ")" );
                return false;
            }
            msg.read.Position = oldPosition;
            return HookExecutor.CallStaticHook ( "CanMoveItem", item, __instance, num2, num3, num4 ) == null;
        }
    }
}