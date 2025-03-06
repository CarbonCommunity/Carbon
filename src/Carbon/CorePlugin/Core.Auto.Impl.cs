namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
#if !MINIMAL

	#region Implementation

	[Conditional("!MINIMAL")]
	internal object IRecyclerThinkSpeed(Recycler recycler)
	{
		if (recycler.IsSafezoneRecycler())
		{
			if (SafezoneRecycleTickMultiplier != -1)
			{
				return SafezoneRecycleTickMultiplier;
			}

			return null;
		}

		if (RecycleTickMultiplier != -1)
		{
			return RecycleTickMultiplier;
		}

		return null;
	}

	[Conditional("!MINIMAL")]
	internal object ICraftDurationMultiplier(ItemBlueprint bp, float workbenchLevel, bool isInTutorial)
	{
		if (isInTutorial)
		{
			return null;
		}

		var workbench = workbenchLevel - bp.workbenchLevelRequired;

		return workbench switch
		{
			0 when CraftingSpeedMultiplierNoWB != -1 => CraftingSpeedMultiplierNoWB,
			1 when CraftingSpeedMultiplierWB1 != -1 => CraftingSpeedMultiplierWB1,
			2 when CraftingSpeedMultiplierWB2 != -1 => CraftingSpeedMultiplierWB2,
			3 when CraftingSpeedMultiplierWB3 != -1 => CraftingSpeedMultiplierWB3,
			_ => null
		};
	}

	[Conditional("!MINIMAL")]
	internal object IMixingSpeedMultiplier(MixingTable table, float originalValue)
	{
		if (MixingSpeedMultiplier == -1 || table.currentRecipe == null)
		{
			return null;
		}

		if (originalValue == table.currentRecipe.MixingDuration * table.currentQuantity)
		{
			return MixingSpeedMultiplier;
		}

		return null;
	}

	[Conditional("!MINIMAL")]
	internal object IVendingBuyDuration()
	{
		if (VendingMachineBuyDurationMultiplier != -1)
		{
			return VendingMachineBuyDurationMultiplier;
		}

		return null;
	}

	[Conditional("!MINIMAL")]
	internal void IOnExcavatorInit(ExcavatorArm arm)
	{
		if (ExcavatorResourceTickRateMultiplier != -1)
		{
			arm.resourceProductionTickRate *= ExcavatorResourceTickRateMultiplier;
		}

		if (ExcavatorTimeForFullResourcesMultiplier != -1)
		{
			arm.timeForFullResources *= ExcavatorTimeForFullResourcesMultiplier;
		}

		if (ExcavatorBeltSpeedMaxMultiplier != -1)
		{
			arm.beltSpeedMax *= ExcavatorBeltSpeedMaxMultiplier;
		}
	}

	[Conditional("!MINIMAL")]
	internal object IOvenSmeltSpeedMultiplier(BaseOven oven)
	{
		if (OvenBlacklistCache == null)
		{
			return null;
		}

		if (Enumerable.Contains(OvenBlacklistCache, oven.ShortPrefabName) ||
		    Enumerable.Contains(OvenBlacklistCache, oven.GetType().Name))
		{
			if (OvenBlacklistSpeedMultiplier != -1)
			{
				return OvenBlacklistSpeedMultiplier;
			}

			return null;
		}

		if (OvenSpeedMultiplier != -1)
		{
			return OvenSpeedMultiplier;
		}

		return null;
	}

	[Conditional("!MINIMAL")]
	private void IResearchDuration() { }

	[Conditional("!MINIMAL")]
	private object CanUnlockTechTreeNode()
	{
		if (NoTechTreeUnlockCache)
		{
			return false;
		}

		return null;
	}

	#endregion

#endif
}
