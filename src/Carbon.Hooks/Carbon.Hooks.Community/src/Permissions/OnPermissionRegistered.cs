using API.Hooks;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnPermissionRegistered", "OnPermissionRegistered", typeof(Permission), nameof(Permission.RegisterPermission))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Permissions")]
		[MetadataAttribute.Info("Gets called when a permission has been registered for a plugin.")]
		[MetadataAttribute.Parameter("permission", typeof(string))]
		[MetadataAttribute.Parameter("plugin", typeof(Plugin))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnPermissionRegistered : Patch;
	}
}
