
///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using System.IO;

namespace Carbon.LoaderEx.Context;

internal sealed class Directory
{
	private static readonly string[]
		Needles = { ".", "..", "../.." };

	internal static readonly string
		Game, GameManaged, GameHarmony,

		Carbon, CarbonManaged, CarbonHarmony, CarbonLib, CarbonLogs;

	static Directory()
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

		if (Game == null) throw new System.Exception("Unable to find root folder");

		GameManaged = Path.GetFullPath(Path.Combine(Game, "RustDedicated_Data", "Managed"));
		GameHarmony = Path.GetFullPath(Path.Combine(Game, "HarmonyMods"));

		Carbon = Path.GetFullPath(Path.Combine(Game, "carbon"));
		CarbonLogs = Path.GetFullPath(Path.Combine(Carbon, "logs"));
		CarbonHarmony = Path.GetFullPath(Path.Combine(Carbon, "harmony"));
		CarbonManaged = Path.GetFullPath(Path.Combine(Carbon, "managed"));
		CarbonLib = Path.GetFullPath(Path.Combine(CarbonManaged, "lib"));
	}
}
