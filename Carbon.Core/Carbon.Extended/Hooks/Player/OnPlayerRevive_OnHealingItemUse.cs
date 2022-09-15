using Carbon.Core;
using Harmony;
using Oxide.Core;

namespace Carbon.Extended
{
    [Hook ( "OnPlayerRevive", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "target", typeof ( BasePlayer ) )]
    [Hook.Info ( "Called when the recover after reviving with a medical tool." )]
    [Hook.Info ( "Useful for canceling the reviving." )]
    [HarmonyPatch ( typeof ( MedicalTool ), "GiveEffectsTo" )]
    public class MedicalTool_GiveEffectsTo
    {
        public static bool Prefix ( BasePlayer player, ref MedicalTool __instance )
        {
            if ( player == null ) return true;

            var component = __instance.GetOwnerItemDefinition ().GetComponent<ItemModConsumable> ();

            if ( component != null )
            {
                return true;
            }

            var ownerPlayer = __instance.GetOwnerPlayer ();

            if ( player != ownerPlayer && player.IsWounded () && __instance.canRevive )
            {
                return Interface.CallHook ( "OnPlayerRevive", __instance.GetOwnerPlayer (), player ) == null;
            }

            return true;
        }
    }

    [Hook ( "OnHealingItemUse", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "this", typeof ( MedicalTool ) )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Info ( "Called when a player attempts to use a medical tool." )]
    [HarmonyPatch ( typeof ( MedicalTool ), "GiveEffectsTo" )]
    public class MedicalTool_GiveEffectsTo_OnHealingItemUse
    {
        public static bool Prefix ( BasePlayer player, ref MedicalTool __instance )
        {
            if ( player == null ) return true;

            var component = __instance.GetOwnerItemDefinition ().GetComponent<ItemModConsumable> ();

            if ( component != null )
            {
                return true;
            }

            if ( Interface.CallHook ( "OnHealingItemUse", __instance, player ) != null )
            {
                return false;
            }

            return true;
        }
    }
}