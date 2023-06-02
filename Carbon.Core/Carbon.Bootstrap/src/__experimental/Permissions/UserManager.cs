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

internal sealed class UserManager : IUserManagement
{
	private static Database _database;

	internal UserManager(Database handler)
	{
		_database = handler;
		_database.Execute("PRAGMA foreign_keys = ON;");

		if (!_database.TableExists("User"))
		{
			try
			{
				_database.Execute(
					"CREATE TABLE `User` (" +
					"  `SteamID` TEXT NOT NULL UNIQUE," +
					"  `LastSeenNickname` TEXT NOT NULL ON CONFLICT REPLACE DEFAULT 'Unnamed'," +
					"  `Language` TEXT NOT NULL ON CONFLICT REPLACE DEFAULT 'en'," +
					"  PRIMARY KEY(`SteamID`)" +
					")"
				);
				Logger.Warn($"Created table 'User'");
			}
			catch (System.Exception e)
			{
				Logger.Error($"Error while creating table 'Group'", e);
			}
		}

		if (!_database.TableExists("UserGroups"))
		{
			try
			{
				_database.Execute(
					"CREATE TABLE `UserGroups` (" +
					"	`SteamID` TEXT," +
					"	`GroupID` TEXT," +
					"	FOREIGN KEY(`GroupID`) REFERENCES `Group`(`GroupID`)," +
					"	FOREIGN KEY(`SteamID`) REFERENCES `User`(`SteamID`)" +
					"	UNIQUE(`SteamID`, `GroupID`)" +
					")"
				);
				Logger.Warn($"Created table 'UserGroups'");
			}
			catch (System.Exception e)
			{
				Logger.Error($"Error while creating table 'Group'", e);
			}
		}

		if (!_database.TableExists("UserPermissions"))
		{
			try
			{
				_database.Execute(
					"CREATE TABLE `UserPermissions` (" +
					"	`SteamID` TEXT NOT NULL," +
					"	`Permission` TEXT," +
					"	UNIQUE(`SteamID`, `Permission`)," +
					"	FOREIGN KEY(`SteamID`) REFERENCES `User`(`SteamID`)" +
					")"
				);
				Logger.Warn($"Created table 'UserPermissions'");
			}
			catch (System.Exception e)
			{
				Logger.Error($"Error while creating table 'Group'", e);
			}
		}

		_database.Execute(
			"CREATE TRIGGER IF NOT EXISTS `on_new_user`" +
			"	AFTER INSERT ON `User`" +
			"BEGIN" +
			"	INSERT INTO `UserGroups` (`SteamID`, `GroupID`)" +
			"	VALUES (NEW.`SteamID`, 'default');" +
			"END;"
		);

		_database.Execute(
			"CREATE TRIGGER IF NOT EXISTS `on_remove_user`" +
			"	BEFORE DELETE ON `User`" +
			"BEGIN" +
			"	DELETE FROM `UserGroups` WHERE `SteamID` = OLD.`SteamID`;" +
			"	DELETE FROM `UserPermissions` WHERE `SteamID` = OLD.`SteamID`;" +
			"END;"
		);
	}

	// --- USER TABLE ----------------------------------------------------------
	public void Insert(string steamID, string nickname, string language)
	{
		try
		{
			string query = "INSERT OR REPLACE INTO `User` (`SteamID`, `LastSeenNickname`, `Language`) VALUES (?, ?, ?)";
			_database.Execute(query, steamID, nickname, language);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.Insert", e);
		}
	}

	public void Remove(string steamID)
	{
		try
		{
			string query = "DELETE FROM `User` WHERE `SteamID` = ?";
			_database.Execute(query, steamID);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.Delete", e);
		}
	}

	public string GetPlayerName(string steamID)
	{
		try
		{
			string query = "SELECT `LastSeenNickname` FROM `User` WHERE `SteamID` = ?";
			return _database.Query<string, string> (query, steamID);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.GetPlayerName", e);
			return default;
		}
	}

	public string GetPlayerName(ulong steamID)
		=> GetPlayerName($"{steamID}");

	public string GetPlayerLanguage(string steamID)
	{
		try
		{
			string query = "SELECT `Language` FROM `User` WHERE `SteamID` = ?";
			return _database.Query<string, string> (query, steamID);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.GetPlayerLanguage", e);
			return default;
		}
	}

	public string GetPlayerLanguage(ulong steamID)
		=> GetPlayerLanguage($"{steamID}");


	// --- USER PERMISSIONS TABLE ----------------------------------------------
	public void AddUserPermission(string steamID, string permission)
	{
		try
		{
			string query = "INSERT OR REPLACE INTO `UserPermissions` (`SteamID`, `Permission`) VALUES (?, ?)";
			_database.Execute(query, steamID, permission);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.AddUserPermission", e);
		}
	}

	public void RemoveUserPermission(string steamID, string permission)
	{
		try
		{
			string query = "DELETE FROM `UserPermissions` WHERE `SteamID` = ? AND `Permission` = ?";
			_database.Execute(query, steamID, permission);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.RemoveUserPermission", e);
		}
	}

	public void ResetPermissions(string steamID)
	{
		try
		{
			string query = "DELETE FROM `UserPermissions` WHERE `SteamID` = ?";
			_database.Execute(query, steamID);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.ResetPermissions", e);
		}
	}

	// --- USER GROUP TABLE ----------------------------------------------------
	public void AddToGroup(string steamID, string groupID)
	{
		try
		{
			string query = "INSERT OR REPLACE INTO `UserGroups` (`SteamID`, `GroupID`) VALUES (?, ?)";
			_database.Execute(query, steamID, groupID);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.AddToGroup", e);
		}
	}

	public void RemoveFromGroup(string steamID, string groupID)
	{
		try
		{
			string query = "DELETE FROM `UserGroups` WHERE `SteamID` = ? AND `GroupID = ?";
			_database.Execute(query, steamID, groupID);
		}
		catch (System.Exception e)
		{
			Logger.Error($"{this}.AddToGroup", e);
		}
	}
}

