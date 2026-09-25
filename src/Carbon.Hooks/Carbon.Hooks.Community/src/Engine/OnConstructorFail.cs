using System;
using Carbon.Hooks;
using Carbon.Core;
using Carbon.Plugins;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnConstructorFail", "OnConstructorFail", typeof(ModLoader), nameof(ModLoader.InitializePlugin))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Engine")]
		[MetadataAttribute.Info("Gets called when a plugin's constructor throws an exception.")]
		[MetadataAttribute.Info("Fatal error which forcefully unloads the plugin right after.")]
		[MetadataAttribute.Parameter("plugin", typeof(Plugin))]
		[MetadataAttribute.Parameter("exception", typeof(Exception))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.Return(typeof(void), Discarded = true)]

		public class OnConstructorFail : Patch
		{

		}
	}
}
