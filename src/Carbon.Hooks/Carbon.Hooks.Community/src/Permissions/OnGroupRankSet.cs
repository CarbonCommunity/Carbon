using Carbon.Hooks;
using Carbon.Plugins;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnGroupRankSet", "OnGroupRankSet", typeof(Permission), nameof(Permission.SetGroupRank))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Permissions")]
		[MetadataAttribute.Info("Gets called when group rank is set.")]
		[MetadataAttribute.Parameter("group", typeof(string))]
		[MetadataAttribute.Parameter("rank", typeof(int))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.Return(typeof(void), Discarded = true)]

		public class OnGroupRankSet : Patch;
	}
}
