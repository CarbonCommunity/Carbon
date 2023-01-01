
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SharpCompress.Common;
using SharpCompress.Readers;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

internal sealed class Updater
{
	private readonly static OSType Platform;
	private readonly static ReleaseType Release;
	private readonly static string Repository;

	public enum OSType { Windows, Linux }
	public enum ReleaseType { Develop, Staging, Production }

	static Updater()
	{
		Platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) switch
		{
			true => OSType.Windows,
			false => OSType.Linux
		};

		Repository = @"CarbonCommunity/Carbon.Redist";

		Release =
#if DEBUG
		ReleaseType.Develop;
#else
		Release.Production;
#endif
	}

	private static string GithubReleaseURL(string file)
	{
		string branch = string.Empty;
		string suffix = string.Empty;
		string target = string.Empty;

		switch (Release)
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

		switch (Platform)
		{
			case OSType.Windows:
				suffix = string.Empty;
				break;

			case OSType.Linux:
				suffix = $"Unix";
				break;
		}

		return $"https://raw.githubusercontent.com/{Repository}/{branch}/Modules/{target}{suffix}/{file}";
	}

	internal static void DoUpdate(Action<bool> callback = null)
	{
		IReadOnlyList<string> files = new List<string>(){
			@"carbon/managed/Carbon.Hooks.dll"
		};

		bool retval = false;

		foreach (string file in files)
		{
			string url = GithubReleaseURL(file);
			Logger.Warn($"Updating component '{file} [{Platform}]' using the '{Release}' branch");

			Carbon.Supervisor.ASM.Download(url, (string identifier, byte[] buffer) =>
			{
				if (buffer == null) throw new Exception("buffer is null");

				Logger.Warn($"Patch downloaded [{Path.GetExtension(url)}], processing {buffer.Length} bytes from memory");
				string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

				try
				{
					string destination = Path.GetFullPath(Path.Combine(root, file));
					File.WriteAllBytes(destination, buffer);
					retval = true;
				}
				catch (System.Exception e)
				{
					Logger.Error($"Error while updating component '{file} [{Platform}]'", e);
					retval = false;
				}
				finally
				{
					if (callback != null) callback(retval);
				}
			});
		}
	}
}