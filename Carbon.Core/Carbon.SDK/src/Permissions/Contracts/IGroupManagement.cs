/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Permissions;

public interface IGroupManagement
{
	public void Insert(string groupID, string title, int rank, string parent);
	public void Remove(string steamID);


	public string GetGroupTitle(string groupID);
	public int GetGroupRank(string groupID);
	public string GetGroupParent(string groupID);

	public void AddGroupPermission(string groupID, string permission);
	public void RemoveGroupPermission(string groupID, string permission);
	public void ResetPermissions(string groupID);
}