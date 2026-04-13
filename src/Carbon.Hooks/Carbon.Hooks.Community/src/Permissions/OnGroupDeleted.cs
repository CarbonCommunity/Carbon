using API.Hooks;
using Oxide.Core.Libraries;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnGroupDeleted", "OnGroupDeleted", typeof(Permission), nameof(Permission.RemoveGroup))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Permissions")]
		[MetadataAttribute.Info("Gets called when group got obliterated.")]
		[MetadataAttribute.Parameter("group", typeof(string))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnGroupDeleted : Patch;
	}
}
