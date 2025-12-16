using System.Runtime.InteropServices;

namespace Carbon.TestRunner;

internal static class Utils
{
	public static void MakeExecutableExecutable(string executablePath)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			File.SetUnixFileMode(executablePath, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
		}
	}
}
