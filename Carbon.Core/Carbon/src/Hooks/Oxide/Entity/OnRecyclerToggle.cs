///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnRecyclerToggle", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("entity", typeof(Recycler))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when a recycler is turned on or off.")]
	[OxideHook.Patch(typeof(Recycler), "SVSwitch")]
	public class Recycler_SVSwitch
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref Recycler __instance)
		{
			var oldPosition = msg.read.Position;
			var flag = msg.read.Bit();

			if (flag == __instance.IsOn())
			{
				return false;
			}
			if (msg.player == null)
			{
				return false;
			}
			if (Interface.CallHook("OnRecyclerToggle", __instance, msg.player) != null)
			{
				return false;
			}

			msg.read.Position = oldPosition;
			return true;
		}
	}
}
