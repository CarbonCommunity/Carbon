///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///

namespace Carbon.LoaderEx.Components.Supervisor;

public static class Core
{
	public static void Exit()
		=> HarmonyLoader.GetInstance().Unload("Carbon.dll");

	public static void Restart()
	{
		HarmonyLoader.GetInstance().Unload("Carbon.dll");
		HarmonyLoader.GetInstance().Load("Carbon.dll");
	}
}
