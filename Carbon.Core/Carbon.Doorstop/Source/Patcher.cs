///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Carbon.Utility
{
	public static class Patcher
	{
		private static readonly string Base, Tools, Managed;

		private static readonly string[] Needles = {
			".", "..", "../.."
		};

		static Patcher()
		{
			foreach (string Needle in Needles)
			{
				string t = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Needle));
				if (!Directory.Exists(Path.Combine(t, "RustDedicated_Data"))) continue;
				Base = t; break;
			}

			if (Base == null)
				throw new Exception("Unable to find root folder");

			Tools = Path.GetFullPath(Path.Combine(
				Base, "carbon", "tools"));

			Managed = Path.GetFullPath(Path.Combine(
				Base, "RustDedicated_Data", "Managed"));
		}

		public static bool IsPatched()
		{
			try
			{
				Type t = Type.GetType("ServerMgr, Assembly-CSharp");
				MethodInfo m = t.GetMethod("Shutdown", BindingFlags.Public | BindingFlags.Instance) ?? null;
				return (m == null) ? false : m.IsPublic;
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR! Cannot get the assembly type.");
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public static bool DoPatch()
		{
			Console.WriteLine(
				"  __.-._  " + Environment.NewLine +
				"  '-._\"7'    Assembly-CSharp.dll not patched." + Environment.NewLine +
				"   /'.-c     Execute the carbon publicizer you must." + Environment.NewLine +
				"   |  /T     Application will now exit. Hmm." + Environment.NewLine +
				"  _)_/LI  " + Environment.NewLine
			);

			return Publicizer.Publicize(Path.Combine(Managed, "Assembly-CSharp.dll"));
		}

		public static void SpawnWorker()
		{
			Console.WriteLine($">> {Tools}");

			Process Handler = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo
				{
					FileName = Path.Combine(Tools, "publicizer.sh"),
					Arguments = $"{Process.GetCurrentProcess().Id}",
					CreateNoWindow = true,
					UseShellExecute = true,
				}
			};

			Handler.Start();
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}
	}
}