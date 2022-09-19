using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [Hook ( "OnCollectiblePickup", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Resources )]
    [Hook.Parameter ( "this", typeof ( CollectibleEntity ) )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Info ( "Called when the player collects an item." )]
    [Hook.Patch ( typeof ( CollectibleEntity ), "DoPickup" )]
    public class CollectibleEntity_DoPickup
    {
        public static bool Prefix ( BasePlayer reciever, ref CollectibleEntity __instance )
        {
            if ( __instance.itemList == null )
            {
                return false;
            }

            return HookExecutor.CallStaticHook ( "OnCollectiblePickup", __instance, reciever ) == null;
        }
    }
}