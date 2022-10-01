///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Extended
{
	[OxideHook("OnLoseCondition", typeof(float)), OxideHook.Category(Hook.Category.Enum.Item)]
	[OxideHook.Parameter("item", typeof(Item))]
	[OxideHook.Parameter("amount", typeof(float))]
	[OxideHook.Info("Called right before the condition of the item is modified.")]
	[OxideHook.Info("Return a float value to override the amount.")]
	[OxideHook.Patch(typeof(Item), "LoseCondition")]
	public class Item_LoseCondition
	{
		public static bool Prefix (ref float amount, ref Item __instance)
		{
			if (!__instance.hasCondition || ConVar.Debugging.disablecondition) return false;

			var result = Interface.CallHook("OnLoseCondition", __instance, amount);

			if (result is float)
			{
				amount = (float)result;
			}

			return result == null;
		}
	}
}
