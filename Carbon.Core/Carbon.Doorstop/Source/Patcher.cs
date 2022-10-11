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
				Logger.Error("Coulnd't get reflection for 'ServerMgr'.", ex);
				return false;
			}
		}

		public static bool DoPatch()
		{
			Logger.None(
				"  __.-._  " + Environment.NewLine +
				"  '-._\"7'    Assembly-CSharp.dll not patched." + Environment.NewLine +
				"   /'.-c     Execute the carbon publicizer you must." + Environment.NewLine +
				"   |  /T     Application will now exit. Hmm." + Environment.NewLine +
				"  _)_/LI  " + Environment.NewLine
			);

			return Publicizer.Publicize(Path.Combine(Managed, "Assembly-CSharp.dll"));
		}

		public static bool DoCopy()
		{
			try
			{
				string Backup = Path.Combine(Managed, "Assembly-CSharp-backup.dll");
				string Original = Path.Combine(Managed, "Assembly-CSharp.dll");
				string Publicized = Path.Combine(Managed, "__Assembly-CSharp.dll");

				if (File.Exists(Backup)) File.Delete(Backup);
				File.Move(Original, Backup);
				File.Move(Publicized, Original);

				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("Unable to replace 'Assembly-CSharp.dll'", ex);
				return false;
			}
		}

		public static void DoSpawnStub()
		{
			Process Handler = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo
				{
#if UNIX
					FileName = Path.Combine(Tools, "publicizer.sh"),
#else
					FileName = Path.Combine(Tools, "publicizer.bat"),
#endif
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