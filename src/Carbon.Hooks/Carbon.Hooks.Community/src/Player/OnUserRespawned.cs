using API.Hooks;
using Carbon.Core;
using Oxide.Core.Libraries.Covalence;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnUserRespawned", "OnUserRespawned", typeof(CorePlugin), "OnPlayerRespawned")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Gets called when a covalence player fully respawned.")]
		[MetadataAttribute.Parameter("player", typeof(IPlayer))]
		[MetadataAttribute.OxideCompatible]

		public class OnUserRespawned : Patch;
	}
}
