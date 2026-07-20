using API.Hooks;
using Carbon.Core;
using Oxide.Core.Libraries.Covalence;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnUserConnected", "OnUserConnected", typeof(CorePlugin), "IOnPlayerConnected")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Parameter("player", typeof(IPlayer))]
		[MetadataAttribute.OxideCompatible]

		public class OnUserConnected : Patch;
	}
}
