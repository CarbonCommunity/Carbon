using API.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Server
{
	public partial class Server_Hooks
	{
		[HookAttribute.Patch("OnServerCommand", "OnServerCommand", typeof(CorePlugin), "IOnServerCommand")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Server")]
		[MetadataAttribute.Info("Gets called when executing a native command.")]
		[MetadataAttribute.Parameter("arg", typeof(ConsoleSystem.Arg))]
		[MetadataAttribute.Return(typeof(bool))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnServerCommand : Patch;
	}
}
