using API.Hooks;
using Oxide.Core.Libraries;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Engine
{
	public partial class Engine_Hooks
	{
		[HookAttribute.Patch("OnUserNameUpdated", "OnUserNameUpdated", typeof(Permission), nameof(Permission.UpdateNickname))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Permissions")]
		[MetadataAttribute.Info("Gets called the nickname of a player gets updated.")]
		[MetadataAttribute.Parameter("id", typeof(string))]
		[MetadataAttribute.Parameter("oldName", typeof(string))]
		[MetadataAttribute.Parameter("newName", typeof(string))]
		[MetadataAttribute.Assembly("Carbon.Common.dll")]
		[MetadataAttribute.OxideCompatible]

		public class OnUserNameUpdated : Patch;
	}
}
