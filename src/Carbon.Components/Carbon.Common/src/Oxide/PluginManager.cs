public delegate void PluginEvent(Plugin plugin);

public class PluginManager
{
	public string ConfigPath => Defines.GetConfigsFolder();

	public event PluginEvent OnPluginAdded;
	public event PluginEvent OnPluginRemoved;

	public bool AddPlugin(RustPlugin plugin)
	{
		OnPluginAdded?.Invoke(plugin);

		if (plugin.Package.IsValid && !plugin.Package.Plugins.Contains(plugin))
		{
			plugin.Package.Plugins.Add(plugin);
			return true;
		}

		return false;
	}
	public bool RemovePlugin(RustPlugin plugin)
	{
		OnPluginRemoved?.Invoke(plugin);
		return plugin.Package.Plugins?.RemoveAll(x => x == plugin) > 0;
	}

	public Plugin GetPlugin(string name)
	{
		if (name == "RustCore") return Community.Runtime.Core;

		return Community.Runtime.Plugins.FindPlugin(name);
	}
	public IEnumerable<Plugin> GetPlugins()
	{
		return Community.Runtime.Plugins.Plugins.AsEnumerable();
	}
}
