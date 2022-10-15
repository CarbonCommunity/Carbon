
///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using System.IO;

namespace Carbon.Utility;

internal sealed class Context
{
	private static readonly string[] Needles = {
			".", "..", "../.."
		};

	public static readonly string
		GameDirectory, GameManagedDirectory, CarbonDirectory;

	static Context()
	{
		GameDirectory = null;

		foreach (string Needle in Needles)
		{
			string t = Path.GetFullPath(Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory, Needle));

			if (!Directory.Exists(Path.Combine(t, "RustDedicated_Data"))) continue;
			GameDirectory = t;
			break;
		}

		if (GameDirectory == null)
			throw new System.Exception("Unable to find root folder");

		GameManagedDirectory = Path.GetFullPath(
			Path.Combine(GameDirectory, "RustDedicated_Data", "Managed"));

		CarbonDirectory = Path.GetFullPath(
			Path.Combine(GameDirectory, "carbon"));
	}
}