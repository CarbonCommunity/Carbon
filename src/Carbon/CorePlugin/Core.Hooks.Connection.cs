using Connection = Network.Connection;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	internal static object IOnPlayerConnected(BasePlayer player)
	{
		var core = Singleton<CorePlugin>();

		player.SendEntitySnapshot(CommunityEntity.ServerInstance);

		core.permission.RefreshUser(player);

		// OnPlayerConnected
		HookCaller.CallStaticHook(2848347654, player);

		// OnUserConnected
		HookCaller.CallStaticHook(1253832323, player.AsIPlayer());

		return null;
	}
	internal static object IOnUserApprove(Connection connection)
	{
		var username = connection.username;
		var text = connection.userid.ToString();
		var obj = Regex.Replace(connection.ipaddress, Player.ipPattern, string.Empty);

		// CanClientLogin
		var canClient = HookCaller.CallStaticHook(3081308902, connection);

		// CanUserLogin
		var canUser = HookCaller.CallStaticHook(1045800646, username, text, obj);

		var obj4 = (canClient == null) ? canUser : canClient;
		if (obj4 is string || (obj4 is bool obj4Value && !obj4Value))
		{
			ConnectionAuth.Reject(connection, (obj4 is string) ? obj4.ToString() : "Connection was rejected", null);
			return Cache.True;
		}

		if (Community.Runtime.ClientConfig.Enabled)
		{
			Community.Runtime.CarbonClient.OnConnected(connection);
		}

		// OnUserApprove
		if (HookCaller.CallStaticHook(2666432541, connection) != null)
		{
			// OnUserApproved
			return HookCaller.CallStaticHook(1330253375, username, text, obj);
		}

		return null;
	}
	internal static object IOnPlayerBanned(Connection connection, AuthResponse status)
	{
		// OnPlayerBanned
		HookCaller.CallStaticHook(140408349, connection, status.ToString());

		return null;
	}

	private void OnPlayerDisconnected(BasePlayer player, string reason)
	{
		// OnUserDisconnected
		HookCaller.CallStaticHook(649612044, player?.AsIPlayer(), reason);

		if (player.IsAdmin && !player.IsOnGround())
		{
			var newPosition = player.transform.position;

			if (Physics.Raycast(newPosition, Vector3.down, out var hit, float.MaxValue, ~0, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				newPosition.y = hit.point.y;

				if (Vector3.Distance(player.transform.position, newPosition) > 3.5f)
				{
					player.SetServerFall(false);
					player.Teleport(newPosition);
					player.estimatedVelocity = Vector3.zero;
					NextFrame(() =>
					{
						if (player != null)
						{
							player.SetServerFall(true);
						}
					});
					Logger.Warn($"Moved admin player {player.net.connection} on the object underneath so it doesn't die from fall damage.");
				}
			}
		}

		if (Community.Runtime.ClientConfig.Enabled)
		{
			Community.Runtime.CarbonClient.OnDisconnected(player.Connection);
		}
	}
	private void OnPlayerKicked(BasePlayer basePlayer, string reason)
	{
		// OnUserKicked
		HookCaller.CallStaticHook(3928650942, basePlayer.AsIPlayer(), reason);
	}
	private object OnPlayerRespawn(BasePlayer basePlayer)
	{
		// OnUserRespawn
		return HookCaller.CallStaticHook(3398288406, basePlayer.AsIPlayer());
	}
	private void OnPlayerRespawned(BasePlayer basePlayer)
	{
		// OnUserRespawned
		HookCaller.CallStaticHook(960522643, basePlayer.AsIPlayer());
	}
	private void OnClientAuth(Connection connection)
	{
		connection.username = Regex.Replace(connection.username, @"<[^>]*>", string.Empty);
	}
}
