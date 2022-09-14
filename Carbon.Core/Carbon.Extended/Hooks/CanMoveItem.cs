using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "MoveItem" )]
    public class CanMoveItem
    {
        public static bool Prefix ( BaseEntity.RPCMessage msg , ref PlayerInventory __instance )
        {
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
            return HookExecutor.CallStaticHook ( "CanMoveItem", item, __instance, num2, num3, num4 ) == null;
        }
    }
}