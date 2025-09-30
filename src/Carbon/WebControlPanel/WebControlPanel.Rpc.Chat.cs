using Facepunch;
using Facepunch.Math;

namespace Carbon;

public static partial class WebControlPanel
{
	public static readonly uint CHAT_LOG = Vault.Pool.Get("RPC_ChatLog");

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.ChatView)]
	private static void RPC_ChatTail(BridgeRead read)
	{
		int count = ConVar.Chat.History.Size - read.Int32();
		var logs = ConVar.Chat.History.Skip(count);
		var write = StartRpcResponse();
		write.WriteObject(logs.Count());
		foreach (var log in logs)
		{
			write.WriteObject((int)log.Channel);
			write.WriteObject(log.Message);
			write.WriteObject(log.UserId);
			write.WriteObject(log.Username);
			write.WriteObject(log.Color);
			write.WriteObject(log.Time);
		}
		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.ChatInput)]
	private static void RPC_ChatInput(BridgeRead read)
	{
		var username = read.String();
		var message = read.String();
		var color = read.String();
		var userid = read.UInt64();
		ConVar.Chat.Broadcast(message, username, color, userid);
	}

	public static void OnPlayerChat(BasePlayer player, string message, ConVar.Chat.ChatChannel channel)
	{
		if (server == null)
		{
			return;
		}
		using var connections = Pool.Get<PooledList<BridgeConnection>>();
		foreach (var connection in server.Connections.Values)
		{
			if (connection.Reference is not Account account || !account.Permissions.chat_view)
			{
				continue;
			}
			connections.Add(connection);
		}

		if (connections.Count == 0)
		{
			return;
		}
		var write = StartRpcResponse(CHAT_LOG);
		write.WriteObject((int)channel);
		write.WriteObject(message);
		write.WriteObject(player.UserIDString);
		write.WriteObject(player.displayName);
		write.WriteObject(ConVar.Chat.GetNameColor(player.userID, player));
		write.WriteObject(Epoch.Current);
		SendRpcResponse(connections, write);
	}
}
