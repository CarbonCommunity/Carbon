namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.PlayersView)]
	private static void RPC_Players(BridgeRead read)
	{
		var canSeeIps = Account.HasPermission(read.Connection, PermissionTypes.PlayersIp);
		var write = StartRpcResponse();
		write.WriteObject(BasePlayer.activePlayerList.Count);
		for (int i = 0; i < BasePlayer.activePlayerList.Count; i++)
		{
			new PlayerInfo(BasePlayer.activePlayerList[i]).Serialize(write, !canSeeIps);
		}
		write.WriteObject(BasePlayer.sleepingPlayerList.Count);
		for (int i = 0; i < BasePlayer.sleepingPlayerList.Count; i++)
		{
			new PlayerInfo(BasePlayer.sleepingPlayerList[i]).Serialize(write, !canSeeIps);
		}
		SendRpcResponse(read.Connection, write);
	}

	public struct PlayerInfo(BasePlayer player)
	{
		private ulong steamId = player.userID;
		private ulong ownerSteamId = player.OwnerID;
		private string displayName = player.displayName;
		private int ping = player.IsConnected ? Network.Net.sv.GetAveragePing(player.Connection) : -1;
		private string address = player.IsConnected ? player.Connection.ipaddress : string.Empty;
		private ulong entityId = player.net.ID.Value;
		private int connectedSeconds = player.secondsConnected;
		private float violationLevel = player.violationLevel;
		private int currentLevel = 0;
		private int unspentXp = 0;
		private float health = player.health;

		public void Serialize(BridgeWrite write, bool excludeIps)
		{
			write.WriteObject(steamId);
			write.WriteObject(ownerSteamId);
			write.WriteObject(displayName);
			write.WriteObject(ping);
			write.WriteObject(excludeIps ? "hidden" : address);
			write.WriteObject(entityId);
			write.WriteObject(connectedSeconds);
			write.WriteObject(violationLevel);
			write.WriteObject(currentLevel);
			write.WriteObject(unspentXp);
			write.WriteObject(health);
		}
	}
}
