using Fleck;

namespace Carbon;

public static partial class WebControlPanel
{
	public static BridgeWrite StartRpcResponse(string rpc = null)
	{
		var write = BridgeWrite.Rent();
		write.BridgeMessage(BridgeMessages.Channels.Rpc);
		write.WriteObject(string.IsNullOrEmpty(rpc) ? currentRpcId : Vault.Pool.Get(rpc));
		return write;
	}

	public static void SendRpcResponse(BridgeConnection connection, BridgeWrite write)
	{
		connection.Send(write);
		BridgeWrite.Return(ref write);
	}

	public static void SendRpcResponse(List<BridgeConnection> connections, BridgeWrite write)
	{
		for (int i = 0; i < connections.Count; i++)
		{
			connections[i].Send(write);
		}
		BridgeWrite.Return(ref write);
	}

	public static void RpcResponse(BridgeRead read)
	{
		var write = StartRpcResponse();
		SendRpcResponse(read.Connection, write);
	}

	public static void RpcResponse<T1>(BridgeRead read, T1 arg1)
	{
		var write = StartRpcResponse();
		write.WriteObject(arg1);
		SendRpcResponse(read.Connection, write);
	}

	public static void RpcResponse<T1, T2>(BridgeRead read, T1 arg1, T2 arg2)
	{
		var write = StartRpcResponse();
		write.WriteObject(arg1);
		write.WriteObject(arg2);
		SendRpcResponse(read.Connection, write);
	}

	public static void RpcResponse<T1, T2, T3>(BridgeRead read, T1 arg1, T2 arg2, T3 arg3)
	{
		var write = StartRpcResponse();
		write.WriteObject(arg1);
		write.WriteObject(arg2);
		write.WriteObject(arg3);
		SendRpcResponse(read.Connection, write);
	}

	public static void RpcResponse<T1, T2, T3, T4>(BridgeRead read, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		var write = StartRpcResponse();
		write.WriteObject(arg1);
		write.WriteObject(arg2);
		write.WriteObject(arg3);
		write.WriteObject(arg4);
		SendRpcResponse(read.Connection, write);
	}

	public static void RpcResponse<T1, T2, T3, T4, T5>(BridgeRead read, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		var write = StartRpcResponse();
		write.WriteObject(arg1);
		write.WriteObject(arg2);
		write.WriteObject(arg3);
		write.WriteObject(arg4);
		write.WriteObject(arg5);
		SendRpcResponse(read.Connection, write);
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
