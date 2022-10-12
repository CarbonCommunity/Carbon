///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnServerMessage", typeof(object)), OxideHook.Category(Hook.Category.Enum.Server)]
	[OxideHook.Info("Called before a server message is sent to all connected players.")]
	[OxideHook.Info("Return a non-null value to stop message from being sent.")]
	[OxideHook.Parameter("message", typeof(string))]
	[OxideHook.Parameter("username", typeof(string))]
	[OxideHook.Parameter("color", typeof(string))]
	[OxideHook.Parameter("userid", typeof(ulong))]
	[OxideHook.Patch(typeof(ConVar.Chat), "Broadcast")]
	public class ConVar_Chat_Broadcast
	{
		public static bool Prefix(string message, string username = "SERVER", string color = "#eee", ulong userid = 0uL)
		{
			return HookExecutor.CallStaticHook("OnServerMessage", message, username, color, userid) == null;
		}
	}
}
