namespace Oxide.Core.Extensions;

public class ExtensionManager
{
	internal static List<Extension> extensionCache = new();

	private List<PluginLoader> pluginloaders = new();

	public IEnumerable<PluginLoader> GetPluginLoaders() => pluginloaders;
	public void RegisterPluginLoader(Oxide.Core.Plugins.PluginLoader loader)
	{
		pluginloaders.Add(loader);
	}
}
