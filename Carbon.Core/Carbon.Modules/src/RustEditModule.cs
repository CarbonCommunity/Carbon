using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Carbon.Base;
using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static BasePlayer;
using static Carbon.Modules.RustEditModule;
using static QRCoder.PayloadGenerator;
using Pool = Facepunch.Pool;
using Time = UnityEngine.Time;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 bmgjet
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public class RustEditModule : CarbonModule<RustEditConfig, RustEditData>
{
	public override string Name => "RustEdit.Ext";
	public override Type Type => typeof(RustEditModule);

	private void OnServerInitialized()
	{
		if (!ConfigInstance.Enabled) return;

		#region Spawnpoints

		if (Spawn_Spawnpoints != null && Spawn_Spawnpoints.Count != 0)
		{
			PutsWarn($"Found {Spawn_Spawnpoints.Count:n0} spawn-points.");
		}

		#endregion

		#region NPC Spawner

		if (NPCSpawner_serializedNPCData == null)
		{
			//Load XML from map
			var data = GetData("SerializedNPCData");

			if (data != null) { NPCSpawner_serializedNPCData = Deserialize<SerializedNPCData>(data); }
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
			NPCSpawner_NPCThread = ServerMgr.Instance.StartCoroutine(NPCAIFunction());
		}

		#endregion

		#region APC

		if (APC_serializedAPCPathList == null) { APC_ReadMapData(); }
		if (APC_serializedAPCPathList != null) { ServerMgr.Instance.StartCoroutine(APC_Setup()); }

		#endregion
	}
	private object CanBradleyApcTarget(BradleyAPC apc, BaseEntity entity)
	{
		if (entity is NPCPlayer && DataInstance.NpcSpawner.APCIgnoreNPCs)
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
		if (turret.OwnerID == 0 && DataInstance.NpcSpawner.NPCAutoTurretsIgnoreNPCs)
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
		if (!DataInstance.NpcSpawner.UseRustEditNPCLogic)
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
		if (!ConfigInstance.Enabled) return null;

		return Spawn_RespawnPlayer();
	}
	private object OnWorldPrefabSpawn(Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		if (!ConfigInstance.Enabled) return null;

		if (prefab.Name.Equals(Spawn_SpawnpointPrefab))
		{
			Spawn_Spawnpoints.Add(position);
			return true;
		}

		return null;
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
			Debug.Log(ex.Message + " " + ex.StackTrace);
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
			Debug.Log(ex.Message + "\n" + ex.StackTrace);
			result = null;
		}
		return result;
	}

	#endregion

	#region Spawn

	internal List<Vector3> Spawn_Spawnpoints = new List<Vector3>();
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

	internal SerializedNPCSpawner NPCSpawner_serializedNPCSpawner;
	internal SerializedNPCData NPCSpawner_serializedNPCData;
	internal List<NPCSpawner> NPCSpawner_NPCsList = new List<NPCSpawner>();
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

	public class SerializedNPCData { public List<SerializedNPCSpawner> npcSpawners = new List<SerializedNPCSpawner>(); }

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
		public static Dictionary<NPCType, string> typeToPrefab = new Dictionary<NPCType, string>
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
			var settings = module.DataInstance.NpcSpawner;

			var position = (Vector3)serializedNPCSpawner.position;
			var num = 1;
			var npc = (ScientistNPC)null;
			var npc2 = (ScarecrowNPC)null;
			stationary = !module.IsOnGround(ref position, out num);

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
					npc.InitializeHealth(module.DataInstance.NpcSpawner.ScientistNPCHealth, settings.ScientistNPCHealth);
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
						module.DressNPC(npc, type);
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
						module.DressNPC(npc, type);
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
						module.DressNPC(npc, type);

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
						module.DressNPC(npc, type);
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
						module.DressNPC(npc, type);
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
						module.DressNPC(npc2, type);
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
						module.DressNPC(npc2, type);
					}
					break;
			}
		}
	}

	private System.Collections.IEnumerator NPCAIFunction()
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
							if (!AnyPlayersNearby(npcs.serializedNPCSpawner.position, DataInstance.NpcSpawner.SpawnBlockingDistance)) { npcs.SpawnNPC(); }
						}
						continue;
					}
					if ((npcs.BOT == null || npcs.BOT.IsDestroyed) && !npcs.Respawning)
					{
						npcs.Respawning = true;
						npcs.Delay = Time.time + (float)UnityEngine.Random.Range(npcs.serializedNPCSpawner.respawnMin, npcs.serializedNPCSpawner.respawnMax);
						continue;
					}
					if (DataInstance.NpcSpawner.UseRustEditNPCLogic || npcs.type == NPCType.Scarecrow || npcs.type == NPCType.Murderer)
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
								RandomMovement(npcs);
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
							RandomMovement(npcs);
							npcs.target = null;
						}
					}
				}
				catch { }
			}

			yield return CoroutineEx.waitForSeconds(DataInstance.NpcSpawner.AITickRate);
		}

		PutsWarn("Stopped AI processing.");
		yield break;
	}

	public bool AnyPlayersNearby(Vector3 position, float maxDist)
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
	public void RandomMovement(NPCSpawner npcs)
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

	public void DressNPC(BasePlayer npc, NPCType npctype)
	{
		if (npc == null) { return; }
		switch (npctype)
		{
			case NPCType.Scientist:
				CreateCloths(npc, "hazmatsuit_scientist_arctic");
				switch ((int)UnityEngine.Random.Range(0f, 2))
				{
					case 0:
						CreateGun(npc, "rifle.lr300", "weapon.mod.flashlight");
						break;
					case 1:
						CreateGun(npc, "rifle.ak", "weapon.mod.flashlight");
						break;
					case 2:
						CreateGun(npc, "pistol.m92", "weapon.mod.flashlight");
						break;
				}
				break;
			case NPCType.Peacekeeper:
				CreateCloths(npc, "hazmatsuit_scientist_peacekeeper");
				CreateGun(npc, "rifle.m39", "weapon.mod.flashlight");
				break;
			case NPCType.HeavyScientist:
				CreateCloths(npc, "scientistsuit_heavy");
				switch ((int)UnityEngine.Random.Range(0f, 2))
				{
					case 0:
						CreateGun(npc, "lmg.m249", "weapon.mod.flashlight");
						break;
					case 1:
						CreateGun(npc, "shotgun.spas12", "weapon.mod.flashlight");
						break;
					case 2:
						CreateGun(npc, "rifle.lr300", "weapon.mod.flashlight");
						break;
				}
				break;
			case NPCType.JunkpileScientist:
				CreateCloths(npc, "hazmatsuit_scientist");
				CreateGun(npc, "pistol.m92", "weapon.mod.flashlight");
				break;
			case NPCType.Bandit:
				CreateCloths(npc, "attire.banditguard");
				CreateGun(npc, "rifle.semiauto", "weapon.mod.flashlight");
				break;
			case NPCType.Murderer:
				CreateCloths(npc, "scarecrow.suit");
				CreateCloths(npc, "jacket");
				CreateGun(npc, "chainsaw");
				break;
			case NPCType.Scarecrow:
				CreateCloths(npc, "halloween.mummysuit");
				switch ((int)UnityEngine.Random.Range(0f, 2))
				{
					case 0:
						CreateGun(npc, "pitchfork");
						break;
					case 1:
						CreateGun(npc, "sickle");
						break;
					case 2:
						CreateGun(npc, "bone.clubsickle");
						break;
				}
				break;
		}
	}
	public void CreateCloths(BasePlayer npc, string itemName, ulong skin = 0)
	{
		if (npc == null) return;

		var item = ItemManager.CreateByName(itemName, 1, skin);
		if (item == null) return;

		if (!item.MoveToContainer(npc.inventory.containerWear, -1, false)) item.Remove();
	}
	public void CreateGun(BasePlayer npc, string itemName, string attachment = "", ulong skinid = 0)
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

		DoFixes(npc, item);
	}
	public void DoFixes(BasePlayer npc, Item item)
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

	public bool IsOnGround(ref Vector3 position, out int areamask)
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

	public byte[] GetData(string name)
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
	public byte[] Serialize<T>(T stream)
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
	public T Deserialize<T>(byte[] bytes)
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

	internal APC_SerializedAPCPathList APC_serializedAPCPathList;
	internal List<APC_CustomAPCSpawner> APC_list = new();
	internal List<BasePlayer> APC_ViewingList = new();
	internal Coroutine APC_Viewingthread;

	internal IEnumerator APC_Setup()
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
			Debug.LogWarning("[Core.APC] Generating " + APC_serializedAPCPathList.paths.Count + " APC paths");
			foreach (APC_SerializedAPCPath sapc in APC_serializedAPCPathList.paths)
			{
				if (sapc.interestNodes.Count == 0) { Debug.LogError("[Core.APC] No APC interest Nodes Found!"); continue; }
				APC_CustomAPCSpawner customAPCSpawner = new GameObject("CustomAPCSpawner").AddComponent<APC_CustomAPCSpawner>();
				customAPCSpawner.LoadBradleyPathing(sapc);
				APC_list.Add(customAPCSpawner);
				yield return CoroutineEx.waitForEndOfFrame;
			}
		}
		yield break;
	}

	internal bool APC_ReadMapData()
	{
		bool flag = false;
		if (APC_serializedAPCPathList != null) { return flag; }
		byte[] array = APC_GetSerializedIOData();
		if (array != null && array.Length != 0)
		{
			APC_serializedAPCPathList = DeserializeMapData<APC_SerializedAPCPathList>(array, out flag);
		}
		if (APC_serializedAPCPathList == null) { APC_serializedAPCPathList = new APC_SerializedAPCPathList(); }
		Debug.LogWarning("[Core.APC] DATA Found:" + flag);
		return flag;
	}

	internal byte[] APC_GetSerializedIOData()
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
				Debug.LogWarning("[Core.APC] Custom APC Spawned @ " + vector);
			}
		}

		public BasePath basePath;
		public BradleyAPC bradleyAPC;
		private bool NeedsSpawn;
		private string prefab = "assets/prefabs/npc/m2bradley/bradleyapc.prefab";
	}
	public class APC_SerializedAPCPath
	{

		public List<VectorData> nodes = new List<VectorData>();
		public List<VectorData> interestNodes = new List<VectorData>();
	}
	public class APC_SerializedAPCPathList
	{
		public List<APC_SerializedAPCPath> paths = new();
	}

	#endregion

	#region Cargo Path

	internal const string _c4 = "assets/prefabs/tools/c4/effects/c4_explosion.prefab";
	internal const string _debris = "assets/prefabs/npc/patrol helicopter/damage_effect_debris.prefab";
	internal static bool DisableSpawns = false;
	internal const bool SinkMode = true;
	internal const int StopDelay = 60;
	internal const int DistanceFromStop = 35;
	internal const int DespawnDistance = 80;
	internal static List<BasePlayer> ViewingList = new();
	internal static Coroutine Viewingthread;
	internal static List<CargoMod> CargoShips = new();
	internal static List<Vector3> CargoPath = new();
	internal static List<Vector3> CargoSpawn = new();
	internal static List<Vector3> CargoStops = new();
	internal static SerializedPathList serializedPathList;

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

			if (CargoSpawn != null && CargoSpawn.Count != 0)
			{
				if (!DisableSpawns)
				{
					cargo.transform.position = CargoSpawn.GetRandom();
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
		foreach (CargoMod cm in CargoShips)
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
			if (ViewingList.Contains(player))
			{
				player.ChatMessage("Stopped Viewing Cargo Path");
				ViewingList.Remove(player);
			}
			else
			{
				player.ChatMessage("Started Viewing Cargo Path");
				ViewingList.Add(player);
			}
			if (Viewingthread == null && ViewingList.Count != 0)
			{
				Viewingthread = ServerMgr.Instance.StartCoroutine(ShowCargoPath());
				return;
			}
			else
			{
				ServerMgr.Instance.StopCoroutine(Viewingthread);
				Viewingthread = null;
			}
		}
	}

	[ChatCommand("rusteditext.cargo.bypasscargospawn")]
	[AuthLevel(ServerUsers.UserGroup.Owner)]
	private void Cargo_BypassCargoSpawn(BasePlayer player)
	{
		DisableSpawns = !DisableSpawns;
		player.ChatMessage("Disabling cargospawn points " + DisableSpawns.ToString());
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
			if (SinkMode)
			{
				var cargos = Pool.GetList<CargoMod>();
				cargos.AddRange(CargoShips);

				foreach (var cm in cargos)
				{
					if (cm._cargoship == cargo)
					{
						CargoShips.Remove(cm);
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

	#endregion

	#region Command Helpers

	public IEnumerator ShowCargoPath()
	{
		var distance = 2000;
		var LastNode = Vector3.zero;

		while (ViewingList != null && ViewingList.Count != 0)
		{
			foreach (BasePlayer bp in ViewingList.ToArray())
			{
				if (!bp.IsConnected || bp.IsSleeping() || !bp.IsAdmin)
				{
					ViewingList.Remove(bp);
					continue;
				}

				foreach (Vector3 spawnpoints in CargoSpawn)
				{
					bp.SendConsoleCommand("ddraw.box", 5f, Color.green, spawnpoints, 10f);
					bp.SendConsoleCommand("ddraw.text", 5f, Color.green, spawnpoints, "<size=20>CargoSpawn</size>");
				}

				foreach (Vector3 stoppoints in CargoStops)
				{
					bp.SendConsoleCommand("ddraw.box", 5f, Color.red, stoppoints, 10f);
					bp.SendConsoleCommand("ddraw.text", 5f, Color.red, stoppoints, "<size=20>CargoStop</size>");
				}

				var nodeindex = 0;
				foreach (Vector3 vector in CargoPath)
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

		ServerMgr.Instance.StopCoroutine(Viewingthread);
		Viewingthread = null;
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
			CargoShips.Add(newcargoship);
		}
	}
	internal static bool Cargo_AreaStop(Vector3 vector3)
	{
		foreach (var vector in CargoStops)
		{
			if (Vector3.Distance(vector, vector3) < DistanceFromStop)
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
		if (CargoSpawn.Count != 0 || CargoStops.Count != 0)
		{
			Debug.LogWarning("[Core.CargoPath] Found " + CargoSpawn.Count.ToString() + " Spawn Points And " + CargoStops.Count.ToString() + " Stop Points");
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
		foreach (var destry in CargoShips)
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
			CargoSpawn.Add(position);
			return false;
		}
		//843218194 - assets/prefabs/tools/map/cargomarker.prefab
		else if (prefab.ID == 843218194)
		{
			CargoStops.Add(position);
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
		if (serializedPathList == null)
		{
			var SPL = new SerializedPathList();
			var nvd = new List<VectorData>();
			foreach (Vector3 vector in CargoPath) { nvd.Add(vector); }
			SPL.vectorData = nvd;
			serializedPathList = SPL;
		}
		System.IO.File.WriteAllBytes("SerializedPathList.xml", SerializeMapData(serializedPathList));
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
		if (serializedPathList != null) { return flag; }

		var array = Cargo_GetSerializedIOData();
		if (array != null && array.Length != 0)
		{
			serializedPathList = DeserializeMapData<SerializedPathList>(array, out flag);
		}
		Debug.LogWarning("[Core.CargoPath] DATA Found:" + flag);
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
		public List<ScientistNPC> NPCs = new List<ScientistNPC>();
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
					StopCargoShip(StopDelay);
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
					while (_cargoship != null && PlayersNearbyfunction(_cargoship.transform.position, DespawnDistance))
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
				RunEffect(_c4, null, ExPoint[0]);
				RunEffect(_debris, null, ExPoint[1]);
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
}

public class RustEditConfig
{
}
public class RustEditData
{
	public NpcSettings NpcSpawner = new NpcSettings();

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
}
