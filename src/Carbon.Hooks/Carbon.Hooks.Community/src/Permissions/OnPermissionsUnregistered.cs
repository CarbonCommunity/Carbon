using Carbon.Hooks;
using Carbon.Plugins;

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
		[MetadataAttribute.Return(typeof(void), Discarded = true)]

		public class OnPermissionsUnregistered : Patch;
	}
}
