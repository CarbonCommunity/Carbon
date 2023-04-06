using System.Collections.Generic;
/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Permissions;

public partial interface IPermissionManager
{
	public IUserManagement Users
	{ get; }

	public IGroupManagement Groups
	{ get; }
}


// 	NONEED public void LoadFromDatafile();
// 	NONEED public void SaveData();
// 	NONEED public void SaveGroups();
// 	NONEED public void SaveUsers();

// 	public bool UserIdValid(string id);


// -- PERM MGMT
// 	public string[] GetPermissions();
// 	public bool PermissionExists(string name, Plugin owner = null);
// 	public void RegisterPermission(string name, BaseHookable owner);
// 	public void UnregisterPermissions(BaseHookable owner);

// 	public string[] GetPermissionUsers(string perm);
// 	public string[] GetPermissionGroups(string perm);

// -- USER
// 	public bool UserExists(string id);
// 	public bool UserExists(string id, out UserData data);
// 	public bool UserHasAnyGroup(string id);
// 	public bool UserHasGroup(string id, string name);
// 	public bool UserHasPermission(string id, string perm);

// 	public string[] GetUserGroups(string id);
// 	OK public void AddUserGroup(string id, string name);
// 	OK public void RemoveUserGroup(string id, string name);

// 	public string[] GetUserPermissions(string id);
// 	OK public bool GrantUserPermission(string id, string perm, Plugin owner);
// 	OK public bool RevokeUserPermission(string id, string perm);

// -- GROUP
// 	OK public bool SetGroupParent(string group, string parent);
// 	OK public bool SetGroupRank(string group, int rank);
// 	OK public bool SetGroupTitle(string group, string title);
// 	OK public int GetGroupRank(string group);
//  OK public string GetGroupParent(string group);
// 	OK public string GetGroupTitle(string group);

// 	public bool GroupExists(string group);
// 	OK public bool GroupAdd(string group, string title, int rank);
// 	OK public bool GroupRemove(string group);

// 	public string[] GetUsersInGroup(string group);

// 	public string[] GetGroupPermissions(string name, bool parents = false);
// 	OK public bool GrantGroupPermission(string name, string perm, Plugin owner);
// 	OK public bool RevokeGroupPermission(string name, string perm);





// 	public string[] GetGroups();

// 	public bool GroupHasPermission(string name, string perm);
// 	public bool GroupsHavePermission(HashSet<string> groups, string perm);

// 	public bool HasCircularParent(string group, string parent);


// 	public UserData GetUserData(string id);
// 	public void RefreshUser(BasePlayer player);
// 	public void UpdateNickname(string id, string nickname);

// 	public KeyValuePair<string, UserData> FindUser(string id);

// 	public void MigrateGroup(string oldGroup, string newGroup);
// 	public void RegisterValidate(Func<string, bool> val);

// 	public void CleanUp();
// 	public void Export(string prefix = "auth");

// }
