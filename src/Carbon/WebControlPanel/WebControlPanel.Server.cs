using Fleck;

namespace Carbon;

public static partial class WebControlPanel
{
	public static void RpcResponse(BridgeRead read)
	{
		using var writer = BridgeWriter.Begin();
		writer.write.BridgeMessage(BridgeMessages.Channels.Rpc);
		writer.write.WriteObject(currentRpcId);
		read.Connection.Send(writer.write);
	}

	public static void RpcResponse<T1>(BridgeRead read, T1 arg1)
	{
		using var writer = BridgeWriter.Begin();
		writer.write.BridgeMessage(BridgeMessages.Channels.Rpc);
		writer.write.WriteObject(currentRpcId);
		writer.write.WriteObject(arg1);
		read.Connection.Send(writer.write);
	}

	public static void RpcResponse<T1, T2>(BridgeRead read, T1 arg1, T2 arg2)
	{
		using var writer = BridgeWriter.Begin();
		writer.write.BridgeMessage(BridgeMessages.Channels.Rpc);
		writer.write.WriteObject(currentRpcId);
		writer.write.WriteObject(arg1);
		writer.write.WriteObject(arg2);
		read.Connection.Send(writer.write);
	}

	public static void RpcResponse<T1, T2, T3>(BridgeRead read, T1 arg1, T2 arg2, T3 arg3)
	{
		using var writer = BridgeWriter.Begin();
		writer.write.BridgeMessage(BridgeMessages.Channels.Rpc);
		writer.write.WriteObject(currentRpcId);
		writer.write.WriteObject(arg1);
		writer.write.WriteObject(arg2);
		writer.write.WriteObject(arg3);
		read.Connection.Send(writer.write);
	}

	public static void RpcResponse<T1, T2, T3, T4>(BridgeRead read, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		using var writer = BridgeWriter.Begin();
		writer.write.BridgeMessage(BridgeMessages.Channels.Rpc);
		writer.write.WriteObject(currentRpcId);
		writer.write.WriteObject(arg1);
		writer.write.WriteObject(arg2);
		writer.write.WriteObject(arg3);
		writer.write.WriteObject(arg4);
		read.Connection.Send(writer.write);
	}

	public static void RpcResponse<T1, T2, T3, T4, T5>(BridgeRead read, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		using var writer = BridgeWriter.Begin();
		writer.write.BridgeMessage(BridgeMessages.Channels.Rpc);
		writer.write.WriteObject(currentRpcId);
		writer.write.WriteObject(arg1);
		writer.write.WriteObject(arg2);
		writer.write.WriteObject(arg3);
		writer.write.WriteObject(arg4);
		writer.write.WriteObject(arg5);
		read.Connection.Send(writer.write);
	}

	public class Server : BridgeServer
	{
		public override bool OnSocketValidate(IWebSocketConnection socket)
		{
			return TryFindAccount(socket.ConnectionInfo.Path.TrimStart('/'), out _);
		}

		public override void OnBridgeConnection(BridgeConnection connection)
		{
			if (TryFindAccount(connection.Socket.ConnectionInfo.Path.TrimStart('/'), out var account))
			{
				connection.Reference = account;
			}
		}

		public override void OnBridgeDisconnection(BridgeConnection connection)
		{
		}
	}

	public class ServerMessages : BridgeMessages
	{
		protected override void OnCommand(BridgeRead read)
		{

		}

		protected override void OnCustom(BridgeRead read)
		{

		}

		protected override void OnRpc(BridgeRead read)
		{
			RunRpc(read);
		}

		protected override void OnUnhandled(BridgeRead read)
		{

		}
	}
}
