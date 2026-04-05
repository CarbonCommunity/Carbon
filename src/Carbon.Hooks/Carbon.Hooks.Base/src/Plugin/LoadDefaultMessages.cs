using API.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Plugin
{
	public partial class Plugin_LoadDefaultMessages
	{
		[HookAttribute.Patch("LoadDefaultMessages", "LoadDefaultMessages [Instance]", typeof(ModLoader), nameof(ModLoader.InitializePlugin), null)]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Plugin")]
		[MetadataAttribute.Info("Gets called when originally there aren't message found in the `carbon/lang` folder.")]
		[MetadataAttribute.Info("Gets called when it initially creates the `carbon/lang` files for the plugin.")]
		[MetadataAttribute.OxideCompatible]

		public class LoadDefaultMessages : Patch
		{

		}
	}
}
