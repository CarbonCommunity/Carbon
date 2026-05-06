using API.Hooks;
using Oxide.Game.Rust.Cui;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnDestroyUI", "OnDestroyUI", typeof(CuiHelper), nameof(CuiHelper.DestroyUi))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("CUI")]
		[MetadataAttribute.Info("Gets called when an UI is being destroyed on a client.")]
		[MetadataAttribute.Info("`name` is the name of the client panel.")]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("name", typeof(string))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnDestroyUI : Patch
		{

		}
	}
}
