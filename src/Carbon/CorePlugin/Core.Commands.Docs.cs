namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("webrcon.rpc")]
	[AuthLevel(2)]
	private void OnWebRConRPC(ConsoleSystem.Arg arg)
	{
		WebControlPanel.RunCommand(arg);
	}

	[ConsoleCommand("loadwebpanelconfig", "Loads the Carbon WebControlPanel configuration (refreshes authorization accounts)")]
	[AuthLevel(2)]
	private void LoadWebControlPanelConfig(ConsoleSystem.Arg arg)
	{
		WebControlPanel.LoadConfig();
	}

	[ConsoleCommand("savewebpanelconfig", "Saves the Carbon WebControlPanel configuration")]
	[AuthLevel(2)]
	private void SaveWebControlPanelConfig(ConsoleSystem.Arg arg)
	{
		WebControlPanel.SaveConfig();
	}
}
