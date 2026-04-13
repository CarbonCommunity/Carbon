using API.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnPlayerUnbanned", "OnPlayerUnbanned", typeof(CorePlugin), "OnServerUserRemove")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Gets called when a player gets unbanned.")]
		[MetadataAttribute.Parameter("playerName", typeof(string))]
		[MetadataAttribute.Parameter("steamId", typeof(ulong))]
		[MetadataAttribute.Parameter("address", typeof(string))]
		[MetadataAttribute.OxideCompatible]

		public class OnPlayerUnbanned : Patch;
	}
}
