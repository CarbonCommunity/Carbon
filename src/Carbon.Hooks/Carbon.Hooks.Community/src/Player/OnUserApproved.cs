using API.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnUserApproved", "OnUserApproved", typeof(CorePlugin), "IOnUserApprove")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Gets called when a connection is approved to join the server.")]
		[MetadataAttribute.Parameter("username", typeof(string))]
		[MetadataAttribute.Parameter("userid", typeof(string))]
		[MetadataAttribute.Parameter("ip", typeof(string))]
		[MetadataAttribute.OxideCompatible]

		public class OnUserApproved : Patch;
	}
}
