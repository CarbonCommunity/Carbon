namespace Oxide.Core.Plugins;

public class PluginLoader
{
	public virtual Type[] CorePlugins { get; } = [];

	public virtual IEnumerable<string> ScanDirectory(string directory)
	{
		foreach (var plugin in CorePlugin.ProcessableFiles)
		{
			if (!plugin.Path.Contains(directory, CompareOptions.OrdinalIgnoreCase))
			{
				continue;
			}

			yield return plugin.Path;
		}
	}
}
