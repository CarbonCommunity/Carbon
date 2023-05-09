
using System;
using System.Collections.Generic;
using System.IO;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public sealed class Updater
{
	private static readonly string Repository
		= @"CarbonCommunity/Carbon.Redist";

	private static string GithubReleaseUrl(string file, string protocol = null)
	{
		string branch = "main";
		string suffix = (Community.Runtime.Analytics.Platform == "linux") ? "Unix" : default;
		string target = (Community.Runtime.Analytics.Branch == "Release") ? "Release" : "Debug";

		return $"https://raw.githubusercontent.com/{Repository}/{branch}/Modules/"
			+ $"{target}{suffix}/{(protocol is null ? $"{file}" : $"/{protocol}/{file}")}";
	}

	public static async void DoUpdate(Action<bool> callback = null)
	{
		// FIXME: the update process is triggering carbon init process twice
		// when more than one file is listed here to be downloaded [and] one of
		// them fails with 404.
		IReadOnlyList<string> files = new List<string>(){
			@"carbon/managed/hooks/Carbon.Hooks.Extra.dll"
		};

		int failed = 0;
		foreach (string file in files)
		{
			Logger.Warn($"Updating component '{Path.GetFileName(file)}@{Community.Runtime.Analytics.Protocol}' using the "
				+ $"'{Community.Runtime.Analytics.Branch} [{Community.Runtime.Analytics.Platform}]' branch");
			byte[] buffer = await Community.Runtime.Downloader.Download(GithubReleaseUrl(file, Community.Runtime.Analytics.Protocol));

			if (buffer is { Length: < 1 })
			{
				Logger.Warn($"[Retry updating component '{Path.GetFileName(file)}' using the "
					+ $"'{Community.Runtime.Analytics.Branch} [{Community.Runtime.Analytics.Platform}]' branch");
				buffer = await Community.Runtime.Downloader.Download(GithubReleaseUrl(file));
			}

			if (buffer is { Length: < 1 })
			{
				Logger.Warn($"Unable to update component '{Path.GetFileName(file)}', please try again later");
				failed++; continue;
			}

			try
			{
				string destination = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file));
				File.WriteAllBytes(destination, buffer);
			}
			catch (System.Exception e)
			{
				Logger.Error($"Error while updating component '{Path.GetFileName(file)}'", e);
				failed++;
			}
		}
		callback?.Invoke(failed == 0);
	}

	// private static async Task<string[]> APIGetFileList()
	// {
	// 	string suffix = (Community.Runtime.Analytics.Platform == "linux") ? "Unix" : default;
	// 	string target = (Community.Runtime.Analytics.Branch == "Release") ? "Release" : "Debug";
	// 	string url = $"https://api.github.com/repos/{Repository}/contents";

	// 	byte[] buffer = await Community.Runtime.Downloader.Download($"{url}/Modules/{target}{suffix}");

	// 	if (buffer is { Length: > 0 })
	// 	{
	// 		string json = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
	// 		List<GithubAPIContents> result = JsonConvert.DeserializeObject<List<GithubAPIContents>>(json);
	// 		return result.Where(x => Regex.IsMatch(x.Name, @"^[\d]{8}$"))
	// 			.OrderByDescending(x => x.Name).Select(x => x.Path).ToArray();
	// 	}
	// 	return default;
	// }

	// public partial class GithubAPIContents
	// {
	// 	[JsonProperty("name")]
	// 	public string Name { get; set; }

	// 	[JsonProperty("path")]
	// 	public string Path { get; set; }

	// 	[JsonProperty("sha")]
	// 	public string Sha { get; set; }

	// 	[JsonProperty("size")]
	// 	public long Size { get; set; }

	// 	[JsonProperty("url")]
	// 	public Uri Url { get; set; }

	// 	[JsonProperty("html_url")]
	// 	public Uri HtmlUrl { get; set; }

	// 	[JsonProperty("git_url")]
	// 	public Uri GitUrl { get; set; }

	// 	[JsonProperty("download_url")]
	// 	public object DownloadUrl { get; set; }

	// 	[JsonProperty("type")]
	// 	public string Type { get; set; }

	// 	[JsonProperty("_links")]
	// 	public GithubAPILinks Links { get; set; }

	// 	public partial class GithubAPILinks
	// 	{
	// 		[JsonProperty("self")]
	// 		public Uri Self { get; set; }

	// 		[JsonProperty("git")]
	// 		public Uri Git { get; set; }

	// 		[JsonProperty("html")]
	// 		public Uri Html { get; set; }
	// 	}
	// }
}
