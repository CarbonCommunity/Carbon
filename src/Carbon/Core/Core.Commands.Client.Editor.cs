using System.Text;
using Carbon.Client;
using Facepunch;

namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("client.editor.addon_live_update", "Carbon Editor requested RCon command for loading an addon.")]
	[AuthLevel(2)]
	private void ClientEditorAddonLiveUpdate(ConsoleSystem.Arg args)
	{
		Community.Runtime.ClientConfig.Addons.Clear();

		Community.Runtime.ClientConfig.Addons.Add(new ClientConfig.AddonEntry
		{
			Url = args.GetString(0),
			Enabled = true
		});

		Logger.Log($" C4C Editor: Received request to load addon from '{args.GetString(0)}'");

		Community.Runtime.ClientConfig.RefreshAddonCache();

		ReloadCarbonClientAddons(true);
	}
}
