///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [OxideHook ( "CanAffordUpgrade", typeof ( bool ) ), OxideHook.Category ( OxideHook.Category.Enum.Player )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "this", typeof ( BuildingBlock ) )]
    [OxideHook.Parameter ( "grade", typeof ( BuildingGrade.Enum ) )]
    [OxideHook.Info ( "Called when the resources for an upgrade are checked." )]
    [OxideHook.Patch ( typeof ( BuildingBlock ), "CanAffordUpgrade" )]
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