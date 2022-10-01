///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;

[HarmonyPatch(typeof(ServerMgr), "OnDisconnected")]
public class OnPlayerDisconnected
{
	public static void Postfix (string strReason, Network.Connection connection)
	{
		HookExecutor.CallStaticHook("OnPlayerDisconnected", connection.player as BasePlayer, strReason);
	}
}
