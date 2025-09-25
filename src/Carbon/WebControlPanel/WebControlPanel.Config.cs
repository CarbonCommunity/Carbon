using Fleck;
using Newtonsoft.Json;

namespace Carbon;

public static partial class WebControlPanel
{
	public static Server server;
	public static ServerMessages serverMessages = new();
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

		if (config.CanStartServer())
		{
			(server = new Server()).Start(config.port, RandomEx.GetRandomString(16), config.ip, serverMessages);
		}
	}

	private static void SaveConfig()
	{
		File.WriteAllText(Defines.GetWebRconConfigFile(), JsonConvert.SerializeObject(config ??= new(), Formatting.Indented));
	}

	public class Server : BridgeServer
	{
		public override bool OnSocketValidate(IWebSocketConnection socket)
		{
			return base.OnSocketValidate(socket);
		}

		public override void OnBridgeConnection(BridgeConnection connection)
		{
			base.OnBridgeConnection(connection);
		}
	}

	public class Config
	{
		public string ip;
		public int port;
		public Account[] accounts = [new Account
		{
			id = "owner",
			password = RandomEx.GetRandomString(7),
		}];

		public bool CanStartServer()
		{
			if (string.IsNullOrEmpty(ip) || port == 0)
			{
				return false;
			}
			return true;
		}
	}
}
