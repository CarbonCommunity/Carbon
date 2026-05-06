using API.Hooks;
using Oxide.Core.Libraries;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnUserPermissionGranted", "OnUserPermissionGranted", typeof(Permission), nameof(Permission.GrantUserPermission))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Permissions")]
		[MetadataAttribute.Info("Gets called when a permission has been granted to a user.")]
		[MetadataAttribute.Parameter("playerId", typeof(string))]
		[MetadataAttribute.Parameter("permission", typeof(string))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnUserPermissionGranted : Patch;
	}
}
