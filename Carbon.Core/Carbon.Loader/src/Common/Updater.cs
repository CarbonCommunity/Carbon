using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Carbon.LoaderEx.Utility;
using SharpCompress.Common;
using SharpCompress.Readers;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx.Common;

public static class Updater
{
	private readonly static OSPlatform Platform;
	private readonly static ReleaseType Release;

	public enum OS { Windows, Linux }
	public enum ReleaseType { Develop, Staging, Production }
	public enum Component { Core, Hooks, Extension, Miscellaneous }

	static Updater()
	{
		Platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) switch
		{
			true => OSPlatform.Windows,
			false => OSPlatform.Linux
		};

		Release =
#if DEBUG
		ReleaseType.Develop;
#else
        Release.Production;
#endif
	}

	private static string GenerateDownloadURL(string repository, OS os, ReleaseType release)
	{
		string tag = string.Empty;
		string file = string.Empty;
		string target = string.Empty;

		switch (release)
		{
			case ReleaseType.Develop:
				tag = "develop_build";
				target = "Debug";
				break;

			case ReleaseType.Staging:
				tag = "staging_build";
				target = "Debug";
				break;

			case ReleaseType.Production:
				tag = "production_build";
				target = "Release";
				break;
		}

		switch (os)
		{
			case OS.Windows:
				file = $"Carbon.{target}.zip";
				break;

			case OS.Linux:
				file = $"Carbon.{target}Unix.tar.gz";
				break;
		}

		return $"https://github.com/{repository}/releases/download/{tag}/{file}";
	}

	private static string GetGithubRawURL(string repository, string file, OS os, ReleaseType release)
	{
		string branch = string.Empty;
		string suffix = string.Empty;
		string target = string.Empty;

		switch (release)
		{
			case ReleaseType.Develop:
				branch = "main";
				target = "Debug";
				break;

			case ReleaseType.Staging:
				branch = "main";
				target = "Debug";
				break;

			case ReleaseType.Production:
				branch = "main";
				target = "Release";
				break;
		}

		switch (os)
		{
			case OS.Windows:
				suffix = string.Empty;
				break;

			case OS.Linux:
				suffix = $"Unix";
				break;
		}

		return $"https://github.com/{repository}/raw/{branch}/Release/{target}{suffix}/{file}";
	}

	internal static void UpdateFromGithub(Component component,
		string repository, object os, object release, IReadOnlyList<string> files, Action<bool> callback = null)
	{
		/*
		bool retval = false;

		string url = GenerateDownloadURL(repository, (OS)os, (Release)release);
		Logger.Warn($"Updating Carbon component '{component} [{os}]' from the '{release}' branch");

		Program.GetInstance().Downloader.DownloadAsync(url, (string identifier, byte[] buffer) =>
		{
			Logger.Warn($"Patch downloaded [{Path.GetExtension(url)}], processing {buffer.Length} bytes from memory");

			try
			{
				using MemoryStream archive = new MemoryStream(buffer);
				{
					using (IReader reader = ReaderFactory.Open(archive))
					{
						while (reader.MoveToNextEntry())
						{
							if (reader.Entry.IsDirectory || !files.Contains(reader.Entry.Key, StringComparer.OrdinalIgnoreCase)) continue;
							string destination = Path.GetFullPath(Path.Combine(Context.Directories.Game, reader.Entry.Key));

							using (EntryStream entry = reader.OpenEntryStream())
							{
								using (var fs = new FileStream(destination, FileMode.OpenOrCreate))
								{
									Logger.Debug($" - Updated {destination}");
									entry.CopyTo(fs);
								}
							}
						}
					}
				}
				retval = true;
			}
			catch (System.Exception e)
			{
				Logger.Error($"Error while updating {component}", e);
				retval = false;
			}
			finally
			{
				if (callback != null) callback(retval);
			}
		});
		*/
	}

	internal static void UpdateModuleFromGithub(Component component,
		string repository, object os, object release, string file, Action<bool> callback = null)
	{
		/*

				bool retval = false;
				string url = GenerateModuleDownloadURL(repository, (OS)os, (Release)release, file);
				Logger.Warn($"Updating Carbon component '{component} [{os}]' from the '{release}' branch");

				Program.GetInstance().Downloader.DownloadAsync(url, (string identifier, byte[] buffer) =>
				{
					Logger.Warn($"Patch downloaded [{Path.GetExtension(url)}], processing {buffer.Length} bytes from memory");

					try
					{
						using MemoryStream archive = new MemoryStream(buffer);
						{
							using (IReader reader = ReaderFactory.Open(archive))
							{
								while (reader.MoveToNextEntry())
								{
									if (reader.Entry.IsDirectory || !files.Contains(reader.Entry.Key, StringComparer.OrdinalIgnoreCase)) continue;


									using (EntryStream entry = reader.OpenEntryStream())
									{
										using (var fs = new FileStream(destination, FileMode.OpenOrCreate))
										{
											Logger.Debug($" - Updated {destination}");
											entry.CopyTo(fs);
										}
									}
								}
							}
						}
						retval = true;
					}
					catch (System.Exception e)
					{
						Logger.Error($"Error while updating {component}", e);
						retval = false;
					}
					finally
					{
						if (callback != null) callback(retval);
					}
				});
				*/
	}

	internal static void UpdateCarbon(object os, object release, Action<bool> callback = null)
	{
		IReadOnlyList<string> files = new List<string>(){
			@"carbon/managed/Carbon.Doorstop.dll",
			@"carbon/managed/Carbon.dll",
			@"harmonymods/Carbon.Loader.dll"
		};

		UpdateFromGithub(Component.Core,
			"CarbonCommunity/Carbon.Core", os, release, files, callback);
	}

	internal static void UpdateHooks(object os, object release, Action<bool> callback = null)
	{
		IReadOnlyList<string> files = new List<string>(){
			@"harmonymods/Carbon.Hooks.dll"
		};

		UpdateFromGithub(Component.Hooks,
			"CarbonCommunity/Carbon.Core", os, release, files, callback);
	}
}