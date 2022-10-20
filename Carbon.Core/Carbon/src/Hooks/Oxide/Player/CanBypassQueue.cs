///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Network;
using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("CanBypassQueue", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("connection", typeof(Connection))]
	[OxideHook.Info("Called before the player is added to the connection queue.")]
	[OxideHook.Patch(typeof(ConnectionQueue), "CanJumpQueue")]
	public class ConnectionQueue_CanJumpQueue
	{
		public static bool Prefix(Connection connection, out bool __result)
		{
			__result = default;

			var result = Interface.CallHook("CanBypassQueue", connection);

			if (result is bool boolResult)
			{
				__result = boolResult;
				return false;
			}

			return true;
		}
	}
}
