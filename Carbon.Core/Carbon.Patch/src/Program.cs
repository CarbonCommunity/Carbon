using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using CommandLine;
using Humanlights.Extensions;

namespace Carbon.Patch
{
	internal class Program
	{
		private static Dictionary<string, string> commonList = new Dictionary <string, string>
		{
			{ "/Carbon.Core/Carbon/bin/Release/net48/Carbon.dll", "Release/Carbon.dll" },
			{ "/Carbon.Core/Carbon/bin/ReleaseUnix/net48/Carbon.dll", "Release/Carbon-Unix.dll" },
		};

		private static Dictionary<string, string> windowsList = new Dictionary<string, string>
		{
			{ "/Carbon.Core/Carbon/bin/Release/net48/Carbon.dll", "HarmonyMods/Carbon.dll" },
			{ "/Carbon.Core/Carbon.Doorstop/bin/Release/net48/Carbon.Doorstop.dll", "RustDedicated_Data/Managed/Carbon.Doorstop.dll" },
			{ "/Tools/Helpers/doorstop_config.ini", "doorstop_config.ini" },
			{ "/Tools/UnityDoorStop/winhttp.dll", "winhttp.dll" },
			{ "/Tools/NStrip/NStrip/bin/Release/net452/NStrip.exe", "carbon/tools/NStrip.exe" },
		};

		private static Dictionary<string, string> unixList = new Dictionary<string, string>
		{
			{ "/Carbon.Core/Carbon/bin/ReleaseUnix/net48/Carbon.dll", "HarmonyMods/Carbon-Unix.dll" },
			{ "/Tools/Helpers/linux_prepatch.sh", "carbon_prepatch.sh" },
			{ "/Tools/NStrip/NStrip/bin/Release/net452/NStrip.exe", "carbon/tools/NStrip.exe" },
		};

		public static void Main(string[] args)
		{
			Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed<CommandLineOptions>(Arguments =>
			{
				var f = Arguments.Path + "/Release";
				Console.WriteLine(f);

				try { OsEx.Folder.DeleteContents(f); } catch { }
				OsEx.Folder.Create(f);

				foreach (KeyValuePair<string, string> File in commonList)
				{
					try
					{
						OsEx.File.Copy($"{Arguments.Path}/{File.Key}", $"{Arguments.Path}/{File.Value}");
					}
					catch { new Exception($"Error processing: {Arguments.Path}/{File.Key}"); }
				}

				//
				// Windows patch
				//
				using (var memoryStream = new MemoryStream())
				{
					using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
					{
						foreach (KeyValuePair<string, string> File in windowsList)
						{
							try
							{
								archive.CreateEntryFromFile($"{Arguments.Path}/{File.Key}", $"{Arguments.Path}/{File.Value}");
							} 
							catch { new Exception($"Error processing: {Arguments.Path}/{File.Key}"); }
						}
					}

					var output = $"{Arguments.Path}/Release/Carbon.Patch.zip";
					OsEx.File.Delete(output);
					OsEx.File.Create(output, new byte[0]);

					using (var fileStream = new FileStream(output, FileMode.Open))
					{
						memoryStream.Seek(0, SeekOrigin.Begin);
						memoryStream.CopyTo(fileStream);
					}
				}

				//
				// Linux patch
				//
				using (var memoryStream = new MemoryStream())
				{
					using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
					{
						foreach (KeyValuePair<string, string> File in unixList)
						{
							try
							{
								archive.CreateEntryFromFile($"{Arguments.Path}/{File.Key}", $"{Arguments.Path}/{File.Value}");
							}
							catch { new Exception($"Error processing: {Arguments.Path}/{File.Key}"); }
						}
					}

					var output = $"{Arguments.Path}/Release/Carbon.Patch-Unix.zip";
					OsEx.File.Delete(output);
					OsEx.File.Create(output, new byte[0]);

					using (var fileStream = new FileStream(output, FileMode.Open))
					{
						memoryStream.Seek(0, SeekOrigin.Begin);
						memoryStream.CopyTo(fileStream);
					}
				}
			});
		}
	}
}