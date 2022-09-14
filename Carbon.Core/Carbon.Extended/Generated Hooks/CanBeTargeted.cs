using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HelicopterTurret ), "InFiringArc" )]
    public class CanBeTargeted
    {
        public static bool Prefix ( BaseCombatEntity potentialtarget, ref System.Boolean __result, ref HelicopterTurret __instance )
        {
            CarbonCore.Log ( $"Prefix CanBeTargeted" );

            var result = HookExecutor.CallStaticHook ( "CanBeTargeted", a0, __instance );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}