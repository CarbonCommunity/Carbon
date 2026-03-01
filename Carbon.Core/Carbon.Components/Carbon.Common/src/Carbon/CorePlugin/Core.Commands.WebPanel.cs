namespace Carbon.Core;

public partial class CorePlugin
{
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

	[ConsoleCommand("webpanel.setenabled", "Should the WebControlPanel server be started/stopped")]
	[AuthLevel(2)]
	private void TryToggleWebControlPanelServer(ConsoleSystem.Arg arg)
	{
		WebControlPanel.config.Enabled = arg.GetBool(0, false);
		WebControlPanel.SaveConfig();
		WebControlPanel.RestartServer();
	}

	[CommandVar("webpanel.connected", "Is the WebControlPanel server connected")]
	[AuthLevel(2)]
	private bool IsWebControlPanelServerConnected => WebControlPanel.server?.IsConnected() ?? false;

	[ConsoleCommand("webpanel.clients", "Print all WebControlPanel clients")]
	[AuthLevel(2)]
	private void GetWebControlPanelClients(ConsoleSystem.Arg arg)
	{
		if (!IsWebControlPanelServerConnected)
		{
			arg.ReplyWith("The WebControlPanel server isn't connected");
			return;
		}

		using var table = new StringTable("id", "address", "account");
		foreach (var connection in WebControlPanel.server.Connections)
		{
			table.AddRow($"{connection.Key}", $"{connection.Value.Socket.ConnectionInfo.ClientIpAddress}:{connection.Value.Socket.ConnectionInfo.ClientPort}", connection.Value.Reference is not WebControlPanel.Account account ? "N/A" : account.Name);
		}
		arg.ReplyWith(table.ToStringMinimal());
	}
}
