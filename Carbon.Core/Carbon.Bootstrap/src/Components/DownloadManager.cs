using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using API.Contracts;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;

internal sealed class DownloadManager : MonoBehaviour, IDownloadManager
{
	private struct DownloadItem
	{
		public string URL;
		public DateTime Start;
		public string Identifier;
		public Action<string, byte[]> Callback;
	}

	private static readonly int _concurrency = 3;
	private Queue<DownloadItem> _donwloadQueue;
	private int _currentDownloads;
	private string _userAgent;

	private void Awake()
	{
		_donwloadQueue = new Queue<DownloadItem>();
		try { _userAgent = $"Carbon v{Assembly.GetExecutingAssembly().GetName().Version}"; } catch { }
	}

	private void Update()
	{
		if (_donwloadQueue.Count == 0 || _currentDownloads > _concurrency) return;

		WebClient webClient = new WebClient();
		webClient.DownloadDataCompleted += OnDownloadDataCompleted;
		webClient.Headers.Add(HttpRequestHeader.UserAgent, _userAgent);

		DownloadItem job = _donwloadQueue.Dequeue();
		job.Start = DateTime.UtcNow;
		_currentDownloads++;

		Utility.Logger.Log($"Download job '{job.Identifier}' started");
		webClient.DownloadDataAsync(address: new Uri(job.URL), job);
	}

	private void OnDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
	{
		_currentDownloads--;
		WebClient webClient = (WebClient)sender;
		DownloadItem job = (DownloadItem)e.UserState;

		try
		{
			if (e.Error != null) throw new Exception(e.Error.Message);
			if (e.Cancelled) throw new Exception("Job was cancelled");

			TimeSpan ts = DateTime.UtcNow - ((DownloadItem)e.UserState).Start;
			Utility.Logger.Log($"Download job '{job.Identifier}' finished [{FormatBytes(e.Result.LongLength / ts.TotalSeconds):0}/sec]");

			if (job.Callback == null) throw new Exception("Callback is null, this is a bug");
			job.Callback(job.Identifier, e.Result);
		}
		catch (Exception ex)
		{
			Utility.Logger.Error($"Download job '{job.Identifier}' failed", ex);

			if (job.Callback == null) throw new Exception("Callback is null, this is a bug");
			job.Callback(job.Identifier, new byte[] { });
		}
		finally
		{
			webClient.Dispose();
		}
	}

	public async Task<byte[]> Download(string url)
	{
		var tcs = new TaskCompletionSource<byte[]>();

		DownloadItem job = new DownloadItem()
		{
			URL = url,
			Callback = (id, bytes) => tcs.SetResult(bytes),
			Identifier = $"{Guid.NewGuid():N}",
		};

		Utility.Logger.Log($"New download request with token '{job.Identifier}': {job.URL}");
		_donwloadQueue.Enqueue(job);
		byte[] bytes = await tcs.Task;

		return bytes;
	}

	public void DownloadAsync(string url, Action<string, byte[]> callback)
	{
		DownloadItem job = new DownloadItem()
		{
			URL = url,
			Callback = callback,
			Identifier = $"{Guid.NewGuid():N}",
		};

		Utility.Logger.Log($"New async download request with token '{job.Identifier}': {job.URL}");
		_donwloadQueue.Enqueue(job);
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
		return string.Format("{0:0}{1}", bytes, arg);
	}
}
