#if !MINIMAL

using ProtoBuf;

namespace Carbon.Modules;

public partial class AdminModule
{
	public class PermissionsTab
	{
		internal static Permission permission;

		public enum HookableTypes
		{
			Plugin,
			Module
		}

		public enum SortTypes
		{
			Loaded,
			Name,
			Version
		}

		public static string[] SortTypeNames = Enum.GetNames(typeof(SortTypes));

		public static Tab Get()
		{
			permission = Community.Runtime.Core.permission;

			var tab = new Tab("permissions", "Permissions", Community.Runtime.Core, (ap, tab) =>
			{
				ap.SetStorage(tab, "toggleall", true);
				ap.SetStorage(tab, "groupedit", false);

				tab.ClearColumn(1);
				tab.ClearColumn(2);
				tab.ClearColumn(3);
				ap.Clear();

				ap.SetStorage(tab, "pluginedit", false);
				ap.SetStorage(tab, "option", 0);

				GeneratePlayers(tab, permission, ap);
			}, "permissions.use");

			tab.AddName(0, "Options", TextAnchor.MiddleLeft);

			tab.AddButton(0, "Players", ap =>
			{
				ap.SetStorage(tab, "toggleall", true);
				ap.SetStorage(tab, "groupedit", false);

				tab.ClearColumn(1);
				tab.ClearColumn(2);
				tab.ClearColumn(3);
				ap.Clear();

				ap.SetStorage(tab, "pluginedit", false);
				ap.SetStorage(tab, "option", 0);

				GeneratePlayers(tab, permission, ap);
			}, type: (ap) => ap.GetStorage(tab, "option", 0) == 0 ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);

			GeneratePlayers(tab, permission, PlayerSession.Blank);

			tab.AddButton(0, "Groups", ap =>
			{
				ap.SetStorage(tab, "toggleall", true);
				ap.SetStorage(tab, "pluginedit", false);
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
						["displayname"] = ModalModule.Modal.Field.Make("Display Name", ModalModule.Modal.Field.FieldTypes.String),
						["language"] = ModalModule.Modal.Field.Make("Language", ModalModule.Modal.Field.FieldTypes.String)
					}, (pl, mod) =>
					{
						var user = permission.GetUserData(mod.Get<string>("steamid"), addIfNotExisting: true);
						user.LastSeenNickname = mod.Get<string>("displayname");
						user.Language = mod.Get<string>("language");

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

							GenerateHookables(tab, ap, perms, permission.FindUser(player.UserIDString), null, HookableTypes.Plugin);
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

								GenerateHookables(tab, ap, perms, player, null, HookableTypes.Plugin);
							}, type: (_instance) => ap.GetStorage<string>(tab, "player", null) == player.Key ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
						}
					}
				}
			}
		}
		public static void GenerateHookables(Tab tab, PlayerSession ap, Permission permission, KeyValuePair<string, UserData> player, string selectedGroup, HookableTypes hookableType)
		{
			var groupEdit = ap.GetStorage<bool>(tab, "groupedit");
			var pluginEdit = ap.GetStorage<bool>(tab, "pluginedit");
			var filter = ap.GetStorage(tab, "pluginfilter", string.Empty)?.Trim().ToLower();

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
						PlayersTab.ShowInfo(1, tab, ap, existentPlayer);
					}, ap => Tab.OptionButton.Types.Warned),
					new Tab.OptionButton(!groupEdit ? $"{(hookableType == HookableTypes.Plugin ? "▼ Modules" : "▼ Groups")}" : "▼ Plugins", (ap2) =>
					{
						if (groupEdit)
						{
							ap.SetStorage(tab, "groupedit", !groupEdit);
							GenerateHookables(tab, ap, permission, player, null, HookableTypes.Plugin);
						}
						else if (hookableType == HookableTypes.Plugin)
						{
							GenerateHookables(tab, ap, permission, player, null, HookableTypes.Module);
						}
						else
						{
							ap.SetStorage(tab, "groupedit", !groupEdit);
							GenerateHookables(tab, ap, permission, player, null, hookableType);
						}
					}),
					new Tab.OptionButton("Edit User", (ap2) =>
					{
						Singleton.Modal.Open(ap.Player, "Edit User", new()
						{
							["steamid"] = ModalModule.Modal.Field.Make("Steam ID", ModalModule.Modal.Field.FieldTypes.String, required: false, @default: player.Key, isReadOnly: true),
							["displayname"] = ModalModule.Modal.Field.Make("Display Name", ModalModule.Modal.Field.FieldTypes.String, @default: player.Value.LastSeenNickname, required: true),
							["language"] = ModalModule.Modal.Field.Make("Language", ModalModule.Modal.Field.FieldTypes.String, @default: player.Value.Language, required: true)
						}, (pl, mod) =>
						{
							var user = permission.GetUserData(player.Key);
							user.LastSeenNickname = mod.Get<string>("displayname");
							user.Language = mod.Get<string>("language");

							GeneratePlayers(tab, permission, ap);
						});
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
					var temp = Facepunch.Pool.Get<List<string>>();
					var groups = Community.Runtime.Core.permission.GetGroups();
					temp.Add("None");
					temp.AddRange(groups);
					temp.Remove(selectedGroup);

					var array = temp.ToArray();
					Facepunch.Pool.FreeUnmanaged(ref temp);

					var parent = permission.GetGroupParent(selectedGroup);
					var parentIndex = Array.IndexOf(array, parent);
					Singleton.Modal.Open(ap.Player, $"Editing '{selectedGroup}'", new Dictionary<string, ModalModule.Modal.Field>()
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
						GenerateHookables(tab, ap, permission, permission.FindUser(ap.Player.UserIDString), selectedGroup,hookableType);

						Singleton.NextFrame(() => Singleton.Draw(ap.Player));
					});
				}));
				tab.AddButtonArray(2,
					new Tab.OptionButton("Duplicate Group", ap =>
					{
						var temp = Facepunch.Pool.Get<List<string>>();
						var groups = Community.Runtime.Core.permission.GetGroups();
						temp.Add("None");
						temp.AddRange(groups);

						var array = temp.ToArray();
						Facepunch.Pool.FreeUnmanaged(ref temp);

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

							Singleton.NextFrame(() => Singleton.Draw(ap.Player));
						});
					}, ap => Tab.OptionButton.Types.None),
					new Tab.OptionButton(!pluginEdit ? hookableType == HookableTypes.Module ? "Players" : "Modules" : "Plugins", ap =>
					{
						if (pluginEdit)
						{
							ap.SetStorage(tab, "pluginedit", !pluginEdit);
							GenerateHookables(tab, ap, permission, player, selectedGroup, HookableTypes.Plugin);
						}
						else if (hookableType == HookableTypes.Plugin)
						{
							GenerateHookables(tab, ap, permission, player, selectedGroup, HookableTypes.Module);
						}
						else
						{
							ap.SetStorage(tab, "pluginedit", !pluginEdit);
							GenerateHookables(tab, ap, permission, player, selectedGroup, hookableType);
						}
					}, ap => Tab.OptionButton.Types.None));
			}

			if (groupEdit)
			{
				tab.ClearColumn(3);

				tab.AddName(2, "Groups", TextAnchor.MiddleLeft);
				{
					tab.AddInput(2, "Search", ap => ap.GetStorage(tab, "groupfilter", string.Empty), (ap, args) =>
					{
						ap.SetStorage(tab, "groupfilter", args.ToString(" "));
						GenerateHookables(tab, ap, permission, player, selectedGroup, hookableType);
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

							GenerateHookables(tab, ap, permission, player, selectedGroup, hookableType);
						}, type: (_instance) => permission.UserHasGroup(player.Key, group) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);
					}
				}
			}
			else if (!pluginEdit)
			{
				tab.AddName(2, hookableType == HookableTypes.Module ? "Modules" : "Plugins", TextAnchor.MiddleLeft);
				{
					tab.AddInput(2, "Search", ap => ap.GetStorage(tab, "pluginfilter", string.Empty), (ap, args) =>
					{
						ap.SetStorage(tab, "pluginfilter", args.ToString(" "));
						GenerateHookables(tab, ap, permission, player, selectedGroup, hookableType);
					});

					var sort = (SortTypes)ap.GetStorage(tab, "sorttype", 0);
					var sortFlip = ap.GetStorage(tab, "sortflip", false);

					tab.AddDropdown(2, "Sorting", ap => (int)sort, (session, index) =>
					{
						if ((int)sort != index)
						{
							ap.SetStorage(tab, "sortflip", false);
							ap.SetStorage(tab, "sorttype", index);
						}
						else
						{
							ap.SetStorage(tab, "sortflip", !sortFlip);
						}

						GenerateHookables(tab, ap, permission, player, selectedGroup, hookableType);
					}, SortTypeNames);

					var plugins = hookableType == HookableTypes.Plugin ? ModLoader.Packages.SelectMany(x => x.Plugins).Where(x =>
					{
						if (x.permission.permset.TryGetValue(x, out var perms))
						{
							if (!string.IsNullOrEmpty(filter))
							{
								return perms.Any(y => x.Name.Trim().ToLower().Contains(filter));
							}

							return perms.Count > 0;
						}

						return false;
					}).Select(x => x as BaseHookable) : Community.Runtime.ModuleProcessor.Modules.Where(x => {
						if (permission.permset.TryGetValue(x, out var perms))
						{
							if (!string.IsNullOrEmpty(filter))
							{
								return permission.GetPermissions().Any(y => x.Name.Trim().ToLower().Contains(filter));
							}

							return perms.Count > 0;
						}

						return false;
					});

					switch (sort)
					{
						case SortTypes.Name:
							plugins = plugins.OrderBy(x => x.Name);
							break;

						case SortTypes.Version:
							plugins = plugins.OrderBy(x => x.Version.ToString());
							break;
					}

					if (sortFlip)
					{
						plugins = plugins.Reverse();
					}

					foreach (var plugin in plugins)
					{
						tab.AddRow(2, new Tab.OptionButton($"{plugin.Name} ({plugin.Version})", instance3 =>
						{
							ap.SetStorage(tab, "toggleall", true);
							ap.SetStorage(tab, "plugin", plugin);
							ap.SetStorage(tab, "pluginr", instance3.LastPressedRow);
							ap.SetStorage(tab, "pluginc", instance3.LastPressedColumn);

							GeneratePermissions(tab, ap, permission, plugin, player, selectedGroup);
						}, type: (_instance) => ap.GetStorage<BaseHookable>(tab, "plugin", null) == plugin ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
					}
				}
			}
			else
			{
				tab.AddName(2, "Players", TextAnchor.MiddleLeft);
				{
					tab.AddInput(2, "Search", ap => ap.GetStorage(tab, "pluginfilter", string.Empty), (ap, args) =>
					{
						ap.SetStorage(tab, "pluginfilter", args.ToString(" "));
						GenerateHookables(tab, ap, permission, player, selectedGroup, hookableType);
					});

					var users = permission.userdata.Where(x =>
					{
						if (!x.Value.Groups.Contains(selectedGroup))
						{
							return false;
						}

						if (!string.IsNullOrEmpty(filter))
						{
							return x.Value.LastSeenNickname.ToLower().StartsWith(filter) || x.Key.Contains(filter);
						}

						return true;
					});

					foreach (var user in users)
					{
						tab.AddRow(2, new Tab.OptionButton($"{user.Value.LastSeenNickname} ({user.Key})", instance3 =>
						{
							ap.SetStorage(tab, "toggleall", true);
							ap.SetStorage(tab, "groupedit", false);
							ap.SetStorage(tab, "pluginedit", false);

							tab.ClearColumn(1);
							tab.ClearColumn(2);
							tab.ClearColumn(3);
							ap.Clear();

							ap.SetStorage(tab, "option", 0);
							ap.SetStorage(tab, "player", user.Key);

							GeneratePlayers(tab, permission, ap);
							GenerateHookables(tab, ap, permission, user, null, hookableType);

						}, type: (_instance) => Tab.OptionButton.Types.Selected));
					}
				}
			}
		}
		public static void GeneratePermissions(Tab tab,  PlayerSession ap, Permission perms, BaseHookable hookable, KeyValuePair<string, UserData> player, string selectedGroup)
		{
			var grantAllStatus = ap.GetStorage(tab, "toggleall", true);
			var filter = ap.GetStorage(tab, "permfilter", string.Empty)?.Trim().ToLower();

			tab.ClearColumn(3);
			tab.AddName(3, "Permissions", TextAnchor.MiddleLeft);
			tab.AddInput(3, "Search", ap => ap.GetStorage(tab, "permfilter", string.Empty), (ap, args) =>
			{
				ap.SetStorage(tab, "permfilter", args.ToString(" "));
				GeneratePermissions(tab, ap, perms, hookable, player, selectedGroup);
			});
			tab.AddButton(3, grantAllStatus ? "Grant All" : "Revoke All", ap =>
			{
				foreach (var perm in perms.GetPermissions(hookable))
				{
					if (string.IsNullOrEmpty(selectedGroup))
					{
						if (grantAllStatus)
						{
							if (!perms.UserHasPermission(player.Key, perm))
								perms.GrantUserPermission(player.Key, perm, hookable);
						}
						else
						{
							if (perms.UserHasPermission(player.Key, perm))
								perms.RevokeUserPermission(player.Key, perm);
						}
					}
					else
					{
						if (grantAllStatus)
						{
							if (!perms.GroupHasPermission(selectedGroup, perm))
								perms.GrantGroupPermission(selectedGroup, perm, hookable);
						}
						else
						{
							if (perms.GroupHasPermission(selectedGroup, perm))
								perms.RevokeGroupPermission(selectedGroup, perm);
						}
					}
				}

				ap.SetStorage(tab, "toggleall", !grantAllStatus);

				GeneratePermissions(tab, ap, permission, hookable, player, selectedGroup);
			}, ap => grantAllStatus ? Tab.OptionButton.Types.Warned : Tab.OptionButton.Types.Important);

			foreach (var perm in perms.GetPermissions(hookable))
			{
				if (!string.IsNullOrEmpty(filter) && !perm.Contains(filter, CompareOptions.OrdinalIgnoreCase))
				{
					continue;
				}

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
						else perms.GrantUserPermission(player.Key, perm, hookable);
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
						else permission.GrantGroupPermission(selectedGroup, perm, hookable);
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
					var temp = Facepunch.Pool.Get<List<string>>();
					var groups = Community.Runtime.Core.permission.GetGroups();
					temp.Add("None");
					temp.AddRange(groups);

					var array = temp.ToArray();
					Facepunch.Pool.FreeUnmanaged(ref temp);

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

						Singleton.NextFrame(() => Singleton.Draw(ap.Player));
					});
				}, (_instance) => Tab.OptionButton.Types.Warned);

				foreach (var group in permission.GetGroups().OrderBy(x => permission.GetGroupData(x).Rank))
				{
					if (!string.IsNullOrEmpty(groupFilter) && !group.Contains(groupFilter)) continue;

					var data = permission.GetGroupData(group);

					tab.AddButton(1, string.IsNullOrEmpty(data.Title) ? $"{group}" : $"{data.Title} ({group})", instance2 =>
					{
						ap.SetStorage(tab, "group", group);
						ap.ClearStorage(tab, "plugin");

						tab.ClearColumn(2);
						tab.ClearColumn(3);

						GenerateHookables(tab, ap, permission, permission.FindUser(ap.Player.UserIDString), group, HookableTypes.Plugin);
					}, type: (_instance) => ap.GetStorage(tab, "group", string.Empty) == group ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);
				}
			}
		}
	}
}

#endif
