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
			UnityEngine.Assertions.Assert.IsTrue(Directory.Exists(Carbon), "Carbon folder is missing");

			CarbonLogs = Path.Combine(Carbon, "logs");
			if (!Directory.Exists(CarbonLogs)) Directory.CreateDirectory(CarbonLogs);

			CarbonHarmony = Path.Combine(Carbon, "harmony");
			if (!Directory.Exists(CarbonLogs)) Directory.CreateDirectory(CarbonHarmony);

			CarbonManaged = Path.Combine(Carbon, "managed");
			if (!Directory.Exists(CarbonLogs)) Directory.CreateDirectory(CarbonManaged);

			CarbonLib = Path.Combine(CarbonManaged, "lib");
			if (!Directory.Exists(CarbonLogs)) Directory.CreateDirectory(CarbonLib);

			CarbonModules = Path.Combine(CarbonManaged, "ext");
			if (!Directory.Exists(CarbonLogs)) Directory.CreateDirectory(CarbonModules);
		}
		catch (System.Exception e)
		{
			Logger.Error("Critical error while loading Carbon", e);
			Rust.Application.Quit();
			throw;
		}

	}
}
