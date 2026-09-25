using System;
using System.IO;
using Newtonsoft.Json;

namespace Carbon.Managers;

/// <summary>
/// The slice of carbon/config.json needed before <see cref="Community"/> loads its full config.
/// </summary>
[Serializable]
public class BootConfig
{
	public static BootConfig Singleton;

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

		if (!File.Exists(Defines.GetConfigFile()))
		{
			Singleton = new();
			return;
		}

		Singleton = JsonConvert.DeserializeObject<BootConfig>(File.ReadAllText(Defines.GetConfigFile()));
	}
}
