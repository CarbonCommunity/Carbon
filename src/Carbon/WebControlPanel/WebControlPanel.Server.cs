using Fleck;

namespace Carbon;

public static partial class WebControlPanel
{
	public class Server : BridgeServer
	{
		public override void OnBridgeConnection(BridgeConnection connection)
		{
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
