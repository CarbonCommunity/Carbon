using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [Hook ( "CanAffordUpgrade" ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "this", typeof ( BuildingBlock ) )]
    [Hook.Parameter ( "grade", typeof ( BuildingGrade.Enum ) )]
    [Hook.Info ( "Called when the resources for an upgrade are checked." )]
    [HarmonyPatch ( typeof ( BuildingBlock ), "CanAffordUpgrade" )]
    public class BuildingBlock_CanAffordUpgrade
    {
        public static bool Prefix ( BuildingGrade.Enum iGrade, BasePlayer player, ref BuildingBlock __instance, ref bool __result )
        {
            var result = HookExecutor.CallStaticHook ( "CanAffordUpgrade", player, __instance, iGrade );

            if ( result != null )
            {
                __result = ( bool )result;
            }

            return result == null;
        }
    }
}