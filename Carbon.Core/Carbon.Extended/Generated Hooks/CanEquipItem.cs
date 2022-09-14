using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "CanEquipItem" )]
    public class CanEquipItem
    {
        public static bool Prefix ( Item item, System.Int32 targetSlot, ref System.Boolean __result )
        {
            CarbonCore.Log ( $"Prefix CanEquipItem" );

            var result = HookExecutor.CallStaticHook ( "CanEquipItem" );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}