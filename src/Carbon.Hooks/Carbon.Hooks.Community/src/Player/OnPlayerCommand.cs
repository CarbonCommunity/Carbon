using API.Hooks;
using Carbon.Core;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class Player_Hooks
	{
		[HookAttribute.Patch("OnPlayerCommand", "OnPlayerCommand", typeof(CorePlugin), nameof(CorePlugin.IOnPlayerCommand))]
		[HookAttribute.Options(HookFlags.MetadataOnly)]

		[MetadataAttribute.Category("Player")]
		[MetadataAttribute.Info("Gets called when a player executes command.")]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("command", typeof(string))]
		[MetadataAttribute.Parameter("args", typeof(string[]))]
		[MetadataAttribute.Return(typeof(object))]
		[MetadataAttribute.OxideCompatible]

		public class OnPlayerCommand : Patch;
	}
}
