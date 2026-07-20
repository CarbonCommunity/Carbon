using API.Hooks;
using Carbon.Core;
using static ConVar.Chat;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnPlayerChat", "OnPlayerChat", typeof(CorePlugin), "IOnPlayerChat")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Gets called when a player sends a chat message.")]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("message", typeof(string))]
		[MetadataAttribute.Parameter("channel", typeof(ChatChannel))]
		[MetadataAttribute.OxideCompatible]

		public class OnPlayerChat : Patch;
	}
}
