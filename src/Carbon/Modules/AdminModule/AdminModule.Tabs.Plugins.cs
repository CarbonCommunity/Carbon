#if !MINIMAL

using System.IO.Compression;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Game.Rust.Cui;
using ProtoBuf;
using static ConsoleSystem;
using Timer = Oxide.Plugins.Timer;

namespace Carbon.Modules;

public partial class AdminModule
{
	public static class PluginsTab
	{
		public enum VendorTypes
		{
			Installed,
			Codefling,
			uMod,
		}
		public enum FilterTypes
		{
			None,
			Price,
			Author,
			Installed,
			OutOfDate,
			Favourites,
			Owned
		}

		public static bool DropdownShow;
		public static string[] DropdownOptions { get; } =
		[
			"A-Z",
			"Price",
			"Author",
			"Installed",
			"Pending Update",
			"Favourites",
			"Owned"
		];
		public static PlayerSession.Page PlaceboPage { get; } = new PlayerSession.Page();
		public static List<string> TagFilter = new();
		public static string[] PopularTags { get; } =
		[
			"gui",
			"admin",
			"moderation",
			"chat",
			"building",
			"discord",
			"libraries",
			"loot",
			"pve",
			"event",
			"logging",
			"anti-cheat",
			"economics",
			"npc",
			"info",
			"limitations",
			"statistics",
			"monuments",
			"seasonal",
			"banan",
			"peanus"
		];

		public static Vendor CodeflingInstance;
		public static Vendor uModInstance;
		public static Vendor LocalInstance;

		public static Vendor GetVendor(VendorTypes vendor)
		{
			switch (vendor)
			{
				case VendorTypes.Codefling:
					return CodeflingInstance;

				case VendorTypes.uMod:
					return uModInstance;

				case VendorTypes.Installed:
					return LocalInstance;
			}

			return default;
		}

		public static Tab Get()
		{
			OsEx.Folder.Create(Path.Combine(Defines.GetScriptsFolder(), "backups"));

			var tab = new Tab("plugins", "Plugins", Community.Runtime.Core, (ap, t) =>
			{
				ap.SetStorage(t, "selectedplugin", (Plugin)default);
				LocalInstance?.Refresh();
			}, "plugins.use")
			{
				Override = (t, cui, container, parent, ap) => Draw(cui, container, parent, t, ap)
			};

			CodeflingInstance = new Codefling();
			if (CodeflingInstance is IVendorStored cfStored && !cfStored.Load())
			{
				CodeflingInstance.FetchList((vendor) =>
				{
					CodeflingInstance.Refresh();
					CodeflingInstance.VersionCheck();
				});
			}

			uModInstance = new uMod();
			if (uModInstance is IVendorStored umodStored && !umodStored.Load())
			{
				uModInstance.FetchList((vendor) =>
				{
					uModInstance.Refresh();
					uModInstance.VersionCheck();
				});
			}

			LocalInstance = new Installed();
			LocalInstance.Refresh();

			ServerOwner.Load();

			return tab;
		}

		public static List<Plugin> GetPlugins(Vendor vendor, Tab tab, PlayerSession ap, int pluginCount)
		{
			return GetPlugins(vendor, tab, ap, out _, pluginCount);
		}
		public static List<Plugin> GetPlugins(Vendor vendor, Tab tab, PlayerSession ap, out int maxPages, int pluginCount)
		{
			maxPages = 0;

			var resultList = Facepunch.Pool.Get<List<Plugin>>();
			var customList = Facepunch.Pool.Get<List<Plugin>>();

			using (TimeMeasure.New("GetPluginsFromVendor"))
			{
				try
				{
					var filter = ap.GetStorage(tab, "filter", FilterTypes.None);
					var flip = ap.GetStorage(tab, "flipfilter", false);

					IEnumerable<Plugin> plugins = filter switch
					{
						FilterTypes.Price => flip ? vendor.PriceData.Reverse() : vendor.PriceData,
						FilterTypes.Author => flip ? vendor.AuthorData.Reverse() : vendor.AuthorData,
						FilterTypes.Installed => flip ? vendor.InstalledData.Reverse() : vendor.InstalledData,
						FilterTypes.OutOfDate => flip ? vendor.OutOfDateData.Reverse() : vendor.OutOfDateData,
						FilterTypes.Owned => flip ? vendor.OwnedData.Reverse() : vendor.OwnedData,
						_ => flip ? vendor.FetchedPlugins.AsEnumerable().Reverse() : vendor.FetchedPlugins,
					};

					var search = ap.GetStorage<string>(tab, "search");
					if (!string.IsNullOrEmpty(search))
					{
						foreach (var plugin in plugins)
						{
							if (plugin.Status != Status.Approved ||
								(plugin.ExistentPlugin != null && plugin.ExistentPlugin.IsPrecompiled)) continue;

							if (filter == FilterTypes.Favourites)
							{
								if (ServerOwner.Singleton.FavouritePlugins.Contains(plugin.Name))
								{
									customList.Add(plugin);
									continue;
								}

								continue;
							}

							if (plugin.Id == search)
							{
								customList.Add(plugin);
								continue;
							}

							if (plugin.Name.ToLower().Trim().Contains(search.ToLower().Trim()))
							{
								customList.Add(plugin);
								continue;
							}

							if (plugin.Author.ToLower().Trim().Contains(search.ToLower().Trim()))
							{
								customList.Add(plugin);
								continue;
							}

							if (TagFilter.Count > 0 && plugin.Tags != null)
							{
								var hasTag = false;
								foreach (var tag in plugin.Tags)
								{
									if (TagFilter.Contains(tag))
									{
										hasTag = true;
										break;
									}
								}

								if (hasTag)
								{
									customList.Add(plugin);
									continue;
								}
							}
						}
					}
					else
					{
						foreach (var plugin in plugins)
						{
							if (plugin.Status != Status.Approved ||
								(plugin.ExistentPlugin != null && plugin.ExistentPlugin.IsPrecompiled)) continue;

							if (filter == FilterTypes.Favourites)
							{
								if (ServerOwner.Singleton.FavouritePlugins.Contains(plugin.Name))
								{
									customList.Add(plugin);
									continue;
								}

								continue;
							}

							if (TagFilter.Count > 0 && plugin.Tags != null)
							{
								var hasTag = false;
								foreach (var tag in plugin.Tags)
								{
									if (TagFilter.Contains(tag))
									{
										hasTag = true;
										break;
									}
								}

								if (hasTag)
								{
									customList.Add(plugin);
									continue;
								}
							}
							else customList.Add(plugin);
						}
					}

					plugins = null;

					maxPages = (customList.Count - 1) / pluginCount;

					var page2 = ap.GetStorage(tab, "page", 0);
					if (page2 > maxPages) ap.SetStorage(tab, "page", maxPages);

					var page = pluginCount * page2;
					var count = (page + pluginCount).Clamp(0, customList.Count);

					if (count > 0)
					{
						for (int i = page; i < count; i++)
						{
							try { resultList.Add(customList[i]); }
							catch { break; }
						}
					}
				}
				catch (Exception ex)
				{
					Facepunch.Pool.FreeUnmanaged(ref resultList);

					Logger.Error($"Failed getting plugins.", ex);
				}

				Facepunch.Pool.FreeUnmanaged(ref customList);
			}

			return resultList;
		}

		public static void DownloadThumbnails(Vendor vendor, Tab tab, PlayerSession ap)
		{
			var plugins = GetPlugins(vendor, tab, ap, 15);

			var images = Facepunch.Pool.Get<List<string>>();
			var imagesSafe = Facepunch.Pool.Get<List<string>>();

			foreach (var element in plugins)
			{
				if (element.HasNoImage()) continue;

				if (element.HasInvalidImage())
				{
					imagesSafe.Add(element.Image);
					continue;
				}

				images.Add(element.Image);
			}

			var eraseAllBeforehand = false;

			if (images.Count > 0) Singleton.ImageDatabase.QueueBatch(eraseAllBeforehand, images);
			if (imagesSafe.Count > 0) Singleton.ImageDatabase.QueueBatch(eraseAllBeforehand, imagesSafe);

			Facepunch.Pool.FreeUnmanaged(ref plugins);
			Facepunch.Pool.FreeUnmanaged(ref images);
			Facepunch.Pool.FreeUnmanaged(ref imagesSafe);
		}
		public static void Draw(CUI cui, CuiElementContainer container, string parent, Tab tab, PlayerSession ap)
		{
			ap.SetDefaultStorage(tab, "vendor", VendorTypes.Installed);

			var header = cui.CreatePanel(container, parent, "0.2 0.2 0.2 0.5",
				xMin: 0f, xMax: 1f, yMin: 0.95f, yMax: 1f);

			var vendorType = ap.GetStorage(tab, "vendor", VendorTypes.Installed);
			var vendor = GetVendor(vendorType);

			var vendors = Enum.GetNames(typeof(VendorTypes));
			var cuts = 1f / vendors.Length;
			var offset = 0f;
			foreach (var value in vendors)
			{
				var vType = (VendorTypes)Enum.Parse(typeof(VendorTypes), value);
				var v = GetVendor(vType);
				cui.CreateProtectedButton(container, header, vendorType == vType ? "0.3 0.72 0.25 0.8" : "0.2 0.2 0.2 0.3", "1 1 1 0.7", $"<b>{value.Replace("_", ".")}</b>{(v == null ? "" : $" — {v?.BarInfo}")}", 11,
					xMin: offset, xMax: offset + cuts, command: $"pluginbrowser.changetab {value}");
				offset += cuts;
			}

			var grid = cui.CreatePanel(container, parent, "0.2 0.2 0.2 0.3",
				xMin: 0f, xMax: 0.8f, yMin: 0, yMax: 0.95f);

			var spacing = 0.015f;
			var columnSize = 0.195f - spacing;
			var rowSize = 0.3f - spacing;
			var column = 0.02f;
			var row = 0f;
			var yOffset = 0.05f;

			var plugins = GetPlugins(vendor, tab, ap, out var maxPages, 15);

			for (int i = 0; i < 15; i++)
			{
				if (i > plugins.Count() - 1) continue;

				var plugin = plugins.ElementAt(i);

				var card = cui.CreatePanel(container, grid, "0.2 0.2 0.2 0.4",
					xMin: column, xMax: column + columnSize, yMin: 0.69f + row - yOffset, yMax: 0.97f + row - yOffset);

				if (plugin.Owned)
				{
					cui.CreateImage(container, card, "glow", "1 1 1 0.5", OxMin: -20, OxMax: 20, OyMin: -20, OyMax: 20);
				}

				if (plugin.HasNoImage() || Singleton.DataInstance.HidePluginIcons)
				{
					cui.CreatePanel(container, card, "0.2 0.2 0.2 0.5");
					cui.CreateImage(container, card, vendor.Logo, "0.2 0.2 0.2 0.85", xMin: 0.2f, xMax: 0.8f, yMin: 0.2f + vendor.LogoRatio, yMax: 0.8f - vendor.LogoRatio);
				}
				else
				{
					if (Singleton.ImageDatabase.HasImage(plugin.ImageThumbnail))
					{
						cui.CreateImage(container, card, plugin.ImageThumbnail, "1 1 1 1");
					}
					else
					{
						cui.CreateClientImage(container, card, plugin.ImageThumbnail, "1 1 1 1");
					}
				}

				var cardTitle = cui.CreatePanel(container, card, "0 0 0 0.9", yMax: 0.25f);
				cui.CreatePanel(container, cardTitle, "0 0 0 0.2", blur: true);

				cui.CreateText(container, cardTitle, "1 1 1 1", plugin.Name, 11, xMin: 0.05f, yMax: 0.87f, align: TextAnchor.UpperLeft);
				cui.CreateText(container, cardTitle, "0.6 0.6 0.3 0.8", $"by <b>{(plugin.ExistentPlugin != null ? plugin.ExistentPlugin.Author : plugin.Author)}</b>", 9, xMin: 0.05f, yMin: 0.15f, align: TextAnchor.LowerLeft);
				cui.CreateText(container, cardTitle, "0.6 0.75 0.3 0.8", plugin.IsPaid() ? $"<b>${plugin.OriginalPrice}</b>" : "<b>FREE</b>", 11, xMax: 0.95f, yMin: 0.1f, align: TextAnchor.LowerRight);

				var shadowShift = -0.003f;
				cui.CreateText(container, card, "0 0 0 0.9", $"v{plugin.Version}", 9, xMax: 0.97f, yMax: 0.95f, OyMin: shadowShift, OxMin: shadowShift, OyMax: shadowShift, OxMax: shadowShift, align: TextAnchor.UpperRight);
				cui.CreateText(container, card, "0.6 0.75 0.3 1", $"v{plugin.Version}", 9, xMax: 0.97f, yMax: 0.95f, align: TextAnchor.UpperRight);

				var shadowShift2 = 0.1f;
				if (plugin.IsInstalled())
				{
					cui.CreateImage(container, card, "installed", "0 0 0 0.9", xMin: 0.04f, xMax: 0.16f, yMin: 0.83f, yMax: 0.95f, OyMin: shadowShift2, OxMin: -shadowShift2, OyMax: shadowShift2, OxMax: -shadowShift2);
					cui.CreateImage(container, card, "installed", plugin.IsUpToDate() ? "0.6 0.75 0.3 1" : "0.85 0.4 0.3 1", xMin: 0.04f, xMax: 0.16f, yMin: 0.83f, yMax: 0.95f);
				}

				cui.CreateProtectedButton(container, card, "0 0 0 0", "0 0 0 0", string.Empty, 0, command: $"pluginbrowser.selectplugin {plugin.Id}");

				var favouriteButton = cui.CreateProtectedButton(container, card, "0 0 0 0", "0 0 0 0", string.Empty, 0, xMin: 0.84f, xMax: 0.97f, yMin: 0.73f, yMax: 0.86f, command: $"pluginbrowser.interact 10 {plugin.Name}");
				cui.CreateImage(container, favouriteButton, "star", ServerOwner.Singleton.FavouritePlugins.Contains(plugin.Name) ? "0.9 0.8 0.4 0.95" : "0.2 0.2 0.2 0.4");

				var autoUpdateButton = cui.CreateProtectedButton(container, card, "0 0 0 0", "0 0 0 0", string.Empty, 0, xMin: 0.84f, xMax: 0.97f, yMin: 0.59f, yMax: 0.72f, command: $"pluginbrowser.interact 11 {plugin.Name}");
				cui.CreateImage(container, autoUpdateButton, "update-pending", ServerOwner.Singleton.AutoUpdate.Contains(plugin.Name) ? "0.8 0.4 0.9 0.95" : "0.2 0.2 0.2 0.4");

				column += columnSize + spacing;

				if (i % 5 == 4)
				{
					row -= rowSize + spacing;
					column = 0.02f;
				}
			}

			if (plugins.Count == 0)
			{
				cui.CreateText(container, parent, "1 1 1 0.5", "No plugins found for that query", 10, align: TextAnchor.MiddleCenter, yMax: 0.95f);
			}

			var sidebar = cui.CreatePanel(container, parent, "0.2 0.2 0.2 0.3",
				xMin: 0.80f, xMax: 1f, yMax: 0.93f);

			var topbar = cui.CreatePanel(container, parent, "0.1 0.1 0.1 0.7",
				xMin: 0f, xMax: 0.8f, yMin: 0.89f, yMax: 0.94f);

			var drop = cui.CreatePanel(container, sidebar, "0 0 0 0", yMin: 0.95f, OxMin: -155);
			Singleton.TabPanelDropdown(cui, PlaceboPage, container, drop, null, $"pluginbrowser.changesetting filter_dd", 1, 0, (int)ap.GetStorage(tab, "filter", FilterTypes.None), DropdownOptions, null, DropdownShow);

			const float topbarYScale = 0.1f;
			cui.CreateText(container, topbar, "1 1 1 1", plugins.Count > 0 ? $"/ {maxPages + 1:n0}" : "NONE", plugins.Count > 0 ? 10 : 8, xMin: plugins.Count > 0 ? 0.925f : 0.92f, xMax: 0.996f, align: TextAnchor.MiddleLeft);
			if (plugins.Count != 0) cui.CreateProtectedInputField(container, topbar, "1 1 1 1", $"{ap.GetStorage(tab, "page", 0) + 1}", 10, 3, false, xMin: 0.8f, xMax: 0.92f, align: TextAnchor.MiddleRight, command: $"pluginbrowser.page ");
			cui.CreateProtectedButton(container, topbar, "0.4 0.7 0.3 0.8", "1 1 1 0.6", "<", 10, xMin: 0.86f, xMax: 0.886f, yMin: topbarYScale, yMax: 1f - topbarYScale, command: "pluginbrowser.page -1");
			cui.CreateProtectedButton(container, topbar, "0.4 0.7 0.3 0.8", "1 1 1 0.6", ">", 10, xMin: 0.97f, xMax: 0.996f, yMin: topbarYScale, yMax: 1f - topbarYScale, command: "pluginbrowser.page +1");

			var filterSectionOffset = 0.45f;
			var filterSectionOffsetSize = 0.1f;
			var tagsOption = cui.CreatePanel(container, sidebar, "0 0 0 0", yMin: filterSectionOffset - filterSectionOffsetSize, yMax: filterSectionOffset - 0.05f);
			var rowNumber = 0;
			var count = 0;
			var countPerRow = 3;
			var buttonSize = 1f / countPerRow;
			var buttonOffset = 0f;
			var rows = 7;
			foreach (var tag in PopularTags.Take(countPerRow * rows))
			{
				count++;

				cui.CreateProtectedButton(container, tagsOption, TagFilter.Contains(tag) ? "#b08d2e" : "0.2 0.2 0.2 0.5", "1 1 1 0.6", tag.ToUpper(), 8,
					xMin: buttonOffset, xMax: buttonOffset += buttonSize,
					OyMin: rowNumber * -25, OyMax: rowNumber * -25, command: $"pluginbrowser.tagfilter {tag}");

				if (count % countPerRow == 0)
				{
					buttonOffset = 0;
					rowNumber++;
				}
			}

			var auth = vendor as IVendorAuthenticated;

			if (auth != null)
			{
				var user = cui.CreatePanel(container, topbar, "0 0 0 0", xMax: 0.275f);

				if (auth.IsLoggedIn) cui.CreateClientImage(container, user, auth.User.AvatarUrl, "1 1 1 0.8", xMin: 0.02f, xMax: 0.12f, yMin: 0.1f, yMax: 0.9f);
				cui.CreateText(container, user, "1 1 1 0.9", auth.IsLoggedIn ? auth.User.DisplayName : "Not logged-in", 10, xMin: auth.IsLoggedIn ? 0.14f : 0.025f, yMax: 0.9f, align: TextAnchor.UpperLeft);
				cui.CreateText(container, user, "1 1 0.3 0.9", auth.IsLoggedIn ? $"as {auth.User.Authority}" : "Login to explore premium plugins!", 8, xMin: auth.IsLoggedIn ? 0.14f : 0.025f, yMin: 0.1f, align: TextAnchor.LowerLeft);
				cui.CreateProtectedButton(container, user, auth.IsLoggedIn ? "0.8 0.1 0 0.8" : "0.1 0.8 0 0.8", "1 1 1 0.5", auth.IsLoggedIn ? "<b>LOGOUT</b>" : "<b>LOGIN</b>", 8, xMin: 0.75f, xMax: 0.975f, command: "pluginbrowser.login");
			}

			var isInstalled = vendor is Installed;
			var searchQuery = ap.GetStorage<string>(tab, "search");
			var search = cui.CreatePanel(container, topbar, "0 0 0 0", xMin: 0.6f, xMax: 0.855f, yMin: 0f, OyMax: -0.5f);
			cui.CreateProtectedInputField(container, search, string.IsNullOrEmpty(searchQuery) ? "0.8 0.8 0.8 0.6" : "1 1 1 1", string.IsNullOrEmpty(searchQuery) ? "Search..." : searchQuery, 10, 20, false, xMin: 0.06f, align: TextAnchor.MiddleLeft, needsKeyboard: Singleton.HandleEnableNeedsKeyboard(ap), command: "pluginbrowser.search  ");
			cui.CreateProtectedButton(container, search, string.IsNullOrEmpty(searchQuery) ? "0.2 0.2 0.2 0.8" : "#d43131", "1 1 1 0.6", "X", 10, xMin: 0.95f, yMin: topbarYScale, yMax: 1f - topbarYScale, OxMin: -30, OxMax: -22.5f, command: "pluginbrowser.search  ");

			var reloadButton = cui.CreateProtectedButton(container, search, isInstalled ? "0.2 0.2 0.2 0.4" : "0.2 0.2 0.2 0.8", "1 1 1 0.6", string.Empty, 0, xMin: 0.9f, xMax: 1, yMin: topbarYScale, yMax: 1f - topbarYScale, command: "pluginbrowser.refreshvendor");
			cui.CreateImage(container, reloadButton, "reload", "1 1 1 0.4", xMin: 0.225f, xMax: 0.775f, yMin: 0.25f, yMax: 0.75f);

			if (TagFilter.Contains("peanus")) cui.CreateClientImage(container, grid, "https://media.discordapp.net/attachments/1078801277565272104/1085062151221293066/15ox1d_1.jpg?width=827&height=675", "1 1 1 1", xMax: 0.8f);
			if (TagFilter.Contains("banan")) cui.CreateClientImage(container, grid, "https://upload.wikimedia.org/wikipedia/commons/2/23/Banan.jpg", "1 1 1 1", xMax: 0.8f);

			var selectedPlugin = ap.GetStorage<Plugin>(tab, "selectedplugin");

			if (selectedPlugin != null)
			{
				vendor.CheckMetadata(selectedPlugin.Id, () => { Singleton.Draw(ap.Player); });

				var mainPanel = cui.CreatePanel(container, parent, "0.15 0.15 0.15 0.35", blur: true);
				cui.CreatePanel(container, mainPanel, "0 0 0 0.9");

				var image = cui.CreatePanel(container, parent, "0 0 0 0.5", xMin: 0.08f, xMax: 0.45f, yMin: 0.15f, yMax: 0.85f);

				if (selectedPlugin.HasNoImage())
				{
					cui.ImageDatabase.Queue(selectedPlugin.Image);
					cui.CreateImage(container, image, vendor.Logo, "0.2 0.2 0.2 0.4", xMin: 0.2f, xMax: 0.8f, yMin: 0.2f + vendor.LogoRatio, yMax: 0.8f - vendor.LogoRatio);
				}
				else if (!cui.ImageDatabase.HasImage(selectedPlugin.Image))
				{
					cui.ImageDatabase.Queue(selectedPlugin.Image);
					cui.CreateClientImage(container, image, selectedPlugin.Image, "1 1 1 1", xMin: 0.05f, xMax: 0.95f, yMin: 0.05f, yMax: 0.95f);
				}
				else
				{
					cui.CreateImage(container, image, selectedPlugin.Image, "1 1 1 1", xMin: 0.05f, xMax: 0.95f, yMin: 0.05f, yMax: 0.95f);
				}

				var pluginName = cui.CreateText(container, mainPanel, "1 1 1 1", selectedPlugin.Name, 25, xMin: 0.505f, yMax: 0.8f, align: TextAnchor.UpperLeft, font: CUI.Handler.FontTypes.RobotoCondensedBold);
				cui.CreateText(container, mainPanel, "1 1 1 0.5", $"by <b>{(selectedPlugin.ExistentPlugin != null ? selectedPlugin.ExistentPlugin.Author : selectedPlugin.Author)}</b>  <b>•</b>  v{selectedPlugin.Version}  <b>•</b>  Updated on {selectedPlugin.UpdateDate}  <b>•</b>  {selectedPlugin.DownloadCount:n0} downloads", 11, xMin: 0.48f, yMax: 0.74f, align: TextAnchor.UpperLeft);
				cui.CreateText(container, mainPanel, "1 1 1 0.3", $"{(!selectedPlugin.HasLookup ? "Fetching metdata..." : $"{selectedPlugin.Description}\n\n{selectedPlugin.Changelog}")}", 11, xMin: 0.48f, xMax: 0.85f, yMax: 0.635f, align: TextAnchor.UpperLeft);
				const float badgeYMin = 5;
				const float badgeYMax = 20;
				var priceBadge = cui.CreatePanel(container, pluginName, "0.3 0.4 0.9 0.25", xMax: 0f, yMin: 1, OyMin: badgeYMin, OyMax: badgeYMax, OxMax: 40);
				cui.CreateText(container, priceBadge, "0.4 0.5 1 1", selectedPlugin.IsPaid() ? $"${selectedPlugin.OriginalPrice}" : "FREE", 8);

				if (selectedPlugin.Owned)
				{
					var ownedBadge = cui.CreatePanel(container, pluginName, "0.9 0.4 0.3 0.25", xMax: 0f, yMin: 1, OyMin: badgeYMin, OyMax: badgeYMax, OxMin: 42, OxMax: 90);
					cui.CreateText(container, ownedBadge, "1 0.5 0.4 1", "★ OWNED", 8);
				}

				cui.CreateProtectedButton(container, mainPanel, "0 0 0 0", "0 0 0 0", string.Empty, 0, align: TextAnchor.MiddleCenter, command: "pluginbrowser.deselectplugin");

				var favouriteButton = cui.CreateProtectedButton(container, mainPanel, "0 0 0 0", "0 0 0 0", string.Empty, 0, xMin: 0.48f, xMax: 0.495f, yMin: 0.755f, yMax: 0.785f, command: $"pluginbrowser.interact 10 {selectedPlugin.Name}");
				cui.CreateImage(container, favouriteButton, "star", ServerOwner.Singleton.FavouritePlugins.Contains(selectedPlugin.Name) ? "0.9 0.8 0.4 0.95" : "0.2 0.2 0.2 0.4");

				#region Tags

				var tagOffset = 0f;
				var tagSpacing = 0.012f;
				var tags = cui.CreatePanel(container, mainPanel, "0 0 0 0", xMin: 0.48f, xMax: 0.8f, yMin: 0.66f, yMax: 0.7f);
				var tempTags = Facepunch.Pool.Get<List<string>>();
				var counter = 0;

				if (selectedPlugin.Tags != null && selectedPlugin.Tags.Count() > 0)
				{
					foreach (var tag in selectedPlugin.Tags)
					{
						counter += tag.Length;
						tempTags.Add(tag);

						if (counter > 50)
						{
							break;
						}
					}
				}

				foreach (var tag in tempTags)
				{
					if (string.IsNullOrEmpty(tag)) continue;

					var size = ((float)tag.Length).Scale(0, 5, 0.03f, 0.125f);
					var bg = cui.CreateProtectedButton(container, tags, TagFilter.Contains(tag) ? "0.8 0.3 0.3 0.8" : "0.2 0.2 0.2 0.4", "1 1 1 1", tag.ToUpper(), 8, xMin: tagOffset, xMax: tagOffset + size, command: $"pluginbrowser.tagfilter {tag}");

					tagOffset += size + tagSpacing;
				}

				Facepunch.Pool.FreeUnmanaged(ref tempTags);

				#endregion

				cui.CreateProtectedButton(container, mainPanel, "#2f802f", "1 1 1 1", "<", 10, align: TextAnchor.MiddleCenter, command: "pluginbrowser.changeselectedplugin -1", xMin: 0f, xMax: 0.02f, yMin: 0.45f, yMax: 0.55f);
				cui.CreateProtectedButton(container, mainPanel, "#2f802f", "1 1 1 1", ">", 10, align: TextAnchor.MiddleCenter, command: "pluginbrowser.changeselectedplugin 1", xMin: 0.98f, xMax: 1f, yMin: 0.45f, yMax: 0.55f);

				cui.CreateProtectedButton(container, parent: mainPanel,
					color: "0.6 0.2 0.2 0.9",
					textColor: "1 0.5 0.5 1",
					text: "X", 9,
					xMin: 0.965f, xMax: 0.99f, yMin: 0.955f, yMax: 0.99f,
					command: "pluginbrowser.deselectplugin",
					font: CUI.Handler.FontTypes.DroidSansMono);

				if (Singleton.HasAccess(ap.Player, "plugins.setup"))
				{
					var buttonColor = string.Empty;
					var elementColor = string.Empty;
					var icon = string.Empty;
					var status = string.Empty;
					var scale = 0f;
					var callMode = 0;
					var isAdmin = auth != null && auth.IsLoggedIn && auth.User.IsAdmin;

					if (!selectedPlugin.IsInstalled())
					{
						if (!isAdmin && !selectedPlugin.Owned && selectedPlugin.IsPaid())
						{
							buttonColor = "#2f802f";
							elementColor = "#75f475";
							icon = "shopping";
							status = "BUY NOW";
							scale = 0.572f;
							callMode = 3;
						}
						else
						{
							if (!selectedPlugin.IsBusy)
							{
								buttonColor = "#2f802f";
								elementColor = "#75f475";
								status = "DOWNLOAD";
								icon = "clouddl";
								scale = 0.595f;
								callMode = 0;
							}
							else
							{
								buttonColor = "#78772e";
								elementColor = "#c3bd5b";
								icon = "clouddl";
								status = "IN PROGRESS";
								scale = 0.595f;
							}
						}
					}
					else
					{
						if (selectedPlugin.IsUpToDate())
						{
							buttonColor = "#802f2f";
							elementColor = "#c35b5b";
							icon = "trashcan";
							status = "REMOVE";
							scale = 0.564f;
							callMode = 2;
						}
						else
						{
							buttonColor = "#2f802f";
							elementColor = "#75f475";
							icon = "clouddl";
							status = "UPDATE";
							scale = 0.564f;
							callMode = 1;
						}
					}

					if (vendor is not Installed)
					{
						if (isAdmin || selectedPlugin.Owned || !selectedPlugin.IsPaid() || selectedPlugin.IsInstalled())
						{
							var mainButton = cui.CreateProtectedButton(container, mainPanel, buttonColor, "0 0 0 0", string.Empty, 0, xMin: 0.48f, xMax: scale, yMin: 0.175f, yMax: 0.235f, align: TextAnchor.MiddleRight, command: selectedPlugin.IsBusy ? "" : $"pluginbrowser.interact {callMode} {selectedPlugin.Id}");
							cui.CreateText(container, mainButton, elementColor, status, 11, xMax: 0.88f, align: TextAnchor.MiddleRight);
							cui.CreateImage(container, mainButton, icon, elementColor, xMin: 0.1f, xMax: 0.3f, yMin: 0.2f, yMax: 0.8f);
						}

						if(selectedPlugin.IsInstalled())
						{
							var secondaryButton = cui.CreateProtectedButton(container, mainPanel, "#802f2f", "0 0 0 0", string.Empty, 0, xMin: 0.48f, xMax: scale, yMin: 0.175f, yMax: 0.235f, OxMin: 82, OxMax: 82, align: TextAnchor.MiddleRight, command: selectedPlugin.IsBusy ? "" : $"pluginbrowser.interact 4 {selectedPlugin.Id}");
							cui.CreateText(container, secondaryButton, "#c35b5b", "RELOAD", 11, xMax: 0.88f, align: TextAnchor.MiddleRight);
							cui.CreateImage(container, secondaryButton, "reload", "#c35b5b", xMin: 0.075f, xMax: 0.315f, yMin: 0.2f, yMax: 0.8f);
						}
					}
					else if(selectedPlugin.IsInstalled())
					{
						var secondaryButton = cui.CreateProtectedButton(container, mainPanel, "#802f2f", "0 0 0 0", string.Empty, 0, xMin: 0.48f, xMax: scale, yMin: 0.175f, yMax: 0.235f, align: TextAnchor.MiddleRight, command: selectedPlugin.IsBusy ? "" : $"pluginbrowser.interact 4 {selectedPlugin.Id}");
						cui.CreateText(container, secondaryButton, "#c35b5b", "RELOAD", 11, xMax: 0.88f, align: TextAnchor.MiddleRight);
						cui.CreateImage(container, secondaryButton, "reload", "#c35b5b", xMin: 0.075f, xMax: 0.315f, yMin: 0.2f, yMax: 0.8f);
					}

					if (selectedPlugin.IsInstalled())
					{
						var path = Path.Combine(Defines.GetConfigsFolder(), selectedPlugin.ExistentPlugin.Config.Filename);

						if (OsEx.File.Exists(path))
						{
							cui.CreateProtectedButton(container, mainPanel, "0.1 0.1 0.1 0.8", "0.7 0.7 0.7 0.7", "EDIT CONFIG", 11,
								xMin: 0.48f, xMax: 0.564f, yMin: 0.175f, yMax: 0.235f, OyMin: 35, OyMax: 35,
								command: selectedPlugin.IsBusy ? "" : $"pluginbrowser.interact 3 {selectedPlugin.Id}");

							cui.CreateProtectedButton(container, mainPanel, "0.1 0.1 0.1 0.8", "0.7 0.7 0.7 0.7", "EDIT LANG", 11,
								xMin: 0.48f, xMax: 0.56f, yMin: 0.175f, yMax: 0.235f, OyMin: 35, OyMax: 35, OxMin: 82, OxMax: 82,
								command: selectedPlugin.IsBusy ? "" : $"pluginbrowser.interact 5 {selectedPlugin.Id}");
						}
					}
				}
			}

			if (auth != null)
			{
				if (auth.IsLoggedIn && auth.User.PendingAccessToken)
				{
					var mainPanel = cui.CreatePanel(container, parent, "0.15 0.15 0.15 0.35", blur: true);
					cui.CreatePanel(container, mainPanel, "0 0 0 0.9");

					var image = cui.CreatePanel(container, parent, "1 1 1 1", xMin: 0.12f, xMax: 0.45f, yMin: 0.2f, yMax: 0.8f);

					var code = string.Format(auth.AuthRequestEndpoint, auth.AuthCode);
					var qr = cui.CreateQRCodeImage(container, image, code,
						brandUrl: vendor.Logo,
						brandColor: "0 0 0 1",
						brandBgColor: "1 1 1 1", 15, true, true, "0 0 0 1", xMin: 0, xMax: 1, yMin: 0, yMax: 1);
					var authUrl = cui.CreatePanel(container, image, "0.1 0.1 0.1 0.8", yMax: 0, OyMin: -20);
					cui.CreateInputField(container, authUrl, "1 1 1 1", auth.AuthRequestEndpointPreview, 9, 0, true);

					if (auth.User.PendingResult != LoggedInUser.RequestResult.None)
					{
						var icon = string.Empty;
						var color = string.Empty;

						switch (auth.User.PendingResult)
						{
							case LoggedInUser.RequestResult.Complete:
								icon = "checkmark";
								color = "#81c740";
								break;
						}

						if (!string.IsNullOrEmpty(icon))
						{
							cui.CreatePanel(container, image, "0 0 0 0.4", blur: true);
							cui.CreateImage(container, image, icon, color, xMin: 0.3f, xMax: 0.7f, yMin: 0.3f, yMax: 0.7f);
						}
					}

					cui.CreateText(container, parent, "1 1 1 1", $"{vendor.Type} Auth", 25, xMin: 0.51f, yMax: 0.75f, align: TextAnchor.UpperLeft, font: CUI.Handler.FontTypes.RobotoCondensedBold);
					cui.CreateText(container, parent, "1 1 1 0.5", $"Securely log into your {vendor.Type} account through OAuth-based login!\n\nScan the QR code or go to the URL, log into {vendor.Type} and type in the provided authentication code below to complete the login process.", 15, xMin: 0.51f, xMax: 0.9f, yMax: 0.67f, align: TextAnchor.UpperLeft);

					cui.CreateText(container, parent, "1 1 1 1", "Authorization code:", 13, xMin: 0.51f, yMax: 0.35f, align: TextAnchor.UpperLeft);
					var pan = cui.CreatePanel(container, parent, "0.1 0.1 0.1 1",
						xMin: 0.5f, xMax: 0.8f,
						yMin: 0.21f, yMax: 0.31f);

					cui.CreateInputField(container, pan, "1 1 1 1", auth.AuthCode, 30, 0, true,
						xMin: 0.05f,
						align: TextAnchor.MiddleLeft,
						font: CUI.Handler.FontTypes.RobotoCondensedBold);

					cui.CreateProtectedButton(container, parent: mainPanel,
						color: "0.6 0.2 0.2 0.9",
						textColor: "1 0.5 0.5 1",
						text: "X", 9,
						xMin: 0.965f, xMax: 0.99f, yMin: 0.955f, yMax: 0.99f,
						command: "pluginbrowser.closelogin",
						font: CUI.Handler.FontTypes.DroidSansMono);
				}
			}

			Facepunch.Pool.FreeUnmanaged(ref plugins);
		}

		#region Vendors

		[ProtoContract]
		[ProtoInclude(100, typeof(Codefling))]
		[ProtoInclude(101, typeof(uMod))]
		public abstract class Vendor
		{
			public virtual string Type { get; }
			public virtual string Url { get; }
			public virtual string Logo { get; }
			public virtual float LogoRatio { get; }
			public virtual string Hero { get; }

			public virtual bool CanRefresh { get; } = true;

			public virtual string BarInfo { get; }

			public IEnumerable<Plugin> PriceData;
			public IEnumerable<Plugin> AuthorData;
			public IEnumerable<Plugin> InstalledData;
			public IEnumerable<Plugin> OutOfDateData;
			public IEnumerable<Plugin> OwnedData;

			public virtual string ListEndpoint { get; }
			public virtual string DownloadEndpoint { get; }
			public virtual string PluginLookupEndpoint { get; }

			[ProtoMember(1)]
			public List<Plugin> FetchedPlugins = new();

			[ProtoMember(2)]
			public long LastTick;

			public abstract void Refresh();
			public abstract void FetchList(Action<Vendor> callback = null);
			public abstract void Download(string id, Action onTimeout = null);
			public abstract void Uninstall(string id);
			public abstract void CheckMetadata(string id, Action onMetadataRetrieved);

			public virtual void VersionCheck()
			{
				foreach (var plugin in FetchedPlugins)
				{
					if (plugin.IsInstalled() && ServerOwner.Singleton.IsAutoUpdatable(plugin.Name) && !plugin.IsUpToDate())
					{
						// 3156772569 aka OnPluginOutdated
						HookCaller.CallStaticHook(3156772569, plugin.Name, new VersionNumber(plugin.CurrentVersion()), new VersionNumber(plugin.Version), plugin.ExistentPlugin, Type);
					}
				}
			}
		}

		public interface IVendorStored
		{
			bool Load();
			void Save();
		}
		public interface IVendorAuthenticated
		{
			public string AuthCode { get; set; }
			public string AuthRequestEndpoint { get; }
			public string AuthRequestEndpointPreview { get; }
			public string AuthValidationEndpoint { get; }
			public string AuthUserInfoEndpoint { get; }
			public string AuthOwnedPluginsEndpoint { get; }
			public string AuthDownloadFileEndpoint { get; }
			public KeyValuePair<HttpRequestHeader, string> AuthHeader { get; }

			public float AuthValidationCheckRate { get; }
			public Timer ValidationTimer { get; set; }

			public LoggedInUser User { get; set; }
			public bool IsLoggedIn { get; }

			public void Validate(PlayerSession session, Action onCompletion);
			public void RefreshUser(PlayerSession session);
		}

		[ProtoContract]
		public class LoggedInUser
		{
			[ProtoMember(1)]
			public int Id;
			[ProtoMember(2)]
			public string Authority;
			[ProtoMember(3)]
			public string DisplayName;
			[ProtoMember(4)]
			public string AvatarUrl;
			[ProtoMember(5)]
			public string AccessTokenEncoded;
			[ProtoMember(6)]
			public string CoverUrl;

			[ProtoMember(500)]
			public bool PendingAccessToken;
			[ProtoMember(501)]
			public RequestResult PendingResult;
			[ProtoMember(502)]
			public bool IsAdmin;

			[ProtoMember(600)]
			public List<string> OwnedFiles { get; } = new();

			public string AccessToken;

			public enum RequestResult
			{
				None,
				Processing,
				Complete
			}
		}

		public enum Status
		{
			Pending = 1,
			Approved = 0,
			Hidden = -1,
			Deleted = -2,
		}

		[ProtoContract]
		public class Codefling : Vendor, IVendorStored, IVendorAuthenticated
		{
			public override string Type => "Codefling";
			public override string Url => "https://codefling.com";
			public override string Logo => "cflogo";
			public override float LogoRatio => 0f;
			public override string Hero => "cf_hero";

			public override string BarInfo => $"{FetchedPlugins.Count(x => !x.IsPaid()):n0} free, {FetchedPlugins.Count(x => x.IsPaid()):n0} paid";

			public override string ListEndpoint => "https://codefling.com/db/?category=2";
			public string List2Endpoint => "https://codefling.com/db/?category=21";
			public override string DownloadEndpoint => "https://codefling.com/files/file/[ID]-a?do=download";

			internal Dictionary<string, string> _headers = new();

			private readonly static string _backSlashes = "\\";

			public override void Refresh()
			{
				if (FetchedPlugins == null) return;

				var plugins = Facepunch.Pool.Get<List<RustPlugin>>();
				Community.Runtime.Core.plugins.GetAllNonAlloc(plugins);
				var auth = this as IVendorAuthenticated;

				foreach (var plugin in FetchedPlugins)
				{
					try
					{
						var fileName = Path.GetFileName(plugin.File);
						var fileNameNoExtension = Path.GetFileNameWithoutExtension(plugin.File);
						plugin.SetOwned(auth.User != null && auth.User.OwnedFiles.Contains(plugin.Id));

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
					catch (Exception ex)
					{
						Logger.Warn($"{plugin.File} ({ex.Message})\n{ex.StackTrace}");
					}
				}

				Facepunch.Pool.FreeUnmanaged(ref plugins);

				PriceData = FetchedPlugins.Where(x => x.Status == Status.Approved).OrderBy(x => x.OriginalPrice.ToFloat());
				AuthorData = FetchedPlugins.Where(x => x.Status == Status.Approved).OrderBy(x => x.Author);
				InstalledData = FetchedPlugins.Where(x => x.IsInstalled());
				OutOfDateData = FetchedPlugins.Where(x => x.Status == Status.Approved).Where(x => x.IsInstalled() && !x.IsUpToDate());
				OwnedData = FetchedPlugins.Where(x => x.Owned);
			}
			public override void FetchList(Action<Vendor> callback = null)
			{
				Community.Runtime.Core.webrequest.Enqueue(ListEndpoint, null, (error, data) =>
				{
					if (error != 200)
					{
						Logger.Log($"[{Type}] Failed fetching vendor. Error code {error}!");
						return;
					}

					FetchedPlugins.Clear();
					var plugins = Facepunch.Pool.Get<List<RustPlugin>>();
					Community.Runtime.Core.plugins.GetAllNonAlloc(plugins);
					ParseData(data, false, false, FetchedPlugins, callback, this, plugins);
					Facepunch.Pool.FreeUnmanaged(ref plugins);

					Community.Runtime.Core.webrequest.Enqueue(List2Endpoint, null, (error, data) =>
					{
						if (error != 200)
						{
							Logger.Error($"[{Type}] Failed parsing data for vendor. Error code {error}!");
							return;
						}

						var plugins = Facepunch.Pool.Get<List<RustPlugin>>();
						Community.Runtime.Core.plugins.GetAllNonAlloc(plugins);
						ParseData(data, true, true, FetchedPlugins, callback, this, plugins);
						Facepunch.Pool.FreeUnmanaged(ref plugins);

						VersionCheck();
					}, Community.Runtime.Core);

					static void ParseData(string data, bool doSave, bool insert, List<Plugin> fetchedPlugins, Action<Vendor> callback, Vendor vendor, List<RustPlugin> plugins)
					{
						try
						{
							var list = JArray.Parse(data);
							foreach (var token in list)
							{
								//var fileStatus = token["file_status"]?.ToString();
								var price = token["prices"];

								var plugin = new Plugin
								{
									Id = token["id"]?.ToString(),
									Name = token["title"]?.ToString(),
									Author = token["author"]?.ToString(),
									Description = token["description"]?.ToString().Replace(_backSlashes, string.Empty),
									Version = token["version"]?.ToString(),
									OriginalPrice = price == null || !price.HasValues ? "FREE" : price["USD"]?.ToString(),
									Date = token["date"]?.ToString(),
									UpdateDate = token["updated"]?.ToString(),
									Changelog = token["changelog"]?.ToString().Replace(_backSlashes, string.Empty),
									File = token["fileName"]?.ToString(),
									Image = $"https://codefling.com/cdn-cgi/image/width=1250,height=1250,quality=100,blur=25,fit=cover,format=jpeg/{token["primaryScreenshot"]?.ToString()}",
									ImageThumbnail = $"https://codefling.com/cdn-cgi/image/width=246,height=246,quality=75,fit=cover,format=jpeg/{token["primaryScreenshot"]?.ToString()}",
									Tags = token["tags"]?.Select(x => x.ToString()),
									DownloadCount = (token["downloads"]?.ToString().ToInt()).GetValueOrDefault(),
									// Dependencies = token["file_depends"]?.ToString().Split(),
									CarbonCompatible = (token["compatibility"]?.ToString().ToBool()).GetValueOrDefault(),
									Rating = (token["rating"]?.ToString().ToFloat()).GetValueOrDefault(0),
									Status = Status.Approved,
									HasLookup = true
								};

								var updateDate = DateTimeOffset.FromUnixTimeSeconds(plugin.UpdateDate.ToLong());
								var date = DateTimeOffset.FromUnixTimeSeconds(plugin.Date.ToLong());
								plugin.UpdateDate = updateDate.UtcDateTime.ToString();
								plugin.Date = date.UtcDateTime.ToString();

								try { plugin.Description = plugin.Description.TrimStart('\t').Replace("\t", "\n").Split('\n')[0]; } catch { }

								if (plugin.OriginalPrice == "{}") plugin.OriginalPrice = "FREE";
								try { plugin.ExistentPlugin = plugins.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.FilePath) == Path.GetFileNameWithoutExtension(plugin.File)) as RustPlugin; } catch { }

								if (insert)
								{
									fetchedPlugins.Insert(0, plugin);
								}
								else
								{
									fetchedPlugins.Add(plugin);
								}
							}

							if (doSave)
							{
								callback?.Invoke(vendor);

								Logger.Log($"[{vendor.GetType()} Tab] Fetched latest plugin information.");

								if (vendor is IVendorStored stored)
									stored.Save();
							}
						}
						catch (Exception ex)
						{
							Logger.Error($" Couldn't fetch Codefling API to get the plugins list. Most likely because it's down.", ex);
						}
					}
				}, Community.Runtime.Core);
			}
			public override void Download(string id, Action onTimeout = null)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				plugin.IsBusy = true;
				plugin.DownloadCount++;

				var core = Community.Runtime.Core;

				core.timer.In(2f, () =>
				{
					if (plugin.IsBusy)
					{
						plugin.IsBusy = false;
						onTimeout?.Invoke();
					}
				});

				if (IsLoggedIn)
				{
					var extension = Path.GetExtension(plugin.File);

					switch (extension)
					{
						case ".zip":
							core.webrequest.Enqueue(string.Format(AuthDownloadFileEndpoint, plugin.Id), null, (error, source) =>
							{
								if (error != 200)
								{
									Logger.Error($"Auth token for Codefling is expired! Please log in once again.");
									User = null;
									return;
								}

								var jobject = JObject.Parse(source);
								var name = jobject["files"][0]["name"].ToString();
								var file = jobject["files"][0]["url"].ToString();
								var path = Path.Combine(Defines.GetScriptsFolder(), name);

								core.webrequest.EnqueueData(file, null, (_, source) =>
								{
									plugin.IsBusy = false;

									using var stream = new MemoryStream(source);
									using var zip = new ZipArchive(stream);

									const string sourceExtension = ".cs";
									const string dllExtension = ".dll";
									const string jsonExtension = ".json";
									const string dataFolder = "data";
									const string configFolder = "config";

									static void StoreFile(ZipArchiveEntry entry, string path, string context)
									{
										using var memoryStream = new MemoryStream();
										using var entryStream = entry.Open();
										entryStream.CopyTo(memoryStream);
										var bytes = memoryStream.ToArray();

										OsEx.File.Create(path, bytes);
										Singleton.Puts($" Extracted plugin {context} file '{entry.Name}'");
									}

									foreach (var file in zip.Entries)
									{
										switch (Path.GetExtension(file.Name))
										{
											case sourceExtension:
												{
													using var reader = new StreamReader(file.Open());
													var fileSource = reader.ReadToEnd();

													OsEx.File.Create(Path.Combine(Defines.GetScriptsFolder(), file.Name), fileSource);
													Singleton.Puts($" Extracted plugin file {file.Name}");
												}
												break;

											case dllExtension:
												{
													StoreFile(file, Path.Combine(Defines.GetLibFolder(), file.Name), "extension");
												}
												break;

											case jsonExtension:
												{
													switch (file.FullName)
													{
														case var data when data.Contains(dataFolder):
															{
																var offsetIndex = data.IndexOf(dataFolder) + 5;
																var subFolder = Path.GetDirectoryName(data[offsetIndex..]);
																StoreFile(file, Path.Combine(Defines.GetDataFolder(), subFolder, file.Name), "data");
															}
															break;

														case var config when config.Contains(configFolder):
															{
																var offsetIndex = config.IndexOf(configFolder) + 5;
																var subFolder = Path.GetDirectoryName(config[offsetIndex..]);
																StoreFile(file, Path.Combine(Defines.GetConfigsFolder(), subFolder, file.Name), "config");
															}
															break;
													}
												}
												break;
										}
									}

									Singleton.Puts($"Downloaded {plugin.Name}");
									OsEx.File.Create(path, source);
								}, core, headers: _headers);

							}, core, headers: _headers);
							break;

						case ".cs":
							core.webrequest.Enqueue(string.Format(AuthDownloadFileEndpoint, plugin.Id), null, (error, source) =>
							{
								if (error != 200)
								{
									Logger.Error($"Auth token for Codefling is expired! Please log in once again.");
									User = null;
									return;
								}

								var jobject = JObject.Parse(source);
								var name = jobject["files"][0]["name"].ToString();
								var file = jobject["files"][0]["url"].ToString();
								var path = Path.Combine(Defines.GetScriptsFolder(), plugin.File);
								jobject = null;

								core.webrequest.Enqueue(file, null, (_, source) =>
								{
									plugin.IsBusy = false;

									Singleton.Puts($"Downloaded {plugin.Name}");
									OsEx.File.Create(path, source);
								}, core, headers: _headers);

							}, core, headers: _headers);
							break;
					}
				}
				else
				{
					var path = Path.Combine(Defines.GetScriptsFolder(), plugin.File);
					var url = DownloadEndpoint.Replace("[ID]", id);

					core.webrequest.Enqueue(url, null, (error, source) =>
					{
						if (error != 200)
						{
							Logger.Error($"[{Type}] Failed downloading item '{plugin.Name} by {plugin.Author}'. Error code {error}!");
							return;
						}

						plugin.IsBusy = false;

						if (!source.StartsWith("<!DOCTYPE html>"))
						{
							Singleton.Puts($"Downloaded {plugin.Name}");
							OsEx.File.Create(path, source);
						}
					}, core, headers: new Dictionary<string, string>
					{
						["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.63",
						["accept"] = "ext/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
					});
				}
			}
			public override void Uninstall(string id)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				ModLoader.UninitializePlugin(plugin.ExistentPlugin);
				OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Defines.GetScriptsFolder(), "backups", plugin.ExistentPlugin.FileName), true);
				plugin.ExistentPlugin = null;
			}
			public override void CheckMetadata(string id, Action onMetadataRetrieved)
			{

			}

			#region Auth

			[ProtoMember(50, IsRequired = false)]
			public LoggedInUser User { get; set; }

			public string AuthRequestEndpoint => "https://codefling.com/auth/?pin={0}";
			public string AuthRequestEndpointPreview => "codefling.com/auth";
			public string AuthValidationEndpoint => "https://codefling.com/auth/bearer?code={0}";
			public string AuthUserInfoEndpoint => "https://codefling.com/api/core/me";
			public string AuthOwnedPluginsEndpoint => "https://codefling.com/api/nexus/purchases?perPage=100000&itemType=file&itemApp=downloads";
			public string AuthDownloadFileEndpoint => "https://codefling.com/api/downloads/files/{0}/download";
			public KeyValuePair<HttpRequestHeader, string> AuthHeader => new(HttpRequestHeader.Authorization, "Bearer {0}");
			public float AuthValidationCheckRate => 5f;
			public Timer ValidationTimer { get; set; }
			public string AuthCode { get; set; }

			public void Validate(PlayerSession session, Action onComplete)
			{
				var core = Community.Runtime.Core;

				ValidationTimer = core.timer.Every(AuthValidationCheckRate, () =>
				{
					if (User == null || !session.IsInMenu)
					{
						ValidationTimer?.Destroy();
						ValidationTimer = null;
						User = null;
						return;
					}

					var url = string.Format(AuthValidationEndpoint, AuthCode);

					core.webrequest.Enqueue(url, null, (code, result) =>
					{
						User ??= new();

						switch (code)
						{
							case 401:
								var previous = User.PendingResult;
								User.PendingResult = LoggedInUser.RequestResult.Processing;

								if (previous != User.PendingResult)
								{
									Singleton.Draw(session.Player);
								}
								break;

							default:
								var jobject = JObject.Parse(result);
								User.AccessToken = jobject["accesstoken"].ToString();
								User.AccessTokenEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(User.AccessToken));
								ValidationTimer.Destroy();
								ValidationTimer = null;
								_headers[AuthHeader.Key.ToString()] = string.Format(AuthHeader.Value, User.AccessToken);
								Analytics.codefling_login();

								User.PendingResult = LoggedInUser.RequestResult.Complete;
								onComplete?.Invoke();
								break;
						}
					}, null);
				});
			}
			public void RefreshUser(PlayerSession session)
			{
				if (!IsLoggedIn) return;

				var core = Community.Runtime.Core;
				var authHeader = AuthHeader;
				var headers = new Dictionary<string, string>()
				{
					[authHeader.Key.ToString()] = string.Format(authHeader.Value, User.AccessToken)
				};

				core.webrequest.Enqueue(AuthUserInfoEndpoint, null, (code, info) =>
				{
					switch (code)
					{
						case 200:
							var jobject = JObject.Parse(info);
							User.Authority = jobject["primaryGroup"]["name"]?.ToString();
							User.AvatarUrl = jobject["photoUrl"]?.ToString();
							User.DisplayName = jobject["formattedName"]?.ToString();
							User.CoverUrl = jobject["coverPhotoUrl"]?.ToString();
							User.Id = jobject["id"].ToString().ToInt();
							User.IsAdmin = User.Authority == "Administrator";

							core.webrequest.Enqueue(AuthOwnedPluginsEndpoint, null, (code, data) =>
							{
								var jobject = JObject.Parse(data);
								User.OwnedFiles.Clear();

								foreach (var item in jobject["results"])
								{
									User.OwnedFiles.Add(item["itemId"].ToString());
								}

								Refresh();
								Save();
								Singleton.Draw(session.Player);
							}, core, headers: headers);
							break;

						default:
							User = null;
							Refresh();
							Save();
							Singleton.Draw(session.Player);
							break;
					}
				}, core, headers: headers);
			}

			public bool IsLoggedIn => User != null;

			#endregion

			#region Stored

			public bool Load()
			{
				try
				{
					var path = Path.Combine(Defines.GetDataFolder(), "vendordata_cf.db");
					if (!OsEx.File.Exists(path)) return false;

					using var file = new MemoryStream(OsEx.File.ReadBytes(path));
					var value = Serializer.Deserialize<Codefling>(file);

					LastTick = value.LastTick;
					FetchedPlugins.Clear();
					FetchedPlugins.AddRange(value.FetchedPlugins);
					User = value.User;

					if (User != null && !string.IsNullOrEmpty(User.AccessTokenEncoded))
					{
						User.AccessToken = Encoding.UTF8.GetString(Convert.FromBase64String(User.AccessTokenEncoded));
						_headers[AuthHeader.Key.ToString()] = string.Format(AuthHeader.Value, User.AccessToken);
					}

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
					var path = Path.Combine(Defines.GetDataFolder(), "vendordata_cf.db");
					using var file = new MemoryStream();
					LastTick = DateTime.Now.Ticks;
					Serializer.Serialize(file, this);
					OsEx.File.Create(path, file.ToArray());
					Singleton.Puts($"Stored {Type} to file: {path}");
				}
				catch (Exception ex)
				{
					Logger.Error($"{Type}.Save error", ex);
				}
			}

			#endregion
		}

		[ProtoContract]
		public class uMod : Vendor, IVendorStored
		{
			public override string Type => "uMod";
			public override string Url => "https://umod.org";
			public override string Logo => "umodlogo";
			public override float LogoRatio => 0.2f;
			public override string Hero => "umod_hero";

			public override string BarInfo => $"{FetchedPlugins.Count:n0} free";

			public override string ListEndpoint => "https://umod.org/plugins/search.json?page=[ID]&sort=title&sortdir=asc&categories%5B0%5D=universal&categories%5B1%5D=rust";
			public override string DownloadEndpoint => "https://umod.org/plugins/[ID].cs";
			public override string PluginLookupEndpoint => "https://umod.org/plugins/[ID]/latest.json";

			public override void Refresh()
			{
				if (FetchedPlugins == null) return;

				var plugins = Facepunch.Pool.Get<List<RustPlugin>>();
				Community.Runtime.Core.plugins.GetAllNonAlloc(plugins);

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

				Community.Runtime.Core.webrequest.Enqueue(ListEndpoint.Replace("[ID]", "0"), null, (error, data) =>
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
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				var path = Path.Combine(Defines.GetScriptsFolder(), plugin.File);
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
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				ModLoader.UninitializePlugin(plugin.ExistentPlugin);
				OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Defines.GetScriptsFolder(), "backups", plugin.ExistentPlugin.FileName), true);
				plugin.ExistentPlugin = null;
			}
			public override void CheckMetadata(string id, Action onMetadataRetrieved)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
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


				Community.Runtime.Core.webrequest.Enqueue(ListEndpoint.Replace("[ID]", $"{page}"), null, (error, data) =>
				{
					if (error != 200)
					{
						Logger.Error($"[{Type}] Failed fetching page for vendor. Error code {error}!");
						return;
					}

					var list = JObject.Parse(data);
					var file = list["data"];
					var plugins = Facepunch.Pool.Get<List<RustPlugin>>();
					Community.Runtime.Core.plugins.GetAllNonAlloc(plugins);
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

						if (!string.IsNullOrEmpty(p.Description) && !p.Description.EndsWith(".")) p.Description += ".";

						if (string.IsNullOrEmpty(p.Author.Trim())) p.Author = "Unmaintained";
						if (p.OriginalPrice == "{}") p.OriginalPrice = "FREE";
						try { p.ExistentPlugin = plugins.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.FilePath) == Path.GetFileNameWithoutExtension(p.File)) as RustPlugin; } catch { }

						if (!FetchedPlugins.Any(x => x.Name == p.Name)) FetchedPlugins.Add(p);
					}
					Facepunch.Pool.FreeUnmanaged(ref plugins);

					if (page % (maxPage / 4) == 0 || page == maxPage - 1)
					{
						Logger.Log($"Caching plugin metadata page {page} out of {maxPage}");
					}
				}, Community.Runtime.Core);
				Community.Runtime.Core.timer.In(5f, () => FetchPage(page + 1, maxPage, callback));
			}

			public bool Load()
			{
				try
				{
					var path = Path.Combine(Defines.GetDataFolder(), "vendordata_umod.db");
					if (!OsEx.File.Exists(path)) return false;

					using var file = new MemoryStream(OsEx.File.ReadBytes(path));
					var value = Serializer.Deserialize<uMod>(file);

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

		#endregion

		[ProtoContract]
		public class Installed : Vendor
		{
			public override string Type => "Installed";
			public override string Url => "none";
			public override string Logo => "carbonw";
			public override string Hero => "installed_hero";

			public override float LogoRatio => 0.23f;
			public override string ListEndpoint => string.Empty;
			public override string DownloadEndpoint => string.Empty;
			public override string BarInfo => $"{FetchedPlugins.Count:n0} loaded";

			public override bool CanRefresh => false;

			internal string[] _defaultTags = ["carbon", "oxide"];

			public override void CheckMetadata(string id, Action callback)
			{
			}

			public override void Download(string id, Action onTimeout = null)
			{
			}

			public override void FetchList(Action<Vendor> callback = null)
			{
			}

			public bool Load()
			{
				return true;
			}

			public override void Refresh()
			{
				FetchedPlugins.Clear();

				foreach (var package in ModLoader.Packages)
				{
					foreach (var plugin in package.Plugins)
					{
						if (plugin.IsCorePlugin) continue;

						var existent = FetchedPlugins.FirstOrDefault(x => x.ExistentPlugin == plugin);

						if (existent == null)
						{
							existent = CodeflingInstance.FetchedPlugins.FirstOrDefault(x => x.ExistentPlugin == plugin);

							if (existent == null)
							{
								existent = uModInstance.FetchedPlugins.FirstOrDefault(x => x.ExistentPlugin == plugin);

								if (existent == null)
								{
									existent = new Plugin
									{
										Name = plugin.Name,
										Author = plugin.Author,
										Version = plugin.Version.ToString(),
										ExistentPlugin = plugin,
										Description = "This is an unlisted plugin.",
										Tags = _defaultTags,
										File = plugin.FileName,
										Id = plugin.Name,
										UpdateDate = DateTime.UtcNow.ToString(),
										Rating = -1
									};
								}

							}

							FetchedPlugins.Add(existent);
						}
					}
				}

				FetchedPlugins = FetchedPlugins.OrderBy(x => x.Name).ToList();
				PriceData = FetchedPlugins.OrderBy(x => x.OriginalPrice);
				AuthorData = FetchedPlugins.OrderBy(x => x.Author);
				InstalledData = FetchedPlugins.Where(x => x.IsInstalled());
				OutOfDateData = FetchedPlugins.Where(x => x.IsInstalled() && !x.IsUpToDate());
				OwnedData = FetchedPlugins.OrderBy(x => x.Owned);
			}

			public void Save()
			{
			}

			public override void Uninstall(string id)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				ModLoader.UninitializePlugin(plugin.ExistentPlugin);
				OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Defines.GetScriptsFolder(), "backups", plugin.ExistentPlugin.FileName));
				plugin.ExistentPlugin = null;
			}
		}

		[ProtoContract]
		public class ServerOwner
		{
			public static ServerOwner Singleton { get; internal set; } = new ServerOwner();

			[ProtoMember(1)]
			public List<string> FavouritePlugins = new();

			[ProtoMember(2)]
			public List<string> AutoUpdate = new();

			public bool IsAutoUpdatable(string pluginName)
			{
				return AutoUpdate.Contains(pluginName);
			}

			public static void Load()
			{
				try
				{
					var path = Path.Combine(Defines.GetDataFolder(), "vendordata_svowner.db");
					if (!OsEx.File.Exists(path))
					{
						Save();
						return;
					}

					using var file = new MemoryStream(OsEx.File.ReadBytes(path));
					Singleton = Serializer.Deserialize<ServerOwner>(file);

					Singleton.FavouritePlugins ??= new();
					Singleton.AutoUpdate ??= new();
				}
				catch (Exception ex)
				{
					Logger.Error($"ServerOwner.Load failed", ex);
					Singleton = new();
					Save();
				}
			}
			public static void Save()
			{
				try
				{
					var path = Path.Combine(Defines.GetDataFolder(), "vendordata_svowner.db");
					using var file = new MemoryStream();

					Serializer.Serialize(file, Singleton);
					OsEx.File.Create(path, file.ToArray());
				}
				catch (Exception ex)
				{
					Logger.Error($"ServerOwner.Save failed", ex);
				}
			}
		}

		[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
		public class Plugin
		{
			public string Id;
			public string Name;
			public string Author;
			public string Version;
			public string Description;
			public string Changelog;
			public string OriginalPrice;
			public string SalePrice;
			public string[] Dependencies;
			public string File;
			public string Image;
			public string ImageThumbnail;
			public int ImageSize;
			public IEnumerable<string> Tags;
			public int DownloadCount;
			public float Rating;
			public string Date;
			public string UpdateDate;
			public bool HasLookup;
			public Status Status = Status.Approved;
			public bool CarbonCompatible;
			public bool Owned;

			internal RustPlugin ExistentPlugin;
			internal bool IsBusy;

			public Plugin()
			{
			}

			[ProtoIgnore]
			public bool HasRating => Rating != -1;

			[ProtoIgnore]
			public bool HasPrice => OriginalPrice != "Null";

			public bool HasInvalidImage()
			{
				return ImageSize >= 2504304;
			}
			public bool HasNoImage()
			{
				return string.IsNullOrEmpty(Image) || Image.Equals("Null");
			}
			public bool IsInstalled()
			{
				return ExistentPlugin != null && ExistentPlugin.IsLoaded;
			}
			public string CurrentVersion()
			{
				return !IsInstalled() ? "N/A" : ExistentPlugin.Version.ToString();
			}
			public bool IsPaid()
			{
				return OriginalPrice != "FREE" && OriginalPrice != "Null";
			}
			public bool IsUpToDate()
			{
				if (!IsInstalled()) return false;

				return ExistentPlugin.Version.ToString() == Version;
			}

			public void SetOwned(bool wants) => Owned = wants;
			public void SetExistentPlugin(RustPlugin plugin) => ExistentPlugin = plugin;
		}
	}

	#region Commands

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.changetab")]
	private void PluginBrowserChange(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = PluginsTab.GetVendor(ap.SetStorage(tab, "vendor", (PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), args.Args[0])));
		vendor.Refresh();
		PluginsTab.TagFilter.Clear();
		PluginsTab.DropdownShow = false;
		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));
		Singleton.Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.interact")]
	private void PluginBrowserInteract(Arg args)
	{
		var player = args.Player();

		if (!Singleton.HasAccess(player, "plugins.setup")) return;

		var ap = Singleton.GetPlayerSession(player);
		var tab = Singleton.GetTab(ap.Player);
		var vendor = PluginsTab.GetVendor(ap.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed));
		var arg = new string[args.Args.Length];
		Array.Copy(args.Args, arg, args.Args.Length);

		switch (arg[0])
		{
			case "0":
				vendor.Download(arg[1], () => Singleton.Draw(args.Player()));
				Array.Clear(arg, 0, arg.Length);
				break;
			case "1":
				tab.CreateDialog($"Are you sure you want to update '{ap.GetStorage<PluginsTab.Plugin>(tab, "selectedplugin").Name}'?", ap =>
				{
					vendor.Download(arg[1], () => Singleton.Draw(args.Player()));
					Array.Clear(arg, 0, arg.Length);
				}, null);
				break;

			case "2":
				tab.CreateDialog($"Are you sure you want to uninstall '{ap.GetStorage<PluginsTab.Plugin>(tab, "selectedplugin").Name}'?", ap =>
				{
					Singleton.Puts($"Uninstalling {arg[1]} on {vendor?.GetType().Name}");
					vendor.Uninstall(arg[1]);
					Array.Clear(arg, 0, arg.Length);
				}, null);
				break;

			case "3":
			{
				var plugin = vendor.FetchedPlugins.FirstOrDefault(x => x.Id == args.Args[1]).ExistentPlugin;
				var path = Path.Combine(Defines.GetConfigsFolder(), plugin.Config.Filename);
				Singleton.SetTab(ap.Player, ConfigEditor.Make(OsEx.File.ReadText(path),
					(ap, jobject) =>
					{
						Community.Runtime.Core.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
					},
					(ap, jobject) =>
					{
						OsEx.File.Create(path, jobject.ToString(Formatting.Indented));
						Community.Runtime.Core.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
					},
					(ap, jobject) =>
					{
						OsEx.File.Create(path, jobject.ToString(Formatting.Indented));
						plugin.ProcessorProcess.MarkDirty();
						Community.Runtime.Core.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
					}));
				Array.Clear(arg, 0, arg.Length);
				break;
			}

			case "4":
			{
				var plugin = vendor.FetchedPlugins.FirstOrDefault(x => x.Id == args.Args[1]).ExistentPlugin;

				if (plugin != null)
				{
					plugin.ProcessorProcess.MarkDirty();
					Community.Runtime.Core.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
				}
				break;
			}

			case "5":
			{
				var plugin = vendor.FetchedPlugins.FirstOrDefault(x => x.Id == args.Args[1]).ExistentPlugin;

				Singleton.SetTab(ap.Player, LangEditor.Make(plugin,
					(ap) =>
					{
						Community.Runtime.Core.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
					}));
				break;
			}

			case "10":
				{
					var pluginName = arg.Skip(1).ToString(" ");
					if (PluginsTab.ServerOwner.Singleton.FavouritePlugins.Contains(pluginName))
					{
						PluginsTab.ServerOwner.Singleton.FavouritePlugins.Remove(pluginName);
						Logger.Log($" [{vendor.Type}] Unfavorited plugin '{pluginName}'");
					}
					else
					{
						PluginsTab.ServerOwner.Singleton.FavouritePlugins.Add(pluginName);
						Logger.Log($" [{vendor.Type}] Favorited plugin '{pluginName}'");
					}
					Array.Clear(arg, 0, arg.Length);
				}
				break;

			case "11":
				{
					var pluginName = arg.Skip(1).ToString(" ");
					if (PluginsTab.ServerOwner.Singleton.AutoUpdate.Contains(pluginName))
					{
						PluginsTab.ServerOwner.Singleton.AutoUpdate.Remove(pluginName);
						Logger.Log($" [{vendor.Type}] Marked plugin '{pluginName}' for auto-update");
					}
					else
					{
						PluginsTab.ServerOwner.Singleton.AutoUpdate.Add(pluginName);
						Logger.Log($" [{vendor.Type}] Unmarked plugin '{pluginName}' for auto-update");
					}
					Array.Clear(arg, 0, arg.Length);
				}
				break;

		}

		Singleton.Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.page")]
	private void PluginBrowserPage(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = PluginsTab.GetVendor(ap.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed));
		vendor.Refresh();
		PluginsTab.GetPlugins(vendor, tab, ap, out var maxPages, 15);

		var page = ap.GetStorage(tab, "page", 0);

		switch (args.Args[0])
		{
			case "+1":
				page++;
				break;
			case "-1":
				page--;
				break;

			default:
				page = args.Args[0].ToInt() - 1;
				break;
		}

		if (page < 0) page = maxPages;
		else if (page > maxPages) page = 0;

		ap.SetStorage(tab, "page", page);

		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.tagfilter")]
	private void PluginBrowserTagFilter(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = PluginsTab.GetVendor(ap.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed));
		vendor.Refresh();

		var filter = args.Args.ToString(" ");

		if (PluginsTab.TagFilter.Contains(filter)) PluginsTab.TagFilter.Remove(filter);
		else PluginsTab.TagFilter.Add(filter);

		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.search")]
	private void PluginBrowserSearch(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = PluginsTab.GetVendor(ap.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed));
		vendor.Refresh();

		var search = ap.SetStorage(tab, "search", args.Args.ToString(" "));
		var page = ap.SetStorage(tab, "page", 0);

		if (search == "Search...") ap.SetStorage(tab, "search", string.Empty);

		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.refreshvendor")]
	private void PluginBrowserRefreshVendor(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = PluginsTab.GetVendor(ap.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed));

		if (vendor is PluginsTab.Installed) return;

		tab.CreateDialog("Are you sure you want to redownload the plugin list?\nThis might take a while.", ap =>
		{
			var id = string.Empty;
			switch (vendor)
			{
				case PluginsTab.Codefling:
					id = "cf";
					break;

				case PluginsTab.uMod:
					id = "umod";
					break;
			}

			var dataPath = Path.Combine(Defines.GetDataFolder(), $"vendordata_{id}.db");
			OsEx.File.Delete(dataPath);

			if (vendor is PluginsTab.IVendorStored stored && !stored.Load())
			{
				vendor.FetchList(vendor => vendor.Refresh());
				vendor.Refresh();
			}
			if (vendor is PluginsTab.IVendorAuthenticated auth)
			{
				auth.RefreshUser(ap);
			}

			Singleton.Draw(args.Player());
		}, null);

		Singleton.Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.selectplugin")]
	private void PluginBrowserSelectPlugin(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = PluginsTab.GetVendor(ap.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed));
		vendor.Refresh();

		ap.SetStorage(tab, "selectedplugin", vendor.FetchedPlugins.FirstOrDefault(x => x.Id == args.Args[0]));

		Singleton.Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.deselectplugin")]
	private void PluginBrowserDeselectPlugin(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = PluginsTab.GetVendor(ap.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed));
		vendor.Refresh();

		ap.SetStorage(tab, "selectedplugin", (PluginsTab.Plugin)default);

		Singleton.Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.changeselectedplugin")]
	private void PluginBrowserChangeSelected(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = PluginsTab.GetVendor(ap.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed));
		vendor.Refresh();

		var plugins = PluginsTab.GetPlugins(vendor, tab, ap, 15);
		var nextPage = plugins.IndexOf(ap.GetStorage<PluginsTab.Plugin>(tab, "selectedplugin")) + args.Args[0].ToInt();
		ap.SetStorage(tab, "selectedplugin", plugins[nextPage > plugins.Count - 1 ? 0 : nextPage < 0 ? plugins.Count - 1 : nextPage]);
		Facepunch.Pool.FreeUnmanaged(ref plugins);

		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.changesetting")]
	private void PluginBrowserChangeSetting(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = PluginsTab.GetVendor(ap.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed));

		switch (args.Args[0])
		{
			case "filter_dd":
				PluginsTab.DropdownShow = !PluginsTab.DropdownShow;

				if (args.HasArgs(4))
				{
					var index = args.Args[3].ToInt();
					var filter = ap.GetStorage(tab, "filter", PluginsTab.FilterTypes.None);
					var flipFilter = ap.GetStorage<bool>(tab, "flipfilter");

					if ((int)filter == index)
					{
						ap.SetStorage(tab, "flipfilter", !flipFilter);
					}
					else
					{
						ap.SetStorage(tab, "flipfilter", false);
					}

					ap.SetStorage(tab, "page", 0);
					ap.SetStorage(tab, "filter", (PluginsTab.FilterTypes)index);
				}
				break;
		}

		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		vendor.Refresh();
		Singleton.Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.login")]
	private void PluginBrowserLogin(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);

		var vendor = PluginsTab.GetVendor(ap.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed));
		if (vendor is PluginsTab.IVendorAuthenticated auth)
		{
			if (auth.IsLoggedIn)
			{
				tab.CreateDialog("Are you sure you want to log out?", onConfirm: ap =>
				{
					auth.User = null;
					vendor.Refresh();
					Singleton.Draw(args.Player());
					if (vendor is PluginsTab.IVendorStored store) store.Save();
				}, null);
			}
			else
			{
				auth.AuthCode = Extensions.StringEx.Truncate(Guid.NewGuid().ToString(), 6).ToUpper();
				auth.User = new PluginsTab.LoggedInUser
				{
					PendingAccessToken = true
				};

				var currentCode = auth.AuthCode;
				var core = Community.Runtime.Core;
				var authHeader = auth.AuthHeader;

				core.timer.In(5f, () =>
				{
					if (currentCode != auth.AuthCode || !ap.IsInMenu)
					{
						auth.User = null;
						return;
					}

					auth.Validate(ap, () =>
					{
						core.timer.In(2f, () =>
						{
							auth.User.PendingAccessToken = false;
							Singleton.Draw(args.Player());

							auth.RefreshUser(ap);
						});

						Singleton.Draw(args.Player());
					});
				});
			}
		}

		Singleton.Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.closelogin")]
	private void PluginBrowserCloseLogin(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = PluginsTab.GetVendor(ap.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed));
		if (vendor is PluginsTab.IVendorAuthenticated auth)
		{
			auth.User = null;
			auth.ValidationTimer?.Destroy();

			Singleton.Draw(args.Player());
		}
	}

	[Conditional("!MINIMAL")]
	[ConsoleCommand("adminmodule.downloadplugin", "Downloads a plugin from a vendor (if available). Syntax: adminmodule.downloadplugin <codefling|umod> <plugin>")]
	[AuthLevel(2)]
	private void DownloadPlugin(Arg args)
	{
		var vendor = PluginsTab.GetVendor(args.GetString(0) == "codefling" ? PluginsTab.VendorTypes.Codefling : PluginsTab.VendorTypes.uMod);
		if (vendor == null)
		{
			Singleton.PutsWarn($"Couldn't find that vendor.");
			return;
		}

		var plugin = vendor.FetchedPlugins.FirstOrDefault(x => x.Name.Equals(args.GetString(1), StringComparison.InvariantCultureIgnoreCase));

		if (plugin == null)
		{
			Singleton.PutsWarn($"Cannot find that plugin.");
			return;
		}

		vendor.Download(plugin.Id, () => { Singleton.PutsWarn($"Couldn't download {plugin.Name}."); });
	}

	[Conditional("!MINIMAL")]
	[ConsoleCommand("adminmodule.updatevendor", "Downloads latest vendor information. Syntax: adminmodule.updatevendor <codefling|umod>")]
	[AuthLevel(2)]
	private void UpdateVendor(Arg args)
	{
		var vendor = PluginsTab.GetVendor(args.Args[0] == "codefling" ? PluginsTab.VendorTypes.Codefling : PluginsTab.VendorTypes.uMod);
		if (vendor == null)
		{
			Singleton.PutsWarn($"Couldn't find that vendor.");
			return;
		}

		var id = string.Empty;
		switch (vendor)
		{
			case PluginsTab.Codefling:
				id = "cf";
				break;

			case PluginsTab.uMod:
				id = "umod";
				break;
		}

		var dataPath = Path.Combine(Defines.GetDataFolder(), $"vendordata_{id}.db");
		OsEx.File.Delete(dataPath);

		if (vendor is PluginsTab.IVendorStored stored && !stored.Load())
		{
			vendor.FetchList(vendor => vendor.Refresh());
			vendor.Refresh();
		}
	}

	#endregion
}

#endif
