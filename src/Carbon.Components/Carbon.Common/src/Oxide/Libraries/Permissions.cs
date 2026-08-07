using Facepunch;
using Logger = Carbon.Logger;

namespace Oxide.Core.Libraries;

public class Permission : Library
{
	public enum SerializationMode
	{
		Storeless = -1,
		Protobuf = 0,
		SQL = 1
	}

	public override bool IsGlobal => false;

	public bool IsLoaded { get; set; }

	public Permission()
	{
		permset = new Dictionary<BaseHookable, HashSet<string>>();

		RegisterValidate(delegate (string value)
		{
			return ulong.TryParse(value, out var output) && ((output == 0UL) ? 1 : ((int)Math.Floor(Math.Log10(output) + 1.0))) >= 17;
		});

		LoadFromDatafile();
		CleanUp();
	}

	public static readonly char[] Star = ['*'];
	public static readonly string StarStr = "*";

	public Dictionary<string, UserData> userdata = new(StringComparer.OrdinalIgnoreCase);
	public Dictionary<string, GroupData> groupdata = new(StringComparer.OrdinalIgnoreCase);
	public readonly Dictionary<BaseHookable, HashSet<string>> permset;

	private Func<string, bool> validate;

	internal static readonly UserData _blankUser = new();

	private static FieldInfo _iPlayerFieldCache;
	public static FieldInfo iPlayerField
		=> _iPlayerFieldCache ??= typeof(BasePlayer).GetField("IPlayer", BindingFlags.Public | BindingFlags.Instance);

	private const int ParentGroupDepthLimit = 32;

	public virtual void LoadFromDatafile()
	{
		Utility.DatafileToProto<Dictionary<string, UserData>>("oxide.users", true);
		Utility.DatafileToProto<Dictionary<string, GroupData>>("oxide.groups", true);

		var needsUserSave = false;
		var needsGroupSave = false;

		userdata = (ProtoStorage.Load<Dictionary<string, UserData>>("oxide.users") ?? new Dictionary<string, UserData>(StringComparer.OrdinalIgnoreCase));
		{
			var validatedUsers = new Dictionary<string, UserData>(StringComparer.OrdinalIgnoreCase);
			var groupSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var permissionSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach (var data in userdata)
			{
				var value = data.Value;

				permissionSet.Clear();
				groupSet.Clear();

				foreach (string item in value.Perms)
				{
					permissionSet.Add(item);
				}

				value.Perms = new HashSet<string>(permissionSet, StringComparer.OrdinalIgnoreCase);

				foreach (string item2 in value.Groups)
				{
					groupSet.Add(item2);
				}

				value.Groups = new HashSet<string>(groupSet, StringComparer.OrdinalIgnoreCase);
				if (validatedUsers.TryGetValue(data.Key, out var userData))
				{
					userData.Perms.UnionWith(value.Perms);
					userData.Groups.UnionWith(value.Groups);
					needsUserSave = true;
				}
				else
				{
					validatedUsers.Add(data.Key, value);
				}
			}

			permissionSet.Clear();
			groupSet.Clear();
			userdata.Clear();
			userdata = null;
			userdata = validatedUsers;

			CovalencePlugin.PlayerManager.RefreshDatabase(userdata);
		}

		groupdata = (ProtoStorage.Load<Dictionary<string, GroupData>>("oxide.groups") ?? new Dictionary<string, GroupData>());
		{
			var validatedGroups = new Dictionary<string, GroupData>(StringComparer.OrdinalIgnoreCase);
			var permissionSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach (var data in groupdata)
			{
				var value = data.Value;
				permissionSet.Clear();
				foreach (var item in value.Perms)
				{
					permissionSet.Add(item);
				}
				value.Perms = new HashSet<string>(permissionSet, StringComparer.OrdinalIgnoreCase);
				if (validatedGroups.ContainsKey(data.Key))
				{
					validatedGroups[data.Key].Perms.UnionWith(value.Perms);
					needsGroupSave = true;
				}
				else
				{
					validatedGroups.Add(data.Key, value);
				}
			}

			foreach (var data in groupdata)
			{
				if (!string.IsNullOrEmpty(data.Value.ParentGroup) && HasCircularParent(data.Key, data.Value.ParentGroup))
				{
					Logger.Warn("Detected circular parent group for '{keyValuePair.Key}'! Removing parent '{keyValuePair.Value.ParentGroup}'");
					data.Value.ParentGroup = null;
					needsGroupSave = true;
				}
			}

			permissionSet.Clear();
			groupdata.Clear();
			groupdata = null;
			groupdata = validatedGroups;
		}

		var playerDefaultGroup = Community.Runtime.Config.Permissions.PlayerDefaultGroup;
		var adminDefaultGroup = Community.Runtime.Config.Permissions.AdminDefaultGroup;
		var moderatorDefaultGroup = Community.Runtime.Config.Permissions.ModeratorDefaultGroup;

		if (!string.IsNullOrEmpty(playerDefaultGroup) && !GroupExists(playerDefaultGroup))
			CreateGroup(playerDefaultGroup, playerDefaultGroup.ToCamelCase(), 0);
		if (!string.IsNullOrEmpty(adminDefaultGroup) && !GroupExists(adminDefaultGroup))
			CreateGroup(adminDefaultGroup, adminDefaultGroup.ToCamelCase(), 1);
		if (!string.IsNullOrEmpty(moderatorDefaultGroup) && !GroupExists(moderatorDefaultGroup))
			CreateGroup(moderatorDefaultGroup, moderatorDefaultGroup.ToCamelCase(), 1);

		IsLoaded = true;

		if (needsUserSave)
		{
			SaveUsers();
		}

		if (needsGroupSave)
		{
			SaveGroups();
		}
	}

	public virtual void Export(string prefix = "auth")
	{
		if (!IsLoaded) return;

		Interface.Oxide.DataFileSystem.WriteObject(prefix + ".groups", groupdata, false);
		Interface.Oxide.DataFileSystem.WriteObject(prefix + ".users", userdata, false);
	}

	public virtual void SaveData()
	{
		SaveUsers();
		SaveGroups();
	}
	public virtual void SaveUsers()
	{
		ProtoStorage.Save(userdata, "oxide.users");
	}
	public virtual void SaveGroups()
	{
		ProtoStorage.Save(groupdata, "oxide.groups");
	}

	public virtual void RegisterValidate(Func<string, bool> val)
	{
		validate = val;
	}

	public virtual void CleanUp()
	{
		if (!IsLoaded || validate == null) return;

		using var pooled = Pool.Get<PooledList<string>>();
		foreach (var key in userdata.Keys)
		{
			if (!validate(key)) pooled.Add(key);
		}

		if (pooled.Count == 0) return;

		for (var i = 0; i < pooled.Count; i++)
		{
			userdata.Remove(pooled[i]);
		}
	}

	public virtual void MigrateGroup(string oldGroup, string newGroup)
	{
		if (!IsLoaded) return;

		if (GroupExists(oldGroup))
		{
			var fileDataPath = ProtoStorage.GetFileDataPath("oxide.groups.data");

			File.Copy(fileDataPath, fileDataPath + ".old", true);
			foreach (string perm in GetGroupPermissions(oldGroup, false))
			{
				GrantGroupPermission(newGroup, perm, null);
			}

			if (GetUsersInGroup(oldGroup).Length == 0)
			{
				RemoveGroup(oldGroup);
			}
		}
	}

	public virtual void RegisterPermission(string name, BaseHookable owner)
	{
		if (string.IsNullOrEmpty(name))
		{
			return;
		}

		if (!name.IsLower())
		{
			name = name.ToLower();
		}

		if (PermissionExists(name, owner))
		{
			return;
		}

		if (PermissionExists(name))
		{
			Logger.Warn($"Trying to register permission '{name}' but already used by another plugin. (Requestee plugin '{owner.Name}')");
			return;
		}

		if (!permset.TryGetValue(owner, out var hashSet))
		{
			hashSet = new HashSet<string>();
			permset.Add(owner, hashSet);
		}
		hashSet.Add(name);
		// OnPermissionRegistered
		HookCaller.CallStaticHook(4257240972, name, owner);
	}
	public virtual void UnregisterPermissions(BaseHookable owner)
	{
		if (owner == null)
		{
			return;
		}

		if (permset.TryGetValue(owner, out var hashSet))
		{
			hashSet.Clear();
			permset.Remove(owner);

			// OnPermissionsUnregistered
			HookCaller.CallStaticHook(2952085131, owner);
		}
	}
	public virtual bool PermissionExists(string name, BaseHookable owner = null)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}

		if (!name.IsLower())
		{
			name = name.ToLower();
		}

		var lastIsStar = name[name.Length - 1] == '*';
		var isAllStar = lastIsStar && name.Length == 1;
		var prefixLen = lastIsStar ? name.Length - 1 : 0;

		if (owner == null)
		{
			if (permset.Count == 0)
			{
				return false;
			}

			if (lastIsStar)
			{
				if (isAllStar)
				{
					foreach (var kvp in permset)
					{
						if (kvp.Value.Count > 0) return true;
					}
					return false;
				}

				foreach (var kvp in permset)
				{
					var set = kvp.Value;
					if (set.Count == 0) continue;
					foreach (var p in set)
					{
						if (StartsWithPrefix(p, name, prefixLen)) return true;
					}
				}
				return false;
			}

			foreach (var kvp in permset)
			{
				if (kvp.Value.Contains(name)) return true;
			}
			return false;
		}

		if (!permset.TryGetValue(owner, out var ownerSet) || ownerSet.Count == 0)
		{
			return false;
		}

		if (lastIsStar)
		{
			if (isAllStar) return true;

			foreach (var p in ownerSet)
			{
				if (StartsWithPrefix(p, name, prefixLen)) return true;
			}
			return false;
		}

		return ownerSet.Contains(name);
	}

	public virtual bool UserIdValid(string id)
	{
		return validate == null || validate(id);
	}
	public virtual bool UserExists(string id)
	{
		if (userdata.ContainsKey(id))
		{
			return true;
		}

		foreach (var user in userdata)
		{
			if (user.Value.LastSeenNickname.Equals(id, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}
	public virtual bool UserExists(string id, out UserData data)
	{
		return userdata.TryGetValue(id, out data);
	}

	public virtual UserData GetUserData(string id, bool addIfNotExisting = false)
	{
		if (!userdata.TryGetValue(id, out var result))
		{
			if (!addIfNotExisting)
			{
				return _blankUser;
			}

			userdata.Add(id, result = new UserData());
		}

		return result;
	}

	public virtual GroupData GetGroupData(string id)
	{
		if (groupdata.TryGetValue(id, out var result))
		{
			return result;
		}

		return null;
	}

	public virtual KeyValuePair<string, UserData> FindUser(string id)
	{
		if (id.IsSteamId())
			return new KeyValuePair<string, UserData>(id, GetUserData(id, true));

		using var validUsers = Pool.Get<PooledList<KeyValuePair<string, UserData>>>();
		validUsers.Clear();
		foreach (var user in userdata)
		{
			if (user.Value == null) continue;
			if (user.Key == id)
				return new KeyValuePair<string, UserData>(user.Key, user.Value);
			if (!string.IsNullOrEmpty(user.Value.LastSeenNickname) && user.Value.LastSeenNickname.IndexOf(id, StringComparison.InvariantCultureIgnoreCase) >= 0)
				validUsers.Add(user);
		}
		if (validUsers.Count >= 1)
		{
			if (validUsers.Count > 1)
			{
				Logger.Warn($"Found {validUsers.Count} users with '{id}' in nickname:");
				foreach (var validUser in validUsers)
					Logger.Warn($"  - {validUser.Key} ({validUser.Value.LastSeenNickname})");
				Logger.Warn($"Using first ({validUsers[0].Key}) as an result...");
			}
			return new KeyValuePair<string, UserData>(validUsers[0].Key, validUsers[0].Value);
		}

		return default;
	}

	public virtual void RefreshUser(BasePlayer player)
	{
		if (player == null)
		{
			return;
		}

		var perms = Community.Runtime.Config.Permissions;
		var userId = player.UserIDString;

		var user = GetUserData(userId, addIfNotExisting: true);
		user.Player = player.AsIPlayer();
		user.LastSeenNickname = player.displayName;

		if (player.net != null && player.net.connection != null && player.net.connection.info != null)
		{
			user.Language = player.net.connection.info.GetString("global.language", Community.Runtime.Config.Language);
		}
		else
		{
			user.Language = Community.Runtime.Config.Language;
		}

		CommitUser(userId, user);

		var playerDefaultGroup = perms.PlayerDefaultGroup;
		var adminDefaultGroup = perms.AdminDefaultGroup;
		var moderatorDefaultGroup = perms.ModeratorDefaultGroup;

		if (perms.AutoGrantPlayerGroup && !string.IsNullOrEmpty(playerDefaultGroup))
		{
			AddUserGroup(userId, playerDefaultGroup);
		}

		if (perms.AutoGrantAdminGroup && !string.IsNullOrEmpty(adminDefaultGroup))
		{
			if (player.net is { connection.authLevel: 2 })
			{
				AddUserGroup(userId, adminDefaultGroup);
			}
			else if (UserHasGroup(userId, adminDefaultGroup))
			{
				RemoveUserGroup(userId, adminDefaultGroup);
			}
		}

		if (perms.AutoGrantModeratorGroup && !string.IsNullOrEmpty(moderatorDefaultGroup))
		{
			if (player.net is { connection.authLevel: 1 })
			{
				AddUserGroup(userId, moderatorDefaultGroup);
			}
			else if (UserHasGroup(userId, moderatorDefaultGroup))
			{
				RemoveUserGroup(userId, moderatorDefaultGroup);
			}
		}

		if (!string.IsNullOrEmpty(adminDefaultGroup) && player.net is { connection.authLevel: 3 })
		{
			AddUserGroup(userId, adminDefaultGroup);
		}

		var existing = iPlayerField.GetValue(player);
		RustPlayer rustPlayer;
		if (existing == null)
		{
			rustPlayer = new RustPlayer(player);
			iPlayerField.SetValue(player, rustPlayer);
		}
		else
		{
			rustPlayer = (RustPlayer)existing;
		}

		rustPlayer.Object = player;
		rustPlayer.Name = player.displayName.Sanitize();
	}
	public virtual void UpdateNickname(string id, string nickname)
	{
		if (!UserExists(id))
		{
			return;
		}

		var userData = GetUserData(id);
		var lastSeenNickname = userData.LastSeenNickname;
		userData.LastSeenNickname = nickname.Sanitize();
		CommitUser(id, userData);

		// OnUserNameUpdated
		HookCaller.CallStaticHook(4255507790, id, lastSeenNickname, userData.LastSeenNickname);
	}
	public virtual void CommitUser(string userId, UserData data)
	{

	}

	public virtual bool UserHasAnyGroup(string id)
	{
		return UserExists(id) && GetUserData(id).Groups.Count > 0;
	}
	public virtual bool GroupsHavePermission(HashSet<string> groups, string perm)
	{
		if (groups != null && groups.Count != 0 && !string.IsNullOrEmpty(perm))
		{
			foreach (var group in groups)
			{
				if (GroupHasPermission(group, perm))
				{
					return true;
				}
			}
		}

		return false;
	}
	public virtual bool GroupHasPermission(string name, string perm)
	{
		if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(perm))
		{
			return false;
		}

		var current = name;
		for (var depth = 0; depth < ParentGroupDepthLimit; depth++)
		{
			if (!groupdata.TryGetValue(current, out var data))
			{
				return false;
			}

			if (data.Perms.Count > 0 && data.Perms.Contains(perm))
			{
				return true;
			}

			var parent = data.ParentGroup;
			if (string.IsNullOrEmpty(parent) || string.Equals(parent, current, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			current = parent;
		}
		return false;
	}
	public virtual bool UserHasPermission(string id, string perm)
	{
		if (string.IsNullOrEmpty(perm) || string.IsNullOrEmpty(id) || perm.Equals(StarStr))
		{
			return false;
		}

		if (id.Equals("server_console"))
		{
			return true;
		}

		var userData = GetUserData(id);

		if (userData.Perms.Count > 0 && userData.Perms.Contains(perm))
		{
			return true;
		}

		var groups = userData.Groups;
		if (groups.Count > 0)
		{
			foreach (var group in groups)
			{
				if (GroupHasPermission(group, perm))
				{
					return true;
				}
			}
		}

		return false;
	}

	public virtual string[] GetUserGroups(string id)
	{
		var groups = GetUserData(id).Groups;
		return groups.Count == 0 ? Array.Empty<string>() : groups.ToArray();
	}
	public virtual string[] GetUserPermissions(string id)
	{
		var userData = GetUserData(id);
		var direct = userData.Perms;
		var groups = userData.Groups;

		if (direct.Count == 0 && groups.Count == 0) return Array.Empty<string>();

		using var pooled = Pool.Get<PooledHashSet<string>>();

		foreach (var p in direct) pooled.Add(p);

		foreach (var group in groups)
		{
			CollectGroupPermissions(group, pooled);
		}

		if (pooled.Count == 0) return Array.Empty<string>();

		var arr = new string[pooled.Count];
		var i = 0;
		foreach (var s in pooled) arr[i++] = s;
		return arr;
	}
	public virtual string[] GetGroupPermissions(string name, bool parents = false)
	{
		if (string.IsNullOrEmpty(name) || !groupdata.TryGetValue(name, out var groupData))
		{
			return Array.Empty<string>();
		}

		using var pooled = Pool.Get<PooledHashSet<string>>();

		var current = groupData;
		var currentName = name;
		for (var depth = 0; depth < ParentGroupDepthLimit; depth++)
		{
			foreach (var p in current.Perms) pooled.Add(p);
			var parent = current.ParentGroup;
			if (string.IsNullOrEmpty(parent) || string.Equals(parent, currentName, StringComparison.OrdinalIgnoreCase))
				break;
			if (!groupdata.TryGetValue(parent, out current))
				break;
			currentName = parent;
		}

		if (pooled.Count == 0) return Array.Empty<string>();

		var arr = new string[pooled.Count];
		var i = 0;
		foreach (var s in pooled) arr[i++] = s;
		return arr;
	}
	public virtual string[] GetPermissions()
	{
		if (permset.Count == 0) return Array.Empty<string>();

		using var pooled = Pool.Get<PooledHashSet<string>>();

		foreach (var kvp in permset)
		{
			foreach (var p in kvp.Value) pooled.Add(p);
		}
		if (pooled.Count == 0) return Array.Empty<string>();
		var arr = new string[pooled.Count];
		var i = 0;
		foreach (var s in pooled) arr[i++] = s;
		return arr;
	}
	public virtual string[] GetPermissions(BaseHookable hookable)
	{
		if (hookable == null || !permset.TryGetValue(hookable, out var set) || set.Count == 0)
		{
			return Array.Empty<string>();
		}

		var arr = new string[set.Count];
		var i = 0;
		foreach (var s in set) arr[i++] = s;
		return arr;
	}
	public virtual string[] GetPermissionUsers(string perm)
	{
		if (string.IsNullOrEmpty(perm)) return Array.Empty<string>();

		if (!perm.IsLower())
		{
			perm = perm.ToLower();
		}

		using var hashSet = Pool.Get<PooledHashSet<string>>();

		foreach (var keyValuePair in userdata)
		{
			var value = keyValuePair.Value;
			if (value.Perms.Count > 0 && value.Perms.Contains(perm))
			{
				hashSet.Add(keyValuePair.Key + "(" + value.LastSeenNickname + ")");
			}
		}

		if (hashSet.Count == 0) return Array.Empty<string>();

		var arr = new string[hashSet.Count];
		var i = 0;
		foreach (var s in hashSet) arr[i++] = s;
		return arr;
	}
	public virtual string[] GetPermissionGroups(string perm)
	{
		if (string.IsNullOrEmpty(perm)) return Array.Empty<string>();

		if (!perm.IsLower())
		{
			perm = perm.ToLower();
		}

		using var hashSet = Pool.Get<PooledHashSet<string>>();

		foreach (var keyValuePair in groupdata)
		{
			var value = keyValuePair.Value;
			if (value.Perms.Count > 0 && value.Perms.Contains(perm))
			{
				hashSet.Add(keyValuePair.Key);
			}
		}

		if (hashSet.Count == 0) return Array.Empty<string>();

		var arr = new string[hashSet.Count];
		var i = 0;
		foreach (var s in hashSet) arr[i++] = s;
		return arr;
	}

	public virtual void AddUserGroup(string id, string name, bool addIfNotExisting = false)
	{
		if (!GroupExists(name) || !GetUserData(id, addIfNotExisting).Groups.Add(name.ToLower()))
		{
			return;
		}

		// OnUserGroupAdded
		HookCaller.CallStaticHook(3116013984, id, name);
	}
	public virtual void RemoveUserGroup(string id, string name)
	{
		if (!name.IsLower())
		{
			name = name.ToLower();
		}

		if (!GroupExists(name)) return;

		var userData = GetUserData(id);

		if (name.Equals(StarStr))
		{
			if (userData.Groups.Count <= 0)
			{
				return;
			}

			foreach (var group in userData.Groups)
			{
				// OnUserGroupRemoved
				HookCaller.CallStaticHook(1018697706, id, group);
			}

			userData.Groups.Clear();
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
	public virtual bool UserHasGroup(string id, string name)
	{
		if (string.IsNullOrEmpty(name) || !groupdata.ContainsKey(name))
		{
			return false;
		}

		var groups = GetUserData(id).Groups;
		return groups.Count > 0 && groups.Contains(name);
	}
	public virtual bool GroupExists(string groupName)
	{
		if (string.IsNullOrEmpty(groupName))
		{
			return false;
		}

		if (groupName.Length == 1 && groupName[0] == '*')
		{
			return true;
		}

		return groupdata.ContainsKey(groupName);
	}

	public virtual string[] GetGroups()
	{
		return groupdata.Count == 0 ? Array.Empty<string>() : groupdata.Keys.ToArray();
	}
	public virtual string[] GetUsersInGroup(string group)
	{
		if (!GroupExists(group))
		{
			return Array.Empty<string>();
		}

		if (!group.IsLower())
		{
			group = group.ToLower();
		}

		using var pooled = Pool.Get<PooledList<string>>();

		foreach (var u in userdata)
		{
			var value = u.Value;
			if (value.Groups.Count > 0 && value.Groups.Contains(group))
			{
				pooled.Add(u.Key + " (" + value.LastSeenNickname + ")");
			}
		}

		if (pooled.Count == 0) return Array.Empty<string>();

		var arr = new string[pooled.Count];
		for (var i = 0; i < pooled.Count; i++) arr[i] = pooled[i];
		return arr;
	}

	public virtual string GetGroupTitle(string group)
	{
		if (string.IsNullOrEmpty(group) || !groupdata.TryGetValue(group, out var groupData))
		{
			return string.Empty;
		}

		return groupData.Title;
	}
	public virtual int GetGroupRank(string group)
	{
		if (string.IsNullOrEmpty(group) || !groupdata.TryGetValue(group, out var groupData))
		{
			return default;
		}

		return groupData.Rank;
	}
	public virtual string GetGroupParent(string group)
	{
		if (string.IsNullOrEmpty(group) || !groupdata.TryGetValue(group, out var groupData))
		{
			return string.Empty;
		}

		return groupData.ParentGroup;
	}

	public virtual bool GrantUserPermission(string id, string perm, BaseHookable owner)
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

		if (perm.Length > 0 && perm[perm.Length - 1] == '*')
		{
			return GrantWildcard(data.Perms, perm, owner, id, isUser: true);
		}

		if (!data.Perms.Add(perm))
		{
			return false;
		}

		// OnUserPermissionGranted
		HookCaller.CallStaticHook(4054877424, id, perm);
		return true;
	}
	public virtual bool RevokeUserPermission(string id, string perm)
	{
		if (string.IsNullOrEmpty(perm))
		{
			return false;
		}

		var userData = GetUserData(id);
		if (userData.Perms.Count == 0) return false;

		if (!perm.IsLower())
		{
			perm = perm.ToLower();
		}

		if (perm.Length > 0 && perm[perm.Length - 1] == '*')
		{
			if (perm.Length == 1)
			{
				using var snapshot = Pool.Get<PooledList<string>>();
				snapshot.AddRange(userData.Perms);
				userData.Perms.Clear();

				for (var i = 0; i < snapshot.Count; i++)
				{
					// OnUserPermissionRevoked
					HookCaller.CallStaticHook(1879829838, id, snapshot[i]);
				}
				return true;
			}

			return RevokeWildcardUser(userData.Perms, perm, id);
		}

		if (!userData.Perms.Remove(perm))
		{
			return false;
		}

		// OnUserPermissionRevoked
		HookCaller.CallStaticHook(1879829838, id, perm);
		return true;
	}
	public virtual bool GrantGroupPermission(string name, string perm, BaseHookable owner)
	{
		if (!PermissionExists(perm, owner) || !GroupExists(name))
		{
			return false;
		}

		if (!name.IsLower())
		{
			name = name.ToLower();
		}

		if (!groupdata.TryGetValue(name, out var data)) return false;

		if (!perm.IsLower())
		{
			perm = perm.ToLower();
		}

		if (perm.Length > 0 && perm[perm.Length - 1] == '*')
		{
			return GrantWildcard(data.Perms, perm, owner, name, isUser: false);
		}

		if (!data.Perms.Add(perm))
		{
			return false;
		}

		// OnGroupPermissionGranted
		HookCaller.CallStaticHook(2479711677, name, perm);
		return true;
	}
	public virtual bool RevokeGroupPermission(string name, string perm)
	{
		if (string.IsNullOrEmpty(perm) || string.IsNullOrEmpty(name))
		{
			return false;
		}

		if (!name.IsLower())
		{
			name = name.ToLower();
		}

		if (!groupdata.TryGetValue(name, out var groupData))
		{
			return false;
		}

		if (groupData.Perms.Count == 0) return false;

		if (!perm.IsLower())
		{
			perm = perm.ToLower();
		}

		if (perm.Length > 0 && perm[perm.Length - 1] == '*')
		{
			if (perm.Length == 1)
			{
				foreach (var permission in groupData.Perms)
				{
					// OnGroupPermissionRevoked
					HookCaller.CallStaticHook(3443835039, name, permission);
				}

				groupData.Perms.Clear();
				return true;
			}

			return RevokeWildcardGroup(groupData.Perms, perm, name);
		}

		if (!groupData.Perms.Remove(perm))
		{
			return false;
		}

		// OnGroupPermissionRevoked
		HookCaller.CallStaticHook(3443835039, name, perm);
		return true;
	}

	public virtual bool CreateGroup(string group, string title, int rank)
	{
		if (string.IsNullOrEmpty(group) || GroupExists(group))
		{
			return false;
		}

		var value = new GroupData
		{
			Title = title,
			Rank = rank
		};

		if (!group.IsLower())
		{
			group = group.ToLower();
		}

		groupdata.Add(group, value);

		// OnGroupCreated
		HookCaller.CallStaticHook(1889097028, group, title, rank);
		return true;
	}
	public virtual bool RemoveGroup(string group)
	{
		if (string.IsNullOrEmpty(group)) return false;

		if (!group.IsLower())
		{
			group = group.ToLower();
		}

		var removed = groupdata.Remove(group);

		if (removed)
		{
			foreach (var groupData in groupdata.Values)
			{
				if (groupData.ParentGroup == group)
				{
					groupData.ParentGroup = string.Empty;
				}
			}
		}

		var anyChanged = false;
		foreach (var userData in userdata.Values)
		{
			if (userData.Groups.Count > 0 && userData.Groups.Remove(group))
			{
				anyChanged = true;
			}
		}
		if (anyChanged)
		{
			SaveUsers();
		}

		if (removed)
		{
			// OnGroupDeleted
			HookCaller.CallStaticHook(3702696305, group);
		}
		return removed;
	}

	public virtual bool SetGroupTitle(string group, string title)
	{
		if (string.IsNullOrEmpty(group)) return false;

		if (!group.IsLower())
		{
			group = group.ToLower();
		}

		if (!groupdata.TryGetValue(group, out var groupData)) return false;
		if (groupData.Title == title) return true;
		groupData.Title = title;

		// OnGroupTitleSet
		HookCaller.CallStaticHook(1035562059, group, title);
		return true;
	}
	public virtual bool SetGroupRank(string group, int rank)
	{
		if (string.IsNullOrEmpty(group)) return false;

		if (!group.IsLower())
		{
			group = group.ToLower();
		}

		if (!groupdata.TryGetValue(group, out var groupData)) return false;
		if (groupData.Rank == rank) return true;
		groupData.Rank = rank;

		// OnGroupRankSet
		HookCaller.CallStaticHook(407332709, group, rank);
		return true;
	}
	public virtual bool SetGroupParent(string group, string parent)
	{
		if (string.IsNullOrEmpty(group)) return false;

		if (!group.IsLower())
		{
			group = group.ToLower();
		}

		if (!groupdata.TryGetValue(group, out var groupData)) return false;

		if (string.IsNullOrEmpty(parent))
		{
			groupData.ParentGroup = null;
			return true;
		}

		if (!parent.IsLower())
		{
			parent = parent.ToLower();
		}

		if (!groupdata.ContainsKey(parent) || group.Equals(parent)) return false;

		if (!string.IsNullOrEmpty(groupData.ParentGroup) && groupData.ParentGroup.Equals(parent)) return true;
		if (HasCircularParent(group, parent)) return false;

		groupData.ParentGroup = parent;

		// OnGroupParentSet
		HookCaller.CallStaticHook(3763369361, group, parent);
		return true;
	}
	public virtual bool HasCircularParent(string group, string parent)
	{
		if (!groupdata.TryGetValue(parent, out var groupData))
		{
			return false;
		}

		using var hashSet = Pool.Get<PooledHashSet<string>>();
		hashSet.Add(group);
		hashSet.Add(parent);

		while (!string.IsNullOrEmpty(groupData.ParentGroup))
		{
			if (!hashSet.Add(groupData.ParentGroup))
			{
				return true;
			}
			if (!groupdata.TryGetValue(groupData.ParentGroup, out groupData))
			{
				return false;
			}
		}

		return false;
	}

	protected internal static bool StartsWithPrefix(string stored, string permWithStar, int prefixLen)
	{
		if (prefixLen == 0) return true;
		if (stored.Length < prefixLen) return false;
		return string.CompareOrdinal(stored, 0, permWithStar, 0, prefixLen) == 0;
	}

	private bool RevokeWildcardUser(HashSet<string> perms, string perm, string id)
	{
		var prefixLen = perm.Length - 1;

		using var pooled = Pool.Get<PooledList<string>>();
		foreach (var s in perms)
		{
			if (StartsWithPrefix(s, perm, prefixLen)) pooled.Add(s);
		}

		if (pooled.Count == 0) return false;

		for (var i = 0; i < pooled.Count; i++)
		{
			var s = pooled[i];
			if (perms.Remove(s))
			{
				// OnUserPermissionRevoked
				HookCaller.CallStaticHook(1879829838, id, s);
			}
		}
		return true;
	}

	private bool RevokeWildcardGroup(HashSet<string> perms, string perm, string name)
	{
		var prefixLen = perm.Length - 1;

		using var pooled = Pool.Get<PooledList<string>>();
		foreach (var s in perms)
		{
			if (StartsWithPrefix(s, perm, prefixLen)) pooled.Add(s);
		}

		if (pooled.Count == 0) return false;

		for (var i = 0; i < pooled.Count; i++)
		{
			var s = pooled[i];
			if (perms.Remove(s))
			{
				// OnGroupPermissionRevoked
				HookCaller.CallStaticHook(3443835039, name, s);
			}
		}
		return true;
	}

	private void CollectGroupPermissions(string name, HashSet<string> output)
	{
		if (string.IsNullOrEmpty(name)) return;

		var current = name;
		for (var depth = 0; depth < ParentGroupDepthLimit; depth++)
		{
			if (!groupdata.TryGetValue(current, out var data)) return;
			foreach (var p in data.Perms) output.Add(p);
			var parent = data.ParentGroup;
			if (string.IsNullOrEmpty(parent) || string.Equals(parent, current, StringComparison.OrdinalIgnoreCase))
				return;
			current = parent;
		}
	}

	private bool GrantWildcard(HashSet<string> target, string perm, BaseHookable owner, string subjectKey, bool isUser)
	{
		var hookId = isUser ? 4054877424u : 2479711677u;
		var any = false;

		if (perm.Length == 1)
		{
			if (owner == null)
			{
				foreach (var kvp in permset)
				{
					var src = kvp.Value;
					if (src.Count == 0) continue;
					foreach (var s in src)
					{
						if (target.Add(s))
						{
							any = true;
							HookCaller.CallStaticHook(hookId, subjectKey, s);
						}
					}
				}
			}
			else
			{
				if (!permset.TryGetValue(owner, out var src) || src.Count == 0) return false;
				foreach (var s in src)
				{
					if (target.Add(s))
					{
						any = true;
						HookCaller.CallStaticHook(hookId, subjectKey, s);
					}
				}
			}
			return any;
		}

		var prefixLen = perm.Length - 1;

		if (owner == null)
		{
			foreach (var kvp in permset)
			{
				var src = kvp.Value;
				if (src.Count == 0) continue;
				foreach (var s in src)
				{
					if (StartsWithPrefix(s, perm, prefixLen) && target.Add(s))
					{
						any = true;
						HookCaller.CallStaticHook(hookId, subjectKey, s);
					}
				}
			}
		}
		else
		{
			if (!permset.TryGetValue(owner, out var src) || src.Count == 0) return false;
			foreach (var s in src)
			{
				if (StartsWithPrefix(s, perm, prefixLen) && target.Add(s))
				{
					any = true;
					HookCaller.CallStaticHook(hookId, subjectKey, s);
				}
			}
		}

		return any;
	}
}
