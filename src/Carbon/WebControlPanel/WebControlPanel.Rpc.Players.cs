namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.PlayersView)]
	private static void RPC_Players(BridgeRead read)
	{
		var write = StartRpcResponse();
		write.WriteObject(BasePlayer.activePlayerList.Count);
		for (int i = 0; i < BasePlayer.activePlayerList.Count; i++)
		{
			new PlayerInfo(BasePlayer.activePlayerList[i]).Serialize(write);
		}
		SendRpcResponse(read.Connection, write);
	}
}
