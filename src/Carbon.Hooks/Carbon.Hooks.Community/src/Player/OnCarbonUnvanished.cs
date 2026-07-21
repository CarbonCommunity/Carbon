using API.Hooks;
using Carbon.Modules;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnCarbonUnvanished", "OnCarbonUnvanished", typeof(VanishModule), nameof(VanishModule.DoVanish))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Called when a player becomes unvanished.")]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]

		public class OnCarbonUnvanished : Patch;
	}
}
