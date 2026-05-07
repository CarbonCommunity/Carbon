namespace Carbon.Core;

public static partial class ModLoader
{
	public class PackageBank : List<Package>
	{
		public Package FindPackage(string name)
		{
			return this.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCulture));
		}

		public int FindPackageIndex(string name)
		{
			return this.FindIndex(x => x.Name.Equals(name, StringComparison.InvariantCulture));
		}

		public bool RemovePackage(string name)
		{
			var index = FindPackageIndex(name);
			if (index == -1)
			{
				return false;
			}
			RemoveAt(index);
			return true;
		}

		public RustPlugin FindPlugin(string name)
		{
			if (string.IsNullOrEmpty(name)) return null;
			for (var i = 0; i < Count; i++)
			{
				var plugin = this[i].FindPlugin(name);
				if (plugin != null) return plugin;
			}
			return null;
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
