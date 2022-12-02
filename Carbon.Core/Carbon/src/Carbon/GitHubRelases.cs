using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using Carbon.Extensions;

namespace Carbon.Core;

public class GitHubRelases
{
	public static string GetDownload(Community.OS os, ReleaseTypes type)
	{
		var branch = string.Empty;
		var file = string.Empty;
		var mode = string.Empty;

		switch (type)
		{
			case ReleaseTypes.Develop:
			case ReleaseTypes.Staging:
				mode = "Debug";
				break;

			case ReleaseTypes.Production:
				mode = "Release";
				break;
		}

		switch (type)
		{
			case ReleaseTypes.Develop:
				branch = "develop_build";
				break;

			case ReleaseTypes.Staging:
				branch = "staging_build";
				break;

			case ReleaseTypes.Production:
				branch = "production_build";
				break;
		}

		switch (os)
		{
			case Community.OS.Win:
				file = $"Carbon.{mode}.zip";
				break;

			case Community.OS.Linux:
				file = $"Carbon.{mode}Unix.tar.gz";
				break;
		}

		return $"https://github.com/CarbonCommunity/Carbon.Core/releases/download/{branch}/{file}";
	}
	public static void Update(Community.OS os, ReleaseTypes type, bool reboot = true)
	{
		var url = GetDownload(os, type);
		var client = new WebClient();

		Logger.Warn($" Updating Carbon {os} with {type} branch from '{url}'...");

		client.DownloadDataCompleted += (object sender, DownloadDataCompletedEventArgs e) =>
		{
			Logger.Warn($" Downloaded patch. Processing in memory...");

			try
			{
				using var file = new MemoryStream(e.Result);
				using var zip = new ZipArchive(file, ZipArchiveMode.Read);

				foreach (var entry in zip.Entries)
				{
					try
					{
						if (entry.FullName == "carbon\\managed\\Carbon.dll")
						{
							using var stream = entry.Open();
							using var ms = new MemoryStream();
							stream.CopyTo(ms);
							ms.Position = 0;

							OsEx.File.Create(Defines.DllManagedPath, ms.ToArray());
						}
						else if (entry.FullName == "carbon\\managed\\Carbon.pdb")
						{
							using var stream = entry.Open();
							using var ms = new MemoryStream();
							stream.CopyTo(ms);
							ms.Position = 0;

							OsEx.File.Create(Defines.DllManagedPdbPath, ms.ToArray());
						}
					}
					catch (Exception ex)
					{
						Logger.Error($"  Failed processing '{entry.FullName}'...", ex);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed updating Carbon to latest {os} OS version from the {type} branch. More info:", ex);
				return;
			}

			Logger.Warn($" Completed.");

			if (reboot)
			{
				HookCaller.CallStaticHook("OnServerSave");
				Carbon.LoaderEx.Components.Supervisor.Core.Reboot();
			}
		};
		client.DownloadDataAsync(new Uri(url));
	}

	public enum ReleaseTypes
	{
		Develop,
		Staging,
		Production
	}
}
