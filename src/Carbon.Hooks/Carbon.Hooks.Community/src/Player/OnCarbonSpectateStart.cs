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
		[HookAttribute.Patch("OnCarbonSpectateStart", "OnCarbonSpectateStart", typeof(AdminModule), "StartSpectating")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Called when a player starts spectating another player.")]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("target", typeof(BasePlayer))]

		public class OnCarbonSpectateStart : Patch;
	}
}
