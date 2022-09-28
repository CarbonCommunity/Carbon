///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
    [OxideHook ( "CanStackItem", typeof ( object ) ), OxideHook.Category ( Hook.Category.Enum.Item )]
    [OxideHook.Parameter ( "item", typeof ( Item ) )]
    [OxideHook.Parameter ( "targetItem", typeof ( Item ) )]
    [OxideHook.Info ( "Called when moving an item onto another item." )]
    [OxideHook.Patch ( typeof ( Item ), "CanStack" )]
    public class PlayerInventory_MoveItem
    {
        public static bool Prefix ( Item item, ref Item __instance )
        {
            return HookExecutor.CallStaticHook ( "CanStackItem", __instance, item ) == null;
        }
    }
}