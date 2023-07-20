/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class AdminModule : CarbonModule<AdminConfig, AdminData>
{
	public class PermissionsTab
	{
		internal static Permission permission;

		public static Tab Get()
		{
			permission = Community.Runtime.CorePlugin.permission;

			var tab = new Tab("permissions", "Permissions", Community.Runtime.CorePlugin, (ap, tab) =>
			{
				tab.ClearColumn(1);
				tab.ClearColumn(2);
				tab.ClearColumn(3);
				GeneratePlayers(tab, permission, ap);
			}, 3);

			tab.AddName(0, "Options", TextAnchor.MiddleLeft);

			tab.AddButton(0, "Players", ap =>
			{
				ap.SetStorage(tab, "groupedit", false);

				tab.ClearColumn(1);
				tab.ClearColumn(2);
				tab.ClearColumn(3);
				ap.Clear();

				ap.SetStorage(tab, "option", 0);

				GeneratePlayers(tab, permission, ap);
			}, type: (ap) => ap.GetStorage(tab, "option", 0) == 0 ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);

			GeneratePlayers(tab, permission, PlayerSession.Blank);

			tab.AddButton(0, "Groups", ap =>
			{
				ap.SetStorage(tab, "groupedit", false);

				tab.ClearColumn(1);
				tab.ClearColumn(2);
				tab.ClearColumn(3);
				ap.Clear();

				ap.ClearStorage(tab, "player");
				ap.ClearStorage(tab, "plugin");

				ap.SetStorage(tab, "option", 1);
				GenerateGroups(tab, permission, ap);
			}, type: (ap) => ap.GetStorage(tab, "option", 0) == 1 ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);
			tab.AddColumn(1);
			tab.AddColumn(2);
			tab.AddColumn(3);

			return tab;
		}

		public static void GeneratePlayers(Tab tab, Permission perms, PlayerSession ap)
		{
			var localPlayers = ap.GetStorage(tab, "localplayers", true);
			var filter = ap.GetStorage(tab, "playerfilter", string.Empty)?.Trim().ToLower();

			tab.ClearColumn(1);
			tab.AddName(1, "Players", TextAnchor.MiddleLeft);
			{
				tab.AddInput(1, "Search", ap => ap.GetStorage(tab, "playerfilter", string.Empty), (ap, args) =>
				{
					ap.SetStorage(tab, "playerfilter", args.ToString(" "));
					GeneratePlayers(tab, perms, ap);
				});
				tab.AddButtonArray(1, new Tab.OptionButton("Data Users", ap =>
				{
					localPlayers = ap.SetStorage(tab, "localplayers", !localPlayers);
					GeneratePlayers(tab, perms, ap);
				}, ap => !localPlayers ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("Add User", ap =>
				{
					Singleton.Modal.Open(ap.Player, "Create New User", new()
					{
						["steamid"] = ModalModule.Modal.Field.Make("Steam ID", ModalModule.Modal.Field.FieldTypes.String, true, customIsInvalid: field => !field.Get<string>().IsSteamId() ? "Not a valid Steam ID." : permission.UserExists(field.Get<string>()) ? "User with the same Steam ID already exists." : string.Empty),
						["displayname"] = ModalModule.Modal.Field.Make("Display Name", ModalModule.Modal.Field.FieldTypes.String)
					}, (pl, mod) =>
					{
						var user = permission.GetUserData(mod.Get<string>("steamid"));
						user.LastSeenNickname = mod.Get<string>("displayname");

						GeneratePlayers(tab, perms, ap);
					});
				}, ap => Tab.OptionButton.Types.None));

				if (localPlayers)
				{
					var players = BasePlayer.allPlayerList.Where(x =>
					{
						if (!x.userID.IsSteamId()) return false;

						if (!string.IsNullOrEmpty(filter))
						{
							return x.displayName.ToLower().Contains(filter) || x.UserIDString.Contains(filter);
						}

						return true;
					});

					foreach (var player in players)
					{
						tab.AddRow(1, new Tab.OptionButton($"{player.displayName} ({player.userID})", instance2 =>
						{
							ap.SetStorage(tab, "player", player.UserIDString);

							ap.ClearStorage(tab, "plugin");

							tab.ClearColumn(3);

							GeneratePlugins(tab, ap, perms, permission.FindUser(player.UserIDString), null);
						}, type: (_instance) => ap.GetStorage<string>(tab, "player", null) == player.UserIDString ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
					}
				}
				else
				{
					foreach (var player in permission.userdata)
					{
						if (player.Key.Contains(filter) || (!string.IsNullOrEmpty(player.Value.LastSeenNickname) && player.Value.LastSeenNickname.ToLower().Contains(filter)))
						{
							tab.AddRow(1, new Tab.OptionButton($"{(string.IsNullOrEmpty(player.Value.LastSeenNickname) ? "Unknown" : player.Value.LastSeenNickname)} ({player.Key})", instance2 =>
							{
								ap.SetStorage(tab, "player", player.Key);

								ap.ClearStorage(tab, "plugin");

								tab.ClearColumn(3);

								GeneratePlugins(tab, ap, perms, player, null);
							}, type: (_instance) => ap.GetStorage<string>(tab, "player", null) == player.Key ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
						}
					}
				}
			}
		}
		public static void GeneratePlugins(Tab tab, PlayerSession ap, Permission permission, KeyValuePair<string, UserData> player, string selectedGroup)
		{
			var groupEdit = ap.GetStorage<bool>(tab, "groupedit");
			var filter = ap.GetStorage(tab, "pluginfilter", string.Empty)?.Trim().ToLower();
			var plugins = ModLoader.LoadedPackages.SelectMany(x => x.Plugins).Where(x =>
			{
				if (!string.IsNullOrEmpty(filter))
				{
					return x.IsCorePlugin || x.permission.GetPermissions().Any(y => y.StartsWith(x.Name.ToLower()) && x.Name.Trim().ToLower().Contains(filter));
				}

				return x.IsCorePlugin || x.permission.GetPermissions().Any(y => y.StartsWith(x.Name.ToLower()));
			});

			tab.ClearColumn(2);
			if (string.IsNullOrEmpty(selectedGroup))
			{
				tab.AddName(2, $"{player.Value.LastSeenNickname}", TextAnchor.LowerCenter);
				tab.AddText(2, player.Key, 8, "1 1 1 0.6", align: TextAnchor.UpperCenter, isInput: true);

				var existentPlayer = BasePlayer.FindAwakeOrSleeping(player.Key);
				tab.AddButtonArray(2,
					new Tab.OptionButton("Select Player", (ap2) =>
					{
						Singleton.SetTab(ap.Player, "players");
						var tab = Singleton.GetTab(ap.Player);
						ap.SetStorage(tab, "playerfilterpl", player);
						PlayersTab.RefreshPlayers(tab, ap);
						PlayersTab.ShowInfo(tab, ap, existentPlayer);
					}, ap => Tab.OptionButton.Types.Warned),
					new Tab.OptionButton(groupEdit ? "Edit Plugins" : "Edit Groups", (ap2) =>
					{
						ap.SetStorage(tab, "groupedit", !groupEdit);
						GeneratePlugins(tab, ap, permission, player, null);
					}));
			}
			else
			{
				tab.AddName(2, $"{selectedGroup}");
				tab.AddButtonArray(2, new Tab.OptionButton("Delete", ap =>
				{
					tab.CreateDialog($"Are you sure you want to delete the '{selectedGroup}' group?", ap2 =>
					{
						permission.RemoveGroup(selectedGroup);

						tab.ClearColumn(1);
						tab.ClearColumn(2);
						tab.ClearColumn(3);
						GenerateGroups(tab, permission, ap);
					}, null);
				}, (ap) => Tab.OptionButton.Types.Important), new Tab.OptionButton("Edit", ap =>
				{
					var temp = Facepunch.Pool.GetList<string>();
					var groups = Community.Runtime.CorePlugin.permission.GetGroups();
					temp.Add("None");
					temp.AddRange(groups);
					temp.Remove(selectedGroup);

					var array = temp.ToArray();
					Facepunch.Pool.FreeList(ref temp);

					var parent = permission.GetGroupParent(selectedGroup);
					var parentIndex = Array.IndexOf(array, parent);
					tab.CreateModal(ap.Player, $"Editing '{selectedGroup}'", new Dictionary<string, ModalModule.Modal.Field>()
					{
						["name"] = ModalModule.Modal.Field.Make("Name", ModalModule.Modal.Field.FieldTypes.String, true, selectedGroup, true),
						["dname"] = ModalModule.Modal.Field.Make("Display Name", ModalModule.Modal.Field.FieldTypes.String, @default: permission.GetGroupTitle(selectedGroup)),
						["rank"] = ModalModule.Modal.Field.Make("Rank", ModalModule.Modal.Field.FieldTypes.Integer, @default: permission.GetGroupRank(selectedGroup)),
						["parent"] = ModalModule.Modal.EnumField.MakeEnum("Parent", array, @default: string.IsNullOrEmpty(parent) ? 0 : Array.IndexOf(array, parent), customIsInvalid: field => permission.GetGroupParent(array[field.Get<int>()]) == selectedGroup ? $"Circular parenting detected with '{array[field.Get<int>()]}'." : null)
					}, (ap2, modal) =>
					{
						var parentIndex = modal.Get<int>("parent");
						permission.SetGroupTitle(selectedGroup, modal.Get<string>("dname"));
						permission.SetGroupRank(selectedGroup, modal.Get<int>("rank"));
						if (parentIndex != 0) permission.SetGroupParent(selectedGroup, array[parentIndex]);
						else permission.SetGroupParent(selectedGroup, null);

						tab.ClearColumn(1);
						tab.ClearColumn(2);
						tab.ClearColumn(3);
						GenerateGroups(tab, permission, ap);
						GeneratePlugins(tab, ap, permission, permission.FindUser(ap.Player.UserIDString), selectedGroup);
					});
				}));
				tab.AddButton(2, "Duplicate Group", ap =>
				{
					var temp = Facepunch.Pool.GetList<string>();
					var groups = Community.Runtime.CorePlugin.permission.GetGroups();
					temp.Add("None");
					temp.AddRange(groups);

					var array = temp.ToArray();
					Facepunch.Pool.FreeList(ref temp);

					Singleton.Modal.Open(ap.Player, "Duplicate Group", new Dictionary<string, ModalModule.Modal.Field>
					{
						["name"] = ModalModule.Modal.Field.Make("Name", ModalModule.Modal.Field.FieldTypes.String, true, customIsInvalid: (field) => permission.GetGroups().Any(x => x == field.Get<string>()) ? "Group with that name already exists." : null),
						["dname"] = ModalModule.Modal.Field.Make("Display Name", ModalModule.Modal.Field.FieldTypes.String, @default: string.Empty),
						["rank"] = ModalModule.Modal.Field.Make("Rank", ModalModule.Modal.Field.FieldTypes.Integer, @default: 0),
						["parent"] = ModalModule.Modal.EnumField.MakeEnum("Parent", array, @default: 0)
					}, onConfirm: (BasePlayer p, ModalModule.Modal modal) =>
					{
						var name = modal.Get<string>("name");
						var parentIndex = modal.Get<int>("parent");
						permission.CreateGroup(name, modal.Get<string>("dname"), modal.Get<int>("rank"));
						if (parentIndex != 0) permission.SetGroupParent(modal.Get<string>("name"), array[parentIndex]);

						var perms = permission.GetGroupPermissions(selectedGroup);
						foreach (var perm in perms)
						{
							permission.GrantGroupPermission(name, perm, null);
						}

						tab.ClearColumn(1);
						tab.ClearColumn(2);
						tab.ClearColumn(3);
						GenerateGroups(tab, permission, ap);
					});
				}, ap => Tab.OptionButton.Types.None);
			}

			if (groupEdit)
			{
				tab.ClearColumn(3);

				tab.AddName(2, "Groups", TextAnchor.MiddleLeft);
				{
					tab.AddInput(2, "Search", ap => ap.GetStorage(tab, "groupfilter", string.Empty), (ap, args) =>
					{
						ap.SetStorage(tab, "groupfilter", args.ToString(" "));
						GeneratePlugins(tab, ap, permission, player, selectedGroup);
					});

					var groupFilter = ap.GetStorage<string>(tab, "groupfilter");

					foreach (var group in permission.GetGroups())
					{
						if (!string.IsNullOrEmpty(groupFilter) && !group.Contains(groupFilter)) continue;

						tab.AddButton(2, $"{group}", ap =>
						{
							if (permission.UserHasGroup(player.Key, group))
							{
								permission.RemoveUserGroup(player.Key, group);
							}
							else
							{
								permission.AddUserGroup(player.Key, group);
							}

							GeneratePlugins(tab, ap, permission, player, selectedGroup);
						}, type: (_instance) => permission.UserHasGroup(player.Key, group) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);
					}
				}
			}
			else
			{
				tab.AddName(2, "Plugins", TextAnchor.MiddleLeft);
				{
					tab.AddInput(2, "Search", ap => ap.GetStorage(tab, "pluginfilter", string.Empty), (ap, args) =>
					{
						ap.SetStorage(tab, "pluginfilter", args.ToString(" "));
						GeneratePlugins(tab, ap, permission, player, selectedGroup);
					});

					foreach (var plugin in plugins)
					{
						tab.AddRow(2, new Tab.OptionButton($"{plugin.Name} ({plugin.Version})", instance3 =>
						{
							ap.SetStorage(tab, "plugin", plugin);
							ap.SetStorage(tab, "pluginr", instance3.LastPressedRow);
							ap.SetStorage(tab, "pluginc", instance3.LastPressedColumn);

							GeneratePermissions(tab, permission, plugin, player, selectedGroup);
						}, type: (_instance) => ap.GetStorage<RustPlugin>(tab, "plugin", null) == plugin ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
					}
				}
			}
		}
		public static void GeneratePermissions(Tab tab, Permission perms, RustPlugin plugin, KeyValuePair<string, UserData> player, string selectedGroup)
		{
			tab.ClearColumn(3);
			tab.AddName(3, "Permissions", TextAnchor.MiddleLeft);
			foreach (var perm in perms.GetPermissions(plugin))
			{
				if (string.IsNullOrEmpty(selectedGroup))
				{
					var isInherited = false;
					var list = "";

					foreach (var group in perms.GetUserGroups(player.Key))
						if (perms.GroupHasPermission(group, perm))
						{
							isInherited = true;
							list += $"<b>{group}</b>, ";
						}

					tab.AddRow(3, new Tab.OptionButton($"{perm}", instance5 =>
					{
						if (perms.UserHasPermission(player.Key, perm))
							perms.RevokeUserPermission(player.Key, perm);
						else perms.GrantUserPermission(player.Key, perm, plugin);
					}, type: (_instance) => isInherited ? Tab.OptionButton.Types.Important : perms.UserHasPermission(player.Key, perm) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));

					if (isInherited)
					{
						tab.AddText(3, $"Inherited by the following groups: {list.TrimEnd(',', ' ')}", 8, "1 1 1 0.6", TextAnchor.UpperLeft, CUI.Handler.FontTypes.RobotoCondensedRegular);
					}
				}
				else
				{
					tab.AddRow(3, new Tab.OptionButton($"{perm}", instance5 =>
					{
						if (permission.GroupHasPermission(selectedGroup, perm))
							permission.RevokeGroupPermission(selectedGroup, perm);
						else permission.GrantGroupPermission(selectedGroup, perm, plugin);
					}, type: (_instance) => permission.GroupHasPermission(selectedGroup, perm) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
				}
			}

		}
		public static void GenerateGroups(Tab tab, Permission perms, PlayerSession ap)
		{
			tab.ClearColumn(1);
			tab.AddName(1, "Groups", TextAnchor.MiddleLeft);
			{
				tab.AddInput(1, "Search", ap => ap.GetStorage(tab, "groupfilter", string.Empty), (ap, args) =>
				{
					ap.SetStorage(tab, "groupfilter", args.ToString(" "));
					GenerateGroups(tab, perms, ap);
				});

				var groupFilter = ap.GetStorage<string>(tab, "groupfilter");

				tab.AddButton(1, "Add Group", ap =>
				{
					var temp = Facepunch.Pool.GetList<string>();
					var groups = Community.Runtime.CorePlugin.permission.GetGroups();
					temp.Add("None");
					temp.AddRange(groups);

					var array = temp.ToArray();
					Facepunch.Pool.FreeList(ref temp);

					Singleton.Modal.Open(ap.Player, "Create Group", new Dictionary<string, ModalModule.Modal.Field>()
					{
						["name"] = ModalModule.Modal.Field.Make("Name", ModalModule.Modal.Field.FieldTypes.String, true, customIsInvalid: (field) => perms.GetGroups().Any(x => x == field.Get<string>()) ? "Group with that name already exists." : null),
						["dname"] = ModalModule.Modal.Field.Make("Display Name", ModalModule.Modal.Field.FieldTypes.String, @default: string.Empty),
						["rank"] = ModalModule.Modal.Field.Make("Rank", ModalModule.Modal.Field.FieldTypes.Integer, @default: 0),
						["parent"] = ModalModule.Modal.EnumField.MakeEnum("Parent", array, @default: 0)
					}, onConfirm: (BasePlayer player, ModalModule.Modal modal) =>
					{
						var parentIndex = modal.Get<int>("parent");
						perms.CreateGroup(modal.Get<string>("name"), modal.Get<string>("dname"), modal.Get<int>("rank"));
						if (parentIndex != 0) perms.SetGroupParent(modal.Get<string>("name"), array[parentIndex]);

						tab.ClearColumn(1);
						tab.ClearColumn(2);
						tab.ClearColumn(3);
						GenerateGroups(tab, perms, ap);
					});
				}, (_instance) => Tab.OptionButton.Types.Warned);

				foreach (var group in permission.GetGroups())
				{
					if (!string.IsNullOrEmpty(groupFilter) && !group.Contains(groupFilter)) continue;

					tab.AddButton(1, $"{group}", instance2 =>
					{
						ap.SetStorage(tab, "group", group);
						ap.ClearStorage(tab, "plugin");

						tab.ClearColumn(2);
						tab.ClearColumn(3);

						GeneratePlugins(tab, ap, permission, permission.FindUser(ap.Player.UserIDString), group);
					}, type: (_instance) => ap.GetStorage(tab, "group", string.Empty) == group ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);
				}
			}
		}
	}
}
