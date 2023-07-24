using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Carbon.Base;
using Carbon.Contracts;
using Carbon.Core;
using Carbon.Extensions;
using Newtonsoft.Json;
using Oxide.Core.Libraries;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;
#pragma warning disable IDE0051

public class DRMModule : CarbonModule<DRMConfig, EmptyModuleData>
{
	public override string Name => "DRM";
	public override Type Type => typeof(DRMModule);

	public override void Load()
	{
		base.Load();

		if (GetEnabled())
		{
			foreach (var processor in ConfigInstance.Processors)
			{
				processor.Value.Uninitialize();
				processor.Value.Initialize();
			}
		}
	}

	public override void OnDisabled(bool initialized)
	{
		base.OnDisabled(initialized);

		foreach (var processor in ConfigInstance.Processors)
		{
			processor.Value.Uninitialize();
		}
	}

	[ConsoleCommand("drm.request", "Requests the downloading ")]
	[AuthLevel(2)]
	private void RequestEntry(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(2))
		{
			arg.ReplyWith("Invalid syntax: drm.request <drm_id> <entry_id>");
			return;
		}

		var drm = ConfigInstance.Processors[arg.Args[0]];

		if (drm == null)
		{
			arg.ReplyWith($"Couldn't find that DRM processing configuration.");
			return;
		}

		var entry = drm.Entries.FirstOrDefault(x => x.Id == ConfigInstance.Processors.FirstOrDefault(x => x.Key == arg.Args[1]).Key);

		if (entry == null)
		{
			arg.ReplyWith($"Couldn't find that DRM processing configuration.");
			return;
		}

		drm.Validate(() => drm.RequestEntry(entry));
	}

	[ConsoleCommand("drm.reboot")]
	[AuthLevel(2)]
	private void Reboot(ConsoleSystem.Arg args)
	{
		foreach (var processor in ConfigInstance.Processors)
		{
			processor.Value.Uninitialize();
			processor.Value.Initialize();
		}
	}

	public class Provider
	{
		public string Name { get; set; }
		public string ValidationEndpoint { get; set; }
		public string DownloadEndpoint { get; set; }
		public string PublicKey { get; set; }
		public bool Disabled { get; set; } = false;
		public List<Entry> Entries { get; set; } = new List<Entry>();

		[JsonIgnore]
		public bool IsOnline { get; internal set; }

		[JsonIgnore]
		public ModLoader.ModPackage Mod { get; } = new ModLoader.ModPackage();

		[JsonIgnore]
		public List<BaseProcessor.Instance> ProcessorInstances { get; } = new List<BaseProcessor.Instance>();

		#region Logging

		protected void Puts(object message)
			=> Logger.Log($"[{Name}] {message}");
		protected void PutsError(object message, Exception ex = null)
			=> Logger.Error($"[{Name}] {message}", ex);
		protected void PutsWarn(object message)
			=> Logger.Warn($"[{Name}] {message}");

		#endregion

		public WebRequests.WebRequest Enqueue(string url, string body, Action<int, string> callback, RequestMethod method = RequestMethod.GET, Dictionary<string, string> headers = null, float timeout = 0f, Action<int, object, Exception> onException = null)
		{
			return new WebRequests.WebRequest(url, callback, null)
			{
				Method = method.ToString(),
				RequestHeaders = headers,
				Timeout = timeout,
				Body = body,
				ErrorCallback = onException

			}.Start();
		}

		public void Validate(Action callback = null, bool launch = false)
		{
			if (string.IsNullOrEmpty(ValidationEndpoint))
			{
				PutsWarn("Not set up.");
				return;
			}

			Puts($"Validating...");

			Enqueue(string.Format(ValidationEndpoint, PublicKey), null, (code, data) =>
			{
				IsOnline = code == 200;

				if (IsOnline)
				{
					Puts($"Success!");

					if (launch) Launch();
					callback?.Invoke();
				}
				else PutsError($"Failed to validate.");
			}, onException: (code, data, exception) =>
			{
				PutsError($"Failed with '{code}' code.");
			});
		}

		public void Initialize()
		{
			if (Disabled)
			{
				PutsWarn($"The DRM provider is disabled.");
				return;
			}

			Validate(launch: true);

			Mod.Name = $"{Name} DRM";
			ModLoader.LoadedPackages.Add(Mod);
		}
		public void Uninitialize()
		{
			foreach (var entry in Entries)
			{
				DisposeEntry(entry);
			}

			ProcessorInstances.Clear();

			ModLoader.LoadedPackages.Remove(Mod);
		}

		public void Launch()
		{
			foreach (var entry in Entries)
			{
				RequestEntry(entry);
			}
		}

		public void RequestEntry(Entry entry)
		{
			DisposeEntry(entry);

			PutsWarn($"Loading '{entry.Id}' entry...");
			Enqueue(string.Format(DownloadEndpoint, PublicKey, entry.Id, entry.PrivateKey), null, (code, data) =>
			{
				Logger.Debug($"{entry.Id} DRM", $"Got response code '{code}' with {ByteEx.Format(data.Length).ToUpper()} of data");
				if (code != 200) return;

				try
				{
					var response = JsonConvert.DeserializeObject<DownloadResponse>(data);
					Logger.Debug($"{entry.Id} DRM", $"Deserialized response type '{response.FileType}'. Processing...");

					switch (response.FileType)
					{
						case DownloadResponse.FileTypes.Script:
							var instance = new ScriptInstance
							{
								File = entry.Id,
								_mod = Mod,
								_source = DecodeBase64(response.Data)
							};
							ProcessorInstances.Add(instance);
							instance.Execute();
							break;

						case DownloadResponse.FileTypes.DLL:
							var source = Convert.FromBase64String(response.Data);
							var assembly = Assembly.Load(source);

							foreach (var type in assembly.GetTypes())
							{
								ModLoader.InitializePlugin(type, out var plugin, Mod);
							}
							break;
					}
				}
				catch (Exception ex)
				{
					PutsError($"Failed loading '{entry.Id}'", ex);
				}
			});
		}
		public void DisposeEntry(Entry entry)
		{
			var alreadyProcessedInstance = ProcessorInstances.FirstOrDefault(x => x.File == entry.Id);

			if (alreadyProcessedInstance != null)
			{
				try { alreadyProcessedInstance.Dispose(); } catch { }
				ProcessorInstances.Remove(alreadyProcessedInstance);
				PutsWarn($"Unloading '{entry.Id}' entry");
			}
		}

		public static string EncodeBase64(string value)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
		}
		public static string DecodeBase64(string value)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(value));
		}

		public class ScriptInstance : BaseProcessor.Instance, IScriptProcessor.IScript
		{
			internal ModLoader.ModPackage _mod;
			internal string _source;

			public IScriptLoader Loader { get; set; }

			public override void Dispose()
			{
				foreach (var plugin in Loader.Scripts)
				{
					plugin.Dispose();
					_mod.Plugins.Remove(plugin.Instance);
				}

				base.Dispose();
			}
			public override void Execute()
			{
				try
				{
					Loader.Parser = Parser;
					Loader.File = File;
					Loader.Source = _source;
					Loader.Mod = _mod;
					Loader.Instance = this;
					Loader.Load();
				}
				catch (Exception ex)
				{
					Logger.Warn($"Failed processing {File}:\n{ex}");
				}
			}
		}
	}
	public class Entry
	{
		public string Id { get; set; }
		public string PrivateKey { get; set; }
	}
	public class DownloadResponse
	{
		public FileTypes FileType { get; set; }
		public string Data { get; set; }

		public DownloadResponse WithFileType(FileTypes type)
		{
			FileType = type;
			return this;
		}
		public DownloadResponse WithData(string source)
		{
			Data = Provider.EncodeBase64(source);
			return this;
		}
		public DownloadResponse WithDataFile(string file)
		{
			return WithData(OsEx.File.ReadText(file));
		}

		public enum FileTypes
		{
			Script = 512,
			DLL = 1024
		}
	}
}

public class DRMConfig
{
	public Dictionary<string, DRMModule.Provider> Processors { get; set; } = new()
	{
		["my_drm"] = new()
		{
			Name = "My DRM",
			ValidationEndpoint = "https://my.endpoint/{0:Public Key}",
			DownloadEndpoint = "https://my.endpoint/{0}:Public Key}?id={1:Entry Id}&pk={2:Private Key}",
			PublicKey = "Public Key",
			Entries = new()
			{
				new()
				{
					Id = "entry_id",
					PrivateKey = "Private Key"
				}
			},
			Disabled = true
		}
	};
}
