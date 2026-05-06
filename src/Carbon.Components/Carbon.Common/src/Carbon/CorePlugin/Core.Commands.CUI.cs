using Oxide.Game.Rust.Cui;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	[ConsoleCommand("wipeui", "Clears the entire CUI containers and their elements from the caller's client.")]
	[AuthLevel(2)]
	private void WipeUI(ConsoleSystem.Arg arg)
	{
		if (arg.Player() is BasePlayer player)
		{
			arg.ReplyWith($"Cleared {CuiHelper.DestroyActivePanelList(player):n0} CUI panels");
		}
		else
		{
			arg.ReplyWith($"This command can only be called from a client.");
		}
	}
}
