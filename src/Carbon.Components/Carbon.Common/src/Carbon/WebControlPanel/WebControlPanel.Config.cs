using Facepunch;
using Newtonsoft.Json;

namespace Carbon;

public static partial class WebControlPanel
{
	public static void LoadConfig()
	{
		var configFile = Defines.GetWebPanelConfigFile();
		if (!File.Exists(configFile))
		{
			SaveConfig();
			return;
		}
		config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));

		// Save the config in case we change the config structure
		SaveConfig();
		RestartServer();
	}

	public static void SaveConfig()
	{
		File.WriteAllText(Defines.GetWebPanelConfigFile(), JsonConvert.SerializeObject(config ??= new(), Formatting.Indented));
	}

	public static void RestartServer()
	{
		server?.Shutdown();
		if (config.ShouldStartServer(out string reason))
		{
			BridgeServerInfo serverInfo = default;
			serverInfo.port = config.BridgeServer.Port;
			serverInfo.ip = config.BridgeServer.Ip;
			serverInfo.messages = serverMessages;
			serverInfo.context = nameof(WebControlPanel);
			serverInfo.maxConnections = config.BridgeServer.MaxConnections;
			serverInfo.maxConnectionsPerIp = config.BridgeServer.MaxConnectionsPerIp;
			(server ??= new Server()).Start(serverInfo);
			if (Community.IsServerInitialized && !MAPINFO_CACHE.IsValid())
			{
				MAPINFO_CACHE = MapInfo.Get(config.Panel.MapImageScale);
			}
		}
		if (config.Enabled && !string.IsNullOrEmpty(reason))
		{
			Logger.Warn($"WebControlPanel couldn't start: {reason}");
		}
	}

	public class Config
	{
		public bool Enabled = false;
		public PanelConfig Panel = new();
		public ServerConfig BridgeServer = new();
		public Account[] WebAccounts = [new()
		{
			Name = "owner",
			Password = RandomEx.GetRandomString(7),
			Permissions = new Permissions(true)
		}];

		public bool ShouldStartServer(out string reason)
		{
			if (!Enabled)
			{
				reason = "The server is disabled in the config";
				return false;
			}

			if (string.IsNullOrEmpty(BridgeServer.Ip))
			{
				reason = "The server IP isn't set in the config. Can just be set to 'localhost'";
				return false;
			}

			if (BridgeServer.Port == 0)
			{
				reason = "The server port isn't set in the config";
				return false;
			}

			reason = null;
			return true;
		}
	}
}
