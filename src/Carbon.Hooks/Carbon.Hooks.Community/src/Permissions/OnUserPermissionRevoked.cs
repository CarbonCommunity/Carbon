using Carbon.Hooks;
using Carbon.Plugins;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnUserPermissionRevoked", "OnUserPermissionRevoked", typeof(Permission), nameof(Permission.RevokeUserPermission))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Permissions")]
		[MetadataAttribute.Info("Gets called when a permission has been revoked from a user.")]
		[MetadataAttribute.Parameter("playerId", typeof(string))]
		[MetadataAttribute.Parameter("permission", typeof(string))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.Return(typeof(void), Discarded = true)]

		public class OnUserPermissionRevoked : Patch;
	}
}
