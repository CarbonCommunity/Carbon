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
		if (db.TableExists("data"))
		{
			return;
		}
		
	}

	public override void LoadFromDatafile()
	{
		InitializeDb();
	}

	public class PermissionDatabase : Facepunch.Sqlite.Database
	{

	}
}
