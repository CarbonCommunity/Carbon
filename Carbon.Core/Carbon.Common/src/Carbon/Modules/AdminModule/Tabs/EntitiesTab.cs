#if !MINIMAL

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using Network;
using StringEx = Carbon.Extensions.StringEx;

namespace Carbon.Modules;

public partial class AdminModule : CarbonModule<AdminConfig, AdminData>
{
	public class EntitiesTab
	{
		internal static int EntityCount = 0;

		internal static RustPlugin Core = Community.Runtime.CorePlugin;
		internal static AdminModule Admin = GetModule<AdminModule>();
		internal static PlayerSession LastContainerLooter;
		internal static string[] BuildingGrades = new string[]
		{
			"Twig",
			"Wood",
			"Stone",
			"Metal",
			"Top Tier"
		};
		internal const string MultiselectionReplacement = "-";

		public static Tab Get()
		{
			var tab = new Tab("entities", "Entities", Community.Runtime.CorePlugin, (ap, tab2) => { tab2.ClearColumn(1); ResetSelection(tab2, ap); DrawEntities(tab2, ap); }, 2);
			tab.AddColumn(0);
			tab.AddColumn(1);

			return tab;
		}

		internal static void SelectEntity(Tab tab, PlayerSession session, BaseEntity entity)
		{
			var selectedEntitites = (List<BaseEntity>)null;

			if (!session.HasStorage(tab, "selectedentities"))
			{
				selectedEntitites = session.SetStorage(tab, "selectedentities", new List<BaseEntity>());
			}
			else
			{
				selectedEntitites = session.GetStorage<List<BaseEntity>>(tab, "selectedentities");
			}

			if (!session.GetStorage(tab, "multi", false)) selectedEntitites.Clear();
			if (!selectedEntitites.Contains(entity)) selectedEntitites.Add(entity);
		}
		internal static void ResetSelection(Tab tab, PlayerSession session)
		{
			var selectedEntitites = (List<BaseEntity>)null;

			if (!session.HasStorage(tab, "selectedentities"))
			{
				selectedEntitites = session.SetStorage(tab, "selectedentities", new List<BaseEntity>());
			}
			else
			{
				selectedEntitites = session.GetStorage<List<BaseEntity>>(tab, "selectedentities");
				selectedEntitites.Clear();
			}
		}

		internal static void DrawEntities(Tab tab, PlayerSession ap3)
		{
			tab.ClearColumn(0);
			tab.AddName(0, "Entities");

			var selectedEntitites = ap3.GetStorage<List<BaseEntity>>(tab, "selectedentities");

			if (!ap3.HasStorage(tab, "selectedentities"))
			{
				selectedEntitites = ap3.SetStorage(tab, "selectedentities", new List<BaseEntity>());
			}

			tab.AddInputButton(0, "Search Entity", 0.3f,
				new Tab.OptionInput(null, ap => ap.GetStorage(tab, "filter", string.Empty), 0, false, (ap, args) => { ap.SetStorage(tab, "filter", args.ToString(" ")); DrawEntities(tab, ap); }),
				new Tab.OptionButton($"Refresh", ap => { DrawEntities(tab, ap); }));

			var isMulti = ap3.GetStorage(tab, "multi", false);
			tab.AddToggle(0, "Multi-selection", ap =>
			{
				isMulti = ap.SetStorage(tab, "multi", !isMulti);
				selectedEntitites.Clear();
				tab.ClearColumn(1);
				DrawEntities(tab, ap3);
			}, ap => isMulti);

			var pool = Facepunch.Pool.GetList<BaseEntity>();
			EntityCount = 0;

			var usedFilter = ap3.GetStorage(tab, "filter", string.Empty)?.ToLower()?.Trim();
			var map = Entities.Get<BaseEntity>(true);
			var validateFilter = ap3.GetStorage<Func<BaseEntity, bool>>(tab, "validatefilter");
			var maximumRange = ((int)World.Size).Clamp(1, int.MaxValue) / 2;
			var range = ap3.GetStorage(tab, "range", maximumRange);
			map.Each(entity =>
			{
				pool.Add(entity);
				EntityCount++;
			}, entity => entity != null && entity.transform != null && (validateFilter == null || validateFilter.Invoke(entity)) && entity.transform.position != Vector3.zero
				&& (string.IsNullOrEmpty(usedFilter) || entity.ToString().ToLower().Contains(usedFilter) || entity.name.ToLower().Contains(usedFilter) || entity.GetType().Name?.ToLower() == usedFilter)
				&& (range == -1 || ap3 == null || (ap3.Player != null && Vector3.Distance(ap3.Player.transform.position, entity.transform.position) <= range)));
			map.Dispose();

			tab.AddRange(0, "Range", 0, maximumRange, ap => range, (ap, value) => { try { ap.SetStorage(tab, "range", (int)value); DrawEntities(tab, ap); } catch (Exception ex) { Logger.Error($"Oof", ex); } }, ap => $"{range:0.0}m");
			tab.AddName(0, $"Entities  ({EntityCount:n0})", TextAnchor.MiddleLeft);

			var filter = ap3.GetStorage(tab, "filter", string.Empty);
			tab.AddButtonArray(0,
				new Tab.OptionButton("Players", ap => { ap.SetStorage(tab, "filter", nameof(BasePlayer)); DrawEntities(tab, ap); }, ap => filter == nameof(BasePlayer) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("Containers", ap => { ap.SetStorage(tab, "filter", nameof(StorageContainer)); validateFilter = null; DrawEntities(tab, ap); }, ap => filter == nameof(StorageContainer) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("Deployables", ap => { ap.SetStorage(tab, "filter", nameof(Deployable)); validateFilter = null; DrawEntities(tab, ap); }, ap => filter == nameof(Deployable) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("Collectibles", ap => { ap.SetStorage(tab, "filter", nameof(CollectibleEntity)); validateFilter = null; DrawEntities(tab, ap); }, ap => filter == nameof(CollectibleEntity) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("NPCs", ap => { ap.SetStorage(tab, "filter", nameof(NPCPlayer)); validateFilter = null; DrawEntities(tab, ap); }, ap => filter == nameof(NPCPlayer) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("I/O", ap => { ap.SetStorage(tab, "filter", nameof(IOEntity)); validateFilter = null; DrawEntities(tab, ap); }, ap => filter == nameof(IOEntity) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));

			switch (ap3.GetStorage(tab, "filter", string.Empty))
			{
				case nameof(BasePlayer):
					tab.AddButtonArray(0,
						new Tab.OptionButton("Online", ap => { validateFilter = entity => entity is BasePlayer player && player.IsConnected; DrawEntities(tab, ap); }),
						new Tab.OptionButton("Offline", ap => { validateFilter = entity => entity is BasePlayer player && !player.IsConnected; DrawEntities(tab, ap); }),
						new Tab.OptionButton("Dead", ap => { validateFilter = entity => entity is BasePlayer player && player.IsDead(); DrawEntities(tab, ap); }));
					break;
			}

			foreach (var entity in pool)
			{
				var name = entity.ToString();

				switch (entity)
				{
					case BasePlayer player:
						name = $"{player.displayName}";
						break;
				}

				tab.AddButton(0, name, ap =>
				{
					if (selectedEntitites.Contains(entity))
					{
						selectedEntitites.Remove(entity);
						tab.ClearColumn(1);
					}
					else
					{
						SelectEntity(tab, ap, entity);
					}

					DrawEntitySettings(tab, 1, ap);
				}, ap => selectedEntitites.Contains(entity) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);
			}

			Facepunch.Pool.FreeList(ref pool);

			if (EntityCount == 0)
			{
				tab.AddText(0, "No entities found with that filter", 9, "1 1 1 0.2", TextAnchor.MiddleCenter, CUI.Handler.FontTypes.RobotoCondensedRegular);
			}
		}
		internal static void DrawEntitySettings(Tab tab, int column = 1, PlayerSession ap3 = null)
		{
			var selectedEntitites = ap3.GetStorage<List<BaseEntity>>(tab, "selectedentities");
			tab.ClearColumn(column);

			if (selectedEntitites.Count == 0) return;

			var entity = selectedEntitites[0];
			var multiSelection = selectedEntitites.Count > 1;
			var sameTypeSelection = selectedEntitites.All(x => x.GetType() == entity.GetType());

			tab.AddName(column, "Hierarchy");

			if (column != 1) tab.AddButton(column, "<", ap => { DrawEntities(tab, ap); DrawEntitySettings(tab, 1, ap); }, ap => Tab.OptionButton.Types.Warned);

			if (entity != null && !entity.IsDestroyed)
			{
				var player = entity as BasePlayer;
				var owner = BasePlayer.FindByID(entity.OwnerID);

				if (player != ap3?.Player)
				{
					tab.AddButtonArray(column,
						new Tab.OptionButton("Kill", ap =>
						{
							tab.CreateDialog($"Are you sure about that?", ap =>
							{
								DoAll<BaseEntity>(e => e.Kill());
								DrawEntities(tab, ap);
								tab.ClearColumn(column);
							}, null);
						}, ap => Tab.OptionButton.Types.Important),
						new Tab.OptionButton("Kill (Gibbed)", ap =>
						{
							tab.CreateDialog($"Are you sure about that?", ap =>
							{
								DoAll<BaseEntity>(e => e.Kill(BaseNetworkable.DestroyMode.Gib));
								DrawEntities(tab, ap);
								tab.ClearColumn(column);
							}, null);
						}));
				}

				tab.AddInput(column, "Id", ap => multiSelection ? MultiselectionReplacement : $"{entity.net.ID} [{entity.GetType().FullName}]", null);
				tab.AddInput(column, "Name", ap => multiSelection ? MultiselectionReplacement : $"{entity.ShortPrefabName}", null);

				if (!multiSelection)
				{
					var ownerPlayer = BasePlayer.FindByID(entity.OwnerID);

					tab.AddInputButton(column, "Owner", 0.3f,
						new Tab.OptionInput(null, ap => $"{(entity.OwnerID.IsSteamId() ? $"{(ownerPlayer == null ? entity.OwnerID.ToString() : ownerPlayer.displayName)}" : "None")}", 0, true, null),
						new Tab.OptionButton("Select", ap =>
						{
							if (owner == null) return;

							SelectEntity(tab, ap, owner);
							DrawEntities(tab, ap);
							DrawEntitySettings(tab, 1, ap);
						}, ap => owner == null ? Tab.OptionButton.Types.None : Tab.OptionButton.Types.Selected));
				}

				tab.AddInput(column, "Prefab", ap => multiSelection ? MultiselectionReplacement : $"{entity.PrefabName}", null);
				tab.AddInput(column, "Flags", ap => multiSelection ? MultiselectionReplacement : entity.flags == 0 ? "None" : $"{entity.flags}", null);
				tab.AddButton(column, "Edit Flags", ap => { DrawEntitySettings(tab, 0, ap); DrawEntityFlags(tab, ap, 1); });
				tab.AddInput(column, "Position", ap => multiSelection ? MultiselectionReplacement : $"{entity.transform.position}", null);
				tab.AddInput(column, "Rotation", ap => multiSelection ? MultiselectionReplacement : $"{entity.transform.rotation}", null);

				if (sameTypeSelection)
				{
					if (!multiSelection)
					{
						tab.AddButtonArray(column,
							new Tab.OptionButton("TeleportTo", ap => { ap.Player.Teleport(entity.transform.position); }),
							new Tab.OptionButton("Teleport2Me", ap =>
							{
								tab.CreateDialog($"Are you sure about that?", ap =>
								{
									if (entity is BasePlayer player)
									{
										player.Teleport(ap.Player.transform.position);
									}
									else
									{
										entity.transform.position = ap.Player.transform.position;
										entity.SendNetworkUpdate_Position();
									}
								}, null);
							}));
					}

					if (entity is StorageContainer storage)
					{
						if (!multiSelection)
						{
							tab.AddButton(column, "Loot Container", ap =>
							{
								LastContainerLooter = ap;

								ap.SetStorage(tab, "lootedent", entity);
								Admin.Subscribe("OnEntityVisibilityCheck");
								Admin.Subscribe("OnEntityDistanceCheck");

								Core.timer.In(0.2f, () => Admin.Close(ap.Player));
								Core.timer.In(0.5f, () =>
								{
									SendEntityToPlayer(ap.Player, entity);

									ap.Player.inventory.loot.Clear();
									ap.Player.inventory.loot.PositionChecks = false;
									ap.Player.inventory.loot.entitySource = storage;
									ap.Player.inventory.loot.itemSource = null;
									ap.Player.inventory.loot.AddContainer(storage.inventory);
									ap.Player.inventory.loot.MarkDirty();
									ap.Player.inventory.loot.SendImmediate();

									ap.Player.ClientRPCPlayer(null, ap.Player, "RPC_OpenLootPanel", storage.panelName);
								});
							});
						}
					}

					if (entity is BasePlayer)
					{
						tab.AddInput(column, "Display Name", ap => multiSelection ? MultiselectionReplacement : player.displayName);
						tab.AddInput(column, "Steam ID", ap => multiSelection ? MultiselectionReplacement : player.UserIDString);
						tab.AddInput(column, "IP", ap => multiSelection ? MultiselectionReplacement : $"{player.net?.connection?.ipaddress}", null, hidden: true);

						if (!multiSelection && Singleton.HasAccessLevel(ap3?.Player, 2))
						{
							tab.AddButtonArray(1, new Tab.OptionButton("Kick", ap =>
							{
								Singleton.Modal.Open(ap.Player, $"Kick {player.displayName}", new Dictionary<string, ModalModule.Modal.Field>
								{
									["reason"] = ModalModule.Modal.Field.Make("Reason", ModalModule.Modal.Field.FieldTypes.String, @default: "Stop doing that.")
								}, onConfirm: (p, m) =>
								{
									player.Kick(m.Get<string>("reason"));
								});
							}), new Tab.OptionButton("Ban", ap =>
							{
								Singleton.Modal.Open(ap.Player, $"Ban {player.displayName}", new Dictionary<string, ModalModule.Modal.Field>
								{
									["reason"] = ModalModule.Modal.Field.Make("Reason", ModalModule.Modal.Field.FieldTypes.String, @default: "Stop doing that."),
									["until"] = ModalModule.Modal.ButtonField.MakeButton("Until", "Select Date", m =>
									{
										Core.NextTick(() => Singleton.DatePicker.Draw(ap.Player, date => ap.SetStorage(tab, "date", date)));
									})
								}, onConfirm: (p, m) =>
								{
									var date = ap.GetStorage(tab, "date", DateTime.UtcNow.AddYears(100));
									var now = DateTime.UtcNow;
									date = new DateTime(date.Year, date.Month, date.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);

									if (now <= date) date = DateTime.UtcNow.AddYears(100);

									var then = now - date;

									player.AsIPlayer().Ban(m.Get<string>("reason"), then);
								});
							}));
						}
						tab.AddButtonArray(column,
							new Tab.OptionButton("Loot", ap =>
							{
								if (multiSelection) return;

								LastContainerLooter = ap;
								ap.SetStorage(tab, "lootedent", entity);
								SendEntityToPlayer(ap.Player, entity);

								Core.timer.In(0.2f, () => Admin.Close(ap.Player));
								Core.timer.In(0.5f, () =>
								{
									SendEntityToPlayer(ap.Player, entity);

									ap.Player.inventory.loot.Clear();
									ap.Player.inventory.loot.PositionChecks = false;
									ap.Player.inventory.loot.entitySource = RelationshipManager.ServerInstance;
									ap.Player.inventory.loot.itemSource = null;
									ap.Player.inventory.loot.AddContainer(player.inventory.containerMain);
									ap.Player.inventory.loot.AddContainer(player.inventory.containerWear);
									ap.Player.inventory.loot.AddContainer(player.inventory.containerBelt);
									ap.Player.inventory.loot.MarkDirty();
									ap.Player.inventory.loot.SendImmediate();

									ap.Player.ClientRPCPlayer(null, ap.Player, "RPC_OpenLootPanel", "player_corpse");
								});
							}),
							new Tab.OptionButton("Respawn", ap =>
							{
								tab.CreateDialog($"Are you sure about that?", ap =>
								{
									DoAll<BasePlayer>(e =>
									{
										e.Hurt(player.MaxHealth());
										e.Respawn();
										e.EndSleeping();
									});
								}, null);
							}));

						tab.AddName(1, "Inventory Lock");
						tab.AddButtonArray(1,
							new Tab.OptionButton("Main", ap =>
							{
								player.inventory.containerMain.SetLocked(!player.inventory.containerMain.IsLocked());
							}, ap => player.inventory.containerMain.IsLocked() ? Tab.OptionButton.Types.Important : Tab.OptionButton.Types.None),
							new Tab.OptionButton("Belt", ap =>
							{
								player.inventory.containerBelt.SetLocked(!player.inventory.containerBelt.IsLocked());
							}, ap => player.inventory.containerBelt.IsLocked() ? Tab.OptionButton.Types.Important : Tab.OptionButton.Types.None),
							new Tab.OptionButton("Wear", ap =>
							{
								player.inventory.containerWear.SetLocked(!player.inventory.containerWear.IsLocked());
							}, ap => player.inventory.containerWear.IsLocked() ? Tab.OptionButton.Types.Important : Tab.OptionButton.Types.None));

						tab.AddInput(column, "PM", null, (ap, args) =>
						{
							DoAll<BasePlayer>(e =>
							{
								e.ChatMessage($"[{ap.Player.displayName}]: {args.ToString(" ")}");
							});
						});

						if (!multiSelection && ap3 != null)
						{
							if (!PlayersTab.BlindedPlayers.Contains(player))
							{
								tab.AddButton(1, "Blind Player", ap =>
								{
									tab.CreateDialog("Are you sure you want to blind the player?", ap =>
									{
										using var cui = new CUI(Singleton.Handler);
										var container = cui.CreateContainer("blindingpanel", "0 0 0 1", needsCursor: true, needsKeyboard: Singleton.HandleEnableNeedsKeyboard(ap));
										cui.CreateClientImage(container, "blindingpanel", null, "https://carbonmod.gg/assets/media/cui/bsod.png", "1 1 1 1");
										cui.Send(container, player);
										PlayersTab.BlindedPlayers.Add(player);
										EntitiesTab.SelectEntity(tab, ap, entity);
										DrawEntitySettings(tab, column, ap3);

										if (ap.Player == player) Core.timer.In(1, () => { Singleton.Close(player); });
									}, null);
								});
							}
							else
							{
								tab.AddButton(1, "Unblind Player", ap =>
								{
									using var cui = new CUI(Singleton.Handler);
									cui.Destroy("blindingpanel", player);
									PlayersTab.BlindedPlayers.Remove(player);
									EntitiesTab.SelectEntity(tab, ap, entity);
									DrawEntitySettings(tab, column, ap3);
								}, ap => Tab.OptionButton.Types.Selected);
							}
						}
					}

					if (!multiSelection && ap3.Player != player && (ap3.Player.spectateFilter != player?.UserIDString && ap3.Player.spectateFilter != entity.net.ID.ToString()))
					{
						tab.AddButton(1, "Spectate", ap =>
						{
							StartSpectating(ap.Player, entity);
							SelectEntity(tab, ap, entity);
							DrawEntitySettings(tab, column, ap3);
						});
					}
					if (!multiSelection && !string.IsNullOrEmpty(ap3.Player.spectateFilter) && (ap3.Player.UserIDString == player?.UserIDString || ap3.Player.spectateFilter == entity.net.ID.ToString()))
					{
						tab.AddButton(1, "End Spectating", ap =>
						{
							StopSpectating(ap.Player);
							SelectEntity(tab, ap, entity);
							DrawEntitySettings(tab, column, ap3);
						}, ap => Tab.OptionButton.Types.Selected);
					}

					if (!multiSelection && entity.parentEntity.IsValid(true)) tab.AddButton(column, $"Parent: {entity.parentEntity.Get(true)}", ap => { DrawEntities(tab, ap); SelectEntity(tab, ap, entity.parentEntity.Get(true)); DrawEntitySettings(tab, 1, ap); });

					if (!multiSelection && entity.children.Count > 0)
					{
						tab.AddName(column, "Children", TextAnchor.MiddleLeft);
						foreach (var child in entity.children)
						{
							tab.AddButton(column, $"{child}", ap => { SelectEntity(tab, ap, child); DrawEntities(tab, ap); DrawEntitySettings(tab, 1, ap); });
						}
					}

					switch (entity)
					{
						case CCTV_RC cctv:
							{
								tab.AddName(column, "CCTV", TextAnchor.MiddleLeft);
								tab.AddInput(column, "Identifier", ap => multiSelection ? MultiselectionReplacement : cctv.GetIdentifier(), (ap, args) => { cctv.UpdateIdentifier(args.ToString(""), true); });
								if (!multiSelection)
								{
									tab.AddButton(column, "View CCTV", ap =>
									{
										Core.timer.In(0.1f, () => { Admin.Close(ap.Player); ap.SetStorage(tab, "wasviewingcam", true); });
										Core.timer.In(0.3f, () =>
										{
											Admin.Subscribe("OnEntityDismounted");
											Admin.Subscribe("CanDismountEntity");

											var station = GameManager.server.CreateEntity("assets/prefabs/deployable/computerstation/computerstation.deployed.prefab", ap.Player.transform.position) as ComputerStation;
											station.skinID = 69696;
											station.SendControlBookmarks(ap.Player);
											station.Spawn();
											station.checkPlayerLosOnMount = false;
											station.legacyDismount = true;

											station.MountPlayer(ap.Player);
											ViewCamera(ap.Player, station, cctv);
										});
									});
								}
								break;
							}
						case CodeLock codeLock:
							{
								tab.AddName(column, "Code Lock", TextAnchor.MiddleLeft);
								tab.AddInput(column, "Code", ap => multiSelection ? MultiselectionReplacement : codeLock.code, (ap, args) =>
								{
									var code = args.ToString(" ");

									foreach (var character in code)
										if (char.IsLetter(character)) return;

									DoAll<CodeLock>(e => e.code = StringEx.Truncate(code, 4));
								});
								break;
							}
						case Minicopter minicopter:
							{
								tab.AddName(column, "Minicopter", TextAnchor.MiddleLeft);

								if (!minicopter)
								{
									tab.AddButton(column, "Open Fuel", ap => { LastContainerLooter = ap; Core.timer.In(0.2f, () => Admin.Close(ap.Player)); Core.timer.In(0.5f, () => { minicopter.engineController.FuelSystem.GetFuelContainer().PlayerOpenLoot(ap.Player, doPositionChecks: false); }); });
								}
								break;
							}
						case BuildingBlock block:
							{
								tab.AddName(column, "Building Block", TextAnchor.MiddleLeft);
								tab.AddDropdown(column, "Grade", (ap) => (int)block.grade, (ap, index) =>
								{
									DoAll<BuildingBlock>(e =>
									{
										e.ChangeGrade((BuildingGrade.Enum)index, true);
										e.skinID = 0;
									});

									DrawEntitySettings(tab, column, ap);
								}, BuildingGrades);
							}
							break;
					}

					if (entity is BaseCombatEntity combat)
					{
						tab.AddName(column, "Combat", TextAnchor.MiddleLeft);
						tab.AddRange(column, "Health", 0, combat.MaxHealth(), ap => combat.health, (ap, value) =>
						{
							DoAll<BaseCombatEntity>(e => e.SetHealth(value));
						}, ap => $"{combat.health:0}");

						if (entity is BasePlayer)
						{
							tab.AddRange(column, "Thirst", 0, player.metabolism.hydration.max, ap => player.metabolism.hydration.value, (ap, value) => DoAll<BasePlayer>(e => e.metabolism.hydration.SetValue(value)), ap => $"{player.metabolism.hydration.value:0}");
							tab.AddRange(column, "Hunger", 0, player.metabolism.calories.max, ap => player.metabolism.calories.value, (ap, value) => DoAll<BasePlayer>(e => e.metabolism.calories.SetValue(value)), ap => $"{player.metabolism.calories.value:0}");
							tab.AddRange(column, "Radiation", 0, player.metabolism.radiation_poison.max, ap => player.metabolism.radiation_poison.value, (ap, value) => DoAll<BasePlayer>(e => e.metabolism.radiation_poison.SetValue(value)), ap => $"{player.metabolism.radiation_poison.value:0}");
							tab.AddRange(column, "Bleeding", 0, player.metabolism.bleeding.max, ap => player.metabolism.bleeding.value, (ap, value) => DoAll<BasePlayer>(e => e.metabolism.bleeding.SetValue(value)), ap => $"{player.metabolism.bleeding.value:0}");
						}
					}
				}
			}
			else
			{
				tab.ClearColumn(1);
				DrawEntities(tab, ap3);
			}

			void DoAll<T>(Action<T> callback) where T : BaseEntity
			{
				foreach (var selectedEntity in selectedEntitites)
				{
					if (selectedEntity == null) continue;

					callback?.Invoke((T)selectedEntity);
				}
			}
		}
		internal static void DrawEntityFlags(Tab tab, PlayerSession session, int column = 1)
		{
			var selectedEntitites = session.GetStorage(tab, "selectedentities", new List<BaseEntity>());

			tab.ClearColumn(column);
			if (selectedEntitites.Count == 0) return;

			var entity = selectedEntitites[0];

			var counter = 0;
			var currentButtons = Facepunch.Pool.GetList<Tab.OptionButton>();

			tab.ClearColumn(column);

			tab.AddName(column, "Entity Flags", TextAnchor.MiddleLeft);
			foreach (var flag in Enum.GetNames(typeof(BaseEntity.Flags)).OrderBy(x => x))
			{
				var flagValue = (BaseEntity.Flags)Enum.Parse(typeof(BaseEntity.Flags), flag);
				var isDifferent = selectedEntitites.All(x => x.HasFlag(flagValue));
				var hasFlag = entity.HasFlag(flagValue);

				currentButtons.Add(new Tab.OptionButton(flag, ap =>
				{
					DoAll<BaseEntity>(e => e.SetFlag(flagValue, !hasFlag));
					DrawEntitySettings(tab, 0, ap);
					DrawEntityFlags(tab, ap, column);
				}, ap => isDifferent ? Tab.OptionButton.Types.Warned : hasFlag ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
				counter++;

				if (counter >= 5)
				{
					tab.AddButtonArray(column, currentButtons.ToArray());
					currentButtons.Clear();
					counter = 0;
				}
			}

			Facepunch.Pool.FreeList(ref currentButtons);

			void DoAll<T>(Action<T> callback) where T : BaseEntity
			{
				foreach (var selectedEntity in selectedEntitites)
				{
					if (selectedEntity == null) continue;

					callback?.Invoke((T)selectedEntity);
				}
			}
		}

		internal static void ViewCamera(BasePlayer player, ComputerStation station, CCTV_RC camera)
		{
			player.net.SwitchSecondaryGroup(camera.net.group);
			station.currentlyControllingEnt.uid = camera.net.ID;
			station.currentPlayerID = player.userID;
			var b = camera.InitializeControl(new CameraViewerId(station.currentPlayerID, 0L));
			station.SetFlag(BaseEntity.Flags.Reserved2, b, recursive: false, networkupdate: false);
			station.SendNetworkUpdateImmediate();
			station.SendControlBookmarks(player);
		}
		internal static void SendEntityToPlayer(BasePlayer player, BaseEntity entity)
		{
			var connection = player.Connection;

			if (connection == null)
			{
				return;
			}

			var netWrite = Net.sv.StartWrite();

			if (netWrite == null)
			{
				return;
			}

			++connection.validate.entityUpdates;

			netWrite.PacketID(Message.Type.Entities);
			netWrite.UInt32(connection.validate.entityUpdates);

			entity.ToStreamForNetwork(netWrite, new BaseNetworkable.SaveInfo() { forConnection = connection, forDisk = false });
			netWrite.Send(new SendInfo(connection));
		}
	}
}

#endif
