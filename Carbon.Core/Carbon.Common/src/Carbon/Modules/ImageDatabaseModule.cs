using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using Carbon.Base;
using Carbon.Core;
using Carbon.Extensions;
using Facepunch;
using Oxide.Core.Libraries;
using ProtoBuf;
using QRCoder;

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

	internal ImageDatabaseDataProto _protoData { get; set; }

	internal string[] DefaultImages = new string[]
	{
		"https://carbonmod.gg/assets/media/carbonlogo_b.png",
		"https://carbonmod.gg/assets/media/carbonlogo_w.png",
		"https://carbonmod.gg/assets/media/carbonlogo_bs.png",
		"https://carbonmod.gg/assets/media/carbonlogo_ws.png",
		"https://carbonmod.gg/assets/media/cui/checkmark.png"
	};
	internal IEnumerator _executeQueue(QueuedThread thread, Action<List<QueuedThreadResult>> onFinished)
	{
		thread.Start();

		while (thread != null && !thread.IsDone) { yield return null; }

		onFinished?.Invoke(thread.Result);
	}
	internal string _getProtoDataPath()
	{
		return Path.Combine(Defines.GetModulesFolder(), Name, "data.db");
	}

	private void OnServerInitialized()
	{
		if (Validate())
		{
			Save();
		}

		LoadDefaultImages();
	}
	private void OnServerSave()
	{
		SaveDatabase();
	}

	public override void Load()
	{
		var path = _getProtoDataPath();

		if (OsEx.File.Exists(path))
		{
			using var stream = new MemoryStream(OsEx.File.ReadBytes(path));
			try { _protoData = Serializer.Deserialize<ImageDatabaseDataProto>(stream); } catch { _protoData = new ImageDatabaseDataProto(); }
		}
		else
		{
			_protoData = new ImageDatabaseDataProto();
		}

		base.Load();
	}
	public override void Save()
	{
		base.Save();

		SaveDatabase();
	}
	public void SaveDatabase()
	{
		var path = _getProtoDataPath();
		OsEx.Folder.Create(Path.GetDirectoryName(path));

		using var file = System.IO.File.OpenWrite(path);
		Serializer.Serialize(file, _protoData ??= new ImageDatabaseDataProto());
		file.Flush();
		file.Dispose();
	}
	private void LoadDefaultImages()
	{
		QueueBatch(false, DefaultImages.ToArray());
		AddMap("carbonb", "https://carbonmod.gg/assets/media/carbonlogo_b.png");
		AddMap("carbonw", "https://carbonmod.gg/assets/media/carbonlogo_w.png");
		AddMap("carbonbs", "https://carbonmod.gg/assets/media/carbonlogo_bs.png");
		AddMap("carbonws", "https://carbonmod.gg/assets/media/carbonlogo_ws.png");
		AddMap("checkmark", "https://carbonmod.gg/assets/media/cui/checkmark.png");
	}

	public override bool PreLoadShouldSave()
	{
		var shouldSave = false;

		if (_protoData.Map == null)
		{
			_protoData.Map = new Dictionary<string, uint>();
			shouldSave = true;
		}

		if (_protoData.CustomMap == null)
		{
			_protoData.CustomMap = new Dictionary<string, string>();
			shouldSave = true;
		}

		return shouldSave;
	}

	public bool Validate()
	{
		if (_protoData.Identifier != CommunityEntity.ServerInstance.net.ID)
		{
			PutsWarn($"The server identifier has changed. Wiping old image database.");
			_protoData.Map.Clear();
			_protoData.CustomMap.Clear();
			_protoData.Identifier = CommunityEntity.ServerInstance.net.ID;
			return true;
		}

		var invalidations = Pool.GetList<string>();

		foreach (var pointer in _protoData.Map)
		{
			if (FileStorage.server.Get(pointer.Value, FileStorage.Type.png, _protoData.Identifier) == null)
			{
				invalidations.Add(pointer.Key);
			}
		}

		foreach (var invalidation in invalidations)
		{
			_protoData.Map.Remove(invalidation);
		}

		var invalidated = invalidations.Count > 0;
		Pool.FreeList(ref invalidations);

		return invalidated;
	}

	public void QueueBatch(bool @override, params string[] urls)
	{
		QueueBatch(1f, @override, urls);
	}
	public void QueueBatch(float scale, bool @override, params string[] urls)
	{
		QueueBatch(scale, @override, results =>
		{
			foreach (var result in results)
			{
				if (result.OriginalData == result.ProcessedData)
				{
					var id = FileStorage.server.Store(result.ProcessedData, FileStorage.Type.png, _protoData.Identifier);
					_protoData.Map.Add($"{result.Url}_0", id);
				}
				else
				{
					var originalId = FileStorage.server.Store(result.OriginalData, FileStorage.Type.png, _protoData.Identifier);
					var processedId = FileStorage.server.Store(result.ProcessedData, FileStorage.Type.png, _protoData.Identifier);
					_protoData.Map.Add($"{result.Url}_0", originalId);
					_protoData.Map.Add($"{result.Url}_{scale:0.0}", processedId);
				}
			}
		}, urls);
	}
	public void QueueBatch(float scale, bool @override, Action<List<QueuedThreadResult>> onComplete, params string[] urls)
	{
		var thread = new QueuedThread
		{
			Scale = scale
		};
		thread.ImageUrls.AddRange(urls);

		if (!@override)
		{
			foreach (var url in urls)
			{
				if (GetImage(url, scale, true) != 0) thread.ImageUrls.Remove(url);
			}
		}
		else
		{
			foreach (var url in thread.ImageUrls)
			{
				DeleteImage(url, 0);
				if (scale != 0f) DeleteImage(url, scale);
			}
		}

		if (Config.PrintInitializedBatchLogs && thread.ImageUrls.Count > 0) Puts($"Added {thread.ImageUrls.Count:n0} to the queue (scale: {(scale == 0 ? "default" : $"{scale:0.0}")})...");

		Community.Runtime.CorePlugin.persistence.StartCoroutine(_executeQueue(thread, results =>
		{
			try
			{
				onComplete?.Invoke(results);
				if (Config.PrintCompletedBatchLogs && results.Count > 0) Puts($"Completed queue of {results.Count:n0} urls (scale: {(scale == 0 ? "default" : $"{scale:0.0}")}).");
			}
			catch (Exception ex)
			{
				PutsError($"Failed QueueBatch of {urls.Length:n0}.", ex);
			}
		}));
	}

	public void AddMap(string key, string url)
	{
		_protoData.CustomMap[key] = url;
	}
	public void RemoveMap(string key)
	{
		if (_protoData.CustomMap.ContainsKey(key)) _protoData.CustomMap.Remove(key);
	}

	public uint GetImage(string url, float scale = 0, bool silent = false)
	{
		if (_protoData.CustomMap.TryGetValue(url, out var realUrl))
		{
			url = realUrl;
		}

		var id = scale == 0 ? "0" : scale.ToString("0.0");

		if (_protoData.Map.TryGetValue($"{url}_{id}", out var uid))
		{
			if (!silent && Config.PrintCompletedBatchLogs) Puts($"Retrieved image '{url}' (scale: {(scale == 0 ? "default" : $"{scale:0.0}")}).");
			return uid;
		}

		return scale != 0 ? GetImage(url, 0, silent) : 0;
	}
	public string GetImageString(string url, float scale = 0, bool silent = false)
	{
		return GetImage(url, scale, silent).ToString();
	}
	public bool DeleteImage(string url, float scale = 0)
	{
		var id = scale == 0 ? "0" : scale.ToString("0.0");
		var name = $"{url}_{id}";

		if (_protoData.Map.TryGetValue(name, out var uid))
		{
			if (Config.PrintDeletedImageLogs) Puts($"Deleted image '{url}' (scale: {(scale == 0 ? "default" : $"{scale:0.0}")}).");

			FileStorage.server.Remove(uid, FileStorage.Type.png, _protoData.Identifier);
			_protoData.Map.Remove(name);
			return true;
		}

		return false;
	}

	public uint GetQRCode(string text, int pixels = 20, bool transparent = false, bool quietZones = true, bool whiteMode = false)
	{
		if (_protoData.Map.TryGetValue($"qr_{UiCommandAttribute.Uniquify(text)}_{pixels}_0", out uint uid)) return uid;

		if (text.StartsWith("http"))
		{
			PayloadGenerator.Url generator = new PayloadGenerator.Url(text);
			text = generator.ToString();
		}

		using (var qrGenerator = new QRCodeGenerator())
		using (var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
		using (var qrCode = new QRCode(qrCodeData))
		{
			var qrCodeImage = qrCode.GetGraphic(pixels, whiteMode ? Color.White : Color.Black, transparent ? Color.Transparent : whiteMode ? Color.Black : Color.White, quietZones);

			using var output = new MemoryStream();
			qrCodeImage.Save(output, ImageFormat.Png);
			qrCodeImage.Dispose();

			var raw = output.ToArray();
			uid = FileStorage.server.Store(raw, FileStorage.Type.png, _protoData.Identifier);
			_protoData.Map.Add($"qr_{UiCommandAttribute.Uniquify(text)}_{pixels}_0", uid);
			return uid;
		};
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
	public bool PrintInitializedBatchLogs { get; set; } = true;
	public bool PrintCompletedBatchLogs { get; set; } = true;
	public bool PrintRetrievedImageLogs { get; set; } = false;
	public bool PrintDeletedImageLogs { get; set; } = false;
}
public class ImageDatabaseData
{
}

[ProtoContract]
public class ImageDatabaseDataProto
{
	[ProtoMember(1)]
	public uint Identifier { get; set; }

	[ProtoMember(2)]
	public Dictionary<string, uint> Map { get; set; }

	[ProtoMember(3)]
	public Dictionary<string, string> CustomMap { get; set; }
}
