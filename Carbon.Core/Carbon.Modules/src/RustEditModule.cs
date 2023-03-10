using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Xml.Serialization;
using Carbon.Base;
using ConVar;
using Facepunch;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static BasePlayer;
using static Carbon.Modules.RustEditModule;
using static QRCoder.PayloadGenerator;
using static UnityEngine.UI.GridLayoutGroup;
using Color = UnityEngine.Color;
using Pool = Facepunch.Pool;
using Random = UnityEngine.Random;
using Time = UnityEngine.Time;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 bmgjet, bg
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public class RustEditModule : CarbonModule<RustEditConfig, RustEditData>
{
	internal static RustEditModule Singleton { get; internal set; }

	public override string Name => "RustEdit.Ext";
	public override Type Type => typeof(RustEditModule);

	public RustEditModule()
	{
		Singleton = this;
	}

	private void OnServerInitialized()
	{
		if (!ConfigInstance.Enabled) return;

		#region Spawnpoints
		try
		{
			if (Spawn_Spawnpoints != null && Spawn_Spawnpoints.Count != 0)
			{
				PutsWarn($"Found {Spawn_Spawnpoints.Count:n0} spawn-points.");
			}
		}
		catch (Exception ex) { PutsError($"Failed to install Spawnpoints.", ex); }
		#endregion

		#region NPC Spawner
		try
		{
			if (NPCSpawner_serializedNPCData == null)
			{
				//Load XML from map
				var data = NPCSpawner_GetData("SerializedNPCData");

				if (data != null) { NPCSpawner_serializedNPCData = NPCSpawner_Deserialize<SerializedNPCData>(data); }
			}
			if (NPCSpawner_serializedNPCData != null)
			{
				var npcSpawners = NPCSpawner_serializedNPCData.npcSpawners;
				if (npcSpawners != null && npcSpawners.Count != 0)
				{
					Puts($"Loaded {NPCSpawner_serializedNPCData.npcSpawners.Count:n0} NPC Spawners.");
					for (int i = 0; i < npcSpawners.Count; i++)
					{
						//Setup spawners
						var npcspawner = new GameObject("NPCSpawner").AddComponent<NPCSpawner>();
						npcspawner.Initialize(NPCSpawner_serializedNPCData.npcSpawners[i]);
						NPCSpawner_NPCsList.Add(npcspawner);
					}
				}
			}
			if (NPCSpawner_NPCThread == null)
			{
				//Start NPC management thread.
				NPCSpawner_NPCThread = ServerMgr.Instance.StartCoroutine(NPCSpawner_NPCAIFunction());
			}
		}
		catch (Exception ex) { PutsError($"Failed to install NPC Spawner.", ex); }
		#endregion

		#region APC
		try
		{
			if (APC_serializedAPCPathList == null) { APC_ReadMapData(); }
			if (APC_serializedAPCPathList != null) { ServerMgr.Instance.StartCoroutine(APC_Setup()); }
		}
		catch (Exception ex) { PutsError($"Failed to install APC.", ex); }
		#endregion

		#region Cargo Path
		try
		{
			if (Cargo_CargoSpawn.Count != 0 || Cargo_CargoStops.Count != 0)
			{
				Singleton.PutsWarn($"Found {Cargo_CargoSpawn.Count:n0} spawnpoints and {Cargo_CargoStops.Count:n0} stop points.");
			}
			foreach (var ship in BaseNetworkable.serverEntities.OfType<CargoShip>())
			{
				if (ship.gameObject.HasComponent<CargoMod>()) continue;

				var newcargoship = ship.gameObject.AddComponent<CargoMod>();
				newcargoship._cargoship = ship;
				newcargoship.CargoThread = ServerMgr.Instance.StartCoroutine(newcargoship.CargoTick());
				Cargo_CargoShips.Add(newcargoship);
			}
		}
		catch (Exception ex) { PutsError($"Failed to install Cargo Path.", ex); }
		#endregion

		#region I/O
		try
		{
			IO_RunCore_IO();
		}
		catch (Exception ex) { PutsError($"Failed to install IO.", ex); }
		#endregion

		#region Deployables

		Deployables_InitializeHook();

		#endregion
	}

	#region Hooks

	private object CanBradleyApcTarget(BradleyAPC apc, BaseEntity entity)
	{
		if (entity is NPCPlayer && Config.NpcSpawner.APCIgnoreNPCs)
		{
			foreach (var npcspawner in NPCSpawner_NPCsList)
			{
				if (npcspawner == null || npcspawner.BOT == null) continue;
				if (npcspawner.BOT == entity) return false;
			}
		}

		return null;
	}
	private object OnTurretTarget(AutoTurret turret, BaseCombatEntity entity)
	{
		if (turret.OwnerID == 0 && Config.NpcSpawner.NPCAutoTurretsIgnoreNPCs)
		{
			foreach (var npcspawner in NPCSpawner_NPCsList)
			{
				if (npcspawner == null || npcspawner.BOT == null)
				{
					continue;
				}

				if (npcspawner.BOT == entity)
				{
					return false;
				}
			}
		}

		return null;
	}
	private object OnBasePlayerAttacked(BasePlayer player, HitInfo info)
	{
		if (!Config.NpcSpawner.UseRustEditNPCLogic)
		{
			// BMG Pirate Logic
			if (info.InitiatorPlayer != null)
			{
				// Has Been Attacked by player
				foreach (var nPCSpawner in NPCSpawner_NPCsList)
				{
					// Find if is a bot
					if (nPCSpawner.BOT.ToPlayer() == player)
					{
						var attackingplayer = info.InitiatorPlayer;

						// Get attacking player
						if (attackingplayer == null)
						{
							return true;
						}
						if (activePlayerList.Contains(attackingplayer))
						{
							// Attacking player is human player
							// If scarecrow is damage go to where attacker was
							if (player is ScarecrowNPC scarecrow)
							{
								scarecrow.Brain.Navigator.SetDestination(attackingplayer.transform.position, BaseNavigator.NavigationSpeed.Fast);
								return true;
							}

							// If ScientistNPC then fire back on the place player was
							if (player is ScientistNPC npc && npc.Brain != null)
							{
								// Adjust targets and settings temp
								npc.Brain.Senses.Memory.Targets.Clear();
								npc.Brain.Senses.Memory.Players.Clear();
								npc.Brain.Senses.Memory.Threats.Clear();
								npc.Brain.Senses.Memory.Targets.Add(attackingplayer);
								npc.Brain.Senses.Memory.Players.Add(attackingplayer);
								npc.Brain.Senses.Memory.Threats.Add(attackingplayer);

								if (npc.Brain.AttackRangeMultiplier != 10)
								{
									var oldTLR = npc.Brain.TargetLostRange;
									var oldARM = npc.Brain.AttackRangeMultiplier;
									var oldAIM = npc.aimConeScale;
									npc.aimConeScale = 0.5f;
									npc.Brain.TargetLostRange = 800;
									npc.Brain.AttackRangeMultiplier = 10;
									nPCSpawner.cooldown = true;
									npc.Brain.Events.Memory.Entity.Set(attackingplayer, 0);

									npc.Invoke(() =>
									{
										if (nPCSpawner != null) nPCSpawner.cooldown = false;

										// Restore default settings and remove targets
										if (npc != null)
										{
											npc.aimConeScale = oldAIM;
											npc.Brain.TargetLostRange = oldTLR;
											npc.Brain.AttackRangeMultiplier = oldARM;
											npc.Brain.Senses.Memory.Targets.Clear();
											npc.Brain.Senses.Memory.Players.Clear();
											npc.Brain.Senses.Memory.Threats.Clear();
										}
									}, 8f);
								}
							}
						}

						return true;
					}
				}
			}
		}

		return null;
	}
	private object OnPlayerRespawn(BasePlayer player, BasePlayer.SpawnPoint point)
	{
		return Spawn_RespawnPlayer();
	}
	private object OnEntityTakeDamage(BaseCombatEntity entity)
	{
		if (entity == null)
		{
			return null;
		}

		#region I/O

		if (IO_Protect.Contains(entity.transform.position))
		{
			return false;
		}

		#endregion

		#region Deployables

		if (!Deployables_ProtectedHook(entity))
		{
			return false;
		}

		#endregion

		return null;
	}
	private object CanLootEntity(BasePlayer player, StorageContainer container)
	{
		if (player == null || container == null || (player.IsAdmin && player.IsGod() && player.IsFlying))
		{
			return true;
		}

		if (IO_Protect.Contains(container.transform.position))
		{
			return false;
		}

		return null;
	}
	private object CanLootEntity(BasePlayer player, ContainerIOEntity container)
	{
		if (player == null || container == null || (player.IsAdmin && player.IsGod() && player.IsFlying))
		{
			return true;
		}

		if (IO_Protect.Contains(container.transform.position))
		{
			return false;
		}

		return null;
	}
	private object OnTurretAuthorize(AutoTurret turret, BasePlayer player)
	{
		if (player == null || turret == null || (player.IsAdmin && player.IsGod() && player.IsFlying))
		{
			return null;
		}
		if (IO_AutoTurrets.ContainsKey(turret))
		{
			return false;
		}

		return null;
	}
	private object CanPickupEntity(BasePlayer player, BaseCombatEntity entity)
	{
		if (entity == null)
		{
			return null;
		}

		#region I/O

		if (IO_Protect.Contains(entity.transform.position))
		{
			return false;
		}

		#endregion

		#region Deployables

		try
		{ if (player.IsAdmin && player.IsGod() && player.IsFlying)
			{
				return null;
			}

			if (!Deployables_ProtectedHook(entity))
			{
				return false;
			}
		} catch { }

		#endregion

		return null;
	}
	private object OnEntityKill(BaseNetworkable networkable)
	{
		if (networkable == null)
		{
			return null;
		}

		#region I/O

		if (IO_Protect.Contains(networkable.transform.position))
		{
			return false;
		}

		#endregion

		#region Deployables

		if (!Deployables_KillHook(networkable))
		{
			return false;
		}

		#endregion

		return null;
	}
	private void OnEntitySpawned(LootContainer container)
	{
		Deployables_EntitySpawned(container);
	}

	#endregion

	#region Custom Hooks

	private object ICanWorldPrefabSpawn(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		#region IO
		try
		{
			if (!IO_WorldSpawnHook(null, prefab, position, rotation, scale))
			{
				return false;
			}
		}
		catch (Exception ex) { PutsError($"Failed IOnWorldPrefabSpawn for IO.", ex); }
		#endregion

		#region Cargo Paths
		try
		{
			switch (prefab.ID)
			{
				case 2741054453:
				case 843218194:
					Cargo_CargoSpawn.Add(position);
					return false;
			}
		}
		catch (Exception ex) { PutsError($"Failed IOnWorldPrefabSpawn for Cargo Path.", ex); }
		#endregion

		#region Spawn
		try
		{
			if (prefab.Name.Equals(Spawn_SpawnpointPrefab))
			{
				Spawn_Spawnpoints.Add(position);
				return null;
			}
		}
		catch (Exception ex) { PutsError($"Failed IOnWorldPrefabSpawn for Spawn.", ex); }
		#endregion

		return null;
	}
	private object ICanWorldPrefabSpawnData(PrefabData data)
	{
		#region Deployables

		if (!Deployables_SpawnHook(data))
		{
			return false;
		}

		#endregion

		return null;
	}

	#endregion

	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		Puts($"Subscribing to hooks");
		Subscribe("CanBradleyApcTarget");
		Subscribe("OnTurretTarget");
		Subscribe("OnBasePlayerAttacked");
		Subscribe("OnPlayerRespawn");
		Subscribe("OnWorldPrefabSpawn");
		Subscribe("OnEntitySpawned");
		Subscribe("IOnCargoShipEgressStart");
		Subscribe("ICanCargoShipBlockWaterFor");
		Subscribe("ICanWorldPrefabSpawn");
		Subscribe("ICanCargoShipBlockWaterFor");
		Subscribe("ICanGenerateOceanPatrolPath");
		Subscribe("IOnGenerateOceanPatrolPath");
		Subscribe("IOnPostGenerateOceanPatrolPath");
		Subscribe("IPostSaveLoad");
		Subscribe("IPostSaveSave");
		Subscribe("ICanWireToolModifyEntity");
		Subscribe("IPreTurretTargetTick");
		Subscribe("ICanDie");
		Subscribe("ICanCH47CreateMapMarker");
		Subscribe("IPreObjectSetHierarchyGroup");
		Subscribe("ICanDoorManipulatorDoAction");
		Subscribe("OnEntityTakeDamage");
		Subscribe("CanLootEntity");
	}
	public override void OnDisabled(bool initialized)
	{
		base.OnDisabled(initialized);

		Puts($"Unsubscribing hooks");
		Unsubscribe("CanBradleyApcTarget");
		Unsubscribe("OnTurretTarget");
		Unsubscribe("OnBasePlayerAttacked");
		Unsubscribe("OnPlayerRespawn");
		Unsubscribe("OnWorldPrefabSpawn");
		Unsubscribe("OnEntitySpawned");
		Unsubscribe("IOnCargoShipEgressStart");
		Unsubscribe("ICanCargoShipBlockWaterFor");
		Unsubscribe("ICanWorldPrefabSpawn");
		Unsubscribe("ICanCargoShipBlockWaterFor");
		Unsubscribe("ICanGenerateOceanPatrolPath");
		Unsubscribe("IOnGenerateOceanPatrolPath");
		Unsubscribe("IOnPostGenerateOceanPatrolPath");
		Unsubscribe("IPostSaveLoad");
		Unsubscribe("IPostSaveSave");
		Unsubscribe("ICanWireToolModifyEntity");
		Unsubscribe("IPreTurretTargetTick");
		Unsubscribe("ICanDie");
		Unsubscribe("ICanCH47CreateMapMarker");
		Unsubscribe("IPreObjectSetHierarchyGroup");
		Unsubscribe("ICanDoorManipulatorDoAction");
		Unsubscribe("OnEntityTakeDamage");
		Unsubscribe("CanLootEntity");
	}

	#region Common

	public static T DeserializeMapData<T>(byte[] bytes, out bool flag)
	{
		T result;

		try
		{
			using MemoryStream memoryStream = new MemoryStream(bytes);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			T t = (T)((object)xmlSerializer.Deserialize(memoryStream));
			flag = true;
			result = t;
		}
		catch (Exception ex)
		{
			Singleton.PutsError("Failed DeserializeMapData", ex);
			flag = false;
			result = default(T);
		}

		return result;
	}
	public static byte[] SerializeMapData<T>(T stream)
	{
		if (stream == null)
		{
			return null;
		}

		byte[] result;

		try
		{
			using MemoryStream memoryStream = new MemoryStream();
			new XmlSerializer(typeof(T)).Serialize(memoryStream, stream);
			result = memoryStream.ToArray();
		}
		catch (Exception ex)
		{
			Singleton.PutsError("Failed DeserializeMapData", ex);
			result = null;
		}

		return result;
	}

	#endregion

	#region Spawn

	internal List<Vector3> Spawn_Spawnpoints = new();
	internal const string Spawn_SpawnpointPrefab = "assets/bundled/prefabs/modding/volumes_and_triggers/spawn_point.prefab";

	[ChatCommand("showspawnpoints")]
	public void DoBroadcastSpawnpoints(BasePlayer player)
	{
		if (!ConfigInstance.Enabled) return;

		Spawn_BroadcastSpawnpoints(player);
	}

	public SpawnPoint Spawn_RespawnPlayer()
	{
		if (Spawn_Spawnpoints == null || Spawn_Spawnpoints.Count == 0) { return null; }

		var spawnpoint = Spawn_Spawnpoints.GetRandom();
		var height = TerrainMeta.HeightMap.GetHeight(spawnpoint);

		if (spawnpoint.y <= height && AntiHack.TestInsideTerrain(spawnpoint)) { spawnpoint.y = height + 0.1f; }

		return new SpawnPoint { pos = spawnpoint, rot = default };
	}
	public void Spawn_BroadcastSpawnpoints(BasePlayer player)
	{
		if (!player.IsAdmin) return;

		player.ChatMessage("Showing spawn points");

		foreach (var spawnpoint in Spawn_Spawnpoints)
		{
			player.SendConsoleCommand("ddraw.sphere", 8f, Color.blue, spawnpoint, 1f);
		}
	}

	#endregion

	#region NPC Spawner

	internal SerializedNPCData NPCSpawner_serializedNPCData;
	internal List<NPCSpawner> NPCSpawner_NPCsList = new();
	internal Coroutine NPCSpawner_NPCThread;

	public enum NPCType
	{
		Scientist,
		Peacekeeper,
		HeavyScientist,
		JunkpileScientist,
		Bandit,
		Murderer,
		Scarecrow
	}

	public class SerializedNPCData { public List<SerializedNPCSpawner> npcSpawners = new(); }

	public class SerializedNPCSpawner
	{
		public int npcType;
		public int respawnMin;
		public int respawnMax;
		public VectorData position;
		public string category;
	}

	public class NPCSpawner : MonoBehaviour
	{
		public SerializedNPCSpawner serializedNPCSpawner;
		public BaseEntity BOT;
		public BaseEntity target = null;
		public bool Respawning = true;
		public float Delay = 0;
		public float RoamDistance = 30f;
		public NPCType type;
		public string prefab;
		public bool cooldown = false;
		public Vector3 OldPos;
		public bool stationary = false;
		public static Dictionary<NPCType, string> typeToPrefab = new()
		{
			{ NPCType.Scientist,"assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_roamtethered.prefab" },
			{ NPCType.Peacekeeper,"assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_peacekeeper.prefab" },
			{ NPCType.HeavyScientist,"assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_heavy.prefab" },
			{ NPCType.JunkpileScientist,"assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_junkpile_pistol.prefab" },
			{ NPCType.Bandit,"assets/rust.ai/agents/npcplayer/humannpc/banditguard/npc_bandit_guard.prefab" },
			{ NPCType.Murderer,"assets/prefabs/npc/scarecrow/scarecrow.prefab" },
			{ NPCType.Scarecrow,"assets/prefabs/npc/scarecrow/scarecrow.prefab" },
		};

		public void Initialize(SerializedNPCSpawner serializedNPCSpawner0)
		{
			serializedNPCSpawner = serializedNPCSpawner0;
			type = (NPCType)serializedNPCSpawner.npcType;
			prefab = typeToPrefab[type];
			OldPos = serializedNPCSpawner.position;
		}

		public void SpawnNPC()
		{
			var module = GetModule<RustEditModule>();
			var settings = module.Config.NpcSpawner;

			var position = (Vector3)serializedNPCSpawner.position;
			var num = 1;
			var npc = (ScientistNPC)null;
			var npc2 = (ScarecrowNPC)null;
			stationary = !module.NPCSpawner_IsOnGround(ref position, out num);

			if (settings.UseRustEditNPCLogic || type == NPCType.Scarecrow || type == NPCType.Murderer)
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(prefab, position, Quaternion.identity, false);
				if (baseEntity != null)
				{
					baseEntity.enableSaving = false;
					baseEntity.gameObject.AwakeFromInstantiate();
					baseEntity.EnableSaving(false);
					baseEntity.Spawn();
					Respawning = false;
					BOT = baseEntity;
					npc = baseEntity.GetComponent<ScientistNPC>();
					npc2 = baseEntity.GetComponent<ScarecrowNPC>();

					try
					{
						var navagent = baseEntity.GetComponent<NavMeshAgent>();
						if (navagent != null)
						{
							navagent.areaMask = num;
							navagent.agentTypeID = ((num == 25 || num == 8) ? 0 : -1372625422);
							if (stationary)
							{
								navagent.isStopped = true;
								navagent.enabled = false;
							}
						}
					}
					catch { }
				}
			}
			else
			{
				prefab = StringPool.Get(3763080634);
				var prefabvar = GameManager.server.FindPrefab(prefab);
				var go = Facepunch.Instantiate.GameObject(prefabvar);
				go.SetActive(false);
				go.name = prefab;
				go.transform.position = position;
				npc = go.GetComponent<ScientistNPC>();

				SceneManager.MoveGameObjectToScene(go, Rust.Server.EntityScene);
				if (npc != null)
				{
					npc.userID = (ulong)UnityEngine.Random.Range(0, 10000000);
					npc.UserIDString = npc.userID.ToString();
					npc.displayName = "[NPC]" + RandomUsernames.Get(npc.userID);
					npc.startHealth = 120;
					go.SetActive(true);
					npc.EnableSaving(false);
					npc.Spawn();
					Respawning = false;
					BOT = npc;

					try
					{
						var navagent = npc.GetComponent<NavMeshAgent>();
						if (navagent != null)
						{
							navagent.areaMask = num;
							navagent.agentTypeID = ((num == 25 || num == 8) ? 0 : -1372625422);
							if (stationary)
							{
								navagent.isStopped = true;
								navagent.enabled = false;
							}
						}
					}
					catch { }
				}
			}
			var player = (BasePlayer)null;

			if (npc != null) player = npc; else if (npc2 != null) player = npc2;

			switch (type)
			{
				case NPCType.Scientist:
					npc.InitializeHealth(module.Config.NpcSpawner.ScientistNPCHealth, settings.ScientistNPCHealth);
					if (npc != null)
					{
						RoamDistance = settings.ScientistNPCRoamRange;
						npc.aimConeScale = settings.ScientistNPCAimMultiplyer;
						npc.damageScale = settings.ScientistNPCDamageScale;
						npc.Brain.CheckLOS = settings.ScientistNPCCheckLOS;
						npc.Brain.IgnoreSafeZonePlayers = settings.ScientistNPCIgnoreSafeZonePlayers;
						npc.Brain.SenseRange = settings.ScientistNPCSenseRange;
						npc.Brain.MemoryDuration = settings.ScientistNPCMemoryDuration; ;
						npc.Brain.TargetLostRange = settings.ScientistNPCTargetLostRange;
						npc.Brain.AttackRangeMultiplier = settings.ScientistNPCAttackRangeMultiplier;
						if (settings.UseRustEditNPCLogic) return;
						npc.inventory.Strip();
						module.NPCSpawner_DressNPC(npc, type);
					}
					break;
				case NPCType.Peacekeeper:
					player.InitializeHealth(settings.PeacekeeperNPCHealth, settings.PeacekeeperNPCHealth);
					if (npc != null)
					{
						RoamDistance = settings.PeacekeeperNPCRoamRange;
						npc.aimConeScale = settings.PeacekeeperNPCAimMultiplyer;
						npc.damageScale = settings.PeacekeeperNPCDamageScale;
						npc.Brain.CheckLOS = settings.PeacekeeperNPCCheckLOS;
						npc.Brain.IgnoreSafeZonePlayers = settings.PeacekeeperNPCIgnoreSafeZonePlayers;
						npc.Brain.SenseRange = settings.PeacekeeperNPCSenseRange;
						npc.Brain.MemoryDuration = settings.PeacekeeperNPCMemoryDuration; ;
						npc.Brain.TargetLostRange = settings.PeacekeeperNPCTargetLostRange;
						npc.Brain.AttackRangeMultiplier = settings.PeacekeeperNPCAttackRangeMultiplier;
						if (settings.UseRustEditNPCLogic) return;
						npc.inventory.Strip();
						module.NPCSpawner_DressNPC(npc, type);
					}
					break;
				case NPCType.HeavyScientist:
					player.InitializeHealth(settings.HeavyScientistNPCHealth, settings.HeavyScientistNPCHealth);
					if (npc != null)
					{
						RoamDistance = settings.HeavyScientistNPCRoamRange;
						npc.aimConeScale = settings.HeavyScientistNPCAimMultiplyer;
						npc.damageScale = settings.HeavyScientistNPCDamageScale;
						npc.Brain.CheckLOS = settings.HeavyScientistNPCCheckLOS;
						npc.Brain.IgnoreSafeZonePlayers = settings.HeavyScientistNPCIgnoreSafeZonePlayers;
						npc.Brain.SenseRange = settings.HeavyScientistNPCSenseRange;
						npc.Brain.MemoryDuration = settings.HeavyScientistNPCMemoryDuration; ;
						npc.Brain.TargetLostRange = settings.HeavyScientistNPCTargetLostRange;
						npc.Brain.AttackRangeMultiplier = settings.HeavyScientistNPCAttackRangeMultiplier;
						if (settings.UseRustEditNPCLogic) return;
						npc.inventory.Strip();
						module.NPCSpawner_DressNPC(npc, type);

					}
					break;
				case NPCType.JunkpileScientist:
					player.InitializeHealth(settings.JunkpileScientistNPCHealth, settings.JunkpileScientistNPCHealth);
					if (npc != null)
					{
						RoamDistance = settings.JunkpileScientistNPCRoamRange;
						npc.aimConeScale = settings.JunkpileScientistNPCAimMultiplyer;
						npc.damageScale = settings.JunkpileScientistNPCDamageScale;
						npc.Brain.CheckLOS = settings.JunkpileScientistNPCCheckLOS;
						npc.Brain.IgnoreSafeZonePlayers = settings.JunkpileScientistNPCIgnoreSafeZonePlayers;
						npc.Brain.SenseRange = settings.JunkpileScientistNPCSenseRange;
						npc.Brain.MemoryDuration = settings.JunkpileScientistNPCMemoryDuration; ;
						npc.Brain.TargetLostRange = settings.JunkpileScientistNPCTargetLostRange;
						npc.Brain.AttackRangeMultiplier = settings.JunkpileScientistNPCAttackRangeMultiplier;
						if (settings.UseRustEditNPCLogic) return;
						npc.inventory.Strip();
						module.NPCSpawner_DressNPC(npc, type);
					}
					break;
				case NPCType.Bandit:
					player.InitializeHealth(settings.BanditNPCHealth, settings.BanditNPCHealth);
					if (npc != null)
					{
						RoamDistance = settings.BanditNPCRoamRange;
						npc.aimConeScale = settings.BanditNPCAimMultiplyer;
						npc.damageScale = settings.BanditNPCDamageScale;
						npc.Brain.CheckLOS = settings.BanditNPCCheckLOS;
						npc.Brain.IgnoreSafeZonePlayers = settings.BanditNPCIgnoreSafeZonePlayers;
						npc.Brain.SenseRange = settings.BanditNPCSenseRange;
						npc.Brain.MemoryDuration = settings.BanditNPCMemoryDuration; ;
						npc.Brain.TargetLostRange = settings.BanditNPCTargetLostRange;
						npc.Brain.AttackRangeMultiplier = settings.BanditNPCAttackRangeMultiplier;

						if (settings.UseRustEditNPCLogic) return;

						npc.inventory.Strip();
						module.NPCSpawner_DressNPC(npc, type);
					}
					break;
				case NPCType.Murderer:
					player.InitializeHealth(settings.MurdererNPCHealth, settings.MurdererNPCHealth);
					if (npc2 != null)
					{
						RoamDistance = settings.MurdererNPCRoamRange;
						npc2.damageScale = settings.MurdererNPCDamageScale;
						npc2.Brain.CheckLOS = settings.MurdererNPCCheckLOS;
						npc2.Brain.IgnoreSafeZonePlayers = settings.MurdererNPCIgnoreSafeZonePlayers;
						npc2.Brain.SenseRange = settings.MurdererNPCSenseRange;
						npc2.Brain.MemoryDuration = settings.MurdererNPCMemoryDuration;
						npc2.Brain.TargetLostRange = settings.MurdererNPCTargetLostRange;
						npc2.Brain.AttackRangeMultiplier = settings.MurdererNPCAttackRangeMultiplier;
						npc2.inventory.Strip();
						module.NPCSpawner_DressNPC(npc2, type);
					}
					break;
				case NPCType.Scarecrow:
					player.InitializeHealth(settings.ScarecrowNPCHealth, settings.ScarecrowNPCHealth);
					if (npc2 != null)
					{
						RoamDistance = settings.ScarecrowNPCRoamRange;
						npc2.damageScale = settings.ScarecrowNPCDamageScale;
						npc2.Brain.CheckLOS = settings.ScarecrowNPCCheckLOS;
						npc2.Brain.IgnoreSafeZonePlayers = settings.ScarecrowNPCIgnoreSafeZonePlayers;
						npc2.Brain.SenseRange = settings.ScarecrowNPCSenseRange;
						npc2.Brain.MemoryDuration = settings.ScarecrowNPCMemoryDuration;
						npc2.Brain.TargetLostRange = settings.ScarecrowNPCTargetLostRange;
						npc2.Brain.AttackRangeMultiplier = settings.ScarecrowNPCAttackRangeMultiplier;
						npc2.inventory.Strip();
						module.NPCSpawner_DressNPC(npc2, type);
					}
					break;
			}
		}
	}

	private System.Collections.IEnumerator NPCSpawner_NPCAIFunction()
	{
		yield return CoroutineEx.waitForSeconds(5);

		var checks = 0;
		var _instruction = ConVar.FPS.limit > 30 ? CoroutineEx.waitForSeconds(0.01f) : CoroutineEx.waitForSeconds(0.001f);
		Puts($"AI thread running on {NPCSpawner_NPCsList.Count:n0} NPCs");

		while (NPCSpawner_NPCThread != null)
		{
			foreach (NPCSpawner npcs in NPCSpawner_NPCsList)
			{
				if (++checks >= 50)
				{
					checks = 0;
					yield return _instruction;
				}

				try
				{
					if (npcs == null) { continue; }
					//Respawn Logic
					if (npcs.Respawning)
					{
						if (npcs.Delay <= Time.time)
						{
							if (!NPCSpawner_AnyPlayersNearby(npcs.serializedNPCSpawner.position, Config.NpcSpawner.SpawnBlockingDistance)) { npcs.SpawnNPC(); }
						}
						continue;
					}
					if ((npcs.BOT == null || npcs.BOT.IsDestroyed) && !npcs.Respawning)
					{
						npcs.Respawning = true;
						npcs.Delay = Time.time + (float)UnityEngine.Random.Range(npcs.serializedNPCSpawner.respawnMin, npcs.serializedNPCSpawner.respawnMax);
						continue;
					}
					if (Config.NpcSpawner.UseRustEditNPCLogic || npcs.type == NPCType.Scarecrow || npcs.type == NPCType.Murderer)
					{
						//Rust Edit AI Logic and Scarecrow/murderer Fix
						ScarecrowNPC scnpc = npcs.BOT as ScarecrowNPC;
						if (scnpc != null)
						{
							if (npcs.target == null)
							{
								try { npcs.target = scnpc.Brain.Senses.GetNearestTarget(scnpc.Brain.SenseRange); }
								catch { npcs.target = null; }
							}
							if (npcs.target != null)
							{
								//If target out of range then clear target.
								if (Vector3.Distance(npcs.target.transform.position, scnpc.transform.position) >= scnpc.Brain.SenseRange)
								{
									scnpc.Brain.Events.Memory.Entity.Set(scnpc, 0);
									scnpc.Brain.Navigator.SetDestination(npcs.serializedNPCSpawner.position, BaseNavigator.NavigationSpeed.Normal);
									npcs.target = null;
								}
								else
								{
									scnpc.Brain.Events.Memory.Entity.Set(npcs.target, 0);
									float dist;
									if (scnpc.IsTargetInRange(npcs.target, out dist))
									{
										BaseMelee weapon = scnpc?.GetHeldEntity() as BaseMelee;
										if (weapon == null) { continue; }
										//Do melee slash if in its reach
										if (!weapon.HasAttackCooldown() && Vector3.Distance(npcs.target.transform.position, scnpc.transform.position) <= weapon.maxDistance)
										{
											//melee hit
											if (scnpc.MeleeAttack())
											{
												npcs.target.ToPlayer().Hurt(weapon.TotalDamage() * scnpc.damageScale, DamageType.Slash, scnpc);
												weapon.StartAttackCooldown(weapon.attackSpacing);
											}
										}
									}
								}
							}
							else
							{
								//No target so do random movement
								NPCSpawner_RandomMovement(npcs);
								npcs.target = null;
							}
						}
					}
					else
					{
						//bmgjet pirates logic
						ScientistNPC scinpc = npcs.BOT as ScientistNPC;
						if (scinpc == null) { continue; }
						try { if (scinpc.GetGun().primaryMagazine.contents < 1) { scinpc.AttemptReload(); } } catch { }
						if (npcs.target == null)
						{
							//Find targets in range if has no target
							try { npcs.target = scinpc.Brain.Senses.GetNearestTarget(scinpc.Brain.SenseRange); }
							catch { npcs.target = null; }
						}
						if (npcs.target != null)
						{
							//If taget is beyound sensor range return home and clear memory
							if (Vector3.Distance(npcs.target.transform.position, npcs.BOT.transform.position) > scinpc.Brain.SenseRange)
							{
								if (!npcs.cooldown)
								{
									npcs.target = null;
									scinpc.Brain.Events.Memory.Entity.Set(scinpc, 0);
								}
								if (!scinpc.Brain.Navigator.Moving)
								{
									scinpc.Brain.Navigator.SetDestination(npcs.serializedNPCSpawner.position, BaseNavigator.NavigationSpeed.Fast);
								}
							}
							else
							{
								//Chase target
								if (!npcs.cooldown)
								{
									scinpc.Brain.Events.Memory.Entity.Set(npcs.target, 0);
								}
								if (!scinpc.Brain.Navigator.Moving)
								{
									scinpc.Brain.Navigator.SetDestination(npcs.target.transform.position, BaseNavigator.NavigationSpeed.Normal);
								}
							}
						}
						else
						{
							//No target so do random movement
							NPCSpawner_RandomMovement(npcs);
							npcs.target = null;
						}
					}
				}
				catch { }
			}

			yield return CoroutineEx.waitForSeconds(Config.NpcSpawner.AITickRate);
		}

		PutsWarn("Stopped AI processing.");
		yield break;
	}

	public bool NPCSpawner_AnyPlayersNearby(Vector3 position, float maxDist)
	{
		var list = Pool.GetList<BasePlayer>();
		Vis.Entities(position, maxDist, list, 131072);

		var result = false;
		foreach (BasePlayer basePlayer in list)
		{
			if (basePlayer.IsNpc || basePlayer.IsConnected)
			{
				continue;
			}
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive())
			{
				result = true;
				break;
			}
		}
		Facepunch.Pool.FreeList(ref list);
		return result;
	}
	public void NPCSpawner_RandomMovement(NPCSpawner npcs)
	{
		var bn = (BaseNavigator)null;

		if (npcs.BOT is ScientistNPC scientist) { bn = scientist.Brain.Navigator; }
		if (npcs.BOT is ScarecrowNPC scarecrow) { bn = scarecrow.Brain.Navigator; }

		if (bn == null) return;

		if (!bn.Moving && !npcs.stationary)
		{
			var newPos = npcs.serializedNPCSpawner.position;
			var d = npcs.RoamDistance / 2f;
			var vector = UnityEngine.Random.insideUnitCircle * d;

			newPos += new Vector3(vector.x, 0, vector.y);
			bn.SetDestination(newPos, BaseNavigator.NavigationSpeed.Slow);
		}
	}

	public void NPCSpawner_DressNPC(BasePlayer npc, NPCType npctype)
	{
		if (npc == null) { return; }
		switch (npctype)
		{
			case NPCType.Scientist:
				NPCSpawner_CreateCloths(npc, "hazmatsuit_scientist_arctic");
				switch ((int)UnityEngine.Random.Range(0f, 2))
				{
					case 0:
						NPCSpawner_CreateGun(npc, "rifle.lr300", "weapon.mod.flashlight");
						break;
					case 1:
						NPCSpawner_CreateGun(npc, "rifle.ak", "weapon.mod.flashlight");
						break;
					case 2:
						NPCSpawner_CreateGun(npc, "pistol.m92", "weapon.mod.flashlight");
						break;
				}
				break;
			case NPCType.Peacekeeper:
				NPCSpawner_CreateCloths(npc, "hazmatsuit_scientist_peacekeeper");
				NPCSpawner_CreateGun(npc, "rifle.m39", "weapon.mod.flashlight");
				break;
			case NPCType.HeavyScientist:
				NPCSpawner_CreateCloths(npc, "scientistsuit_heavy");
				switch ((int)UnityEngine.Random.Range(0f, 2))
				{
					case 0:
						NPCSpawner_CreateGun(npc, "lmg.m249", "weapon.mod.flashlight");
						break;
					case 1:
						NPCSpawner_CreateGun(npc, "shotgun.spas12", "weapon.mod.flashlight");
						break;
					case 2:
						NPCSpawner_CreateGun(npc, "rifle.lr300", "weapon.mod.flashlight");
						break;
				}
				break;
			case NPCType.JunkpileScientist:
				NPCSpawner_CreateCloths(npc, "hazmatsuit_scientist");
				NPCSpawner_CreateGun(npc, "pistol.m92", "weapon.mod.flashlight");
				break;
			case NPCType.Bandit:
				NPCSpawner_CreateCloths(npc, "attire.banditguard");
				NPCSpawner_CreateGun(npc, "rifle.semiauto", "weapon.mod.flashlight");
				break;
			case NPCType.Murderer:
				NPCSpawner_CreateCloths(npc, "scarecrow.suit");
				NPCSpawner_CreateCloths(npc, "jacket");
				NPCSpawner_CreateGun(npc, "chainsaw");
				break;
			case NPCType.Scarecrow:
				NPCSpawner_CreateCloths(npc, "halloween.mummysuit");
				switch ((int)UnityEngine.Random.Range(0f, 2))
				{
					case 0:
						NPCSpawner_CreateGun(npc, "pitchfork");
						break;
					case 1:
						NPCSpawner_CreateGun(npc, "sickle");
						break;
					case 2:
						NPCSpawner_CreateGun(npc, "bone.clubsickle");
						break;
				}
				break;
		}
	}
	public void NPCSpawner_CreateCloths(BasePlayer npc, string itemName, ulong skin = 0)
	{
		if (npc == null) return;

		var item = ItemManager.CreateByName(itemName, 1, skin);
		if (item == null) return;

		if (!item.MoveToContainer(npc.inventory.containerWear, -1, false)) item.Remove();
	}
	public void NPCSpawner_CreateGun(BasePlayer npc, string itemName, string attachment = "", ulong skinid = 0)
	{
		var item = ItemManager.CreateByName(itemName, 1, skinid);
		if (item == null) return;

		if (!item.MoveToContainer(npc.inventory.containerBelt, -1, false)) { item.Remove(); return; }

		var be = item.GetHeldEntity();
		if (be != null && be is BaseProjectile)
		{
			if (!string.IsNullOrEmpty(attachment))
			{
				var moditem = ItemManager.CreateByName(attachment, 1, 0);
				if (moditem != null && item.contents != null)
				{
					if (!moditem.MoveToContainer(item.contents)) { item.contents.itemList.Add(moditem); }
				}
			}
		}

		NPCSpawner_DoFixes(npc, item);
	}
	public void NPCSpawner_DoFixes(BasePlayer npc, Item item)
	{
		//
		// Reload mods on guns
		//
		npc.Invoke(() =>
		{
			if (npc != null && item != null)
			{
				npc.UpdateActiveItem(item.uid);
			}
		}, 2f);

		//
		// Chainsaw fix
		//
		npc.Invoke(() =>
		{
			if (npc != null)
			{
				if (npc.GetHeldEntity() is Chainsaw chainsaw)
				{
					chainsaw.SetFlag(BaseEntity.Flags.On, true);
					chainsaw.SendNetworkUpdateImmediate();
				}
			}
		}, 4f);
	}

	public bool NPCSpawner_IsOnGround(ref Vector3 position, out int areamask)
	{
		NavMeshHit navMeshHit;
		bool result;
		if (NavMesh.SamplePosition(position, out navMeshHit, 8f, -1))
		{
			areamask = navMeshHit.mask;
			position = navMeshHit.position;
			result = true;
		}
		else
		{
			areamask = -1;
			result = false;
		}
		return result;
	}

	public byte[] NPCSpawner_GetData(string name)
	{
		foreach (MapData MD in World.Serialization.world.maps)
		{
			try
			{
				if (System.Text.Encoding.ASCII.GetString(MD.data).Contains(name))
				{
					return MD.data;
				}
			}
			catch { }
		}

		return null;
	}
	public byte[] NPCSpawner_Serialize<T>(T stream)
	{
		if (stream == null) return null;

		try
		{
			using (var memoryStream = new MemoryStream())
			{
				new XmlSerializer(typeof(T)).Serialize(memoryStream, stream);
				return memoryStream.ToArray();
			}
		}
		catch (Exception ex)
		{
			PutsError("Failed serialization.", ex);
		}

		return null;
	}
	public T NPCSpawner_Deserialize<T>(byte[] bytes)
	{
		try
		{
			using (var memoryStream = new MemoryStream(bytes))
			{
				var xmlSerializer = new XmlSerializer(typeof(T));
				return (T)xmlSerializer.Deserialize(memoryStream);
			}
		}
		catch (Exception ex)
		{
			PutsError("Failed deserialization.", ex);
		}

		return default;
	}

	#endregion

	#region APC

	internal static APC_SerializedAPCPathList APC_serializedAPCPathList;
	internal static List<APC_CustomAPCSpawner> APC_list = new();
	internal static List<BasePlayer> APC_ViewingList = new();
	internal static Coroutine APC_Viewingthread;

	internal static IEnumerator APC_Setup()
	{
		bool HasPath;
		if (APC_serializedAPCPathList == null) { yield break; }
		else
		{
			List<APC_SerializedAPCPath> paths = APC_serializedAPCPathList.paths;
			int? num = (paths != null) ? new int?(paths.Count) : null;
			HasPath = (num.GetValueOrDefault() > 0 & num != null);
		}
		if (HasPath)
		{
			Singleton.PutsWarn($"Generating {APC_serializedAPCPathList.paths.Count} APC paths.");

			foreach (APC_SerializedAPCPath sapc in APC_serializedAPCPathList.paths)
			{
				if (sapc.interestNodes.Count == 0) { Singleton.PutsError("No APC interest nodes found."); continue; }
				APC_CustomAPCSpawner customAPCSpawner = new GameObject("CustomAPCSpawner").AddComponent<APC_CustomAPCSpawner>();
				customAPCSpawner.LoadBradleyPathing(sapc);
				APC_list.Add(customAPCSpawner);
				yield return CoroutineEx.waitForEndOfFrame;
			}
		}
		yield break;
	}
	internal static bool APC_ReadMapData()
	{
		bool flag = false;
		if (APC_serializedAPCPathList != null) { return flag; }
		byte[] array = APC_GetSerializedIOData();
		if (array != null && array.Length != 0)
		{
			APC_serializedAPCPathList = DeserializeMapData<APC_SerializedAPCPathList>(array, out flag);
		}
		if (APC_serializedAPCPathList == null) { APC_serializedAPCPathList = new APC_SerializedAPCPathList(); }
		Singleton.PutsWarn("No APC data found.");
		return flag;
	}
	internal static byte[] APC_GetSerializedIOData()
	{
		foreach (MapData MD in World.Serialization.world.maps)
		{
			try
			{
				if (System.Text.Encoding.ASCII.GetString(MD.data).Contains("SerializedAPCPathList")) { return MD.data; }
			}
			catch { }
		}
		return null;
	}
	internal static void APC_ForceRespawn(BasePlayer player)
	{
		int counter = 0;
		for (int i = 0; i < APC_list.Count; i++)
		{
			APC_CustomAPCSpawner customAPCSpawner = APC_list[i];
			if (!(customAPCSpawner == null) && (customAPCSpawner.bradleyAPC == null || !customAPCSpawner.bradleyAPC.IsAlive()))
			{
				customAPCSpawner.ForceRespawn();
				counter++;
			}
		}
		player.ChatMessage($"Respawned {counter:n0} bradleys");
	}
	internal static void APC_ForceKill(BasePlayer player)
	{
		int counter = 0;
		for (int i = 0; i < APC_list.Count; i++)
		{
			var customAPCSpawner = APC_list[i];
			if (!(customAPCSpawner == null) && (customAPCSpawner.bradleyAPC != null && customAPCSpawner.bradleyAPC.IsAlive()))
			{
				customAPCSpawner.bradleyAPC.DieInstantly();
				counter++;
			}
		}
		player.ChatMessage($"Killed {counter:n0} bradleys");
	}
	internal static void APC_GetStatus(BasePlayer player)
	{
		var counterA = 0;
		var counterD = 0;
		for (int i = 0; i < APC_list.Count; i++)
		{
			var customAPCSpawner = APC_list[i];
			if (!(customAPCSpawner == null) && customAPCSpawner.bradleyAPC != null)
			{
				if (customAPCSpawner.bradleyAPC.IsAlive())
				{
					counterA++;
				}
				else
				{
					counterD++;
				}
			}
		}
		player.ChatMessage($"Bradleys: Alive: {counterA:n0} Dead: {counterD:n0}");
	}
	internal static void APC_showapcpath(BasePlayer player)
	{
		if (player.IsAdmin)
		{
			if (APC_ViewingList.Contains(player))
			{
				player.ChatMessage("Stopped Viewing Cargo Path");
				APC_ViewingList.Remove(player);
			}
			else
			{
				player.ChatMessage("Started Viewing Cargo Path");
				APC_ViewingList.Add(player);
			}
			if (APC_Viewingthread == null && APC_ViewingList.Count != 0)
			{
				APC_Viewingthread = ServerMgr.Instance.StartCoroutine(ShowAPCPath());
				return;
			}
			else
			{
				ServerMgr.Instance.StopCoroutine(APC_Viewingthread);
				APC_Viewingthread = null;
			}
		}
	}
	internal static IEnumerator ShowAPCPath()
	{
		while (APC_ViewingList != null && APC_ViewingList.Count != 0)
		{
			foreach (BasePlayer bp in APC_ViewingList.ToArray())
			{
				if (!bp.IsConnected || bp.IsSleeping() || !bp.IsAdmin)
				{
					APC_ViewingList.Remove(bp);
					continue;
				}
				foreach (var capc in APC_list)
				{
					Vector3 LastNode = Vector3.zero;
					foreach (PathInterestNode pin in capc.basePath.interestZones)
					{
						bp.SendConsoleCommand("ddraw.sphere", 5f, Color.red, pin.transform.position, 1f);
						bp.SendConsoleCommand("ddraw.text", 5f, Color.green, pin.transform.position, "<size=20>APC Intrest Point</size>");
					}
					foreach (BasePathNode node in capc.basePath.nodes)
					{
						if (Vector3.Distance(node.transform.position, bp.transform.position) < 2000)
						{
							bp.SendConsoleCommand("ddraw.sphere", 5f, Color.green, node.transform.position, 3f);
							if (LastNode != Vector3.zero) { bp.SendConsoleCommand("ddraw.line", 5f, Color.blue, node.transform.position + new Vector3(0, 1, 0), LastNode + new Vector3(0, 1, 0)); }
							LastNode = node.transform.position;
						}
						else { LastNode = Vector3.zero; }
					}
				}
				yield return CoroutineEx.waitForSeconds(5f);
			}
		}
		ServerMgr.Instance.StopCoroutine(APC_Viewingthread);
		APC_Viewingthread = null;
		yield break;
	}

	#region Commands

	[ChatCommand("rusteditext.apc.apcforcerespawn")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void APC_APCForceRespawn(BasePlayer player)
	{
		APC_ForceRespawn(player);
	}

	[ChatCommand("rusteditext.apc.apcforcekill")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void APC_APCForceKill(BasePlayer player)
	{
		APC_ForceKill(player);
	}

	[ChatCommand("rusteditext.apc.apcstatus")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void APC_APCGetStatus(BasePlayer player)
	{
		APC_GetStatus(player);
	}

	[ChatCommand("rusteditext.apc.showapcpath")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void APC_APCShowAPCPath(BasePlayer player)
	{
		APC_ForceKill(player);
	}

	#endregion

	public class APC_CustomAPCSpawner : MonoBehaviour
	{
		public void LoadBradleyPathing(APC_SerializedAPCPath serializedAPCPath)
		{
			basePath = new GameObject("CustomAPCSpawner").AddComponent<BasePath>();
			basePath.nodes = new List<BasePathNode>();
			basePath.interestZones = new List<PathInterestNode>();
			basePath.speedZones = new List<PathSpeedZone>();
			for (int i = 0; i < serializedAPCPath.nodes.Count; i++)
			{
				BasePathNode basePathNode = new GameObject("BasePathNode").AddComponent<BasePathNode>();
				basePathNode.transform.position = serializedAPCPath.nodes[i];
				basePath.nodes.Add(basePathNode);
			}
			for (int j = 0; j < basePath.nodes.Count; j++)
			{
				BasePathNode basePathNode2 = basePath.nodes[j];
				if (!(basePathNode2 == null))
				{
					basePathNode2.linked = new List<BasePathNode>();
					basePathNode2.linked.Add((j == 0) ? basePath.nodes[basePath.nodes.Count - 1] : basePath.nodes[j - 1]);
					basePathNode2.linked.Add((j == basePath.nodes.Count - 1) ? basePath.nodes[0] : basePath.nodes[j + 1]);
					basePathNode2.maxVelocityOnApproach = -1f;
				}
			}
			for (int k = 0; k < serializedAPCPath.interestNodes.Count; k++)
			{
				PathInterestNode pathInterestNode = new GameObject("PathInterestNode").AddComponent<PathInterestNode>();
				pathInterestNode.transform.position = serializedAPCPath.interestNodes[k];
				basePath.interestZones.Add(pathInterestNode);
			}
			InvokeHandler.Invoke(this, new Action(Spawner), 3f);
			InvokeHandler.InvokeRepeating(this, new Action(Checker), 5f, 5f);
		}

		private void Checker()
		{
			if (!NeedsSpawn && (bradleyAPC == null || !bradleyAPC.IsAlive()))
			{
				Reset();
			}
		}

		private void Reset()
		{
			InvokeHandler.CancelInvoke(this, new Action(this.Spawner));
			InvokeHandler.Invoke(this, new Action(this.Spawner), UnityEngine.Random.Range(Bradley.respawnDelayMinutes - Bradley.respawnDelayVariance, Bradley.respawnDelayMinutes + Bradley.respawnDelayVariance) * 60f);
			NeedsSpawn = true;
		}

		public void ForceRespawn()
		{
			InvokeHandler.CancelInvoke(this, new Action(this.Spawner));
			Spawner();
		}

		private void Spawner()
		{
			SpawnBradley();
			NeedsSpawn = false;
		}

		private void SpawnBradley()
		{
			if (bradleyAPC == null)
			{
				Vector3 vector = (basePath.interestZones.Count > 0) ? basePath.interestZones.GetRandom<PathInterestNode>().transform.position : basePath.nodes[0].transform.position;
				bradleyAPC = GameManager.server.CreateEntity(prefab, vector, default(Quaternion), true).GetComponent<BradleyAPC>();
				bradleyAPC.enableSaving = false;
				bradleyAPC.Spawn();
				bradleyAPC.pathLooping = true;
				bradleyAPC.InstallPatrolPath(basePath);
				//Interface.CallHook("RustEdit_APCSpawned", bradleyAPC);

				Singleton.PutsWarn($"Custom APC spawned @ {vector}");
			}
		}

		public BasePath basePath;
		public BradleyAPC bradleyAPC;
		private bool NeedsSpawn;
		private string prefab = "assets/prefabs/npc/m2bradley/bradleyapc.prefab";
	}
	public class APC_SerializedAPCPath
	{

		public List<VectorData> nodes = new();
		public List<VectorData> interestNodes = new();
	}
	public class APC_SerializedAPCPathList
	{
		public List<APC_SerializedAPCPath> paths = new();
	}

	#endregion

	#region Cargo Path

	internal const string Cargo__c4 = "assets/prefabs/tools/c4/effects/c4_explosion.prefab";
	internal const string Cargo__debris = "assets/prefabs/npc/patrol helicopter/damage_effect_debris.prefab";
	internal static bool Cargo_DisableSpawns = false;
	internal const bool Cargo_SinkMode = true;
	internal const int Cargo_StopDelay = 60;
	internal const int Cargo_DistanceFromStop = 35;
	internal const int Cargo_DespawnDistance = 80;
	internal static List<BasePlayer> Cargo_ViewingList = new();
	internal static Coroutine Cargo_Viewingthread;
	internal static List<CargoMod> Cargo_CargoShips = new();
	internal static List<Vector3> Cargo_CargoPath = new();
	internal static List<Vector3> Cargo_CargoSpawn = new();
	internal static List<Vector3> Cargo_CargoStops = new();
	internal static SerializedPathList Cargo_serializedPathList;

	public class SerializedPathList
	{
		public List<VectorData> vectorData = new();
	}

	#region Hooks

	private void OnEntitySpawned(CargoShip cargo)
	{
		if (cargo != null)
		{
			Cargo_ApplyCargoMod(cargo);

			if (Cargo_CargoSpawn != null && Cargo_CargoSpawn.Count != 0)
			{
				if (!Cargo_DisableSpawns)
				{
					cargo.transform.position = Cargo_CargoSpawn.GetRandom();
				}
			}
		}
	}
	private object OnPlayerViolation(BasePlayer player, AntiHackType type, float amount)
	{
		var entity = player.GetParentEntity();

		if (type == AntiHackType.InsideTerrain && entity != null && entity is CargoShip)
		{
			player.Hurt(5f);
			return false;
		}

		return null;
	}

	#endregion

	#region Commands

	[ChatCommand("rusteditext.cargo.egresscargo")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void Cargo_Egresscargo(BasePlayer player)
	{
		foreach (CargoMod cm in Cargo_CargoShips)
		{
			if (cm != null && cm._cargoship != null)
			{
				cm._cargoship.StartEgress();
			}
		}

		player.ChatMessage("Egressing CargoShips.");
	}

	[ChatCommand("rusteditext.cargo.showcargopath")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void Cargo_ShowCargoPath(BasePlayer player)
	{
		if (player.IsAdmin)
		{
			if (Cargo_ViewingList.Contains(player))
			{
				player.ChatMessage("Stopped Viewing Cargo Path");
				Cargo_ViewingList.Remove(player);
			}
			else
			{
				player.ChatMessage("Started Viewing Cargo Path");
				Cargo_ViewingList.Add(player);
			}
			if (Cargo_Viewingthread == null && Cargo_ViewingList.Count != 0)
			{
				Cargo_Viewingthread = ServerMgr.Instance.StartCoroutine(ShowCargoPath());
				return;
			}
			else
			{
				ServerMgr.Instance.StopCoroutine(Cargo_Viewingthread);
				Cargo_Viewingthread = null;
			}
		}
	}

	[ChatCommand("rusteditext.cargo.bypasscargospawn")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void Cargo_BypassCargoSpawn(BasePlayer player)
	{
		Cargo_DisableSpawns = !Cargo_DisableSpawns;
		player.ChatMessage("Disabling cargospawn points " + Cargo_DisableSpawns.ToString());
	}

	[ChatCommand("rusteditext.cargo.exportcargopath")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void Cargo_ExportCargoPath(BasePlayer player)
	{
		player.ChatMessage("Exporting SerializedPathList.xml!");
		Cargo_SaveMap();
		player.ChatMessage("Saved in server root.");
	}

	#endregion

	#region Custom Hooks

	private void IOnCargoShipEgressStart(CargoShip cargo)
	{
		if (cargo != null)
		{
			if (Cargo_AreaBlocked(cargo.transform.position))
			{
				cargo.Invoke(() => { cargo.StartEgress(); }, 30);
				return;
			}
			if (Cargo_SinkMode)
			{
				var cargos = Pool.GetList<CargoMod>();
				cargos.AddRange(Cargo_CargoShips);

				foreach (var cm in cargos)
				{
					if (cm._cargoship == cargo)
					{
						Cargo_CargoShips.Remove(cm);
						cm._cargoship.targetNodeIndex = -1;
						cm.AllowCrash = true;
						break;
					}
				}

				Pool.FreeList(ref cargos);
			}
		}
	}
	private object ICanCargoShipBlockWaterFor()
	{
		return true;
	}
	private object ICanGenerateOceanPatrolPath()
	{
		if (Cargo_ReadMapData())
		{
			if (Cargo_serializedPathList != null)
			{
				var newPath = new List<Vector3>();

				foreach (var vd in Cargo_serializedPathList.vectorData)
				{
					newPath.Add(new Vector3(vd.x, vd.y, vd.z));
				}

				return Cargo_CargoPath = newPath;
			}
		}

		return null;
	}
	private object IOnGenerateOceanPatrolPath(List<Vector3> list)
	{
		if (Cargo_CargoPath != null && Cargo_CargoPath.Count != 0)
		{
			Singleton.PutsWarn($"Loaded custom path with {list.Count:n0} nodes.");
			return Cargo_CargoPath;
		}

		return null;
	}
	private void IOnPostGenerateOceanPatrolPath(List<Vector3> list)
	{
		Cargo_CargoPath = list;
	}

	#endregion

	#region Command Helpers

	public IEnumerator ShowCargoPath()
	{
		var distance = 2000;
		var LastNode = Vector3.zero;

		while (Cargo_ViewingList != null && Cargo_ViewingList.Count != 0)
		{
			foreach (BasePlayer bp in Cargo_ViewingList.ToArray())
			{
				if (!bp.IsConnected || bp.IsSleeping() || !bp.IsAdmin)
				{
					Cargo_ViewingList.Remove(bp);
					continue;
				}

				foreach (Vector3 spawnpoints in Cargo_CargoSpawn)
				{
					bp.SendConsoleCommand("ddraw.box", 5f, Color.green, spawnpoints, 10f);
					bp.SendConsoleCommand("ddraw.text", 5f, Color.green, spawnpoints, "<size=20>CargoSpawn</size>");
				}

				foreach (Vector3 stoppoints in Cargo_CargoStops)
				{
					bp.SendConsoleCommand("ddraw.box", 5f, Color.red, stoppoints, 10f);
					bp.SendConsoleCommand("ddraw.text", 5f, Color.red, stoppoints, "<size=20>CargoStop</size>");
				}

				var nodeindex = 0;
				foreach (Vector3 vector in Cargo_CargoPath)
				{
					if (Vector3.Distance(vector, bp.transform.position) < distance)
					{
						var colour = Color.white;
						if (Cargo_AreaBlocked(vector)) { colour = Color.red; }

						bp.SendConsoleCommand("ddraw.sphere", 5f, colour, vector, 20f);

						if (LastNode != Vector3.zero) { bp.SendConsoleCommand("ddraw.line", 5f, colour, vector, LastNode); }

						bp.SendConsoleCommand("ddraw.text", 5f, colour, vector, "<size=25>" + nodeindex.ToString() + "</size>");
						LastNode = vector;
					}
					else { LastNode = Vector3.zero; }
					nodeindex++;
				}
				yield return CoroutineEx.waitForSeconds(5f);
			}
		}

		ServerMgr.Instance.StopCoroutine(Cargo_Viewingthread);
		Cargo_Viewingthread = null;
		yield break;
	}

	#endregion

	internal static void Cargo_ApplyCargoMod(CargoShip cs, bool reload = false)
	{
		if (cs != null)
		{
			if (cs.gameObject.HasComponent<CargoMod>()) { return; }
			var newcargoship = cs.gameObject.AddComponent<CargoMod>();
			newcargoship._cargoship = cs;
			newcargoship.CargoThread = ServerMgr.Instance.StartCoroutine(newcargoship.CargoTick());
			Cargo_CargoShips.Add(newcargoship);
		}
	}
	internal static bool Cargo_AreaStop(Vector3 vector3)
	{
		foreach (var vector in Cargo_CargoStops)
		{
			if (Vector3.Distance(vector, vector3) < Cargo_DistanceFromStop)
			{
				return true;
			}
		}

		return false;
	}
	internal static bool Cargo_AreaBlocked(Vector3 vector3)
	{
		var colliders = Pool.GetList<Collider>();
		Vis.Colliders(vector3, 5, colliders, -1, QueryTriggerInteraction.Collide);
		foreach (var collider in colliders) { if (collider.name.Contains("prevent_building_")) { return true; } }
		Pool.FreeList(ref colliders);
		return false;
	}
	internal static bool Cargo_InitializeHook()
	{
		if (Cargo_CargoSpawn.Count != 0 || Cargo_CargoStops.Count != 0)
		{
			Singleton.PutsWarn($"Found {Cargo_CargoSpawn.Count:n0} spawnpoints and {Cargo_CargoStops.Count:n0} stop points.");
		}

		foreach (var b in BaseNetworkable.serverEntities)
		{
			if (b is CargoShip)
			{
				Cargo_ApplyCargoMod(b as CargoShip, true);
			}
		}

		return true;
	}
	internal static bool Cargo_ShutdownHook()
	{
		foreach (var destry in Cargo_CargoShips)
		{
			if (destry != null && destry._cargoship != null)
			{
				ServerMgr.Instance.StopCoroutine(destry.CargoThread);
				UnityEngine.Object.Destroy(destry);
			}
		}
		return true;
	}
	internal static bool Cargo_World_SpawnHook(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		//2741054453 - assets/bundled/prefabs/world/event_cargoship.prefab
		if (prefab.ID == 2741054453)
		{
			Cargo_CargoSpawn.Add(position);
			return false;
		}
		//843218194 - assets/prefabs/tools/map/cargomarker.prefab
		else if (prefab.ID == 843218194)
		{
			Cargo_CargoStops.Add(position);
			return false;
		}
		return true;
	}
	internal static bool Cargo_SpawnHook(CargoShip __instance)
	{

		return true;
	}
	internal static bool Cargo_AntiHackHook(BasePlayer player, AntiHackType type)
	{
		if (player != null)
		{
			try
			{
				var be = player.GetParentEntity();

				if (be != null && be is CargoShip && type == AntiHackType.InsideTerrain)
				{
					player.Hurt(5f);
				}

				return false;
			}
			catch { }
		}
		return true;
	}
	internal static void Cargo_SaveMap()
	{
		if (Cargo_serializedPathList == null)
		{
			var SPL = new SerializedPathList();
			var nvd = new List<VectorData>();
			foreach (Vector3 vector in Cargo_CargoPath) { nvd.Add(vector); }
			SPL.vectorData = nvd;
			Cargo_serializedPathList = SPL;
		}
		System.IO.File.WriteAllBytes("SerializedPathList.xml", SerializeMapData(Cargo_serializedPathList));
	}
	internal static byte[] Cargo_GetSerializedIOData()
	{
		foreach (var md in World.Serialization.world.maps)
		{
			try
			{
				if (System.Text.Encoding.ASCII.GetString(md.data).Contains("SerializedPathList")) { return md.data; }
			}
			catch { }
		}
		return null;
	}

	internal static bool Cargo_ReadMapData()
	{
		var flag = false;
		if (Cargo_serializedPathList != null) { return flag; }

		var array = Cargo_GetSerializedIOData();
		if (array != null && array.Length != 0)
		{
			Cargo_serializedPathList = DeserializeMapData<SerializedPathList>(array, out flag);
		}

		Singleton.PutsWarn($"Cargo Path data valid: {flag}");
		return flag;
	}
	internal static byte[] Cargo_SerializeMapData<T>(T stream)
	{
		if (stream == null)
		{
			return null;
		}

		byte[] result;

		try
		{
			using (var memoryStream = new MemoryStream())
			{
				new XmlSerializer(typeof(T)).Serialize(memoryStream, stream);
				result = memoryStream.ToArray();
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message + "\n" + ex.StackTrace);
			result = null;
		}

		return result;
	}

	#region Command

	public class CargoMod : FacepunchBehaviour
	{
		public CargoShip _cargoship;
		public bool HasStopped = false;
		public bool AllowCrash = false;
		public Coroutine CargoThread;
		public FieldInfo _currentThrottle = typeof(CargoShip).GetField("currentThrottle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		public FieldInfo _turnScale = typeof(CargoShip).GetField("turnScale", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		public FieldInfo _currentTurnSpeed = typeof(CargoShip).GetField("currentTurnSpeed", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		public FieldInfo _currentVelocity = typeof(CargoShip).GetField("currentVelocity", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		public int LastNode = -1;
		public List<ScientistNPC> NPCs = new();
		private float PlayerCheck = 0;
		private bool sunk = false;
		public bool inhaled = false;

		public IEnumerator CargoTick()
		{
			while (_cargoship != null)
			{
				if (AllowCrash)
				{
					HasStopped = true;
					_cargoship.CancelInvoke(new Action(_cargoship.RespawnLoot));
					_cargoship.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
					_cargoship.targetNodeIndex = -1;
					_currentThrottle.SetValue(_cargoship, 0);
					Vector3 bow = _cargoship.transform.position + (_cargoship.transform.forward * -35) + (_cargoship.transform.up * 1);
					List<Vector3> ExpoPos = new List<Vector3>();
					ExpoPos.Add(bow + _cargoship.transform.right * 6);
					ExpoPos.Add(bow + _cargoship.transform.right * -6);
					ExpoPos.Add(bow + (_cargoship.transform.forward * -10) + (_cargoship.transform.right * 6));
					ExpoPos.Add(bow + (_cargoship.transform.forward * -10) + (_cargoship.transform.right * -6));
					_cargoship.Invoke(() => { if (ExpoPos != null) { playexp(ExpoPos); } }, 10);
					yield break;
				}
				if (!HasStopped && Cargo_AreaStop(_cargoship.transform.position))
				{
					HasStopped = true;
					StopCargoShip(Cargo_StopDelay);
				}
				if (_cargoship.transform.position.y < -1) { AllowCrash = true; }
				yield return CoroutineEx.waitForSeconds(1f);
			}
			yield break;
		}

		public IEnumerator Sink()
		{

			while (_cargoship != null && HasStopped)
			{
				if (PlayerCheck < Time.time)
				{
					PlayerCheck = Time.time + 3f;
					foreach (BaseEntity b in _cargoship.children.ToArray())
					{
						if (b is ScientistNPC)
						{
							if (b == null || b.IsDestroyed) { continue; }
							if ((b as ScientistNPC).IsAlive() && b.transform.position.y < -1.5f) { (b as ScientistNPC).Kill(); }
							continue;
						}
					}
				}
				_cargoship.transform.position += _cargoship.transform.up * -1.5f * Time.deltaTime;
				if ((_cargoship.transform.position.y - 1) <= TerrainMeta.HeightMap.GetHeight(_cargoship.transform.position))
				{
					sunk = true;
					while (_cargoship != null && PlayersNearbyfunction(_cargoship.transform.position, Cargo_DespawnDistance))
					{
						yield return CoroutineEx.waitForSeconds(5f);
					}
					killme();
					yield break;
				}
				yield return CoroutineEx.waitForSeconds(0.01f);
			}
			yield break;
		}

		public void StopCargoShip(float seconds)
		{
			if (_cargoship == null || AllowCrash) { return; }
			LastNode = _cargoship.targetNodeIndex;
			if (LastNode == -1) { LastNode = _cargoship.GetClosestNodeToUs(); }
			_cargoship.targetNodeIndex = -1;
			_currentThrottle.SetValue(_cargoship, 0);
			_cargoship.Invoke(() =>
			{
				if (_cargoship != null)
				{
					_cargoship.targetNodeIndex = LastNode;
					LastNode = -1;
					_cargoship.Invoke(() => { HasStopped = false; }, 60);
				}
			}, seconds);
		}

		public void killme()
		{
			if (_cargoship != null)
			{
				foreach (BaseEntity b in _cargoship.children.ToArray())
				{
					if (b != null && b.GetParentEntity() is CargoShip)
					{
						if (b is BasePlayer || b is LootableCorpse || b is BaseCorpse || b is PlayerCorpse)
						{
							Vector3 oldpos = b.transform.position;
							oldpos.y = 1;
							b.SetParent(null, true, true);
							continue;
						}
						b.Kill();
					}
				}
			}
			_cargoship.Invoke(() =>
			{
				if (_cargoship != null && !_cargoship.IsDestroyed) { _cargoship.Kill(); }
				Destroy(this);
			}, 0.1f);
		}

		public bool PlayersNearbyfunction(Vector3 _base, float Distance)
		{
			if (_base == null) { return false; }
			List<BasePlayer> obj = Pool.GetList<BasePlayer>();
			Vis.Entities(_base, Distance, obj, 131072);
			bool result = false;
			if (obj == null) { return false; }
			foreach (BasePlayer player in obj)
			{
				if (player == null) { continue; }
				if (!player.IsSleeping() && player.IsAlive() && !player.IsNpc && player.IsConnected && player.transform.position.y < -2f)
				{
					if (sunk) { player.metabolism.radiation_poison.Add(2); player.Hurt(1, Rust.DamageType.Radiation); }
					result = true;
				}
			}
			Pool.FreeList(ref obj);
			return result;
		}

		private void playexp(List<Vector3> ExPoint)
		{
			if (ExPoint != null && ExPoint.Count > 1)
			{
				RunEffect(Cargo__c4, null, ExPoint[0]);
				RunEffect(Cargo__debris, null, ExPoint[1]);
				ExPoint.RemoveAt(0);
				ExPoint.RemoveAt(0);
				_cargoship.Invoke(() => { playexp(ExPoint); }, 0.5f);
				return;
			}
			ServerMgr.Instance.StartCoroutine(Sink());
		}

		private void RunEffect(string name, BaseEntity entity = null, Vector3 position = new Vector3(), Vector3 offset = new Vector3())
		{
			if (entity != null) { Effect.server.Run(name, entity, 0, offset, position, null, true); return; }
			Effect.server.Run(name, position, Vector3.up, null, true);
		}
	}

	#endregion

	#endregion

	#region I/O

	internal static List<BaseEntity> IO_DestroyOnUnload = new();
	internal static Dictionary<Vector3, PrefabData> IO_Elevators = new();
	internal static List<Vector3> IO_Protect;
	internal static List<IOEntity> IO_Processed = new();
	internal static Dictionary<AutoTurret, bool> IO_AutoTurrets = new();
	internal static List<Dictionary<Vector3, Vector3>> IO_Wires = new();
	internal static SerializedIOData IO_serializedIOData;
	internal static Type[] IO_types = new Type[]
	{
			typeof(GroundWatch),
			typeof(DestroyOnGroundMissing),
			typeof(DeployableDecay)
	};

	internal static void IO_RunCore_IO()
	{
		foreach (KeyValuePair<Vector3, PrefabData> be in IO_Elevators)
		{
			try
			{
				IO_RecreateElevatorBase("assets/prefabs/deployable/elevator/elevator.prefab", be.Value.position, be.Value.rotation);
			}
			catch { Debug.LogError("Fault With Currupted Elevators"); }
		}
		if (IO_serializedIOData != null)
		{
			foreach (SerializedIOEntity SIOE in IO_serializedIOData.entities)
			{
				try
				{
					BaseEntity IO = IO_FindEntity(SIOE.position, SIOE.fullPath);
					if (IO == null) { continue; }
					if (SIOE.timerLength == 0) { SIOE.timerLength += 0.25f; }
					try
					{
						if (IO is Elevator elevator)
						{
							try
							{
								IO_CreateElevator(elevator, SIOE);
							}
							catch
							{
								Debug.LogError("Elevator Fault");
							}
							continue;
						}
						else if (IO is CardReader reader)
						{
							if (reader != null)
							{
								SIOE.accessLevel += 1;
								var cardReaderMonitor = IO.gameObject.GetComponent<CardReaderMonitor>() ?? IO.gameObject.AddComponent<CardReaderMonitor>();
								if (cardReaderMonitor != null)
								{
									cardReaderMonitor.Timerlength = SIOE.timerLength;
								}
								reader.accessLevel = SIOE.accessLevel;
								reader.accessDuration = SIOE.timerLength;
								reader.SetFlag(BaseEntity.Flags.Reserved1, SIOE.accessLevel == 1, false, true);
								reader.SetFlag(BaseEntity.Flags.Reserved2, SIOE.accessLevel == 2, false, true);
								reader.SetFlag(BaseEntity.Flags.Reserved3, SIOE.accessLevel == 3, false, true);
							}
						}
						else if (IO is PressButton pressButton)
						{
							pressButton.pressDuration = SIOE.timerLength;
						}
						else if (IO is RFBroadcaster rf)
						{
							rf.frequency = SIOE.frequency;
							rf.MarkDirty();
							rf.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
						}
						else if (IO is CCTV_RC cctv)
						{
							cctv.isStatic = true;
							cctv.UpdateIdentifier(SIOE.rcIdentifier, true);
						}
						else if (IO is Telephone telephone)
						{
							telephone.Controller.PhoneName = SIOE.phoneName;
						}
						else if (IO is TimerSwitch timerSwitch)
						{
							timerSwitch.timerLength = SIOE.timerLength;
						}
						else if (IO is SamSite samSite)
						{
							samSite.staticRespawn = true;
						}
						else if (IO is ElectricalBranch electricalBranch)
						{
							electricalBranch.branchAmount = SIOE.branchAmount;
						}
						else if (IO is PowerCounter power)
						{
							power.SetFlag(PowerCounter.Flag_ShowPassthrough, SIOE.counterPassthrough);
							power.SetCounterNumber(0);
							power.targetCounterNumber = SIOE.targetCounterNumber;
						}
						else if (IO is AutoTurret)
						{
							var manager = IO.gameObject.GetComponent<AutoTurretManager>() ?? IO.gameObject.AddComponent<AutoTurretManager>();
							manager.SetupTurret(SIOE.unlimitedAmmo, SIOE.peaceKeeper, SIOE.autoTurretWeapon);
						}
						else if (IO is WheelSwitch wheelSwitch)
						{
							var connection = IO.gameObject.GetComponent<IOEntityToWheelSwitchConnection>() ?? IO.gameObject.AddComponent<IOEntityToWheelSwitchConnection>();
							connection.SetTarget(wheelSwitch);
						}
						else if (IO is DoorManipulator doorManipulator)
						{
							doorManipulator.SetTargetDoor(doorManipulator.FindDoor(true));
						}
					}
					catch
					{
						Debug.LogError("Failed To Set Setting " + IO.ToString());
					}
					IOEntity IOE = IO as IOEntity;
					if (IOE == null) { continue; }
					Array.Resize<IOEntity.IOSlot>(ref IOE.outputs, SIOE.outputs.Length);
					for (int i = 0; i < IOE.outputs.Length; i++)
					{
						try
						{
							IOE.outputs[i] = new IOEntity.IOSlot { connectedTo = new IOEntity.IORef() };
							if (SIOE.outputs[i] != null)
							{
								IOEntity I = (IO_FindEntity(SIOE.outputs[i].position, SIOE.outputs[i].fullPath) as IOEntity);
								if (I is Elevator && !(IOE is ElectricGenerator))
								{
									int Floor = SIOE.outputs[i].connectedTo;
									int socket = 0;
									if (Floor % 2 == 0) { Floor = (Floor / 2); }
									else { Floor = ((Floor + 1) / 2); socket = 1; }
									List<Elevator> list = new List<Elevator>();
									Vis.Entities(IO_GetWorldSpaceFloorPosition(I as Elevator, Floor - 1), 1f, list);
									foreach (Elevator be in list)
									{
										IO_RunWire(IOE, i, be, socket);
										break;
									}
									continue;
								}
								if (I != null)
								{
									IO_RunWire(IOE, i, I, SIOE.outputs[i].connectedTo);
								}
							}
						}
						catch { }
					}
				}
				catch (Exception e)
				{
					Debug.LogError(e.ToString());
				}
			}
		}
		foreach (IOEntity IO in IO_Processed)
		{
			try
			{
				IO.MarkDirtyForceUpdateOutputs();
				IO.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
			catch { }
		}

		Singleton.PutsWarn($"Ran {IO_Wires.Count:n0} IO connections.");
	}
	internal static void IO_ReadMapData()
	{
		IO_Protect = new List<Vector3>();
		if (IO_serializedIOData != null) { return; }
		byte[] array = IO_GetSerializedIOData();
		if (array != null)
		{
			IO_serializedIOData = DeserializeMapData<SerializedIOData>(array, out var flag);

			if (flag && IO_serializedIOData != null)
			{
				foreach (var sioe in IO_serializedIOData.entities)
				{
					IO_Protect.Add(new Vector3(sioe.position.x, sioe.position.y, sioe.position.z));
				}
			}

			Singleton.PutsWarn($"I/O data valid: {flag}");
		}
	}
	internal static byte[] IO_GetSerializedIOData()
	{
		foreach (var map in World.Serialization.world.maps)
		{
			if (System.Text.Encoding.ASCII.GetString(map.data).Contains("SerializedIOData"))
			{
				return map.data;
			}
		}
		return null;
	}
	internal static bool IO_DirtyElevators()
	{
		var restart = false;
		foreach (var be in IO_Elevators)
		{
			var list = Pool.GetList<BaseEntity>();
			var floor = be.Key;

			for (int i = 0; i < 100; i++)
			{
				Vis.Entities(floor, 3f, list);
				floor.y = i * 3f;
				floor.y -= 1f;
				foreach (BaseEntity bel in list)
				{
					if (bel is Elevator || bel is ElevatorLift)
					{
						if (!bel.IsDestroyed)
						{
							restart = true;
							bel.Kill();
						}
					}
				}
			}

			Pool.FreeList(ref list);
		}
		return restart;
	}
	internal static void IO_RunWire(IOEntity OutputSlot, int s_slot, IOEntity InputSlot, int Input_slot)
	{
		if (InputSlot.inputs[Input_slot] == null || InputSlot.inputs.Length < Input_slot - 1 || OutputSlot.outputs[s_slot] == null || OutputSlot.outputs.Length < s_slot - 1)
		{
			Debug.LogError("Failed To Run Wire No Socket: " + OutputSlot.ToString() + ":" + s_slot + " To " + InputSlot.ToString() + ":" + Input_slot);
			return;
		}
		if (InputSlot is WheelSwitch)
		{
			IOEntityToWheelSwitchConnection WheelSwitchConnection = OutputSlot.gameObject.GetComponent<IOEntityToWheelSwitchConnection>() ?? OutputSlot.gameObject.AddComponent<IOEntityToWheelSwitchConnection>();
			WheelSwitchConnection.SetTarget(InputSlot as WheelSwitch);
		}
		InputSlot.inputs[Input_slot].connectedTo.Set(OutputSlot);
		InputSlot.inputs[Input_slot].connectedToSlot = s_slot;
		InputSlot.inputs[Input_slot].connectedTo.Init();
		OutputSlot.outputs[s_slot].connectedTo.Set(InputSlot);
		OutputSlot.outputs[s_slot].connectedToSlot = Input_slot;
		OutputSlot.outputs[s_slot].connectedTo.Init();
		if (!IO_Processed.Contains(OutputSlot)) { IO_Processed.Add(OutputSlot); }
		if (!IO_Processed.Contains(InputSlot)) { IO_Processed.Add(InputSlot); }
		IO_LogWires(InputSlot, OutputSlot);
	}
	internal static void IO_LogWires(IOEntity Input, IOEntity Output)
	{
		Vector3 P1 = Input.transform.position;
		Vector3 P2 = Output.transform.position;
		if (Input is CardReader || Input is TimerSwitch || Input is PowerCounter || Input is ElectricSwitch || Input is SmartSwitch || Input is PressButton || Input is ORSwitch || Input is XORSwitch || Input is ANDSwitch || Input is DoorManipulator || Input is FuseBox) { P1.y += 0.8f; }
		if (Output is CardReader || Output is TimerSwitch || Output is PowerCounter || Output is ElectricSwitch || Output is SmartSwitch || Output is PressButton || Output is ORSwitch || Output is XORSwitch || Output is ANDSwitch || Output is DoorManipulator || Output is FuseBox) { P2.y += 0.8f; }
		IO_Wires.Add(new Dictionary<Vector3, Vector3>() { { P1, P2 } });
	}
	internal static void IO_CreateElevator(Elevator baseelevator, SerializedIOEntity serializedIOEntity)
	{
		IO_Processed.Add(baseelevator);
		Elevator Top = baseelevator;
		for (int i = 1; i < serializedIOEntity.floors; i++)
		{
			Elevator elevator2 = GameManager.server.CreateEntity(baseelevator.PrefabName, baseelevator.transform.position + baseelevator.transform.up * (3f * (float)i), baseelevator.transform.rotation, true) as Elevator;
			elevator2.pickup.enabled = false;
			elevator2.EnableSaving(false);
			elevator2.Spawn();
			IO_SetupEntity(elevator2);
			IO_Protect.Add(elevator2.transform.position);
			IO_Processed.Add(elevator2);
			IO_DestroyOnUnload.Add(elevator2);
			elevator2.RefreshEntityLinks();
			elevator2.OnDeployed(null, null, null);
			elevator2.SetFlag(BaseEntity.Flags.Reserved2, true, false, false);
			Top = elevator2;
		}
		Top.SetFlag(BaseEntity.Flags.Reserved1, true, false, false);
		IO_CreateLift(Top);
	}
	internal static void IO_CreateLift(Elevator elevator)
	{		
		var elevatorLift = elevator.liftEntity;
		if (elevatorLift == null)
		{
			elevatorLift = IO_FindLift(elevator);
			if (elevatorLift == null)
			{
				elevatorLift = GameManager.server.CreateEntity(elevator.LiftEntityPrefab.resourcePath, IO_GetWorldSpaceFloorPosition(elevator, elevator.Floor), elevator.LiftRoot.rotation, true) as ElevatorLift;
				elevatorLift.SetParent(elevator, true, false);
				elevatorLift.enableSaving = false;
				elevatorLift.Spawn();
				IO_DestroyOnUnload.Add(elevatorLift);
				IO_Protect.Add(elevatorLift.transform.position);
				elevator.liftEntity = elevatorLift;
				return;
			}
		}
		if (elevatorLift != null)
		{
			elevatorLift.pickup.enabled = false;
			elevatorLift.enableSaving = false;
			elevatorLift.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			IO_DestroyOnUnload.Add(elevatorLift);
			IO_Protect.Add(elevatorLift.transform.position);
		}
	}
	internal static ElevatorLift IO_FindLift(Elevator elevator)
	{
		foreach (var baseEntity in elevator.children)
		{
			if (baseEntity is ElevatorLift lift)
			{
				return lift;
			}
		}
		return null;
	}
	internal static Vector3 IO_GetWorldSpaceFloorPosition(Elevator elevator, int floor)
	{
		var num = elevator.Floor - floor;
		var vector = elevator.transform.up * ((float)num * 3f);
		vector.y -= 1f;
		return elevator.transform.position - vector;
	}
	internal static BaseEntity IO_FindEntity(Vector3 pos, string path = "")
	{
		if (pos == null || path == null) { return null; }
		foreach (BaseNetworkable bn in BaseEntity.serverEntities)
		{
			if (bn == null || bn.IsDestroyed) { continue; }
			if (bn.PrefabName != path && path != "") { continue; }
			if (Vector3.Distance(pos, bn.transform.position) < 0.01f)
			{
				BaseEntity be = bn as BaseEntity;
				if (be != null) { return be; }
			}
		}
		return null;
	}
	internal static BaseEntity IO_RecreateElevatorBase(string path, Vector3 pos, Quaternion rot)
	{
		BaseEntity elevator = GameManager.server.CreateEntity(path, pos, rot, true);
		IO_Protect.Add(elevator.transform.position);
		elevator.Spawn();
		IO_SetupEntity(elevator);
		IO_DestroyOnUnload.Add(elevator);
		elevator.SendNetworkUpdateImmediate();
		return elevator;
	}
	internal static  void IO_SetupEntity(BaseEntity be)
	{
		if (be is ReactiveTarget)
		{
			(be as ReactiveTarget).ResetTarget();
		}
		BaseEntity.Flags flags = be.flags;
		if (be is Elevator)
		{
			be.enableSaving = false;
		}
		else
		{
			be.enableSaving = true;
		}
		be.ResetState();
		be.flags = flags;
		IO_RustEditIOEntity(be);
		if (be is IOEntity)
		{
			IOEntity BEIO = be as IOEntity;
			if (BEIO != null)
			{
				BEIO.ResetIOState();
				try { BEIO.OnCircuitChanged(true); } catch { }
				IO_ResetIOEntity(BEIO);
				BEIO.SendIONetworkUpdate();
			}
		}
	}
	internal static void IO_RustEditIOEntity(BaseEntity ioentity_0)
	{
		for (int i = 0; i < IO_types.Length; i++)
		{
			Component component = ioentity_0.GetComponent(IO_types[i]);
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
		}
		DecayEntity componentInParent = ioentity_0.GetComponentInParent<DecayEntity>();
		if (componentInParent != null)
		{
			componentInParent.decay = null;
		}
	}
	internal static void IO_ResetIOEntity(IOEntity ioentity_0)
	{
		if (!(ioentity_0 == null))
		{
			for (int i = 0; i < ioentity_0.inputs.Length; i++)
			{
				ioentity_0.inputs[i].Clear();
			}
			for (int j = 0; j < ioentity_0.outputs.Length; j++)
			{
				ioentity_0.outputs[j].Clear();
			}
		}
	}
	internal static void IO_ReloadDoors(BasePlayer player)
	{
		var reloaded = 0;

		var currentDoors = new Dictionary<BaseEntity, Vector3>();
		foreach (BaseNetworkable bn in BaseNetworkable.serverEntities)
		{
			if (bn == null || !bn.PrefabName.ToLower().Contains("hinged"))
			{
				continue;
			}
			currentDoors.Add(bn as BaseEntity, bn.transform.position);
		}
		player.ChatMessage("Found " + currentDoors.Count + " Doors on server!");
		foreach (PrefabData pd in World.Serialization.world.prefabs)
		{
			string prefabpath = StringPool.Get(pd.id);
			if (!prefabpath.ToLower().Contains("hinged"))
			{
				continue;
			}
			if (!currentDoors.ContainsValue(pd.position))
			{
				List<Door> olddoor = new List<Door>();
				Vis.Entities<Door>(pd.position, 1.5f, olddoor);
				if (olddoor != null && olddoor.Count != 0) { continue; }
				try
				{
					StabilityEntity newdoor = GameManager.server.CreateEntity(prefabpath, pd.position, Quaternion.Euler(pd.rotation)) as StabilityEntity;
					if (newdoor != null)
					{
						newdoor.Spawn();
						IO_SetupEntity(newdoor);
						newdoor.grounded = true;
						newdoor.pickup.enabled = false;
						reloaded++;
						newdoor.Invoke(() =>
						{
							List<DoorManipulator> list = new List<DoorManipulator>();
							Vis.Entities<DoorManipulator>(pd.position, 1.5f, list);
							if (list != null && list.Count != 0) { list[0].SetTargetDoor(newdoor as Door); }
						}
						, 5f);
					}
				}
				catch { }
			}
		}
		player.ChatMessage("Reloaded " + reloaded + " Missing Doors");
	}
	internal static void IO_ShowWires(BasePlayer player)
	{
		bool Revert = false;
		if (!player.IsAdmin)
		{
			Revert = true;
			player.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, true);
		}
		foreach (Dictionary<Vector3, Vector3> v in IO_Wires)
		{
			foreach (KeyValuePair<Vector3, Vector3> w in v)
			{
				if (Vector3.Distance(w.Key, player.transform.position) < 100 || Vector3.Distance(w.Value, player.transform.position) < 100)
				{
					Vector3 point1 = w.Key + new Vector3(0, 0.5f, 0);
					Vector3 point2 = w.Value + new Vector3(0, 0.5f, 0);
					if (player.IsAdmin)
					{
						player.SendConsoleCommand("ddraw.line", 10f, Color.green, point1, point2);
					}
				}
			}
		}
		if (Revert)
		{
			player.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, false);
		}
	}
	internal static bool IO_WorldSpawnHook(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		if (IO_Protect == null)
		{
			IO_ReadMapData();
		}
		if (position == null || prefab == null)
		{
			return true;
		}
		if (prefab.ID == 3978222077)
		{
			PrefabData pd = new PrefabData();
			pd.position = position;
			pd.id = 3978222077;
			pd.rotation = rotation;
			pd.scale = scale;
			pd.category = category;
			IO_Elevators.Add(position, pd);
			return false;
		}
		if (IO_Protect.Contains(position))
		{
			var be = prefab.Object.GetComponent<BaseEntity>();
			if (be != null)
			{
				IO_SetupEntity(be);
			}
		}
		return true;
	}

	#region Commands

	[ChatCommand("rusteditext.io.showiowires")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void IO_ShowIOWires(BasePlayer player)
	{
		player.ChatMessage("Showing IO Wires");
		IO_ShowWires(player);
	}

	[ChatCommand("rusteditext.io.reloaddoors")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void IO_ReloadDoor(BasePlayer player)
	{
		player.ChatMessage("Reloading Doors");
		IO_ReloadDoors(player);
	}

	#endregion

	#region Custom Hooks

	private object IPostSaveLoad()
	{
		if (IO_DirtyElevators())
		{
			SaveRestore.Save(true);
			System.Diagnostics.Process runProg = new System.Diagnostics.Process();
			try
			{
				runProg.StartInfo.FileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
				runProg.StartInfo.Arguments = Facepunch.CommandLine.Full;
				runProg.StartInfo.CreateNoWindow = false;
				runProg.Start();
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex);
			}
			Rust.Application.isQuitting = true;
			Network.Net.sv.Stop("Restarting");
			System.Diagnostics.Process.GetCurrentProcess().Kill();
			Rust.Application.Quit();
			return false;
		}

		return true;
	}
	private void IPostSaveSave()
	{
		foreach (var entity in IO_DestroyOnUnload)
		{
			if (BaseEntity.saveList.Contains(entity))
			{
				BaseEntity.saveList.Remove(entity);
			}
		}
	}
	private object ICanWireToolModifyEntity(BasePlayer player, BaseEntity entity)
	{
		if (player == null || entity == null || (player.IsAdmin && player.IsGod() && player.IsFlying))
		{
			return null;
		}
		if (IO_Protect.Contains(entity.transform.position))
		{
			return false;
		}

		return null;
	}
	private object IPreTurretTargetTick(AutoTurret turret)
	{
		if (turret == null || !IO_AutoTurrets.ContainsKey(turret))
		{
			return null;
		}
		if (UnityEngine.Time.realtimeSinceStartup >= turret.nextVisCheck)
		{
			turret.nextVisCheck = UnityEngine.Time.realtimeSinceStartup + UnityEngine.Random.Range(0.2f, 0.3f);

			if (turret.ObjectVisible(turret.target))
			{
				turret.lastTargetSeenTime = UnityEngine.Time.realtimeSinceStartup;
			}
		}
		if (turret.targetTrigger.entityContents != null)
		{
			foreach (var baseEntity in turret.targetTrigger.entityContents)
			{
				if (!(baseEntity == null))
				{
					var basePlayer = baseEntity as BasePlayer;
					if (basePlayer == null || (basePlayer.IsAdmin && basePlayer.IsGod() && basePlayer.IsFlying) || turret.IsAuthed(basePlayer) || (turret.PeacekeeperMode() && !basePlayer.IsHostile())) { continue; }
					if (!turret.target.IsNpc && turret.target.IsAlive() && turret.InFiringArc(turret.target) && turret.ObjectVisible(turret.target))
					{
						turret.SetTarget(basePlayer);
						break;
					}
				}
			}
			if (turret.target != null)
			{
				turret.EnsureReloaded(true);
				BaseProjectile attachedWeapon = turret.GetAttachedWeapon();
				if (UnityEngine.Time.time >= turret.nextShotTime && turret.ObjectVisible(turret.target) && Mathf.Abs(turret.AngleToTarget(turret.target)) < turret.GetMaxAngleForEngagement())
				{
					if (attachedWeapon)
					{
						if (attachedWeapon.primaryMagazine.contents > 0)
						{
							//unlimited ammo
							if (IO_AutoTurrets[turret])
							{
								attachedWeapon.primaryMagazine.contents = attachedWeapon.primaryMagazine.capacity;
								attachedWeapon.SendNetworkUpdateImmediate();
							}
							BasePlayer basePlayer = (turret.target as BasePlayer);
							if (basePlayer != null && basePlayer.IsAdmin && basePlayer.IsGod() && basePlayer.IsFlying)
							{
								return false;
							}
							if (turret.PeacekeeperMode() && !turret.target.IsHostile())
							{
								return false;
							}
							turret.FireAttachedGun(turret.AimOffset(turret.target), turret.aimCone, null, turret.PeacekeeperMode() ? turret.target : null);
							float num = attachedWeapon.isSemiAuto ? (attachedWeapon.repeatDelay * 1.5f) : attachedWeapon.repeatDelay;
							num = attachedWeapon.ScaleRepeatDelay(num);
							turret.nextShotTime = UnityEngine.Time.time + num;
						}
						else
						{
							turret.nextShotTime = UnityEngine.Time.time + 5f;
						}
					}
				}
			}
		}

		return null;
	}
	private object ICanDie(BaseCombatEntity entity, HitInfo info)
	{
		if (entity == null)
		{
			return null;
		}
		if (IO_Protect.Contains(entity.transform.position))
		{
			return false;
		}

		return null;
	}

	#endregion

	public class SerializedIOData
	{
		public List<SerializedIOEntity> entities = new();
	}

	public class SerializedConnectionData
	{
		public string fullPath;

		public VectorData position;

		public bool input;

		public int connectedTo;

		public int type;
	}

	public class SerializedIOEntity
	{
		public string fullPath;

		public VectorData position;

		public SerializedConnectionData[] inputs;

		public SerializedConnectionData[] outputs;

		public int accessLevel;

		public int doorEffect;

		public float timerLength;

		public int frequency;

		public bool unlimitedAmmo;

		public bool peaceKeeper;

		public string autoTurretWeapon;

		public int branchAmount;

		public int targetCounterNumber;

		public string rcIdentifier;

		public bool counterPassthrough;

		public int floors = 1;

		public string phoneName;
	}

	internal class AutoTurretManager : MonoBehaviour
	{
		public bool PeaceKeeper { get; private set; }
		private AutoTurret autoTurret;

		private void Awake()
		{
			autoTurret = GetComponent<AutoTurret>();
			enabled = false;
		}

		public void SetupTurret(bool UnlimitedAmmo, bool Peacekeeper, string GunName)
		{
			PeaceKeeper = Peacekeeper;
			autoTurret.SetPeacekeepermode(Peacekeeper);
			if (!string.IsNullOrEmpty(GunName))
			{
				Item item = ItemManager.CreateByName(GunName, 1, 0UL);
				if (!item.MoveToContainer(autoTurret.inventory, 0, false))
				{
					item.Remove();
				}
				autoTurret.inventory.MarkDirty();
				item.MarkDirty();
				if (!autoTurret.IsInvoking(new Action(autoTurret.UpdateAttachedWeapon)))
				{
					autoTurret.Invoke(new Action(autoTurret.UpdateAttachedWeapon), 0.5f);
				}
				autoTurret.Invoke(new Action(GiveAmmo), 1f);
				IO_AutoTurrets.Add(autoTurret, UnlimitedAmmo);
				if (!UnlimitedAmmo)
				{
					autoTurret.InvokeRepeating(() =>
					{
						if (!autoTurret.HasClipAmmo())
						{
							GiveAmmo();
						}
					}, 60, 60);
				}
			}
		}

		private void GiveAmmo()
		{
			if (autoTurret == null) { return; }
			var attachedWeapon = autoTurret.GetAttachedWeapon();
			if (attachedWeapon == null)
			{
				autoTurret.Invoke(() => GiveAmmo(), 0.2f);
				return;
			}
			autoTurret.inventory.AddItem(attachedWeapon.primaryMagazine.ammoType, attachedWeapon.primaryMagazine.capacity, 0uL);
			attachedWeapon.primaryMagazine.contents = attachedWeapon.primaryMagazine.capacity;
			attachedWeapon.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			autoTurret.Invoke(autoTurret.UpdateTotalAmmo, 0.25f);
		}

	}

	internal class CardReaderMonitor : MonoBehaviour
	{
		private CardReader cardReader;

		public float Timerlength = 0;

		private bool TimeOut;
		private void Awake()
		{
			cardReader = GetComponent<CardReader>();
			enabled = false;
			cardReader.Invoke(() => { TimerLength(); }, 1f);
		}

		private void Update()
		{
			if (!TimeOut && cardReader.HasFlag(BaseEntity.Flags.On))
			{
				TimeOut = true;
				cardReader.Invoke(new Action(RestState), Timerlength);
			}
		}

		public void TimerLength()
		{
			if (Timerlength < 0.01f)
			{
				UnityEngine.Object.Destroy(this);
			}
			else
			{
				RestState();
				enabled = true;
			}
		}

		private void RestState()
		{
			cardReader.ResetIOState();
			TimeOut = false;
		}
	}

	internal class IOEntityToWheelSwitchConnection : MonoBehaviour
	{
		private void Awake()
		{
			enabled = false;
			IOTrigger = GetComponent<IOEntity>();
			Triggered = ((IOTrigger is WheelSwitch) ? BaseEntity.Flags.Reserved1 : BaseEntity.Flags.On);
		}

		private void Update()
		{
			if (Running)
			{
				if (!IOTrigger.HasFlag(Triggered))
				{
					Running = false;
					for (int i = 0; i < ConnectedWheelSwitches.Count; i++)
					{
						WheelSwitch wheelSwitch = ConnectedWheelSwitches[i];
						if (wheelSwitch != null && wheelSwitch.IsInvoking(new Action(wheelSwitch.Powered)))
						{
							wheelSwitch.CancelInvoke(new Action(wheelSwitch.Powered));
						}
					}
				}
			}
			else if (IOTrigger.HasFlag(Triggered))
			{
				Running = true;
				for (int j = 0; j < ConnectedWheelSwitches.Count; j++)
				{
					WheelSwitch wheelSwitch2 = ConnectedWheelSwitches[j];
					if (wheelSwitch2 != null)
					{
						wheelSwitch2.InvokeRepeating(new Action(wheelSwitch2.Powered), 0f, 0.1f);
					}
				}
			}
		}

		public void SetTarget(WheelSwitch target)
		{
			if (!(target == null))
			{
				ConnectedWheelSwitches.Add(target);
				enabled = true;
			}
		}

		private IOEntity IOTrigger;

		private List<WheelSwitch> ConnectedWheelSwitches = new();

		private bool Running;

		private BaseEntity.Flags Triggered;
	}

	#endregion

	#region Deployables

	internal static bool Deployables_Loaded;
	internal static SerializedVehicleData Deployables_serializedVehicleData;
	internal static SerializedVendingContainerData Deployables_serializedVendingContainerData;
	internal static SerializedLootableContainerData Deployables_serializedLootableContainerData;
	internal static List<CustomVendingMachines> Deployables_customVendingMachine = new();
	internal static List<CustomLootContainer> Deployables_customLootContainer = new();
	internal static List<SpawnerHandler> Deployables_Spawners = new();
	internal static List<PrefabData> Deployables_AnimalsSpawners = new();
	internal static List<BaseEntity> Deployables_CoalingTower = new();
	internal static List<PrefabData> Deployables_SpawnPoints = new();
	internal static List<TakenV> Deployables_TakenVehicles = new();
	internal static List<PrefabData> Deployables_LootProfiles = new();
	internal static Coroutine Deployables_PrefabSpawnerThread;
	internal static Dictionary<string, byte[]> Deployables_CachedDownloads = new();
	internal static Dictionary<uint, uint> Deployables_PaintedSigns = new();

	#region Commands

	[ChatCommand("rusteditext.deployables.showanimalspawns")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void Deployables_ShowAnimalSpawns(BasePlayer player)
	{
		foreach (var pd in Deployables_AnimalsSpawners)
		{
			var pos = new Vector3(pd.position.x, pd.position.y, pd.position.z);
			var prefab = StringPool.Get(pd.id).Split('/');
			player.SendConsoleCommand("ddraw.sphere", 10f, UnityEngine.Color.white, pos, 1f);
			player.SendConsoleCommand("ddraw.text", 10f, UnityEngine.Color.white, pos, "<size=25>" + prefab.Last() + "</size>");
		}
		player.ChatMessage("Showing Animal Spawn Points.");
	}

	[ChatCommand("rusteditext.deployables.showlootspawns")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void Deployables_ShowLootSpawns(BasePlayer player)
	{
		foreach (var pd in Deployables_AnimalsSpawners)
		{
			var pos = new Vector3(pd.position.x, pd.position.y, pd.position.z);
			var prefab = StringPool.Get(pd.id).Split('/');
			player.SendConsoleCommand("ddraw.sphere", 10f, UnityEngine.Color.white, pos, 1f);
			player.SendConsoleCommand("ddraw.text", 10f, UnityEngine.Color.white, pos, "<size=25>" + prefab.Last() + "</size>");
		}
		player.ChatMessage("Showing Animal Spawn Points.");
	}

	[ChatCommand("rusteditext.deployables.showvendingspawns")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void Deployables_ShowVendingSpawns(BasePlayer player)
	{
		foreach (var pd in Deployables_customVendingMachine)
		{
			player.SendConsoleCommand("ddraw.sphere", 10f, UnityEngine.Color.white, pd.position, 1f);
			player.SendConsoleCommand("ddraw.text", 10f, UnityEngine.Color.white, pd.position, "<size=25>" + pd.vendingContainerData.filename + "</size>");
		}
		player.ChatMessage("Showing Custom Vending Spawn Points.");
	}

	[ChatCommand("rusteditext.deployables.showentityspawns")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void Deployables_ShowEntitySpawns(BasePlayer player)
	{
		foreach (var pd in Deployables_SpawnPoints)
		{
			var pos = new Vector3(pd.position.x, pd.position.y, pd.position.z);
			var prefab = StringPool.Get(pd.id).Split('/');
			player.SendConsoleCommand("ddraw.sphere", 10f, UnityEngine.Color.white, pos, 1f);
			player.SendConsoleCommand("ddraw.text", 10f, UnityEngine.Color.white, pos, "<size=25>" + prefab.Last() + "</size>");
		}
		player.ChatMessage("Showing Managed Entitys Spawn Points.");
	}

	#endregion

	#region Custom Hooks

	private object ICanCH47CreateMapMarker(CH47Helicopter heli)
	{
		if (Config.Deployables.EnableCH47MapMarker)
		{
			return null;
		}

		if (!Deployables_CreateMapMarkerHook(heli))
		{
			return false;
		}

		return null;
	}
	private void IPreObjectSetHierarchyGroup(GameObject @object, string root)
	{
		Deployables_WorldSpawnHook(@object, root);
	}
	private object ICanDoorManipulatorDoAction(DoorManipulator dm)
	{
		foreach (var coal in Deployables_CoalingTower)
		{
			if (Vector3.Distance(coal.transform.position, dm.transform.position) <= 30)
			{
				coal.SetFlag(BaseEntity.Flags.Reserved8, dm.IsPowered());
				return false;
			}
		}

		return null;
	}

	#endregion

	internal static bool Deployables_ShutdownHook()
	{
		foreach (KeyValuePair<uint, uint> oldsign in Deployables_PaintedSigns)
		{
			FileStorage.server.Remove(oldsign.Key, FileStorage.Type.png, oldsign.Value);
		}
		Debug.LogWarning("Cleaning Sign Data");
		return true;
	}
	internal static bool Deployables_InitializeHook()
	{
		try
		{
			if (Deployables_SpawnPoints != null && Deployables_SpawnPoints.Count != 0)
			{
				Singleton.PutsWarn($"Creating {Deployables_SpawnPoints.Count:n0}/{Deployables_Spawners.Count:n0} managed prefabs/spawners.");

				//Start thread.
				if (Deployables_PrefabSpawnerThread == null) { Deployables_PrefabSpawnerThread = ServerMgr.Instance.StartCoroutine(Deployables_SpawnManagerPrefabs()); }
			}
		}
		catch { }
		try
		{
			if (Deployables_customVendingMachine != null && Deployables_customVendingMachine.Count != 0)
			{
				for (int i = Deployables_customVendingMachine.Count - 1; i >= 0; i--)
				{
					CustomVendingMachines VendingMachine = Deployables_customVendingMachine[i];
					Deployables_StockVending(VendingMachine);
				}
				Singleton.PutsWarn($"Stocked {Deployables_customVendingMachine.Count:n0} custom vending-machines.");
			}
		}
		catch { }
		try
		{
			if (Deployables_customLootContainer != null && Deployables_customLootContainer.Count != 0)
			{
				Singleton.PutsWarn($"Filling {Deployables_customLootContainer.Count:n0} custom lootable containers.");
			}
		}
		catch { }
		try
		{
			if (Deployables_AnimalsSpawners != null && Deployables_AnimalsSpawners.Count != 0)
			{
				foreach (var pd in Deployables_AnimalsSpawners)
				{
					try
					{
						var baseEntity = GameManager.server.CreateEntity(StringPool.Get(pd.id), pd.position, pd.rotation, false);

						if (baseEntity != null)
						{
							baseEntity.enableSaving = false;
							baseEntity.gameObject.AwakeFromInstantiate();
							baseEntity.Spawn();

							var ass = baseEntity.gameObject.AddComponent<AnimalSpawner>();
							ass.PD = pd;
							ass.Lifetime = (float)Time.time + Singleton.Config.Deployables.AnimalsMinRespawnDelay;
							if (Singleton.Config.Deployables.LogManagedSpawns)
							{
								Singleton.PutsWarn($"Spawned {baseEntity} @ {baseEntity.transform.position}");
							}
						}
					}
					catch { }
				}

				Singleton.PutsWarn($"Set up {Deployables_AnimalsSpawners.Count:n0} animal spawners.");
			}
		}
		catch { }
		return true;
	}
	internal static bool Deployables_KillHook(BaseNetworkable __instance)
	{
		if (__instance == null) { return true; }
		if (__instance is SprayCanSpray_Decal) { return !(__instance as SprayCanSpray_Decal).HasFlag(BaseEntity.Flags.Placeholder); }
		if (__instance is Signage) { return !(__instance as Signage).HasFlag(BaseEntity.Flags.Placeholder); }
		return true;
	}
	internal static void Deployables_BuildPrefabList()
	{
		if (Deployables_serializedVehicleData == null)
		{
			byte[] XMLData = Deployables_GetSerializedData("SerializedVehicleData");
			if (XMLData != null) { Deployables_serializedVehicleData = Deployables_DeSerializePlugin<SerializedVehicleData>(XMLData); }
		}

		if (Deployables_serializedVehicleData != null) { Deployables_SpawnPoints.AddRange(Deployables_serializedVehicleData.vehicles); }
		for (int i = 0; i < World.Serialization.world.prefabs.Count; i++)
		{
			if (Singleton.Config.Deployables.ManagedPrefabs.Contains(StringPool.Get(World.Serialization.world.prefabs[i].id)))
			{
				Deployables_SpawnPoints.Add(World.Serialization.world.prefabs[i]);
			}
		}
		Singleton.PutsWarn($"Managed prefab list with {Deployables_SpawnPoints.Count:n0} spawners.");
		try
		{
			if (Deployables_serializedVendingContainerData == null)
			{
				byte[] XMLData = Deployables_GetSerializedData("SerializedVendingContainerData");
				//File.WriteAllBytes("SerializedVendingContainerData.xml", XMLData);
				if (XMLData != null) { Deployables_serializedVendingContainerData = Deployables_DeSerializePlugin<SerializedVendingContainerData>(XMLData); }
			}
			Singleton.PutsWarn($"Managed vending machines: {Deployables_serializedVendingContainerData.entities.Count:n0}");
		}
		catch { }
		try
		{
			if (Deployables_serializedLootableContainerData == null)
			{
				byte[] XMLData = Deployables_GetSerializedData("SerializedLootableContainerData");
				//File.WriteAllBytes("SerializedLootableContainerData.xml", XMLData);
				if (XMLData != null) { Deployables_serializedLootableContainerData = Deployables_DeSerializePlugin<SerializedLootableContainerData>(XMLData); }
			}
			Singleton.PutsWarn($"Managed lootable containers: {Deployables_serializedLootableContainerData.entities.Count:n0}");
		}
		catch { }
	}
	internal static string Deployables_GetContainerInfo(string cat)
	{
		string result;
		if (string.IsNullOrEmpty(cat)) { result = string.Empty; }
		else
		{
			string[] array = cat.Split(':');
			if (array.Length <= 2) { result = string.Empty; }
			else { result = array[3]; }
		}
		return result;
	}
	internal static byte[] Deployables_GetSerializedData(string Dataname)
	{
		foreach (MapData MD in World.Serialization.world.maps) { try { if (System.Text.Encoding.ASCII.GetString(MD.data).Contains(Dataname)) { return MD.data; } } catch { } }
		return null;
	}
	internal static int Deployables_SumbEntity(SpawnGroup.SpawnEntry spawnEntry) { return spawnEntry.weight; }
	internal static string Deployables_GetPrefabName(SpawnGroup spawnGroup)
	{
		float num = (float)spawnGroup.prefabs.Sum(new Func<SpawnGroup.SpawnEntry, int>(Deployables_SumbEntity));
		string result;
		if (num == 0f) { result = null; }
		else
		{
			float num2 = Random.Range(0f, num);
			foreach (SpawnGroup.SpawnEntry spawnEntry in spawnGroup.prefabs)
			{
				if ((num2 -= (float)spawnEntry.weight) <= 0f) { return spawnEntry.prefab.resourcePath; }
			}
			result = spawnGroup.prefabs[spawnGroup.prefabs.Count - 1].prefab.resourcePath;
		}
		return result;
	}
	internal static bool Deployables_ProtectedHook(BaseCombatEntity __instance, HitInfo info = null)
	{
		//Logic to stop pickup and decay.
		if (__instance == null) { return true; }
		if (Singleton.Config.Deployables.ManagedPrefabs.Contains(__instance.PrefabName))
		{
			if (Singleton.Config.Deployables.DisableDamageLikeRE)
			{
				//Blcosk all damage
				return false;
			}
			if (info == null)
			{
				//Blocks pickup
				return false;
			}
			if (info.damageTypes.GetMajorityDamageType() == Rust.DamageType.Decay || __instance.HasFlag(BaseEntity.Flags.Placeholder))
			{
				//Blocks decay and damage if has flag.
				return false;
			}
		}
		return true;
	}
	internal static void Deployables_WorldSpawnHook(GameObject gameObject, string strRoot)
	{
		if (gameObject != null)
		{
			//Card and ore spawners ect
			if (Singleton.Config.Deployables.ManagedSpawners.Contains(gameObject.name))
			{
				foreach (Transform child in gameObject.transform.GetAllChildren())
				{
					SpawnGroup component = child.GetComponent<SpawnGroup>();
					if (component != null)
					{
						if (component.WantsTimedSpawn()) { component.respawnDelayMax = float.PositiveInfinity; }
						Deployables_Spawners.Add(component.gameObject.AddComponent<SpawnerHandler>());
						return;
					}
				}
			}
			//Vending Machines
			NPCVendingMachine npcv = gameObject.GetComponent<NPCVendingMachine>();
			if (npcv != null)
			{
				foreach (PrefabData pd in Deployables_LootProfiles)
				{
					if (pd.position == npcv.transform.position)
					{
						VendingContainerData vendingContainerData = Deployables_serializedVendingContainerData.entities.Find((VendingContainerData x) => x.filename == Deployables_GetContainerInfo(pd.category));
						if (vendingContainerData != null)
						{
							Deployables_customVendingMachine.Add(new CustomVendingMachines(npcv, vendingContainerData));
							npcv.enableSaving = false;
							if (BaseEntity.saveList.Contains(npcv))
							{
								BaseEntity.saveList.Remove(npcv);
							}
							return;
						}
					}
				}
			}
			//Crates
			LootContainer loc = gameObject.GetComponent<LootContainer>();
			if (loc != null)
			{
				foreach (PrefabData pd in Deployables_LootProfiles)
				{
					if (pd.position == loc.transform.position)
					{
						LootableContainerData lootableContainerData = Deployables_serializedLootableContainerData.entities.Find((LootableContainerData x) => x.filename == Deployables_GetContainerInfo(pd.category));
						if (lootableContainerData == null)
						{
							Deployables_customLootContainer.Add(new CustomLootContainer(loc, null));
						}
						else
						{
							Deployables_customLootContainer.Add(new CustomLootContainer(loc, lootableContainerData));
						}

					}
				}
			}
		}
	}
	internal static  void Deployables_ClearContainer(LootContainer lootContainer)
	{
		if (lootContainer != null)
		{
			//Should loop until end but put 100 limit incase it gets stuck so doesnt lock server
			for (int i = 0; i < 100; i++)
			{
				ItemContainer inventory = lootContainer.inventory;
				bool flag;
				if (inventory == null)
				{
					break;
				}
				else
				{
					List<Item> itemList = inventory.itemList;
					int? num = (itemList != null) ? new int?(itemList.Count) : null;
					flag = (num.GetValueOrDefault() > 0 & num != null);
				}
				if (!flag)
				{
					break;
				}
				Item item = lootContainer.inventory.itemList[0];
				if (item != null)
				{
					item.RemoveFromContainer();
					item.Remove(0f);
				}
			}
		}
	}
	internal static  void Deployables_ContainerRespawner(LootContainer lootContainer, LootableContainerData lootableContainerData)
	{
		if (lootContainer != null && !lootContainer.IsDestroyed)
		{
			Deployables_ClearContainer(lootContainer);
			Deployables_AddContainerItems(lootContainer, lootableContainerData);
		}
	}
	internal static  void Deployables_AddContainerItems(LootContainer lootContainer, LootableContainerData lootableContainerData)
	{
		if (!(lootContainer == null) && !lootContainer.IsDestroyed)
		{
			lootContainer.Invoke(delegate ()
			{
				Deployables_ContainerRespawner(lootContainer, lootableContainerData);
			}, (float)Mathf.Clamp(UnityEngine.Random.Range(lootableContainerData.refreshRateMin, lootableContainerData.refreshRateMax) * 60, 60, int.MaxValue));
			int num = UnityEngine.Random.Range(lootableContainerData.spawnAmountMin, lootableContainerData.spawnAmountMax);
			lootContainer.inventory.ServerInitialize(null, num);
			lootContainer.inventory.GiveUID();
			List<LootableItemData> list = new List<LootableItemData>(lootableContainerData.items);
			for (int i = 0; i < num; i++)
			{
				LootableItemData lootableItemData = list.GetRandom<LootableItemData>();
				if (lootableItemData != null)
				{
					Item item;
					if (!lootableItemData.blueprint)
					{
						//Create New Item
						item = ItemManager.CreateByName(lootableItemData.shortname, Mathf.Clamp(UnityEngine.Random.Range(lootableItemData.minimum, lootableItemData.maximum), 1, int.MaxValue), 0UL);
					}
					else
					{
						//Create new blueprint
						item = ItemManager.CreateByName("blueprintbase", Mathf.Clamp(UnityEngine.Random.Range(lootableItemData.minimum, lootableItemData.maximum), 1, int.MaxValue), 0UL);
						ItemDefinition itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname == lootableItemData.shortname);
						item.blueprintTarget = ((itemDefinition != null) ? itemDefinition.itemid : 0);
					}
					if (item != null)
					{
						item.MoveToContainer(lootContainer.inventory);
					}
					list.Remove(lootableItemData);
				}
			}
		}
	}
	internal static  void Deployables_StockVending(CustomVendingMachines vendingm)
	{
		NPCVendingMachine npcvendingMachine = vendingm.npcVendingMachine;
		if (npcvendingMachine == null)
		{
			npcvendingMachine = Deployables_CreateVE<NPCVendingMachine>(vendingm.prefabName, vendingm.position, vendingm.rotation);
			npcvendingMachine.enableSaving = false;
			npcvendingMachine.Spawn();
			vendingm.npcVendingMachine = npcvendingMachine;
		}
		VendingContainerData vendingContainerData = vendingm.vendingContainerData;
		List<VendingItemData> list = new List<VendingItemData>(vendingContainerData.items);
		int num = (list.Count < 7) ? list.Count : 7;
		List<NPCVendingOrder.Entry> list2 = new List<NPCVendingOrder.Entry>();
		float[] array = new float[num];
		for (int i = 0; i < num; i++)
		{
			VendingItemData vendingItemData = list.GetRandom<VendingItemData>();
			if (vendingItemData != null)
			{
				list.Remove(vendingItemData);
				NPCVendingOrder.Entry item = new NPCVendingOrder.Entry
				{
					sellItem = ItemManager.FindItemDefinition(vendingItemData.sellItemShortname),
					sellItemAmount = vendingItemData.sellItemAmount,
					sellItemAsBP = vendingItemData.sellItemBlueprint,
					currencyItem = ItemManager.FindItemDefinition(vendingItemData.currencyItemShortname),
					currencyAmount = vendingItemData.currencyItemAmount,
					currencyAsBP = vendingItemData.currencyItemBlueprint,
					weight = vendingItemData.weight,
					refillDelay = 10f,
					refillAmount = 1
				};
				array[i] = Time.realtimeSinceStartup + 10f;
				list2.Add(item);
			}
		}
		npcvendingMachine.vendingOrders = ScriptableObject.CreateInstance<NPCVendingOrder>();
		npcvendingMachine.vendingOrders.orders = list2.ToArray();
		npcvendingMachine.refillTimes = array;
		npcvendingMachine.InstallFromVendingOrders();
	}
	internal static T Deployables_CreateVE<T>(string string_0, Vector3 vector3_0, Quaternion quaternion_0) where T : Component
	{
		GameObject gameObject = GameManager.server.FindPrefab(string_0);
		T result;
		if (gameObject == null)
		{
			result = default(T);
		}
		else
		{
			gameObject = Instantiate.GameObject(gameObject, vector3_0, quaternion_0);
			if (!(gameObject == null))
			{
				gameObject.name = string_0;
				UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject, Rust.Server.EntityScene);
				UnityEngine.Object.Destroy(gameObject.GetComponent<Spawnable>());
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(true);
				}
				T component = gameObject.GetComponent<T>();
				result = component;
			}
			else
			{
				result = default(T);
			}
		}
		return result;
	}
	internal static void Deployables_EntitySpawned(LootContainer __instance)
	{
		if (__instance == null) { return; }

		if (Singleton.Config.Deployables.LockedCrates.Contains(__instance.PrefabName))
		{
			__instance.InitializeHealth(Singleton.Config.Deployables.LockedCratesHealth, Singleton.Config.Deployables.LockedCratesHealth);
			__instance.SetFlag(BaseEntity.Flags.Locked, true);
		}
		foreach (CustomLootContainer clc in Deployables_customLootContainer)
		{
			if (clc == null) { continue; }
			if (clc.position == __instance.transform.position)
			{
				Deployables_ContainerRespawner(__instance, clc.lootableContainerData);
				break;
			}
		}
	}
	internal static bool Deployables_SpawnHook(PrefabData prefab)
	{
		if (!Deployables_Loaded)
		{
			Deployables_Loaded = true;
			Deployables_BuildPrefabList();
		}

		if (Singleton.Config.Deployables.RemovePrefabsLocations.Contains(prefab.position))
		{
			return false;
		}

		var info = Deployables_GetContainerInfo(prefab.category);

		if (!string.IsNullOrEmpty(info)) { Deployables_LootProfiles.Add(prefab); }

		switch (prefab.id)
		{
			case 273775677: //magnetcrane.entity.prefab
				return false;
			case 2047788867: //workcart_aboveground.entity.prefab
				return false;
			case 2138552575: //workcart_aboveground2.entity.prefab
				return false;
			case 612480793: //workcart.entity.prefab
				return false;
			case 1470785358: //trainwagonb.entity.prefab
				return false;
			case 382138391: //trainwagona.entity.prefab
				return false;
			case 1549305686: //trainwagonc.entity.prefab
				return false;
			case 2780689094: //trainwagond.entity.prefab
				return false;
			case 3542189355: //_basetrainwagon.entity.prefab
				return false;
			case 541826828: //assets/content/vehicles/locomotive/train.prefab
				return false;
			case 3185889399: //assets/content/vehicles/train/trainwagonunloadable.entity.prefab
				return false;
			case 1888107197: //assets/content/vehicles/train/trainwagonunloadablefuel.entity.prefab
				return false;
			case 3078064982: //assets/content/vehicles/train/trainwagonunloadableloot.entity.prefab
				return false;
			case 749308997: //polarbear.prefab
				Deployables_AnimalsSpawners.Add(prefab);
				return false;
			case 1799741974: //bear.prefab
				Deployables_AnimalsSpawners.Add(prefab);
				return false;
			case 1378621008: //stag.prefab
				Deployables_AnimalsSpawners.Add(prefab);
				return false;
			case 502341109: //boar.prefab
				Deployables_AnimalsSpawners.Add(prefab);
				return false;
			case 2144238755: //wolf.prefab
				Deployables_AnimalsSpawners.Add(prefab);
				return false;
			case 152398164: //chicken.prefab
				Deployables_AnimalsSpawners.Add(prefab);
				return false;
			case 3168507223: //- assets/prefabs/misc/xmas/neon_sign/sign.neon.xl.prefab
			case 708840119:  //- assets/prefabs/misc/xmas/neon_sign/sign.neon.xl.animated.prefab
			case 2628005754: //- assets/prefabs/misc/xmas/neon_sign/sign.neon.125x215.prefab
			case 3591916872: //- assets/prefabs/misc/xmas/neon_sign/sign.neon.125x215.animated.prefab
			case 3919686896: //- assets/prefabs/misc/xmas/neon_sign/sign.neon.125x125.prefab
				return false;

		}
		return true;
	}
	internal static bool Deployables_CreateMapMarkerHook(CH47Helicopter __instance)
	{
		if (__instance != null && Deployables_serializedVehicleData != null)
		{
			foreach (PrefabData pd in Deployables_serializedVehicleData.vehicles) { if (__instance.transform.position == pd.position) { return false; } }
		}
		return true;
	}
	internal static BaseEntity Deployables_ScanEntityList(Vector3 pos, uint ID)
	{
		foreach (BaseNetworkable bn in BaseNetworkable.serverEntities)
		{
			if (ID == 0 && bn.transform.position == pos) { return bn as BaseEntity; }
			if (bn.transform.position == pos && bn.prefabID == ID) { return bn as BaseEntity; }
		}
		return null;
	}
	internal static BaseEntity Deployables_FindEntity(Vector3 pos, float radius, uint ID)
	{
		//Scan area and return if something found with correct ID with in radius.
		List<BaseNetworkable> list = Pool.GetList<BaseNetworkable>();
		Vis.Entities<BaseNetworkable>(pos, radius, list, -1);
		foreach (BaseNetworkable baseentity in list) { if (baseentity.prefabID == ID) { try { return baseentity as BaseEntity; } catch { } } }
		Pool.FreeList<BaseNetworkable>(ref list);
		return null;
	}
	internal static  void Deployables_PairHangerDoor(Door door)
	{
		if (!door.HasFlag(BaseEntity.Flags.Reserved11))
		{
			List<DoorManipulator> list = new List<DoorManipulator>();
			Vis.Entities<DoorManipulator>(door.transform.position, Singleton.Config.Deployables.HangerDoorScanRange, list);
			if (list != null && list.Count != 0)
			{
				list[0].SetTargetDoor(door);
				if (Singleton.Config.Deployables.HideDoorManipulators)
				{
					list[0]._limitedNetworking = true;
				}
				door.SetFlag(BaseEntity.Flags.Reserved11, true);
			}
		}
	}
	internal static  void Deployables_PairDoor(Door door)
	{
		if (!door.HasFlag(BaseEntity.Flags.Reserved11))
		{
			List<DoorManipulator> list = new List<DoorManipulator>();
			Vis.Entities<DoorManipulator>(door.transform.position, 2f, list);
			if (list != null && list.Count != 0)
			{
				list[0].SetTargetDoor(door);
				if (Singleton.Config.Deployables.HideDoorManipulators)
				{
					list[0]._limitedNetworking = true;
				}
				door.SetFlag(BaseEntity.Flags.Locked, true);
				door.SetFlag(BaseEntity.Flags.Reserved11, true);
			}
		}
	}
	internal static  IEnumerator Deployables_SpawnManagerPrefabs()
	{
		bool HangerDoorsDone = false;
		bool FoundCoalings = false;
		bool DoneSigns = false;
		//Loop while there is a spawn list
		while (Deployables_SpawnPoints != null && Deployables_SpawnPoints.Count != 0)
		{
			//Do all in list
			foreach (PrefabData pd in Deployables_SpawnPoints)
			{
				try
				{
					bool spawn = true;
					string prefab = StringPool.Get(pd.id);
					BaseEntity be = null;
					//Different distance tollerance since some things could move.
					switch (prefab)
					{
						case string _ when prefab.Contains("coaling_tower_mechanism"):
							spawn = false;
							if (!FoundCoalings)
							{
								//Only find them once since they are indestructable
								be = Deployables_ScanEntityList(pd.position, pd.id);
								if (be != null)
								{
									Deployables_CoalingTower.Add(be);
								}
							}
							break;
						case string _ when prefab.Contains("door.hinged.arctic.garage"):
							spawn = false;
							if (!HangerDoorsDone)
							{
								//Only find them once since they are indestructable
								be = Deployables_ScanEntityList(pd.position, pd.id);
								Deployables_PairHangerDoor(be as Door);
							}
							break;
						case string _ when prefab.Contains("sign."):
							spawn = false;
							if (!DoneSigns)
							{
								be = Deployables_ScanEntityList(pd.position, pd.id);
							}
							break;
						case string _ when prefab.Contains("vehicles"):
						case string _ when prefab.Contains("ch47.entity"):
							be = Deployables_FindEntity(pd.position, 8f, pd.id);
							break;
						case string _ when prefab.Contains("boogieboard.deployed"):
							be = Deployables_FindEntity(pd.position, 4f, pd.id);
							break;
						//case string _ when prefab.Contains("wall.frame.garagedoor"):
						//    be = FindEntity(pd.position, 4.0f, pd.id);
						//    break;
						//case string _ when prefab.Contains("hinged"):
						//case string _ when prefab.Contains("junkpile"):
						//case string _ when prefab.Contains("wall.frame.shopfront"):
						//    be = FindEntity(pd.position, 3f, pd.id);
						//    break;
						//case string _ when prefab.Contains("halloween-bone-collectable"):
						//case string _ when prefab.Contains("dragondoorknocker"):
						//case string _ when prefab.Contains("skull_door_knocker"):
						//    be = ScanEntityList(pd.position, pd.id);
						//    break;
						//case string _ when prefab.Contains("doorgarland"):
						//case string _ when prefab.Contains("double_doorgarland"):
						//case string _ when prefab.Contains("windowgarland"):
						//    be = FindEntity(pd.position, 4.0f, pd.id);
						//    break;
						//case string _ when prefab.Contains("wall.external.high.stone"):
						//case string _ when prefab.Contains("deployed"):
						//case string _ when prefab.Contains("sign.post.town"):
						//    be = FindEntity(pd.position, 1.1f, pd.id);
						//    break;
						default:
							be = Deployables_ScanEntityList(pd.position, pd.id);
							break;
					}
					if (be == null)
					{
						//Nothing found so spawn it.
						try
						{
							if (prefab.Contains("cassetterecorder")) { continue; }
							if (!Deployables_AnyPlayersNearby(pd.position, Singleton.Config.Deployables.PlayerDistanceBlock))
							{
								foreach (TakenV ttv in Deployables_TakenVehicles.ToList())
								{
									if (ttv.baseent == null || ttv.baseent.IsDestroyed) { Deployables_TakenVehicles.Remove(ttv); }
									else if (ttv.spawndata == pd)
									{
										spawn = false;
										break;
									}
								}
								if (!spawn) { continue; }
								be = GameManager.server.CreateEntity(prefab, pd.position, Quaternion.Euler(pd.rotation)) as BaseEntity;
								if (be != null)
								{
									be.Spawn();
									if (Singleton.Config.Deployables.LogManagedSpawns)
									{
										Singleton.PutsWarn($"Spawned {be} @ {be.transform.position}");
									}
									//Ground doos so they dont fall apart.
									if (be is StabilityEntity) { (be as StabilityEntity).grounded = true; }
									UnityEngine.Object.Destroy(be.GetComponent<GroundWatch>());
									if (be is NeonSign)
									{
										foreach (var mesh in be.GetComponentsInChildren<MeshCollider>()) { UnityEngine.Object.DestroyImmediate(mesh); }
										NeonSign neon = be as NeonSign;
										if (neon != null)
										{
											neon.pickup.enabled = false;
											neon.currentFrame = 0;
											neon.animationSpeed = 1;
											byte[] Blank = Convert.FromBase64String(Singleton.Config.Deployables.DefaultSignImage);
											if (neon.prefabID == 708840119)
											{
												Deployables_ApplySignage(neon, null, Blank, 0);
												Deployables_ApplySignage(neon, null, Blank, 1);
												Deployables_ApplySignage(neon, null, Blank, 2);
												Deployables_ApplySignage(neon, null, Blank, 3);
												Deployables_ApplySignage(neon, null, Blank, 4);
											}
											else if (neon.prefabID == 3591916872)
											{
												Deployables_ApplySignage(neon, null, Blank, 0);
												Deployables_ApplySignage(neon, null, Blank, 1);
												Deployables_ApplySignage(neon, null, Blank, 2);
											}
											else
											{
												Deployables_ApplySignage(neon, null, Blank, 0);
											}
											neon.UpdateHasPower(100, 1);
											neon.SendNetworkUpdateImmediate(true);
										}
									}
									if (be.PrefabName.Contains("vehicles") || be.PrefabName.Contains("ch47.entity"))
									{
										TakenV tv = new TakenV();
										tv.baseent = be;
										tv.spawndata = pd;
										Deployables_TakenVehicles.Add(tv);
									}
									if (be is Door)
									{
										Door door = be as Door;
										//Pair Any Door Controllers
										door.canTakeLock = false;
										door.Invoke(() => { if (door != null) { Deployables_PairDoor(door); } }, 5f);
									}
									if (be.gameObject.GetComponent<NavMeshObstacle>() == null)
									{
										NavMeshObstacle nmo = be.gameObject.AddComponent<NavMeshObstacle>();
										nmo.carving = true;
									}
								}
							}
						}
						catch { }
					}
					if (be != null)
					{
						//Apply Skins
						if (Singleton.Config.Deployables.EnableMapSkins)
						{
							if (be.skinID == 0 && pd.category.Contains("skinid="))
							{
								try
								{
									string[] settings = pd.category.Split(new string[] { "skinid=" }, System.StringSplitOptions.RemoveEmptyEntries);
									string _skinid = settings[1].Split(':')[0];
									be.skinID = ulong.Parse(_skinid);
									be.SendNetworkUpdateImmediate();
									if (be is SprayCanSpray_Decal)
									{
										be.SetFlag(BaseEntity.Flags.Placeholder, true);
									}
								}
								catch
								{
								}
							}
						}
						//Apply Images
						if (Singleton.Config.Deployables.EnableImages)
						{
							PhotoFrame photoframe = be as PhotoFrame;
							if (photoframe != null)
							{
								Deployables_ProcessImages(photoframe, pd, DoneSigns);
							}
							else
							{
								Signage sign = be as Signage;
								if (sign != null)
								{
									Deployables_ProcessImages(sign, pd, DoneSigns);
								}
							}
						}
						if (Singleton.Config.Deployables.FillSwimmingPools)
						{
							if (be is PaddlingPool && !be.HasFlag(BaseEntity.Flags.Reserved4))
							{
								Item water = ItemManager.CreateByItemID(-1779180711, 2000);
								if (water != null)
								{
									water.info.itemType = ItemContainer.ContentsType.Liquid;
									if (water.MoveToContainer((be as PaddlingPool).inventory))
									{
										be.SetFlag(BaseEntity.Flags.Reserved4, true);
										be.SetFlag(BaseEntity.Flags.Locked, true);
									}
								}
							}
						}

						//If exsists
						BaseCombatEntity bce = be as BaseCombatEntity;
						if (bce != null)
						{
							//Add to protected list if its not already protected.
							bce.SetFlag(BaseEntity.Flags.Placeholder, true);
							//Disable pickup
							bce.pickup.enabled = false;
							if (bce.Health() < bce.MaxHealth())
							{
								//Heal a little bit if damaged
								bce.health += 5;
							}
						}
					}
				}
				catch { }
				//delay between next object to ease load on servers.
				yield return CoroutineEx.waitForSeconds(0.01f);
			}
			//Reduces load by only scanning hanger doors once.
			HangerDoorsDone = true;
			FoundCoalings = true;
			DoneSigns = true;
			//Recheck after X seconds.
			yield return CoroutineEx.waitForSeconds(Singleton.Config.Deployables.RecheckerSeconds);
		}
	}
	internal static void Deployables_ProcessImages(BaseEntity sign, PrefabData pd, bool DoneSigns)
	{
		PhotoFrame photoframe = sign as PhotoFrame;
		Signage signage = sign as Signage;
		if (!sign.HasFlag(BaseEntity.Flags.Placeholder))
		{
			sign.SetFlag(BaseEntity.Flags.Placeholder, true);
		}
		if ((!sign.HasFlag(BaseEntity.Flags.Locked) && pd.category.Contains("img=")) || (DoneSigns == false && pd.category.Contains("img=")))
		{
			sign.SetFlag(BaseEntity.Flags.Locked, true);
			try
			{
				int frames = 0;
				string[] settings = pd.category.Split(new string[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries)[0].Split(new string[] { "img=" }, System.StringSplitOptions.RemoveEmptyEntries);
				foreach (string s in settings)
				{
					try
					{
						if (Deployables_OnlyHexInString(s))
						{
							string imagelink = Encoding.UTF8.GetString(Deployables_StringToByteArray(s));
							int numframes = 0;
							if (signage != null)
							{
								numframes = signage.paintableSources.Length;
							}
							else if (photoframe != null)
							{
								numframes = 1;
							}

							if (imagelink.Contains(".png") || imagelink.Contains(".jpg") && frames < numframes)
							{
								if (!imagelink.StartsWith("http"))
								{
									imagelink = Singleton.Config.Deployables.URLPrefix + imagelink;
								}
								ServerMgr.Instance.StartCoroutine(Deployables_DownloadSignData(imagelink, signage, photoframe, frames++));
							}
							else if (imagelink.Contains(".gif"))
							{
								if (!imagelink.StartsWith("http"))
								{
									imagelink = Singleton.Config.Deployables.URLPrefix + imagelink;
								}
								ServerMgr.Instance.StartCoroutine(Deployables_DownloadSignData(imagelink, signage, null, 0, true));
							}
						}
					}
					catch { }
				}
			}
			catch
			{
			}
		}
	}
	internal static IEnumerator Deployables_DownloadSignData(string url, Signage sign, PhotoFrame photoframe, int frame, bool gif = false)
	{
		byte[] response;
		if (Deployables_CachedDownloads.Count == 0)
		{
			Singleton.PutsWarn($"Downloading and applying sign images.");
		}
		if (Deployables_CachedDownloads.ContainsKey(url)) { response = Deployables_CachedDownloads[url]; }
		else
		{
			try
			{
				response = new System.Net.WebClient().DownloadData(url);
				Deployables_CachedDownloads.Add(url, response);
			}
			catch
			{
				Singleton.PutsWarn($"Download failed: {url}");
				yield break;
			}
		}
		if (!gif)
		{
			Deployables_ApplySignage(sign, photoframe, response, frame);
		}
		else
		{
			if (!sign.HasComponent<GifPlayer>())
			{
				GifPlayer gifplayer = sign.gameObject.AddComponent<GifPlayer>();
				gifplayer.gif = new AnimatedGif(response);
				gifplayer.sign = sign;
			}
		}
		yield break;
	}
	internal static void Deployables_ApplySignage(Signage sign, PhotoFrame photoframe, byte[] imageBytes, int index)
	{
		//byte[] resizedImage;
		uint img;
		if (sign != null)
		{
			//  if (!_signSizes.ContainsKey(sign.ShortPrefabName)) { return; }
			// resizedImage = ImageResize(imageBytes, _signSizes[sign.ShortPrefabName].Width, _signSizes[sign.ShortPrefabName].Height);
			int size = Math.Max(sign.paintableSources.Length, 1);
			if (index >= size) { return; }
			if (sign.textureIDs == null || sign.textureIDs.Length != size) { Array.Resize(ref sign.textureIDs, size); }
			img = FileStorage.server.Store(imageBytes, FileStorage.Type.png, sign.net.ID);
			sign.textureIDs[index] = img;
			if (!Deployables_PaintedSigns.ContainsKey(img))
			{
				Deployables_PaintedSigns.Add(img, sign.net.ID);
			}
			sign.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
		else if (photoframe != null)
		{
			//  if (!_signSizes.ContainsKey(photoframe.ShortPrefabName)) { return; }
			if (index >= 1) { return; }
			//  resizedImage = ImageResize(imageBytes, _signSizes[photoframe.ShortPrefabName].Width, _signSizes[photoframe.ShortPrefabName].Height);
			img = FileStorage.server.Store(imageBytes, FileStorage.Type.png, photoframe.net.ID);
			photoframe._overlayTextureCrc = img;
			if (!Deployables_PaintedSigns.ContainsKey(img))
			{
				Deployables_PaintedSigns.Add(img, photoframe.net.ID);
			}
			photoframe.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}
	internal static bool Deployables_OnlyHexInString(string test)
	{
		return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
	}
	internal static byte[] Deployables_StringToByteArray(string hex)
	{
		return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
	}
	internal static bool Deployables_AnyPlayersNearby(Vector3 position, float maxDist)
	{
		//Disables respawning when player is near by.
		List<BasePlayer> list = Facepunch.Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(position, maxDist, list, 131072);
		bool result = false;
		foreach (BasePlayer basePlayer in list)
		{
			if (basePlayer.IsNpc) { continue; }
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive())
			{
				result = true;
				break;
			}
		}
		Facepunch.Pool.FreeList<BasePlayer>(ref list);
		return result;
	}
	internal static T Deployables_DeSerializePlugin<T>(byte[] bytes)
	{
		T result;
		try
		{
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				T t = (T)((object)xmlSerializer.Deserialize(memoryStream));
				result = t;
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message + " " + ex.StackTrace);
			result = default(T);
		}
		return result;
	}

	public class GifPlayer : MonoBehaviour
	{
		public AnimatedGif gif;
		public Signage sign;
		private float delay = 0;
		private int frame = 0;
		private List<uint> Images;
		private void Awake()
		{
			delay = Time.time + 5; //Setup start delay
		}

		public void FixedUpdate()
		{
			if (delay < Time.time)
			{
				return; //Cancel since not waited long enough
			}
			if (gif == null) { return; }
			if ((Images == null || Images.Count == 0) && gif.Images.Count != 0)
			{
				Images = new List<uint>();
				foreach (AnimatedGifFrame i in gif.Images)
				{
					uint img = FileStorage.server.Store(ImageToByteArray(i.Image), FileStorage.Type.png, sign.net.ID);
					if (!Deployables_PaintedSigns.ContainsKey(img)) { Deployables_PaintedSigns.Add(img, sign.net.ID); }
					Images.Add(img);
				}
			}
			if (frame >= gif.Images.Count - 1) { frame = 0; }
			delay = Time.time + gif.Images[frame].Duration; //Set up next scan
			sign.textureIDs[0] = Images[frame];
			sign.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			frame += 1;
		}
		public byte[] ImageToByteArray(System.Drawing.Image imageIn)
		{
			using (var ms = new MemoryStream())
			{
				imageIn.Save(ms, imageIn.RawFormat);
				return ms.ToArray();
			}
		}
		private void OnDestroy()
		{
			if (!Rust.Application.isQuitting)
			{
				//If game not destrying on quit then setup respawn.
			}
		}
	}
	public class AnimatedGif
	{
		private List<AnimatedGifFrame> mImages = new();
		public AnimatedGif(byte[] Image)
		{
			var img = new Bitmap(new MemoryStream(Image));
			int frames = img.GetFrameCount(FrameDimension.Time);
			if (frames <= 1) { return; }
			byte[] times = img.GetPropertyItem(0x5100).Value;
			int frame = 0;
			for (; ; )
			{
				int dur = BitConverter.ToInt32(times, 4 * frame);
				mImages.Add(new AnimatedGifFrame(new Bitmap(img), dur));
				if (++frame >= frames) break;
				img.SelectActiveFrame(FrameDimension.Time, frame);
			}
			img.Dispose();
		}
		public List<AnimatedGifFrame> Images { get { return mImages; } }
	}
	public class AnimatedGifFrame
	{
		private int mDuration;
		private Image mImage;
		internal AnimatedGifFrame(Image img, int duration)
		{
			mImage = img; mDuration = duration;
		}
		public Image Image { get { return mImage; } }
		public int Duration { get { return mDuration; } }
	}
	public class SignSize
	{
		public int Width;
		public int Height;
		public int ImageWidth;
		public int ImageHeight;
		public SignSize(int width, int height)
		{
			Width = width;
			Height = height;
			ImageWidth = width;
			ImageHeight = height;
		}
	}
	public class CustomLootContainer
	{
		public CustomLootContainer(LootContainer lootContainer0, LootableContainerData lootableContainerData0)
		{
			lootContainer = lootContainer0;
			prefabName = lootContainer.PrefabName;
			position = lootContainer.transform.position;
			rotation = lootContainer.transform.rotation;
			lootableContainerData = lootableContainerData0;
		}
		public string prefabName;
		public Vector3 position;
		public Quaternion rotation;
		public LootContainer lootContainer;
		public LootableContainerData lootableContainerData;
	}
	public class CustomVendingMachines
	{
		public CustomVendingMachines(NPCVendingMachine npcVendingMachine0, VendingContainerData vendingContainerData0)
		{
			npcVendingMachine = npcVendingMachine0;
			prefabName = npcVendingMachine.PrefabName;
			position = npcVendingMachine.transform.position;
			rotation = npcVendingMachine.transform.rotation;
			vendingContainerData = vendingContainerData0;
		}
		public string prefabName;
		public Vector3 position;
		public Quaternion rotation;
		public NPCVendingMachine npcVendingMachine;
		public VendingContainerData vendingContainerData;
	}
	public class SerializedVehicleData { public List<PrefabData> vehicles = new(); }
	public class TakenV
	{
		public BaseEntity baseent;
		public PrefabData spawndata;
	}
	public class SerializedLootableContainerData
	{
		public List<LootableContainerData> entities = new();
	}
	public class LootableContainerData
	{
		public string filename = string.Empty;
		public List<LootableItemData> items = new();
		public int respawnRateMin = 1;
		public int respawnRateMax = 1;
		public int refreshRateMin = 1;
		public int refreshRateMax = 1;
		public int spawnAmountMin = 1;
		public int spawnAmountMax = 1;
	}
	public class LootableItemData
	{
		public string shortname;
		public int minimum;
		public int maximum;
		public bool blueprint;
	}
	public class SerializedVendingContainerData
	{
		public List<VendingContainerData> entities = new();
	}
	public class VendingContainerData
	{
		public string filename = string.Empty;
		public List<VendingItemData> items = new();
	}
	public class VendingItemData
	{
		public string sellItemShortname;
		public int sellItemAmount;
		public bool sellItemBlueprint;
		public string currencyItemShortname;
		public int currencyItemAmount;
		public bool currencyItemBlueprint;
		public int weight;
	}
	public class AnimalSpawner : MonoBehaviour
	{
		public PrefabData PD;
		public float Lifetime;

		public void Spawn(BaseEntity baseEntity)
		{
			if (baseEntity)
			{
				if (!Deployables_AnyPlayersNearby(PD.position, Singleton.Config.Deployables.PlayerDistanceBlockAnimals) && (Lifetime <= UnityEngine.Time.time))
				{
					baseEntity.enableSaving = false;
					baseEntity.gameObject.AwakeFromInstantiate();
					baseEntity.Spawn();
					AnimalSpawner ass = baseEntity.gameObject.AddComponent<AnimalSpawner>();
					ass.PD = PD;
					ass.Lifetime = (float)UnityEngine.Time.time + Singleton.Config.Deployables.AnimalsMinRespawnDelay;
					return;
				}
				baseEntity.Invoke(() => { Spawn(baseEntity); }, 30f);
			}
		}

		private void OnDestroy()
		{
			if (!Rust.Application.isQuitting)
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(StringPool.Get(PD.id), PD.position, PD.rotation, false);
				if (baseEntity)
				{
					Spawn(baseEntity);
					if (Singleton.Config.Deployables.LogManagedSpawns)
					{
						Singleton.PutsWarn($"Spawned {baseEntity} @ {baseEntity.transform.position}");
					}
				}
			}
		}
	}
	public class SpawnerHandler : MonoBehaviour
	{
		private SpawnGroup spawnGroup;
		private BaseSpawnPoint baseSpawnPoint;
		internal bool flag = false;

		private class SpawnManager : MonoBehaviour
		{
			private void OnDestroy()
			{
				if (!Rust.Application.isQuitting)
				{
					//If game not destrying on quit then setup respawn.
					spawnmanager.flag = false;
					spawnmanager.SetSpawn();
				}
			}
			public SpawnerHandler spawnmanager;
		}
		private void Awake()
		{
			spawnGroup = GetComponent<SpawnGroup>();
			baseSpawnPoint = GetComponentInChildren<BaseSpawnPoint>();
			spawnGroup.Clear();
			CreateEntity();
		}

		internal void SetSpawn()
		{
			if (!flag)
			{
				//Invoke delay to respawn
				InvokeHandler.Invoke(this, new Action(CreateEntity), Singleton.Config.Deployables.KeyCardsRespawnSeconds);
			}
		}

		private void CreateEntity()
		{
			baseSpawnPoint.GetLocation(out var pos, out var rot);

			var baseEntity = GameManager.server.CreateEntity(Deployables_GetPrefabName(spawnGroup), pos, rot, false);
			if (baseEntity)
			{
				baseEntity.enableSaving = false;
				baseEntity.gameObject.AwakeFromInstantiate();
				baseEntity.Spawn();
				baseEntity.gameObject.AddComponent<SpawnManager>().spawnmanager = this;
				flag = true;
				if (Singleton.Config.Deployables.LogManagedSpawns)
				{
					Singleton.PutsWarn($"Spawned {baseEntity} @ {baseEntity.transform.position}");
				}
			}
		}
	}

	#endregion
}

public class RustEditConfig
{
	public NpcSettings NpcSpawner = new();
	public DeployablesSettings Deployables = new();

	public class NpcSettings
	{
		public bool UseRustEditNPCLogic = false;
		public float AITickRate = 2f;
		public float SpawnBlockingDistance = 30f;
		public bool APCIgnoreNPCs = true;
		public bool NPCAutoTurretsIgnoreNPCs = true;
		public float ScientistNPCHealth = 120;
		public float ScientistNPCAimMultiplyer = 0.8f;
		public float ScientistNPCRoamRange = 50;
		public bool ScientistNPCIgnoreSafeZonePlayers = true;
		public bool ScientistNPCCheckLOS = true;
		public float ScientistNPCDamageScale = 0.4f;
		public float ScientistNPCSenseRange = 40f;
		public float ScientistNPCMemoryDuration = 1f;
		public float ScientistNPCTargetLostRange = 80f;
		public float ScientistNPCAttackRangeMultiplier = 1f;
		public float PeacekeeperNPCHealth = 120;
		public float PeacekeeperNPCAimMultiplyer = 0.8f;
		public float PeacekeeperNPCRoamRange = 30;
		public bool PeacekeeperNPCIgnoreSafeZonePlayers = true;
		public bool PeacekeeperNPCCheckLOS = true;
		public float PeacekeeperNPCDamageScale = 0.4f;
		public float PeacekeeperNPCSenseRange = 30f;
		public float PeacekeeperNPCMemoryDuration = 1f;
		public float PeacekeeperNPCTargetLostRange = 80f;
		public float PeacekeeperNPCAttackRangeMultiplier = 1f;
		public float HeavyScientistNPCHealth = 190;
		public float HeavyScientistNPCAimMultiplyer = 0.8f;
		public float HeavyScientistNPCRoamRange = 50;
		public bool HeavyScientistNPCIgnoreSafeZonePlayers = true;
		public bool HeavyScientistNPCCheckLOS = true;
		public float HeavyScientistNPCDamageScale = 0.6f;
		public float HeavyScientistNPCSenseRange = 30f;
		public float HeavyScientistNPCMemoryDuration = 1f;
		public float HeavyScientistNPCTargetLostRange = 80f;
		public float HeavyScientistNPCAttackRangeMultiplier = 1f;
		public float JunkpileScientistNPCHealth = 120;
		public float JunkpileScientistNPCAimMultiplyer = 0.8f;
		public float JunkpileScientistNPCRoamRange = 50;
		public bool JunkpileScientistNPCIgnoreSafeZonePlayers = true;
		public bool JunkpileScientistNPCCheckLOS = true;
		public float JunkpileScientistNPCDamageScale = 0.4f;
		public float JunkpileScientistNPCSenseRange = 20f;
		public float JunkpileScientistNPCMemoryDuration = 1f;
		public float JunkpileScientistNPCTargetLostRange = 80f;
		public float JunkpileScientistNPCAttackRangeMultiplier = 1f;
		public float BanditNPCHealth = 120;
		public float BanditNPCAimMultiplyer = 0.8f;
		public float BanditNPCRoamRange = 50;
		public bool BanditNPCIgnoreSafeZonePlayers = true;
		public bool BanditNPCCheckLOS = true;
		public float BanditNPCDamageScale = 0.4f;
		public float BanditNPCSenseRange = 40f;
		public float BanditNPCMemoryDuration = 1f;
		public float BanditNPCTargetLostRange = 80f;
		public float BanditNPCAttackRangeMultiplier = 1f;
		public float MurdererNPCHealth = 100;
		public float MurdererNPCRoamRange = 30;
		public bool MurdererNPCIgnoreSafeZonePlayers = true;
		public bool MurdererNPCCheckLOS = true;
		public float MurdererNPCDamageScale = 0.4f;
		public float MurdererNPCSenseRange = 20f;
		public float MurdererNPCMemoryDuration = 1f;
		public float MurdererNPCTargetLostRange = 40f;
		public float MurdererNPCAttackRangeMultiplier = 1f;
		public float ScarecrowNPCHealth = 60;
		public float ScarecrowNPCRoamRange = 30;
		public bool ScarecrowNPCIgnoreSafeZonePlayers = true;
		public bool ScarecrowNPCCheckLOS = true;
		public float ScarecrowNPCDamageScale = 0.4f;
		public float ScarecrowNPCSenseRange = 20f;
		public float ScarecrowNPCMemoryDuration = 1f;
		public float ScarecrowNPCTargetLostRange = 40f;
		public float ScarecrowNPCAttackRangeMultiplier = 1f;
	}
	public class DeployablesSettings
	{
		public float HangerDoorScanRange = 5f;
		public int RecheckerSeconds = 720;
		public int KeyCardsRespawnSeconds = 1200;
		public int PlayerDistanceBlock = 80;
		public int PlayerDistanceBlockAnimals = 100;
		public int AnimalsMinRespawnDelay = 300;
		public bool DisableDamageLikeRE = false;
		public bool EnableCH47MapMarker = false;
		public bool HideDoorManipulators = true;
		public bool FillSwimmingPools = true;
		public bool EnableMapSkins = true;
		public bool EnableImages = true;
		public string URLPrefix = "http://";
		public bool LogManagedSpawns = false;
		public int LockedCratesHealth = 100;
		public List<Vector3> RemovePrefabsLocations = new() { new Vector3(0, -499, 0) };
		public List<string> LockedCrates = new()
		{
			"assets/bundled/prefabs/radtown/dmloot/dm tier3 lootbox.prefab",
			"assets/bundled/prefabs/radtown/dmloot/dm tier2 lootbox.prefab",
			"assets/bundled/prefabs/radtown/dmloot/dm tier1 lootbox.prefab",
			"assets/bundled/prefabs/radtown/dmloot/dm res.prefab",
			"assets/bundled/prefabs/radtown/dmloot/dm food.prefab",
			"assets/bundled/prefabs/radtown/dmloot/dm construction tools.prefab",
			"assets/bundled/prefabs/radtown/dmloot/dm construction resources.prefab",
			"assets/bundled/prefabs/radtown/dmloot/dm medical.prefab",
			"assets/bundled/prefabs/radtown/dmloot/dm ammo.prefab",
			"assets/bundled/prefabs/radtown/dmloot/dm c4.prefab"
		};
		public List<string> ManagedSpawners = new()
		{
			"assets/bundled/prefabs/hapis/desk_greencard_hapis.prefab",
			"assets/bundled/prefabs/radtown/desk_bluecard.prefab",
			"assets/bundled/prefabs/radtown/desk_greencard.prefab",
			"assets/bundled/prefabs/radtown/desk_redcard.prefab",
			"assets/bundled/prefabs/modding/lootables/red_card_spawner.prefab",
			"assets/bundled/prefabs/modding/lootables/green_card_spawner.prefab",
			"assets/bundled/prefabs/modding/lootables/blue_card_spawner.prefab",
		};
		public List<string> ManagedPrefabs = new()
		{
			"assets/prefabs/building/wall.window.bars/wall.window.bars.metal.prefab",
			"assets/prefabs/misc/xmas/neon_sign/sign.neon.xl.prefab",
			"assets/prefabs/misc/xmas/neon_sign/sign.neon.xl.animated.prefab",
			"assets/prefabs/misc/xmas/neon_sign/sign.neon.125x215.prefab",
			"assets/prefabs/misc/xmas/neon_sign/sign.neon.125x215.animated.prefab",
			"assets/prefabs/misc/xmas/neon_sign/sign.neon.125x125.prefab",
			"assets/prefabs/misc/summer_dlc/photoframe/photoframe.landscape.prefab",
			"assets/prefabs/misc/summer_dlc/photoframe/photoframe.large.prefab",
			"assets/prefabs/misc/summer_dlc/photoframe/photoframe.portrait.prefab",
			"assets/prefabs/deployable/signs/sign.small.wood.prefab",
			"assets/prefabs/deployable/signs/sign.post.town.prefab",
			"assets/prefabs/deployable/signs/sign.post.single.prefab",
			"assets/prefabs/deployable/signs/sign.post.double.prefab",
			"assets/prefabs/deployable/signs/sign.pole.banner.large.prefab",
			"assets/prefabs/deployable/signs/sign.hanging.banner.large.prefab",
			"assets/prefabs/deployable/signs/sign.hanging.ornate.prefab",
			"assets/prefabs/deployable/signs/sign.hanging.prefab",
			"assets/prefabs/deployable/signs/sign.huge.wood.prefab",
			"assets/prefabs/deployable/signs/sign.large.wood.prefab",
			"assets/prefabs/deployable/signs/sign.medium.wood.prefab",
			"assets/prefabs/deployable/signs/sign.pictureframe.landscape.prefab",
			"assets/prefabs/deployable/signs/sign.pictureframe.portrait.prefab",
			"assets/prefabs/deployable/signs/sign.pictureframe.tall.prefab",
			"assets/prefabs/deployable/signs/sign.pictureframe.xl.prefab",
			"assets/prefabs/deployable/signs/sign.pictureframe.xxl.prefab",
			"assets/prefabs/building/wall.window.embrasure/shutter.metal.embrasure.a.prefab",
			"assets/prefabs/building/wall.window.embrasure/shutter.metal.embrasure.b.prefab",
			"assets/prefabs/building/wall.window.shutter/shutter.wood.a.prefab",
			"assets/prefabs/building/floor.ladder.hatch/floor.ladder.hatch.prefab",
			"assets/prefabs/building/floor.triangle.ladder.hatch/floor.triangle.ladder.hatch.prefab",
			"assets/scenes/prefabs/trainyard/coaling_tower_mechanism.entity.prefab",
			"assets/content/vehicles/train/trainwagonunloadable.entity.prefab",
			"assets/content/vehicles/train/trainwagonunloadablefuel.entity.prefab",
			"assets/content/vehicles/train/trainwagonunloadableloot.entity.prefab",
			"assets/prefabs/tools/spraycan/sprays/spray.decal.prefab",
			"assets/prefabs/building/wall.frame.cell/wall.frame.cell.gate.prefab",
			"assets/prefabs/building/wall.frame.cell/wall.frame.cell.prefab",
			"assets/prefabs/building/wall.frame.fence/wall.frame.fence.gate.prefab",
			"assets/prefabs/building/wall.frame.shopfront/wall.frame.shopfront.prefab",
			"assets/content/building/parts/static/wall.frame.shopfront_static.prefab",
			"assets/content/building/parts/static/wall.frame.shopfront.door_static.prefab",
			"assets/prefabs/building/wall.frame.shopfront/wall.frame.shopfront.metal.prefab",
			"assets/prefabs/building/wall.frame.netting/wall.frame.netting.prefab",
			"assets/prefabs/building/wall.frame.fence/wall.frame.fence.prefab",
			"assets/content/vehicles/train/trainwagonb.entity.prefab",
			"assets/content/vehicles/train/trainwagona.entity.prefab",
			"assets/content/vehicles/train/trainwagonc.entity.prefab",
			"assets/content/vehicles/train/_basetrainwagon.entity.prefab",
			"assets/content/vehicles/train/trainwagond.entity.prefab",
			"assets/content/vehicles/workcart/workcart_aboveground.entity.prefab",
			"assets/content/vehicles/workcart/workcart_aboveground2.entity.prefab",
			"assets/content/vehicles/workcart/workcart.entity.prefab",
			"assets/content/vehicles/mlrs/mlrs.entity.prefab",
			"assets/content/structures/carshredder/carshredder.entity.prefab",
			"assets/content/structures/arctic_base_modules/door.hinged.arctic.garage.prefab",
			"assets/prefabs/building/wall.frame.garagedoor/wall.frame.garagedoor.prefab",
			"assets/bundled/prefabs/static/door.hinged.bunker.door.prefab",
			"assets/bundled/prefabs/static/door.hinged.bunker_hatch.prefab",
			"assets/bundled/prefabs/static/door.hinged.cargo_ship.prefab",
			"assets/bundled/prefabs/static/door.hinged.elevator_door.prefab",
			"assets/bundled/prefabs/static/door.hinged.garage_a.prefab",
			"assets/bundled/prefabs/static/door.hinged.industrial_a_a.prefab",
			"assets/bundled/prefabs/static/door.hinged.industrial_a_b.prefab",
			"assets/bundled/prefabs/static/door.hinged.industrial_a_c.prefab",
			"assets/bundled/prefabs/static/door.hinged.industrial_a_d.prefab",
			"assets/bundled/prefabs/static/door.hinged.industrial_a_e.prefab",
			"assets/bundled/prefabs/static/door.hinged.industrial_a_f.prefab",
			"assets/bundled/prefabs/static/door.hinged.industrial_a_g.prefab",
			"assets/bundled/prefabs/static/door.hinged.industrial_a_h.prefab",
			"assets/bundled/prefabs/static/door.hinged.security.blue.prefab",
			"assets/bundled/prefabs/static/door.hinged.security.green.prefab",
			"assets/bundled/prefabs/static/door.hinged.security.red.prefab",
			"assets/bundled/prefabs/static/door.hinged.underwater_labs.public.prefab",
			"assets/bundled/prefabs/static/door.hinged.underwater_labs.security.prefab",
			"assets/bundled/prefabs/static/door.hinged.vent.prefab",
			"assets/bundled/prefabs/static/door.hinged.wood.static.prefab",
			"assets/prefabs/building/gates.external.high/gates.external.high.wood/gates.external.high.wood.prefab",
			"assets/prefabs/building/wall.external.high.wood/wall.external.high.wood.prefab",
			"assets/prefabs/building/gates.external.high/gates.external.high.stone/gates.external.high.stone.prefab",
			"assets/prefabs/building/wall.external.high.stone/wall.external.high.stone.prefab",
			"assets/prefabs/misc/easter/painted eggs/collectableegg.prefab",
			"assets/content/vehicles/crane_magnet/magnetcrane.entity.prefab",
			"assets/prefabs/building/door.double.hinged/door.double.hinged.toptier.prefab",
			"assets/prefabs/building/door.double.hinged/door.double.hinged.wood.prefab",
			"assets/prefabs/building/door.hinged/door.hinged.metal.prefab",
			"assets/prefabs/building/door.hinged/door.hinged.toptier.prefab",
			"assets/prefabs/building/door.hinged/door.hinged.wood.prefab",
			"assets/prefabs/misc/permstore/factorydoor/door.hinged.industrial.d.prefab",
			"assets/prefabs/misc/twitch/industrialdoora/door.hinged.industrial.a.prefab",
			"assets/content/vehicles/scrap heli carrier/scraptransporthelicopter.prefab",
			"assets/content/vehicles/minicopter/minicopter.entity.prefab",
			"assets/content/vehicles/submarine/submarineduo.entity.prefab",
			"assets/content/vehicles/submarine/submarinesolo.entity.prefab",
			"assets/content/vehicles/boats/rhib/rhib.prefab",
			"assets/content/vehicles/boats/rowboat/rowboat.prefab",
			"assets/content/vehicles/snowmobiles/tomahasnowmobile.prefab",
			"assets/content/vehicles/snowmobiles/snowmobile.prefab",
			"assets/rust.ai/nextai/testridablehorse.prefab",
			"assets/prefabs/deployable/barricades/barricade.wood.prefab",
			"assets/prefabs/deployable/barricades/barricade.woodwire.prefab",
			"assets/prefabs/deployable/barricades/barricade.stone.prefab",
			"assets/prefabs/deployable/barricades/barricade.sandbags.prefab",
			"assets/prefabs/deployable/barricades/barricade.metal.prefab",
			"assets/prefabs/deployable/barricades/barricade.concrete.prefab",
			"assets/prefabs/deployable/tier 1 workbench/workbench1.deployed.prefab",
			"assets/prefabs/deployable/tier 2 workbench/workbench2.deployed.prefab",
			"assets/prefabs/deployable/tier 3 workbench/workbench3.deployed.prefab",
			"assets/prefabs/deployable/bbq/bbq.deployed.prefab",
			"assets/prefabs/deployable/table/table.deployed.prefab",
			"assets/prefabs/deployable/bed/bed_deployed.prefab",
			"assets/prefabs/deployable/survivalfishtrap/survivalfishtrap.deployed.prefab",
			"assets/prefabs/deployable/chair/chair.deployed.prefab",
			"assets/prefabs/deployable/computerstation/computerstation.deployed.prefab",
			"assets/prefabs/deployable/door barricades/door_barricade_a.prefab",
			"assets/prefabs/deployable/door barricades/door_barricade_a_large.prefab",
			"assets/prefabs/deployable/dropbox/dropbox.deployed.prefab",
			"assets/prefabs/deployable/fireplace/fireplace.deployed.prefab",
			"assets/prefabs/deployable/frankensteintable/frankensteintable.deployed.prefab",
			"assets/prefabs/deployable/fridge/fridge.deployed.prefab",
			"assets/prefabs/deployable/furnace.large/furnace.large.prefab",
			"assets/prefabs/deployable/furnace/furnace.prefab",
			"assets/prefabs/deployable/hitch & trough/hitchtrough.deployed.prefab",
			"assets/prefabs/deployable/lantern/lantern.deployed.prefab",
			"assets/prefabs/voiceaudio/soundlight/soundlight.deployed.prefab",
			"assets/prefabs/voiceaudio/microphonestand/microphonestand.deployed.prefab",
			"assets/prefabs/voiceaudio/laserlight/laserlight.deployed.prefab",
			"assets/prefabs/voiceaudio/hornspeaker/connectedspeaker.deployed.prefab",
			"assets/prefabs/npc/flame turret/flameturret.deployed.prefab",
			"assets/prefabs/deployable/rug/rug.deployed.prefab",
			"assets/prefabs/deployable/research table/researchtable_deployed.prefab",
			"assets/prefabs/deployable/secretlab chair/secretlabchair.deployed.prefab",
			"assets/prefabs/deployable/single shot trap/guntrap.deployed.prefab",
			"assets/prefabs/deployable/sleeping bag/sleepingbag_leather_deployed.prefab",
			"assets/prefabs/deployable/sofa/sofa.deployed.prefab",
			"assets/prefabs/deployable/spinner_wheel/spinner.wheel.deployed.prefab",
			"assets/prefabs/deployable/tool cupboard/cupboard.tool.deployed.prefab",
			"assets/prefabs/deployable/tuna can wall lamp/tunalight.deployed.prefab",
			"assets/prefabs/deployable/waterpurifier/waterpurifier.deployed.prefab",
			"assets/prefabs/deployable/woodenbox/woodbox_deployed.prefab",
			"assets/prefabs/instruments/drumkit/drumkit.deployed.prefab",
			"assets/prefabs/deployable/mailbox/mailbox.deployed.prefab",
			"assets/prefabs/deployable/mixingtable/mixingtable.deployed.prefab",
			"assets/prefabs/deployable/planters/planter.small.deployed.prefab",
			"assets/prefabs/deployable/planters/planter.large.deployed.prefab",
			"assets/prefabs/deployable/oil refinery/refinery_small_deployed.prefab",
			"assets/prefabs/deployable/sofa/sofa.pattern.deployed.prefab",
			"assets/prefabs/deployable/repair bench/repairbench_deployed.prefab",
			"assets/prefabs/deployable/locker/locker.deployed.prefab",
			"assets/prefabs/deployable/small stash/small_stash_deployed.prefab",
			"assets/prefabs/instruments/xylophone/xylophone.deployed.prefab",
			"assets/prefabs/misc/chinesenewyear/chineselantern/chineselantern.deployed.prefab",
			"assets/prefabs/misc/chinesenewyear/dragondoorknocker/dragondoorknocker.deployed.prefab",
			"assets/prefabs/misc/chinesenewyear/newyeargong/newyeargong.deployed.prefab",
			"assets/prefabs/misc/chinesenewyear/sky_lantern/skylantern.deployed.prefab",
			"assets/prefabs/misc/chinesenewyear/throwablefirecrackers/firecrackers.deployed.prefab",
			"assets/prefabs/misc/easter/door_wreath/easter_door_wreath_deployed.prefab",
			"assets/prefabs/misc/easter/faberge_egg_a/rustigeegg_a.deployed.prefab",
			"assets/prefabs/misc/easter/faberge_egg_b/rustigeegg_b.deployed.prefab",
			"assets/prefabs/misc/easter/faberge_egg_c/rustigeegg_c.deployed.prefab",
			"assets/prefabs/misc/easter/faberge_egg_d/rustigeegg_d.deployed.prefab",
			"assets/prefabs/misc/easter/faberge_egg_e/rustigeegg_e.deployed.prefab",
			"assets/prefabs/misc/halloween/cursed_cauldron/cursedcauldron.deployed.prefab",
			"assets/prefabs/voiceaudio/discofloor/discofloor.deployed.prefab",
			"assets/prefabs/voiceaudio/discofloor/skins/discofloor.largetiles.deployed.prefab",
			"assets/prefabs/tools/flareold/flare.deployed.prefab",
			"assets/prefabs/voiceaudio/cassetterecorder/cassetterecorder.deployed.prefab",
			"assets/prefabs/misc/halloween/candies/collectablecandy.prefab",
			"assets/prefabs/misc/xmas/xmastree/xmas_tree_a.deployed.prefab",
			"assets/prefabs/misc/xmas/xmastree/xmas_tree.deployed.prefab",
			"assets/prefabs/misc/xmas/wreath/christmas_door_wreath_deployed.prefab",
			"assets/prefabs/misc/xmas/windowgarland/windowgarland.deployed.prefab",
			"assets/prefabs/misc/xmas/stockings/stocking_large_deployed.prefab",
			"assets/prefabs/misc/xmas/stockings/stocking_small_deployed.prefab",
			"assets/prefabs/misc/xmas/snowman/snowman.deployed.prefab",
			"assets/prefabs/misc/halloween/trophy skulls/skins/skulltrophy.jar.deployed.prefab",
			"assets/prefabs/misc/halloween/skull_door_knocker/skull_door_knocker.deployed.prefab",
			"assets/prefabs/misc/halloween/skull spikes/skullspikes.deployed.prefab",
			"assets/prefabs/misc/halloween/skull spikes/skins/skullspikes.pumpkin.deployed.prefab",
			"assets/prefabs/misc/halloween/scarecrow/scarecrow.deployed.prefab",
			"assets/prefabs/misc/halloween/skull spikes/skins/skullspikes.candles.deployed.prefab",
			"assets/prefabs/misc/halloween/deployablegravestone/gravestone.wood.deployed.prefab",
			"assets/prefabs/misc/halloween/deployablegravestone/gravestone.stone.deployed.prefab",
			"assets/content/structures/excavator/prefabs/diesel_collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/stone/sulfur-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/wood/wood-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/stone/stone-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/berry-black/berry-black-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/stone/metal-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/berry-blue/berry-blue-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/berry-green/berry-green-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/stone/metal-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/stone/halloween/halloween-wood-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/berry-red/berry-red-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/berry-white/berry-white-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/stone/halloween/halloween-sulfur-collectible.prefab",
			"assets/bundled/prefabs/autospawn/collectable/berry-yellow/berry-yellow-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/corn/corn-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/stone/halloween/halloween-stone-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/hemp/hemp-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/mushrooms/mushroom-cluster-5.prefab",
			"assets/bundled/prefabs/autospawn/collectable/stone/halloween/halloween-metal-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/mushrooms/mushroom-cluster-6.prefab",
			"assets/bundled/prefabs/autospawn/collectable/stone/halloween/halloween-bone-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/potato/potato-collectable.prefab",
			"assets/bundled/prefabs/autospawn/collectable/pumpkin/pumpkin-collectable.prefab",
			"assets/prefabs/misc/xmas/sled/sled.deployed.prefab",
			"assets/prefabs/misc/halloween/trophy skulls/skins/skulltrophy.jar2.deployed.prefab",
			"assets/prefabs/misc/halloween/trophy skulls/skins/skulltrophy.table.deployed.prefab",
			"assets/prefabs/misc/xmas/sled/skins/sled.deployed.xmas.prefab",
			"assets/prefabs/misc/halloween/trophy skulls/skins/skulltrophy.table.deployed.prefab",
			"assets/prefabs/misc/xmas/poweredlights/xmas.advanced.lights.deployed.prefab",
			"assets/prefabs/misc/xmas/pookie/pookie_deployed.prefab",
			"assets/prefabs/misc/halloween/trophy skulls/skulltrophy.deployed.prefab",
			"assets/prefabs/misc/xmas/giant_candy_cane/giantcandycane.deployed.prefab",
			"assets/prefabs/misc/xmas/lollipop_bundle/giantlollipops.deployed.prefab",
			"assets/prefabs/misc/xmas/double_doorgarland/double_doorgarland.deployed.prefab",
			"assets/prefabs/misc/xmas/advent_calendar/advendcalendar.deployed.prefab",
			"assets/prefabs/misc/xmas/christmas_lights/xmas.lightstring.deployed.prefab",
			"assets/prefabs/misc/xmas/doorgarland/doorgarland.deployed.prefab",
			"assets/prefabs/misc/summer_dlc/abovegroundpool/abovegroundpool.deployed.prefab",
			"assets/prefabs/misc/twitch/hobobarrel/hobobarrel.deployed.prefab",
			"assets/prefabs/misc/summer_dlc/paddling_pool/paddlingpool.deployed.prefab",
			"assets/prefabs/misc/summer_dlc/inner_tube/skins/innertube.unicorn.deployed.prefab",
			"assets/prefabs/misc/summer_dlc/inner_tube/skins/innertube.horse.deployed.prefab",
			"assets/prefabs/misc/summer_dlc/inner_tube/innertube.deployed.prefab",
			"assets/prefabs/misc/summer_dlc/boogie_board/boogieboard.deployed.prefab",
			"assets/prefabs/misc/summer_dlc/beach_towel/beachtowel.deployed.prefab",
			"assets/prefabs/misc/summer_dlc/beach_chair/beachtable.deployed.prefab",
			"assets/prefabs/misc/summer_dlc/beach_chair/beachparasol.deployed.prefab",
			"assets/prefabs/misc/summer_dlc/beach_chair/beachchair.deployed.prefab",
			"assets/prefabs/misc/junkpile/junkpile_a.prefab",
			"assets/prefabs/misc/junkpile/junkpile_b.prefab",
			"assets/prefabs/misc/junkpile/junkpile_c.prefab",
			"assets/prefabs/misc/junkpile/junkpile_d.prefab",
			"assets/prefabs/misc/junkpile/junkpile_e.prefab",
			"assets/prefabs/misc/junkpile/junkpile_f.prefab",
			"assets/prefabs/misc/junkpile/junkpile_g.prefab",
			"assets/prefabs/misc/junkpile/junkpile_h.prefab",
			"assets/prefabs/misc/junkpile/junkpile_i.prefab",
			"assets/prefabs/misc/junkpile/junkpile_j.prefab",
			"assets/prefabs/misc/junkpile_water/junkpile_water_c.prefab",
			"assets/prefabs/misc/junkpile_water/junkpile_water_b.prefab",
			"assets/prefabs/misc/junkpile_water/junkpile_water_a.prefab"
		};
		public string DefaultSignImage = "iVBORw0KGgoAAAANSUhEUgAAANcAAAB9CAYAAAAx+vY9AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAB/SURBVHhe7cGBAAAAAMOg+VNf4QBVAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA8aqR4AAFsKyZjAAAAAElFTkSuQmCC";
	}
}
public class RustEditData
{
}
