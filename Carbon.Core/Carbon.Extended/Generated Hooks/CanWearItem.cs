using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "CanWearItem" )]
    public class CanWearItem
    {
        public static bool Prefix ( Item item, System.Int32 targetSlot, ref System.Boolean __result )
        {
            CarbonCore.Log ( $"Prefix CanWearItem" );

            var result = HookExecutor.CallStaticHook ( "CanWearItem" );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}