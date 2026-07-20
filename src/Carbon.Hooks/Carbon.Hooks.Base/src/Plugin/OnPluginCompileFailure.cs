using System;
using API.Hooks;
using Carbon.Managers;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Plugin
{
	public partial class Plugin_Outdated
	{
		[HookAttribute.Patch("OnPluginCompileFailure", "OnPluginCompileFailure", typeof(ScriptLoader), "Compile")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Plugin")]
		[MetadataAttribute.Info("Gets called whenever a plugin compilation fails.")]
		[MetadataAttribute.Parameter("pluginName", typeof(string))]
		[MetadataAttribute.Parameter("error", typeof(Exception))]

		public class OnPluginCompileFailure : Patch;
	}
}
