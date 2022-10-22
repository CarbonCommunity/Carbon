///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnHotAirBalloonToggle", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("balloon", typeof(HotAirBalloon))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when a player tries to toggle a hot air balloon on or off.")]
	[OxideHook.Patch(typeof(HotAirBalloon), "EngineSwitch")]
	public class HotAirBalloon_EngineSwitch_OnHotAirBalloonToggle
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref HotAirBalloon __instance)
		{
			if (Interface.CallHook("OnHotAirBalloonToggle", __instance, msg.player) != null)
			{
				return false;
			}

			var b = msg.read.Bit();
			__instance.SetFlag(BaseEntity.Flags.On, b, false, true);
			if (__instance.IsOn())
			{
				__instance.Invoke(__instance.ScheduleOff, 60f);
				Interface.CallHook("OnHotAirBalloonToggled", __instance, msg.player);
				return false;
			}

			__instance.CancelInvoke(__instance.ScheduleOff);
			Interface.CallHook("OnHotAirBalloonToggled", __instance, msg.player);
			return false;
		}
	}

	[OxideHook("OnHotAirBalloonToggled"), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Require("OnHotAirBalloonToggle")]
	[OxideHook.Parameter("balloon", typeof(HotAirBalloon))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called right after a player has toggled a hot air balloon on or off.")]
	[OxideHook.Patch(typeof(HotAirBalloon), "EngineSwitch")]
	public class HotAirBalloon_EngineSwitch_OnHotAirBalloonToggled
	{
		public static void Prefix(BaseEntity.RPCMessage msg) { }
	}
}
