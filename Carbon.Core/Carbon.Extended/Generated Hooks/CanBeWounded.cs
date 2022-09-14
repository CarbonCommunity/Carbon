using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "EligibleForWounding" )]
    public class CanBeWounded
    {
        public static bool Prefix ( HitInfo info, ref System.Boolean __result )
        {
            CarbonCore.Log ( $"Prefix CanBeWounded" );

            var result = HookExecutor.CallStaticHook ( "CanBeWounded" );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}