using Carbon.Documentation;

namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("webrcon.rpc")]
	[AuthLevel(2)]
	private void OnWebRConRPC(ConsoleSystem.Arg arg)
	{
		WebControlPanel.Run(arg);
	}
}
