using API.Hooks;
using Oxide.Core.Libraries;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnGroupPermissionRevoked", "OnGroupPermissionRevoked", typeof(Permission), nameof(Permission.RevokeGroupPermission))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Permissions")]
		[MetadataAttribute.Info("Gets called when a permission has been revoked from a group.")]
		[MetadataAttribute.Parameter("group", typeof(string))]
		[MetadataAttribute.Parameter("permission", typeof(string))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnGroupPermissionRevoked : Patch;
	}
}
