///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System.Diagnostics;
using Carbon;
using Carbon.Utility;

namespace Carbon
{
	public class Entrypoint : IHarmonyModHooks
	{
		public void OnLoaded(OnHarmonyModLoadedArgs args)
		{
			Logger.Log(">> Carbon.Loader using Harmony entrypoint");
			if (Patcher.IsPatched()) return;

			Patcher.DoPatch();
			Patcher.SpawnWorker();
			Process.GetCurrentProcess().Kill();
		}

		public void OnUnloaded(OnHarmonyModUnloadedArgs args) { }
	}
}

namespace Doorstop
{
	public class Entrypoint
	{
		public static void Start()
		{
			Logger.Log(">> Carbon.Loader using UnityDoorstop entrypoint");
			if (Patcher.IsPatched()) return;

			Patcher.DoPatch();
			Patcher.DoCopy();
		}
	}
}