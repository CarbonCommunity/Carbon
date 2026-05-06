using API.Hooks;
using Oxide.Game.Rust.Cui;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("CanUseUI", "CanUseUI", typeof(CuiHelper), nameof(CuiHelper.AddUi))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("CUI")]
		[MetadataAttribute.Info("Gets called when an UI is about to be sent to a player.")]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("json", typeof(string))]
		[MetadataAttribute.Return(typeof(bool))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class CanUseUI : Patch
		{

		}
	}
}
