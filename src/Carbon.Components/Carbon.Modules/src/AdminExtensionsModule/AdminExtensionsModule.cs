using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Carbon.Base;
using Carbon.Extensions;
using Newtonsoft.Json;
using Oxide.Core;
using ProtoBuf;
using UnityEngine;

namespace Carbon.Modules;

public partial class AdminExtensionsModule : CarbonModule<AdminExtensionsConfig, EmptyModuleData>
{
	public override string Name => "AdminExtensions";
	public override VersionNumber Version => new(1, 0, 0);
	public override Type Type => typeof(AdminExtensionsModule);
	public override bool ForceModded => false;

#if !MINIMAL
	private readonly HashSet<ulong> _tpmUsers = [];
	private const string NoReason = "No reason given";

	public override void OnServerInit(bool initial)
	{
		base.OnServerInit(initial);
		OnEnabled(true);
	}

	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		if (!initialized)
		{
			return;
		}

		Permissions.RegisterPermission(ConfigInstance.NameFilter.BypassPermission, this);
		Permissions.RegisterPermission(ConfigInstance.Blind.Permission, this);
		Permissions.RegisterPermission(ConfigInstance.Empower.Permission, this);
		Permissions.RegisterPermission(ConfigInstance.PrivateMessage.Permission, this);
		Permissions.RegisterPermission(ConfigInstance.Lock.Permission, this);
		Permissions.RegisterPermission(ConfigInstance.TeleportMarker.Permission, this);
		Permissions.RegisterPermission(ConfigInstance.Mute.Permission, this);
		Permissions.RegisterPermission(ConfigInstance.MuteList.Permission, this);
		Permissions.RegisterPermission(ConfigInstance.Ban.Permission, this);
		Permissions.RegisterPermission(ConfigInstance.Unban.Permission, this);
		Permissions.RegisterPermission(ConfigInstance.Kick.Permission, this);
		Permissions.RegisterPermission(ConfigInstance.ToggleCadmin.Permission, this);

		Community.Runtime.Core.cmd.AddChatCommand(ConfigInstance.Blind.Command, this, nameof(CmdBlind));
		Community.Runtime.Core.cmd.AddChatCommand(ConfigInstance.Empower.Command, this, nameof(CmdEmpower));
		Community.Runtime.Core.cmd.AddChatCommand(ConfigInstance.PrivateMessage.Command, this, nameof(CmdPrivateMessage));
		Community.Runtime.Core.cmd.AddChatCommand(ConfigInstance.Lock.Command, this, nameof(CmdLockPlayerInventory));
		Community.Runtime.Core.cmd.AddChatCommand(ConfigInstance.TeleportMarker.Command, this, nameof(CmdTeleportMarker));
		Community.Runtime.Core.cmd.AddChatCommand(ConfigInstance.Mute.Command, this, nameof(CmdMute));
		Community.Runtime.Core.cmd.AddChatCommand(ConfigInstance.MuteList.Command, this, nameof(CmdMuteList));
		Community.Runtime.Core.cmd.AddChatCommand(ConfigInstance.Ban.Command, this, nameof(CmdBan));
		Community.Runtime.Core.cmd.AddChatCommand(ConfigInstance.Unban.Command, this, nameof(CmdUnban));
		Community.Runtime.Core.cmd.AddChatCommand(ConfigInstance.Kick.Command, this, nameof(CmdKick));
		Community.Runtime.Core.cmd.AddChatCommand(ConfigInstance.ToggleCadmin.Command, this, nameof(CmdToggleCadmin));
	}

	public override void OnDisabled(bool initialized)
	{
		base.OnDisabled(initialized);

		_tpmUsers.Clear();
	}

	[Conditional("!MINIMAL")]
	private void OnPlayerConnected(BasePlayer player)
	{
		if (Permissions.UserHasPermission(player.UserIDString, ConfigInstance.NameFilter.BypassPermission))
		{
			return;
		}

		if (!ConfigInstance.NameFilter.IsValid(player.displayName) &&
		    ConfigInstance.NameFilter.TryRename(player.displayName, out var newName))
		{
			switch (ConfigInstance.NameFilter.Mode)
			{
				case AdminExtensionsConfig.NameFilterSettings.FilterModes.Rename:
					var originalName = player.displayName;
					player.AsIPlayer().Rename(newName);
					Puts($"Updated {originalName}[{player.UserIDString}]'s name to {newName}");
					break;
			}
		}
	}

	[Conditional("!MINIMAL")]
	private object CanUserLogin(string username, string userid)
	{
		if (Permissions.UserHasPermission(userid, ConfigInstance.NameFilter.BypassPermission))
		{
			return null;
		}

		return (ConfigInstance.NameFilter.Mode, !ConfigInstance.NameFilter.IsValid(username)) switch
		{
			(AdminExtensionsConfig.NameFilterSettings.FilterModes.Kick, _) => ConfigInstance.NameFilter.KickMessage,
			_ => null
		};
	}

	[Conditional("!MINIMAL")]
	private void OnMapMarkerAdded(BasePlayer player, MapNote note)
	{
		if (_tpmUsers.Contains(player.userID))
		{
			var position = note.worldPosition + Vector3.up * (TerrainMeta.HeightMap.GetHeight(note.worldPosition) + 1f);
			note.Dispose();
			player.State.pointsOfInterest.Remove(note);
			player.DirtyPlayerState();
			player.SendMarkersToClient();
			player.Teleport(position);
			player.UpdateNetworkGroup();
			player.SendFullSnapshot();
		}
	}

	[Conditional("!MINIMAL")]
	private void CmdBlind(BasePlayer player, string _, string[] args)
	{
		if (!Permissions.UserHasPermission(player.UserIDString, ConfigInstance.Blind.Permission)) return;

		var targetPlayer = BasePlayer.Find(args[0]);
		if (targetPlayer == null)
		{
			player.ChatMessage($"Player '{args[0]}' not found.");
			return;
		}

		if (AdminModule.PlayersTab.BlindedPlayers.Contains(targetPlayer))
		{
			AdminModule.UnblindPlayer(player, targetPlayer);
			player.ChatMessage($"Unblinded {targetPlayer.displayName}.");
			return;
		}

		AdminModule.BlindPlayer(player, targetPlayer);
		player.ChatMessage($"Blinded {targetPlayer.displayName}.");
	}

	[Conditional("!MINIMAL")]
	private void CmdEmpower(BasePlayer player, string _, string[] args)
	{
		if (!Permissions.UserHasPermission(player.UserIDString, ConfigInstance.Empower.Permission)) return;

		var targetPlayer = BasePlayer.Find(args[0]);
		if (targetPlayer == null)
		{
			player.ChatMessage($"Player '{args[0]}' not found.");
			return;
		}

		AdminModule.EmpowerPlayerStats(player, targetPlayer);
		player.ChatMessage($"Empowered {targetPlayer.displayName}.");
	}

	[Conditional("!MINIMAL")]
	private void CmdPrivateMessage(BasePlayer player, string cmd, string[] args)
	{
		if (!Permissions.UserHasPermission(player.UserIDString, ConfigInstance.PrivateMessage.Permission)) return;

		if (args == null || args.Length < 2)
		{
			player.ChatMessage($"Usage: /{cmd} <player> <message>");
			return;
		}

		var targetPlayer = BasePlayer.Find(args[0]);
		if (targetPlayer == null)
		{
			player.ChatMessage($"Player '{args[0]}' not found.");
			return;
		}

		var message = string.Join(" ", args, 1, args.Length - 1);
		AdminModule.PrivateMessagePlayer(player, targetPlayer, message);
	}

	[Conditional("!MINIMAL")]
	private void CmdLockPlayerInventory(BasePlayer player, string cmd, string[] args)
	{
		if (!Permissions.UserHasPermission(player.UserIDString, ConfigInstance.Lock.Permission)) return;

		if (args == null || args.Length < 2)
		{
			player.ChatMessage($"Usage: /{cmd} <player> <main|wear|belt|all> [toggle]");
			return;
		}

		var targetPlayer = BasePlayer.Find(args[0]);
		if (targetPlayer == null)
		{
			player.ChatMessage($"Player '{args[0]}' not found.");
			return;
		}

		var shouldToggle = args.Length <= 2;
		var wants = args.Length > 2 && args[2].ToBool();

		switch (args[1])
		{
			case "main":
				AdminModule.LockPlayerContainer(player, targetPlayer, targetPlayer.inventory.containerMain,
					shouldToggle ? !targetPlayer.inventory.containerMain.IsLocked() : wants);
				break;
			case "wear":
				AdminModule.LockPlayerContainer(player, targetPlayer, targetPlayer.inventory.containerWear,
					shouldToggle ? !targetPlayer.inventory.containerWear.IsLocked() : wants);
				break;
			case "belt":
				AdminModule.LockPlayerContainer(player, targetPlayer, targetPlayer.inventory.containerBelt,
					shouldToggle ? !targetPlayer.inventory.containerBelt.IsLocked() : wants);
				break;
			case "all":
				AdminModule.LockPlayerContainer(player, targetPlayer, targetPlayer.inventory.containerBelt,
					shouldToggle ? !targetPlayer.inventory.containerBelt.IsLocked() : wants);
				AdminModule.LockPlayerContainer(player, targetPlayer, targetPlayer.inventory.containerWear,
					shouldToggle ? !targetPlayer.inventory.containerWear.IsLocked() : wants);
				AdminModule.LockPlayerContainer(player, targetPlayer, targetPlayer.inventory.containerMain,
					shouldToggle ? !targetPlayer.inventory.containerMain.IsLocked() : wants);
				break;
			default:
				player.ChatMessage($"Container '{args[0]}' not found.");
				return;
		}
	}

	[Conditional("!MINIMAL")]
	private void CmdTeleportMarker(BasePlayer player, string _, string[] args)
	{
		if (!Permissions.UserHasPermission(player.UserIDString, ConfigInstance.TeleportMarker.Permission)) return;

		if (_tpmUsers.Contains(player.userID))
		{
			player.ChatMessage("Teleport Marker disabled.");
			_tpmUsers.Remove(player.userID);

			return;
		}

		player.ChatMessage("Teleport Marker enabled.");
		_tpmUsers.Add(player.userID);
	}

	[Conditional("!MINIMAL")]
	private void CmdMute(BasePlayer player, string cmd, string[] args)
	{
		if (!Permissions.UserHasPermission(player.UserIDString, ConfigInstance.Mute.Permission)) return;

		if (args == null || args.Length == 0)
		{
			player.ChatMessage($"Usage: /{cmd} <player> [reason]");
			return;
		}

		var target = BasePlayer.Find(args[0]);
		if (target == null)
		{
			player.ChatMessage($"Player '{args[0]}' not found.");
			return;
		}

		var reason = string.Join(" ", args, 1, args.Length - 1);
		if (string.IsNullOrEmpty(reason))
		{
			reason = NoReason;
		}

		AdminModule.MutePlayer(player, target, !target.State.chatMuted, reason);
		player.ChatMessage($"You have {(target.State.chatMuted ? "muted" : "unmuted")} {target.displayName}. Reason: {reason}");
	}

	[Conditional("!MINIMAL")]
	private void CmdMuteList(BasePlayer player)
	{
		if (!Permissions.UserHasPermission(player.UserIDString, ConfigInstance.MuteList.Permission)) return;

		var message = Facepunch.Pool.Get<StringBuilder>();
		message.Clear();
		message.AppendLine("Muted Players:");
		var mutedPlayers = 0;
		foreach (var target in BasePlayer.activePlayerList)
		{
			if (!target.State.chatMuted) continue;
			message.AppendLine(target.displayName);
			mutedPlayers++;
		}

		message.AppendLine(mutedPlayers == 0
			? "No players are currently muted."
			: $"Total muted players: {mutedPlayers}");
		player.ChatMessage(message.ToString());
		Facepunch.Pool.FreeUnmanaged(ref message);
	}

	[Conditional("!MINIMAL")]
	private void CmdBan(BasePlayer player, string cmd, string[] args)
	{
		if (!Permissions.UserHasPermission(player.UserIDString, ConfigInstance.Ban.Permission)) return;

		if (args == null || args.Length == 0)
		{
			player.ChatMessage($"Usage: /{cmd} <player> [reason]");
			return;
		}

		var target = BasePlayer.FindAwakeOrSleeping(args[0]);
		if (target == null)
		{
			player.ChatMessage($"Player '{args[0]}' not found.");
			return;
		}

		var reason = string.Join(" ", args, 1, args.Length - 1);
		if (string.IsNullOrEmpty(reason))
		{
			reason = NoReason;
		}

		AdminModule.BanPlayer(player, target, reason);
		player.ChatMessage($"You have banned {target.displayName}. Reason: {reason}");
	}

	[Conditional("!MINIMAL")]
	private void CmdUnban(BasePlayer player, string cmd, string[] args)
	{
		if (!Permissions.UserHasPermission(player.UserIDString, ConfigInstance.Unban.Permission)) return;

		if (args == null || args.Length == 0)
		{
			player.ChatMessage($"Usage: /{cmd} <player>");
			return;
		}

		var target = BasePlayer.FindAwakeOrSleeping(args[0]);
		if (target == null)
		{
			player.ChatMessage($"Player '{args[0]}' not found.");
			return;
		}

		AdminModule.UnbanPlayer(player, target);
		player.ChatMessage($"You have unbanned {target.displayName}.");
	}

	[Conditional("!MINIMAL")]
	private void CmdKick(BasePlayer player, string cmd, string[] args)
	{
		if (!Permissions.UserHasPermission(player.UserIDString, ConfigInstance.Kick.Permission)) return;

		if (args == null || args.Length == 0)
		{
			player.ChatMessage($"Usage: /{cmd} <player> [reason]");
			return;
		}

		var target = BasePlayer.Find(args[0]);
		if (target == null)
		{
			player.ChatMessage($"Player '{args[0]}' not found.");
			return;
		}

		var reason = string.Join(" ", args, 1, args.Length - 1);
		if (string.IsNullOrEmpty(reason))
		{
			reason = NoReason;
		}

		AdminModule.KickPlayer(player, target, reason);
		player.ChatMessage($"You have kicked {target.displayName}. Reason: {reason}");
	}

	[Conditional("!MINIMAL")]
	private void CmdToggleCadmin(BasePlayer player, string _, string[] args)
	{
		if (!Permissions.UserHasPermission(player.UserIDString, ConfigInstance.ToggleCadmin.Permission)) return;

		var value = player.HasPlayerFlag(BasePlayer.PlayerFlags.IsDeveloper);
		player.SetPlayerFlag(BasePlayer.PlayerFlags.IsDeveloper, !value);
		player.ChatMessage($"You've {(!value ? "enabled" : "disabled")} <color=orange>cadmin</color> mode.");
	}

#endif
}

public class AdminExtensionsConfig
{
	/*
	 *
	 * Copyright (c) 2025 Carbon Community
	 * Copyright (c) 2025 headtapper
	 * All rights reserved.
	 *
	 */
	public class NameFilterSettings
	{
		[JsonProperty("Mode (0=None 1=Kick 2=Rename)")]
		public FilterModes Mode;

		public string BypassPermission = "adminextensions.namefilter.bypass";
		public string CharacterWhitelist = "._- ";
		public string KickMessage = "Your name must only contain English alphanumeric characters.";

		public enum FilterModes
		{
			None,
			Kick,
			Rename
		}

		public bool IsCharacterValid(char character)
		{
			return char.IsLetterOrDigit(character) || CharacterWhitelist.Contains(character.ToString());
		}

		public bool IsValid(string displayName)
		{
			for (int i = 0; i < displayName.Length; i++)
			{
				var character = displayName[i];
				if (!IsCharacterValid(character))
				{
					return false;
				}
			}

			return true;
		}

		public bool TryRename(string displayName, out string correctedName)
		{
			correctedName = string.Join(string.Empty, displayName.Where(x => IsCharacterValid(x)));
			return displayName != correctedName;
		}
	}

	public class CommandSettings
	{
		public string Command;
		public string Permission;
	}

	public NameFilterSettings NameFilter = new();

	public CommandSettings Blind = new() { Command = "blind", Permission = "adminextensions.blind" };

	public CommandSettings Empower = new() { Command = "empower", Permission = "adminextensions.empower" };

	public CommandSettings PrivateMessage = new() { Command = "cpm", Permission = "adminextensions.pm" };

	public CommandSettings Lock = new() { Command = "lock", Permission = "adminextensions.lock" };

	public CommandSettings TeleportMarker = new() { Command = "tpm", Permission = "adminextensions.tpm" };

	public CommandSettings Mute = new() { Command = "mute", Permission = "adminextensions.mute" };

	public CommandSettings MuteList = new() { Command = "mutelist", Permission = "adminextensions.mutelist" };

	public CommandSettings Ban = new() { Command = "ban", Permission = "adminextensions.ban" };

	public CommandSettings Unban = new() { Command = "unban", Permission = "adminextensions.unban" };

	public CommandSettings Kick = new() { Command = "kick", Permission = "adminextensions.kick" };

	public CommandSettings ToggleCadmin = new() { Command = "cadmin", Permission = "adminextensions.cadmin" };
}
