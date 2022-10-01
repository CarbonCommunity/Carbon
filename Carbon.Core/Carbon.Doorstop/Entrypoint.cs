using System;
using System.Diagnostics;
using System.IO;

namespace Doorstop
{
	public class Entrypoint
	{
		private static string NStripPath
			=> Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "carbon", "tools", "NStrip.exe");

		private static string NStripOpts
			= "--public --include-compiler-generated --keep-resources --no-strip --overwrite --unity-non-serialized";

		private static string AssemblyCSharp
			=> Path.GetFullPath(Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory, "RustDedicated_Data", "Managed", "Assembly-CSharp.dll"));

		public static void Start ()
		{
			File.WriteAllText("__doorstop.log", "Carbon.Doorstop starting");

			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = NStripPath,
					Arguments = $@"{NStripOpts} ""{AssemblyCSharp}""",
					WindowStyle = ProcessWindowStyle.Hidden,
					CreateNoWindow = true
				}).WaitForExit();
			}
			catch { }
		}
	}
}
