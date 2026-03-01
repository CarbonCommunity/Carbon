using Carbon.Test;

namespace Carbon.Plugins;

public partial class Tests
{
	public class PermissionSqlMigration
	{
		private const string UserId = "76561198158946081";
		private const string GroupId = "sqlmigrationtestgroup";
		private const string GroupIdTwo = "sqlmigrationtestgroup2";
		private const string ParentGroupId = "sqlmigrationtestparent";
		private const string ChildGroupId = "sqlmigrationtestchild";
		private const string UserPerm = "sqlmigrationtest.user";
		private const string GroupPerm = "sqlmigrationtest.group";
		private const string EdgePermOne = "sqlmigrationtest.edge.one";
		private const string EdgePermTwo = "sqlmigrationtest.edge.two";

		[Integrations.Test.Assert]
		public void migration_and_sql_behavior(Integrations.Test.Assert test)
		{
			var permission = singleton.permission;

			test.IsFalse(permission.UserExists(UserId), $"permission.UserExists(\"{UserId}\")");
			permission.RegisterPermission(UserPerm, singleton);
			permission.RegisterPermission(GroupPerm, singleton);

			test.IsTrue(permission.CreateGroup(GroupId, GroupId, 0), $"permission.CreateGroup(\"{GroupId}\")");
			test.IsTrue(permission.CreateGroup(GroupIdTwo, GroupIdTwo, 0), $"permission.CreateGroup(\"{GroupIdTwo}\")");

			permission.RegisterPermission(EdgePermOne, singleton);
			permission.RegisterPermission(EdgePermTwo, singleton);

			permission.GetUserData(UserId, addIfNotExisting: true);
			permission.AddUserGroup(UserId, GroupId);
			permission.AddUserGroup(UserId, GroupIdTwo);
			permission.GrantGroupPermission(GroupId, GroupPerm, singleton);
			permission.GrantUserPermission(UserId, UserPerm, singleton);

			test.IsTrue(permission.UserHasGroup(UserId, GroupId), "protobuf user has group");
			test.IsTrue(permission.UserHasPermission(UserId, UserPerm), "protobuf user has perm");
			test.IsTrue(permission.GroupHasPermission(GroupId, GroupPerm), "protobuf group has perm");

			singleton.server.Command("c.migrate_perms_sql");

			permission = singleton.permission;
			test.IsTrue(permission.GetType().Name == "PermissionSql", "permission switched to sql");

			test.IsTrue(permission.UserHasGroup(UserId, GroupId), "sql user has group after migration");
			test.IsTrue(permission.UserHasPermission(UserId, UserPerm), "sql user has perm after migration");
			test.IsTrue(permission.GroupHasPermission(GroupId, GroupPerm), "sql group has perm after migration");

			permission.userdata.Remove(UserId);
			test.IsTrue(permission.UserExists(UserId), "sql UserExists should query db");

			permission.RemoveUserGroup(UserId, "*");
			permission.userdata.Remove(UserId);
			var reloadedAfterGroupRemoval = permission.GetUserData(UserId, addIfNotExisting: true);
			test.IsTrue(reloadedAfterGroupRemoval.Groups.Count == 0, "sql RemoveUserGroup('*') clears groups");

			permission.GrantUserPermission(UserId, UserPerm, singleton);
			permission.RevokeUserPermission(UserId, "*");
			permission.userdata.Remove(UserId);
			var reloadedAfterPermRemoval = permission.GetUserData(UserId, addIfNotExisting: true);
			test.IsTrue(reloadedAfterPermRemoval.Perms.Count == 0, "sql RevokeUserPermission('*') clears perms");

			permission.userdata.Remove(UserId);
			permission.AddUserGroup(UserId, GroupIdTwo);
			test.IsTrue(permission.UserHasGroup(UserId, GroupIdTwo), "sql AddUserGroup loads from db");
			permission.GrantUserPermission(UserId, UserPerm, singleton);
			test.IsTrue(permission.UserHasPermission(UserId, UserPerm), "sql GrantUserPermission loads from db");

			test.IsTrue(permission.CreateGroup(ParentGroupId, ParentGroupId, 0), $"permission.CreateGroup(\"{ParentGroupId}\")");
			test.IsTrue(permission.CreateGroup(ChildGroupId, ChildGroupId, 0), $"permission.CreateGroup(\"{ChildGroupId}\")");
			test.IsTrue(permission.SetGroupParent(ChildGroupId, ParentGroupId.ToUpperInvariant()), "sql set group parent (mixed-case parent)");
			permission.GrantGroupPermission(ParentGroupId, EdgePermOne, singleton);
			permission.AddUserGroup(UserId, ChildGroupId, addIfNotExisting: true);
			test.IsTrue(permission.UserHasPermission(UserId, EdgePermOne), "sql parent group permission inheritance");

			permission.GrantGroupPermission(GroupIdTwo, "sqlmigrationtest.edge.*", singleton);
			test.IsTrue(permission.GroupHasPermission(GroupIdTwo, EdgePermOne), "sql wildcard group perm grants edge one");
			test.IsTrue(permission.GroupHasPermission(GroupIdTwo, EdgePermTwo), "sql wildcard group perm grants edge two");
			permission.groupdata.Clear();
			permission.GetType().GetMethod("LoadGroups")?.Invoke(permission, null);
			test.IsTrue(permission.GroupHasPermission(GroupIdTwo, EdgePermOne), "sql wildcard group perm persists to db");
			test.IsTrue(permission.GroupHasPermission(GroupIdTwo, EdgePermTwo), "sql wildcard group perm persists to db");

			test.IsTrue(permission.GrantGroupPermission(GroupIdTwo, GroupPerm, singleton), "sql can grant additional group perm");
			test.IsTrue(permission.GroupHasPermission(GroupIdTwo, GroupPerm), "sql group has additional perm");
			test.IsTrue(permission.RevokeGroupPermission(GroupIdTwo, "sqlmigrationtest.edge.*"), "sql wildcard group perm revoke (prefix*)");
			test.IsFalse(permission.GroupHasPermission(GroupIdTwo, EdgePermOne), "sql wildcard group perm revoke removes edge one");
			test.IsFalse(permission.GroupHasPermission(GroupIdTwo, EdgePermTwo), "sql wildcard group perm revoke removes edge two");
			test.IsTrue(permission.GroupHasPermission(GroupIdTwo, GroupPerm), "sql wildcard group perm revoke keeps other perms");
			permission.groupdata.Clear();
			permission.GetType().GetMethod("LoadGroups")?.Invoke(permission, null);
			test.IsFalse(permission.GroupHasPermission(GroupIdTwo, EdgePermOne), "sql wildcard group perm revoke persisted (edge one)");
			test.IsFalse(permission.GroupHasPermission(GroupIdTwo, EdgePermTwo), "sql wildcard group perm revoke persisted (edge two)");
			test.IsTrue(permission.GroupHasPermission(GroupIdTwo, GroupPerm), "sql wildcard group perm revoke persisted (other perm)");
			test.IsTrue(permission.RevokeGroupPermission(GroupIdTwo, "*"), "sql wildcard group perm revoke (*)");
			test.IsFalse(permission.GroupHasPermission(GroupIdTwo, GroupPerm), "sql wildcard group perm revoke (*) removes remaining perms");

			permission.GrantGroupPermission(GroupIdTwo, "sqlmigrationtest.edge.*", singleton);
			permission.groupdata.Clear();
			permission.GetType().GetMethod("LoadGroups")?.Invoke(permission, null);
			test.IsTrue(permission.GroupHasPermission(GroupIdTwo, EdgePermTwo), "sql wildcard group perm restored after revoke tests");

			permission.GetUserData(UserId, addIfNotExisting: true);
			permission.UpdateNickname(UserId, "SqlMigrationTestNick");
			test.IsNotNull(permission.GetUserData("SqlMigrationTestNick"), "sql GetUserData(nickname) does not throw");
			permission.userdata.Remove(UserId);
			test.IsTrue(permission.UserExists("SqlMigrationTestNick"), "sql UserExists resolves nickname");

			const string NickCmdPerm = "sqlmigrationtest.nickcmdperm";
			permission.RegisterPermission(NickCmdPerm, singleton);
			singleton.server.Command($"c.grant user SqlMigrationTestNick {NickCmdPerm}");
			test.IsTrue(permission.UserHasPermission(UserId, NickCmdPerm), "sql c.grant user <nickname> <perm> works");

			permission.userdata.Remove(UserId);
			var reloadedAfterNickname = permission.GetUserData(UserId);
			test.IsTrue(reloadedAfterNickname.Groups.Count > 0, "sql UpdateNickname keeps groups in db");
			test.IsTrue(permission.UserHasPermission(UserId, UserPerm), "sql UpdateNickname keeps perms in db (CommitUser does not cascade)");

			permission.userdata.Remove(UserId);
			test.IsTrue(permission.UserExists(UserId, out var outData) && outData != null, "sql UserExists(out data) queries db");

			const string UserEdgePermOne = "sqlmigrationtest.useredge.one";
			const string UserEdgePermTwo = "sqlmigrationtest.useredge.two";
			permission.RegisterPermission(UserEdgePermOne, singleton);
			permission.RegisterPermission(UserEdgePermTwo, singleton);
			test.IsTrue(permission.GrantUserPermission(UserId, "sqlmigrationtest.useredge.*", singleton), "sql wildcard user perm grant (prefix*)");
			test.IsTrue(permission.UserHasPermission(UserId, UserEdgePermOne), "sql wildcard user perm grants useredge one");
			test.IsTrue(permission.UserHasPermission(UserId, UserEdgePermTwo), "sql wildcard user perm grants useredge two");
			test.IsTrue(permission.RevokeUserPermission(UserId, "sqlmigrationtest.useredge.*"), "sql wildcard user perm revoke (prefix*)");
			permission.userdata.Remove(UserId);
			test.IsFalse(permission.GetUserData(UserId).Perms.Contains(UserEdgePermOne), "sql wildcard user perm revoke persisted (useredge one)");
			test.IsFalse(permission.GetUserData(UserId).Perms.Contains(UserEdgePermTwo), "sql wildcard user perm revoke persisted (useredge two)");

			permission.userdata.Remove(UserId);
			var sqlReloadedBeforeProto = permission.GetUserData(UserId);
			test.Log($"sql groups before proto: {string.Join(",", sqlReloadedBeforeProto.Groups)}");
			test.IsTrue(sqlReloadedBeforeProto.Groups.Count > 0, "sql has groups before proto migration");

			singleton.server.Command("c.migrate_perms_proto");
			permission = singleton.permission;
			test.IsTrue(permission.GetType().Name == "Permission", "permission switched to protobuf");
			test.IsTrue(permission.UserExists(UserId), "protobuf user exists after migration");
			test.IsTrue(permission.UserExists("SqlMigrationTestNick"), "protobuf UserExists resolves nickname after migration");
			test.IsTrue(permission.FindUser("SqlMigrationTestNick").Key == UserId, "protobuf FindUser resolves nickname after migration");
			test.Log($"proto child group count: {permission.GetUserData(UserId).Groups.Count}");
			test.Log($"proto child group parent: '{permission.GetGroupParent(ChildGroupId)}'");
			test.Log($"proto parent group perms: {permission.GetGroupPermissions(ParentGroupId, true).Length}");
			test.IsTrue(permission.UserHasGroup(UserId, ChildGroupId), "protobuf retains child group membership");
			test.IsTrue(permission.GetGroupParent(ChildGroupId) == ParentGroupId, "protobuf retains group parent relation");
			test.IsTrue(permission.GroupHasPermission(ParentGroupId, EdgePermOne), "protobuf retains parent group perm");
			test.IsTrue(permission.UserHasPermission(UserId, EdgePermOne), "protobuf retains parent group permission");
			test.IsTrue(permission.GroupHasPermission(GroupIdTwo, EdgePermTwo), "protobuf retains wildcard group perms");
		}
	}
}
