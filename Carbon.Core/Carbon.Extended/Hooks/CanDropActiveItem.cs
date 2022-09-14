using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "ShouldDropActiveItem" )]
    public class CanDropActiveItem
    {
        public static bool Prefix ( ref System.Boolean __result )
        {
            CarbonCore.Log ( $"Prefix CanDropActiveItem" );

            var result = HookExecutor.CallStaticHook ( "CanDropActiveItem" );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}