///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnBuyVendingItem", typeof(object)), OxideHook.Category(Hook.Category.Enum.Vending)]
	[OxideHook.Parameter("machine", typeof(VendingMachine))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("sellOrderId", typeof(int))]
	[OxideHook.Parameter("numberOfTransactions", typeof(int))]
	[OxideHook.Info("Called when a player buys an item from a vending machine.")]
	[OxideHook.Patch(typeof(VendingMachine), "BuyItem")]
	public class VendingMachine_BuyItem
	{
		public static bool Prefix(BaseEntity.RPCMessage rpc, ref VendingMachine __instance)
		{
			if (!__instance.OccupiedCheck(rpc.player))
			{
				return false;
			}

			var num = rpc.read.Int32();
			var num2 = rpc.read.Int32();

			if (__instance.IsVending())
			{
				rpc.player.ShowToast(GameTip.Styles.Red_Normal, VendingMachine.WaitForVendingMessage);
				return false;
			}

			if (Interface.CallHook("OnBuyVendingItem", __instance, rpc.player, num, num2) != null)
			{
				return false;
			}

			__instance.SetPendingOrder(rpc.player, num, num2);
			__instance.Invoke(new Action(__instance.CompletePendingOrder), __instance.GetBuyDuration());
			return false;
		}
	}
}
