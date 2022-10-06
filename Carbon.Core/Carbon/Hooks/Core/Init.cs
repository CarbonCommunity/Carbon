///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using HarmonyLib;

[HarmonyPatch(typeof(Bootstrap), "StartupShared")]
public class Init
{
	public static void Prefix()
	{
		CarbonCore.Instance.Init();
	}
}
