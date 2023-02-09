using System;
using System.IO;
using Carbon.Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Doorstop;

public class Entrypoint
{
	public static void Start()
	{
		References.Load();
		Context.Init();

		Logger.Log(">> Carbon.Doorstop using UnityDoorstop entrypoint");

		Execute(Path.Combine(Context.RustManaged, "Assembly-CSharp.dll"));
	}

	public static void Execute(string filePath)
	{
		try
		{
			Logger.Init();

			Publicizer.Read(filePath);

			if (!Publicizer.IsPublic("ServerMgr", "Shutdown"))
			{
				Logger.Warn("Assembly is not publicized");
				Publicizer.Publicize(module =>
				{
					try
					{
						Injector.Inject(module);
					}
					catch (Exception ex)
					{
						Logger.Error($"Failed injecting: {ex}");
					}
				});
				Publicizer.Write(filePath);
			}
			else
			{
				Logger.Log("All validation checks passed");
			}
		}
		catch { /* exit */ }
	}
}
