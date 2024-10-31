using Carbon.Client.SDK;

namespace Carbon.Core;

public partial class CorePlugin
{
	private void IOnCarbonClientReady(ICarbonConnection client)
	{
		if (!Community.Runtime.ClientConfig.Enabled)
		{
			return;
		}

		Logger.Log($"{client.Username}[{client.UserId}] is ready");

		var connection = Network.Net.sv.FindConnection(client.Connection);
		Community.Runtime.CarbonClient.SendRequestToPlayer(connection);
	}

	private void OnClientAddonsDownload(ICarbonConnection client)
	{
		if (!Community.Runtime.ClientConfig.Enabled)
		{
			return;
		}

		Logger.Log($"{client.Username}[{client.UserId}] is downloading addons");
		client.IsDownloadingAddons = true;
	}

	private void OnClientAddonsFinalized(ICarbonConnection client)
	{
		if (!Community.Runtime.ClientConfig.Enabled)
		{
			return;
		}

		Logger.Log($"{client.Username}[{client.UserId}] finished downloading addons");
		client.IsDownloadingAddons = false;
	}
}
