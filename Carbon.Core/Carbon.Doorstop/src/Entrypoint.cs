///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.IO;
using Carbon.Utility;

namespace Doorstop;

public class Entrypoint
{
	public static void Start()
	{
		Logger.Log(">> Carbon.Doorstop using UnityDoorstop entrypoint");

		try
		{
			Publicizer.Read(
				Path.Combine(Context.Managed, "Assembly-CSharp.dll")
			);

			if (!Publicizer.IsPublic("ServerMgr", "Shutdown"))
			{
				Logger.Warn("Assembly is not publicized");

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
			else
			{
				Logger.Log("All validation checks passed" + Environment.NewLine);
			}
		}
		catch { /* exit */}
	}
}
