using API.Hooks;
using Carbon.Core;
using Oxide.Plugins;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnPluginUnloaded", "OnPluginUnloaded", typeof(ModLoader), nameof(ModLoader.UninitializePlugin))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Engine")]
		[MetadataAttribute.Info("Gets called when a plugin is unloaded.")]
		[MetadataAttribute.Parameter("plugin", typeof(RustPlugin))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnPluginUnloaded : Patch
		{

		}
	}
}
