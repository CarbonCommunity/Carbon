using API.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Server
{
	public partial class Server_Hooks
	{
		[HookAttribute.Patch("OnServerShutdown", "OnServerShutdown", typeof(CorePlugin), "IOnServerShutdown")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Server")]
		[MetadataAttribute.Info("Called on server shutdown.")]
		[MetadataAttribute.OxideCompatible]

		public class OnServerShutdown : Patch;
	}
}
