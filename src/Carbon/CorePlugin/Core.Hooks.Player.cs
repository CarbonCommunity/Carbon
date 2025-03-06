using Connection = Network.Connection;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	internal static object IOnBasePlayerAttacked(BasePlayer basePlayer, HitInfo hitInfo)
	{
		if (!Community.IsServerInitialized || _isPlayerTakingDamage || basePlayer == null || hitInfo == null || basePlayer.IsDead() || basePlayer is NPCPlayer)
		{
			return null;
		}

		// OnEntityTakeDamage
		if (HookCaller.CallStaticHook(952055589, basePlayer, hitInfo) != null)
		{
			return Cache.True;
		}

		_isPlayerTakingDamage = true;

		try
		{
			basePlayer.OnAttacked(hitInfo);
		}
		finally
		{
			_isPlayerTakingDamage = false;
		}

		return Cache.True;
	}
	internal static object IOnBasePlayerHurt(BasePlayer basePlayer, HitInfo hitInfo)
	{
		if (!_isPlayerTakingDamage)
		{
			// OnEntityTakeDamage
			return HookCaller.CallStaticHook(952055589, basePlayer, hitInfo);
		}

		return null;
	}
	internal static object IOnBaseCombatEntityHurt(BaseCombatEntity entity, HitInfo hitInfo)
	{
		if (entity is not BasePlayer)
		{
			// OnEntityTakeDamage
			return HookCaller.CallStaticHook(952055589, entity, hitInfo);
		}

		return null;
	}
	internal static object ICanPickupEntity(BasePlayer basePlayer, DoorCloser entity)
	{
		// CanPickupEntity
		if (HookCaller.CallStaticHook(861710679, basePlayer, entity) is bool result)
		{
			return result;
		}

		return null;
	}

	private void OnPlayerSetInfo(Connection connection, string key, string val)
	{
		switch (key)
		{
			case "global.language":
				lang.SetLanguage(val, connection.userid.ToString());

				if (connection.player is BasePlayer player)
				{
					// OnPlayerLanguageChanged
					HookCaller.CallStaticHook(1945313578, player, val);
					HookCaller.CallStaticHook(1945313578, player.AsIPlayer(), val);
				}
				break;
		}
	}
}
