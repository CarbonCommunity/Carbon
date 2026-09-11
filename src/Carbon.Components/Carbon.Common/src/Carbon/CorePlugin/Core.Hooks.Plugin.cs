using Rust.Ai.Gen2;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	private void OnPluginLoaded(RustPlugin plugin)
	{
		WebControlPanel.SendPluginsToAllConnections();
	}

	private void OnPluginUnloaded(RustPlugin plugin)
	{
		WebControlPanel.SendPluginsToAllConnections();
	}
}
