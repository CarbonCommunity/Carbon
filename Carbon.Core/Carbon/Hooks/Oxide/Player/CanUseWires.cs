///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Oxide.Core;

namespace Carbon.Extended
{
	[OxideHook("CanUseWires", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Useful for allowing or preventing a player from using wires.")]
	[OxideHook.Patch(typeof(WireTool), "CanPlayerUseWires")]
	public class WireTool_CanPlayerUseWires
	{
		public static bool Prefix(BasePlayer player)
		{
			return Interface.CallHook("CanUseWires", player) == null;
		}
	}
}
