using API.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Plugin
{
	public partial class Plugin_Unload
	{
		[HookAttribute.Patch("Unload", "Unload [Instance]", typeof(ModLoader), nameof(ModLoader.InitializePlugin), null)]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Plugin")]
		[MetadataAttribute.Info("Gets called when the plugin has fully shut down and disposed.")]
		[MetadataAttribute.OxideCompatible]

		public class Unload : Patch
		{

		}
	}
}
