using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Carbon.Patch
{
	internal partial class Program
	{
		static void CreateReleaseDirectory()
		{
			try
			{
				string Release = Path.GetFullPath(
					Path.Combine(Arguments.basePath, "Release"));

				if (Directory.Exists(Release))
					Directory.Delete(Release, true);

				Directory.CreateDirectory(Release);
				Utility.LogInformation($"Created {Release}");
			}
			catch (Exception e)
			{
				Utility.LogError("Prepare release folder failed, execution aborted.");
				Utility.LogError(e.Message);
				Environment.Exit(2);
			}
		}

		static void ProcessCommonFiles()
		{
			if (!CopyFiles(commonList))
			{
				Utility.LogError("Some files failed to copy, execution aborted");
				Environment.Exit(3);
			}
		}

		static void ProcessWindowsFiles()
		{
			string Output = Path.GetFullPath(
					Path.Combine(Arguments.basePath, "Release/Carbon.Patch.zip"));

			if (!CreateArchive(Output, windowsList))
			{
				Utility.LogError("Unable to create archive, execution aborted");
				Environment.Exit(4);
			}
		}

		static void ProcessUnixFiles()
		{
			string Output = Path.GetFullPath(
					Path.Combine(Arguments.basePath, "Release/Carbon.Patch-Unix.zip"));

			if (!CreateArchive(Output, unixList))
			{
				Utility.LogError("Unable to create archive, execution aborted");
				Environment.Exit(5);
			}
		}

		static bool CopyFiles(Dictionary<string, string> Files)
		{
			Utility.LogNone(string.Empty);

			foreach (KeyValuePair<string, string> File in Files)
			{
				string Source = Path.GetFullPath(File.Key
					.Replace("%TARGET%", Arguments.targetConfiguration)
					.Replace("%BASE%", Arguments.basePath)
				);

				string Destination = Path.GetFullPath(File.Value
					.Replace("%TARGET%", Arguments.targetConfiguration)
					.Replace("%BASE%", Arguments.basePath)
				);

				try
				{
					System.IO.File.Copy(Source, Destination);
					Utility.LogInformation(
						$"Copied {Utility.GetSmallerString(Source)}" +
						$" to {Utility.GetSmallerString(Destination)}"
					);
				}
				catch (Exception e)
				{
					Utility.LogError("File copy operation failed.");
					Utility.LogError(e.Message);
					return false;
				}
			}

			return true;
		}

		static bool CreateArchive(string Output, Dictionary<string, string> Files)
		{
			Utility.LogNone(string.Empty);
			Utility.LogInformation($"New ZIP archive {Output}");

			using (var memoryStream = new MemoryStream())
			{
				using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
				{
					foreach (KeyValuePair<string, string> File in Files)
					{
						string Source = Path.GetFullPath(File.Key
							.Replace("%TARGET%", Arguments.targetConfiguration)
							.Replace("%BASE%", Arguments.basePath)
						);

						try
						{
							archive.CreateEntryFromFile(Source, File.Value);
							Utility.LogInformation(
								$" > Added {Utility.GetSmallerString(Source)}");
						}
						catch (Exception e)
						{
							Utility.LogError(" > File archive operation failed.");
							Utility.LogError($" > {e.Message}");
							return false;
						}
					}
				}

				if (File.Exists(Output)) File.Delete(Output);

				using (var fileStream = new FileStream(Output, FileMode.Create))
				{
					memoryStream.Seek(0, SeekOrigin.Begin);
					memoryStream.CopyTo(fileStream);
				}
			}

			return true;
		}
	}
}