using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( KeyLock ), "OnTryToOpen" )]
    public class CanUseLockedEntity
    {
        public static bool Prefix ( BasePlayer player, ref System.Boolean __result, ref KeyLock __instance )
        {
            CarbonCore.Log ( $"Prefix CanUseLockedEntity" );

            var result = HookExecutor.CallStaticHook ( "CanUseLockedEntity", a0, __instance );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}