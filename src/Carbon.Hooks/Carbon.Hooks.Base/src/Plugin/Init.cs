using Carbon.Hooks;
using Carbon.Core;
using Carbon.Plugins;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Plugin
{
	public partial class Plugin_Init
	{
		[HookAttribute.Patch("Init", "Init [Instance]", typeof(ModLoader), nameof(ModLoader.InitializePlugin), null)]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Plugin")]
		[MetadataAttribute.Info("Gets called right after config and lang phrases are read.")]
		[MetadataAttribute.Return(typeof(void), Discarded = true)]

		public class Init : Patch
		{

		}
	}
}
