using System.Drawing.Imaging;
using System.Management.Instrumentation;
using System.Net;
using Facepunch;
using ProtoBuf;
using QRCoder;
using static Carbon.Modules.ImageDatabaseModule;
using Color = System.Drawing.Color;
using Defines = Carbon.Core.Defines;
using Timer = Oxide.Plugins.Timer;

namespace Carbon.Modules;

public partial class ImageDatabaseModule : CarbonModule<ImageDatabaseConfig, EmptyModuleData>
{
	public static ImageDatabaseModule Singleton;

	public override string Name => "ImageDatabase";
	public override Type Type => typeof(ImageDatabaseModule);
	public override VersionNumber Version => new(1, 0, 0);
	public override bool EnabledByDefault => true;
	public override bool ForceEnabled => true;

	internal ImageDatabaseDataProto _protoData { get; set; }

	internal Dictionary<string, string> _defaultImages = new()
	{
		["carbonb"] = "https://cdn.carbonmod.gg/carbonlogo_b.png",
		["carbonw"] = "https://cdn.carbonmod.gg/carbonlogo_w.png",
		["carbonbs"] = "https://cdn.carbonmod.gg/carbonlogo_bs.png",
		["carbonws"] = "https://cdn.carbonmod.gg/carbonlogo_ws.png",
		["cflogo"] = "https://cdn.carbonmod.gg/content/codefling-logo.png",
		["checkmark"] = "https://cdn.carbonmod.gg/content/checkmark.png",
		["umodlogo"] = "https://cdn.carbonmod.gg/content/umod-logo.png",
		["clouddl"] = "https://cdn.carbonmod.gg/content/cloud-dl.png",
		["trashcan"] = "https://cdn.carbonmod.gg/content/trash-can.png",
		["shopping"] = "https://cdn.carbonmod.gg/content/shopping-cart.png",
		["installed"] = "https://cdn.carbonmod.gg/content/installed.png",
		["reload"] = "https://cdn.carbonmod.gg/content/reload.png",
		["update-pending"] = "https://cdn.carbonmod.gg/content/update-pending.png",
		["magnifying-glass"] = "https://cdn.carbonmod.gg/content/magnifying-glass.png",
		["filter"] = "https://cdn.carbonmod.gg/content/filter.png",
		["star"] = "https://cdn.carbonmod.gg/content/star.png",
		["glow"] = "https://cdn.carbonmod.gg/content/glow.png",
		["gear"] = "https://cdn.carbonmod.gg/content/gear.png",
		["sort"] = "https://cdn.carbonmod.gg/content/sort.png",
		["close"] = "https://cdn.carbonmod.gg/content/close.png",
		["fade"] = "https://cdn.carbonmod.gg/content/fade.png",
		["graph"] = "https://cdn.carbonmod.gg/content/graph.png",
		["maximize"] = "https://cdn.carbonmod.gg/content/maximize.png",
		["minimize"] = "https://cdn.carbonmod.gg/content/minimize.png",
		["folder"] = "https://cdn.carbonmod.gg/content/folder.png",
		["file"] = "https://cdn.carbonmod.gg/content/file.png",
		["translate"] = "https://cdn.carbonmod.gg/content/translate.png",
		["cf_hero"] = "https://cdn.carbonmod.gg/content/cf_hero.png",
		["umod_hero"] = "https://cdn.carbonmod.gg/content/umod_hero.png",
		["installed_hero"] = "https://cdn.carbonmod.gg/content/installed_hero.png",
		["hero_fade"] = "https://cdn.carbonmod.gg/content/hero_fade.png",
		["fade_flip"] = "https://cdn.carbonmod.gg/content/fade_flip.png",
		["empty_star"] = "https://cdn.carbonmod.gg/content/empty_star.png",
		["half_star"] = "https://cdn.carbonmod.gg/content/half_star.png",
		["full_star"] = "https://cdn.carbonmod.gg/content/full_star.png",
		["top_left"] = "https://cdn.carbonmod.gg/content/top_left.png",
		["default_profile"] = "https://cdn.carbonmod.gg/content/default_profile.jpg",
		["bsod"] = "https://cdn.carbonmod.gg/content/bsod.png"
	};

	internal string _getProtoDataPath()
	{
		return Path.Combine(Defines.GetModulesFolder(), Name, "data.db");
	}

	internal const int MaximumBytes = 4104304;

	[ConsoleCommand("imagedb.loaddefaults")]
	[AuthLevel(2)]
	private void LoadDefaults(ConsoleSystem.Arg arg)
	{
		LoadDefaultImages(true);
		arg.ReplyWith($"Loading all default images.");
	}

	[ConsoleCommand("imagedb.deleteimage")]
	[AuthLevel(2)]
	private void DeleteImg(ConsoleSystem.Arg arg)
	{
		arg.ReplyWith(DeleteImage(arg.GetString(0))
			? $"Deleted image"
			: $"Couldn't delete image. Probably because it doesn't exist");
	}

	[ConsoleCommand("imagedb.clearinvalid")]
	[AuthLevel(2)]
	private void ClearInvalid(ConsoleSystem.Arg arg)
	{
		var toDelete = Facepunch.Pool.Get<Dictionary<uint, FileStorage.CacheData>>();

		foreach (var file in FileStorage.server._cache)
		{
			if (file.Value.data.Length >= MaximumBytes)
			{
				toDelete.Add(file.Key, file.Value);
			}
		}

		foreach (var data in toDelete)
		{
			FileStorage.server.Remove(data.Key, FileStorage.Type.png, data.Value.entityID);
		}

		arg.ReplyWith($"Removed {toDelete.Count:n0} invalid stored files from FileStorage (above the maximum size of {ByteEx.Format(MaximumBytes, shortName: true).ToUpper()}).");
		Facepunch.Pool.FreeUnmanaged(ref toDelete);
	}

	public override void Init()
	{
		base.Init();

		Singleton = this;
	}
	public override void OnServerInit(bool initial)
	{
		base.OnServerInit(initial);

		if(!initial) return;

		if (Validate())
		{
			Save();
		}

		LoadDefaultImages();
	}
	public override void OnServerSaved()
	{
		base.OnServerSaved();

		SaveDatabase();
	}

	public override void Load()
	{
		var path = _getProtoDataPath();

		if (OsEx.File.Exists(path))
		{
			using var stream = new MemoryStream(OsEx.File.ReadBytes(path));
			try
			{
				_protoData = Serializer.Deserialize<ImageDatabaseDataProto>(stream);
			}
			catch
			{
				_protoData = new ImageDatabaseDataProto();
				Save();
			}
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

		using var stream = new MemoryStream();
		Serializer.Serialize(stream, _protoData ??= new ImageDatabaseDataProto());

		var result = stream.ToArray();
		OsEx.File.Create(path, result);
		Array.Clear(result,0,result.Length);
		result = null;
	}
	private void LoadDefaultImages(bool forced = false)
	{
		Queue(forced, _defaultImages);
	}

	public override bool PreLoadShouldSave(bool newConfig, bool newData)
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
		if (_protoData.Identifier != CommunityEntity.ServerInstance.net.ID.Value)
		{
			PutsWarn($"The server identifier has changed. Wiping old image database. [old {_protoData.Identifier}, new {CommunityEntity.ServerInstance.net.ID.Value}]");
			_protoData.CustomMap.Clear();
			_protoData.Map.Clear();
			_protoData.Identifier = CommunityEntity.ServerInstance.net.ID.Value;
			return true;
		}

		if (!HasImage("checkmark"))
		{
			_protoData.CustomMap.Clear();
			_protoData.Map.Clear();
			return true;
		}

		return false;
	}

	public void QueueBatch(bool @override, IEnumerable<string> urls)
	{
		if (urls == null || !urls.Any())
		{
			return;
		}

		var urlCount = urls.Count();

		QueueBatch(@override, results =>
		{
			foreach (var result in results.Where(result => result.Data != null && result.Data.Length != 0))
			{
				if (result.Data.Length >= MaximumBytes)
				{
					Puts($"Failed storing {urlCount:n0} jobs: {result.Data.Length} more or equal than {MaximumBytes}");
					continue;
				}

				var id = FileStorage.server.Store(result.Data, FileStorage.Type.png, new NetworkableId(_protoData.Identifier));

				if (id != 0)
				{
					_protoData.Map[GetId(result.Url)] = id;
				}
			}
		}, urls);
	}
	public void QueueBatch(bool @override, Action<List<ImageQueueResult>> onComplete, IEnumerable<string> urls)
	{
		if (urls == null || !urls.Any())
		{
			return;
		}

		var queue = Pool.Get<ImageQueue>();
		var existent = Pool.Get<List<ImageQueueResult>>();
		var urlCount = urls.Count();

		try
		{
			queue.ImageUrls.AddRange(urls);

			if (!@override)
			{
				foreach (var url in urls)
				{
					var image = GetImage(url);

					if (image == 0)
					{
						continue;
					}

					existent.Add(new ImageQueueResult
					{
						CRC = image,
						Url = url,
						Success = true
					});

					queue.ImageUrls.Remove(url);
				}
			}
			else
			{
				foreach (var url in queue.ImageUrls)
				{
					DeleteAllImages(url);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed processing queue batch", ex);
		}

		Community.Runtime.Core.persistence.StartCoroutine(RunQueue(queue, results =>
		{
			try
			{
				if (results != null)
				{
					for (var index = 0; index < results.Count; index++)
					{
						var result = results[index];
						if (result.Data.Length >= MaximumBytes)
						{
							Puts($"Failed storing {urlCount:n0} jobs: {result.Data.Length} more or equal than {MaximumBytes}");
							continue;
						}

						var id = FileStorage.server.Store(result.Data, FileStorage.Type.png, new NetworkableId(_protoData.Identifier));

						if (id != 0)
						{
							_protoData.Map[GetId(result.Url)] = id;
							result.Success = true;
							result.CRC = id;
							results[index] = result;
						}
					}

					results.InsertRange(0, existent);

					onComplete?.Invoke(results);
				}
			}
			catch (Exception ex)
			{
				PutsError($"Failed QueueBatch of {urls.Count():n0}", ex);
			}

			Pool.FreeUnmanaged(ref existent);
		}));
	}

	public void Queue(bool @override, Dictionary<string, string> mappedUrls)
	{
		if (mappedUrls == null || mappedUrls.Count == 0)
		{
			return;
		}

		var urls = new List<string>(); // Required for thread consistency (previously Pool.GetList<>)

		foreach (var url in mappedUrls)
		{
			urls.Add(url.Value);
			AddMap(url.Key, url.Value);
		}

		QueueBatch(@override, urls);
	}
	public void Queue(bool @override, Action<List<ImageQueueResult>> onComplete, Dictionary<string, string> mappedUrls)
	{
		if (mappedUrls == null || mappedUrls.Count == 0)
		{
			return;
		}

		var urls = new List<string>(); // Required for thread consistency (previously Pool.GetList<>)

		foreach (var url in mappedUrls)
		{
			urls.Add(url.Value);
			AddMap(url.Key, url.Value);
		}

		QueueBatch(@override, onComplete, urls);
	}
	public void Queue(Dictionary<string, string> mappedUrls)
	{
		if (mappedUrls == null || mappedUrls.Count == 0)
		{
			return;
		}

		Queue(false, mappedUrls);
	}
	public void Queue(Action<List<ImageQueueResult>> onComplete, Dictionary<string, string> mappedUrls)
	{
		if (mappedUrls == null || mappedUrls.Count == 0)
		{
			return;
		}

		Queue(false, onComplete, mappedUrls);
	}

	public void Queue(bool @override, string key, string url)
	{
		if(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(url))
		{
			return;
		}

		AddMap(key, url);
		QueueBatch(@override, [url]);
	}
	public void Queue(bool @override, string url)
	{
		Queue(@override, url, url);
	}
	public void Queue(string key, string url)
	{
		Queue(false, key, url);
	}
	public void Queue(string url)
	{
		Queue(false, url);
	}

	public void AddImage(string keyOrUrl, byte[] imageData, FileStorage.Type type = FileStorage.Type.png)
	{
		_protoData.Map[keyOrUrl] = FileStorage.server.Store(imageData, type, new NetworkableId(_protoData.Identifier));
	}
	public void AddMap(string key, string url)
	{
		_protoData.CustomMap[key] = url;
	}
	public void RemoveMap(string key)
	{
		if (_protoData.CustomMap.ContainsKey(key)) _protoData.CustomMap.Remove(key);
	}

	public string GetKeyImage(string key)
	{
		if(_protoData.CustomMap.TryGetValue(key, out var keyImage))
		{
			return keyImage;
		}

		return null;
	}
	public uint GetImage(string keyOrUrl)
	{
		if (string.IsNullOrEmpty(keyOrUrl))
		{
			return default;
		}

		if (_protoData.CustomMap.TryGetValue(keyOrUrl, out var realUrl))
		{
			keyOrUrl = realUrl;
		}

		var id = GetId(keyOrUrl);

		return !_protoData.Map.TryGetValue(id, out var uid) ? default : uid;
	}
	public string GetImageString(string keyOrUrl)
	{
		return GetImage(keyOrUrl).ToString();
	}
	public bool HasImage(string keyOrUrl)
	{
		return FileStorage.server.Get(GetImage(keyOrUrl), FileStorage.Type.png, new NetworkableId(_protoData.Identifier)) != null;
	}
	public bool DeleteImage(string url)
	{
		var id = GetId(url);

		if (!_protoData.Map.TryGetValue(id, out var uid))
		{
			return false;
		}

		FileStorage.server.Remove(uid, FileStorage.Type.png, new NetworkableId(_protoData.Identifier));
		_protoData.Map.Remove(id);
		return true;
	}
	public void DeleteAllImages(string url)
	{
		var temp = Pool.Get<Dictionary<string, uint>>();

		foreach (var map in _protoData.Map)
		{
			temp.Add(map.Key, map.Value);
		}

		foreach (var map in temp.Where(x => x.Key.StartsWith(url)))
		{
			FileStorage.server.Remove(map.Value, FileStorage.Type.png, new NetworkableId(_protoData.Identifier));
			_protoData.Map.Remove(map.Key);
		}

		Pool.FreeUnmanaged(ref temp);
	}

	public uint GetQRCode(string text, int pixels = 20, bool transparent = false, bool quietZones = true, bool whiteMode = false)
	{
		if (_protoData.Map.TryGetValue($"qr_{Community.Protect(text)}_{pixels}_0", out uint uid))
		{
			return uid;
		}

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
			uid = FileStorage.server.Store(raw, FileStorage.Type.png, new NetworkableId(_protoData.Identifier));
			_protoData.Map.Add($"qr_{Community.Protect(text)}_{pixels}_0", uid);
			return uid;
		}
	}

	public static string GetId(string url)
	{
		return url;
	}

	public static IEnumerator RunQueue(ImageQueue imageQueue, Action<List<ImageQueueResult>> callback)
	{
		imageQueue.Init();

		while (!imageQueue.IsDone)
		{
			yield return null;
		}

		callback?.Invoke(imageQueue.Result);

		Pool.FreeUnsafe(ref imageQueue);
	}

	public class ImageQueue : Pool.IPooled
	{
		public List<string> ImageUrls { get; internal set; } = new();
		public List<ImageQueueResult> Result { get; internal set; } = new();
		public Action<List<ImageQueueResult>> ResultAction { get; set; }

		public bool IsDone;
		public WebRequests.WebRequest.Client Client;

		private Timer _timeout;
		private int _index;
		private bool _poolInit;

		public void Init()
		{
			CreateTimeout();
			MoveNext();
		}
		public void MoveNext()
		{
			if (_index >= ImageUrls.Count)
			{
				IsDone = true;
				return;
			}

			var url = ImageUrls[_index];

			Client.DownloadDataAsync(new Uri(url), url);

			_index++;
		}
		public void CreateTimeout()
		{
			var instance = this;

			_timeout = Community.Runtime.Core.timer.In(Singleton.ConfigInstance.TimeoutPerUrl * ImageUrls.Count, () =>
			{
				if (IsDone)
				{
					return;
				}

				try
				{
					if (Result.Count > 0)
					{
						ResultAction?.Invoke(Result);
					}
				}
				catch (Exception ex)
				{
					Logger.Error($"Failed timeout process", ex);
				}
			});
		}

		public void EnterPool()
		{
			IsDone = false;
			ImageUrls.Clear();
			Result.Clear();
			ResultAction = null;
			_timeout?.Reset();
			_index = 0;
		}

		public void LeavePool()
		{
			if (_poolInit)
			{
				return;
			}

			Client = new();
			Client.Headers.Add("User-Agent", Community.Runtime.Analytics.UserAgent);
			Client.Credentials = CredentialCache.DefaultCredentials;
			Client.Proxy = null;
			Client.DownloadDataCompleted += (_, e) =>
			{
				if (e.Error == null)
				{
					Result.Add(new ImageQueueResult
					{
						Url = (string)e.UserState,
						Data = e.Result
					});
				}

				Community.Runtime.Core.NextFrame(MoveNext);
			};

			_poolInit = true;
		}
	}
	public struct ImageQueueResult
	{
		public string Url;
		public byte[] Data;
		public uint CRC;
		public bool Success;
	}
}

public class ImageDatabaseConfig
{
	public float TimeoutPerUrl { get; set; } = 2f;
}

[ProtoContract]
public class ImageDatabaseDataProto
{
	[ProtoMember(1)]
	public ulong Identifier { get; set; }

	[ProtoMember(2)]
	public Dictionary<string, uint> Map { get; set; }

	[ProtoMember(3)]
	public Dictionary<string, string> CustomMap { get; set; }
}
