using API.Hooks;
using Carbon.Core;
using Oxide.Core.Libraries.Covalence;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnUserDisconnected", "OnUserDisconnected", typeof(CorePlugin), "OnPlayerDisconnected")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Parameter("player", typeof(IPlayer))]
		[MetadataAttribute.Parameter("reason", typeof(string))]
		[MetadataAttribute.OxideCompatible]

		public class OnUserDisconnected : Patch;
	}
}
