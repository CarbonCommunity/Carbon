using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Signage ), "CanUpdateSign" )]
    public class CanUpdateSign
    {
        public static bool Prefix ( BasePlayer player, ref System.Boolean __result, ref Signage __instance )
        {
            CarbonCore.Log ( $"Prefix CanUpdateSign" );

            var result = HookExecutor.CallStaticHook ( "CanUpdateSign", a0, __instance );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}