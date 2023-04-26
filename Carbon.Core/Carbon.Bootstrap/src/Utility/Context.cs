using System;
using System.IO;
using Carbon.Extensions;

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

		Carbon, CarbonData, CarbonExtensions, CarbonHarmony, CarbonHooks,
		CarbonLib, CarbonLogs, CarbonManaged, CarbonModules, CarbonPlugins;

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

			Carbon = Path.GetFullPath(CommandLineEx.GetArgumentResult("-carbon.rootdir", Path.Combine(Game, "carbon")));
			if (!Directory.Exists(Carbon)) throw new Exception("Carbon folder is missing");

			CarbonData = CommandLineEx.GetArgumentResult("-carbon.datadir", Path.Combine(Carbon, "data"));
			if (!Directory.Exists(CarbonData)) Directory.CreateDirectory(CarbonData);

			CarbonExtensions = CommandLineEx.GetArgumentResult("-carbon.extdir", Path.Combine(Carbon, "extensions"));
			if (!Directory.Exists(CarbonExtensions)) Directory.CreateDirectory(CarbonExtensions);

			CarbonHarmony = CommandLineEx.GetArgumentResult("-carbon.harmonydir", Path.Combine(Carbon, "harmony"));
			if (!Directory.Exists(CarbonHarmony)) Directory.CreateDirectory(CarbonHarmony);

			CarbonHooks = Path.Combine(Carbon, "managed", "hooks");
			if (!Directory.Exists(CarbonHooks)) Directory.CreateDirectory(CarbonHooks);

			CarbonLib = Path.Combine(Carbon, "managed", "lib");
			if (!Directory.Exists(CarbonLib)) Directory.CreateDirectory(CarbonLib);

			CarbonLogs = Path.Combine(Carbon, "logs");
			if (!Directory.Exists(CarbonLogs)) Directory.CreateDirectory(CarbonLogs);

			CarbonManaged = Path.Combine(Carbon, "managed");
			if (!Directory.Exists(CarbonManaged)) Directory.CreateDirectory(CarbonManaged);

			CarbonModules = Path.Combine(Carbon, "managed", "modules");
			if (!Directory.Exists(CarbonModules)) Directory.CreateDirectory(CarbonModules);

			CarbonPlugins = CommandLineEx.GetArgumentResult("-carbon.scriptdir", Path.Combine(Carbon, "plugins"));
			if (!Directory.Exists(CarbonPlugins)) Directory.CreateDirectory(CarbonPlugins);
		}
		catch (System.Exception e)
		{
			Logger.Error("Critical error while loading Carbon", e);
			System.Environment.Exit(1);
			throw;
		}

	}
}
