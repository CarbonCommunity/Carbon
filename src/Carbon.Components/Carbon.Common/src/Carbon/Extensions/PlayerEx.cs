namespace Carbon.Extensions;

/// <summary>
/// Common player administration helpers.
/// </summary>
public static class PlayerEx
{
	public static bool IsBanned(ulong userId) => ServerUsers.Is(userId, ServerUsers.UserGroup.Banned);

	/// <summary>Bans (and kicks) a player. A default duration bans permanently.</summary>
	public static void Ban(ulong userId, string displayName, string reason, TimeSpan duration = default)
	{
		if (IsBanned(userId))
		{
			return;
		}

		var expiryUnixTime = -1L;
		if (duration != TimeSpan.Zero)
		{
			expiryUnixTime = new DateTimeOffset(DateTime.UtcNow.Add(duration)).ToUnixTimeSeconds();
		}

		ServerUsers.Set(userId, ServerUsers.UserGroup.Banned, displayName ?? "Unknown", reason, expiryUnixTime);
		ServerUsers.Save();

		var player = BasePlayer.FindByID(userId);
		if (player != null && player.IsConnected)
		{
			player.Kick(reason);
		}
	}

	public static void Ban(this BasePlayer player, string reason, TimeSpan duration = default)
	{
		Ban(player.userID, player.displayName, reason, duration);
	}

	/// <summary>Renames a player everywhere: persistence, connection, network and permission data.</summary>
	public static void Rename(this BasePlayer player, string name)
	{
		name = string.IsNullOrEmpty(name?.Trim()) ? player.displayName : name;

		SingletonComponent<ServerMgr>.Instance.persistance.SetPlayerName(player.userID, name);

		if (player.net?.connection != null)
		{
			player.net.connection.username = name;
		}

		player.displayName = name;
		player._name = name;
		player.SendNetworkUpdateImmediate();

		Community.Runtime.Permission.UpdateNickname(player.UserIDString, name);

		RefreshForOtherClients(player);
	}

	/// <summary>Re-sends the player to everyone who can see it, so visual changes (like names) show up right away.</summary>
	public static void RefreshForOtherClients(BasePlayer player)
	{
		if (global::Rust.Application.isLoading || global::Rust.Application.isLoadingSave || player.IsDestroyed || !player.isSpawned || player.net?.group == BaseNetworkable.LimboNetworkGroup)
		{
			return;
		}

		var connections = player.GetSubscribers();
		if (connections == null)
		{
			return;
		}

		for (var i = 0; i < connections.Count; i++)
		{
			var connection = connections[i];
			if (!connection.connected || connection.player is not BasePlayer viewer || viewer == null || viewer == player || !player.ShouldNetworkTo(viewer))
			{
				continue;
			}

			player.DestroyOnClient(connection);
			player.SendAsSnapshotWithChildren(viewer);
		}
	}
}
