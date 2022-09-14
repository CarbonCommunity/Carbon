using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerLoot ), "StartLootingEntity" )]
    public class OnLootEntity
    {
        public static bool Postfix ( BaseEntity targetEntity, System.Boolean doPositionChecks, ref System.Boolean __result, ref PlayerLoot __instance )
        {
            CarbonCore.Log ( $"Postfix OnLootEntity" );

            var result = HookExecutor.CallStaticHook ( "OnLootEntity", __instance, a0 );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}