///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;

[HarmonyPatch(typeof(ServerMgr), "OpenConnection")]
public class OnServerInitialized
{
	internal static TimeSince _call;

	public static void Postfix ()
	{
		if (_call <= 0.5f) return;

		HookExecutor.CallStaticHook("OnServerInitialized");
		HookExecutor.CallStaticHook("OnServerInitialized", CarbonCore.IsServerFullyInitialized);
		_call = 0;
	}
}
