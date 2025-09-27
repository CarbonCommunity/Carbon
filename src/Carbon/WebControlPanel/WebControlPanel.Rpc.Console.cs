using Facepunch;

namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.ConsoleView)]
	private static void RPC_ConsoleTail(BridgeRead read)
	{
		var logs = Output.HistoryOutput.Skip(read.Int32());
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
}
