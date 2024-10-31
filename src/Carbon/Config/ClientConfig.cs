using Newtonsoft.Json;

namespace Carbon.Client;

[Serializable]
public class ClientConfig
{
	public bool Enabled = false;
	public EnvironmentOptions Environment = new();
	public List<AddonEntry> Addons = [];

	[JsonIgnore] public string[] AddonCache;

	public void RefreshAddonCache()
	{
		if (AddonCache != null)
		{
			Array.Clear(AddonCache, 0, AddonCache.Length);
		}

		AddonCache = Community.Runtime.ClientConfig.Addons.Where(x => x.Enabled).Select(x => x.Url).ToArray();
	}

	[Serializable]
	public class AddonEntry
	{
		public string Url;
		public bool Enabled = true;
	}

	[Serializable]
	public class EnvironmentOptions
	{
		public bool NoMap = false;
	}
}
