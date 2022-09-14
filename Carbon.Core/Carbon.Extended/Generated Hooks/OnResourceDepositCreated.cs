using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResourceDepositManager ), "CreateFromPosition" )]
    public class OnResourceDepositCreated
    {
        public static bool Postfix ( UnityEngine.Vector3 pos, ref ResourceDepositManager+ResourceDeposit __result )
        {
            CarbonCore.Log ( $"Postfix OnResourceDepositCreated" );

            var result = HookExecutor.CallStaticHook ( "OnResourceDepositCreated", l1 );
            
            if ( result != null )
            {
                __result = ( ResourceDepositManager+ResourceDeposit ) result;
                return false;
            }

            return true;
        }
    }
}