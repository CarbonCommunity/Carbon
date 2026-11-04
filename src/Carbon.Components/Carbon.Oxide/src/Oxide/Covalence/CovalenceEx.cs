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
		var rustPlayer = field?.GetValue(player) as RustPlayer;

		// The covalence database and the online player share one instance, so lookups by id see the live player
		if (rustPlayer == null && !_players.TryGetValue(player.UserIDString, out rustPlayer))
		{
			rustPlayer = new RustPlayer(player);
		}

		field?.SetValue(player, rustPlayer);
		_players[player.UserIDString] = rustPlayer;

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
