// using System.Collections.Generic;
// /*
//  *
//  * Copyright (c) 2022-2023 Carbon Community 
//  * All rights reserved.
//  *
//  */

// namespace API.Something;

// public interface ISomething
// {
// 	public bool GroupAdd(string group, string title, int rank);
// 	public bool GroupRemove(string group);
// 	public bool GroupExists(string group);





// 	public bool GrantGroupPermission(string name, string perm, Plugin owner);
// 	public bool GrantUserPermission(string id, string perm, Plugin owner);

// 	public bool GroupHasPermission(string name, string perm);
// 	public bool GroupsHavePermission(HashSet<string> groups, string perm);

// 	public bool HasCircularParent(string group, string parent);

// 	public bool PermissionExists(string name, Plugin owner = null);


// 	public bool RevokeGroupPermission(string name, string perm);
// 	public bool RevokeUserPermission(string id, string perm);

// 	public bool SetGroupParent(string group, string parent);
// 	public bool SetGroupRank(string group, int rank);
// 	public bool SetGroupTitle(string group, string title);

// 	public bool UserExists(string id, out UserData data);
// 	public bool UserExists(string id);
// 	public bool UserHasAnyGroup(string id);
// 	public bool UserHasGroup(string id, string name);
// 	public bool UserHasPermission(string id, string perm);
// 	public bool UserIdValid(string id);

// 	public int GetGroupRank(string group);
// 	public KeyValuePair<string, UserData> FindUser(string id);
// 	public string GetGroupParent(string group);
// 	public string GetGroupTitle(string group);
// 	public string[] GetGroupPermissions(string name, bool parents = false);
// 	public string[] GetGroups();

// 	public string[] GetPermissionGroups(string perm);
// 	public string[] GetPermissions();
// 	public string[] GetPermissionUsers(string perm);

// 	public string[] GetUserGroups(string id);
// 	public string[] GetUserPermissions(string id);
// 	public string[] GetUsersInGroup(string group);
// 	public UserData GetUserData(string id);

// 	public void AddUserGroup(string id, string name);

// 	public void MigrateGroup(string oldGroup, string newGroup);
// 	public void RefreshUser(BasePlayer player);
// 	public void RegisterPermission(string name, BaseHookable owner);
// 	public void RegisterValidate(Func<string, bool> val);
// 	public void RemoveUserGroup(string id, string name);

// 	public void UnregisterPermissions(BaseHookable owner);
// 	public void UpdateNickname(string id, string nickname);









// 	public void CleanUp();
// 	public void Export(string prefix = "auth");
// 	public void LoadFromDatafile();

// 	public void SaveData();
// 	public void SaveGroups();
// 	public void SaveUsers();
// }
