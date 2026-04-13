using API.Hooks;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnPermissionsUnregistered", "OnPermissionsUnregistered", typeof(Permission), nameof(Permission.UnregisterPermissions))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Permissions")]
		[MetadataAttribute.Info("Gets called when all permission of a plugin have been unregistered.")]
		[MetadataAttribute.Parameter("plugin", typeof(Plugin))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnPermissionsUnregistered : Patch;
	}
}
