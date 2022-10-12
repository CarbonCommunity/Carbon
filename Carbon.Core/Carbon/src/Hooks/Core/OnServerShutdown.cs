///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Oxide.Core;

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

		HookExecutor.CallStaticHook("OnServerSave");
		HookExecutor.CallStaticHook("OnServerShutdown");

		CarbonCore.Instance.HarmonyProcessor.Clear();
		CarbonCore.Instance.ScriptProcessor.Clear();
	}
}
