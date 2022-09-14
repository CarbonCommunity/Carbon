using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemContainer ), "Remove" )]
    public class OnItemRemovedFromContainer
    {
        public static bool Postfix ( Item item, ref System.Boolean __result )
        {
            CarbonCore.Log ( $"Postfix OnItemRemovedFromContainer" );

            var result = HookExecutor.CallStaticHook ( "OnItemRemovedFromContainer" );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}