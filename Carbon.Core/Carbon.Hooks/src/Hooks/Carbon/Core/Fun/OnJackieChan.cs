/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Fun
{
	public partial class Fun_BasePlayer
	{
		/*
		[CarbonHook("OnJackieChan"), CarbonHook.Category(Hook.Category.Enum.Special)]
		[CarbonHook.Parameter("this", typeof(BasePlayer))]
		[CarbonHook.Info("Checks if player that connected is Jackie Chan.")]
		[CarbonHook.Patch(typeof(BasePlayer), "PlayerInit")]
		*/

		public class Fun_BasePlayer_PlayerInit_98afa60bc10945799214031b1c017ac5
		{
			public static Metadata metadata = new Metadata("OnJackieChan",
				typeof(BasePlayer), "PlayerInit", new System.Type[] { typeof(Network.Connection) });

			static Fun_BasePlayer_PlayerInit_98afa60bc10945799214031b1c017ac5()
			{
				metadata.SetIdentifier("98afa60bc10945799214031b1c017ac5");
			}

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
}
