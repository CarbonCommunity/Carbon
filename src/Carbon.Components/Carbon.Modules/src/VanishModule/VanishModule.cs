using System;
using System.Collections.Generic;
using System.Linq;
using Carbon.Base;
using Carbon.Components;
using Facepunch;
using HarmonyLib;
using JetBrains.Annotations;
using Network;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Plugins;
using Rust;
using Rust.Ai;
using UnityEngine;

namespace Carbon.Modules;

public partial class VanishModule : CarbonModule<VanishConfig, EmptyModuleData>
{
	private static VanishModule Singleton;

	public override string Name => "Vanish";
	public override Type Type => typeof(VanishModule);
	public override VersionNumber Version => new(1, 0, 0);
	public override bool ForceModded => false;
	public override bool EnabledByDefault => false;

	private readonly CUI.Handler Handler = new();

	private Dictionary<ulong, Vector3> _vanishedPlayers = new(500);
	private BasePlayer _lastLooter;

	private readonly GameObjectRef _drownEffect = new() { guid = "28ad47c8e6d313742a7a2740674a25b5" };
	private readonly GameObjectRef _fallDamageEffect = new() { guid = "ca14ed027d5924003b1c5d9e523a5fce" };
	private readonly GameObjectRef _emptyEffect = new();

	public override void OnServerInit(bool initial)
	{
		Singleton = this;
		base.OnServerInit(initial);

		if (!initial) return;

		OnEnabled(true);
	}
	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		if (!initialized) return;

		Permissions.RegisterPermission(ConfigInstance.VanishPermission, this);
		Permissions.RegisterPermission(ConfigInstance.VanishUnlockWhileVanishedPermission, this);
		Permissions.RegisterPermission(ConfigInstance.PermanentVanishPermission, this);

		Community.Runtime.Core.cmd.AddCovalenceCommand(ConfigInstance.VanishCommand, this, nameof(Vanish), permissions: new [] { ConfigInstance.VanishPermission });
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

	public bool IsPlayerVanished(ulong playerId) => _vanishedPlayers.ContainsKey(playerId);
	public bool IsPlayerVanished(BasePlayer player) => player != null && _vanishedPlayers.ContainsKey(player.userID);

	public Vector3 GetVanishedPlayerPosition(ulong playerId) => _vanishedPlayers.TryGetValue(playerId, out var position) ? position : Vector3.zero;
	public Vector3 GetVanishedPlayerPosition(BasePlayer player) => player != null && _vanishedPlayers.TryGetValue(player.userID, out var position) ? position : Vector3.zero;

	private object CanUseLockedEntity(BasePlayer player, BaseLock @lock)
	{
		if (_vanishedPlayers.ContainsKey(player.userID)
			&& Permissions.UserHasPermission(player.UserIDString, ConfigInstance.VanishUnlockWhileVanishedPermission))
		{
			return true;
		}

		return null;
	}
	private object OnPlayerAttack(BasePlayer player, HitInfo hit)
	{
		if (hit == null || hit.Initiator == null || hit.HitEntity == null) return null;

		if (hit.Initiator is BasePlayer attacker && hit.HitEntity.OwnerID != attacker.userID)
		{
			if (!ConfigInstance.CanDamageWhenVanished && _vanishedPlayers.ContainsKey(attacker.userID))
			{
				player.ChatMessage($"You're vanished. You may not damage this entity owned by {BasePlayer.FindByID(hit.HitEntity.OwnerID)?.displayName ?? hit.HitEntity.OwnerID.ToString()}.");
				return false;
			}
		}

		return null;
	}
	private object CanBradleyApcTarget(BradleyAPC apc, BasePlayer player)
	{
		if (_vanishedPlayers.ContainsKey(player.userID))
		{
			return false;
		}

		return null;
	}
	private void OnPlayerSleepEnded(BasePlayer self)
	{
		if (Permissions.UserHasPermission(self.UserIDString, ConfigInstance.PermanentVanishPermission))
		{
			DoVanish(self, true);
			return;
		}

		if (!_vanishedPlayers.ContainsKey(self.userID))
		{
			return;
		}
		DoVanish(self, true);
	}

	private void CanLootEntity(BasePlayer player, ContainerIOEntity container)
	{
		_lastLooter = player;
	}

	private static void SendEffectTo(string effect, BasePlayer player)
	{
		if (player == null)
		{
			return;
		}

		var effectInstance = Effect.reusableInstance;
		effectInstance.Init(Effect.Type.Generic, player, 0, Vector3.up, Vector3.zero);
		effectInstance.pooledstringid = StringPool.Get(effect);

		var netWrite = Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.Effect);
		effectInstance.WriteToStream(netWrite);
		netWrite.Send(new SendInfo(player.net.connection));
		effectInstance.Clear();
	}

	public void DoVanish(BasePlayer player, bool wants, bool withUI = true, bool toggleNoclip = true)
	{
		if (!wants && _vanishedPlayers.TryGetValue(player.userID, out var originalPosition))
		{
			if (Permissions.UserHasPermission(player.UserIDString, ConfigInstance.PermanentVanishPermission))
			{
				player.ChatMessage("You're permanently vanished due to your permission and/or group.");
				return;
			}

			_vanishedPlayers.Remove(player.userID);
			if (ConfigInstance.TeleportBackOnUnvanish)
			{
				player.Teleport(originalPosition);
			}
		}
		else if(wants && !_vanishedPlayers.ContainsKey(player.userID))
		{
			_vanishedPlayers.Add(player.userID, player.transform.position);
		}

		if (wants)
		{
			_clearTriggers(player);

			player.PauseFlyHackDetection(float.MaxValue);
			player.PauseSpeedHackDetection(float.MaxValue);
			player.PauseTickDistanceDetection(float.MaxValue);
			player.PauseVehicleNoClipDetection(float.MaxValue);
			AntiHack.ShouldIgnore(player);

			player.fallDamageEffect = _emptyEffect;
			player.drownEffect = _emptyEffect;

			player._limitedNetworking = true;
			player.syncPosition = false;
			player.isInvisible = true;

			if (!BasePlayer.invisPlayers.Contains(player))
			{
				BasePlayer.invisPlayers.Add(player);
			}

			BaseEntity.Query.Server.RemovePlayer(player);
			player.DisablePlayerCollider();

			using var temp = Pool.Get<PooledList<Connection>>();
			temp.AddRange(Net.sv.connections.Where(connection => connection.connected && connection.isAuthenticated && connection.player is BasePlayer && connection.player != player));
			player.OnNetworkSubscribersLeave(temp);

			player.transform.localScale = Vector3.zero;

			SimpleAIMemory.AddIgnorePlayer(player);

			if (ConfigInstance.WhooshSoundOnVanish)
			{
				if (ConfigInstance.BroadcastVanishSounds)
				{
					Effect.server.Run(ConfigInstance.Effect.Vanishing, player.transform.position);
				}
				else
				{
					SendEffectTo(ConfigInstance.Effect.Vanishing, player);
				}
			}

			if (withUI) _drawUI(player);

			if (ConfigInstance.EnableLogs) Puts($"{player} just vanished at {player.transform.position}");

			if (ConfigInstance.ToggleNoclipOnVanish && toggleNoclip && player.net.connection.authLevel > 0 && !player.IsFlying)
			{
				player.SendConsoleCommand("noclip");
			}

			var vanishObject = new GameObject("Vanish Collider");
			vanishObject.transform.SetParent(player.transform, true);
			vanishObject.AddComponent<VanishedPlayer>().Init(player);

			// OnCarbonVanished
			Carbon.HookCaller.CallStaticHook(778631450, player);
		}
		else
		{
			player.transform.localScale = Vector3.one;

			player.ResetAntiHack(player.ActivePlayerInd, AntiHack.PlayerSpeedhackStates, AntiHack.PlayerFlyhackStates);
			player.syncPosition = true;
			player._limitedNetworking = false;
			player.isInvisible = false;
			BasePlayer.invisPlayers.Remove(player);

			BaseEntity.Query.Server.RemovePlayer(player);
			BaseEntity.Query.Server.AddPlayer(player);

			player.EnablePlayerCollider();
			player.SendNetworkUpdate();

			player.GetHeldEntity()?.SendNetworkUpdate();
			SimpleAIMemory.RemoveIgnorePlayer(player);

			player.drownEffect = _drownEffect;
			player.fallDamageEffect = _fallDamageEffect;

			player.ForceUpdateTriggers(enter: true, exit: false, invoke: true);

			if (ConfigInstance.GutshotScreamOnUnvanish)
			{
				if (ConfigInstance.BroadcastVanishSounds)
				{
					Effect.server.Run(ConfigInstance.Effect.Unvanishing, player.transform.position);
				}
				else
				{
					SendEffectTo(ConfigInstance.Effect.Unvanishing, player);
				}
			}

			using var cui = new CUI(Handler);
			cui.Destroy("vanishui", player);

			if (ConfigInstance.EnableLogs) Puts($"{player} unvanished at {player.transform.position}");

			if (ConfigInstance.ToggleNoclipOnUnvanish && toggleNoclip && player.net.connection.authLevel > 0 && player.IsFlying)
			{
				player.SendConsoleCommand("noclip");
			}

			var vanishMono = player.GetComponentInChildren<VanishedPlayer>();

			if(vanishMono != null)
			{
				GameObject.Destroy(vanishMono.gameObject);
			}

			// OnCarbonUnvanished
			Carbon.HookCaller.CallStaticHook(3385747762, player);
		}
	}

	private void Vanish(BasePlayer player, string cmd, string[] args)
	{
		DoVanish(player, !_vanishedPlayers.ContainsKey(player.userID));
	}

	private void _clearTriggers(BasePlayer player)
	{
		if (player.triggers != null && player.triggers.Count > 0)
		{
			foreach (var trigger in player.triggers)
			{
				trigger.OnEntityLeave(player);
			}
		}

		foreach (var heli in BaseEntity.serverEntities.OfType<PatrolHelicopter>())
		{
			if (heli.myAI == null || heli.myAI.strafe_target != player)
			{
				continue;
			}
			Logger.Warn($"Patrol Helicopter at {heli.transform.position} ended player strafe for '{player.Connection}'");

			heli.myAI.State_OrbitStrafe_Leave();
			heli.myAI.State_Strafe_Leave();
		}
	}

	private void _drawUI(BasePlayer player)
	{
		using var cui = new CUI(Handler);
		var container = cui.CreateContainer("vanishui", parent: CUI.ClientPanels.Under, destroyUi: "vanishui");
		if (!string.IsNullOrEmpty(ConfigInstance.InvisibleText))
		{
			var textX = ConfigInstance.InvisibleTextAnchorX;
			var textY = ConfigInstance.InvisibleTextAnchorY;
			cui.CreateText(container, "vanishui", color: ConfigInstance.InvisibleTextColor, ConfigInstance.InvisibleText, ConfigInstance.InvisibleTextSize,
				xMin: textX[0], xMax: textX[1], yMin: textY[0], yMax: textY[1], align: ConfigInstance.InvisibleTextAnchor);
		}

		if (!string.IsNullOrEmpty(ConfigInstance.InvisibleIconUrl))
		{
			var anchorMin = ConfigInstance.InvisibleIconMinAnchor;
			var anchorMax = ConfigInstance.InvisibleIconMaxAnchor;
			var offsetMin = ConfigInstance.InvisibleIconMinOffset;
			var offsetMax = ConfigInstance.InvisibleIconMaxOffset;
			cui.CreateClientImage(container, "vanishui", ConfigInstance.InvisibleIconUrl, ConfigInstance.InvisibleIconColor,
				xMin: anchorMin[0], xMax: anchorMax[0], yMin: anchorMin[1], yMax: anchorMax[1],
				OxMin: offsetMin[0], OxMax: offsetMax[0], OyMin: offsetMin[1], OyMax: offsetMax[1]);
		}

		cui.Send(container, player);
	}

	public class VanishedPlayer : FacepunchBehaviour
	{
		public BasePlayer player;

		private void Start()
		{
			InvokeRepeating(nameof(UpdateNetworkGroups), 1f, 5f);
		}

		private void UpdateNetworkGroups()
		{
			if (player == null || !player.IsConnected) return;
			player.net.UpdateGroups(player.transform.position, player.networkRange);
		}

		public void Init(BasePlayer player)
		{
			this.player = player;
			gameObject.layer = (int)Rust.Layer.Reserved1;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;

			var playerCollider = player.colliderValue.Get();
			var vanishCollider = gameObject.AddComponent<CapsuleCollider>();
			vanishCollider.center = playerCollider.center;
			vanishCollider.radius = playerCollider.radius;
			vanishCollider.height = playerCollider.height;
			vanishCollider.direction = playerCollider.direction;
			vanishCollider.isTrigger = true;

			var colliders = Pool.Get<List<Collider>>();
			Vis.Components(gameObject.transform.position, vanishCollider.radius, colliders);

			foreach(var collider in colliders)
			{
				OnTriggerEnter(collider);
			}

			Pool.FreeUnmanaged(ref colliders);
		}

		private void OnTriggerEnter(Collider collider)
		{
			var parent = collider.gameObject.GetComponent<TriggerParent>();

			if (parent == null)
			{
				return;
			}

			parent.OnEntityEnter(player);
		}

		private void OnTriggerExit(Collider collider)
		{
			var parent = collider.gameObject.GetComponent<TriggerParent>();

			if (parent == null)
			{
				return;
			}

			parent.OnEntityLeave(player);
		}
	}

	#region Patches

	[AutoPatch, HarmonyPatch(typeof(Item), nameof(Item.SetItemOwnership), typeof(BasePlayer), typeof(Translate.Phrase))]
	public static class OwnershipPatch
	{
		[UsedImplicitly]
		public static bool Prefix(BasePlayer player, Translate.Phrase reason)
		{
			return !Singleton.IsPlayerVanished(player);
		}
	}

	[AutoPatch, HarmonyPatch(typeof(StorageContainer), nameof(StorageContainer.CanBeLooted), typeof(BasePlayer))]
	public static class StorageContainerPatch
	{
		[UsedImplicitly]
		[HarmonyPrefix]
		public static bool Prefix(ref bool __result, BasePlayer player)
		{
			if (player != null &&
			    Singleton != null && Singleton.IsEnabled() &&
			    Singleton.IsPlayerVanished(player) &&
			    Singleton.Permissions.UserHasPermission(player.UserIDString, Singleton.ConfigInstance.VanishUnlockWhileVanishedPermission))
			{
				__result = true;
				return false;
			}

			return true;
		}
	}

	[AutoPatch, HarmonyPatch(typeof(BasePlayer), nameof(BasePlayer.Teleport), typeof(Vector3))]
	public static class TeleportPatch
	{
		[UsedImplicitly]
		[HarmonyPrefix]
		public static bool Prefix(BasePlayer __instance, Vector3 position)
		{
			if (Singleton == null || !Singleton.IsEnabled() || !Singleton.IsPlayerVanished(__instance)) return true;

			__instance.MovePosition(position, false);
			__instance.ClientRPC(RpcTarget.Player("ForcePositionTo", __instance), position);
			return false;
		}
	}

	[AutoPatch, HarmonyPatch(typeof(BaseEntity), "SignalBroadcast", typeof(BaseEntity.Signal), typeof(string), typeof(Connection), typeof(string), typeof(float))]
	public static class SignalBroadcastPatch
	{
		[UsedImplicitly]
		[HarmonyPrefix]
		public static bool Prefix(Connection sourceConnection)
		{
			if (sourceConnection == null) return true;
			return Singleton == null || !Singleton.IsEnabled() || !Singleton.IsPlayerVanished(sourceConnection.userid);
		}
	}

	[AutoPatch, HarmonyPatch(typeof(EffectNetwork), "Send", typeof(Effect), typeof(EntityNetworkRange))]
	public static class EffectNetworkPatch
	{
		[UsedImplicitly]
		[HarmonyPrefix]
		public static bool Prefix(Effect effect, EntityNetworkRange networkRange)
		{
			if (effect == null || effect.source == 0) return true;
			return Singleton == null || !Singleton.IsEnabled() || !Singleton.IsPlayerVanished(effect.source);
		}
	}

	[AutoPatch, HarmonyPatch(typeof(StorageContainer), nameof(StorageContainer.ShouldRequireAuthIfNoCodelock))]
	private static class StorageContainerPatch2
	{
		[UsedImplicitly]
		[HarmonyPrefix]
		private static bool Prefix(ref bool __result, BaseEntity container)
		{
			if (container is not ContainerIOEntity)
				return true;

			if (Singleton == null || !Singleton.IsEnabled())
				return true;

			var player = Singleton._lastLooter;
			Singleton._lastLooter = null;

			if (player != null &&
			    Singleton.IsPlayerVanished(player) &&
			    Singleton.Permissions.UserHasPermission(player.UserIDString, Singleton.ConfigInstance.VanishUnlockWhileVanishedPermission))
			{
				__result = false;
				return false;
			}

			return true;
		}
	}

	#endregion
}

public class VanishConfig
{
	[JsonProperty("[Anchor] Legend")]
	public string AnchorLegend => "(0=UpperLeft, 1=UpperCenter, 2=UpperRight, 3=MiddleLeft, 4=MiddleCenter, 5=MiddleRight, 6=LowerLeft, 7=LowerCenter, 8=LowerRight)";

	public string VanishPermission = "vanish.allow";
	public string VanishUnlockWhileVanishedPermission = "vanish.unlock";
	public string PermanentVanishPermission = "vanish.permanent";
	public string VanishCommand = "vanish";
	public bool ToggleNoclipOnVanish = true;
	public bool ToggleNoclipOnUnvanish = false;

	public string InvisibleText = "You are currently invisible.";
	public int InvisibleTextSize = 10;
	public string InvisibleTextColor = "#8bba49";
	[JsonProperty("InvisibleTextAnchor [Anchor]")]
	public TextAnchor InvisibleTextAnchor = TextAnchor.LowerCenter;
	public float[] InvisibleTextAnchorX = [0, 1];
	public float[] InvisibleTextAnchorY = [0, 0.025f];

	public string InvisibleIconUrl = "";
	public string InvisibleIconColor = "1 1 1 0.3";
	public float[] InvisibleIconMinAnchor = [0.5f, 0];
	public float[] InvisibleIconMaxAnchor = [0.5f, 0];
	public float[] InvisibleIconMinOffset = [-350f, 15f];
	public float[] InvisibleIconMaxOffset = [-250f, 125];

	public EffectConfig Effect = new();

	public bool BroadcastVanishSounds = false;
	public bool WhooshSoundOnVanish = true;
	public bool GutshotScreamOnUnvanish = true;
	public bool EnableLogs = true;
	public bool TeleportBackOnUnvanish = false;
	public bool CanDamageWhenVanished = true;

	public class EffectConfig
	{
		public string Vanishing = "assets/prefabs/npc/patrol helicopter/effects/rocket_fire.prefab";
		public string Unvanishing = "assets/bundled/prefabs/fx/player/gutshot_scream.prefab";
	}
}
