namespace Carbon.Documentation;

public static partial class WebRCon
{
	public static BridgeServer server;

	private static void StartServer()
	{
		server = new();
	}
}
