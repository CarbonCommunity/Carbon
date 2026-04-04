using System;
using System.IO;
using Carbon.Core;
using Newtonsoft.Json;

namespace Doorstop;

[Serializable]
public class Config
{
	public static Config Singleton;

	public SelfUpdatingConfig SelfUpdating { get; set; } = new();

	public class SelfUpdatingConfig
	{
		public bool Enabled { get; set; } = true;
		public string RedirectUri { get; set; }
	}

	public static void Init()
	{
		if (Singleton != null)
		{
			return;
		}

		if (!File.Exists(Defines.GetConfigFile()))
		{
			Singleton = new();
			return;
		}

		Singleton = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Defines.GetConfigFile()));
	}
}
