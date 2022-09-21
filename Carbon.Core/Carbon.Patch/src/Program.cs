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
		private static Dictionary<string, string> commonList = new Dictionary<string, string>
		{
			{ "Carbon.Core/Carbon/bin/Release/net48/Carbon.dll", "Release/Carbon.dll" },
			{ "Carbon.Core/Carbon/bin/ReleaseUnix/net48/Carbon.dll", "Release/Carbon-Unix.dll" },
		};

		private static Dictionary<string, string> windowsList = new Dictionary<string, string>
		{
			{ "Tools/UnityDoorStop/winhttp.dll", "winhttp.dll" },
			{ "Tools/Helpers/doorstop_config.ini", "doorstop_config.ini" },
			{ "Carbon.Core/Carbon/bin/Release/net48/Carbon.dll", "HarmonyMods/Carbon.dll" },
			{ "Tools/NStrip/NStrip/bin/Release/net452/NStrip.exe", "carbon/tools/NStrip.exe" },
			{ "Carbon.Core/Carbon.Doorstop/bin/Release/net48/Carbon.Doorstop.dll", "RustDedicated_Data/Managed/Carbon.Doorstop.dll" },
		};

		private static Dictionary<string, string> unixList = new Dictionary<string, string>
		{
			{ "Tools/Helpers/linux_prepatch.sh", "carbon_prepatch.sh" },
			{ "Tools/NStrip/NStrip/bin/Release/net452/NStrip.exe", "carbon/tools/NStrip.exe" },
			{ "Carbon.Core/Carbon/bin/ReleaseUnix/net48/Carbon.dll", "HarmonyMods/Carbon-Unix.dll" },
		};

		public static void Main(string[] args)
		{
			Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(Arguments =>
			{
				string Root = Path.GetFullPath(Arguments.Path);
				string Release = Path.GetFullPath(Path.Combine(Root, "Release"));

				try { OsEx.Folder.DeleteContents(Release); } catch { }
				OsEx.Folder.Create(Release);

				foreach (KeyValuePair<string, string> File in commonList)
				{
					try
					{
						string Source = Path.Combine(Root, File.Key);
						string Destination = Path.Combine(Root, File.Value);

						OsEx.File.Copy(Source, Destination);
						Console.WriteLine($"Copy {Source} to {Destination}");
					}
					catch { new Exception($"Error processing: {File.Key}"); }
				}

				//
				// Windows patch
				Console.WriteLine();

				using (var memoryStream = new MemoryStream())
				{
					using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
					{
						foreach (KeyValuePair<string, string> File in windowsList)
						{
							try
							{
								string Source = Path.Combine(Root, File.Key);
								string Destination = File.Value;

								archive.CreateEntryFromFile(Path.Combine(Root, File.Key), File.Value);
								Console.WriteLine($"Added {Source}");
							}
							catch { new Exception($"Error processing: {File.Key}"); }
						}
					}

					var output = Path.Combine(Release, "Carbon.Patch.zip");
					OsEx.File.Delete(output);
					OsEx.File.Create(output, new byte[0]);
					Console.WriteLine($"Zip archive {output} saved");

					using (var fileStream = new FileStream(output, FileMode.Open))
					{
						memoryStream.Seek(0, SeekOrigin.Begin);
						memoryStream.CopyTo(fileStream);
					}
				}

				//
				// Linux patch
				Console.WriteLine();

				using (var memoryStream = new MemoryStream())
				{
					using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
					{
						foreach (KeyValuePair<string, string> File in unixList)
						{
							try
							{
								string Source = Path.Combine(Root, File.Key);
								string Destination = File.Value;

								archive.CreateEntryFromFile(Path.Combine(Root, File.Key), File.Value);
								Console.WriteLine($"Added {Source}");
							}
							catch { new Exception($"Error processing: {File.Key}"); }
						}
					}

					var output = Path.Combine(Release, "Carbon.Patch-Unix.zip");
					OsEx.File.Delete(output);
					OsEx.File.Create(output, new byte[0]);
					Console.WriteLine($"Zip archive {output} saved");

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