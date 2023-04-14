using System.IO;
using API.Permissions;
using Facepunch.Sqlite;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;
#pragma warning disable IDE0051

internal sealed class PermissionManager : UnityEngine.MonoBehaviour, IPermissionManager
{
	public static Database _database;

	public IUserManagement Users
	{ get => _users; }

	public IGroupManagement Groups
	{ get => _groups; }

	private static IUserManagement _users;
	private static IGroupManagement _groups;

	private void Awake()
	{
		_database = new Database();
		_database.Open(Path.Combine(Context.CarbonData, "carbon.db"));
		Logger.Warn($"New SQLite database from file 'carbon.db'");

		_users = new UserManager(_database);
		_groups = new GroupManager(_database);
	}
}

