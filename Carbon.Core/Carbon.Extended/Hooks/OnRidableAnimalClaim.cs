using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseRidableAnimal ), "RPC_Claim" )]
    public class OnRidableAnimalClaim
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnRidableAnimalClaim" );
        }
    }
}