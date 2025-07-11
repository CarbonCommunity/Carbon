using System.Xml.Linq;
using Facepunch;

namespace Carbon.Core;

public partial class CorePlugin
{
	[CommandVar("default_player_group", "The default group for any player with the regular authority level they get assigned to.")]
	[AuthLevel(2)]
	private string DefaultPlayerGroup
	{
		get
		{
			return Community.Runtime.Config.Permissions.PlayerDefaultGroup;
		}
		set
		{
			Community.Runtime.Config.Permissions.PlayerDefaultGroup = value;
			Community.Runtime.SaveConfig();
		}
	}

	[CommandVar("default_admin_group", "The default group players with auth-level 2 get assigned to.")]
	[AuthLevel(2)]
	private string DefaultAdminGroup
	{
		get
		{
			return Community.Runtime.Config.Permissions.AdminDefaultGroup;
		}
		set
		{
			Community.Runtime.Config.Permissions.AdminDefaultGroup = value;
			Community.Runtime.SaveConfig();
		}
	}

	[CommandVar("default_mod_group", "The default group players with auth-level 1 get assigned to.")]
	[AuthLevel(2)]
	private string DefaultModeratorGroup
	{
		get
		{
			return Community.Runtime.Config.Permissions.ModeratorDefaultGroup;

		}
		set
		{
			Community.Runtime.Config.Permissions.ModeratorDefaultGroup = value;
			Community.Runtime.SaveConfig();
		}
	}

	[CommandVar("autogrant_player_group", "Carbon should automatically grant (newer) players the default player group to them.")]
	[AuthLevel(2)]
	private bool AutoGrantPlayerGroup
	{
		get
		{
			return Community.Runtime.Config.Permissions.AutoGrantPlayerGroup;
		}
		set
		{
			Community.Runtime.Config.Permissions.AutoGrantPlayerGroup = value;
			Community.Runtime.SaveConfig();
		}
	}

	[CommandVar("autogrant_admin_group", "Carbon should automatically grant (auth level 2) players the default admin group to them.")]
	[AuthLevel(2)]
	private bool AutoGrantAdminGroup
	{
		get
		{
			return Community.Runtime.Config.Permissions.AutoGrantAdminGroup;
		}
		set
		{
			Community.Runtime.Config.Permissions.AutoGrantAdminGroup = value;
			Community.Runtime.SaveConfig();
		}
	}

	[CommandVar("autogrant_mod_group", "Carbon should automatically grant (auth level 1) players the default moderator group to them.")]
	[AuthLevel(2)]
	private bool AutoGrantModeratorGroup
	{
		get
		{
			return Community.Runtime.Config.Permissions.AutoGrantModeratorGroup;
		}
		set
		{
			Community.Runtime.Config.Permissions.AutoGrantModeratorGroup = value;
			Community.Runtime.SaveConfig();
		}
	}

	[ConsoleCommand("grant", "Grant one or more permissions to users or groups. Do 'c.grant' for syntax info.")]
	[AuthLevel(2)]
	private void Grant(ConsoleSystem.Arg arg)
	{
		static void PrintWarn(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith($"Syntax: c.grant <user|group> <name|id> <perm>\n" +
						  $"Syntax: c.grant <user|group> <name|id> *");
		}

		if (!arg.HasArgs(3))
		{
			PrintWarn(arg);
			return;
		}

		var action = arg.GetString(0);
		var name = arg.GetString(1);
		var perm = arg.GetString(2);
		var user = permission.FindUser(name);

		if (!permission.PermissionExists(perm))
		{
			arg.ReplyWith($"Couldn't grant permission - permission does not exist.");
			return;
		}

		var wildcard = perm.Equals(Permission.StarStr);

		switch (action)
		{
			case "user":
				if (string.IsNullOrEmpty(user.Key))
				{
					arg.ReplyWith($"Couldn't grant user permission - user not found, use full name or steam ID.");
				}
				else if (permission.UserHasPermission(user.Key, perm) && !wildcard)
				{
					arg.ReplyWith($"Already has that permission assigned.");
				}
				else if (wildcard)
				{
					var currentPerms = permission.GetUserPermissions(user.Key);

					if (permission.GrantUserPermission(user.Key, perm, null))
					{
						var affectedPerms = permission.GetUserPermissions(user.Key).Except(currentPerms);
						var affectedCount = affectedPerms.Count();

						arg.ReplyWith($"Granted user '{user.Value.LastSeenNickname}' {affectedCount:n0} {affectedCount.Plural("permission", "permissions")}: {affectedPerms.ToString(", ")}");
					}
					else
					{
						arg.ReplyWith($"Couldn't grant user permissions - most likely because they're all already granted.");
					}
				}
				else if (permission.GrantUserPermission(user.Key, perm, null))
				{
					arg.ReplyWith($"Granted user '{user.Value.LastSeenNickname}' permission '{perm}'");
				}
				else
				{
					arg.ReplyWith($"Couldn't grant user permission.");
				}

				break;

			case "group":
				if (!permission.GroupExists(name))
				{
					arg.ReplyWith($"Couldn't grant group permission - group not found, use full name.");
				}
				else if (permission.GroupHasPermission(name, perm) && !wildcard)
				{
					arg.ReplyWith($"Already has that permission assigned.");
				}
				else if (wildcard)
				{
					var currentPerms = permission.GetGroupPermissions(name);

					if (permission.GrantGroupPermission(name, perm, null))
					{
						var affectedPerms = permission.GetGroupPermissions(name).Except(currentPerms);
						var affectedCount = affectedPerms.Count();

						arg.ReplyWith($"Granted group '{name}' {affectedCount:n0} {affectedCount.Plural("permission", "permissions")}: {affectedPerms.ToString(", ")}");
					}
					else
					{
						arg.ReplyWith($"Couldn't grant group permissions - most likely because they're all already granted.");
					}
				}
				else if (permission.GrantGroupPermission(name, perm, null))
				{
					arg.ReplyWith($"Granted group '{name}' permission '{perm}'");
				}
				else
				{
					arg.ReplyWith($"Couldn't grant group permission.");
				}
				break;

			default:
				PrintWarn(arg);
				break;
		}
	}

	[ConsoleCommand("revoke", "Revoke one or more permissions from users or groups. Do 'c.revoke' for syntax info.")]
	[AuthLevel(2)]
	private void Revoke(ConsoleSystem.Arg arg)
	{
		static void PrintWarn(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith($"Syntax: c.revoke <user|group> <name|id> <perm>\n" +
						  $"Syntax: c.revoke <user|group> <name|id> *");
		}

		if (!arg.HasArgs(3))
		{
			PrintWarn(arg);
			return;
		}

		var action = arg.GetString(0);
		var name = arg.GetString(1);
		var perm = arg.GetString(2);
		var user = permission.FindUser(name);

		if (!permission.PermissionExists(perm))
		{
			arg.ReplyWith($"Couldn't revoke permission - permission does not exist.");
			return;
		}

		var wildcard = perm.Equals(Permission.StarStr);

		switch (action)
		{
			case "user":
				if (string.IsNullOrEmpty(user.Key))
				{
					arg.ReplyWith($"Couldn't revoke user permission - user not found, use full name or steam ID.");
				}
				else if (!permission.UserHasPermission(user.Key, perm) && !wildcard)
				{
					arg.ReplyWith($"User does not have that permission assigned.");
				}
				else if (wildcard)
				{
					var currentPerms = permission.GetUserPermissions(user.Key);

					if (permission.RevokeUserPermission(user.Key, perm))
					{
						var affectedPerms = currentPerms.Except(permission.GetUserPermissions(user.Key));
						var affectedCount = affectedPerms.Count();

						arg.ReplyWith($"Revoked user '{user.Value.LastSeenNickname}' {affectedCount:n0} {affectedCount.Plural("permission", "permissions")}: {affectedPerms.ToString(", ")}");
					}
					else
					{
						arg.ReplyWith($"Couldn't revoke user permissions - most likely because none of them are granted.");
					}
				}
				else if (permission.RevokeUserPermission(user.Key, perm))
				{
					arg.ReplyWith($"Revoked user '{user.Value?.LastSeenNickname}' permission '{perm}'");
				}
				else
				{
					arg.ReplyWith($"Couldn't revoke user permission.");
				}
				break;

			case "group":
				if (!permission.GroupExists(name))
				{
					arg.ReplyWith($"Couldn't revoke group permission - group not found, use full name.");
				}
				else if (!permission.GroupHasPermission(name, perm) && !wildcard)
				{
					arg.ReplyWith($"Group does not have that permission assigned.");
				}
				else if (wildcard)
				{
					var currentPerms = permission.GetGroupPermissions(name);

					if (permission.RevokeGroupPermission(name, perm))
					{
						var affectedPerms = currentPerms.Except(permission.GetGroupPermissions(name));
						var affectedCount = affectedPerms.Count();

						arg.ReplyWith($"Revoked group '{name}' {affectedCount:n0} {affectedCount.Plural("permission", "permissions")}: {affectedPerms.ToString(", ")}");
					}
					else
					{
						arg.ReplyWith($"Couldn't revoke group permissions - most likely because none of them are granted.");
					}
				}
				else if (permission.RevokeGroupPermission(name, perm))
				{
					arg.ReplyWith($"Revoked group '{name}' permission '{perm}'");
				}
				else
				{
					arg.ReplyWith($"Couldn't revoke group permission.");
				}
				break;

			default:
				PrintWarn(arg);
				break;
		}
	}

	[ConsoleCommand("show", "Displays information about a specific player or group (incl. permissions, groups and user list). Do 'c.show' for syntax info.")]
	[AuthLevel(2)]
	private void Show(ConsoleSystem.Arg arg)
	{
		static void PrintWarn(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith($"Syntax: c.show <groups|perms>\n" +
						  $"Syntax: c.show <group|user|perm> <name|id>");
		}

		if (!arg.HasArgs(1))
		{
			PrintWarn(arg);
			return;
		}

		var action = arg.GetString(0);

		switch (action)
		{
			case "user":
			{
				if (!arg.HasArgs(2))
				{
					PrintWarn(arg);
					return;
				}

				var name = arg.GetString(1);
				var user = permission.FindUser(name);

				if (user.Value == null)
				{
					arg.ReplyWith($"Couldn't find that user.");
					return;
				}

				var permissions = permission.GetUserPermissions(user.Key);
				arg.ReplyWith(
					$"User {user.Value.LastSeenNickname}[{user.Key}] found in {user.Value.Groups.Count:n0} groups:\n  {user.Value.Groups.Select(x => x).ToString(", ", " and ")}\n" +
					$"and has {permissions.Count():n0} permissions:\n  {permissions.ToString(", ")}");
				break;
			}
			case "group":
			{
				if (!arg.HasArgs(2))
				{
					PrintWarn(arg);
					return;
				}

				var name = arg.GetString(1);

				if (!permission.GroupExists(name))
				{
					arg.ReplyWith($"Couldn't find that group.");
					return;
				}

				var users = permission.GetUsersInGroup(name);
				var permissions = permission.GetGroupPermissions(name, false);
				arg.ReplyWith($"Group {name} has {users.Length:n0} users:\n  {users.Select(x => x).ToString(", ")}\n" +
							  $"and has {permissions.Length:n0} permissions:\n  {permissions.Select(x => x).ToString(", ")}");
				break;
			}
			case "perm":
			{
				if (!arg.HasArgs(2))
				{
					PrintWarn(arg);
					return;
				}

				var name = arg.GetString(1);

				if (!permission.PermissionExists(name))
				{
					arg.ReplyWith($"Couldn't find that permission.");
					return;
				}

				var users = permission.GetPermissionUsers(name);
				var groups = permission.GetPermissionGroups(name);
				arg.ReplyWith($"Permission {name} is granted to {users.Length:n0} users:\n  {users.Select(x => x).ToString(", ")}\n" +
							  $"and {groups.Length:n0} groups:\n  {groups.Select(x => x).ToString(", ")}");
				break;
			}
			case "groups":
			{
				var groups = permission.GetGroups();
				if (groups.Count() == 0)
				{
					arg.ReplyWith($"Couldn't find any group.");
					return;
				}

				arg.ReplyWith($"Groups:\n {groups.ToString(", ")}");
				break;
			}
			case "perms":
			{
				var perms = permission.GetPermissions();

				if (!perms.Any())
				{
					arg.ReplyWith($"Couldn't find any permission.");
				}

				arg.ReplyWith($"Permissions:\n {perms.ToString(", ")}");

				break;
			}

			default:
				PrintWarn(arg);
				break;
		}
	}

	[ConsoleCommand("usergroup", "Adds or removes a player from a group. Do 'c.usergroup' for syntax info.")]
	[AuthLevel(2)]
	private void UserGroup(ConsoleSystem.Arg arg)
	{
		static void PrintWarn(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith($"Syntax: c.usergroup <add|remove> <player> <group>\n" +
						  $"Syntax: c.usergroup <addall|removeall> <group>");
		}

		var action = arg.GetString(0);
		var player = string.Empty;
		var group = string.Empty;
		KeyValuePair<string, UserData> user = default;

		switch (action)
		{
			case "add":
			case "remove":
			{
				if (!arg.HasArgs(3))
				{
					PrintWarn(arg);
					return;
				}

				player = arg.GetString(1);
				group = arg.GetString(2);

				if (!permission.GroupExists(group))
				{
					arg.ReplyWith($"Group '{group}' could not be found.");
					return;
				}

				user = permission.FindUser(player);

				if (user.Value == null)
				{
					arg.ReplyWith($"Couldn't find that player.");
					return;
				}

				break;
			}

			default:
			{
				if (!arg.HasArgs(2))
				{
					PrintWarn(arg);
					return;
				}

				group = arg.GetString(1);

				if (!permission.GroupExists(group))
				{
					arg.ReplyWith($"Group '{group}' could not be found.");
					return;
				}

				break;
			}
		}

		switch (action)
		{
			case "add":
				if (permission.UserHasGroup(user.Key, group))
				{
					arg.ReplyWith($"{user.Value.LastSeenNickname}[{user.Key}] is already in '{group}' group.");
					return;
				}

				permission.AddUserGroup(user.Key, group);
				arg.ReplyWith($"Added {user.Value.LastSeenNickname}[{user.Key}] to '{group}' group.");
				break;

			case "remove":
				if (!permission.UserHasGroup(user.Key, group))
				{
					arg.ReplyWith($"{user.Value.LastSeenNickname}[{user.Key}] isn't in '{group}' group.");
					return;
				}

				permission.RemoveUserGroup(user.Key, group);
				arg.ReplyWith($"Removed {user.Value.LastSeenNickname}[{user.Key}] from '{group}' group.");
				break;

			case "addall":
			{
				group = group.ToLower();

				var count = permission.userdata.Count(userDataValue => permission.GetUserData(userDataValue.Key).Groups.Add(group));
				arg.ReplyWith($"Added {count:n0} users to '{group}' group.");
				break;
			}

			case "removeall":
			{
				group = group.ToLower();

				var count = permission.userdata.Count(userDataValue => permission.GetUserData(userDataValue.Key).Groups.Remove(group));
				arg.ReplyWith($"Removed {count:n0} users from '{group}' group.");
				break;
			}

			default:
				PrintWarn(arg);
				break;
		}
	}

	[ConsoleCommand("group", "Adds or removes a group. Do 'c.group' for syntax info.")]
	[AuthLevel(2)]
	private void Group(ConsoleSystem.Arg arg)
	{
		static void PrintWarn(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith($"Syntax: c.group add <group> [<displayName>] [<rank>]\n" +
						  $"Syntax: c.group remove <group>\n" +
						  $"Syntax: c.group set <group> <title|rank> <value>\n" +
						  $"Syntax: c.group parent <group> [<parent>]");
		}

		if (!arg.HasArgs(1))
		{
			PrintWarn(arg);
			return;
		}

		var action = arg.GetString(0);

		switch (action)
		{
			case "add":
			{
				if (!arg.HasArgs(2))
				{
					PrintWarn(arg);
					return;
				}

				var group = arg.GetString(1);

				if (permission.GroupExists(group))
				{
					arg.ReplyWith($"Group '{group}' already exists. To set any values for this group, use 'c.group set'.");
					return;
				}

				if (permission.CreateGroup(group, arg.HasArgs(3) ? arg.GetString(2) : group, arg.HasArgs(4) ? arg.GetInt(3) : 0))
				{
					arg.ReplyWith($"Created '{group}' group.");
				}
			}
			break;

			case "set":
			{
				if (!arg.HasArgs(4))
				{
					PrintWarn(arg);
					return;
				}

				var group = arg.GetString(1);

				if (!permission.GroupExists(group))
				{
					arg.ReplyWith($"Group '{group}' does not exists.");
					return;
				}

				var set = arg.GetString(2);
				var value = arg.GetString(3);

				switch (set)
				{
					case "title":
						permission.SetGroupTitle(group, value);
						break;

					case "rank":
						permission.SetGroupRank(group, value.ToInt());
						break;
				}

				arg.ReplyWith($"Set '{group}' group.");
			}
			break;

			case "remove":
			{
				if (!arg.HasArgs(2))
				{
					PrintWarn(arg);
					return;
				}

				var group = arg.GetString(1);

				if (permission.RemoveGroup(group))
				{
					arg.ReplyWith($"Removed '{group}' group.");
				}
				else
				{
					arg.ReplyWith($"Couldn't remove '{group}' group.");
				}
			}
			break;

			case "parent":
			{
				if (!arg.HasArgs(3))
				{
					PrintWarn(arg);
					return;
				}

				var group = arg.GetString(1);
				var parent = arg.GetString(2);

				if (permission.SetGroupParent(group, parent))
				{
					arg.ReplyWith($"Changed '{group}' group's parent to '{parent}'.");
				}
				else
				{
					arg.ReplyWith($"Couldn't change '{group}' group's parent to '{parent}'.");
				}
			}
			break;

			default:
				PrintWarn(arg);
				break;
		}
	}

	[ConsoleCommand("migrate_sql", "This will migrate all groups and users to a locally stored SQLite database from your Protobuf/Storeless database. A server reboot will be necessary after the process is done.")]
	[AuthLevel(2)]
	private void MigrateToSql(ConsoleSystem.Arg arg)
	{
		if (Community.Runtime.Config.Permissions.PermissionSerialization == Permission.SerializationMode.SQL)
		{
			arg.ReplyWith("Permission serialization must be anything but SQL");
			return;
		}

		var sql = new PermissionSql();
		sql.Migrate(Community.Runtime.Core.permission);
		sql.Dispose();

		Community.Runtime.Config.Permissions.PermissionSerialization = Permission.SerializationMode.SQL;
		Community.Runtime.SaveConfig();
	}
}
