using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResourceDispenser ), "GiveResourceFromItem" )]
    public class OnDispenserGather
    {
        public static void Postfix ( BaseEntity entity, ItemAmount itemAmt, System.Single gatherDamage, System.Single destroyFraction, AttackEntity attackWeapon , ref ResourceDispenser __instance )
        {
            HookExecutor.CallStaticHook ( "OnDispenserGather" );
        }
    }
}