using API.Hooks;
using Carbon.Core;
using Network;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnUserApprove", "OnUserApprove", typeof(CorePlugin), "IOnUserApprove")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Gets called when a connection is or not approved to join the server.")]
		[MetadataAttribute.Parameter("connection", typeof(Connection))]
		[MetadataAttribute.OxideCompatible]

		public class OnUserApprove : Patch;
	}
}
