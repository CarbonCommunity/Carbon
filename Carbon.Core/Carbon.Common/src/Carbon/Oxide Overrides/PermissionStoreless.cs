/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Oxide;

public class PermissionStoreless : Permission
{
	public override void SaveData()
	{

	}
	public override void LoadFromDatafile()
	{
		userdata = new Dictionary<string, UserData>();
		groupdata = new Dictionary<string, GroupData>();

		if (!GroupExists(Community.Runtime.Config.PlayerDefaultGroup)) CreateGroup(Community.Runtime.Config.PlayerDefaultGroup, Community.Runtime.Config.PlayerDefaultGroup?.ToCamelCase(), 0);
		if (!GroupExists(Community.Runtime.Config.AdminDefaultGroup)) CreateGroup(Community.Runtime.Config.AdminDefaultGroup, Community.Runtime.Config.AdminDefaultGroup?.ToCamelCase(), 1);

		IsLoaded = true;
	}
}
