using API.Hooks;
using Carbon.Core;

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
		[MetadataAttribute.OxideCompatible]

		public class Init : Patch
		{

		}
	}
}
