using API.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Structure
{
	public partial class Structure_Hooks
	{
		[HookAttribute.Patch("OnCupboardAssign", "OnCupboardAssign", typeof(CorePlugin), "IOnCupboardAuthorize")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Structure")]
		[MetadataAttribute.Info("Called when a player is assigned to a cupboard.")]
		[MetadataAttribute.Parameter("priv", typeof(BuildingPrivlidge))]
		[MetadataAttribute.Parameter("targetId", typeof(ulong))]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Return(typeof(object))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnCupboardAssign : Patch
		{

		}
	}
}
