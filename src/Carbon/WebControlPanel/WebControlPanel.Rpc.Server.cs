using Facepunch;

namespace Carbon;

public static partial class WebControlPanel
{
	[Rpc, Condition.Permission(PermissionTypes.ServerInfo)]
	private static void RPC_Test(BridgeRead read)
	{
		Logger.Log($"{read.Int32()} {read.String()}");

		RpcResponse(read);
	}
}
