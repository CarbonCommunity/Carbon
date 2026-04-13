using API.Hooks;
using Oxide.Core.Libraries;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnGroupTitleSet", "OnGroupTitleSet", typeof(Permission), nameof(Permission.SetGroupTitle))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Permissions")]
		[MetadataAttribute.Info("Gets called when group title is set.")]
		[MetadataAttribute.Parameter("group", typeof(string))]
		[MetadataAttribute.Parameter("title", typeof(string))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnGroupTitleSet : Patch;
	}
}
