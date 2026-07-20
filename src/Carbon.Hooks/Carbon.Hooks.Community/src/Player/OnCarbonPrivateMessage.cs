using API.Hooks;
using Carbon.Core;
using Carbon.Modules;
using Network;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnCarbonPrivateMessage", "OnCarbonPrivateMessage", typeof(AdminModule), "PrivateMessagePlayer")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Called when a user sends a private message to another player via Carbon.")]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("target", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("message", typeof(string))]

		public class OnCarbonPrivateMessage : Patch;
	}
}
