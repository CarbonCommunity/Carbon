using Facepunch;
using Newtonsoft.Json;

namespace Carbon;

public static partial class WebControlPanel
{
	public static void LoadConfig()
	{
		var configFile = Defines.GetWebRconConfigFile();
		if (!File.Exists(configFile))
		{
			SaveConfig();
			return;
		}
		config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));

		// Save the config in case we change the config structure
		SaveConfig();

		server?.Shutdown();
		if (config.ShouldStartServer())
		{
			(server ??= new Server()).Start(config.BridgeServer.Port, config.BridgeServer.Ip, serverMessages, context: nameof(WebControlPanel));
		}
	}

	public static void SaveConfig()
	{
		File.WriteAllText(Defines.GetWebRconConfigFile(), JsonConvert.SerializeObject(config ??= new(), Formatting.Indented));
	}

	public class Config
	{
		public bool Enabled = true;
		public ServerConfig BridgeServer = new();
		public Account[] WebAccounts = [new()
		{
			id = "owner",
			password = RandomEx.GetRandomString(7),
			permissions = new Permissions(true)
		}];

		public bool ShouldStartServer()
		{
			return Enabled && !string.IsNullOrEmpty(BridgeServer.Ip) && BridgeServer.Port != 0;
		}
	}
}
