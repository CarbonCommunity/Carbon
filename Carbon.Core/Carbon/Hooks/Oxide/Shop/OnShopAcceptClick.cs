///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnShopAcceptClick", typeof(object)), OxideHook.Category(Hook.Category.Enum.Shop)]
	[OxideHook.Parameter("this", typeof(ShopFront))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when a player is trying to accept a trade in ShopFront")]
	[OxideHook.Patch(typeof(ShopFront), "AcceptClicked")]
	public class ShopFront_AcceptClicked
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref ShopFront __instance)
		{
			if (!__instance.IsTradingPlayer(msg.player))
			{
				return false;
			}

			if (__instance.vendorPlayer == null || __instance.customerPlayer == null)
			{
				return false;
			}

			return HookExecutor.CallStaticHook("OnShopAcceptClick", __instance, msg.player) == null;
		}
	}
}
