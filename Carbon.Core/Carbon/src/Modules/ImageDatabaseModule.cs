using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Net;
using Carbon.Base;
using Oxide.Core.Libraries;
using System.Text;
using Carbon.Extensions;
using static Oxide.Plugins.RustPlugin;
using System.Collections;
using Org.BouncyCastle.Utilities;
using Epic.OnlineServices;
using Facepunch;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public class ImageDatabaseModule : CarbonModule<ImageDatabaseConfig, ImageDatabaseData>
{
	public override string Name => "Image Database";
	public override Type Type => typeof(ImageDatabaseModule);
	public override bool EnabledByDefault => true;

	internal IEnumerator _executeQueue(QueuedThread thread, Action<List<QueuedThreadResult>> onFinished)
	{
		thread.Start();

		while (thread != null && !thread.IsDone) { yield return null; }

		onFinished?.Invoke(thread.Result);
	}

	private void OnServerInitialized()
	{
		if (Validate())
		{
			Save();
		}
	}

	public override bool PreLoadShouldSave()
	{
		var shouldSave = false;

		if (DataInstance.Map == null)
		{
			DataInstance.Map = new Dictionary<string, uint>();
			shouldSave = true;
		}

		return shouldSave;
	}

	public bool Validate()
	{
		if (DataInstance.Identifier != CommunityEntity.ServerInstance.net.ID)
		{
			PutsWarn($"The server identifier has changed. Wiping old image database.");
			DataInstance.Map.Clear();
			DataInstance.Identifier = CommunityEntity.ServerInstance.net.ID;
			return true;
		}

		var invalidations = Pool.GetList<string>();

		foreach (var pointer in DataInstance.Map)
		{
			if(FileStorage.server.Get(pointer.Value, FileStorage.Type.png, DataInstance.Identifier) == null)
			{
				invalidations.Add(pointer.Key);
			}
		}

		foreach(var invalidation in invalidations)
		{
			DataInstance.Map.Remove(invalidation);
		}

		var invalidated = invalidations.Count > 0;
		Pool.FreeList(ref invalidations);

		return invalidated;
	}

	public void QueueBatch(params string[] urls)
	{
		QueueBatch(1f, urls);
	}
	public void QueueBatch(float scale, params string[] urls)
	{
		QueueBatch(scale, results =>
		{
			foreach(var result in results)
			{
				if(result.OriginalData == result.ProcessedData)
				{
					var id = FileStorage.server.Store(result.ProcessedData, FileStorage.Type.png, DataInstance.Identifier);
					DataInstance.Map.Add($"{result.Url}_0", id);
				}
				else
				{
					var originalId = FileStorage.server.Store(result.OriginalData, FileStorage.Type.png, DataInstance.Identifier);
					var processedId = FileStorage.server.Store(result.ProcessedData, FileStorage.Type.png, DataInstance.Identifier);
					DataInstance.Map.Add($"{result.Url}_0", originalId);
					DataInstance.Map.Add($"{result.Url}_{scale:0.0}", processedId);
				}
			}
		}, urls);
	}
	public void QueueBatch(float scale, Action<List<QueuedThreadResult>> onComplete, params string[] urls)
	{
		var thread = new QueuedThread
		{
			Scale = scale
		};
		thread.ImageUrls.AddRange(urls);

		foreach (var url in urls)
		{
			DeleteImage(url, 0);
			if (scale != 0f) DeleteImage(url, scale);
		}

		Community.Runtime.CorePlugin.persistence.StartCoroutine(_executeQueue(thread, results => onComplete?.Invoke(results)));
	}

	public uint GetImage(string url, float scale = 0)
	{
		var id = scale == 0 ? "0" : scale.ToString("0.0");

		if (DataInstance.Map.TryGetValue($"{url}_{id}", out var uid))
		{
			return uid;
		}

		return 0;
	}
	public bool DeleteImage(string url, float scale = 0)
	{
		var id = scale == 0 ? "0" : scale.ToString("0.0");
		var name = $"{url}_{id}";

		if (DataInstance.Map.TryGetValue(name, out var uid))
		{
			FileStorage.server.Remove(uid, FileStorage.Type.png, DataInstance.Identifier);
			DataInstance.Map.Remove(name);
			return true;
		}

		return false;
	}

	public class QueuedThread : BaseThreadedJob, IDisposable
	{
		public List<string> ImageUrls { get; internal set; } = new();
		public float Scale { get; set; } = 1f;

		public List<QueuedThreadResult> Result { get; internal set; } = new();

		internal Queue<string> _urlQueue = new();
		internal int _processed;
		internal WebRequests _webRequests;
		internal WebRequests.WebRequest.Client _client;
		internal bool _finishedProcessing;

		public override void Start()
		{
			base.Start();

			_webRequests = new WebRequests();
			foreach (var url in ImageUrls) { _urlQueue.Enqueue(url); }
		}
		public override void ThreadFunction()
		{
			base.ThreadFunction();

			_client = new WebRequests.WebRequest.Client();
			{
				_client.Headers.Add("User-Agent", $"Carbon ImageDatabase (v{Community.Version}); https://github.com/Carbon-Modding/Carbon.Core");
				_client.Credentials = CredentialCache.DefaultCredentials;
				_client.Proxy = null;

				_client.DownloadDataCompleted += (object sender, DownloadDataCompletedEventArgs e) =>
				{
					Console.WriteLine($"{e.Error}");

					_processed++;
					Result.Add(new QueuedThreadResult
					{
						Url = (string)e.UserState,
						OriginalData = e.Result,
						ProcessedData = e.Result
					});

					_doQueue();
				};

				_doQueue();
			}

			while (_processed != ImageUrls.Count)
			{
				continue;
			}

			_client.Dispose();
			_client = null;

			_processImages();

			while (!_finishedProcessing)
			{
				continue;
			}
		}
		public override void Dispose()
		{
			ImageUrls.Clear();
			Result.Clear();
			Scale = default;
			_urlQueue.Clear();
			_client?.Dispose();
			_webRequests = null;
			_finishedProcessing = default;

			ImageUrls = null;
			Result = null;
			_urlQueue = null;

			base.Dispose();
		}

		internal void _doQueue()
		{
			if (_urlQueue.Count == 0) return;

			try
			{
				var pick = _urlQueue.Dequeue();
				_client.DownloadDataAsync(new Uri(pick), pick);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_processed++;
			}
		}
		internal void _processImages()
		{
			if (Scale == 1f)
			{
				_finishedProcessing = true;
				return;
			}

			foreach (var result in Result)
			{
				try
				{
					using var stream = new MemoryStream(result.OriginalData);
					using var image = Image.FromStream(stream);
					using var graphics = Graphics.FromImage(image);
					using var resized = new Bitmap((int)(image.Width * Scale), (int)(image.Height * Scale));
					resized.MakeTransparent();
					using var resizedGraphic = Graphics.FromImage(resized);
					resizedGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
					resizedGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
					resizedGraphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
					resizedGraphic.DrawImage(image, new Rectangle(0, 0, (int)(image.Width * Scale), (int)(image.Height * Scale)));
					using var output = new MemoryStream();
					resized.Save(output, ImageFormat.Png);
					resized.Dispose();
					result.ProcessedData = output.ToArray();
				}
				catch { }
			}

			_finishedProcessing = true;
		}
	}

	public class QueuedThreadResult : IDisposable
	{
		public string Url { get; set; }
		public byte[] OriginalData { get; set; }
		public byte[] ProcessedData { get; set; }

		public void Dispose()
		{
			OriginalData = null;
			ProcessedData = null;
		}
	}
}

public class ImageDatabaseConfig
{
}
public class ImageDatabaseData
{
	public uint Identifier { get; set; }
	public Dictionary<string, uint> Map { get; set; }
}
