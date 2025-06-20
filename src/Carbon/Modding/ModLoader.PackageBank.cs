namespace Carbon.Core;

public static partial class ModLoader
{
	public class PackageBank : List<Package>
	{
		public Package FindPackage(string name)
		{
			return this.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCulture));
		}

		public RustPlugin FindPlugin(string name)
		{
			return this.Select(package => package.FindPlugin(name)).FirstOrDefault(plugin => plugin != null);
		}

		public void GetAllHookables(List<RustPlugin> plugins, bool ignoreCore = false)
		{
			foreach (var hookable in this)
			{
				foreach (var plugin in hookable.Plugins)
				{
					if (plugin.IsCorePlugin && ignoreCore)
					{
						continue;
					}
					plugins.Add(plugin);
				}
			}
		}
	}
}
