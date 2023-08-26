using Carbon.Client.Packets;

namespace Carbon.Client;

public class RPCHooks
{
	[RPC.Method("pong")]
	private static void Pong(BasePlayer player, Network.Message message)
	{
		var client = CarbonClient.Get(player);

		if (client.HasCarbonClient)
		{
			Logger.Warn($"Player '{player.Connection}' attempted registering twice.");
			return;
		}

		var result = CarbonClient.Receive<RPCList>(message);
		result.Sync();
		result.Dispose();

		client.HasCarbonClient = true;
		client.Send("clientinfo");
		Logger.Log($"{client.Connection} joined with Carbon client");
	}

	[RPC.Method("inventoryopen")]
	private static void InventoryOpen(BasePlayer player, Network.Message message)
	{
		// OnInventoryOpen
		HookCaller.CallStaticHook(3601759205, player);
	}

	[RPC.Method("inventoryclose")]
	private static void InventoryClose(BasePlayer player, Network.Message message)
	{
		// OnInventoryClose
		HookCaller.CallStaticHook(3858974801, player);
	}

	[RPC.Method("clientinfo")]
	private static void ClientInfo(BasePlayer player, Network.Message message)
	{
		var info = CarbonClient.Receive<ClientInfo>(message);
		var client = CarbonClient.Get(player);

		client.ScreenWidth = info.ScreenWidth;
		client.ScreenHeight = info.ScreenHeight;
	}

}
