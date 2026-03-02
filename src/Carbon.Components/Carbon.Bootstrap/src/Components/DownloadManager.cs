using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using API.Abstracts;
using API.Contracts;
using Carbon;
using Logger = Utility.Logger;

namespace Components;
#pragma warning disable IDE0051

internal sealed class DownloadManager : CarbonBehaviour, IDownloadManager
{
	private sealed class DownloadItem
	{
		public string URL;
		public DateTime Start;
		public string Identifier;
		public Action<string, byte[]> Callback;
		public CancellationToken Token;
		public bool SuppressErrors;
		public CancellationTokenRegistration Registration;
		public WebClient Client;
		public bool Started;
		public bool Completed;
	}

	private const int Concurrency = 4;
	private Queue<DownloadItem> _downloadQueue;
	private int _currentDownloads;
	private string _userAgent;
	private readonly object _downloadSync = new();

	private void Awake()
	{
		_downloadQueue = new Queue<DownloadItem>();
		try { _userAgent = $"Carbon v{Assembly.GetExecutingAssembly().GetName().Version}"; }
		catch
		{
			// ignored
		}

		ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
		if (Enum.TryParse("Tls13", out SecurityProtocolType tls13))
		{
			ServicePointManager.SecurityProtocol |= tls13;
		}
	}

	private void Update()
	{
		while (_downloadQueue.Count > 0 && _currentDownloads < Concurrency)
		{
			var job = _downloadQueue.Dequeue();
			if (job.Token.IsCancellationRequested) continue;

			var webClient = new WebClient();
			webClient.DownloadDataCompleted += OnDownloadDataCompleted;
			webClient.Headers.Add(HttpRequestHeader.UserAgent, _userAgent);
			webClient.Headers.Add(HttpRequestHeader.CacheControl, "no-store, no-cache, must-revalidate, max-age=0");
			webClient.Headers.Add(HttpRequestHeader.Pragma, "no-cache");
			job.Client = webClient;

			job.Registration = job.Token.Register(() => CancelJob(job));

			if (job.Token.IsCancellationRequested)
			{
				CancelJob(job);
				continue;
			}

			lock (_downloadSync)
			{
				job.Started = true;
				_currentDownloads++;
				job.Start = DateTime.UtcNow;
			}

			try
			{
				webClient.DownloadDataAsync(new Uri(job.URL), job);
			}
			catch (Exception ex)
			{
				CompleteJob(job, webClient, null, ex, false);
			}
		}
	}

	private void OnDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
	{
		var webClient = (WebClient)sender;
		var job = (DownloadItem)e.UserState;

		if (e.Cancelled || job.Token.IsCancellationRequested)
		{
			CompleteJob(job, webClient, null, null, true);
			return;
		}

		if (e.Error != null)
		{
			CompleteJob(job, webClient, null, e.Error, false);
			return;
		}

		CompleteJob(job, webClient, e.Result, null, false);
	}

	private void CancelJob(DownloadItem job)
	{
		var webClient = job.Client;
		try { webClient?.CancelAsync(); }
		catch (Exception)
		{
			// ignored
		}

		CompleteJob(job, webClient, null, null, true);
	}

	private void CompleteJob(DownloadItem job, WebClient webClient, byte[] result, Exception error, bool cancelled)
	{
		lock (_downloadSync)
		{
			if (job.Completed) return;
			job.Completed = true;
			if (job.Started)
			{
				_currentDownloads--;
			}
		}

		try
		{
			if (cancelled) return;
			if (error != null)
			{
				if (!job.SuppressErrors)
					Logger.Error($"Download job '{job.URL}' failed", error);

				if (job.Callback != null)
					job.Callback(job.Identifier, []);
				else
					Logger.Error("Download callback is null");
			}
			else
			{
				if (!Community.Runtime.Config.Logging.ReducedLogging && result != null)
				{
					var ts = DateTime.UtcNow - job.Start;
					Logger.Log($"Download job '{job.URL}' finished [{FormatBytes(result.LongLength / ts.TotalSeconds)}/sec]");
				}

				if (job.Callback != null)
					job.Callback(job.Identifier, result);
				else
					Logger.Error("Download callback is null");
			}
		}
		finally
		{
			job.Registration.Dispose();
			webClient?.Dispose();
		}
	}

	public Task<byte[]> Download(string url, CancellationToken token)
	{
		return Download(url, token, false);
	}

	public async Task<byte[]> Download(string url, CancellationToken token, bool suppressErrors)
	{
		var tcs = new TaskCompletionSource<byte[]>();
		using (token.Register(() => tcs.TrySetCanceled()))
		{
			var job = new DownloadItem
			{
				URL = url,
				Callback = (_, bytes) => tcs.TrySetResult(bytes),
				Identifier = $"{Guid.NewGuid():N}",
				Token = token,
				SuppressErrors = suppressErrors
			};

			_downloadQueue.Enqueue(job);

			try
			{
				return await tcs.Task;
			}
			catch (TaskCanceledException)
			{
				return null;
			}
		}
	}

	public Task<byte[]> Download(string url)
	{
		return Download(url, CancellationToken.None);
	}

	public void DownloadAsync(string url, Action<string, byte[]> callback)
	{
		var job = new DownloadItem { URL = url, Callback = callback, Identifier = $"{Guid.NewGuid():N}", Token = CancellationToken.None };

		_downloadQueue.Enqueue(job);
	}

	private static string FormatBytes(double bytes)
	{
		string arg;

		if (bytes > 1048576.0)
		{
			arg = "mb";
			bytes /= 1048576.0;
		}
		else if (bytes > 1024.0)
		{
			arg = "kb";
			bytes /= 1024.0;
		}
		else
		{
			arg = "b";
		}

		return $"{bytes:0}{arg}";
	}
}
