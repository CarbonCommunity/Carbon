///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///

namespace Carbon.LoaderEx.Components.Supervisor;

public static class Core
{
	public static void Start()
		=> HarmonyLoader.GetInstance().Load("Carbon.dll");

	public static void Exit()
		=> HarmonyLoader.GetInstance().Unload("Carbon.dll");

	public static void Reboot()
	{
		Exit();
		Start();
	}

	public static bool IsStarted
		=> HarmonyLoader.GetInstance().IsLoaded("Carbon.dll");
}
