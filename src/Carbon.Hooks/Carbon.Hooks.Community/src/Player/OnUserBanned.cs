using API.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnUserBanned", "OnUserBanned", typeof(CorePlugin), "OnServerUserSet")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Gets called when a player gets banned.")]
		[MetadataAttribute.Parameter("playerName", typeof(string))]
		[MetadataAttribute.Parameter("playerId", typeof(string))]
		[MetadataAttribute.Parameter("address", typeof(string))]
		[MetadataAttribute.Parameter("reason", typeof(string))]
		[MetadataAttribute.Parameter("expiry", typeof(long))]
		[MetadataAttribute.OxideCompatible]

		public class OnUserBanned : Patch;
	}
}
