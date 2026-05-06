using API.Hooks;
using Carbon.Managers;
using Oxide.Core.Plugins;
using Oxide.Plugins;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("LoadDefaultConfig", "LoadDefaultConfig", typeof(Plugin), "LoadConfig")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Engine")]
		[MetadataAttribute.Info("Gets called when a plugin should initialize default config.")]
		[MetadataAttribute.Info("You should not use this. Override `LoadDefaultConfig` virtual method instead.")]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class LoadDefaultConfig : Patch
		{
		}
	}
}
