/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using System;
using System.Text.RegularExpressions;
using API.Commands;
using Carbon.Extensions;
using Carbon.Plugins;
using ConVar;
using Network;
using Oxide.Core;

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
		Interface.CallHook("OnPlayerConnected", player);
	}

	private object IOnUserApprove(Connection connection)
	{
		var username = connection.username;
		var text = connection.userid.ToString();
		var obj = Regex.Replace(connection.ipaddress, global::Oxide.Game.Rust.Libraries.Player.ipPattern, "");

		var canClient = Interface.CallHook("CanClientLogin", connection);
		var canUser = Interface.CallHook("CanUserLogin", username, text, obj);

		var obj4 = (canClient == null) ? canUser : canClient;
		if (obj4 is string || (obj4 is bool obj4Value && !obj4Value))
		{
			ConnectionAuth.Reject(connection, (obj4 is string) ? obj4.ToString() : "Connection was rejected", null);
			return true;
		}

		if (Interface.CallHook("OnUserApprove", connection) != null)
			return Interface.CallHook("OnUserApproved", username, text, obj);

		return null;
	}
	private void OnPlayerKicked(BasePlayer basePlayer, string reason)
	{
		Interface.CallHook("OnUserKicked", basePlayer.AsIPlayer(), reason);
	}
	private object OnPlayerRespawn(BasePlayer basePlayer)
	{
		return Interface.CallHook("OnUserRespawn", basePlayer.AsIPlayer());
	}
	private void OnPlayerRespawned(BasePlayer basePlayer)
	{
		Interface.CallHook("OnUserRespawned", basePlayer.AsIPlayer());
	}
	private void IOnPlayerBanned(Connection connection, AuthResponse status)
	{
		Interface.CallHook("OnPlayerBanned", connection, status.ToString());
	}
	private void OnClientAuth(Connection connection)
	{
		connection.username = Regex.Replace(connection.username, @"<[^>]*>", string.Empty);
	}

	#endregion

	#region Entity

	private void IOnEntitySaved(BaseNetworkable baseNetworkable, BaseNetworkable.SaveInfo saveInfo)
	{
		if (!Community.IsServerFullyInitializedCache || saveInfo.forConnection == null)
		{
			return;
		}

		Interface.CallHook("OnEntitySaved", baseNetworkable, saveInfo);
	}

	#endregion

	#region NPC

	private object IOnNpcTarget(BaseNpc npc, BaseEntity target)
	{
		if (Interface.CallHook("OnNpcTarget", npc, target) == null)
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
		if (!Community.IsServerFullyInitializedCache || _isPlayerTakingDamage || basePlayer == null || hitInfo == null || basePlayer.IsDead() || basePlayer is NPCPlayer)
		{
			return null;
		}

		if (Interface.CallHook("OnEntityTakeDamage", basePlayer, hitInfo) != null)
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
			return Interface.CallHook("OnEntityTakeDamage", basePlayer, hitInfo);
		}

		return null;
	}
	private object IOnBaseCombatEntityHurt(BaseCombatEntity entity, HitInfo hitInfo)
	{
		if (entity is not BasePlayer)
		{
			return Interface.CallHook("OnEntityTakeDamage", entity, hitInfo);
		}

		return null;
	}
	private object ICanPickupEntity(BasePlayer basePlayer, DoorCloser entity)
	{
		if (Interface.CallHook("CanPickupEntity", basePlayer, entity) is bool result)
		{
			return result;
		}

		return null;
	}
	private void OnPlayerSetInfo(Connection connection, string key, string val)
	{
		if (key == "global.language")
		{
			lang.SetLanguage(val, connection.userid.ToString());

			if (connection.player is BasePlayer player)
			{
				Interface.CallHook("OnPlayerLanguageChanged", player, val);
				Interface.CallHook("OnPlayerLanguageChanged", player.AsIPlayer(), val);
			}
		}
	}

	#endregion

	#region Server

	private void OnServerUserSet(ulong steamId, ServerUsers.UserGroup group, string playerName, string reason, long expiry)
	{
		if (Community.IsServerFullyInitializedCache && group == ServerUsers.UserGroup.Banned)
		{
			var playerId = steamId.ToString();
			var player = BasePlayer.FindByID(steamId).AsIPlayer();
			Interface.CallHook("OnPlayerBanned", playerName, steamId, player.Address ?? "0", reason, expiry);
			Interface.CallHook("OnUserBanned", playerName, playerId, player.Address ?? "0", reason, expiry);
		}
	}

	private void OnServerUserRemove(ulong steamId)
	{
		if (Community.IsServerFullyInitializedCache &&
			ServerUsers.users.ContainsKey(steamId) &&
			ServerUsers.users[steamId].group == ServerUsers.UserGroup.Banned)
		{
			var playerId = steamId.ToString();
			var player = BasePlayer.FindByID(steamId).AsIPlayer();
			Interface.CallHook("OnPlayerUnbanned", player.Name ?? "Unnamed", steamId, player.Address ?? "0");
			Interface.CallHook("OnUserUnbanned", player.Name ?? "Unnamed", playerId, player.Address ?? "0");
		}
	}

	#endregion

	#region Item

	// To fix:
	// Major issue with caching the 'OnLoseCondition' hook called in <redacted> (Type must not be ByRef Parameter name: typeArgs) (HookCallerCommon.CreateDelegate)
	/*
	private object IOnLoseCondition(Item item, float amount)
	{
		object[] arguments = { item, amount };
		Interface.CallHook("OnLoseCondition", arguments);
		amount = (float)arguments[1];

		var condition = item.condition;
		item.condition -= amount;
		if (item.condition <= 0f && item.condition < condition)
		{
			item.OnBroken();
		}

		return true;
	}*/

	#endregion

	#region Commands

	private object IOnPlayerCommand(BasePlayer player, string message)
	{
		if (Community.Runtime == null) return true;

		try
		{
			var fullString = message.Substring(1);
			var split = fullString.Split(ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries);
			var command = split[0].Trim();
			var args = split.Length > 1 ? Facepunch.Extend.StringExtensions.SplitQuotesStrings(fullString.Substring(command.Length + 1)) : _emptyStringArray;
			Array.Clear(split, 0, split.Length);

			if (HookCaller.CallStaticHook("OnPlayerCommand", player, command, args) != null)
			{
				return false;
			}

			var commandArgs = Facepunch.Pool.Get<PlayerArgs>();
			commandArgs.Arguments = args;
			commandArgs.Player = player;

			Community.Runtime.CommandManager.Contains(Community.Runtime.CommandManager.Chat, command, out var cmd);

			if (Community.Runtime.CommandManager.Execute(cmd, commandArgs))
			{
				Facepunch.Pool.Free(ref commandArgs);
				return false;
			}

			if (HookCaller.CallStaticHook("OnUnknownPlayerCommand", player, command, args) != null)
			{
				return false;
			}
		}
		catch (Exception ex) { Logger.Error($"Failed IOnPlayerCommand.", ex); }

		return true;
	}
	private object IOnServerCommand(ConsoleSystem.Arg arg)
	{
		if (arg != null && arg.cmd != null && arg.cmd.FullName == "chat.say") return null;

		if (HookCaller.CallStaticHook("OnServerCommand", arg) == null)
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
			return Interface.CallHook("OnPlayerOfflineChat", playerId, playerName, message, channel);
		}

		var hook1 = Interface.CallHook("OnPlayerChat", basePlayer, message, channel);
		var hook2 = Interface.CallHook("OnUserChat", basePlayer.AsIPlayer(), message);

		if (hook1 != null)
		{
			return hook1;
		}

		return hook2;
	}

	#endregion

	private void IOnServerShutdown()
	{
		Logger.Log($"Saving plugin configuration and data..");
		HookCaller.CallStaticHook("OnServerSave");

		Logger.Log($"Shutting down Carbon..");
		Interface.Oxide.OnShutdown();
		Community.Runtime.ScriptProcessor.Clear();
	}
}
