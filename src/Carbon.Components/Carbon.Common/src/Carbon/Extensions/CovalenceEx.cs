public static class CovalenceEx
{
	public static RustPlayer AsIPlayer(this BasePlayer player)
	{
		if (player == null) return default;

		if (Permission.iPlayerField.GetValue(player) is not RustPlayer rustPlayer)
			Permission.iPlayerField.SetValue(player, rustPlayer = new RustPlayer(player));

		rustPlayer.Object = player;

		return rustPlayer;
	}

	public static RustPlayer AsIPlayer(this KeyValuePair<string, UserData> user)
	{
		if (user.Value == null) return default;

		if (user.Value.Player == null) user.Value.Player = new RustPlayer(user.Key, user.Value);

		return user.Value.Player;
	}
}
