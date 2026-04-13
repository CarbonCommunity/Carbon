using API.Hooks;
using Carbon.Core;
using Network;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("CanClientLogin", "CanClientLogin", typeof(CorePlugin), "IOnUserApprove")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Gets called when a client should or not should join the server.")]
		[MetadataAttribute.Parameter("connection", typeof(Connection))]
		[MetadataAttribute.Return(typeof(bool))]
		[MetadataAttribute.OxideCompatible]

		public class CanClientLogin : Patch;
	}
}
