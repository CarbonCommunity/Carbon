using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
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

	private const string StagedSuffix = ".new";
	private const string ReplacedSuffix = ".old";

	private static Uri endpointUri = new(Endpoint);

	private static Uri downloadEndpointUri = new(DownloadEndpoint);

	private static WebClient apiClient;

	private static WebClient updateClient;

	private static bool updateInstalled;

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
		if (updateInstalled)
		{
			Debug.LogWarning(" An update is already installed. Restart the server to load it.");
			return;
		}

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
				Debug.Log("Updating Carbon.Profiler..");
#if UNIX
				updateInstalled = Install(ReadTarGz(args.Result));
#else
				updateInstalled = Install(ReadZip(args.Result));
#endif
				if (updateInstalled)
				{
					callback?.Invoke();
				}
			}
			else
			{
				Debug.LogError($" Couldn't download the update: {args.Error.Message}");
			}

			updateClient.Dispose();
			updateClient = null;
		};
		updateClient.DownloadDataAsync(downloadEndpointUri);
	}

	/// <summary>
	/// Writes every file next to its destination first, then renames it into place. The file it replaces
	/// is renamed rather than overwritten: Windows locks the loaded native library, and overwriting it
	/// in place on Linux changes code that the running server has mapped.
	/// </summary>
	private static bool Install(List<(string Path, byte[] Data)> files)
	{
		var destinations = new List<string>();

		try
		{
			foreach (var (path, data) in files)
			{
				var destination = Path.Combine(HarmonyLoader.modPath, path);
				Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
				File.WriteAllBytes(destination + StagedSuffix, data);
				destinations.Add(destination);
			}
		}
		catch (Exception e)
		{
			Debug.LogError($" Couldn't write the update, nothing was replaced: {e.Message}");
			destinations.ForEach(destination => File.Delete(destination + StagedSuffix));
			return false;
		}

		var installed = true;

		for (var i = 0; i < destinations.Count; i++)
		{
			var destination = destinations[i];
			var replaced = destination + ReplacedSuffix;

			try
			{
				File.Delete(replaced);

				if (File.Exists(destination))
				{
					File.Move(destination, replaced);
				}

				File.Move(destination + StagedSuffix, destination);
				Debug.Log($" - {files[i].Path} ({files[i].Data.Length.FormatBytes(true)})");
			}
			catch (Exception e)
			{
				installed = false;
				Debug.LogError($" - {files[i].Path} [failed]: {e.Message}");
				continue;
			}

			DeleteUnlessLoaded(replaced);
		}

		return installed;
	}

	private static void DeleteUnlessLoaded(string path)
	{
		try
		{
			File.Delete(path);
		}
		catch (Exception e) when (e is IOException or UnauthorizedAccessException)
		{
		}
	}

	private static List<(string Path, byte[] Data)> ReadZip(byte[] archive)
	{
		using var zip = new ZipArchive(new MemoryStream(archive), ZipArchiveMode.Read);

		return zip.Entries.Where(entry => !string.IsNullOrEmpty(entry.Name)).Select(entry =>
		{
			using var stream = entry.Open();
			using var data = new MemoryStream();
			stream.CopyTo(data);
			return (entry.FullName, data.ToArray());
		}).ToList();
	}

	/// <summary>
	/// Reads the regular files of a gzipped tar archive. Extended headers (PAX, GNU) and directories are
	/// skipped; the release archive keeps every path short enough for the ustar name fields.
	/// </summary>
	private static List<(string Path, byte[] Data)> ReadTarGz(byte[] archive)
	{
		var files = new List<(string Path, byte[] Data)>();
		using var tar = new GZipStream(new MemoryStream(archive), CompressionMode.Decompress);
		var header = new byte[512];

		while (ReadExactly(tar, header) && header[0] != 0)
		{
			var size = Convert.ToInt64(ReadField(header, 124, 12), 8);
			var data = new byte[size];
			ReadExactly(tar, data);
			ReadExactly(tar, new byte[(512 - size % 512) % 512]);

			var type = (char)header[156];

			if (type is '0' or '\0')
			{
				var prefix = ReadField(header, 345, 155);
				var name = ReadField(header, 0, 100);
				files.Add((prefix.Length == 0 ? name : $"{prefix}/{name}", data));
			}
		}

		return files;
	}

	private static string ReadField(byte[] header, int offset, int length)
	{
		var end = Array.IndexOf(header, (byte)0, offset, length);
		return Encoding.UTF8.GetString(header, offset, (end < 0 ? offset + length : end) - offset).Trim();
	}

	private static bool ReadExactly(Stream stream, byte[] buffer)
	{
		var read = 0;

		while (read < buffer.Length)
		{
			var count = stream.Read(buffer, read, buffer.Length - read);

			if (count == 0)
			{
				return false;
			}

			read += count;
		}

		return true;
	}
}
