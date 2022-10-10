///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Diagnostics;
using Carbon.Utility;

namespace Doorstop
{
	public class Entrypoint
	{
		public static void Start()
		{
			Console.WriteLine(">> Carbon.Loader is using the Doorstop entrypoint");

			if (!Patcher.IsPatched() && Patcher.DoPatch())
			{
				Patcher.SpawnWorker();
				Process.GetCurrentProcess().Kill();
			}
		}
	}
}
