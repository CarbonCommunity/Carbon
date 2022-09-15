using Carbon.Core;
using Harmony;
using Oxide.Core;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( MedicalTool ), "GiveEffectsTo" )]
    public class MedicalTool_GiveEffectsTo
    {
        public static bool Prefix ( BasePlayer player, ref MedicalTool __instance )
        {
            if ( player == null ) return true;

            var component = __instance.GetOwnerItemDefinition ().GetComponent<ItemModConsumable> ();

            if ( component != null )
            {
                CarbonCore.Error ( $"No consumable for medicaltool :{__instance.name}" );
                return true;
            }

            if ( Interface.CallHook ( "OnHealingItemUse", __instance, player ) != null )
            {
                return false;
            }

            var ownerPlayer = __instance.GetOwnerPlayer ();

            if ( player != ownerPlayer && player.IsWounded () && __instance.canRevive )
            {
                return Interface.CallHook ( "OnPlayerRevive", __instance.GetOwnerPlayer (), player ) == null;
            }

            return true;
        }
    }
}