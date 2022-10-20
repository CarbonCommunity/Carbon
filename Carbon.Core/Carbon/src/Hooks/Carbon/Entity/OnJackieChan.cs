///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[CarbonHook("OnJackieChan"), CarbonHook.Category(Hook.Category.Enum.Special)]
	[CarbonHook.Parameter("this", typeof(BasePlayer))]
	[CarbonHook.Info("Checks if player that connected is Jackie Chan.")]
	[CarbonHook.Patch(typeof(BasePlayer), "PlayerInit")]
	public class OnJackieChan
	{
		public static void Prefix(Network.Connection c)
		{
			try
			{
				var player = c.player as BasePlayer;

				if (player.displayName == "Jackie Chan")
					HookCaller.CallStaticHook("OnJackieChan", player);
			}
			catch { }
		}
	}
}
