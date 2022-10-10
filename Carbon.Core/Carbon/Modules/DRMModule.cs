///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Carbon.Core.Extensions;
using Carbon.Extensions;
using Newtonsoft.Json;
using Oxide.Core.Libraries;

namespace Carbon.Core.Modules
{
	public class DRMModule : CarbonModule<DRMConfig, DRMData>
	{
		public override string Name => "DRM";
		public override Type Type => typeof(DRMModule);
		public override bool EnabledByDefault => true;

		public override void Init()
		{
			base.Init();

			foreach (var processor in Config.DRMs)
			{
				processor.Initialize();
			}
		}
		public override void Dispose()
		{
			foreach (var processor in Config.DRMs)
			{
				processor.Uninitialize();
			}

			base.Dispose();
		}

		[ConsoleCommand("drmtest")]
		private void GenerateTest(ConsoleSystem.Arg args)
		{
			if (!args.IsPlayerCalledAndAdmin() || !args.HasArgs(1)) return;

			CarbonCorePlugin.Reply($"{JsonConvert.SerializeObject(new DownloadResponse().WithFileType(DownloadResponse.FileTypes.Script).WithDataFile(args.Args[0]), Formatting.Indented)}", args);
		}

		public class Processor
		{
			public string Name { get; set; }
			public string ValidationEndpoint { get; set; }
			public string DownloadEndpoint { get; set; }
			public string PublicKey { get; set; }
			public List<Entry> Entries { get; set; } = new List<Entry>();

			[JsonIgnore]
			public bool IsOnline { get; internal set; }

			[JsonIgnore]
			public WebRequests Web { get; } = new WebRequests();

			[JsonIgnore]
			public CarbonLoader.CarbonMod Mod { get; } = new CarbonLoader.CarbonMod();

			#region Logging

			protected void Puts(object message)
				=> Logger.Log($"[{Name}] {message}");
			protected void PutsError(object message, Exception ex = null)
				=> Logger.Error($"[{Name}] {message}", ex);
			protected void PutsWarn(object message)
				=> Logger.Warn($"[{Name}] {message}");

			#endregion

			public void Validate()
			{
				if (string.IsNullOrEmpty(ValidationEndpoint))
				{
					PutsWarn("Not set up.");
					return;
				}

				Puts($"Validating...");

				Web.Enqueue(string.Format(ValidationEndpoint, PublicKey), null, (code, data) =>
				{
					IsOnline = code == 200;

					if (IsOnline)
					{
						Puts($"Success!");

						Launch();
					}
					else PutsError($"Failed to validate.");
				}, null);
			}

			public void Initialize()
			{
				Validate();

				Mod.Name = $"{Name} DRM";
				CarbonLoader._loadedMods.Add(Mod);
			}
			public void Uninitialize()
			{
				CarbonLoader._loadedMods.Remove(Mod);
			}

			public void Launch()
			{
				foreach (var entry in Entries)
				{
					Web.Enqueue(string.Format(DownloadEndpoint, PublicKey, entry.Id, entry.PrivateKey), null, (code, data) =>
					{
						if (code != 200) return;

						try
						{
							var response = JsonConvert.DeserializeObject<DownloadResponse>(data);

							switch (response.FileType)
							{
								case DownloadResponse.FileTypes.Script:
									var loader = new ScriptLoader
									{
										Source = DecodeBase64(response.Data)
									};
									loader.Load();
									break;

								case DownloadResponse.FileTypes.DLL:
									var source = Convert.FromBase64String(response.Data);
									var assembly = Assembly.Load(source);

									foreach (var type in assembly.GetTypes())
									{
										CarbonLoader.InitializePlugin(type, out var plugin, Mod);
									}
									break;
							}
						}
						catch (Exception ex)
						{
							PutsError($"Failed loading '{entry.Id}'", ex);
						}
					}, null);
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
				Data = Processor.EncodeBase64(source);
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
		public List<DRMModule.Processor> DRMs { get; set; } = new List<DRMModule.Processor>();
	}
	public class DRMData
	{

	}
}
