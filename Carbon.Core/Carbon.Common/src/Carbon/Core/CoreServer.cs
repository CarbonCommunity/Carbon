/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Core;
#pragma warning disable IDE0051

public partial class CorePlugin : CarbonPlugin
{
	[CommandVar("recycletick", saved: true)]
	[AuthLevel(2)]
	public float RecycleTick = -1;

	[CommandVar("researchduration", saved: true)]
	[AuthLevel(2)]
	public float ResearchDuration = -1;

	[CommandVar("vendingmachinebuyduration", saved: true)]
	[AuthLevel(2)]
	public float VendingMachineBuyDuration = -1;

	[CommandVar("craftingspeedmultiplier", saved: true)]
	[AuthLevel(2)]
	public float CraftingSpeedMultiplier = -1;

	[CommandVar("mixingspeedmultiplier", saved: true)]
	[AuthLevel(2)]
	public float MixingSpeedMultiplier = -1;

	[CommandVar("exacavatorresourcetickrate", saved: true)]
	[AuthLevel(2)]
	public float ExcavatorResourceTickRate = -1;

	[CommandVar("excavatortimeforfullresources", saved: true)]
	[AuthLevel(2)]
	public float ExcavatorTimeForFullResources = -1;

	[CommandVar("excavatorbeltspeedmax", saved: true)]
	[AuthLevel(2)]
	public float ExcavatorBeltSpeedMax = -1;

	#region Implementation

	private object IRecyclerThinkSpeed()
	{
		if (RecycleTick != -1) return RecycleTick;

		return null;
	}
	private object ICraftDurationMultiplier()
	{
		if (CraftingSpeedMultiplier != -1) return CraftingSpeedMultiplier;

		return null;
	}
	private object IMixingSpeedMultiplier(MixingTable table, float originalValue)
	{
		if (MixingSpeedMultiplier == -1 || table.currentRecipe == null) return null;

		if (originalValue == table.currentRecipe.MixingDuration * table.currentQuantity)
		{
			return MixingSpeedMultiplier;
		}

		return null;
	}
	private object IVendingBuyDuration()
	{
		if (VendingMachineBuyDuration != -1) return VendingMachineBuyDuration;

		return null; ;
	}

	private void OnItemResearch(ResearchTable table, Item targetItem, BasePlayer player)
	{
		table.researchDuration = ResearchDuration;
	}

	#endregion
}
