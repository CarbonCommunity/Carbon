using API.Hooks;
using Oxide.Core.Libraries;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnUserGroupRemoved", "OnUserGroupRemoved", typeof(Permission), nameof(Permission.RemoveUserGroup))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Permissions")]
		[MetadataAttribute.Info("Whenever an user is removed from a group.")]
		[MetadataAttribute.Parameter("playerId", typeof(string))]
		[MetadataAttribute.Parameter("group", typeof(string))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnUserGroupRemoved : Patch;
	}
}
