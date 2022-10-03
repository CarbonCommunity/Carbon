///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using Carbon.Core;
using Carbon.Core.Processors;
using Harmony;
using Humanlights.Extensions;

[HarmonyPatch(typeof(ServerMgr), "OpenConnection")]
public class OnServerInitialized
{
	internal static TimeSince _call;

	public static void Postfix()
	{
		if (_call <= 0.5f) return;

		ScriptLoader.OnFinished();
		_call = 0;
	}
}
