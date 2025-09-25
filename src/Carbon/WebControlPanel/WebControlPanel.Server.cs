using Fleck;

namespace Carbon;

public static partial class WebControlPanel
{
	public static bool TryFindAccount(string password, out Account account)
	{
		return (account = FindAccount(password)) != null;
	}

	public static Account FindAccount(string password)
	{
		for (int i = 0; i < config.accounts.Length; i++)
		{
			var account = config.accounts[i];
			if (account.password == password)
			{
				return account;
			}
		}
		return null;
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

		}

		protected override void OnUnhandled(BridgeRead read)
		{

		}
	}
}
