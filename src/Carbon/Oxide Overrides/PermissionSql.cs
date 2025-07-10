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
		if (db.TableExists("users"))
		{
			return;
		}
		this.db.Execute("CREATE TABLE users ( userId INTEGER PRIMARY KEY, lastSeenNickname TEXT, language TEXT )");
		this.db.Execute("CREATE INDEX IF NOT EXISTS userindex ON users ( userId )");

		this.db.Execute("CREATE TABLE groups ( groupName TEXT PRIMARY KEY, title TEXT, rank INTEGER, parentGroup TEXT )");
		this.db.Execute("CREATE INDEX IF NOT EXISTS groupindex ON groups ( groupName )");
		this.db.Execute("CREATE TABLE groupsPerms ( groupName TEXT, permission TEXT, PRIMARY KEY (groupName, permission), FOREIGN KEY (groupName) REFERENCES groups(groupName) ON DELETE CASCADE )");
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

	public override bool GrantGroupPermission(string name, string perm, BaseHookable owner)
	{
		if (base.GrantGroupPermission(name, perm, owner))
		{
			if (!name.IsLower())
			{
				name = name.ToLower();
			}
			db?.Execute("INSERT OR REPLACE INTO groupsPerms ( groupName, permission ) VALUES ( ?, ?, ? )", name, perm);
			return true;
		}
		return false;
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
			var stmHandle = this.Prepare("SELECT groupName, title, rank, parentGroup FROM groups");
			return this.ExecuteAndReadQueryResults(stmHandle, ReadGroupRow);
		}

		public static (string groupName, GroupData data) ReadGroupRow(IntPtr stmHandle)
		{
			var group = new GroupData();
			var groupName = GetColumnValue<string>(stmHandle, 0);
			group.Title = GetColumnValue<string>(stmHandle, 1);
			group.Rank = GetColumnValue<int>(stmHandle, 2);
			group.ParentGroup = GetColumnValue<string>(stmHandle, 3);
			return (groupName, group);
		}
	}
}
