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
		[HookAttribute.Patch("OnCarbonLockPlayerContainer", "OnCarbonLockPlayerContainer", typeof(AdminModule), "LockPlayerContainer")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Called when a player's inventory container becomes locked or unlocked.")]
		[MetadataAttribute.Parameter("invoker", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("target", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("container", typeof(ItemContainer))]
		[MetadataAttribute.Parameter("locked", typeof(bool))]

		public class OnCarbonLockPlayerContainer : Patch;
	}
}
