public static class CovalenceEx
{
	private static FieldInfo _iPlayerField;
	private static bool _iPlayerFieldResolved;
	private static readonly Dictionary<string, RustPlayer> _players = new();

	/// <summary>BasePlayer.IPlayer, injected into Assembly-CSharp by the publicizer. Null when it isn't patched in.</summary>
	public static FieldInfo IPlayerField
	{
		get
		{
			if (!_iPlayerFieldResolved)
			{
				_iPlayerField = typeof(BasePlayer).GetField("IPlayer", BindingFlags.Public | BindingFlags.Instance);
				_iPlayerFieldResolved = true;
			}

			return _iPlayerField;
		}
	}

	public static RustPlayer AsIPlayer(this BasePlayer player)
	{
		if (player == null) return default;

		var field = IPlayerField;
		var rustPlayer = field != null ? field.GetValue(player) as RustPlayer : _players.TryGetValue(player.UserIDString, out var cached) ? cached : null;

		if (rustPlayer == null)
		{
			rustPlayer = new RustPlayer(player);

			if (field != null) field.SetValue(player, rustPlayer);
			else _players[player.UserIDString] = rustPlayer;
		}

		rustPlayer.Object = player;
		return rustPlayer;
	}

	public static RustPlayer AsIPlayer(this KeyValuePair<string, UserData> user)
	{
		if (user.Value == null) return default;

		if (!_players.TryGetValue(user.Key, out var rustPlayer))
		{
			_players[user.Key] = rustPlayer = new RustPlayer(user.Key, user.Value);
		}

		return rustPlayer;
	}
}
