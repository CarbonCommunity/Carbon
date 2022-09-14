using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Item ), "SplitItem" )]
    public class OnItemSplit
    {
        public static bool Postfix ( System.Int32 split_Amount, ref Item __result )
        {
            CarbonCore.Log ( $"Postfix OnItemSplit" );

            var result = HookExecutor.CallStaticHook ( "OnItemSplit" );
            
            if ( result != null )
            {
                __result = ( Item ) result;
                return false;
            }

            return true;
        }
    }
}