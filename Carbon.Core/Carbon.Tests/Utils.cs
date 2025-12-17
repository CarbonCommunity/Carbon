using System.Runtime.InteropServices;

namespace Carbon.Test;

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
		string folder, string destination, bool subdirectories = true, bool overwrite = true,
		SearchOption option = SearchOption.AllDirectories
	)
	{
		if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(destination))
		{
			if (Directory.Exists(folder))
			{
				var folderInfo = new DirectoryInfo(folder);
				var retDictionary = new Dictionary<string, string>();

				var folders = folderInfo.GetDirectories();
				Directory.CreateDirectory(destination);
				retDictionary.Add(folderInfo.FullName, destination);

				var files = folderInfo.GetFiles();
				foreach (var file in files)
				{
					var tempPath = Path.Combine(destination, file.Name);
					file.CopyTo(tempPath, overwrite);
					retDictionary.Add(file.FullName, tempPath);
				}

				foreach (var subDirectory in folders)
				{
					var tempPath = Path.Combine(destination, subDirectory.Name);
					Copy(subDirectory.FullName, tempPath, subdirectories, overwrite, option);
					retDictionary.Add(subDirectory.FullName, tempPath);
				}

				return retDictionary;
			}
		}

		return null;
	}
}
