///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Extended
{
    [OxideHook ( "CanDropActiveItem", typeof ( bool ) ), OxideHook.Category ( OxideHook.Category.Enum.Item )]
    [OxideHook.Parameter ( "this", typeof ( BasePlayer ) )]
    [OxideHook.Info ( "Called when a player attempts to drop their active item." )]
    [OxideHook.Info ( "Returning true or false overrides default behavior." )]
    [OxideHook.Patch ( typeof ( BasePlayer ), "ShouldDropActiveItem" )]
    public class BasePlayer_ShouldDropActiveItem
    {
        public static bool Prefix ( ref Item __instance, out bool __result )
        {
            var obj = Interface.CallHook ( "CanDropActiveItem", __instance );
            __result =  !( obj is bool ) || ( bool )obj;
            return false;
        }
    }
}