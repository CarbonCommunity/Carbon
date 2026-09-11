using System.Runtime.InteropServices;

namespace Carbon.Profiler.Tests;

internal static class FileSystemUtils
{
	public static void CopyDirectory(string source, string destination)
	{
		foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
		{
			Directory.CreateDirectory(directory.Replace(source, destination));
		}

		foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
		{
			var target = file.Replace(source, destination);
			Directory.CreateDirectory(Path.GetDirectoryName(target)!);
			File.Copy(file, target, true);
		}
	}

	public static void MakeExecutable(string filePath)
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return;
		}

		try
		{
			File.SetUnixFileMode(filePath, File.GetUnixFileMode(filePath) | UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute);
		}
		catch
		{
			// Some file systems do not support chmod. Rust will fail later if this was required.
		}
	}
}
