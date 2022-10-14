
///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.IO;

namespace Carbon.Utility
{
	internal static class Context
	{
		public static readonly string Base, Tools, Managed;

		private static readonly string[] Needles = {
			".", "..", "../.."
		};

		static Context()
		{
			foreach (string Needle in Needles)
			{
				string t = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Needle));
				if (!Directory.Exists(Path.Combine(t, "RustDedicated_Data"))) continue;
				Base = t;
				break;
			}

			if (Base == null)
				throw new System.Exception("Unable to find root folder");

			Tools = Path.GetFullPath(Path.Combine(
				Base, "carbon", "tools"));

			Managed = Path.GetFullPath(Path.Combine(
				Base, "RustDedicated_Data", "Managed"));
		}
	}
}