///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Extended
{
	[OxideHook("OnItemRepair", typeof(object)), OxideHook.Category(Hook.Category.Enum.Item)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("itemToRepair", typeof(Item))]
	[OxideHook.Info("Called right before an item is repaired.")]
	[OxideHook.Patch(typeof(RepairBench), "RepairAnItem")]
	public class RepairBench_RepairAnItem
	{
		public static bool Prefix(Item itemToRepair, BasePlayer player, BaseEntity repairBenchEntity, float maxConditionLostOnRepair, bool mustKnowBlueprint)
		{
			if (itemToRepair == null)
			{
				return false;
			}

			var info = itemToRepair.info;
			var component = info.GetComponent<ItemBlueprint>();

			if (!component)
			{
				return false;
			}
			if (!info.condition.repairable)
			{
				return false;
			}
			if (itemToRepair.condition == itemToRepair.maxCondition)
			{
				return false;
			}
			if (mustKnowBlueprint)
			{
				var itemDefinition = (info.isRedirectOf != null) ? info.isRedirectOf : info;

				if (!player.blueprints.HasUnlocked(itemDefinition) && (!(itemDefinition.Blueprint != null) || itemDefinition.Blueprint.isResearchable))
				{
					return false;
				}
			}

			return Interface.CallHook("OnItemRepair", player, itemToRepair) == null;
		}
	}
}
