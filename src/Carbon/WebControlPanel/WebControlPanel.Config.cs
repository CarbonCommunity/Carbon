using Facepunch;
using Fleck;
using Newtonsoft.Json;

namespace Carbon;

public static partial class WebControlPanel
{
	public static void Shutdown()
	{
		if (server == null)
		{
			return;
		}
		server.Shutdown();
		server = null;
	}

	public static void LoadConfig()
	{
		var configFile = Defines.GetWebRconConfigFile();
		if (!File.Exists(configFile))
		{
			SaveConfig();
			return;
		}
		config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));

		if (server != null)
		{
			using var connections = Pool.Get<PooledList<BridgeConnection>>();
			connections.AddRange(server.Connections.Values);
			for (int i = 0; i < connections.Count; i++)
			{
				connections[i].Socket.Close();
			}
		}

		if (server == null && config.CanStartServer())
		{
			(server = new Server()).Start(config.port, RandomEx.GetRandomString(16), config.ip, serverMessages);
		}
	}

	public static void SaveConfig()
	{
		File.WriteAllText(Defines.GetWebRconConfigFile(), JsonConvert.SerializeObject(config ??= new(), Formatting.Indented));
	}

	public class Config
	{
		public string ip = "youriphere";
		public int port = 0;
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
