///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("CanLootEntity", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(StorageContainer))]
	[OxideHook.Info("Called when the player starts looting a DroppedItemContainer, LootableCorpse, ResourceContainer, BaseRidableAnimal, or StorageContainer entity.")]
	[OxideHook.Patch(typeof(StorageContainer), "PlayerOpenLoot")]
	public class StorageContainer_PlayerOpenLoot
	{
		public static bool Prefix(BasePlayer player, string panelToOpen, bool doPositionChecks, ref StorageContainer __instance, out bool __result)
		{
			__result = default;

			var result = Interface.CallHook("CanLootEntity", player, __instance);

			if (result is bool boolResult)
			{
				__result = boolResult;
				return false;
			}

			return true;
		}
	}

	[OxideHook("CanLootEntity", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(ContainerIOEntity))]
	[OxideHook.Info("Called when the player starts looting a DroppedItemContainer, LootableCorpse, ResourceContainer, BaseRidableAnimal, or StorageContainer entity.")]
	[OxideHook.Patch(typeof(ContainerIOEntity), "PlayerOpenLoot")]
	public class ContainerIOEntity_PlayerOpenLoot
	{
		public static bool Prefix(BasePlayer player, string panelToOpen, bool doPositionChecks, ref ContainerIOEntity __instance, out bool __result)
		{
			__result = default;

			var result = Interface.CallHook("CanLootEntity", player, __instance);

			if (result is bool boolResult)
			{
				__result = boolResult;
				return false;
			}

			return true;
		}
	}

	[OxideHook("CanLootEntity", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(LootableCorpse))]
	[OxideHook.Info("Called when the player starts looting a DroppedItemContainer, LootableCorpse, ResourceContainer, BaseRidableAnimal, or StorageContainer entity.")]
	[OxideHook.Patch(typeof(LootableCorpse), "RPC_LootCorpse")]
	public class LootableCorpse_RPC_LootCorpse
	{
		public static bool Prefix(BaseEntity.RPCMessage rpc, ref LootableCorpse __instance)
		{
			if (!rpc.player || !rpc.player.CanInteract())
			{
				return false;
			}
			if (!__instance.CanLoot())
			{
				return false;
			}
			if (__instance.containers == null)
			{
				return false;
			}

			return Interface.CallHook("CanLootEntity", rpc.player, __instance) == null;
		}
	}

	[OxideHook("CanLootEntity", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(DroppedItemContainer))]
	[OxideHook.Info("Called when the player starts looting a DroppedItemContainer, LootableCorpse, ResourceContainer, BaseRidableAnimal, or StorageContainer entity.")]
	[OxideHook.Patch(typeof(DroppedItemContainer), "RPC_OpenLoot")]
	public class DroppedItemContainer_RPC_OpenLoot
	{
		public static bool Prefix(BaseEntity.RPCMessage rpc, ref DroppedItemContainer __instance)
		{
			if (__instance.inventory == null)
			{
				return false;
			}
			if (!rpc.player || !rpc.player.CanInteract())
			{
				return false;
			}

			return Interface.CallHook("CanLootEntity", rpc.player, __instance) == null;
		}
	}

	[OxideHook("CanLootEntity", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(BaseRidableAnimal))]
	[OxideHook.Info("Called when the player starts looting a DroppedItemContainer, LootableCorpse, ResourceContainer, BaseRidableAnimal, or StorageContainer entity.")]
	[OxideHook.Patch(typeof(BaseRidableAnimal), "RPC_OpenLoot")]
	public class BaseRidableAnimal_RPC_OpenLoot
	{
		public static bool Prefix(BaseEntity.RPCMessage rpc, ref BaseRidableAnimal __instance)
		{
			if (__instance.inventory == null)
			{
				return false;
			}
			if (!rpc.player || !rpc.player.CanInteract())
			{
				return false;
			}
			if (!__instance.CanOpenStorage(rpc.player))
			{
				return false;
			}
			if (__instance.needsBuildingPrivilegeToUse && !rpc.player.CanBuild())
			{
				return false;
			}

			return Interface.CallHook("CanLootEntity", rpc.player, __instance) == null;
		}
	}

	[OxideHook("CanLootEntity", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(ResourceContainer))]
	[OxideHook.Info("Called when the player starts looting a DroppedItemContainer, LootableCorpse, ResourceContainer, BaseRidableAnimal, or StorageContainer entity.")]
	[OxideHook.Patch(typeof(ResourceContainer), "StartLootingContainer")]
	public class ResourceContainer_StartLootingContainer
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref ResourceContainer __instance)
		{
			if (!msg.player || !msg.player.CanInteract())
			{
				return false;
			}
			if (!__instance.lootable)
			{
				return false;
			}

			return Interface.CallHook("CanLootEntity", msg.player, __instance) == null;
		}
	}
}
