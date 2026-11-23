using HarmonyLib;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
#if !MINIMAL

	/// <summary>
	/// Hooks that only exist to back Carbon Auto variables, mapped to whether any of their variables are non-vanilla.
	/// Core only subscribes to these while needed, so vanilla values leave the game methods unpatched.
	/// </summary>
	private static readonly Dictionary<string, Func<CorePlugin, bool>> AutoHooks = new()
	{
		[nameof(IRecyclerThinkSpeed)] = core => core.RecycleTickMultiplier != -1 || core.SafezoneRecycleTickMultiplier != -1,
		[nameof(ICraftDurationMultiplier)] = core => core.CraftingSpeedMultiplierNoWB != -1 || core.CraftingSpeedMultiplierWB1 != -1 ||
		                                             core.CraftingSpeedMultiplierWB2 != -1 || core.CraftingSpeedMultiplierWB3 != -1,
		[nameof(IMixingSpeedMultiplier)] = core => core.MixingSpeedMultiplier != -1,
		[nameof(IVendingBuyDuration)] = core => core.VendingMachineBuyDurationMultiplier != -1,
		[nameof(IOnExcavatorInit)] = core => core.ExcavatorResourceTickRateMultiplier != -1 || core.ExcavatorTimeForFullResourcesMultiplier != -1 ||
		                                     core.ExcavatorBeltSpeedMaxMultiplier != -1,
		[nameof(IOvenSmeltSpeedMultiplier)] = core => core.OvenSpeedMultiplier != -1 || core.OvenBlacklistSpeedMultiplier != -1,
		[nameof(IResearchDuration)] = core => core.ResearchDurationMultiplier != -1,
		[nameof(CanUnlockTechTreeNode)] = core => core.NoTechTreeUnlockCache
	};

	internal static bool IsAutoHook(string hookName)
	{
		return AutoHooks.ContainsKey(hookName);
	}

	/// <summary>
	/// Subscribes or unsubscribes Core from the Carbon Auto hooks based on the current variable values.
	/// </summary>
	public void RefreshAutoHooks()
	{
		var hookManager = Community.Runtime?.HookManager;

		if (hookManager == null)
		{
			return;
		}

		foreach (var autoHook in AutoHooks)
		{
			if (autoHook.Value(this))
			{
				hookManager.Subscribe(autoHook.Key, Name);
			}
			else
			{
				hookManager.Unsubscribe(autoHook.Key, Name);
			}
		}
	}

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
		if (OvenSpeedMultiplier == -1 && OvenBlacklistSpeedMultiplier == -1)
		{
			return null;
		}

		if (!_ovenBlacklistLookup.TryGetValue(oven.prefabID, out var isBlacklisted))
		{
			isBlacklisted = IsOvenBlacklisted(OvenBlacklistCache, oven.ShortPrefabName);
			_ovenBlacklistLookup[oven.prefabID] = isBlacklisted;
		}

		if (isBlacklisted)
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

	private static bool IsOvenBlacklisted(List<string> cache, string shortPrefabName)
	{
		if (cache == null)
		{
			return false;
		}

		for (int i = 0; i < cache.Count; i++)
		{
			if (shortPrefabName.Contains(cache[i], CompareOptions.IgnoreCase))
			{
				return true;
			}
		}

		return false;
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

	[AutoPatch(Silent = true), HarmonyPatch]
	public class GivePatch
	{
		public static List<BasePlayer> giverPlayers = new();

		public static IEnumerable<MethodBase> TargetMethods()
		{
			var parameters = new[] { typeof(ConsoleSystem.Arg) };
			yield return AccessTools.Method(typeof(ConVar.Inventory), nameof(ConVar.Inventory.give), parameters);
			yield return AccessTools.Method(typeof(ConVar.Inventory), nameof(ConVar.Inventory.giveall), parameters);
			yield return AccessTools.Method(typeof(ConVar.Inventory), nameof(ConVar.Inventory.givearm), parameters);
			yield return AccessTools.Method(typeof(ConVar.Inventory), nameof(ConVar.Inventory.giveBp), parameters);
			yield return AccessTools.Method(typeof(ConVar.Inventory), nameof(ConVar.Inventory.giveid), parameters);
			yield return AccessTools.Method(typeof(ConVar.Inventory), nameof(ConVar.Inventory.giveto), parameters);
		}

		public static void Prefix(ConsoleSystem.Arg arg)
		{
			if (!Community.Runtime.Core.NoGiveNoticesCache)
			{
				return;
			}

			var player = arg.Player();
			if (player != null && !giverPlayers.Contains(player))
			{
				giverPlayers.Add(player);
			}
		}
	}

	[AutoPatch(Silent = true), HarmonyPatch(typeof(Item), nameof(Item.SetItemOwnership), typeof(BasePlayer), typeof(Translate.Phrase))]
	public class OwnershipPatch
	{
		public static bool Prefix(BasePlayer player, Translate.Phrase reason)
		{
			if (GivePatch.giverPlayers.Contains(player))
			{
				GivePatch.giverPlayers.Remove(player);
				return false;
			}

			return true;
		}
	}

#endif
}
