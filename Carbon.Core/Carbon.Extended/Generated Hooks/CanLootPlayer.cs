using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "CanBeLooted" )]
    public class CanLootPlayer
    {
        public static bool Prefix ( BasePlayer player, ref System.Boolean __result )
        {
            CarbonCore.Log ( $"Prefix CanLootPlayer" );

            var result = HookExecutor.CallStaticHook ( "CanLootPlayer" );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}