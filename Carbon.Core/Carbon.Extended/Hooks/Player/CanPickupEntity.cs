using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [Hook ( "CanPickupEntity", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "this", typeof ( BaseCombatEntity ) )]
    [Hook.Info ( "alled when a player attempts to pickup a deployed entity (AutoTurret, BaseMountable, BearTrap, DecorDeployable, Door, DoorCloser, ReactiveTarget, SamSite, SleepingBag, SpinnerWheel, StorageContainer, etc.)." )]
    [HarmonyPatch ( typeof ( BaseCombatEntity ), "CanPickup" )]
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