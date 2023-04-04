/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Permissions;

public interface IUserManagement
{
	public void Insert(string steamID, string nickname, string language);
	public void Remove(string steamID);

	public string GetPlayerName(string steamID);
	public string GetPlayerName(ulong steamID);
	public string GetPlayerLanguage(string steamID);
	public string GetPlayerLanguage(ulong steamID);

	public void AddUserPermission(string steamID, string permission);
	public void RemoveUserPermission(string steamID, string permission);
	public void ResetPermissions(string steamID);

	public void AddToGroup(string steamID, string groupID);
	public void RemoveFromGroup(string steamID, string groupID);
}