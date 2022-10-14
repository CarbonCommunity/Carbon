///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon;
using Carbon.Core;

namespace Carbon.Hooks
{
	[Hook.AlwaysPatched]
	[Hook("OnPlayerConnected"), Hook.Category(Hook.Category.Enum.Player)]
	[Hook.Parameter("this", typeof(BasePlayer))]
	[Hook.Info("Called after the player object is created, but before the player has spawned.")]
	[Hook.Patch(typeof(BasePlayer), "PlayerInit")]
	public class OnPlayerConnected
	{
		public static void Postfix(Network.Connection c)
		{
			HookExecutor.CallStaticHook("OnPlayerConnected", c.player as BasePlayer);
		}
	}
}
