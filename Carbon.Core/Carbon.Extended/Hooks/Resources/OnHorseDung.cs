using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "OnHorseDung" ), Hook.Category ( Hook.Category.Enum.Entity )]
    [Hook.Parameter ( "this", typeof ( GrowableEntity ) )]
    [Hook.Parameter ( "item", typeof ( Item ) )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Info ( "Called before the player receives an item from gathering a growable entity." )]
    [HarmonyPatch ( typeof ( BaseRidableAnimal ), "DoDung" )]
    public class GrowableEntity_DoDung
    {
        public static bool Prefix ( ref BaseRidableAnimal __instance )
        {
            var result = Interface.CallHook ( "OnHorseDung", __instance );
            var dungItem = ( Item )null;

            if ( result is Item )
            {
                dungItem = result as Item;
            }

            if ( dungItem == null ) dungItem = ItemManager.Create ( __instance.Dung, 1, 0uL );

            __instance.dungProduction -= 1f;
            dungItem.Drop ( __instance.transform.position + -__instance.transform.forward + Vector3.up * 1.1f + UnityEngine.Random.insideUnitSphere * 0.1f, -__instance.transform.forward );
            return false;
        }
    }
}