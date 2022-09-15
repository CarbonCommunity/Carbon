using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [Hook ( "OnLootEntity" ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "targetEntity", typeof ( BaseEntity ) )]
    [Hook.Info ( "Called when the player starts looting an entity." )]
    [HarmonyPatch ( typeof ( PlayerLoot ), "StartLootingEntity" )]
    public class PlayerLoot_StartLootingEntity
    {
        public static void Postfix ( BaseEntity targetEntity, bool doPositionChecks, ref bool __result, ref PlayerLoot __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntity", __instance.GetComponent<BasePlayer>(), targetEntity );
        }
    }
}
