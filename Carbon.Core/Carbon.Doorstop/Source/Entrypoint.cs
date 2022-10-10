///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Diagnostics;
using Carbon.Utility;

namespace Carbon
{
	public class Entrypoint : IHarmonyModHooks
	{
		public void OnLoaded(OnHarmonyModLoadedArgs args)
		{
			Console.WriteLine(">> Carbon.Loader is using the Harmony entrypoint");

			if (!Patcher.IsPatched() && Patcher.DoPatch())
			{
				Patcher.SpawnWorker();
				Process.GetCurrentProcess().Kill();
			}
		}

		public void OnUnloaded(OnHarmonyModUnloadedArgs args) { }
	}
}