using System;
using System.Collections.Generic;
using System.Linq;
using Carbon.Base;
using Carbon.Components;
using Epic.OnlineServices.Connect;
using Network;
using Newtonsoft.Json;
using Rust.AI;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public class VanishModule : CarbonModule<VanishConfig, EmptyModuleData>
{
	public override string Name => "Vanish";
	public override Type Type => typeof(VanishModule);
	public override bool ForceModded => true;
	public override bool EnabledByDefault => false;
	public CUI.Handler Handler { get; internal set; }

	internal Dictionary<ulong, Vector3> _vanishedPlayers = new(500);

	internal readonly GameObjectRef _drownEffect = new() { guid = "28ad47c8e6d313742a7a2740674a25b5" };
	internal readonly GameObjectRef _fallDamageEffect = new() { guid = "ca14ed027d5924003b1c5d9e523a5fce" };
	internal readonly GameObjectRef _emptyEffect = new();

	public override void Init()
	{
		base.Init();

		Handler = new();
	}
	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		Community.Runtime.CorePlugin.cmd.AddCovalenceCommand(ConfigInstance.VanishCommand, this, nameof(Vanish), authLevel: ConfigInstance.MinimumVanishAuthLevel);
	}
	public override void OnDisabled(bool initialized)
	{
		base.OnDisabled(initialized);

		foreach (var vanished in _vanishedPlayers)
		{
			DoVanish(BasePlayer.FindByID(vanished.Key), false);
		}

		_vanishedPlayers.Clear();
	}

	[HookPriority(Priorities.Highest)]
	private object CanUseLockedEntity(BasePlayer player, BaseLock @lock)
	{
		if (_vanishedPlayers.ContainsKey(player.userID)
			&& player.net.connection.authLevel >= ConfigInstance.MinimumAuthLevelUnlockWhileVanished)
		{
			return true;
		}

		return null;
	}

	[HookPriority(Priorities.Highest)]
	private object OnPlayerAttack(BasePlayer player, HitInfo hit)
	{
		if (hit == null || hit.Initiator == null || hit.HitEntity == null) return null;

		if(hit.Initiator is BasePlayer attacker && hit.HitEntity.OwnerID != attacker.userID)
		{
			if (!ConfigInstance.CanDamageWhenVanished && _vanishedPlayers.ContainsKey(attacker.userID))
			{
				var owner = BasePlayer.FindByID(hit.HitEntity.OwnerID);
				player.ChatMessage($"You're vanished. You may not damage this entity owned by {owner?.displayName ?? hit.HitEntity.OwnerID.ToString()}.");
				return false;
			}
		}

		return null;
	}

	private void OnPlayerConnected(BasePlayer player)
	{
		if (!_vanishedPlayers.ContainsKey(player.userID)) return;

		DoVanish(player, true);
	}

	public void DoVanish(BasePlayer player, bool wants, bool withUI = true)
	{
		if (wants)
		{
			player.PauseFlyHackDetection();
			AntiHack.ShouldIgnore(player);

			player.fallDamageEffect = _emptyEffect;
			player.drownEffect = _emptyEffect;

			player._limitedNetworking = true;
			player.DisablePlayerCollider();

			player.OnNetworkSubscribersLeave(Net.sv.connections.Where(connection => connection.connected && connection.isAuthenticated && connection.player is BasePlayer && connection.player != player).ToList());
			SimpleAIMemory.AddIgnorePlayer(player);

			if (withUI) _drawUI(player);

			if (ConfigInstance.EnableLogs) Puts($"{player} just vanished at {player.transform.position}");
		}
		else
		{
			player.ResetAntiHack();
			player._limitedNetworking = false;

			player.EnablePlayerCollider();
			player.SendNetworkUpdate();

			player.GetHeldEntity()?.SendNetworkUpdate();
			SimpleAIMemory.RemoveIgnorePlayer(player);

			player.drownEffect = _drownEffect;
			player.fallDamageEffect = _fallDamageEffect;

			if (ConfigInstance.GutshotScreamOnUnvanish) Effect.server.Run("assets/bundled/prefabs/fx/player/gutshot_scream.prefab", player.transform.position);

			using var cui = new CUI(Handler);
			cui.Destroy("vanishui", player);

			if (ConfigInstance.EnableLogs) Puts($"{player} unvanished at {player.transform.position}");
		}
	}

	private void Vanish(BasePlayer player, string cmd, string[] args)
	{
		var wants = false;

		if (_vanishedPlayers.TryGetValue(player.userID, out var originalPosition))
		{
			_vanishedPlayers.Remove(player.userID);
			if (ConfigInstance.TeleportBackOnUnvanish) player.Teleport(originalPosition);
		}
		else
		{
			_vanishedPlayers.Add(player.userID, player.transform.position);
			wants = true;
		}

		DoVanish(player, wants);
	}

	internal void _drawUI(BasePlayer player)
	{
		using var cui = new CUI(Handler);
		var container = cui.CreateContainer("vanishui", parent: CUI.ClientPanels.Hud);
		cui.CreateText(container, "vanishui", null, color: "#8bba49", ConfigInstance.InvisibleText, 10,
			yMax: 0.025f, align: UnityEngine.TextAnchor.LowerCenter);

		cui.Send(container, player);
	}
}

public class VanishConfig
{
	public int MinimumVanishAuthLevel = 2;
	public int MinimumAuthLevelUnlockWhileVanished = 2;
	public string VanishCommand = "vanish";
	public string InvisibleText = "You are currently invisible.";
	public bool GutshotScreamOnUnvanish = true;
	public bool EnableLogs = true;
	public bool TeleportBackOnUnvanish = false;
	public bool CanDamageWhenVanished = true;
}
