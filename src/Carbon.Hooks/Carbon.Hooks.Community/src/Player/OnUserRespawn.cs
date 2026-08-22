using API.Hooks;
using Carbon.Core;
using Oxide.Core.Libraries.Covalence;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnUserRespawn", "OnUserRespawn", typeof(CorePlugin), "OnPlayerRespawn")]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Gets called when a covalence player respawns.")]
		[MetadataAttribute.Info("Return a BasePlayer.SpawnPoint, or a SleepingBag when respawning at a bag, to change where the player respawns.")]
		[MetadataAttribute.Parameter("player", typeof(IPlayer))]
		[MetadataAttribute.OxideCompatible]

		public class OnUserRespawn : Patch;
	}
}
