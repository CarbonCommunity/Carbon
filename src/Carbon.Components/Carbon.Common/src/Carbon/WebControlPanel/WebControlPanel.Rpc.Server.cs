using Facepunch;

namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	private static void RPC_ServerInfo(BridgeRead read)
	{
		var write = StartRpcResponse();
		ServerInfoOutput.Get().Serialize(write);
		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	private static void RPC_CarbonInfo(BridgeRead read)
	{
		var analytics = Community.Runtime.Analytics;
		RpcResponse(read, $"Carbon" +
#if MINIMAL
			$" Minimal" +
#endif
		    $" {analytics.Version}/{analytics.Platform}/{analytics.Protocol} [{Build.Git.Branch}] [{Build.Git.Tag}] on Rust {BuildInfo.Current.Build.Number}/{Rust.Protocol.printable} ({BuildInfo.Current.BuildDate}) [{BuildInfo.Current.Scm.ChangeId}]");
	}

	[WebCall]
	private static void RPC_ServerDescription(BridgeRead read)
	{
		RpcResponse(read, ConVar.Server.description);
	}

	[WebCall]
	private static void RPC_ServerHeaderImage(BridgeRead read)
	{
		RpcResponse(read, ConVar.Server.headerimage);
	}

	public struct ServerInfoOutput
	{
		private string Hostname;
		private int MaxPlayers;
		private int Players;
		private int Queued;
		private int Joining;
		private int ReservedSlots;
		private int EntityCount;
		private string GameTime;
		private int Uptime;
		private string Map;
		private float Framerate;
		private int Memory;
		private int MemoryUsageSystem;
		private int Collections;
		private int NetworkIn;
		private int NetworkOut;
		private bool Restarting;
		private string SaveCreatedTime;
		private int Version;
		private string Protocol;

		public static ServerInfoOutput Get()
		{
			if (!Community.IsServerInitialized)
			{
				return default;
			}
			return new ServerInfoOutput
			{
				Hostname = ConVar.Server.hostname,
				MaxPlayers = ConVar.Server.maxplayers,
				Players = BasePlayer.activePlayerList.Count,
				Queued = SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued,
				Joining = SingletonComponent<ServerMgr>.Instance.connectionQueue.Joining,
				ReservedSlots = SingletonComponent<ServerMgr>.Instance.connectionQueue.ReservedCount,
				EntityCount = BaseNetworkable.serverEntities.Count,
				GameTime = TOD_Sky.Instance != null ? TOD_Sky.Instance.Cycle.DateTime.ToString(CultureInfo.CurrentCulture) : System.DateTime.UtcNow.ToString(CultureInfo.CurrentCulture),
				Uptime = (int) UnityEngine.Time.realtimeSinceStartup,
				Map = ConVar.Server.level,
				Framerate = Performance.report.frameRate,
				Memory = (int) Performance.report.memoryAllocations,
				MemoryUsageSystem = (int) Performance.report.memoryUsageSystem,
				Collections = (int) Performance.report.memoryCollections,
				NetworkIn = Network.Net.sv == null ? 0 : (int) Network.Net.sv.GetStat(null, Network.BaseNetwork.StatTypeLong.BytesReceived_LastSecond),
				NetworkOut = Network.Net.sv == null ? 0 : (int) Network.Net.sv.GetStat(null, Network.BaseNetwork.StatTypeLong.BytesSent_LastSecond),
				Restarting = SingletonComponent<ServerMgr>.Instance.Restarting,
				SaveCreatedTime = SaveRestore.SaveCreatedTime.ToString(CultureInfo.CurrentCulture),
				Version = Rust.Protocol.network,
				Protocol = Rust.Protocol.printable
			};
		}

		public void Serialize(BridgeWrite obj)
		{
			obj.WriteObject(Hostname);
			obj.WriteObject(MaxPlayers);
			obj.WriteObject(Players);
			obj.WriteObject(Queued);
			obj.WriteObject(Joining);
			obj.WriteObject(ReservedSlots);
			obj.WriteObject(EntityCount);
			obj.WriteObject(GameTime);
			obj.WriteObject(Uptime);
			obj.WriteObject(Map);
			obj.WriteObject(Framerate);
			obj.WriteObject(Memory);
			obj.WriteObject(MemoryUsageSystem);
			obj.WriteObject(Collections);
			obj.WriteObject(NetworkIn);
			obj.WriteObject(NetworkOut);
			obj.WriteObject(Restarting);
			obj.WriteObject(SaveCreatedTime);
			obj.WriteObject(Version);
			obj.WriteObject(Protocol);
		}
	}
}
