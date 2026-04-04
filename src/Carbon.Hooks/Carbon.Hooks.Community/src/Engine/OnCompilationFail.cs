using API.Hooks;
using Carbon.Core;
using Carbon.Managers;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnCompilationFail", "OnCompilationFail", typeof(ScriptLoader), nameof(ScriptLoader.Compile))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Engine")]
		[MetadataAttribute.Info("Gets called when a plugin fails compiling.")]
		[MetadataAttribute.Parameter("file", typeof(string))]
		[MetadataAttribute.Parameter("result", typeof(ModLoader.CompilationResult))]
		[MetadataAttribute.Assembly("Carbon.dll")]

		public class OnCompilationFail : Patch
		{

		}
	}
}
