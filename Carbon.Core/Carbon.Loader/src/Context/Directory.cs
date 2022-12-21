using System;
using System.IO;
using Carbon.LoaderEx.Utility;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx.Context;

internal sealed class Directories
{
	private static readonly string[]
		Needles = { ".", "..", "../.." };

	internal static readonly string
		Game, GameManaged, GameHarmony,

		Carbon, CarbonManaged, CarbonHarmony, CarbonLib, CarbonModules, CarbonLogs;

	static Directories()
	{
		Game = null;
		foreach (string Needle in Needles)
		{
			string t = Path.GetFullPath(Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory, Needle));

			if (!System.IO.Directory.Exists(Path.Combine(t, "RustDedicated_Data"))) continue;
			Game = t;
			break;
		}

		try
		{
			if (Game == null) throw new System.Exception("Unable to find root folder");

			GameManaged = Path.GetFullPath(Path.Combine(Game, "RustDedicated_Data", "Managed"));
			GameHarmony = Path.GetFullPath(Path.Combine(Game, "HarmonyMods"));

			Carbon = Path.GetFullPath(Path.Combine(Game, "carbon"));
			if (Carbon == null) throw new System.Exception("Unable to find Carbon folder");

			CarbonLogs = Path.GetFullPath(Path.Combine(Carbon, "logs"));
			UnityEngine.Assertions.Assert.IsTrue(Directory.Exists(CarbonLogs));

			CarbonHarmony = Path.GetFullPath(Path.Combine(Carbon, "harmony"));
			UnityEngine.Assertions.Assert.IsTrue(Directory.Exists(CarbonHarmony));

			CarbonManaged = Path.GetFullPath(Path.Combine(Carbon, "managed"));
			UnityEngine.Assertions.Assert.IsTrue(Directory.Exists(CarbonManaged));

			CarbonLib = Path.GetFullPath(Path.Combine(CarbonManaged, "lib"));
			UnityEngine.Assertions.Assert.IsTrue(Directory.Exists(CarbonLib));

			CarbonModules = Path.GetFullPath(Path.Combine(CarbonManaged, "ext"));
			UnityEngine.Assertions.Assert.IsTrue(Directory.Exists(CarbonModules));
		}
		catch (System.Exception e)
		{
			Logger.Error("Critical error while loading Carbon", e);
			Rust.Application.Quit();
			throw;
		}

	}
}
