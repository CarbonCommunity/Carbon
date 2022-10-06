///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;

[HarmonyPatch(typeof(BasePlayer), "PlayerInit")]
public class OnPlayerConnected
{
	public static void Postfix(Network.Connection c)
	{
		HookExecutor.CallStaticHook("OnPlayerConnected", c.player as BasePlayer);
	}
}
