using Facepunch;

namespace Carbon.Oxide;

public class PermissionSql : Permission
{
	public PermissionDatabase db;

	public void InitializeDb()
	{
		if (db != null)
		{
			return;
		}
		var path = Path.Combine(ConVar.Server.filesStorageFolder, "carbon.perms.db");
		db = new PermissionDatabase();
		db.Open(path);
		db.Execute("PRAGMA foreign_keys = ON");
		if (db.TableExists("users"))
		{
			return;
		}

		db.Execute("CREATE TABLE users ( userId INTEGER PRIMARY KEY, lastSeenNickname TEXT, language TEXT )");
		db.Execute("CREATE INDEX IF NOT EXISTS userId ON users ( userId )");
		db.Execute("CREATE TABLE IF NOT EXISTS userPerms (userId INTEGER, permission TEXT, PRIMARY KEY (userId, permission), FOREIGN KEY (userId) REFERENCES users(userId) ON DELETE CASCADE)");
		db.Execute("CREATE TABLE IF NOT EXISTS userGroups (userId INTEGER, groupName TEXT, PRIMARY KEY (userId, groupName), FOREIGN KEY (userId) REFERENCES users(userId) ON DELETE CASCADE, FOREIGN KEY (groupName) REFERENCES groups(groupName) ON DELETE CASCADE)");

		db.Execute("CREATE TABLE groups ( groupName TEXT PRIMARY KEY, title TEXT, rank INTEGER, parentGroup TEXT )");
		db.Execute("CREATE INDEX IF NOT EXISTS groupName ON groups ( groupName )");
		db.Execute("CREATE TABLE IF NOT EXISTS groupsPerms (groupName TEXT, permission TEXT, PRIMARY KEY (groupName, permission), FOREIGN KEY (groupName) REFERENCES groups(groupName) ON DELETE CASCADE)");
	}

	#region Group

	public override bool CreateGroup(string group, string title, int rank)
	{
		if (base.CreateGroup(group, title, rank))
		{
			if (!group.IsLower())
			{
				group = group.ToLower();
			}
			db?.Execute("INSERT OR REPLACE INTO groups ( groupName, title, rank ) VALUES ( ?, ?, ? )", group, title, rank);
			return true;
		}

		return false;
	}

	public override bool RemoveGroup(string group)
	{
		if (base.RemoveGroup(group))
		{
			if (!group.IsLower())
			{
				group = group.ToLower();
			}
			db?.Execute("DELETE FROM groups WHERE groupName = ?", group);
			return true;
		}
		return false;
	}

	public override bool SetGroupTitle(string group, string title)
	{
		if (base.SetGroupTitle(group, title))
		{
			if (!group.IsLower())
			{
				group = group.ToLower();
			}
			db?.Execute("UPDATE groups SET title = ? WHERE groupName = ?", title, group);
			return true;
		}
		return false;
	}

	public override bool SetGroupRank(string group, int rank)
	{
		if (base.SetGroupRank(group, rank))
		{
			if (!group.IsLower())
			{
				group = group.ToLower();
			}
			db?.Execute("UPDATE groups SET rank = ? WHERE groupName = ?", rank, group);
			return true;
		}
		return false;
	}

	public override bool SetGroupParent(string group, string parent)
	{
		if (base.SetGroupParent(group, parent))
		{
			if (!group.IsLower())
			{
				group = group.ToLower();
			}
			db?.Execute("UPDATE groups SET parentGroup = ? WHERE groupName = ?", parent, group);
			return true;
		}
		return false;
	}

	public override bool GrantGroupPermission(string name, string perm, BaseHookable owner)
	{
		if (base.GrantGroupPermission(name, perm, owner))
		{
			if (!name.IsLower())
			{
				name = name.ToLower();
			}
			db?.Execute("INSERT OR IGNORE INTO groupsPerms ( groupName, permission ) VALUES ( ?, ? )", name, perm);
			return true;
		}
		return false;
	}

	public override bool RevokeGroupPermission(string name, string perm)
	{
		if (base.RevokeGroupPermission(name, perm))
		{
			if (!name.IsLower())
			{
				name = name.ToLower();
			}
			db?.Execute("DELETE FROM groupsPerms WHERE groupName = ? AND permission = ?", name, perm);
			return true;
		}
		return false;
	}

	#endregion

	#region Users

	public override UserData GetUserData(string id, bool addIfNotExisting = false)
	{
		if (!UserExists(id))
		{
			if (db.QueryUser(id) is (string userId, UserData data))
			{
				userdata.Add(userId, data);
			}
			else if (addIfNotExisting)
			{
				data = new UserData();
				userdata.Add(id, data);
				CommitUser(id, data);
			}
		}
		return base.GetUserData(id, addIfNotExisting);
	}

	public override void CommitUser(string userId, UserData data)
	{
		db?.Execute("INSERT OR REPLACE INTO users ( userId, lastSeenNickname, language ) VALUES ( ?, ?, ? )", userId, data.LastSeenNickname, data.Language);
	}

	public override bool GrantUserPermission(string id, string perm, BaseHookable owner)
	{
		if (!PermissionExists(perm, owner))
		{
			return false;
		}

		var data = GetUserData(id);

		if (!perm.IsLower())
		{
			perm = perm.ToLower();
		}

		if (perm.EndsWith(StarStr))
		{
			HashSet<string> source;
			if (owner == null)
			{
				source = new HashSet<string>(permset.Values.SelectMany(v => v));
			}
			else if (!permset.TryGetValue(owner, out source))
			{
				return false;
			}

			if (perm.Equals(StarStr))
			{
				return source.Aggregate(false, (c, s) =>
				{
					if (!(c | data.Perms.Add(s)))
					{
						return false;
					}

					db?.Execute("INSERT OR IGNORE INTO userPerms ( userId, permission ) VALUES ( ?, ? )", id, s);

					// OnUserPermissionGranted
					HookCaller.CallStaticHook(4054877424, id, s);
					return true;

				});
			}
			perm = perm.TrimEnd(Star);

			return (from s in source
				where s.StartsWith(perm)
				select s).Aggregate(false, (c, s) =>
			{
				if (!(c | data.Perms.Add(s)))
				{
					return false;
				}

				db?.Execute("INSERT OR IGNORE INTO userPerms ( userId, permission ) VALUES ( ?, ? )", id, s);

				// OnUserPermissionGranted
				HookCaller.CallStaticHook(4054877424, id, s);
				return true;

			});
		}
		else
		{
			if (!data.Perms.Add(perm))
			{
				return false;
			}

			db?.Execute("INSERT OR IGNORE INTO userPerms ( userId, permission ) VALUES ( ?, ? )", id, perm);

			// OnUserPermissionGranted
			HookCaller.CallStaticHook(4054877424, id, perm);
			return true;
		}
	}

	public override bool RevokeUserPermission(string id, string perm)
	{
		if (string.IsNullOrEmpty(perm))
		{
			return false;
		}

		var userData = GetUserData(id);

		if (!perm.IsLower())
		{
			perm = perm.ToLower();
		}

		if (perm.EndsWith(StarStr))
		{
			if (!perm.Equals(StarStr))
			{
				perm = perm.TrimEnd(Star);

				return userData.Perms.RemoveWhere(s =>
				{
					if (!s.StartsWith(perm))
					{
						return false;
					}

					db?.Execute("DELETE FROM userPerms WHERE userId = ? AND permission = ?", id, s);

					// OnUserPermissionRevoked
					HookCaller.CallStaticHook(1879829838, id, s);
					return true;
				}) > 0;
			}

			if (userData.Perms.Count <= 0)
			{
				return false;
			}

			userData.Perms.Clear();
			return true;
		}
		else
		{
			if (!userData.Perms.Remove(perm))
			{
				return false;
			}

			db?.Execute("DELETE FROM userPerms WHERE userId = ? AND permission = ?", id, perm);

			// OnUserPermissionRevoked
			HookCaller.CallStaticHook(1879829838, id, perm);
			return true;
		}
	}

	public override void AddUserGroup(string id, string name)
	{
		if (!name.IsLower())
		{
			name = name.ToLower();
		}
		if (!GroupExists(name) || !GetUserData(id).Groups.Add(name))
		{
			return;
		}

		db?.Execute("INSERT OR IGNORE INTO userGroups ( userId, groupName ) VALUES ( ?, ? )", id, name);

		// OnUserGroupAdded
		HookCaller.CallStaticHook(3116013984, id, name);
	}

	public override void RemoveUserGroup(string id, string name)
	{
		if (!name.IsLower())
		{
			name = name.ToLower();
		}
		if (!GroupExists(name)) return;

		var userData = GetUserData(id);

		if (name.Equals(Star))
		{
			if (userData.Groups.Count <= 0)
			{
				return;
			}

			using var groups = Pool.Get<PooledList<string>>();
			groups.AddRange(userData.Groups);
			foreach (var group in groups)
			{
				RemoveUserGroup(id, group);
			}
		}
		else
		{
			if (!userData.Groups.Remove(name))
			{
				return;
			}

			// OnUserGroupRemoved
			HookCaller.CallStaticHook(1018697706, id, name);
		}
	}

	#endregion

	public override void Dispose()
	{
		base.Dispose();
		db.Close();
		db = null;
	}

	public override void LoadFromDatafile()
	{
		InitializeDb();
		LoadGroups();
		IsLoaded = true;
	}

	public void LoadGroups()
	{
		var groups = db.QueryAllGroups();
		foreach (var group in groups)
		{
			groupdata[group.groupName] = group.data;
		}
	}

	public class PermissionDatabase : Facepunch.Sqlite.Database
	{
		public IEnumerable<(string groupName, GroupData data)> QueryAllGroups()
		{
			return this.ExecuteAndReadQueryResults(Prepare("SELECT groupName, title, rank, parentGroup FROM groups"), ReadGroupRow);
		}
		public (string groupName, GroupData data) ReadGroupRow(IntPtr stmHandle)
		{
			var group = new GroupData();
			var groupName = GetColumnValue<string>(stmHandle, 0);
			group.Title = GetColumnValue<string>(stmHandle, 1);
			group.Rank = GetColumnValue<int>(stmHandle, 2);
			group.ParentGroup = GetColumnValue<string>(stmHandle, 3);
			var perms = QueryGroupPermissions(groupName);
			foreach (var perm in perms)
			{
				group.Perms.Add(perm);
			}
			return (groupName, group);
		}

		public IEnumerable<string> QueryGroupPermissions(string groupName)
		{
			var stmHandle = Prepare("SELECT groupName, permission FROM groupsPerms WHERE groupName = ?");
			Bind(stmHandle, 1, groupName);
			return this.ExecuteAndReadQueryResults(stmHandle, ReadStringRow);
		}
		public string ReadStringRow(IntPtr stmHandle)
		{
			return GetColumnValue<string>(stmHandle, 1);
		}

		public (string userId, UserData data) QueryUser(string id)
		{
			var stmHandle = Prepare("SELECT * FROM users WHERE userId = ?");
			Bind(stmHandle, 1, id);
			return this.ExecuteAndReadQueryResults(stmHandle, ReadUserRow).FirstOrDefault();
		}
		public (string userId, UserData data) ReadUserRow(IntPtr stmHandle)
		{
			var user = new UserData();
			var id = GetColumnValue<ulong>(stmHandle, 0);
			user.LastSeenNickname = GetColumnValue<string>(stmHandle, 1);
			user.Language = GetColumnValue<string>(stmHandle, 2);
			var userId = id.ToString();
			var perms = QueryUserPermissions(userId);
			foreach (var perm in perms)
			{
				user.Perms.Add(perm);
			}
			var groups = QueryUserGroups(userId);
			foreach (var group in groups)
			{
				user.Groups.Add(group);
			}
			return (userId, user);
		}
		public IEnumerable<string> QueryUserPermissions(string userId)
		{
			var stmHandle = Prepare("SELECT userId, permission FROM userPerms WHERE userId = ?");
			Bind(stmHandle, 1, userId);
			return this.ExecuteAndReadQueryResults(stmHandle, ReadStringRow);
		}
		public IEnumerable<string> QueryUserGroups(string userId)
		{
			var stmHandle = Prepare("SELECT userId, groupName FROM userGroups WHERE userId = ?");
			Bind(stmHandle, 1, userId);
			return this.ExecuteAndReadQueryResults(stmHandle, ReadStringRow);
		}
	}
}
