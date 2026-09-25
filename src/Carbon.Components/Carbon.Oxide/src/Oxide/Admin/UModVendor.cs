#if !MINIMAL

using System.Net;
using Facepunch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using static Carbon.Modules.AdminModule;
using static Carbon.Modules.AdminModule.PluginsTab;
using Exception = System.Exception;
using Plugin = Carbon.Modules.AdminModule.PluginsTab.Plugin;

namespace Oxide.Game.Rust;

/// <summary>
/// umod.org plugin browser for the admin panel's Plugins tab. Plugs into the external vendor slot.
/// </summary>
[ProtoContract]
public class UModVendor : Vendor, IVendorStored
{
	public override string Type => "uMod";
	public override string StorageId => "umod";
	public override string Url => "https://umod.org";
	public override string Logo => "umodlogo";
	public override float LogoRatio => 0.2f;
	public override string Hero => "umod_hero";
	public override string Tagline => "A large platform for free plugins curated by the Oxide team.";

	public override string BarInfo => $"{FetchedPlugins.Count:n0} free";

	public override string ListEndpoint => "https://umod.org/plugins/search.json?page=[ID]&sort=title&sortdir=asc&categories%5B0%5D=universal&categories%5B1%5D=rust";
	public override string DownloadEndpoint => "https://umod.org/plugins/[ID].cs";
	public override string PluginLookupEndpoint => "https://umod.org/plugins/[ID]/latest.json";

	public WebRequests.WebRequest FetchingRequest;
	public WebRequests.WebRequest FetchingPageRequest;
	public Timer FetchingTimer;

	public void Dispose()
	{
		FetchingRequest?.Dispose();
		FetchingPageRequest?.Dispose();
		FetchingTimer?.Destroy();
		FetchingRequest = null;
		FetchingPageRequest = null;
		FetchingTimer = null;
	}

	public override void Refresh()
	{
		if (FetchedPlugins == null) return;

		var plugins = Facepunch.Pool.Get<List<Carbon.Plugins.Plugin>>();
		ModLoader.Packages.GetAllHookables(plugins);

		foreach (var plugin in FetchedPlugins)
		{
			var fileName = Path.GetFileName(plugin.File);
			var fileNameNoExtension = Path.GetFileNameWithoutExtension(plugin.File);

			foreach (var existentPlugin in plugins)
			{
				if ((!string.IsNullOrEmpty(existentPlugin.FileName) &&
				     (existentPlugin.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase) ||
				      existentPlugin.FileName.Equals(fileNameNoExtension, StringComparison.OrdinalIgnoreCase))) ||
				    (!string.IsNullOrEmpty(existentPlugin.Name) && !string.IsNullOrEmpty(plugin.Name) &&
				     existentPlugin.Name.Equals(plugin.Name, StringComparison.OrdinalIgnoreCase)))
				{
					plugin.SetExistentPlugin(existentPlugin);
					break;
				}
			}
		}

		Facepunch.Pool.FreeUnmanaged(ref plugins);

		PriceData = FetchedPlugins.OrderBy(x => x.OriginalPrice);
		AuthorData = FetchedPlugins.OrderBy(x => x.Author);
		InstalledData = FetchedPlugins.Where(x => x.IsInstalled());
		OutOfDateData = FetchedPlugins.Where(x => x.IsInstalled() && !x.IsUpToDate());
		OwnedData = FetchedPlugins.Where(x => x.Owned);
	}
	public override void FetchList(Action<Vendor> callback = null)
	{
		FetchedPlugins.Clear();

		Logger.Log($"[{Type}] Caching plugin metadata for displaying plugins in the Admin module -> Plugins tab. This might take a while..");

		FetchingRequest = Community.Runtime.Core.webrequest.Enqueue(ListEndpoint.Replace("[ID]", "0"), null, (error, data) =>
		{
			if(error != 200)
			{
				Logger.Error($"[{Type}] Failed fetching vendor. Error code {error}!");
				return;
			}

			var list = JObject.Parse(data);

			var totalPages = list["last_page"]?.ToString().ToInt();

			if (totalPages == 0)
			{
				Logger.Warn($"[{Type}] Endpoint seems to be down. Will retry gathering plugin metadata again later...");
				list = null;
				return;
			}

			FetchPage(0, totalPages.GetValueOrDefault(), callback);
			list = null;
		}, Community.Runtime.Core);
	}
	public override void Download(string id, Action onTimeout = null)
	{
		var plugin = FetchedPlugins.FirstOrDefault(x => x.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase) ||
		                                                x.Name.Equals(id, StringComparison.CurrentCultureIgnoreCase) ||
		                                                Path.GetFileNameWithoutExtension(x.File).Equals(id, StringComparison.CurrentCultureIgnoreCase));
		var path = plugin.ExistentPlugin == null ? Path.Combine(Defines.GetScriptsFolder(), plugin.File) : plugin.ExistentPlugin.FilePath;
		var url = DownloadEndpoint.Replace("[ID]", plugin.Name);

		plugin.IsBusy = true;

		Community.Runtime.Core.timer.In(2f, () =>
		{
			if (plugin.IsBusy)
			{
				plugin.IsBusy = false;
				onTimeout?.Invoke();
			}
		});

		Community.Runtime.Core.webrequest.Enqueue(url, null, (error, source) =>
		{
			if (error != 200)
			{
				Logger.Error($"[{Type}] Failed downloading item '{plugin.Name} by {plugin.Author}'. Error code {error}!");
				return;
			}

			Singleton.Puts($"Downloaded {plugin.Name}");
			OsEx.File.Move(path, Path.Combine(Defines.GetScriptsFolder(), "backups", plugin.File));
			OsEx.File.Create(path, source);

			plugin.IsBusy = false;
			plugin.DownloadCount++;

		}, Community.Runtime.Core, headers: new Dictionary<string, string>
		{
			["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.63",
			["accept"] = "ext/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
		});
	}
	public override void Uninstall(string id)
	{
		var plugin = FetchedPlugins.FirstOrDefault(x => x.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase) ||
		                                                x.Name.Equals(id, StringComparison.CurrentCultureIgnoreCase) ||
		                                                Path.GetFileNameWithoutExtension(x.File).Equals(id, StringComparison.CurrentCultureIgnoreCase));
		ModLoader.UninitializePlugin(plugin.ExistentPlugin);
		OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Defines.GetScriptsFolder(), "backups", plugin.ExistentPlugin.FileName), true);
		plugin.ExistentPlugin = null;
	}
	public override void CheckMetadata(string id, Action onMetadataRetrieved)
	{
		var plugin = FetchedPlugins.FirstOrDefault(x => x.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase) ||
		                                                x.Name.Equals(id, StringComparison.CurrentCultureIgnoreCase) ||
		                                                Path.GetFileNameWithoutExtension(x.File).Equals(id, StringComparison.CurrentCultureIgnoreCase));
		if (plugin.HasLookup) return;

		Community.Runtime.Core.webrequest.Enqueue(PluginLookupEndpoint.Replace("[ID]", plugin.Name.ToLower().Trim()), null, (error, data) =>
		{
			if (error != 200)
			{
				Logger.Error($"[{Type}] Failed fetching item metadata for '{plugin.Name} by {plugin.Author}'. Error code {error}!");
				return;
			}

			var list = JObject.Parse(data);
			var description = list["description_md"]?.ToString();

			plugin.Changelog = description
				.Replace("<div>", "").Replace("</div>", "")
				.Replace("\\n", "")
				.Replace("<br />", "\n")
				.Replace("<pre>", "")
				.Replace("<p>", "")
				.Replace("</p>", "")
				.Replace("<span class=\"documentation\">", "")
				.Replace("</span>", "")
				.Replace("<code>", "<b>")
				.Replace("</code>", "</b>")
				.Replace("<ul>", "").Replace("</ul>", "")
				.Replace("<li>", "").Replace("</li>", "")
				.Replace("<em>", "").Replace("</em>", "")
				.Replace("<h1>", "<b>").Replace("</h1>", "</b>")
				.Replace("<h2>", "<b>").Replace("</h2>", "</b>")
				.Replace("<h3>", "<b>").Replace("</h3>", "</b>")
				.Replace("<h4>", "<b>").Replace("</h4>", "</b>")
				.Replace("<strong>", "<b>").Replace("</strong>", "</b>");

			if (!string.IsNullOrEmpty(plugin.Changelog) && !plugin.Changelog.EndsWith(".")) plugin.Changelog = plugin.Changelog.Trim() + ".";

			plugin.HasLookup = true;
			onMetadataRetrieved?.Invoke();
		}, Community.Runtime.Core);
	}

	public void FetchPage(int page, int maxPage, Action<Vendor> callback = null)
	{
		if (page > maxPage)
		{
			Save();
			callback?.Invoke(this);
			return;
		}

		FetchingPageRequest = Community.Runtime.Core.webrequest.Enqueue(ListEndpoint.Replace("[ID]", $"{page}"), null, (error, data) =>
		{
			if (error != 200)
			{
				Logger.Error($"[{Type}] Failed fetching page for vendor. Error code {error}!");
				return;
			}

			var list = JObject.Parse(data);
			var file = list["data"];
			var plugins = Facepunch.Pool.Get<List<Carbon.Plugins.Plugin>>();
			ModLoader.Packages.GetAllHookables(plugins);
			foreach (var plugin in file)
			{
				var image = plugin["icon_url"]?.ToString();
				var p = new Plugin
				{
					Id = plugin["url"]?.ToString(),
					Name = plugin["name"]?.ToString(),
					Author = plugin["author"]?.ToString(),
					Version = plugin["latest_release_version"]?.ToString(),
					Description = plugin["description"]?.ToString(),
					OriginalPrice = "FREE",
					File = $"{plugin["name"]?.ToString()}.cs",
					Image = image,
					ImageThumbnail = image,
					ImageSize = 0,
					DownloadCount = (plugin["downloads"]?.ToString().ToInt()).GetValueOrDefault(),
					Date = plugin["published_at"]?.ToString(),
					UpdateDate = plugin["updated_at"]?.ToString(),
					Tags = plugin["tags_all"]?.ToString().Split(','),
					Rating = -1
				};
				p.PreferredVendor = VendorTypes.External;

				if (!string.IsNullOrEmpty(p.Description) && !p.Description.EndsWith(".")) p.Description += ".";

				if (string.IsNullOrEmpty(p.Author.Trim())) p.Author = "Unmaintained";
				if (p.OriginalPrice == "{}") p.OriginalPrice = "FREE";
				try { p.ExistentPlugin = plugins.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.FilePath) == Path.GetFileNameWithoutExtension(p.File)) as Carbon.Plugins.Plugin; } catch { }

				if (!FetchedPlugins.Any(x => x.Name == p.Name)) FetchedPlugins.Add(p);
			}
			Facepunch.Pool.FreeUnmanaged(ref plugins);

			if (page % (maxPage / 4) == 0 || page == maxPage - 1)
			{
				Logger.Log($"Caching plugin metadata page {page} out of {maxPage}");
			}
		}, Community.Runtime.Core);
		FetchingTimer = Community.Runtime.Core.timer.In(5f, () => FetchPage(page + 1, maxPage, callback));
	}

	public bool Load()
	{
		try
		{
			var path = Path.Combine(Defines.GetDataFolder(), "vendordata_umod.db");
			if (!OsEx.File.Exists(path)) return false;

			using var file = new MemoryStream(OsEx.File.ReadBytes(path));
			var value = Serializer.Deserialize<UModVendor>(file);

			LastTick = value.LastTick;
			FetchedPlugins.Clear();
			FetchedPlugins.AddRange(value.FetchedPlugins);

			if ((DateTime.Now - new DateTime(value.LastTick)).TotalHours >= 24)
			{
				Singleton.Puts($"Invalidated {Type} database. Fetching...");
				return false;
			}

			Singleton.Puts($"Loaded {Type} plugin metadata cache from file.");
			Refresh();
		}
		catch
		{
			return false;
		}

		return true;
	}
	public void Save()
	{
		try
		{
			var path = Path.Combine(Defines.GetDataFolder(), "vendordata_umod.db");
			using var file = new MemoryStream();

			LastTick = DateTime.Now.Ticks;
			Serializer.Serialize(file, this);
			OsEx.File.Create(path, file.ToArray());
			Singleton.Puts($"Stored {Type} plugin metadata cache to file.");
		}
		catch (Exception ex)
		{
			Singleton.PutsError($" Couldn't store uMod plugins list.", ex);
		}
	}
}

#endif
