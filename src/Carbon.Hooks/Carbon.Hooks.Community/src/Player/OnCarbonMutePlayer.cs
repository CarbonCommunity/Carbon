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
		[HookAttribute.Patch("OnCarbonMutePlayer", "OnCarbonMutePlayer", typeof(AdminModule), "MutePlayer")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Called when a player becomes mute.")]
		[MetadataAttribute.Parameter("invoker", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("target", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("wants", typeof(bool))]
		[MetadataAttribute.Parameter("reason", typeof(string))]

		public class OnCarbonMutePlayer : Patch;
	}
}
