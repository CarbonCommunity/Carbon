using Carbon.Test;
using Oxide.Core.Libraries;

namespace Carbon.Plugins;

public partial class Tests
{
	public class Permission
	{
		public const string userId = "76561198158946080";
		public const string groupId = "integrationtestgroup";

		public UserData user;
		public GroupData group;

		[Integrations.Test.Assert]
		public void valid_id(Integrations.Test.Assert test)
		{
			test.IsTrue(singleton.permission.UserIdValid(userId), $"singleton.permission.UserIdValid(\"{userId}\")");
		}

		[Integrations.Test.Assert(CancelOnFail = false)]
		public void create_user(Integrations.Test.Assert test)
		{
			test.IsFalse(singleton.permission.UserExists(userId), $"singleton.permission.UserExists(\"{userId}\")");
			test.IsNull(user, "user");

			user = singleton.permission.GetUserData(userId, addIfNotExisting: true);
			test.IsNotNull(user, "user");
		}

		[Integrations.Test.Assert]
		public void create_group(Integrations.Test.Assert test)
		{
			test.IsFalse(singleton.permission.GroupExists(groupId), $"singleton.permission.GroupExists(\"{groupId}\")");
			test.IsNull(group, "group");

			test.IsTrue(singleton.permission.CreateGroup(groupId, groupId, 0), $"singleton.permission.CreateGroup(\"{groupId}\", \"{groupId}\", 0)");

			group = singleton.permission.GetGroupData(groupId);
			test.IsNotNull(group, "group");
		}

		[Integrations.Test.Assert]
		public void add_user_to_group(Integrations.Test.Assert test)
		{
			test.IsFalse(singleton.permission.UserHasGroup(userId, groupId), $"singleton.permission.UserHasGroup(\"{userId}\", \"{groupId}\")");
			singleton.permission.AddUserGroup(userId, groupId);
			test.Log($"singleton.permission.AddUserGroup(\"{userId}\", \"{groupId}\")");
		}

		[Integrations.Test.Assert]
		public void remove_user_from_group(Integrations.Test.Assert test)
		{
			test.IsTrue(singleton.permission.UserHasGroup(userId, groupId), $"singleton.permission.UserHasGroup(\"{userId}\", \"{groupId}\")");
			singleton.permission.RemoveUserGroup(userId, groupId);
			test.Log($"singleton.permission.RemoveUserGroup(\"{userId}\", \"{groupId}\")");
			test.IsFalse(singleton.permission.UserHasGroup(userId, groupId), $"singleton.permission.UserHasGroup(\"{userId}\", \"{groupId}\")");
		}

		[Integrations.Test.Assert]
		public void delete_user(Integrations.Test.Assert test)
		{
			test.IsTrue(singleton.permission.UserExists(userId), $"singleton.permission.UserExists(\"{userId}\")");
			test.IsTrue(singleton.permission.userdata.Remove(userId), $"singleton.permission.userdata.Remove(\"{userId}\")");
		}

		[Integrations.Test.Assert]
		public void delete_group(Integrations.Test.Assert test)
		{
			test.IsTrue(singleton.permission.GroupExists(groupId), $"singleton.permission.GroupExists(\"{groupId}\")");
			test.IsTrue(singleton.permission.groupdata.Remove(groupId), $"singleton.permission.groupdata.Remove(\"{groupId}\")");
		}
	}
} 