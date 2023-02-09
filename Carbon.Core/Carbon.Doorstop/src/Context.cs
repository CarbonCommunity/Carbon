using System;
using System.IO;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Utility;

public static class Context
{
	public static string Base, Carbon, Managed, Modules, RustManaged, Lib;

	private static readonly string[] Needles = {
		".", "..", "../.."
	};

	public static void Init()
	{
		if (Base == null)
		{
			foreach (string Needle in Needles)
			{
				string t = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Needle));
				if (!Directory.Exists(Path.Combine(t, "RustDedicated_Data"))) continue;
				Base = t;
				break;
			}
		}

		if (Base == null)
			throw new System.Exception("Unable to find root folder");

		Carbon = Path.GetFullPath(Path.Combine(
			Base, "carbon"));

		Managed = Path.GetFullPath(Path.Combine(
			Base, "carbon", "managed"));

		Modules = Path.GetFullPath(Path.Combine(
			Base, "carbon", "managed", "modules"));

		Lib = Path.GetFullPath(Path.Combine(
			Base, "carbon", "managed", "lib"));

		RustManaged = Path.GetFullPath(Path.Combine(
			Base, "RustDedicated_Data", "Managed"));
	}
}
