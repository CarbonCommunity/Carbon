///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [OxideHook ( "CanPickupEntity", typeof ( object ) ), OxideHook.Category ( OxideHook.Category.Enum.Player )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "this", typeof ( BaseCombatEntity ) )]
    [OxideHook.Info ( "Called when a player attempts to pickup a deployed entity (AutoTurret, BaseMountable, BearTrap, DecorDeployable, Door, DoorCloser, ReactiveTarget, SamSite, SleepingBag, SpinnerWheel, StorageContainer, etc.)." )]
    [OxideHook.Patch ( typeof ( BaseCombatEntity ), "CanPickup" )]
    public class BaseCombatEntity_CanPickup
    {
        public static bool Prefix ( BasePlayer player, ref bool __result, ref BaseCombatEntity __instance )
        {
            var result = HookExecutor.CallStaticHook ( "CanPickupEntity", player, __instance );

            if ( result is bool value )
            {
                __result = value;
                return false;
            }

            return true;
        }
    }
}