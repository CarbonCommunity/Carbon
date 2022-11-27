/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Static
{
	public partial class Static_ServerMgr
	{
		/*
		[Hook.AlwaysPatched]
		[Hook("OnPlayerDisconnected"), Hook.Category(Hook.Category.Enum.Player)]
		[Hook.Parameter("this", typeof(BasePlayer))]
		[Hook.Parameter("reason", typeof(string), true)]
		[Hook.Info("Called after the player has disconnected from the server.")]
		[Hook.Patch(typeof(ServerMgr), "OnDisconnected")]
		*/

		public class Static_ServerMgr_OnDisconnected_4d9dcdaf8fbd4923a96eef18a7da7488
		{
			public static Metadata metadata = new Metadata("OnPlayerDisconnected",
				typeof(ServerMgr), "OnDisconnected", new System.Type[] { typeof(string), typeof(Network.Connection) });

			static Static_ServerMgr_OnDisconnected_4d9dcdaf8fbd4923a96eef18a7da7488()
			{
				metadata.SetIdentifier("4d9dcdaf8fbd4923a96eef18a7da7488");
				metadata.SetAlwaysPatch(true);
			}

			public static void Postfix(string strReason, Network.Connection connection)
			{
				HookCaller.CallStaticHook("OnPlayerDisconnected", connection.player as BasePlayer, strReason);
			}
		}
	}
}