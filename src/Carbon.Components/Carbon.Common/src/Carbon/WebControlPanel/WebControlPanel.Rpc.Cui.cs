using Facepunch;
using Oxide.Game.Rust.Cui;

namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.DrawUi)]
	private static void RPC_AddUi(BridgeRead read)
	{
		var player = BasePlayer.FindAwakeOrSleepingByID(read.UInt64());
		if (!player.IsValid())
		{
			return;
		}

		var json = read.String();
		if (!string.IsNullOrWhiteSpace(json))
		{
			CuiHelper.AddUi(player, json);
		}
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.DrawUi)]
	private static void RPC_DestroyUi(BridgeRead read)
	{
		var player = BasePlayer.FindAwakeOrSleepingByID(read.UInt64());
		if (!player.IsValid())
		{
			return;
		}

		CuiHelper.DestroyUi(player, read.String());
	}
}
