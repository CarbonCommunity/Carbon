///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Hooks
{
	[Hook.AlwaysPatched]
	[Hook("OnServerShutdown"), Hook.Category(Hook.Category.Enum.Server)]
	[Hook.Info("Useful for saving something / etc on server shutdown.")]
	[Hook.Patch(typeof(ServerMgr), "Shutdown")]
	public class OnServerShutdown
	{
		public static void Prefix()
		{
			Carbon.Logger.Log($"Saving Carbon plugins & shutting down");

			Interface.Oxide.OnShutdown();

			HookCaller.CallStaticHook("OnServerSave");
			HookCaller.CallStaticHook("OnServerShutdown");

			Community.Runtime.HarmonyProcessor.Clear();
			Community.Runtime.ScriptProcessor.Clear();
		}
	}
}
