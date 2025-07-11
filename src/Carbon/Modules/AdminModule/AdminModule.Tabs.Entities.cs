#if !MINIMAL

using API.Abstracts;
using Facepunch;
using Network;
using StringEx = Carbon.Extensions.StringEx;
using TimeEx = Carbon.Extensions.TimeEx;

namespace Carbon.Modules;

public partial class AdminModule
{
	public class EntitiesTab
	{
		internal static int EntityCount = 0;

		internal static RustPlugin Core = Community.Runtime.Core;
		internal static AdminModule Admin = GetModule<AdminModule>();
		internal static PlayerSession LastContainerLooter;
		internal static string[] BuildingGrades =
		[
			"Twig",
			"Wood",
			"Stone",
			"Metal",
			"Top Tier"
		];
		internal const string MultiselectionReplacement = "-";

		public static Tab Get()
		{
			var tab = new Tab("entities", "Entities", Community.Runtime.Core, (ap, tab2) => { tab2.ClearColumn(1); ResetSelection(tab2, ap); DrawEntities(tab2, ap); }, "entities.use");
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

			EntityCount = 0;

			var usedFilter = ap3.GetStorage(tab, "filter", string.Empty)?.ToLower()?.Trim();
			var validateFilter = ap3.GetStorage<Func<BaseEntity, bool>>(tab, "validatefilter");
			var maximumRange = (int)World.Size;
			var range = ap3.GetStorage(tab, "range", maximumRange);

			var map = BaseNetworkable.serverEntities.OfType<BaseEntity>().Where(entity =>
			{
				if (entity == null || entity.transform == null)
				{
					return false;
				}

				if (validateFilter != null && !validateFilter(entity))
				{
					return false;
				}

				if (range != -1 && (ap3.Player != null && Vector3.Distance(ap3.Player.transform.position, entity.transform.position) > range))
				{
					return false;
				}

				return entity.name.Contains(usedFilter, CompareOptions.OrdinalIgnoreCase)
				       || entity.GetType().Name.Contains(usedFilter, CompareOptions.OrdinalIgnoreCase)
				       || entity.OwnerID.ToString().Equals(usedFilter, StringComparison.OrdinalIgnoreCase)
				       || entity.skinID.ToString().Equals(usedFilter, StringComparison.OrdinalIgnoreCase);
			});
			EntityCount = string.IsNullOrEmpty(usedFilter) ? 0 : map.Count();

			tab.AddRange(0, "Range", 0, maximumRange, ap => range, (ap, value) => { try { ap.SetStorage(tab, "range", (int)value); DrawEntities(tab, ap); } catch (Exception ex) { Logger.Error($"Oof", ex); } }, ap => $"{range:0.0}m");
			tab.AddName(0, $"Entities  ({EntityCount:n0})", TextAnchor.MiddleLeft);

			var filter = ap3.GetStorage(tab, "filter", string.Empty);
			tab.AddButtonArray(0,
				new Tab.OptionButton("Players", ap => { ap.SetStorage(tab, "filter", nameof(BasePlayer)); DrawEntities(tab, ap); }, _ => filter == nameof(BasePlayer) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("Containers", ap => { ap.SetStorage(tab, "filter", nameof(StorageContainer)); ap.ClearStorage(tab, "validatefilter"); DrawEntities(tab, ap); }, _ => filter == nameof(StorageContainer) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("Deployables", ap => { ap.SetStorage(tab, "filter", nameof(Deployable)); ap.ClearStorage(tab, "validatefilter"); DrawEntities(tab, ap); }, _ => filter == nameof(Deployable) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("Collectibles", ap => { ap.SetStorage(tab, "filter", nameof(CollectibleEntity)); ap.ClearStorage(tab, "validatefilter"); DrawEntities(tab, ap); }, _ => filter == nameof(CollectibleEntity) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("NPCs", ap => { ap.SetStorage(tab, "filter", nameof(NPCPlayer)); ap.ClearStorage(tab, "validatefilter"); DrawEntities(tab, ap); }, _ => filter == nameof(NPCPlayer) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("I/O", ap => { ap.SetStorage(tab, "filter", nameof(IOEntity)); ap.ClearStorage(tab, "validatefilter"); DrawEntities(tab, ap); }, _ => filter == nameof(IOEntity) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));

			switch (ap3.GetStorage(tab, "filter", string.Empty))
			{
				case nameof(BasePlayer):
					tab.AddButtonArray(0,
						new Tab.OptionButton("Online", ap => { ap.SetStorage(tab, "filter", nameof(BasePlayer)); ap.SetStorage(tab, "validatefilter", new Func<BaseEntity, bool>(entity => entity is BasePlayer player && player.IsConnected)); DrawEntities(tab, ap); }),
						new Tab.OptionButton("Offline", ap => { ap.SetStorage(tab, "validatefilter", new Func<BaseEntity, bool>(entity => entity is BasePlayer player && !player.IsConnected)); DrawEntities(tab, ap); }),
						new Tab.OptionButton("Dead", ap => { ap.SetStorage(tab, "validatefilter", new Func<BaseEntity, bool>(entity => entity is BasePlayer player && player.IsDead())); DrawEntities(tab, ap); }));
					break;
			}

			if (!string.IsNullOrEmpty(usedFilter))
			{
				foreach (var entity in map)
				{
					var name = entity switch
					{
						BasePlayer player => player.displayName,
						_ => entity.ToString()
					};

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
			}

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
			var sameTypeSelection = selectedEntitites.All(x => x != null && entity != null && x.GetType() == entity.GetType());

			tab.AddName(column, "Hierarchy");

			if (column != 1) tab.AddButton(column, "<", ap => { DrawEntities(tab, ap); DrawEntitySettings(tab, 1, ap); }, ap => Tab.OptionButton.Types.Warned);

			if (entity != null && !entity.IsDestroyed)
			{
				var player = entity as BasePlayer;
				var owner = BasePlayer.FindByID(entity.OwnerID);

				if (player != ap3?.Player && Singleton.HasAccess(ap3.Player, "entities.kill_entity"))
				{
					tab.AddButtonArray(column,
						new Tab.OptionButton("Kill", ap =>
						{
							tab.CreateDialog($"Are you sure about that?", ap =>
							{
								DoAll<BaseEntity>(e => e.Kill());

								var selectedEntitites = ap3.GetStorage<List<BaseEntity>>(tab, "selectedentities");

								if (!ap3.HasStorage(tab, "selectedentities"))
								{
									selectedEntitites = ap3.SetStorage(tab, "selectedentities", new List<BaseEntity>());
								}

								selectedEntitites.Clear();

								DrawEntities(tab, ap);
								tab.ClearColumn(column);
							});
						}, ap => Tab.OptionButton.Types.Important),
						new Tab.OptionButton("Kill (Gibbed)", ap =>
						{
							tab.CreateDialog($"Are you sure about that?", ap =>
							{
								DoAll<BaseEntity>(e => e.Kill(BaseNetworkable.DestroyMode.Gib));

								var selectedEntitites = ap3.GetStorage<List<BaseEntity>>(tab, "selectedentities");

								if (!ap3.HasStorage(tab, "selectedentities"))
								{
									selectedEntitites = ap3.SetStorage(tab, "selectedentities", new List<BaseEntity>());
								}

								selectedEntitites.Clear();

								DrawEntities(tab, ap);
								tab.ClearColumn(column);
							}, null);
						}));
				}

				tab.AddInput(column, "Id", ap => multiSelection ? MultiselectionReplacement : $"{entity.net.ID} [<b>{entity.GetType().FullName}</b>]", null);
				tab.AddInput(column, "Name", ap => multiSelection ? MultiselectionReplacement : $"{entity.ShortPrefabName}", null);

				if (!multiSelection)
				{
					tab.AddInputButton(column, "Owner", 0.3f,
						new Tab.OptionInput(null, ap => $"{entity.OwnerID}", 0, !Singleton.HasAccess(ap3.Player, "entities.owner_change"), (ap, args) =>
						{
							var id = args.ToString(string.Empty).ToUlong();
							DoAll<BaseEntity>(e => e.OwnerID = id);

							DrawEntities(tab, ap);
							DrawEntitySettings(tab, 1, ap);
						}),
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
				tab.AddInput(column, "Skin", ap => multiSelection ? MultiselectionReplacement : entity.skinID.ToString(), callback: (session, enumerable) =>
				{
					entity.skinID = enumerable.ToString(" ").ToUlong();
					entity.SendNetworkUpdate();
				});
				tab.AddButton(column, "Edit Flags", ap => { DrawEntitySettings(tab, 0, ap); DrawEntityFlags(tab, ap, 1); });
				tab.AddInput(column, "Position", ap => multiSelection ? MultiselectionReplacement : $"{entity.transform.position} [{MapHelper.PositionToString(entity.transform.position)}]", null);
				tab.AddInput(column, "Rotation", ap => multiSelection ? MultiselectionReplacement : $"{entity.ServerRotation.eulerAngles}", null);

				if (sameTypeSelection)
				{
					if (!multiSelection && Singleton.HasAccess(ap3.Player, "entities.tp_entity"))
					{
						tab.AddButtonArray(column,
							new Tab.OptionButton("TeleportTo",
								ap => { ap.Player.Teleport(entity.transform.position); }),
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
							}),
							new Tab.OptionButton("Teleport2OwnedItem",
								ap =>
								{
									var entities = BaseEntity.Util.FindTargetsOwnedBy(player.userID, string.Empty);

									if (entities.Length > 0)
									{
										var randomEntity = entities[RandomEx.GetRandomInteger(0, entities.Length)];
										ap.Player.Teleport(randomEntity.transform.position);
									}
									else
									{
										Logger.Warn($" No entities owned by {player} could be found to teleport to.");
									}
								}));
					}

					if (entity is StorageContainer storage)
					{
						if (!multiSelection && Singleton.HasAccess(ap3.Player, "entities.loot_entity"))
						{
							tab.AddButton(column, "Loot Container", ap =>
							{
								LastContainerLooter = ap;

								ap.SetStorage(tab, "lootedent", entity);

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

									ap.Player.ClientRPC(RpcTarget.Player("RPC_OpenLootPanel", ap.Player), storage.panelName);
								});
							});
							tab.AddText(1, "To loot a backpack, drag the backpack item over any hotbar slots while looting an entity", 10, "1 1 1 0.4");
						}
					}

					if (entity is BasePlayer)
					{
						tab.AddInput(column, "Display Name",
							ap => multiSelection ? MultiselectionReplacement : player.displayName);
						tab.AddInput(column, "Steam ID",
							ap => multiSelection ? MultiselectionReplacement : player.UserIDString);
						if (Singleton.HasAccess(ap3.Player, "players.see_ips"))
						{
							tab.AddInput(column, "IP",
								ap => multiSelection ? MultiselectionReplacement : $"{player.net?.connection?.ipaddress}",
								null, hidden: true);
						}

						if (!multiSelection && (ap3.Player.IsAdmin || Singleton.Permissions.UserHasPermission(ap3?.Player.UserIDString, "carbon.cmod") ||
						                        player.userID.IsSteamId()))
						{
							tab.AddButtonArray(1, new Tab.OptionButton("Kick", ap =>
							{
								Singleton.Modal.Open(ap.Player, $"Kick {player.displayName}",
									new Dictionary<string, ModalModule.Modal.Field>
									{
										["reason"] = ModalModule.Modal.Field.Make("Reason",
											ModalModule.Modal.Field.FieldTypes.String, @default: "Stop doing that.")
									}, onConfirm: (p, m) =>
									{
										player.Kick(m.Get<string>("reason"));
									});
							}), new Tab.OptionButton("Ban", ap =>
							{
								Singleton.Modal.Open(ap.Player, $"Ban {player.displayName}",
									new Dictionary<string, ModalModule.Modal.Field>
									{
										["reason"] =
											ModalModule.Modal.Field.Make("Reason",
												ModalModule.Modal.Field.FieldTypes.String,
												@default: "Stop doing that."),
										["until"] = ModalModule.Modal.ButtonField.MakeButton("Until", "Select Date",
											m =>
											{
												Core.NextTick(() => Singleton.DatePicker.Draw(ap.Player,
													date => ap.SetStorage(tab, "date", date)));
											})
									}, onConfirm: (p, m) =>
									{
										var date = ap.GetStorage(tab, "date", DateTime.UtcNow.AddYears(100));
										var now = DateTime.UtcNow;
										date = new DateTime(date.Year, date.Month, date.Day, now.Hour, now.Minute,
											now.Second, DateTimeKind.Utc);

										if (now <= date) date = DateTime.UtcNow.AddYears(100);

										var then = now - date;

										player.AsIPlayer().Ban(m.Get<string>("reason"), then);
									});
							}), new Tab.OptionButton(player.IsSleeping() ? "End Sleep" : "Sleep", ap =>
							{
								if (player.IsSleeping())
								{
									player.EndSleeping();
								}
								else
								{
									player.StartSleeping();
								}

								DrawEntitySettings(tab, 1, ap);
							}), new Tab.OptionButton("Hostility", ap =>
							{
								var fields = new Dictionary<string, ModalModule.Modal.Field>
								{
									["duration"] = ModalModule.Modal.Field.Make("Duration",
										ModalModule.Modal.Field.FieldTypes.Float, true, 60f)
								};

								Singleton.Modal.Open(ap.Player, "Player Hostile", fields, (ap, modal) =>
								{
									var duration = modal.Get<float>("duration").Clamp(0f, float.MaxValue);
									player.State.unHostileTimestamp = Network.TimeEx.currentTimestamp + duration;
									player.DirtyPlayerState();
									player.ClientRPC(RpcTarget.Player("SetHostileLength", player), duration);
									fields.Clear();
									fields = null;
									SelectEntity(tab, ap3, owner);
									Singleton.Draw(ap3.Player);
								}, () =>
								{
									fields.Clear();
									fields = null;
								});
							}));
						}
						else tab.AddText(1, $"You need 'carbon.cmod' permission to kick, ban, sleep or change player hostility.",
							10, "1 1 1 0.4");

						var temp = Pool.Get<List<Tab.OptionButton>>();

						if (Singleton.HasAccess(ap3.Player, "entities.loot_players"))
						{
							temp.Add(new Tab.OptionButton("Loot", ap =>
							{
								if (multiSelection) return;

								OpenPlayerContainer(ap, player, tab);
							}));

							temp.Add(new Tab.OptionButton("Strip", ap =>
							{
								if (multiSelection) return;

								player.inventory.Strip();
							}));
						}

						if (Singleton.HasAccess(ap3.Player, "entities.respawn_players"))
						{
							temp.Add(new Tab.OptionButton("Respawn", ap =>
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
						}

						tab.AddButtonArray(column, temp.ToArray());

						Pool.FreeUnmanaged(ref temp);

						tab.AddText(1, "To loot a backpack, drag the backpack item over any hotbar slots while looting a player", 10, "1 1 1 0.4");

						if (Singleton.HasAccess(ap3.Player, "players.inventory_management"))
						{
							tab.AddName(1, "Inventory Lock");
							tab.AddButtonArray(1,
								new Tab.OptionButton("Main", ap =>
									{
										player.inventory.containerMain.SetLocked(!player.inventory.containerMain
											.IsLocked());
									},
									ap => player.inventory.containerMain.IsLocked()
										? Tab.OptionButton.Types.Important
										: Tab.OptionButton.Types.None),
								new Tab.OptionButton("Belt", ap =>
									{
										player.inventory.containerBelt.SetLocked(!player.inventory.containerBelt
											.IsLocked());
									},
									ap => player.inventory.containerBelt.IsLocked()
										? Tab.OptionButton.Types.Important
										: Tab.OptionButton.Types.None),
								new Tab.OptionButton("Wear", ap =>
									{
										player.inventory.containerWear.SetLocked(!player.inventory.containerWear
											.IsLocked());
									},
									ap => player.inventory.containerWear.IsLocked()
										? Tab.OptionButton.Types.Important
										: Tab.OptionButton.Types.None));
						}

						tab.AddInput(column, "PM", null, (ap, args) =>
						{
							DoAll<BasePlayer>(e =>
							{
								e.ChatMessage($"[{ap.Player.displayName}]: {args.ToString(" ")}");
							});
						});

						if (!multiSelection && ap3 != null && Singleton.HasAccess(ap3.Player, "entities.blind_players"))
						{
							if (!PlayersTab.BlindedPlayers.Contains(player))
							{
								tab.AddButton(1, "Blind Player", ap =>
								{
									tab.CreateDialog("Are you sure you want to blind the player?", ap =>
									{
										BlindPlayer(player);
										SelectEntity(tab, ap, entity);
										DrawEntitySettings(tab, column, ap3);

										if (ap.Player == player) Core.timer.In(1, () => { Singleton.Close(player); });
									}, null);
								});
							}
							else
							{
								tab.AddButton(1, "Unblind Player", ap =>
								{
									UnblindPlayer(player);
									EntitiesTab.SelectEntity(tab, ap, entity);
									DrawEntitySettings(tab, column, ap3);
								}, ap => Tab.OptionButton.Types.Selected);
							}
						}
					}

					if (Singleton.HasAccess(ap3.Player, "entities.spectate_players"))
					{
						if (!multiSelection && ap3.Player != player &&
						    (ap3.Player.spectateFilter != player?.UserIDString &&
						     ap3.Player.spectateFilter != entity.net.ID.ToString()))
						{
							tab.AddButton(1, "Spectate", ap =>
							{
								StartSpectating(ap.Player, entity);
								SelectEntity(tab, ap, entity);
								DrawEntitySettings(tab, column, ap3);
							});
						}

						if (!multiSelection && !string.IsNullOrEmpty(ap3.Player.spectateFilter) &&
						    (ap3.Player.UserIDString == player?.UserIDString ||
						     ap3.Player.spectateFilter == entity.net.ID.ToString()))
						{
							tab.AddButton(1, "End Spectating", ap =>
							{
								StopSpectating(ap.Player);
								SelectEntity(tab, ap, entity);
								DrawEntitySettings(tab, column, ap3);
							}, ap => Tab.OptionButton.Types.Selected);
						}
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
									tab.AddButton(column, "Open Fuel", ap => { LastContainerLooter = ap; Core.timer.In(0.2f, () => Admin.Close(ap.Player)); Core.timer.In(0.5f, () => { minicopter.engineController.FuelSystem.LootFuel(ap.Player); }); });
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
							tab.AddRange(column, "Thirst", 0, player.metabolism.hydration.max, _ => player.metabolism.hydration.value, (_, value) => DoAll<BasePlayer>(e => e.metabolism.hydration.SetValue(value)), _ => $"{player.metabolism.hydration.value:0}");
							tab.AddRange(column, "Hunger", 0, player.metabolism.calories.max, _ => player.metabolism.calories.value, (_, value) => DoAll<BasePlayer>(e => e.metabolism.calories.SetValue(value)), _ => $"{player.metabolism.calories.value:0}");
							tab.AddRange(column, "Radiation", 0, player.metabolism.radiation_poison.max, _ => player.metabolism.radiation_poison.value, (_, value) => DoAll<BasePlayer>(e => e.metabolism.radiation_poison.SetValue(value)), _ => $"{player.metabolism.radiation_poison.value:0}");
							tab.AddRange(column, "Bleeding", 0, player.metabolism.bleeding.max, _ => player.metabolism.bleeding.value, (_, value) => DoAll<BasePlayer>(e => e.metabolism.bleeding.SetValue(value)), _ => $"{player.metabolism.bleeding.value:0}");
							tab.AddRange(column, "Wetness", 0, player.metabolism.wetness.max * 10f, ap => player.metabolism.wetness.value * 10f, (_, value) => player.metabolism.wetness.SetValue(value * 0.1f), _ => $"{player.metabolism.wetness.value * 100f:0}%");
							tab.AddButton(column, "Empower Stats", _ =>
							{
								player.SetHealth(player.MaxHealth());
								player.metabolism.hydration.SetValue(player.metabolism.hydration.max);
								player.metabolism.calories.SetValue(player.metabolism.calories.max);
								player.metabolism.radiation_poison.SetValue(0);
								player.metabolism.bleeding.SetValue(0);
								player.metabolism.wetness.SetValue(0);
							});
						}
					}
				}
			}
			else
			{
				tab.ClearColumn(1);
				DrawEntities(tab, ap3);
			}

			return;

			void DoAll<T>(Action<T> callback) where T : BaseEntity
			{
				foreach (var selectedEntity in selectedEntitites.Where(selectedEntity => selectedEntity != null))
				{
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
			var currentButtons = Facepunch.Pool.Get<List<Tab.OptionButton>>();

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

			Facepunch.Pool.FreeUnmanaged(ref currentButtons);

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
