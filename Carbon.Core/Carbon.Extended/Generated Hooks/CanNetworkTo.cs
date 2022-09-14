using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseNetworkable ), "ShouldNetworkTo" )]
    public class CanNetworkTo
    {
        public static bool Prefix ( BasePlayer player, ref System.Boolean __result )
        {
            CarbonCore.Log ( $"Prefix CanNetworkTo" );

            var result = HookExecutor.CallStaticHook ( "CanNetworkTo" );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}