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

	public Dictionary<string, UserData> userdata = [];
	public Dictionary<string, GroupData> groupdata = [];
	public readonly Dictionary<BaseHookable, HashSet<string>> permset;

	private Func<string, bool> validate;

	internal static readonly UserData _blankUser = new();

	private static FieldInfo _iPlayerFieldCache;
	public static FieldInfo iPlayerField
		=> _iPlayerFieldCache ??= typeof(BasePlayer).GetField("IPlayer", BindingFlags.Public | BindingFlags.Instance);

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

		var array = (from k in userdata.Keys
					 where !validate(k)
					 select k).ToArray();

		if (array.Length == 0) return;

		foreach (string key in array)
		{
			userdata.Remove(key);
		}

		Array.Clear(array, 0, array.Length);
		array = null;
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

		if (owner == null)
		{
			if (permset.Count > 0)
			{
				if (name.Equals(StarStr))
				{
					return true;
				}

				if (name.EndsWith(StarStr))
				{
					name = name.TrimEnd(Star);
					return permset.Values.SelectMany(v => v).Any(p => p.StartsWith(name));
				}
			}

			return permset.Values.Any(v => v.Contains(name));
		}

		if (!permset.TryGetValue(owner, out var hashSet)) return false;

		if (hashSet.Count > 0)
		{
			if (name.Equals(StarStr))
			{
				return true;
			}

			if (name.EndsWith(StarStr))
			{
				name = name.TrimEnd(Star);
				return hashSet.Any(p => p.StartsWith(name));
			}
		}

		return hashSet.Contains(name);
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

		var user = GetUserData(player.UserIDString, addIfNotExisting: true);
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

		CommitUser(player.UserIDString, user);

		if (Community.Runtime.Config.Permissions.AutoGrantPlayerGroup && !string.IsNullOrEmpty(Community.Runtime.Config.Permissions.PlayerDefaultGroup))
		{
			AddUserGroup(player.UserIDString, Community.Runtime.Config.Permissions.PlayerDefaultGroup);
		}

		if (Community.Runtime.Config.Permissions.AutoGrantAdminGroup && !string.IsNullOrEmpty(Community.Runtime.Config.Permissions.AdminDefaultGroup))
		{
			if (player.net is { connection.authLevel: 2 })
			{
				AddUserGroup(player.UserIDString, Community.Runtime.Config.Permissions.AdminDefaultGroup);
			}
			else if (UserHasGroup(player.UserIDString, Community.Runtime.Config.Permissions.AdminDefaultGroup))
			{
				RemoveUserGroup(player.UserIDString, Community.Runtime.Config.Permissions.AdminDefaultGroup);
			}
		}

		if (Community.Runtime.Config.Permissions.AutoGrantModeratorGroup &&!string.IsNullOrEmpty(Community.Runtime.Config.Permissions.ModeratorDefaultGroup))
		{
			if (player.net is { connection.authLevel: 1 })
			{
				AddUserGroup(player.UserIDString, Community.Runtime.Config.Permissions.ModeratorDefaultGroup);
			}
			else if (UserHasGroup(player.UserIDString, Community.Runtime.Config.Permissions.ModeratorDefaultGroup))
			{
				RemoveUserGroup(player.UserIDString, Community.Runtime.Config.Permissions.ModeratorDefaultGroup);
			}
		}

		RustPlayer rustPlayer;

		if (iPlayerField.GetValue(player) == null)
		{
			iPlayerField.SetValue(player, rustPlayer = new RustPlayer(player));
		}
		else
		{
			rustPlayer = (RustPlayer)iPlayerField.GetValue(player);
		}

		rustPlayer.Object = player;
	}
	public virtual void UpdateNickname(string id, string nickname)
	{
		if (UserExists(id))
		{
			var userData = GetUserData(id);
			var lastSeenNickname = userData.LastSeenNickname;
			userData.LastSeenNickname = nickname.Sanitize();
			CommitUser(id, userData);

			// OnUserNameUpdated
			HookCaller.CallStaticHook(4255507790, id, lastSeenNickname, userData.LastSeenNickname);
		}
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
		foreach (var group in groups)
		{
			if (GroupHasPermission(group, perm))
			{
				return true;
			}
		}

		return false;
	}
	public virtual bool GroupHasPermission(string name, string perm)
	{
		if (string.IsNullOrEmpty(name) || !GroupExists(name))
		{
			return false;
		}

		foreach (var group in groupdata)
		{
			if (group.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				if (group.Value.ParentGroup != name && GroupHasPermission(group.Value.ParentGroup, perm))
				{
					return true;
				}

				foreach (var permission in group.Value.Perms)
				{
					if (permission.Equals(perm, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
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

		if (GroupsHavePermission(userData.Groups, perm))
		{
			return true;
		}

		foreach (var permission in userData.Perms)
		{
			if (permission.Equals(perm, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}

	public virtual string[] GetUserGroups(string id)
	{
		return GetUserData(id).Groups.ToArray();
	}
	public virtual string[] GetUserPermissions(string id)
	{
		var userData = GetUserData(id);
		return new HashSet<string>(userData.Perms.Concat(userData.Groups.SelectMany(x => GetGroupPermissions(x, true)))).ToArray();
	}
	public virtual string[] GetGroupPermissions(string name, bool parents = false)
	{
		if (!GroupExists(name))
		{
			return Array.Empty<string>();
		}

		if (!groupdata.TryGetValue(name.ToLower(), out var groupData))
		{
			return Array.Empty<string>();
		}

		return new HashSet<string>(groupData.Perms.Concat(GetGroupPermissions(groupData.ParentGroup, parents))).ToArray();
	}
	public virtual string[] GetPermissions()
	{
		return new HashSet<string>(permset.Values.SelectMany(v => v)).ToArray();
	}
	public virtual string[] GetPermissions(BaseHookable hookable)
	{
		return new HashSet<string>(permset.Where(x => x.Key == hookable).SelectMany(v => v.Value)).ToArray();
	}
	public virtual string[] GetPermissionUsers(string perm)
	{
		if (string.IsNullOrEmpty(perm)) return Array.Empty<string>();

		if (!perm.IsLower())
		{
			perm = perm.ToLower();
		}

		var hashSet = Pool.Get<HashSet<string>>();
		foreach (var keyValuePair in userdata.Where(keyValuePair => keyValuePair.Value.Perms.Contains(perm)))
		{
			hashSet.Add(keyValuePair.Key + "(" + keyValuePair.Value.LastSeenNickname + ")");
		}

		var result = hashSet.ToArray();
		hashSet.Clear();
		Pool.FreeUnmanaged(ref hashSet);
		return result;
	}
	public virtual string[] GetPermissionGroups(string perm)
	{
		if (string.IsNullOrEmpty(perm)) return Array.Empty<string>();

		if (!perm.IsLower())
		{
			perm = perm.ToLower();
		}

		var hashSet = Pool.Get<HashSet<string>>();
		foreach (KeyValuePair<string, GroupData> keyValuePair in groupdata)
		{
			if (keyValuePair.Value.Perms.Contains(perm))
			{
				hashSet.Add(keyValuePair.Key);
			}
		}
		var result = hashSet.ToArray();
		hashSet.Clear();
		Pool.FreeUnmanaged(ref hashSet);
		return result;
	}

	public virtual void AddUserGroup(string id, string name)
	{
		if (!GroupExists(name) || !GetUserData(id).Groups.Add(name.ToLower()))
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
		if (!GroupExists(name))
		{
			return false;
		}

		foreach (var group in GetUserData(id).Groups)
		{
			if (group.Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}
	public virtual bool GroupExists(string groupName)
	{
		if (string.IsNullOrEmpty(groupName))
		{
			return false;
		}

		if (groupName.Equals(StarStr))
		{
			return true;
		}

		foreach (var group in groupdata)
		{
			if (group.Key.Equals(groupName, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}

	public virtual string[] GetGroups()
	{
		return [.. groupdata.Keys];
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

		return (from u in userdata
		        where u.Value.Groups.Contains(@group)
		        select u.Key + " (" + u.Value.LastSeenNickname + ")").ToArray();
	}

	public virtual string GetGroupTitle(string group)
	{
		if (!GroupExists(group))
		{
			return string.Empty;
		}

		if (!group.IsLower())
		{
			group = group.ToLower();
		}

		if (!groupdata.TryGetValue(group, out var groupData))
		{
			return string.Empty;
		}

		return groupData.Title;
	}
	public virtual int GetGroupRank(string group)
	{
		if (!GroupExists(group))
		{
			return default;
		}

		if (!group.IsLower())
		{
			group = group.ToLower();
		}

		if (!groupdata.TryGetValue(group, out var groupData))
		{
			return default;
		}

		return groupData.Rank;
	}
	public virtual string GetGroupParent(string group)
	{
		if (!GroupExists(group)) return string.Empty;

		if (!group.IsLower())
		{
			group = group.ToLower();
		}

		if (groupdata.TryGetValue(group, out var groupData))
		{
			return groupData.ParentGroup;
		}
		return string.Empty;
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

		if (perm.EndsWith(StarStr))
		{
			HashSet<string> source;
			if (owner == null)
			{
				source = new HashSet<string>(permset.Values.SelectMany(v => v));
			}
			else if (!permset.TryGetValue(owner, out source))
			{
				return false;
			}

			if (perm.Equals(StarStr))
			{
				return source.Aggregate(false, (c, s) =>
				{
					if (!(c | data.Perms.Add(s)))
					{
						return false;
					}

					// OnUserPermissionGranted
					HookCaller.CallStaticHook(4054877424, id, s);
					return true;

				});
			}
			perm = perm.TrimEnd(Star);

			return (from s in source
					where s.StartsWith(perm)
					select s).Aggregate(false, (c, s) =>
					{
						if (!(c | data.Perms.Add(s)))
						{
							return false;
						}

						// OnUserPermissionGranted
						HookCaller.CallStaticHook(4054877424, id, s);
						return true;

					});
		}
		else
		{
			if (!data.Perms.Add(perm))
			{
				return false;
			}

			// OnUserPermissionGranted
			HookCaller.CallStaticHook(4054877424, id, perm);
			return true;
		}
	}
	public virtual bool RevokeUserPermission(string id, string perm)
	{
		if (string.IsNullOrEmpty(perm))
		{
			return false;
		}

		var userData = GetUserData(id);

		if (!perm.IsLower())
		{
			perm = perm.ToLower();
		}

		if (perm.EndsWith(StarStr))
		{
			if (!perm.Equals(StarStr))
			{
				perm = perm.TrimEnd(Star);

				return userData.Perms.RemoveWhere(s =>
				{
					if (!s.StartsWith(perm))
					{
						return false;
					}

					// OnUserPermissionRevoked
					HookCaller.CallStaticHook(1879829838, id, s);
					return true;
				}) > 0;
			}

			if (userData.Perms.Count <= 0)
			{
				return false;
			}

			userData.Perms.Clear();
			return true;
		}
		else
		{
			if (!userData.Perms.Remove(perm))
			{
				return false;
			}

			// OnUserPermissionRevoked
			HookCaller.CallStaticHook(1879829838, id, perm);
			return true;
		}
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

		if (perm.EndsWith(StarStr))
		{
			HashSet<string> source;
			if (owner == null)
			{
				source = new HashSet<string>(permset.Values.SelectMany(v => v));
			}
			else if (!permset.TryGetValue(owner, out source))
			{
				return false;
			}
			if (perm.Equals(StarStr))
			{
				return source.Aggregate(false, (c, s) =>
				{
					if (!(c | data.Perms.Add(s)))
					{
						return false;
					}

					// OnGroupPermissionGranted
					HookCaller.CallStaticHook(2479711677, name, perm);
					return true;
				});
			}
			perm = perm.TrimEnd(Star).ToLower();

			return (from s in source
					where s.StartsWith(perm)
					select s).Aggregate(false, (c, s) =>
					{
						if (!(c | data.Perms.Add(s)))
						{
							return false;
						}

						// OnGroupPermissionGranted
						HookCaller.CallStaticHook(2479711677, name, perm);
						return true;
					});
		}
		else
		{
			if (!data.Perms.Add(perm))
			{
				return false;
			}

			// OnGroupPermissionGranted
			HookCaller.CallStaticHook(2479711677, name, perm);
			return true;
		}
	}
	public virtual bool RevokeGroupPermission(string name, string perm)
	{
		if (!GroupExists(name) || string.IsNullOrEmpty(perm))
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

		if (!perm.IsLower())
		{
			perm = perm.ToLower();
		}

		if (perm.EndsWith(StarStr))
		{
			if (!perm.Equals(StarStr))
			{
				perm = perm.TrimEnd(Star).ToLower();
				return groupData.Perms.RemoveWhere(s =>
				{
					if (!s.StartsWith(perm)) return false;
					// OnGroupPermissionRevoked
					HookCaller.CallStaticHook(3443835039, name, s);
					return true;

				}) > 0;
			}

			if (groupData.Perms.Count <= 0)
			{
				return false;
			}

			foreach (var permission in groupData.Perms)
			{
				// OnGroupPermissionRevoked
				HookCaller.CallStaticHook(3443835039, name, permission);
			}

			groupData.Perms.Clear();
			return true;
		}
		else
		{
			if (!groupData.Perms.Remove(perm))
			{
				return false;
			}

			// OnGroupPermissionRevoked
			HookCaller.CallStaticHook(3443835039, name, perm);
			return true;
		}
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
		if (!GroupExists(group))
		{
			return false;
		}

		if (!group.IsLower())
		{
			group = group.ToLower();
		}

		var removed = groupdata.Remove(group);

		if (removed)
		{
			foreach (var groupData in groupdata.Values.Where(groupData => groupData.ParentGroup == group))
			{
				groupData.ParentGroup = string.Empty;
			}
		}
		if (userdata.Values.Aggregate(false, (current, userData) => current | userData.Groups.Remove(group)))
		{
			SaveUsers();
		}
		if (removed)
		{
			// OnGroupDeleted
			HookCaller.CallStaticHook(3702696305, group);
		}
		return true;
	}

	public virtual bool SetGroupTitle(string group, string title)
	{
		if (!GroupExists(group)) return false;

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
		if (!GroupExists(group)) return false;

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
		if (!GroupExists(group)) return false;

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

		if (!GroupExists(parent) || group.Equals(parent)) return false;

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

		var hashSet = Pool.Get<HashSet<string>>();
		hashSet.Add(group);
		hashSet.Add(parent);

		void Cleanup()
		{
			hashSet.Clear();
			Pool.FreeUnmanaged(ref hashSet);
		}

		while (!string.IsNullOrEmpty(groupData.ParentGroup))
		{
			if (!hashSet.Add(groupData.ParentGroup))
			{
				Cleanup();
				return true;
			}
			if (!groupdata.TryGetValue(groupData.ParentGroup, out groupData))
			{
				Cleanup();
				return false;
			}
		}

		Cleanup();
		return false;
	}
}
