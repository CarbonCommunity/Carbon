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
		[HookAttribute.Patch("OnCarbonBanPlayer", "OnCarbonBanPlayer", typeof(AdminModule), "BanPlayer")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Called when a player becomes banned.")]
		[MetadataAttribute.Parameter("invoker", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("target", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("reason", typeof(string))]
		[MetadataAttribute.Parameter("expiry", typeof(long))]

		public class OnCarbonBanPlayer : Patch;
	}
}
