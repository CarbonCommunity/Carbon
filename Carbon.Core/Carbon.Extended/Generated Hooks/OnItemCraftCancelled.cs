using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemCrafter ), "CancelTask" )]
    public class OnItemCraftCancelled
    {
        public static bool Postfix ( System.Int32 iID, System.Boolean ReturnItems, ref System.Boolean __result )
        {
            CarbonCore.Log ( $"Postfix OnItemCraftCancelled" );

            var result = HookExecutor.CallStaticHook ( "OnItemCraftCancelled", l1 );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}