namespace Carbon.Client;

public class RPCHooks
{
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
}
