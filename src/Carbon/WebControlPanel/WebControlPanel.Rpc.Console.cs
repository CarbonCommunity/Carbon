using Facepunch;
using Facepunch.Math;

namespace Carbon;

public static partial class WebControlPanel
{
	public static readonly uint CONSOLE_LOG = Vault.Pool.Get("RPC_ConsoleLog");

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.ConsoleView)]
	private static void RPC_ConsoleTail(BridgeRead read)
	{
		var count = Math.Min(0, Output.HistoryOutput.Count - read.Int32());
		var logs = Output.HistoryOutput.Skip(count);
		var write = StartRpcResponse();
		write.WriteObject(logs.Count());
		foreach (var log in logs)
		{
			write.WriteObject(log.Message);
			write.WriteObject(log.Type);
			write.WriteObject(log.Time);
		}
		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.ConsoleInput)]
	private static void RPC_ConsoleInput(BridgeRead read)
	{
		var message = read.String();
		var connection = read.Connection;
		Community.Runtime.Core.NextFrame(() =>
		{
			string result = ConsoleSystem.Run(ConsoleSystem.Option.Server.Quiet(), message);
			if (!string.IsNullOrEmpty(result))
			{
				connection.Reply(result);
			}
		});
	}
	
	private static void OnLog(string message, string stacktrace, LogType type)
	{
		if (server == null)
		{
			return;
		}
		using var connections = Pool.Get<PooledList<BridgeConnection>>();
		foreach (var connection in server.Connections.Values)
		{
			if (connection.Reference is not Account account || !account.Permissions.console_view)
			{
				continue;
			}
			connections.Add(connection);
		}
		var write = StartRpcResponse(CONSOLE_LOG);
		write.WriteObject(message);
		write.WriteObject(Output.LogTypeToString.Get(type));
		write.WriteObject(Epoch.Current);
		SendRpcResponse(connections, write);
	}
}
