#if !MINIMAL

using API.Hooks;
using UnityEngine;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Fixes
{
	public partial class Fixes_ItemCrafter
	{
		[HookAttribute.Patch("IOvenSmeltSpeedMultiplier", "IOvenSmeltSpeedMultiplier", typeof(BaseOven.BaseOvenWorkQueue), nameof(BaseOven.BaseOvenWorkQueue.RunJob), [typeof(BaseOven)])]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class IOvenSmeltSpeedMultiplier : Patch
		{
			public static bool Prefix(BaseOven oven)
			{
				if (Community.Runtime.Core.IOvenSmeltSpeedMultiplier(oven) is not float speedMultiplier)
				{
					return true;
				}
				if (oven.lastCookUpdate > BaseOven.UpdateRate * speedMultiplier)
				{
					var num = Mathf.Floor(oven.lastCookUpdate / BaseOven.UpdateRate);
					oven.lastCookUpdate -= num * (BaseOven.UpdateRate * speedMultiplier);
					oven.Cook(BaseOven.UpdateRate * num);
				}
				if (oven.visualFood && oven.lastCookVisualsUpdate > BaseOven.VisualUpdateRate)
				{
					oven.lastCookVisualsUpdate = 0f;
					oven.CookVisuals();
				}

				return false;
			}
		}
	}
}

#endif
