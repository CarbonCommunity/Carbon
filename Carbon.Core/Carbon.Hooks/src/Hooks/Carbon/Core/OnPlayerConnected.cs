/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Core
{
	public partial class Core_BasePlayer
	{
		/*
		[Hook.AlwaysPatched]
		[Hook("OnPlayerConnected"), Hook.Category(Hook.Category.Enum.Player)]
		[Hook.Parameter("this", typeof(BasePlayer))]
		[Hook.Info("Called after the player object is created, but before the player has spawned.")]
		[Hook.Patch(typeof(BasePlayer), "PlayerInit")]
		*/

		public class Core_BasePlayer_PlayerInit_9e1a7b5738f441d698700fcbf25ca8b1
		{
			public static Metadata metadata = new Metadata("OnPlayerConnected",
				typeof(BasePlayer), "PlayerInit", new System.Type[] { typeof(Network.Connection) });

			static Core_BasePlayer_PlayerInit_9e1a7b5738f441d698700fcbf25ca8b1()
			{
				metadata.SetAlwaysPatch(true);
			}

			public static void Postfix(Network.Connection c)
			{
				HookCaller.CallStaticHook("OnPlayerConnected", c.player as BasePlayer);
			}
		}
	}
}