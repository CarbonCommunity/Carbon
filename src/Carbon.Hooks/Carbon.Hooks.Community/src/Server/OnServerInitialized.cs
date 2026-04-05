using API.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Server
{
	public partial class Server_Hooks
	{
		[HookAttribute.Patch("OnServerInitialized", "OnServerInitialized", typeof(ServerMgr), "OpenConnection", [typeof(bool)])]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Server")]
		[MetadataAttribute.Info("Called after the server startup has been completed and is awaiting connections.")]
		[MetadataAttribute.Info("Also called for plugins that are hotloaded while the server is already started running.")]
		[MetadataAttribute.Parameter("initialized", typeof(bool), true)]
		[MetadataAttribute.OxideCompatible]

		public class OnServerInitialized : Patch;
	}
}
