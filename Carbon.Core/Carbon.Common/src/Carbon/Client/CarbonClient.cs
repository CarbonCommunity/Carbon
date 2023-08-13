using Carbon.Client.Contracts;

namespace Carbon.Client;

public class CarbonClient : ICommunication
{
	public static Dictionary<Network.Connection, CarbonClient> clients { get; internal set; } = new();
	public static CommunityEntity community => RPC.SERVER ? CommunityEntity.ServerInstance : CommunityEntity.ClientInstance;

	public Network.Connection Connection { get; internal set; }

	public bool IsConnected => Connection != null && Connection.active;

	public void Send(IPacket packet, RPC rpc)
	{
		community.ClientRPCEx(new Network.SendInfo(Connection), null, rpc.Name, packet);
	}
	public static void SendPing(Network.Connection connection)
	{
		community.ClientRPCEx(new Network.SendInfo(connection), null, RPC.Get("ping").Name);
	}

	public static CarbonClient Make(Network.Connection connection)
	{
		return new CarbonClient
		{
			Connection = connection
		};
	}

	public static CarbonClient Get(BasePlayer player)
	{
		return Get(player.Connection);
	}
	public static CarbonClient Get(Network.Connection connection)
	{
		if(!clients.TryGetValue(connection, out var value))
		{
			clients.Add(connection, value = Make(connection));
		}

		return value;
	}

	public bool IsValid()
	{
		return IsConnected;
	}
}
