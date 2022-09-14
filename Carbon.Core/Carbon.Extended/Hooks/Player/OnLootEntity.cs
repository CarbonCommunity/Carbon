using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerLoot ), "StartLootingEntity" )]
    public class PlayerLoot_StartLootingEntity
    {
        public static void Postfix ( BaseEntity targetEntity, bool doPositionChecks, ref bool __result, ref PlayerLoot __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntity", __instance.GetComponent<BasePlayer>(), targetEntity );
        }
    }
}
