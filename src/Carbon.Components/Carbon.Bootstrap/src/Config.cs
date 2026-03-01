using System;
using System.IO;
using Newtonsoft.Json;
using Utility;

namespace Carbon;

[Serializable]
public class Config
{
	public static Config Singleton;

	public AnalyticsConfig Analytics { get; set; } = new();

	public class AnalyticsConfig
	{
		public bool Enabled { get; set; } = true;
	}

	public static void Init()
	{
		if (Singleton != null)
		{
			return;
		}

		if (!File.Exists(Context.CarbonConfig))
		{
			Singleton = new();
			return;
		}

		Singleton = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Context.CarbonConfig));
	}
}
