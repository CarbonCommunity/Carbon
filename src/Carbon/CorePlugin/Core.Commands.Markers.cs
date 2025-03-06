namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("wipemarkers", "Removes all markers of the calling player or argument filter.")]
	[AuthLevel(2)]
	private void ClearMarkers(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();

		if (arg.HasArgs(1))
		{
			player = BasePlayer.FindAwakeOrSleeping(arg.GetString(0));
		}

		if (player == null)
		{
			arg.ReplyWith($"Couldn't find that player.");
			return;
		}

		arg.ReplyWith(arg.IsServerside ? $"Removed {player.displayName}'s map notes." : $"Removed all map notes.");

		player.Server_ClearMapMarkers(default);
		player.SendMarkersToClient();
		player.SendNetworkUpdate();
	}
}
