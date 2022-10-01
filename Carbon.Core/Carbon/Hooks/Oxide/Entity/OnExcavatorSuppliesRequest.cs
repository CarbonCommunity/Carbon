///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Hooks.Oxide.Entity
{
	[OxideHook("OnExcavatorSuppliesRequest", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("this", typeof(ExcavatorSignalComputer))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when a player requests a supply drop from the signal computer.")]
	[OxideHook.Info("Best used to intercept the action.")]
	[OxideHook.Patch(typeof(ExcavatorSignalComputer), "RequestSupplies")]
	public class ExcavatorSignalComputer_RequestSupplies
	{
		public static bool Prefix (BaseEntity.RPCMessage rpc, ref ExcavatorSignalComputer __instance)
		{
			if (__instance.HasFlag(BaseEntity.Flags.Reserved7) && __instance.IsPowered() && __instance.chargePower >= ExcavatorSignalComputer.chargeNeededForSupplies)
			{
				if (Interface.CallHook("OnExcavatorSuppliesRequest", __instance, rpc.player) != null)
				{
					return false;
				}
			}
			return true;
		}
	}
}
