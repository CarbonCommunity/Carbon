public delegate void PluginEvent(Plugin plugin);

public class PluginManager
{
	public string ConfigPath => Defines.GetConfigsFolder();

	public event PluginEvent OnPluginAdded;
	public event PluginEvent OnPluginRemoved;

	public bool AddPlugin(RustPlugin plugin)
	{
		OnPluginAdded?.Invoke(plugin);

		var pkg = plugin.Package;
		if (!pkg.IsValid || pkg.Plugins == null || pkg.Plugins.Contains(plugin)) return false;

		pkg.AddPlugin(plugin);
		return true;
	}
	public bool RemovePlugin(RustPlugin plugin)
	{
		OnPluginRemoved?.Invoke(plugin);

		var pkg = plugin.Package;
		if (!pkg.IsValid || pkg.Plugins == null || !pkg.Plugins.Contains(plugin)) return false;

		pkg.RemovePlugin(plugin);
		return true;
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
