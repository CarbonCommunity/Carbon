using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Fun
{
	public partial class Fun_BasePlayer
	{
		[HookAttribute.Patch("OnJackieChan", "OnJackieChan", typeof(BasePlayer), "PlayerInit", new System.Type[] { typeof(Network.Connection) })]
		[HookAttribute.Identifier("98afa60bc10945799214031b1c017ac5")]

		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Info("Checks if player that connected is Jackie Chan.")]

		public class Fun_BasePlayer_98afa60bc10945799214031b1c017ac5 : Patch
		{
			public static void Prefix(Network.Connection c)
			{
				try
				{
					var player = c.player as BasePlayer;

					if (player.displayName == "Jackie Chan")
						HookCaller.CallStaticHook(1777955827, player);
				}
				catch { }
			}
		}
	}
}
