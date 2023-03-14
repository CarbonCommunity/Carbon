using System;
using System.IO;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Utility;

internal sealed class Context
{
	private static readonly string[]
		Needles = { ".", "..", "../.." };

	internal static readonly string
		Game, GameManaged,

		Carbon, CarbonManaged, CarbonExtensions, CarbonHarmony, CarbonLib, CarbonHooks, CarbonModules, CarbonLogs;

	static Context()
	{
		Game = null;
		foreach (string Needle in Needles)
		{
			string t = Path.GetFullPath(Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory, Needle));

			if (!Directory.Exists(Path.Combine(t, "RustDedicated_Data"))) continue;
			Game = t;
			break;
		}

		try
		{
			if (Game == null) throw new Exception("Unable to find root folder");

			GameManaged = Path.GetFullPath(Path.Combine(Game, "RustDedicated_Data", "Managed"));

			Carbon = Path.GetFullPath(Path.Combine(Game, "carbon"));
			if (!Directory.Exists(Carbon)) throw new Exception("Carbon folder is missing");

			CarbonLogs = Path.Combine(Carbon, "logs");
			if (!Directory.Exists(CarbonLogs)) Directory.CreateDirectory(CarbonLogs);

			CarbonHarmony = Path.Combine(Carbon, "harmony");
			if (!Directory.Exists(CarbonHarmony)) Directory.CreateDirectory(CarbonHarmony);

			CarbonManaged = Path.Combine(Carbon, "managed");
			if (!Directory.Exists(CarbonManaged)) Directory.CreateDirectory(CarbonManaged);

			CarbonExtensions = Path.Combine(Carbon, "extensions");
			if (!Directory.Exists(CarbonExtensions)) Directory.CreateDirectory(CarbonExtensions);

			CarbonLib = Path.Combine(Carbon, "managed", "lib");
			if (!Directory.Exists(CarbonLib)) Directory.CreateDirectory(CarbonLib);

			CarbonHooks = Path.Combine(Carbon, "managed", "hooks");
			if (!Directory.Exists(CarbonHooks)) Directory.CreateDirectory(CarbonHooks);

			CarbonModules = Path.Combine(Carbon, "managed", "modules");
			if (!Directory.Exists(CarbonModules)) Directory.CreateDirectory(CarbonModules);
		}
		catch (System.Exception e)
		{
			Logger.Error("Critical error while loading Carbon", e);
			System.Environment.Exit(1);
			throw;
		}

	}
}
