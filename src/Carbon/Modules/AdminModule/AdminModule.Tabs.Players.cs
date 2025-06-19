#if !MINIMAL

using ProtoBuf;

namespace Carbon.Modules;

public partial class AdminModule
{
	readonly int[] _backpacks = new[]
	{
		-907422733,
		2068884361
	};

	public class PlayersTab
	{
		internal static List<BasePlayer> BlindedPlayers = new();

		public static Tab Get()
		{
			var players = new Tab("players", "Players", Community.Runtime.Core, (instance, tab) =>
			{
				tab.ClearColumn(1);
				RefreshPlayers(tab, instance);
			}, "players.use");

			players.AddColumn(0);
			players.AddColumn(1);

			return players;
		}

		public static void RefreshPlayers(Tab tab, PlayerSession ap)
		{
			tab.ClearColumn(0);

			tab.AddInput(0, "Search", ap => ap?.GetStorage<string>(tab, "playerfilter"), (ap2, args) =>
			{
				ap2.SetStorage(tab, "playerfilter", args.ToString(" "));
				RefreshPlayers(tab, ap2);
			});

			var onlinePlayers = BasePlayer.allPlayerList.Distinct().Where(x => x.userID.IsSteamId() && x.IsConnected)
				.OrderBy(x => x.Connection?.connectionTime);
			tab.AddName(0, $"Online ({onlinePlayers.Count():n0})");
			foreach (var player in onlinePlayers)
			{
				AddPlayer(tab, ap, player);
			}

			if (onlinePlayers.Count() == 0) tab.AddText(0, "No online players found.", 10, "1 1 1 0.4");

			var offlinePlayers = BasePlayer.allPlayerList.Distinct().Where(x => x.userID.IsSteamId() && !x.IsConnected);
			tab.AddName(0, $"Offline ({offlinePlayers.Count():n0})");
			foreach (var player in offlinePlayers)
			{
				AddPlayer(tab, ap, player);
			}

			if (offlinePlayers.Count() == 0) tab.AddText(0, "No offline players found.", 10, "1 1 1 0.4");
		}

		public static void AddPlayer(Tab tab, PlayerSession ap, BasePlayer player)
		{
			if (ap != null)
			{
				var filter = ap.GetStorage<string>(tab, "playerfilter");

				if (!string.IsNullOrEmpty(filter) && !(player.displayName.ToLower().Contains(filter.ToLower()) || player.UserIDString.Contains(filter))) return;
			}

			tab.AddButton(0, $"{player.displayName}", _ =>
			{
				ap.SetStorage(tab, "playerfilterpl", player);
				ShowInfo(1, tab, ap, player);
			}, aap => aap == null || !(aap.GetStorage<BasePlayer>(tab, "playerfilterpl", null) == player) ? Tab.OptionButton.Types.None : Tab.OptionButton.Types.Selected);
		}
		public static void ShowInfo(int column, Tab tab, PlayerSession aap, BasePlayer player)
		{
			tab.ClearColumn(column);

			if (column != 1)
			{
				tab.AddButton(column, "<", ap =>
				{
					RefreshPlayers(tab, ap);
					ShowInfo(1, tab, ap, player);
				});
			}

			tab.AddName(column, $"Player Information", TextAnchor.MiddleLeft);
			tab.AddInput(column, "Name", _ => player.displayName, (_, args) =>
			{
				player.AsIPlayer().Rename(args.ToString(" "));
			});
			tab.AddInput(column, "Steam ID", _ => player.UserIDString, null);
			tab.AddInput(column, "Net ID", _ => $"{player.net?.ID}", null);
			if (Singleton.HasAccess(aap.Player, "players.see_ips"))
			{
				tab.AddInput(column, "IP", _ => $"{player.net?.connection?.ipaddress}", null, hidden: true);
			}
			try
			{
				var position = player.transform.position;
				tab.AddInput(column, "Position", _ => $"{position} [{MapHelper.PositionToGrid(position)}]", null);
			}
			catch { }

			tab.AddButton(column, "Player Flags", ap =>
			{
				ShowInfo(0, tab, ap, player);
				PlayerFlags(1, tab, player);
			});

			if (Singleton.HasAccess(aap.Player, "permissions.use"))
			{
				tab.AddName(column, $"Permissions", TextAnchor.MiddleLeft);
				{
					tab.AddButton(column, "View Permissions", ap =>
					{
						var perms = Singleton.FindTab("permissions");
						var permission = Community.Runtime.Core.permission;
						Singleton.SetTab(ap.Player, "permissions");

						ap.SetStorage(tab, "player", player.UserIDString);
						PermissionsTab.GeneratePlayers(perms, permission, ap);
						PermissionsTab.GenerateHookables(perms, ap, permission, permission.FindUser(player.UserIDString), null, PermissionsTab.HookableTypes.Plugin);
					}, _ => Tab.OptionButton.Types.Important);
				}
			}

			if (aap.Player.IsAdmin || Singleton.Permissions.UserHasPermission(aap.Player.UserIDString, "carbon.cmod"))
			{
				tab.AddButtonArray(column, new Tab.OptionButton("Kick", _ =>
				{
					Singleton.Modal.Open(aap.Player, $"Kick {player.displayName}", new Dictionary<string, ModalModule.Modal.Field>
					{
						["reason"] = ModalModule.Modal.Field.Make("Reason", ModalModule.Modal.Field.FieldTypes.String, @default: "Stop doing that.")
					}, onConfirm: (_, m) =>
					{
						player.Kick(m.Get<string>("reason"));
					});
				}), new Tab.OptionButton("Ban", ap =>
				{
					Singleton.Modal.Open(aap.Player, $"Ban {player.displayName}", new Dictionary<string, ModalModule.Modal.Field>
					{
						["reason"] = ModalModule.Modal.Field.Make("Reason", ModalModule.Modal.Field.FieldTypes.String, @default: "Stop doing that."),
						["until"] = ModalModule.Modal.ButtonField.MakeButton("Until", "Select Date", _ =>
						{
							Core.NextTick(() => Singleton.DatePicker.Draw(ap.Player, date => ap.SetStorage(tab, "date", date)));
						})
					}, onConfirm: (_, m) =>
					{
						var date = ap.GetStorage(tab, "date", DateTime.UtcNow.AddYears(100));
						var now = DateTime.UtcNow;
						date = new DateTime(date.Year, date.Month, date.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
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

					ShowInfo(column, tab, ap, player);
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
						ShowInfo(column, tab, aap, player);
						Singleton.Draw(aap.Player);
					}, () =>
					{
						fields.Clear();
						fields = null;
					});
				}));
			}
			else tab.AddText(column, $"You need 'carbon.cmod' permission to kick, ban, sleep or change player hostility",
				10, "1 1 1 0.4");

			tab.AddName(column, $"Actions", TextAnchor.MiddleLeft);

			if (Singleton.HasAccess(aap.Player, "entities.tp_entity"))
			{
				tab.AddButtonArray(column,
					new Tab.OptionButton("TeleportTo", ap => { ap.Player.Teleport(player.transform.position); }),
					new Tab.OptionButton("Teleport2Me", _ =>
					{
						tab.CreateDialog($"Are you sure about that?", ap =>
						{
							player.Teleport(ap.Player.transform.position);
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

			if (Singleton.HasAccess(aap.Player, "entities.loot_players"))
			{
				tab.AddButtonArray(column,
					new Tab.OptionButton("Loot", ap =>
					{
						OpenPlayerContainer(ap, player, tab);
					}),
					new Tab.OptionButton("Strip", ap =>
					{
						player.inventory.Strip();
					}),
					new Tab.OptionButton("Respawn", _ =>
					{
						tab.CreateDialog($"Are you sure about that?", _ =>
						{
							player.Hurt(player.MaxHealth());
							player.Respawn();
							player.EndSleeping();
						}, null);
					}));
				tab.AddText(column, "To loot a backpack, drag the backpack item over any hotbar slots while looting a player", 10, "1 1 1 0.4");
			}

			if (Singleton.HasAccess(aap.Player, "players.inventory_management"))
			{
				tab.AddName(column, "Inventory Lock");
				tab.AddButtonArray(column,
					new Tab.OptionButton("Main", _ =>
						{
							player.inventory.containerMain.SetLocked(!player.inventory.containerMain.IsLocked());
						},
						_ => player.inventory.containerMain.IsLocked()
							? Tab.OptionButton.Types.Important
							: Tab.OptionButton.Types.None),
					new Tab.OptionButton("Belt", _ =>
						{
							player.inventory.containerBelt.SetLocked(!player.inventory.containerBelt.IsLocked());
						},
						_ => player.inventory.containerBelt.IsLocked()
							? Tab.OptionButton.Types.Important
							: Tab.OptionButton.Types.None),
					new Tab.OptionButton("Wear", _ =>
						{
							player.inventory.containerWear.SetLocked(!player.inventory.containerWear.IsLocked());
						},
						_ => player.inventory.containerWear.IsLocked()
							? Tab.OptionButton.Types.Important
							: Tab.OptionButton.Types.None));
			}

			if (Singleton.HasTab("entities"))
			{
				tab.AddButton(column, "Select Entity", ap2 =>
				{
					Singleton.SetTab(ap2.Player, "entities");
					var tab = Singleton.GetTab(ap2.Player);
					EntitiesTab.SelectEntity(tab, ap2, player);
					EntitiesTab.DrawEntities(tab, ap2);
					EntitiesTab.DrawEntitySettings(tab, 1, ap2);
				});
			}

			tab.AddInput(column, "PM", null, (ap, args) => { player.ChatMessage($"[{ap.Player.displayName}]: {args.ToString(" ")}"); });
			if (aap.Player != player && aap.Player.spectateFilter != player.UserIDString)
			{
				tab.AddButton(column, "Spectate", ap =>
				{
					StartSpectating(ap.Player, player);
					ShowInfo(column, tab, ap, player);
				});
			}

			if (Singleton.HasAccess(aap.Player, "entities.spectate_players"))
			{
				if (!string.IsNullOrEmpty(aap.Player.spectateFilter) &&
				    (aap.Player.UserIDString == player.UserIDString ||
				     aap.Player.spectateFilter == player.UserIDString))
				{
					tab.AddButton(column, "End Spectating", ap =>
					{
						StopSpectating(ap.Player);
						ShowInfo(column, tab, ap, player);
					}, _ => Tab.OptionButton.Types.Selected);
				}
			}

			if (Singleton.HasAccess(aap.Player, "entities.blind_players"))
			{
				if (!BlindedPlayers.Contains(player))
				{
					tab.AddButton(column, "Blind Player", _ =>
					{
						tab.CreateDialog("Are you sure you want to blind the player?", ap =>
						{
							using var cui = new CUI(Singleton.Handler);
							var container = cui.CreateContainer("blindingpanel", "0 0 0 1", needsCursor: true,
								needsKeyboard: Singleton.HandleEnableNeedsKeyboard(ap));
							cui.CreateImage(container, "blindingpanel", "bsod", "1 1 1 1");
							cui.Send(container, player);
							BlindedPlayers.Add(player);
							ShowInfo(column, tab, ap, player);

							if (ap.Player == player) Core.timer.In(1, () => { Singleton.Close(player); });
						}, null);
					});
				}
				else
				{
					tab.AddButton(column, "Unblind Player", ap =>
					{
						using var cui = new CUI(Singleton.Handler);
						cui.Destroy("blindingpanel", player);
						BlindedPlayers.Remove(player);
						ShowInfo(column, tab, ap, player);
					}, _ => Tab.OptionButton.Types.Selected);
				}
			}

			tab.AddName(column, "Stats");
			tab.AddName(column, "Combat");
			tab.AddRange(column, "Health", 0, player.MaxHealth(), _ => player.health, (_, value) => player.SetHealth(value), _ => $"{player.health:0}");

			tab.AddRange(column, "Thirst", 0, player.metabolism.hydration.max, _ => player.metabolism.hydration.value, (_, value) => player.metabolism.hydration.SetValue(value), _ => $"{player.metabolism.hydration.value:0}");
			tab.AddRange(column, "Hunger", 0, player.metabolism.calories.max, _ => player.metabolism.calories.value, (_, value) => player.metabolism.calories.SetValue(value), _ => $"{player.metabolism.calories.value:0}");
			tab.AddRange(column, "Radiation", 0, player.metabolism.radiation_poison.max, _ => player.metabolism.radiation_poison.value, (_, value) => player.metabolism.radiation_poison.SetValue(value), _ => $"{player.metabolism.radiation_poison.value:0}");
			tab.AddRange(column, "Bleeding", 0, player.metabolism.bleeding.max, _ => player.metabolism.bleeding.value, (_, value) => player.metabolism.bleeding.SetValue(value), _ => $"{player.metabolism.bleeding.value:0}");
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

			if (Singleton.HasAccess(aap.Player, "players.craft_queue"))
			{
				tab.AddName(column, "Crafting");

				var queue = player.inventory.crafting.queue.Where(x => !x.cancelled);
				foreach (var craft in queue)
				{
					tab.AddInputButton(column,
						$"{craft.blueprint.targetItem.displayName.english} (x{craft.amount}, {TimeEx.Format(craft.endTime - UnityEngine.Time.realtimeSinceStartup)})",
						0.1f,
						new Tab.OptionInput(null,
							_ =>
								$"<size=8>{craft.takenItems.Select(x => $"{x.info.displayName.english} x {x.amount}").ToString(", ")}</size>",
							0, true, null),
						new Tab.OptionButton("X", TextAnchor.MiddleCenter, ap =>
						{
							player.inventory.crafting.CancelTask(craft.taskUID);
							ShowInfo(column, tab, ap, player);
						}, _ => Tab.OptionButton.Types.Important));
				}

				if (!queue.Any())
				{
					tab.AddText(column, "No crafts.", 8, "1 1 1 0.5");
				}
			}
		}
		public static void PlayerFlags(int column, Tab tab, BasePlayer player)
		{
			tab.ClearColumn(column);

			var counter = 0;
			var currentButtons = Facepunch.Pool.Get<List<Tab.OptionButton>>();

			tab.ClearColumn(column);

			tab.AddName(column, "Player Flags", TextAnchor.MiddleLeft);
			foreach (var flag in Enum.GetNames(typeof(BasePlayer.PlayerFlags)).OrderBy(x => x))
			{
				var flagValue = (BasePlayer.PlayerFlags)Enum.Parse(typeof(BasePlayer.PlayerFlags), flag);
				var hasFlag = player.HasPlayerFlag(flagValue);

				currentButtons.Add(new Tab.OptionButton(flag, ap =>
				{
					player.SetPlayerFlag(flagValue, !hasFlag);
					ShowInfo(0, tab, ap, player);
					PlayerFlags(column, tab, player);
				}, ap => hasFlag ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
				counter++;

				if (counter >= 5)
				{
					tab.AddButtonArray(column, currentButtons.ToArray());
					currentButtons.Clear();
					counter = 0;
				}
			}

			Facepunch.Pool.FreeUnmanaged(ref currentButtons);
		}
	}
}

#endif
