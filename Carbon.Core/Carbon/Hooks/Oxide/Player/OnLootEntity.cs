///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [OxideHook ( "OnLootEntity", typeof ( object ) ), OxideHook.Category ( OxideHook.Category.Enum.Player )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "targetEntity", typeof ( BaseEntity ) )]
    [OxideHook.Info ( "Called when the player starts looting an entity." )]
    [OxideHook.Patch ( typeof ( PlayerLoot ), "StartLootingEntity" )]
    public class PlayerLoot_StartLootingEntity
    {
        public static void Postfix ( BaseEntity targetEntity, bool doPositionChecks, ref bool __result, ref PlayerLoot __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntity", __instance.GetComponent<BasePlayer>(), targetEntity );
        }
    }
}
