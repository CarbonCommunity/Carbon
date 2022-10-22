///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnHorseLead", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("animal", typeof(BaseRidableAnimal))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when a player tries to lead a horse.")]
	[OxideHook.Patch(typeof(BaseRidableAnimal), "RPC_Lead")]
	public class BaseRidableAnimal_RPC_Lead
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref BaseRidableAnimal __instance)
		{
			var player = msg.player;

			if (player == null)
			{
				return false;
			}
			if (__instance.HasDriver())
			{
				return false;
			}
			if (__instance.IsForSale())
			{
				return false;
			}
			var flag = __instance.IsLeading();
			var flag2 = msg.read.Bit();

			if (flag == flag2)
			{
				return false;
			}

			if (Interface.CallHook("OnHorseLead", __instance, player) != null)
			{
				return false;
			}

			__instance.SetLeading(flag2 ? player : null);
			__instance.LeadingChanged();
			return false;
		}
	}
}
