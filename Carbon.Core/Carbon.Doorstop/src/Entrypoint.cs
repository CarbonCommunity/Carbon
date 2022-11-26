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
				Publicizer.Publicize();
				Publicizer.Write(
					Path.Combine(Context.Managed, "Assembly-CSharp.dll")
				);
			}
			else
			{
				Logger.Log("All validation checks passed");
			}
		}
		catch { /* exit */}
	}
}
