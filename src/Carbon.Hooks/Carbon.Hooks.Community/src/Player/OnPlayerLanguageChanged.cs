using Carbon.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnPlayerLanguageChanged", "OnPlayerLanguageChanged [BasePlayer]", typeof(CorePlugin), "OnPlayerSetInfo")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Gets called when a player's language gets changed.")]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("var", typeof(string))]
		[MetadataAttribute.Return(typeof(void), Discarded = true)]

		public class OnPlayerLanguageChanged_BasePlayer : Patch;
	}
}
