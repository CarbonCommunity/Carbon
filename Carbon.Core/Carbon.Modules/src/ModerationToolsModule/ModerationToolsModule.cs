using System;
using System.Linq;
using Carbon.Base;
using Carbon.Core;
using Carbon.Extensions;
using ConVar;
using Newtonsoft.Json;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class ModerationToolsModule : CarbonModule<ModerationToolsConfig, EmptyModuleData>
{
	public override string Name => "ModerationTools";
	public override Type Type => typeof(ModerationToolsModule);
	public override bool ForceModded => false;

	public override bool EnabledByDefault => false;

	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		RegisterPermission(ConfigInstance.Moderation.Cmod1Permission);
		RegisterPermission(ConfigInstance.Moderation.Cmod2Permission);

		var cmod1Permissions = new string[] { ConfigInstance.Moderation.Cmod1Permission };
		var cmod2Permissions = new string[] { ConfigInstance.Moderation.Cmod2Permission };
		Community.Runtime.CorePlugin.cmd.AddCovalenceCommand(ConfigInstance.Moderation.CmodCommand, this, nameof(ToggleCadmin), permissions: cmod1Permissions, cooldown: ConfigInstance.Moderation.CmodCommandCooldown);
		Community.Runtime.CorePlugin.cmd.AddConsoleCommand("cmod.mute", this, nameof(Mute), permissions: cmod2Permissions, cooldown: ConfigInstance.Moderation.CmodCommandCooldown, silent: true);
		Community.Runtime.CorePlugin.cmd.AddConsoleCommand("cmod.unmute", this, nameof(Unmute), permissions: cmod2Permissions, cooldown: ConfigInstance.Moderation.CmodCommandCooldown, silent: true);
		Community.Runtime.CorePlugin.cmd.AddConsoleCommand("cmod.mutelist", this, nameof(MuteList), permissions: cmod2Permissions, cooldown: ConfigInstance.Moderation.CmodCommandCooldown, silent: true);
		Community.Runtime.CorePlugin.cmd.AddConsoleCommand("cmod.kick", this, nameof(Kick), permissions: cmod2Permissions, cooldown: ConfigInstance.Moderation.CmodCommandCooldown, silent: true);
		Community.Runtime.CorePlugin.cmd.AddConsoleCommand("cmod.ban", this, nameof(Ban), permissions: cmod2Permissions, cooldown: ConfigInstance.Moderation.CmodCommandCooldown, silent: true);
	}

	private object IDisallowSkinnedItemsFromBeingCraftable()
	{
		if (ConfigInstance.DisallowSkinnedItemsFromBeingCraftable) return true;

		return null;
	}
	private object INoteAdminHack(BasePlayer player)
	{
		if (HasPermission(player, ConfigInstance.Moderation.Cmod1Permission))
		{
			return false;
		}

		return null;
	}
	private object IServerEventToasts(GameTip.Styles style)
	{
		if (ConfigInstance.ShowServerEventToasts) return null;

		return false;
	}
	private object CanUnlockTechTreeNode()
	{
		if (ConfigInstance.NoTechTreeUnlock) return false;

		return null;
	}

	public void Mute(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();
		if (player == null) return;

		var playerName = arg.GetString(0);
		var reason = arg.Args.Skip(1).ToString(" ");

		var targetPlayer = BasePlayer.FindAwakeOrSleeping(playerName);
		if (targetPlayer == null)
		{
			player.ConsoleMessage($"Couldn't find that player.");
			return;
		}

		targetPlayer.SetPlayerFlag(BasePlayer.PlayerFlags.ChatMute, true);
		targetPlayer.ChatMessage($"{player.displayName} has muted you: {reason}");

		var log = $"{targetPlayer.displayName} has been muted by {player}: {reason}";
		player.ConsoleMessage(log);
		Puts(log);
	}
	public void Unmute(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();
		if (player == null) return;

		var playerName = arg.GetString(0);

		var targetPlayer = BasePlayer.FindAwakeOrSleeping(playerName);
		if (targetPlayer == null)
		{
			player.ConsoleMessage($"Couldn't find that player.");
			return;
		}

		targetPlayer.SetPlayerFlag(BasePlayer.PlayerFlags.ChatMute, false);
		targetPlayer.ChatMessage($"{player.displayName} has unmuted you.");

		var log = $"{targetPlayer.displayName} has been unmuted by {player}";
		player.ConsoleMessage(log);
		Puts(log);
	}
	public void MuteList(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();
		if (player == null) return;

		var obj = from x in global::BasePlayer.allPlayerList
				  where x.HasPlayerFlag(global::BasePlayer.PlayerFlags.ChatMute)
				  select new
				  {
					  SteamId = x.UserIDString,
					  Name = x.displayName
				  };

		var log = $"{JsonConvert.SerializeObject(obj, Formatting.Indented)}";
		player.ConsoleMessage(log);
		Puts(log);
	}
	public void Kick(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();
		if (player == null) return;

		var targetPlayer = BasePlayer.FindAwakeOrSleeping(arg.GetString(0));
		if (targetPlayer == null)
		{
			player.ConsoleMessage($"Couldn't find that player.");
			return;
		}

		var reason = arg.Args.Skip(1).ToString(" ") ?? "no reason given";

		Puts($"{player} kicked {targetPlayer}: {reason}");
		Chat.Broadcast($"Kicking {player.displayName} ({reason})", "SERVER", "#eee", 0UL);
		player.Kick("Kicked: " + reason);
	}
	public void Ban(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();
		if (player == null) return;

		var targetPlayer = BasePlayer.FindAwakeOrSleeping(arg.GetString(0));
		if (targetPlayer == null)
		{
			player.ConsoleMessage($"Couldn't find that player.");
			return;
		}

		var user = ServerUsers.Get(targetPlayer.userID);
		if (user != null && user.group == ServerUsers.UserGroup.Banned)
		{
			Puts($"User {player.userID} is already banned");
			return;
		}

		var reason = arg.Args.Skip(1).ToString(" ") ?? "no reason given";
		ServerUsers.Set(player.userID, global::ServerUsers.UserGroup.Banned, player.displayName, reason);
		ServerUsers.Save();

		Puts($"Kickbanned User: {player.userID} - {player.displayName}: {reason}");
		Chat.Broadcast($"Kickbanning {player.displayName} ({reason})", "SERVER", "#eee", 0UL);
		Network.Net.sv.Kick(player.net.connection, $"Banned: {reason}", false);
	}

	private object OnServerMessage(string message, string name)
	{
		var core = Community.Runtime.CorePlugin.To<CorePlugin>();
		var defaultName = core.DefaultServerChatName != "-1" ? core.DefaultServerChatName : "SERVER";

		if (!ConfigInstance.NoGiveNotices || !(name == defaultName && message.Contains("gave"))) return null;

		return true;
	}

	private void ToggleCadmin(BasePlayer player, string cmd, string[] args)
	{
		if (player == null)
		{
			Logger.Warn($"This command can only be called by a player (console/chat).");
			return;
		}

		var value = player.HasPlayerFlag(BasePlayer.PlayerFlags.IsDeveloper);
		player.SetPlayerFlag(BasePlayer.PlayerFlags.IsDeveloper, !value);
		player.ChatMessage($"You've {(!value ? "enabled" : "disabled")} <color=orange>cadmin</color> mode.");
	}
}

public class ModerationToolsConfig
{
	[JsonProperty("Disallow skinned items from being craftable")]
	public bool DisallowSkinnedItemsFromBeingCraftable = true;

	[JsonProperty("No give notices")]
	public bool NoGiveNotices = true;

	[JsonProperty("No TechTree unlock")]
	public bool NoTechTreeUnlock = false;

	[JsonProperty("Show server event toasts")]
	public bool ShowServerEventToasts = true;

	public ModerationSettings Moderation = new();

	public class ModerationSettings
	{
		[JsonProperty("/cadmin command")]
		public string CmodCommand = "cadmin";

		[JsonProperty("/cadmin command cooldown (ms)")]
		public int CmodCommandCooldown = 5000;

		[JsonProperty("/cadmin permission (developer)")]
		public string Cmod1Permission = "carbon.cadmin";

		[JsonProperty("/cmod permission (mute, kick, ban)")]
		public string Cmod2Permission = "carbon.cmod";
	}
}
