///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Diagnostics;
using System.IO;
using Carbon;
using Carbon.Utility;

namespace Doorstop
{
	public class Entrypoint
	{
		public static void Start()
		{
			Logger.Log(">> Carbon.Loader using UnityDoorstop entrypoint");

			Publicizer.Read(
				Path.Combine(Context.Managed, "Assembly-CSharp.dll")
			);

			if (!Publicizer.IsPublic("ServerMgr", "Shutdown"))
			{
				Logger.None(
					"  __.-._  " + Environment.NewLine +
					"  '-._\"7'    Assembly-CSharp.dll not patched." + Environment.NewLine +
					"   /'.-c     Execute the carbon publicizer you must." + Environment.NewLine +
					"   |  /T     Process will now start. Hmm." + Environment.NewLine +
					"  _)_/LI  " + Environment.NewLine
				);

				Publicizer.Publicize();

				Publicizer.Write(
					Path.Combine(Context.Managed, "Assembly-CSharp.dll")
				);
			}
		}
	}
}

#if UNIX
namespace Carbon
{
	public class Entrypoint : IHarmonyModHooks
	{
		public void OnLoaded(OnHarmonyModLoadedArgs args)
		{
			Logger.Log(">> Carbon.Loader using Harmony entrypoint");

			Publicizer.Read(
				Path.Combine(Context.Managed, "Assembly-CSharp.dll")
			);

			if (!Publicizer.IsPublic("ServerMgr", "Shutdown"))
			{
				Logger.None(
					"  __.-._  " + Environment.NewLine +
					"  '-._\"7'    Assembly-CSharp.dll not patched." + Environment.NewLine +
					"   /'.-c     Execute the carbon publicizer you must." + Environment.NewLine +
					"   |  /T     Process will now start. Hmm." + Environment.NewLine +
					"  _)_/LI  " + Environment.NewLine
				);

				Publicizer.Publicize();

				Publicizer.Write(
					Path.Combine(Context.Managed, "Assembly-CSharp.dll")
				);

				Process.GetCurrentProcess().Kill();
			}
		}

		public void OnUnloaded(OnHarmonyModUnloadedArgs args) { }
	}
}
#endif
