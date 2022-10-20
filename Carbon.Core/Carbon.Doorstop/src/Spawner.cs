///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System.Diagnostics;
using System.IO;

namespace Carbon.Utility
{
	internal static class Spawner
	{
#if WIN
		internal static void DoSpawnStub()
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = Path.Combine(Context.Tools, "publicizer.bat"),
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
			}).WaitForExit();
		}
#elif UNIX
		internal static void DoSpawnStub()
		{
			Process Handler = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo
				{
					FileName = Path.Combine(Context.Tools, "publicizer.sh"),
					Arguments = $"{Process.GetCurrentProcess().Id}",
					CreateNoWindow = true,
					UseShellExecute = true,
				}
			};

			Handler.Start();
		}
#endif
	}
}
