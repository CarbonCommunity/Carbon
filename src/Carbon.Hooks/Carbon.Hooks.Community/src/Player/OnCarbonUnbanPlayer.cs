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
		[HookAttribute.Patch("OnCarbonUnbanPlayer", "OnCarbonUnbanPlayer", typeof(AdminModule), "UnbanPlayer")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Called when a player becomes banned.")]
		[MetadataAttribute.Parameter("invoker", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("target", typeof(BasePlayer))]

		public class OnCarbonUnbanPlayer : Patch;
	}
}
