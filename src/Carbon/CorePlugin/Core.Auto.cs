using Newtonsoft.Json;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
#if !MINIMAL
	[CommandVar("isforcemodded", help: "Is the server forcefully set to modded due to options affecting significant gameplay changes in Carbon Auto?")]
	[AuthLevel(2)]
	public bool IsForceModded { get { return CarbonAuto.Singleton.IsForceModded(); } set { } }

	#region Modded

	[CarbonAutoModdedVar("recycletickmultiplier", "Recycle Tick (*)", help: "Configures the recycling ticks multiplier base speed relative.")]
	[AuthLevel(2)]
	public float RecycleTickMultiplier = -1;

	[CarbonAutoModdedVar("safezonerecycletickmultiplier", "Recycle Tick (Safezone) (*)", help: "Configures the SafeZone recycling ticks multiplier base speed relative.")]
	[AuthLevel(2)]
	public float SafezoneRecycleTickMultiplier = -1;

	public float RuntimeResearchDurationMultiplier => ResearchDurationMultiplier == -1 ? 1 : ResearchDurationMultiplier;

	[CarbonAutoModdedVar("researchdurationmultiplier", "Researching Duration (*)", help: "The duration multiplier of blueprint researching finalization time.")]
	[AuthLevel(2)]
	public float ResearchDurationMultiplier = -1;

	[CarbonAutoModdedVar("vendingbuydurationmultiplier", "Vending Buy Duration (*)", help: "The duration multiplier of transaction delay when buying from vending machines.")]
	[AuthLevel(2)]
	public float VendingMachineBuyDurationMultiplier = -1;

	[CarbonAutoModdedVar("craftingspeedmultiplier_nowb", "Crafting Speed - No WB (*)", help: "The time multiplier of crafting items without a workbench.")]
	[AuthLevel(2)]
	public float CraftingSpeedMultiplierNoWB = -1;

	[CarbonAutoModdedVar("craftingspeedmultiplier_wb1", "Crafting Speed - WB 1 (*)", help: "The time multiplier of crafting items at workbench level 1.")]
	[AuthLevel(2)]
	public float CraftingSpeedMultiplierWB1 = -1;

	[CarbonAutoModdedVar("craftingspeedmultiplier_wb2", "Crafting Speed - WB 2 (*)", help: "The time multiplier of crafting items at workbench level 2.")]
	[AuthLevel(2)]
	public float CraftingSpeedMultiplierWB2 = -1;

	[CarbonAutoModdedVar("craftingspeedmultiplier_wb3", "Crafting Speed - WB 3 (*)", help: "The time multiplier of crafting items at workbench level 3.")]
	[AuthLevel(2)]
	public float CraftingSpeedMultiplierWB3 = -1;

	[CarbonAutoModdedVar("mixingspeedmultiplier", "Mixing Speed (*)", help: "The speed multiplier of mixing table crafts.")]
	[AuthLevel(2)]
	public float MixingSpeedMultiplier = -1;

	[CarbonAutoModdedVar("exacavatorresourcetickratemultiplier", "Excavator Resource Rate (*)", help: "Excavator resource tick multiplier rate.")]
	[AuthLevel(2)]
	public float ExcavatorResourceTickRateMultiplier = -1;

	[CarbonAutoModdedVar("excavatortimeforfullresourcesmultiplier", "Excavator Full Resources Time (*)", help: "Excavator time multiplier for processing full resources.")]
	[AuthLevel(2)]
	public float ExcavatorTimeForFullResourcesMultiplier = -1;

	[CarbonAutoModdedVar("excavatorbeltspeedmaxmultiplier", "Excavator Belt Max. Speed (*)", help: "Excavator belt maximum speed multiplier.")]
	[AuthLevel(2)]
	public float ExcavatorBeltSpeedMaxMultiplier = -1;

	public IEnumerable<string> OvenBlacklistCache;

	private string _ovenBlacklist = "furnace,bbq.static,furnace.large";

	[CarbonAutoVar("ovenblacklist", "Oven Blacklist", help: "Blacklisted oven entity prefabs.")]
	[AuthLevel(2)]
	public string OvenBlacklist
	{
		get => _ovenBlacklist;
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				OvenBlacklistCache = default;
				return;
			}

			if (_ovenBlacklist != value || OvenBlacklistCache == null)
			{
				OvenBlacklistCache = value.SplitEnumerable(',');
			}

			_ovenBlacklist = value;
		}
	}

	[CarbonAutoModdedVar("ovenspeedmultiplier", "Oven Speed (*)", help: "The burning speed multiplier of ovens.")]
	[AuthLevel(2)]
	public float OvenSpeedMultiplier = -1;

	[CarbonAutoModdedVar("ovenblacklistspeedmultiplier", "Oven Blacklist Speed Duration (*)", help: "The burning speed multiplier of blacklisted ovens.")]
	[AuthLevel(2)]
	public float OvenBlacklistSpeedMultiplier = -1;

	[CarbonAutoModdedVar("oventemperaturemultiplier", "Oven Temperature (*)", help: "The burning temperature multiplier of ovens.")]
	[AuthLevel(2)]
	public float OvenTemperatureMultiplier = -1;

	[CarbonAutoModdedVar("ovenblacklisttemperaturemultiplier", "Oven Blacklist Temperature Duration (*)", help: "The burning temperature multiplier of blacklisted ovens.")]
	[AuthLevel(2)]
	public float OvenBlacklistTemperatureMultiplier = -1;

	[CarbonAutoVar("notechtreeunlock", "No TechTree Unlocks", help: "Players will no longer be able to progress on any tech trees.")]
	[AuthLevel(2)]
	public string NoTechTreeUnlock
	{
		get => NoTechTreeUnlockCache ? "1" : "-1";
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				NoTechTreeUnlockCache = default;
				return;
			}

			NoTechTreeUnlockCache = value.ToBool(false);
		}
	}
	public bool NoTechTreeUnlockCache;

	#endregion

	#region Vanilla

	[CarbonAutoVar("nogivenotices", "No 'Give' Notices", help: "Will prohibit 'gave' messages to be printed to chat when admins give items.")]
	[AuthLevel(2)]
	public string NoGiveNotices
	{
		get => NoGiveNoticesCache ? "1" : "-1";
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				NoGiveNoticesCache = default;
				return;
			}

			NoGiveNoticesCache = value.ToBool(false);
		}
	}
	public bool NoGiveNoticesCache;

	private string _customMapName = "-1";

	[CarbonAutoVar("defaultserverchatname", "Server Chat Name", help: "Default server chat name.")]
	[AuthLevel(2)]
	public string DefaultServerChatName = "-1";

	[CarbonAutoVar("defaultserverchatcolor", "Server Chat Color", help: "Default server chat message name color.")]
	[AuthLevel(2)]
	public string DefaultServerChatColor = "-1";

	[CarbonAutoVar("defaultserverchatid", "Server Icon ID", help: "Default server chat icon SteamID.")]
	[AuthLevel(2)]
	public long DefaultServerChatId = -1;

	[CarbonAutoVar("custommapname", "Custom Map Name", help: "The map name displayed in the Rust server browser. Shouldn't be longer than 64 characters.")]
	[AuthLevel(2)]
	public string CustomMapName { get { return _customMapName; } set { _customMapName = Carbon.Extensions.StringEx.Truncate(value, 64); } }

	#endregion
#endif
}
