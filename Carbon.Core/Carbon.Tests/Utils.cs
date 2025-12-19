using System.Runtime.InteropServices;

namespace Carbon.Tests;

internal static class Utils
{
	public static void MakeExecutableExecutable(string executablePath)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			File.SetUnixFileMode(executablePath, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
		}
	}

	public static Dictionary<string, string> Copy(
		string src, string dst,
		bool subdirectories = true, bool overwrite = true,
		SearchOption option = SearchOption.AllDirectories
	)
	{
		if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(dst))
		{
			throw new Exception("Folder or destination is empty");
		}

		if (!Directory.Exists(src))
		{
			throw new DirectoryNotFoundException("Src folder not found");
		}

		var folderInfo = new DirectoryInfo(src);
		var retDictionary = new Dictionary<string, string>();

		var folders = folderInfo.GetDirectories();
		Directory.CreateDirectory(dst);
		retDictionary.Add(folderInfo.FullName, dst);

		var files = folderInfo.GetFiles();
		foreach (var file in files)
		{
			var tempPath = Path.Combine(dst, file.Name);
			file.CopyTo(tempPath, overwrite);
			retDictionary.Add(file.FullName, tempPath);
		}

		foreach (var subDirectory in folders)
		{
			var tempPath = Path.Combine(dst, subDirectory.Name);
			Copy(subDirectory.FullName, tempPath, subdirectories, overwrite, option);
			retDictionary.Add(subDirectory.FullName, tempPath);
		}

		return retDictionary;
	}
}
