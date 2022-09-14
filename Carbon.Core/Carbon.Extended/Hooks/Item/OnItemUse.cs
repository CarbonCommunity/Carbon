using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Item ), "UseItem" )]
    public class Item_UseItem
    {
        public static void Prefix ( Item __instance, ref int amountToConsume )
        {
            if ( amountToConsume == 0 )
                return;

            var obj = HookExecutor.CallStaticHook ( "OnItemUse", __instance, amountToConsume );

            if ( obj is int )
            {
                amountToConsume = ( int )obj;
            }
        }
    }
}