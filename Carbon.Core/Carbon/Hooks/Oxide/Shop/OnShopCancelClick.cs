///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("OnShopCancelClick", typeof(object)), OxideHook.Category(Hook.Category.Enum.Shop)]
	[OxideHook.Parameter("this", typeof(ShopFront))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when a player is cancelling a trade")]
	[OxideHook.Info("Return a non-null value to override default behavior")]
	[OxideHook.Patch(typeof(ShopFront), "CancelClicked")]
	public class ShopFront_CancelClicked
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref ShopFront __instance)
		{
			if (!__instance.IsTradingPlayer(msg.player))
			{
				return false;
			}

			return HookExecutor.CallStaticHook("OnShopCancelClick", __instance, msg.player) == null;
		}
	}
}
