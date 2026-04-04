using API.Hooks;
using Carbon.Core;
using Network;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnPlayerBanned", "OnPlayerBanned", typeof(CorePlugin), "IOnPlayerBanned")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Gets called when a connection gets banned.")]
		[MetadataAttribute.Parameter("connection", typeof(Connection))]
		[MetadataAttribute.Parameter("reason", typeof(string))]
		[MetadataAttribute.OxideCompatible]

		public class OnPlayerBanned : Patch;
	}
}
