///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
    [OxideHook ( "OnLootEntityEnd" ), OxideHook.Category ( Hook.Category.Enum.Player )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "this", typeof ( LootableCorpse ) )]
    [OxideHook.Info ( "Called when the player stops looting an entity." )]
    [OxideHook.Patch ( typeof ( LootableCorpse ), "PlayerStoppedLooting" )]
    public class LootableCorpse_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref LootableCorpse __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }

    [OxideHook ( "OnLootEntityEnd" ), OxideHook.Category ( Hook.Category.Enum.Player )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "this", typeof ( StorageContainer ) )]
    [OxideHook.Info ( "Called when the player stops looting an entity." )]
    [OxideHook.Patch ( typeof ( StorageContainer ), "PlayerStoppedLooting" )]
    public class StorageContainer_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref StorageContainer __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }

    [OxideHook ( "OnLootEntityEnd" ), OxideHook.Category ( Hook.Category.Enum.Player )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "this", typeof ( ItemBasedFlowRestrictor ) )]
    [OxideHook.Info ( "Called when the player stops looting an entity." )]
    [OxideHook.Patch ( typeof ( ItemBasedFlowRestrictor ), "PlayerStoppedLooting" )]
    public class ItemBasedFlowRestrictor_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref ItemBasedFlowRestrictor __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }

    [OxideHook ( "OnLootEntityEnd" ), OxideHook.Category ( Hook.Category.Enum.Player )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "this", typeof ( DroppedItemContainer ) )]
    [OxideHook.Info ( "Called when the player stops looting an entity." )]
    [OxideHook.Patch ( typeof ( DroppedItemContainer ), "PlayerStoppedLooting" )]
    public class DroppedItemContainer_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref DroppedItemContainer __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }

    [OxideHook ( "OnLootEntityEnd" ), OxideHook.Category ( Hook.Category.Enum.Player )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "this", typeof ( ContainerIOEntity ) )]
    [OxideHook.Info ( "Called when the player stops looting an entity." )]
    [OxideHook.Patch ( typeof ( ContainerIOEntity ), "PlayerStoppedLooting" )]
    public class ContainerIOEntity_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref ContainerIOEntity __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }
}
