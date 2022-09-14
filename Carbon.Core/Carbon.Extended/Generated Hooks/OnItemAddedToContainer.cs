using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemContainer ), "Insert" )]
    public class OnItemAddedToContainer
    {
        public static bool Postfix ( Item item, ref System.Boolean __result )
        {
            CarbonCore.Log ( $"Postfix OnItemAddedToContainer" );

            var result = HookExecutor.CallStaticHook ( "OnItemAddedToContainer" );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}