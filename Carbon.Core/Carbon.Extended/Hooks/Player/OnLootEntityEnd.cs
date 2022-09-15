using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [Hook ( "OnLootEntityEnd" ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "this", typeof ( LootableCorpse ) )]
    [Hook.Info ( "Called when the player stops looting an entity." )]
    [HarmonyPatch ( typeof ( LootableCorpse ), "PlayerStoppedLooting" )]
    public class LootableCorpse_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref LootableCorpse __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }

    [Hook ( "OnLootEntityEnd" ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "this", typeof ( StorageContainer ) )]
    [Hook.Info ( "Called when the player stops looting an entity." )
    [HarmonyPatch ( typeof ( StorageContainer ), "PlayerStoppedLooting" )]
    public class StorageContainer_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref StorageContainer __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }

    [Hook ( "OnLootEntityEnd" ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "this", typeof ( ItemBasedFlowRestrictor ) )]
    [Hook.Info ( "Called when the player stops looting an entity." )
    [HarmonyPatch ( typeof ( ItemBasedFlowRestrictor ), "PlayerStoppedLooting" )]
    public class ItemBasedFlowRestrictor_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref ItemBasedFlowRestrictor __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }

    [Hook ( "OnLootEntityEnd" ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "this", typeof ( DroppedItemContainer ) )]
    [Hook.Info ( "Called when the player stops looting an entity." )
    [HarmonyPatch ( typeof ( DroppedItemContainer ), "PlayerStoppedLooting" )]
    public class DroppedItemContainer_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref DroppedItemContainer __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }

    [Hook ( "OnLootEntityEnd" ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "this", typeof ( ContainerIOEntity ) )]
    [Hook.Info ( "Called when the player stops looting an entity." )
    [HarmonyPatch ( typeof ( ContainerIOEntity ), "PlayerStoppedLooting" )]
    public class ContainerIOEntity_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref ContainerIOEntity __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }
}
