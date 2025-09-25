using Newtonsoft.Json;

namespace Carbon.Documentation;

public static partial class WebRCon
{
	public static Config config;

	private static void LoadConfig()
	{
		var configFile = Defines.GetWebRconConfigFile();
		if (!File.Exists(configFile))
		{
			SaveConfig();
			return;
		}
		config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));
		Bridge.WebRconServer.SetMessages(new WebRconMessages());
	}

	private static void SaveConfig()
	{
		File.WriteAllText(Defines.GetWebRconConfigFile(), JsonConvert.SerializeObject(config ??= new(), Formatting.Indented));
	}

	public class Config
	{

	}
}
