namespace Carbon.Oxide;

public class PermissionStoreless : Permission
{
	public override void SaveData()
	{

	}
	public override void LoadFromDatafile()
	{
		userdata = [];
		groupdata = [];

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
	}
}
