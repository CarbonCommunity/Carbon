///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnPlayerInput", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("input", typeof(InputState))]
	[OxideHook.Info("Called when input is received from a connected client.")]
	[OxideHook.Patch(typeof(BasePlayer), "OnReceiveTick")]
	public class BasePlayer_OnReceiveTick_OnPlayerInput
	{
		public static bool Prefix(PlayerTick msg, bool wasPlayerStalled, ref BasePlayer __instance)
		{
			var onTick = HookCaller.CallStaticHook("OnPlayerTick", __instance, msg, wasPlayerStalled);
			var onInput = HookCaller.CallStaticHook("OnPlayerInput", __instance, __instance.serverInput);

			return onTick == null && onInput == null;
		}
	}

	[OxideHook("OnPlayerTick", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Require("OnPlayerInput")]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("tick", typeof(PlayerTick))]
	[OxideHook.Parameter("wasPlayerStalled", typeof(bool))]
	[OxideHook.Patch(typeof(BasePlayer), "OnReceiveTick")]
	public class BasePlayer_OnReceiveTick_OnPlayerTick
	{
		public static void Prefix(PlayerTick msg, bool wasPlayerStalled)
		{
		}
	}
}
