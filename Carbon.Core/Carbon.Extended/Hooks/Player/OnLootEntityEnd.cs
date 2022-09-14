using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( LootableCorpse ), "PlayerStoppedLooting" )]
    public class LootableCorpse_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref LootableCorpse __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }

    [HarmonyPatch ( typeof ( StorageContainer ), "PlayerStoppedLooting" )]
    public class StorageContainer_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref StorageContainer __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }

    [HarmonyPatch ( typeof ( ItemBasedFlowRestrictor ), "PlayerStoppedLooting" )]
    public class ItemBasedFlowRestrictor_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref ItemBasedFlowRestrictor __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }

    [HarmonyPatch ( typeof ( DroppedItemContainer ), "PlayerStoppedLooting" )]
    public class DroppedItemContainer_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref DroppedItemContainer __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }

    [HarmonyPatch ( typeof ( ContainerIOEntity ), "PlayerStoppedLooting" )]
    public class ContainerIOEntity_PlayerStoppedLooting
    {
        public static void Prefix ( BasePlayer player, ref ContainerIOEntity __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd", player, __instance );
        }
    }
}
