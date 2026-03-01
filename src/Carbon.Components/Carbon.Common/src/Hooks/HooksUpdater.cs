namespace Carbon.Hooks;

public sealed class Updater
{
	private static string BuildUrl(string file, string protocol = null)
	{
		var suffix = Community.Runtime.Analytics.Platform == "linux" ? "unix" : null;
		var target = Community.Runtime.Analytics.Branch == "Release" ? "release" : "debug";
		return $"https://cdn.carbonmod.gg/hooks/server/{target}{suffix}/{(protocol is null ? $"{file}" : $"{protocol}/{file}")}";
	}

	public static void DoUpdate(Action<bool> callback = null)
	{
		FireAndForget(DoUpdateInternalAsync(callback));
	}

	private static async Task DoUpdateInternalAsync(Action<bool> callback = null)
	{
		var success = false;
		try
		{
			// FIXME: the update process is triggering carbon init process twice
			// when more than one file is listed here to be downloaded [and] one of
			// them fails with 404.
			IReadOnlyList<string> files =
			[
				"carbon/managed/hooks/Carbon.Hooks.Community.dll",
				"carbon/managed/hooks/Carbon.Hooks.Oxide.dll"
			];

			List<Task<bool>> tasks = [];
			foreach (var file in files)
			{
				tasks.Add(UpdateFileAsync(file));
			}

			var failed = 0;
			var results = await Task.WhenAll(tasks);
			foreach (var result in results)
			{
				if (!result)
					failed++;
			}

			success = failed == 0;
		}
		catch (Exception e)
		{
			Logger.Error("Hook update failed", e);
			success = false;
		}
		finally
		{
			if (callback != null)
			{
				try
				{
					callback(success);
				}
				catch (Exception e)
				{
					Logger.Error("Hook update callback failed", e);
				}
			}
		}
	}

	private static void FireAndForget(Task task)
	{
		_ = task.ContinueWith(t =>
		{
			Logger.Error("Hook update task failed", t.Exception);
		}, TaskContinuationOptions.OnlyOnFaulted);
	}

	private static async Task<bool> UpdateFileAsync(string file)
	{
		var fileName = Path.GetFileName(file);
		try
		{
			if (!Community.Runtime.Config.Logging.ReducedLogging)
			{
				Logger.Warn($"Updating component '{fileName}@{Community.Runtime.Analytics.Protocol}' on {Community.Runtime.Analytics.Platform} [{Community.Runtime.Analytics.Branch}]");
			}

			var buffer = await DownloadFile(file, Community.Runtime.Analytics.Protocol);

			if (buffer == null || buffer.Length < 1)
			{
				Logger.Warn($"Retrying component update '{fileName}' on {Community.Runtime.Analytics.Platform} [{Community.Runtime.Analytics.Branch}]...");
				buffer = await DownloadFile(file);
			}

			if (buffer == null || buffer.Length < 1)
			{
				Logger.Warn($"Unable to update component '{fileName}', please try again later");
				return false;
			}

			try
			{
				File.WriteAllBytes(Path.Combine(Defines.GetHooksFolder(), fileName), buffer);
				return true;
			}
			catch (Exception e)
			{
				Logger.Error($"Error while updating component '{fileName}'", e);
				return false;
			}
		}
		catch (Exception e)
		{
			Logger.Error($"Unexpected error while updating component '{fileName}'", e);
			return false;
		}
	}

	private static async Task<byte[]> DownloadFile(string file, string protocol = null)
	{
		var timeoutSpan = TimeSpan.FromSeconds(10);
		using var timeoutCts = new CancellationTokenSource(timeoutSpan);
		var url = BuildUrl(file, protocol);
		var buffer = await Community.Runtime.Downloader.Download(url, timeoutCts.Token);
		if (timeoutCts.IsCancellationRequested)
		{
			Logger.Warn($"Timed out downloading '{file}' after {timeoutSpan.Seconds}s");
		}

		return buffer;
	}
}
