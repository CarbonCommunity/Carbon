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

internal sealed class GroupManager : IGroupManagement
{
	private static Database _database;

	internal GroupManager(Database handler)
	{
		_database = handler;
		_database.Execute("PRAGMA foreign_keys = ON;");

		if (!_database.TableExists("Group"))
		{
			try
			{
				_database.Execute(
					"CREATE TABLE `Group` (" +
					"	`GroupID` TEXT NOT NULL UNIQUE," +
					"	`Title` TEXT," +
					"	`Rank` NUMERIC NOT NULL ON CONFLICT REPLACE DEFAULT 50," +
					"	`ParentGroup` TEXT," +
					"	PRIMARY KEY(`GroupID`)" +
					")"
				);
				Logger.Warn($"Created table 'Group'");
			}
			catch (System.Exception e)
			{
				Logger.Error($"Error while creating table 'Group'", e);
			}
		}

		if (!_database.TableExists("GroupPermissions"))
		{
			try
			{
				_database.Execute(
					"CREATE TABLE `GroupPermissions` (" +
					"	`GroupID` TEXT NOT NULL," +
					"	`Permission` TEXT," +
					"	FOREIGN KEY(`GroupID`) REFERENCES `Group`(`GroupID`)" +
					"   UNIQUE(`GroupID`, `Permission`)" +
					")"
				);
				Logger.Warn($"Created table 'GroupPermissions'");
			}
			catch (System.Exception e)
			{
				Logger.Error($"Error while creating table 'Group'", e);
			}
		}

		_database.Execute(
			"CREATE TRIGGER IF NOT EXISTS `on_remove_group`" +
			"	BEFORE DELETE ON `Group`" +
			"BEGIN" +
			"	DELETE FROM `UserGroups` WHERE `GroupID` = OLD.`GroupID`;" +
			"	DELETE FROM `GroupPermissions` WHERE `GroupID` = OLD.`GroupID`;" +
			"END;"
		);

		// populate the default groups
		Insert("default", "Default user group", 0, null);
		Insert("mod", "Default moderator group", 80, null);
		Insert("admin", "Default administrator group", 90, null);
	}

	// --- GROUP TABLE ---------------------------------------------------------
	public void Insert(string groupID, string title, int rank, string parent)
	{
		try
		{
			string query = "INSERT OR REPLACE INTO `Group` (`GroupID`, `Title`, `Rank`, `ParentGroup`) VALUES (?, ?, ?, ?)";
			_database.Execute(query, groupID, title, rank, parent);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.Insert", e);
		}
	}

	public void Remove(string groupID)
	{
		try
		{
			string query = "DELETE FROM `Group` WHERE `GroupID` = ?";
			_database.Execute(query, groupID);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.Delete", e);
		}
	}

	public string GetGroupTitle(string groupID)
	{
		try
		{
			return _database.Query<string>($"SELECT `Title` FROM `Group` WHERE `GroupID` = {groupID}");
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.GetGroupTitle", e);
			return default;
		}
	}

	public int GetGroupRank(string groupID)
	{
		try
		{
			return _database.Query<int>($"SELECT `Rank` FROM `Group` WHERE `GroupID` = {groupID}");
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.GetGroupTitle", e);
			return default;
		}
	}

	public string GetGroupParent(string groupID)
	{
		try
		{
			return _database.Query<string>($"SELECT `ParentGroup` FROM `Group` WHERE `GroupID` = {groupID}");
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.GetGroupParent", e);
			return default;
		}
	}

	// --- GROUP PERMISSIONS TABLE ---------------------------------------------
	public void AddGroupPermission(string groupID, string permission)
	{
		try
		{
			string query = "INSERT OR REPLACE INTO `GroupPermissions` (`GroupID`, `Permission`) VALUES (?, ?)";
			_database.Execute(query, groupID, permission);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.AddGroupPermission", e);
		}
	}

	public void RemoveGroupPermission(string groupID, string permission)
	{
		try
		{
			string query = "DELETE FROM `GroupPermissions` WHERE `GroupID` = ? AND `Permission` = ?";
			_database.Execute(query, groupID, permission);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.ResetGroupPermissions", e);
		}
	}

	public void ResetPermissions(string groupID)
	{
		try
		{
			string query = "DELETE FROM `GroupPermissions` WHERE `GroupID` = ?";
			_database.Execute(query, groupID);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.ResetGroupPermissions", e);
		}
	}
}
