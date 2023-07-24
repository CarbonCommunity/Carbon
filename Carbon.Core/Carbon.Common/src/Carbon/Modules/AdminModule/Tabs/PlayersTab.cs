#if !MINIMAL

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class AdminModule : CarbonModule<AdminConfig, AdminData>
{
	public class PlayersTab
	{
		internal static RustPlugin Core = Community.Runtime.CorePlugin;
		internal static List<BasePlayer> BlindedPlayers = new();

		public static Tab Get()
		{
			var players = new Tab("players", "Players", Community.Runtime.CorePlugin, (instance, tab) =>
			{
				tab.ClearColumn(1);
				RefreshPlayers(tab, instance);
			}, 1);

			players.AddColumn(0);
			players.AddColumn(1);

			return players;
		}

		public static void RefreshPlayers(Tab tab, PlayerSession ap)
		{
			tab.ClearColumn(0);

			if (Singleton.HasAccessLevel(ap.Player, 1))
			{
				tab.AddInput(0, "Search", ap => ap?.GetStorage<string>(tab, "playerfilter"), (ap2, args) => { ap2.SetStorage(tab, "playerfilter", args.ToString(" ")); RefreshPlayers(tab, ap2); });

				var onlinePlayers = BasePlayer.allPlayerList.Where(x => x.userID.IsSteamId() && x.IsConnected).OrderBy(x => x.Connection?.connectionTime);
				tab.AddName(0, $"Online ({onlinePlayers.Count():n0})");
				foreach (var player in onlinePlayers)
				{
					AddPlayer(tab, ap, player);
				}
				if (onlinePlayers.Count() == 0) tab.AddText(0, "No online players found.", 10, "1 1 1 0.4");

				var offlinePlayers = BasePlayer.allPlayerList.Where(x => x.userID.IsSteamId() && !x.IsConnected);
				tab.AddName(0, $"Offline ({offlinePlayers.Count():n0})");
				foreach (var player in offlinePlayers)
				{
					AddPlayer(tab, ap, player);
				}
				if (offlinePlayers.Count() == 0) tab.AddText(0, "No offline players found.", 10, "1 1 1 0.4");
			}
		}
		public static void AddPlayer(Tab tab, PlayerSession ap, BasePlayer player)
		{
			if (ap != null)
			{
				var filter = ap.GetStorage<string>(tab, "playerfilter");

				if (!string.IsNullOrEmpty(filter) && !(player.displayName.ToLower().Contains(filter.ToLower()) || player.UserIDString.Contains(filter))) return;
			}

			tab.AddButton(0, $"{player.displayName}", aap =>
			{
				ap.SetStorage(tab, "playerfilterpl", player);
				ShowInfo(tab, ap, player);
			}, aap => aap == null || !(aap.GetStorage<BasePlayer>(tab, "playerfilterpl", null) == player) ? Tab.OptionButton.Types.None : Tab.OptionButton.Types.Selected);
		}
		public static void ShowInfo(Tab tab, PlayerSession aap, BasePlayer player)
		{
			tab.ClearColumn(1);

			tab.AddName(1, $"Player Information", TextAnchor.MiddleLeft);
			tab.AddInput(1, "Name", ap => player.displayName, null);
			tab.AddInput(1, "Steam ID", ap => player.UserIDString, null);
			tab.AddInput(1, "Net ID", ap => $"{player.net?.ID}", null);
			try
			{
				var position = player.transform.position;
				tab.AddInput(1, "Position", ap => $"{player.transform.position}", null);
				tab.AddInput(1, "Rotation", ap => $"{player.transform.rotation}", null);
			}
			catch { }

			if (Singleton.HasAccessLevel(aap.Player, 3))
			{
				tab.AddName(1, $"Permissions", TextAnchor.MiddleLeft);
				{
					tab.AddButton(1, "View Permissions", ap =>
					{
						var perms = Singleton.FindTab("permissions");
						var permission = Community.Runtime.CorePlugin.permission;
						Singleton.SetTab(ap.Player, "permissions");

						ap.SetStorage(tab, "player", player.UserIDString);
						PermissionsTab.GeneratePlayers(perms, permission, ap);
						PermissionsTab.GeneratePlugins(perms, ap, permission, permission.FindUser(ap.Player.UserIDString), null);
					}, (ap) => Singleton.HasAccessLevel(player, 3) ? Tab.OptionButton.Types.None : Tab.OptionButton.Types.Important);
				}
			}

			if (Singleton.HasAccessLevel(aap.Player, 2))
			{
				tab.AddButtonArray(1, new Tab.OptionButton("Kick", ap =>
				{
					Singleton.Modal.Open(aap.Player, $"Kick {player.displayName}", new Dictionary<string, ModalModule.Modal.Field>
					{
						["reason"] = ModalModule.Modal.Field.Make("Reason", ModalModule.Modal.Field.FieldTypes.String, @default: "Stop doing that.")
					}, onConfirm: (p, m) =>
					{
						player.Kick(m.Get<string>("reason"));
					});
				}), new Tab.OptionButton("Ban", ap =>
				{
					Singleton.Modal.Open(aap.Player, $"Ban {player.displayName}", new Dictionary<string, ModalModule.Modal.Field>
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
						var then = now - date;

						player.AsIPlayer().Ban(m.Get<string>("reason"), then);
					});
				}));
			}

			tab.AddName(1, $"Actions", TextAnchor.MiddleLeft);

			tab.AddButtonArray(1,
				new Tab.OptionButton("TeleportTo", ap => { ap.Player.Teleport(player.transform.position); }),
				new Tab.OptionButton("Teleport2Me", ap =>
				{
					tab.CreateDialog($"Are you sure about that?", ap =>
					{
						player.transform.position = ap.Player.transform.position;
						player.SendNetworkUpdateImmediate();
					}, null);
				}));

			tab.AddButtonArray(1,
				new Tab.OptionButton("Loot", ap =>
				{
					EntitiesTab.LastContainerLooter = ap;
					ap.SetStorage(tab, "lootedent", player);
					EntitiesTab.SendEntityToPlayer(ap.Player, player);

					Core.timer.In(0.2f, () => Singleton.Close(ap.Player));
					Core.timer.In(0.5f, () =>
					{
						EntitiesTab.SendEntityToPlayer(ap.Player, player);

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
						player.Hurt(player.MaxHealth());
						player.Respawn();
						player.EndSleeping();
					}, null);
				}));

			if (Singleton.HasTab("entities"))
			{
				tab.AddButton(1, "Select Entity", ap2 =>
				{
					Singleton.SetTab(ap2.Player, "entities");
					var tab = Singleton.GetTab(ap2.Player);
					EntitiesTab.SelectEntity(tab, ap2, player);
					EntitiesTab.DrawEntities(tab, ap2);
					EntitiesTab.DrawEntitySettings(tab, 1, ap2);
				});
			}

			tab.AddInput(1, "PM", null, (ap, args) => { player.ChatMessage($"[{ap.Player.displayName}]: {args.ToString(" ")}"); });
			if (aap.Player != player && aap.Player.spectateFilter != player.UserIDString)
			{
				tab.AddButton(1, "Spectate", ap =>
				{
					StartSpectating(ap.Player, player);
					ShowInfo(tab, ap, player);
				});
			}
			if (!string.IsNullOrEmpty(aap.Player.spectateFilter) && (aap.Player.UserIDString == player.UserIDString || aap.Player.spectateFilter == player.UserIDString))
			{
				tab.AddButton(1, "End Spectating", ap =>
				{
					StopSpectating(ap.Player);
					ShowInfo(tab, ap, player);
				}, ap => Tab.OptionButton.Types.Selected);
			}
			if (!BlindedPlayers.Contains(player))
			{
				tab.AddButton(1, "Blind Player", ap =>
				{
					tab.CreateDialog("Are you sure you want to blind the player?", ap =>
					{
						using var cui = new CUI(Singleton.Handler);
						var container = cui.CreateContainer("blindingpanel", "0 0 0 1", needsCursor: true, needsKeyboard: Singleton.HandleEnableNeedsKeyboard(ap));
						cui.CreateClientImage(container, "blindingpanel", null, "https://carbonmod.gg/assets/media/cui/bsod.png", "1 1 1 1");
						cui.Send(container, player);
						BlindedPlayers.Add(player);
						ShowInfo(tab, ap, player);

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
					BlindedPlayers.Remove(player);
					ShowInfo(tab, ap, player);
				}, ap => Tab.OptionButton.Types.Selected);
			}

			tab.AddName(1, "Stats");
			tab.AddName(1, "Combat", TextAnchor.MiddleLeft);
			tab.AddRange(1, "Health", 0, player.MaxHealth(), ap => player.health, (ap, value) => player.SetHealth(value), ap => $"{player.health:0}");

			tab.AddRange(1, "Thirst", 0, player.metabolism.hydration.max, ap => player.metabolism.hydration.value, (ap, value) => player.metabolism.hydration.SetValue(value), ap => $"{player.metabolism.hydration.value:0}");
			tab.AddRange(1, "Hunger", 0, player.metabolism.calories.max, ap => player.metabolism.calories.value, (ap, value) => player.metabolism.calories.SetValue(value), ap => $"{player.metabolism.calories.value:0}");
			tab.AddRange(1, "Radiation", 0, player.metabolism.radiation_poison.max, ap => player.metabolism.radiation_poison.value, (ap, value) => player.metabolism.radiation_poison.SetValue(value), ap => $"{player.metabolism.radiation_poison.value:0}");
			tab.AddRange(1, "Bleeding", 0, player.metabolism.bleeding.max, ap => player.metabolism.bleeding.value, (ap, value) => player.metabolism.bleeding.SetValue(value), ap => $"{player.metabolism.bleeding.value:0}");
		}
	}
}

#endif
