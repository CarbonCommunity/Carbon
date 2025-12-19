using System.Diagnostics;
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

public struct TimedGroupLog : IDisposable
{
	private readonly string _name;
	private readonly Stopwatch _stopwatch;

	private const string Cyan = "\u001b[36;1m"; // Cyan + Bold
	private const string Green = "\u001b[32;1m"; // Green + Bold
	private const string Reset = "\u001b[0m"; // Reset to default

	public TimedGroupLog(string name)
	{
		_name = name;
		_stopwatch = Stopwatch.StartNew();

		// fix, as Microsoft Logger is async and not immediately prints to stdout (should be flushed),
		// so without it some _logger logs could end up inside group (although they were right before using group)
		Thread.Sleep(20);

		Console.WriteLine($"::group::{Cyan}{name}{Reset}");
	}

	private void Stop()
	{
		_stopwatch.Stop();

		Thread.Sleep(20);
		Console.WriteLine("::endgroup::");
		Console.WriteLine($"{Green}✓ {_name} took {_stopwatch.Elapsed:mm\\:ss\\.ffff} {Reset}");
	}

	public void Dispose()
	{
		Stop();
		GC.SuppressFinalize(this);
	}
}
