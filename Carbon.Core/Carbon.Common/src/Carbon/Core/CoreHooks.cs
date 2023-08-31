/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using API.Commands;
using ConVar;
using Connection = Network.Connection;

namespace Carbon.Core;
#pragma warning disable IDE0051

public partial class CorePlugin : CarbonPlugin
{
	internal static bool _isPlayerTakingDamage = false;
	internal readonly string[] _emptyStringArray = new string[0];

	#region Connection

	private void IOnPlayerConnected(BasePlayer player)
	{
		lang.SetLanguage(player.net.connection.info.GetString("global.language", "en"), player.UserIDString);
		player.SendEntitySnapshot(CommunityEntity.ServerInstance);

		permission.RefreshUser(player);

		// OnPlayerConnected
		HookCaller.CallStaticHook(3704844088, player);

		// OnUserConnected
		HookCaller.CallStaticHook(1971459992, player.AsIPlayer());

		Carbon.Client.CarbonClient.SendPing(player.Connection);
	}

	private object IOnUserApprove(Connection connection)
	{
		var username = connection.username;
		var text = connection.userid.ToString();
		var obj = Regex.Replace(connection.ipaddress, global::Oxide.Game.Rust.Libraries.Player.ipPattern, string.Empty);

		// CanClientLogin
		var canClient = HookCaller.CallStaticHook(351619588, connection);

		// CanUserLogin
		var canUser = HookCaller.CallStaticHook(459292092, username, text, obj);

		var obj4 = (canClient == null) ? canUser : canClient;
		if (obj4 is string || (obj4 is bool obj4Value && !obj4Value))
		{
			ConnectionAuth.Reject(connection, (obj4 is string) ? obj4.ToString() : "Connection was rejected", null);
			return true;
		}

		// OnUserApprove
		if (HookCaller.CallStaticHook(1855397793, connection) != null)
			// OnUserApproved
			return HookCaller.CallStaticHook(2225250284, username, text, obj);

		return null;
	}
	private void OnPlayerKicked(BasePlayer basePlayer, string reason)
	{
		// OnUserKicked
		HookCaller.CallStaticHook(3026194467, basePlayer.AsIPlayer(), reason);
	}
	private object OnPlayerRespawn(BasePlayer basePlayer)
	{
		// OnUserRespawn
		return HookCaller.CallStaticHook(2545052102, basePlayer.AsIPlayer());
	}
	private void OnPlayerRespawned(BasePlayer basePlayer)
	{
		// OnUserRespawned
		HookCaller.CallStaticHook(3161392945, basePlayer.AsIPlayer());
	}
	private void IOnPlayerBanned(Connection connection, AuthResponse status)
	{
		// OnPlayerBanned
		HookCaller.CallStaticHook(2433979267, connection, status.ToString());
	}
	private void OnClientAuth(Connection connection)
	{
		connection.username = Regex.Replace(connection.username, @"<[^>]*>", string.Empty);
	}

	#endregion

	#region Entity

	private void IOnEntitySaved(BaseNetworkable baseNetworkable, BaseNetworkable.SaveInfo saveInfo)
	{
		if (!Community.IsServerInitialized || saveInfo.forConnection == null)
		{
			return;
		}

		// OnEntitySaved
		HookCaller.CallStaticHook(3947573992, baseNetworkable, saveInfo);
	}

	#endregion

	#region NPC

	private object IOnNpcTarget(BaseNpc npc, BaseEntity target)
	{
		// OnNpcTarget
		if (HookCaller.CallStaticHook(1265749384, npc, target) == null)
		{
			return null;
		}

		npc.SetFact(BaseNpc.Facts.HasEnemy, 0);
		npc.SetFact(BaseNpc.Facts.EnemyRange, 3);
		npc.SetFact(BaseNpc.Facts.AfraidRange, 1);

		npc.playerTargetDecisionStartTime = 0f;
		return 0f;
	}

	#endregion

	#region Player

	private object IOnBasePlayerAttacked(BasePlayer basePlayer, HitInfo hitInfo)
	{
		if (!Community.IsServerInitialized || _isPlayerTakingDamage || basePlayer == null || hitInfo == null || basePlayer.IsDead() || basePlayer is NPCPlayer)
		{
			return null;
		}

		// OnEntityTakeDamage
		if (HookCaller.CallStaticHook(2713007450, basePlayer, hitInfo) != null)
		{
			return true;
		}

		_isPlayerTakingDamage = true;

		try
		{
			basePlayer.OnAttacked(hitInfo);
		}
		finally { }

		_isPlayerTakingDamage = false;

		return true;
	}
	private object IOnBasePlayerHurt(BasePlayer basePlayer, HitInfo hitInfo)
	{
		if (!_isPlayerTakingDamage)
		{
			// OnEntityTakeDamage
			return HookCaller.CallStaticHook(2713007450, basePlayer, hitInfo);
		}

		return null;
	}
	private object IOnBaseCombatEntityHurt(BaseCombatEntity entity, HitInfo hitInfo)
	{
		if (entity is not BasePlayer)
		{
			// OnEntityTakeDamage
			return HookCaller.CallStaticHook(2713007450, entity, hitInfo);
		}

		return null;
	}
	private object ICanPickupEntity(BasePlayer basePlayer, DoorCloser entity)
	{
		// CanPickupEntity
		if (HookCaller.CallStaticHook(385185486, basePlayer, entity) is bool result)
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
					HookCaller.CallStaticHook(1960580409, player, val);
					HookCaller.CallStaticHook(1960580409, player.AsIPlayer(), val);
				}
				break;
		}
	}

	#endregion

	#region Server

	internal const string _blankZero = "0";
	internal const string _blankUnnamed = "Unnamed";

	private void OnServerUserSet(ulong steamId, ServerUsers.UserGroup group, string playerName, string reason, long expiry)
	{
		if (Community.IsServerInitialized && group == ServerUsers.UserGroup.Banned)
		{
			var playerId = steamId.ToString();
			var player = BasePlayer.FindByID(steamId)?.AsIPlayer();

			// OnPlayerBanned
			HookCaller.CallStaticHook(2433979267, playerName, steamId, player == null ? _blankZero : player.Address, reason, expiry);

			// OnUserBanned
			HookCaller.CallStaticHook(274222292, playerName, playerId, player == null ? _blankZero : player.Address, reason, expiry);
		}
	}
	private void OnServerUserRemove(ulong steamId)
	{
		if (Community.IsServerInitialized &&
			ServerUsers.users.ContainsKey(steamId) &&
			ServerUsers.users[steamId].group == ServerUsers.UserGroup.Banned)
		{
			var playerId = steamId.ToString();
			var player = BasePlayer.FindByID(steamId)?.AsIPlayer();

			// OnPlayerUnbanned
			HookCaller.CallStaticHook(3462729840, player == null || string.IsNullOrEmpty(player.Name) ? _blankUnnamed : player.Name, playerId, player == null || string.IsNullOrEmpty(player.Address) ? _blankZero : player.Address);

			// OnUserUnbanned
			HookCaller.CallStaticHook(4090556101, player == null || string.IsNullOrEmpty(player.Name) ? _blankUnnamed : player.Name, playerId, player == null || string.IsNullOrEmpty(player.Address) ? _blankZero : player.Address);
		}
	}

	#endregion

	#region Item

	private object IOnLoseCondition(Item item, float amount)
	{
		var args = new object[] { item, amount };
		HookCaller.CallStaticHook(3503014187, args, keepArgs: true);
		amount = (float)args[1];

		var condition = item.condition;
		item.condition -= amount;
		if (item.condition <= 0f && item.condition < condition)
		{
			item.OnBroken();
		}

		return true;
	}

	#endregion

	#region Commands

	private object IOnPlayerCommand(BasePlayer player, string message)
	{
		if (Community.Runtime == null) return true;

		try
		{
			var fullString = message[1..];

			if (string.IsNullOrEmpty(fullString))
			{
				return false;
			}

			var split = fullString.Split(ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries);
			var command = split[0].Trim();
			var args = split.Length > 1 ? Facepunch.Extend.StringExtensions.SplitQuotesStrings(fullString[(command.Length + 1)..]) : _emptyStringArray;
			Array.Clear(split, 0, split.Length);

			// OnUserCommand
			if (HookCaller.CallStaticHook(1077563450, player, command, args) != null)
			{
				return false;
			}

			// OnUserCommand
			if (HookCaller.CallStaticHook(2623980812, player.AsIPlayer(), command, args) != null)
			{
				return false;
			}

			if (Community.Runtime.CommandManager.Contains(Community.Runtime.CommandManager.Chat, command, out var cmd))
			{
				var commandArgs = Facepunch.Pool.Get<PlayerArgs>();
				commandArgs.Type = cmd.Type;
				commandArgs.Arguments = args;
				commandArgs.Player = player;
				commandArgs.PrintOutput = true;

				Community.Runtime.CommandManager.Execute(cmd, commandArgs);

				commandArgs.Dispose();
				Facepunch.Pool.Free(ref commandArgs);
				return false;
			}

			if (HookCaller.CallStaticHook(554444971, player, command, args) != null)
			{
				return false;
			}
		}
		catch (Exception ex) { Logger.Error($"Failed IOnPlayerCommand.", ex); }

		return true;
	}
	private object IOnServerCommand(ConsoleSystem.Arg arg)
	{
		if (arg != null && arg.cmd != null && arg.Player() != null && arg.cmd.FullName == "chat.say") return null;

		// OnServerCommand
		if (HookCaller.CallStaticHook(3282920085, arg) == null)
		{
			return null;
		}

		return true;
	}
	private object IOnPlayerChat(ulong playerId, string playerName, string message, Chat.ChatChannel channel, BasePlayer basePlayer)
	{
		if (string.IsNullOrEmpty(message) || message.Equals("text"))
		{
			return true;
		}
		if (basePlayer == null || !basePlayer.IsConnected)
		{
			// OnPlayerOfflineChat
			return HookCaller.CallStaticHook(3391949391, playerId, playerName, message, channel);
		}

		// OnPlayerChat
		var hook1 = HookCaller.CallStaticHook(735197859, basePlayer, message, channel);

		// OnUserChat
		var hook2 = HookCaller.CallStaticHook(2410402155, basePlayer.AsIPlayer(), message);

		if (hook1 != null)
		{
			return hook1;
		}

		return hook2;
	}

	#endregion

	private void IOnServerInitialized()
	{
		if (!Community.IsServerInitialized)
		{
			Community.IsServerInitialized = true;

			Community.Runtime.Analytics.LogEvent("on_server_initialized",
				segments: Community.Runtime.Analytics.Segments,
				metrics: new Dictionary<string, object> {
					{ "plugin_count", ModLoader.LoadedPackages.Sum(x => x.Plugins.Count) },
					{ "plugins_totalmemoryused", $"{ByteEx.Format(ModLoader.LoadedPackages.Sum(x => x.Plugins.Sum(y => y.TotalMemoryUsed)), valueFormat: "0", stringFormat: "{0}{1}").ToLower()}" },
					{ "plugins_totalhooktime", $"{ModLoader.LoadedPackages.Sum(x => x.Plugins.Sum(y => y.TotalHookTime)).RoundUpToNearestCount(100):0}ms" },
					{ "extension_count",Community.Runtime.AssemblyEx.Extensions.Loaded.Count },
					{ "module_count", Community.Runtime.AssemblyEx.Modules.Loaded.Count },
					{ "hook_count", Community.Runtime.HookManager.LoadedDynamicHooks.Count(x => x.IsInstalled) + Community.Runtime.HookManager.LoadedStaticHooks.Count(x => x.IsInstalled) }
				}
			);
		}
	}
	private void IOnServerShutdown()
	{
		Logger.Log($"Saving plugin configuration and data..");
		HookCaller.CallStaticHook(2032593992);

		Logger.Log($"Shutting down Carbon..");
		Interface.Oxide.OnShutdown();
		Community.Runtime.ScriptProcessor.Clear();
	}
}
