using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using Facepunch.Extend;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Carbon;

public class SelfUpdate
{
	public const string Endpoint = "https://api.carbonmod.gg/releases";

	public static readonly string DownloadEndpoint =
#if UNIX
		"https://github.com/CarbonCommunity/Carbon/releases/download/profiler_build/Carbon.Linux.Profiler.tar.gz";
#else
		"https://github.com/CarbonCommunity/Carbon/releases/download/profiler_build/Carbon.Windows.Profiler.zip";
#endif

	public static readonly System.Version CurrentVersion = typeof(SelfUpdate).Assembly.GetName().Version;

	private static Uri endpointUri = new(Endpoint);

	private static Uri downloadEndpointUri = new(DownloadEndpoint);

	private static WebClient apiClient;

	private static WebClient updateClient;

	public static void Api(Action<JArray> callback)
	{
		if (apiClient == null)
		{
			apiClient = new();
			apiClient.DownloadStringCompleted += (sender, args) =>
			{
				if (args.Error != null)
				{
					Debug.LogException(args.Error);
					return;
				}
				callback?.Invoke(JArray.Parse(args.Result));
			};
		}
		apiClient.DownloadStringAsync(endpointUri);
	}

	public static void Update(Action callback)
	{
		if (updateClient != null)
		{
			Debug.LogWarning(" Update already in progress..");
			return;
		}

		updateClient = new();
		updateClient.DownloadDataCompleted += (sender, args) =>
		{
			if (args.Error == null)
			{
				using var stream = new MemoryStream(args.Result);
				Debug.Log("Updating Carbon.Profiler..");
				var root = HarmonyLoader.modPath;
				using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
				foreach (var entry in archive.Entries)
				{
					if (string.IsNullOrEmpty(entry.Name))
					{
						continue;
					}

					var destination = Path.Combine(root, entry.FullName);
					Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

					using var entryStream = entry.Open();
					try
					{
						using var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write);
						entryStream.CopyTo(fileStream);
						Debug.Log($" - {entry.FullName} ({entry.Length.FormatBytes(true)})");
					}
					catch (Exception e)
					{
						Debug.LogWarning($" - {entry.FullName} ({entry.Length.FormatBytes(true)}) [skipped]: {e.Message}");
					}

				}
				callback?.Invoke();
			}

			updateClient.Dispose();
			updateClient = null;
		};
		updateClient.DownloadDataAsync(downloadEndpointUri);
	}
}
