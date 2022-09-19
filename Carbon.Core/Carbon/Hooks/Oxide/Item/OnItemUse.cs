using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [Hook ( "OnItemUse", typeof ( int ) ), Hook.Category ( Hook.Category.Enum.Item )]
    [Hook.Parameter ( "this", typeof ( Item ) )]
    [Hook.Parameter ( "amountToConsume", typeof ( int ) )]
    [Hook.Info ( "Called when an item is used." )]
    [Hook.Info ( "Returning an int overrides the amount consumed." )]
    [Hook.Patch ( typeof ( Item ), "UseItem" )]
    public class Item_UseItem
    {
        public static void Prefix ( ref int amountToConsume, ref Item __instance )
        {
            if ( amountToConsume <= 0 )
                return;

            var obj = HookExecutor.CallStaticHook ( "OnItemUse", __instance, amountToConsume );

            if ( obj is int )
            {
                amountToConsume = ( int )obj;
            }
        }
    }
}