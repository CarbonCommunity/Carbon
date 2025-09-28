namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("webpanel.cmd")]
	[AuthLevel(2)]
	private void OnWebRConRPC(ConsoleSystem.Arg arg)
	{
		WebControlPanel.RunCommand(arg);
	}

	[ConsoleCommand("webpanel.loadcfg", "Loads the Carbon WebControlPanel configuration (refreshes authorization accounts)")]
	[AuthLevel(2)]
	private void LoadWebControlPanelConfig(ConsoleSystem.Arg arg)
	{
		WebControlPanel.LoadConfig();
	}

	[ConsoleCommand("webpanel.savecfg", "Saves the Carbon WebControlPanel configuration")]
	[AuthLevel(2)]
	private void SaveWebControlPanelConfig(ConsoleSystem.Arg arg)
	{
		WebControlPanel.SaveConfig();
	}

	[CommandVar("webpanel.isConnected", "Is the WebControlPanel connected")]
	[AuthLevel(2)]
	private bool IsWebControlPanelConnected => WebControlPanel.server?.IsConnected() ?? false;
}
