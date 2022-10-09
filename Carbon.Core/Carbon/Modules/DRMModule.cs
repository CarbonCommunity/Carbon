///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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

		public class DRMProcessor
		{
			public string Name { get; set; }
			public string ValidationEndpoint { get; set; }
			public string DownloadEndpoint { get; set; }
			public string PublicKey { get; set; }
			public List<DRMEntry> Entries { get; set; } = new List<DRMEntry>();

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

			public void Launch()
			{
				foreach (var entry in Entries)
				{
					Web.Enqueue(string.Format(DownloadEndpoint, PublicKey, entry.Id, entry.PrivateKey), null, (code, data) =>
					{
						if (code != 200) return;

						try
						{
							var bytes = Convert.FromBase64String(data);
							var assembly = Assembly.Load(bytes);

							foreach (var type in assembly.GetTypes())
							{
								CarbonLoader.InitializePlugin(type, out var plugin, Mod);
							}
						}
						catch (Exception ex)
						{
							PutsError($"Failed loading '{entry.Id}'", ex);
						}
					}, null);
				}
			}
		}

		public class DRMEntry
		{
			public string Id { get; set; }
			public string PrivateKey { get; set; }
		}
	}

	public class DRMConfig
	{
		public List<DRMModule.DRMProcessor> DRMs { get; set; } = new List<DRMModule.DRMProcessor>();
	}
	public class DRMData
	{

	}
}
