///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [OxideHook ( "OnCollectiblePickup", typeof ( object ) ), OxideHook.Category ( OxideHook.Category.Enum.Resources )]
    [OxideHook.Parameter ( "this", typeof ( CollectibleEntity ) )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Info ( "Called when the player collects an item." )]
    [OxideHook.Patch ( typeof ( CollectibleEntity ), "DoPickup" )]
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