using API.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Item
{
	public partial class Item_OnLoseCondition
	{
		[HookAttribute.Patch("OnLoseCondition", "OnLoseCondition", typeof(CorePlugin), nameof(CorePlugin.IOnLoseCondition), null)]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Info("Called right before the condition of the item is modified.")]
		[MetadataAttribute.Parameter("item", typeof(Item))]
		[MetadataAttribute.Parameter("amount", typeof(float))]
		[MetadataAttribute.OxideCompatible]

		public class OnLoseCondition : Patch;
	}
}
