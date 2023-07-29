#if !MINIMAL

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using System.IO.Compression;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Game.Rust.Cui;
using ProtoBuf;
using static Carbon.Modules.AdminModule.PluginsTab;
using static ConsoleSystem;
using Timer = Oxide.Plugins.Timer;

namespace Carbon.Modules;

public partial class AdminModule : CarbonModule<AdminConfig, AdminData>
{
	public class PluginsTab
	{
		public enum VendorTypes
		{
			Local,
			Codefling,
			uMod
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

		public static bool DropdownShow { get; set; }
		public static string[] DropdownOptions { get; } = new string[] { "A-Z", "Price", "Author", "Installed", "Needs Update", "Favourites", "Owned" };
		public static PlayerSession.Page PlaceboPage { get; } = new PlayerSession.Page();
		public static List<string> TagFilter { get; set; } = new();
		public static string[] PopularTags { get; } = new string[]
		{
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
		};

		public static Vendor CodeflingInstance { get; set; }
		public static Vendor uModInstance { get; set; }
		public static Vendor Lone_DesignInstance { get; set; }
		public static Vendor LocalInstance { get; set; }

		public static Vendor GetVendor(VendorTypes vendor)
		{
			switch (vendor)
			{
				case VendorTypes.Codefling:
					return CodeflingInstance;

				case VendorTypes.uMod:
					return uModInstance;

				// case VendorTypes.Lone_Design:
				// 	return Lone_DesignInstance;

				case VendorTypes.Local:
					return LocalInstance;
			}

			return default;
		}

		public static Tab Get()
		{
			OsEx.Folder.Create(Path.Combine(Defines.GetScriptFolder(), "backups"));

			var tab = new Tab("plugins", "Plugins", Community.Runtime.CorePlugin, (ap, t) =>
			{
				ap.SetStorage(t, "selectedplugin", (Plugin)null);
				LocalInstance?.Refresh();
			}, 2)
			{
				Override = (t, cui, container, parent, ap) => Draw(cui, container, parent, t, ap)
			};

			CodeflingInstance = new Codefling();
			if (CodeflingInstance is IVendorStored cfStored && !cfStored.Load())
			{
				CodeflingInstance.FetchList();
				CodeflingInstance.Refresh();
			}

			uModInstance = new uMod();
			if (uModInstance is IVendorStored umodStored && !umodStored.Load())
			{
				uModInstance.FetchList();
				uModInstance.Refresh();
			}

			// Lone_DesignInstance = new Lone_Design();
			// if (!Lone_DesignInstance.Load())
			// {
			// 	Lone_DesignInstance.FetchList();
			// 	Lone_DesignInstance.Refresh();
			// }

			LocalInstance = new Local();
			LocalInstance.Refresh();

			ServerOwner.Load();

			return tab;
		}

		public static List<Plugin> GetPlugins(Vendor vendor, Tab tab, PlayerSession ap)
		{
			return GetPlugins(vendor, tab, ap, out _);
		}
		public static List<Plugin> GetPlugins(Vendor vendor, Tab tab, PlayerSession ap, out int maxPages)
		{
			maxPages = 0;

			var resultList = Facepunch.Pool.GetList<Plugin>();
			var customList = Facepunch.Pool.GetList<Plugin>();

			using (TimeMeasure.New("GetPluginsFromVendor", 1))
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

					maxPages = (customList.Count - 1) / 15;

					var page2 = ap.GetStorage(tab, "page", 0);
					if (page2 > maxPages) ap.SetStorage(tab, "page", maxPages);

					var page = 15 * page2;
					var count = (page + 15).Clamp(0, customList.Count);

					if (count > 0)
					{
						for (int i = page; i < count; i++)
						{
							try { resultList.Add(customList[i]); } catch { }
						}
					}
				}
				catch (Exception ex)
				{
					Facepunch.Pool.FreeList(ref resultList);

					Logger.Error($"Failed getting plugins.", ex);
				}

				Facepunch.Pool.FreeList(ref customList);
			}

			return resultList;
		}

		public static void DownloadThumbnails(Vendor vendor, Tab tab, PlayerSession ap)
		{
			var plugins = GetPlugins(vendor, tab, ap);

			var images = Facepunch.Pool.GetList<string>();
			var imagesSafe = Facepunch.Pool.GetList<string>();

			foreach (var element in plugins)
			{
				if (element.NoImage()) continue;

				if (element.HasInvalidImage())
				{
					imagesSafe.Add(element.Image);
					continue;
				}

				images.Add(element.Image);
			}

			var eraseAllBeforehand = false;

			if (images.Count > 0) Singleton.ImageDatabase.QueueBatchCallback(vendor.IconScale, eraseAllBeforehand, result => { }, images.ToArray());
			if (imagesSafe.Count > 0) Singleton.ImageDatabase.QueueBatch(vendor.SafeIconScale, eraseAllBeforehand, imagesSafe.ToArray());

			Facepunch.Pool.FreeList(ref plugins);
			Facepunch.Pool.FreeList(ref images);
			Facepunch.Pool.FreeList(ref imagesSafe);
		}

		public static void Draw(CUI cui, CuiElementContainer container, string parent, Tab tab, PlayerSession ap)
		{
			ap.SetDefaultStorage(tab, "vendor", "Local");

			var header = cui.CreatePanel(container, parent, null, "0.2 0.2 0.2 0.5",
				xMin: 0f, xMax: 1f, yMin: 0.95f, yMax: 1f);

			var vendorName = ap.GetStorage(tab, "vendor", "Local");
			var vendor = GetVendor((VendorTypes)Enum.Parse(typeof(VendorTypes), vendorName));

			var vendors = Enum.GetNames(typeof(VendorTypes));
			var cuts = 1f / vendors.Length;
			var offset = 0f;
			foreach (var value in vendors)
			{
				var v = GetVendor((VendorTypes)Enum.Parse(typeof(VendorTypes), value));
				cui.CreateProtectedButton(container, header, null, vendorName == value ? "0.3 0.72 0.25 0.8" : "0.2 0.2 0.2 0.3", "1 1 1 0.7", $"<b>{value.Replace("_", ".")}</b>{(v == null ? "" : $" — {v?.BarInfo}")}", 11,
					xMin: offset, xMax: offset + cuts, command: $"pluginbrowser.changetab {value}");
				offset += cuts;
			}

			var grid = cui.CreatePanel(container, parent, null, "0.2 0.2 0.2 0.3",
				xMin: 0f, xMax: 0.8f, yMin: 0, yMax: 0.95f);

			var spacing = 0.015f;
			var columnSize = 0.195f - spacing;
			var rowSize = 0.3f - spacing;
			var column = 0.02f;
			var row = 0f;
			var yOffset = 0.05f;

			var plugins = GetPlugins(vendor, tab, ap, out var maxPages);

			for (int i = 0; i < 15; i++)
			{
				if (i > plugins.Count() - 1) continue;

				var plugin = plugins.ElementAt(i);

				var card = cui.CreatePanel(container, grid, null, "0.2 0.2 0.2 0.4",
					xMin: column, xMax: column + columnSize, yMin: 0.69f + row - yOffset, yMax: 0.97f + row - yOffset);

				if (plugin.Owned)
				{
					cui.CreateImage(container, card, null, "glow", "1 1 1 0.5", OxMin: -20, OxMax: 20, OyMin: -20, OyMax: 20);
				}

				if (plugin.NoImage())
				{
					cui.CreateImage(container, card, null, vendor.Logo, "0.2 0.2 0.2 0.4", xMin: 0.2f, xMax: 0.8f, yMin: 0.2f + vendor.LogoRatio, yMax: 0.8f - vendor.LogoRatio);
				}
				else
				{
					if (Singleton.ImageDatabase.GetImage(plugin.Image) != 0) cui.CreateImage(container, card, null, plugin.Image, plugin.HasInvalidImage() ? vendor.SafeIconScale : vendor.IconScale, "1 1 1 1");
					else cui.CreateClientImage(container, card, null, plugin.Image, "1 1 1 1");
				}

				var cardTitle = cui.CreatePanel(container, card, null, "0 0 0 0.9", yMax: 0.25f);
				cui.CreatePanel(container, cardTitle, null, "0 0 0 0.2", blur: true);

				cui.CreateText(container, cardTitle, null, "1 1 1 1", plugin.Name, 11, xMin: 0.05f, yMax: 0.87f, align: TextAnchor.UpperLeft);
				cui.CreateText(container, cardTitle, null, "0.6 0.6 0.3 0.8", $"by <b>{plugin.Author}</b>", 9, xMin: 0.05f, yMin: 0.15f, align: TextAnchor.LowerLeft);
				cui.CreateText(container, cardTitle, null, "0.6 0.75 0.3 0.8", $"<b>{plugin.OriginalPrice}</b>", 11, xMax: 0.95f, yMin: 0.1f, align: TextAnchor.LowerRight);

				var shadowShift = -0.003f;
				cui.CreateText(container, card, null, "0 0 0 0.9", $"v{plugin.Version}", 9, xMax: 0.97f, yMax: 0.95f, OyMin: shadowShift, OxMin: shadowShift, OyMax: shadowShift, OxMax: shadowShift, align: TextAnchor.UpperRight);
				cui.CreateText(container, card, null, "0.6 0.75 0.3 1", $"v{plugin.Version}", 9, xMax: 0.97f, yMax: 0.95f, align: TextAnchor.UpperRight);

				var shadowShift2 = 0.1f;
				if (plugin.IsInstalled())
				{
					cui.CreateImage(container, card, null, "installed", "0 0 0 0.9", xMin: 0.04f, xMax: 0.16f, yMin: 0.83f, yMax: 0.95f, OyMin: shadowShift2, OxMin: -shadowShift2, OyMax: shadowShift2, OxMax: -shadowShift2);
					cui.CreateImage(container, card, null, "installed", plugin.IsUpToDate() ? "0.6 0.75 0.3 1" : "0.85 0.4 0.3 1", xMin: 0.04f, xMax: 0.16f, yMin: 0.83f, yMax: 0.95f);
				}

				cui.CreateProtectedButton(container, card, null, "0 0 0 0", "0 0 0 0", string.Empty, 0, command: $"pluginbrowser.selectplugin {plugin.Id}");

				var favouriteButton = cui.CreateProtectedButton(container, card, null, "0 0 0 0", "0 0 0 0", string.Empty, 0, xMin: 0.84f, xMax: 0.97f, yMin: 0.73f, yMax: 0.86f, command: $"pluginbrowser.interact 10 {plugin.Name}");
				cui.CreateImage(container, favouriteButton, null, "star", ServerOwner.Singleton.FavouritePlugins.Contains(plugin.Name) ? "0.9 0.8 0.4 0.95" : "0.2 0.2 0.2 0.4");

				var autoUpdateButton = cui.CreateProtectedButton(container, card, null, "0 0 0 0", "0 0 0 0", string.Empty, 0, xMin: 0.84f, xMax: 0.97f, yMin: 0.59f, yMax: 0.72f, command: $"pluginbrowser.interact 11 {plugin.Name}");
				cui.CreateImage(container, autoUpdateButton, null, "update-pending", ServerOwner.Singleton.AutoUpdate.Contains(plugin.Name) ? "0.8 0.4 0.9 0.95" : "0.2 0.2 0.2 0.4");

				column += columnSize + spacing;

				if (i % 5 == 4)
				{
					row -= rowSize + spacing;
					column = 0.02f;
				}
			}

			if (plugins.Count == 0)
			{
				cui.CreateText(container, parent, null, "1 1 1 0.5", "No plugins found for that query", 10, align: TextAnchor.MiddleCenter, yMax: 0.95f);
			}

			var sidebar = cui.CreatePanel(container, parent, null, "0.2 0.2 0.2 0.3",
				xMin: 0.80f, xMax: 1f, yMax: 0.93f);

			var topbar = cui.CreatePanel(container, parent, null, "0.1 0.1 0.1 0.7",
				xMin: 0f, xMax: 0.8f, yMin: 0.89f, yMax: 0.94f);

			var drop = cui.CreatePanel(container, sidebar, null, "0 0 0 0", yMin: 0.95f, OxMin: -155);
			Singleton.TabPanelDropdown(cui, PlaceboPage, container, drop, null, $"pluginbrowser.changesetting filter_dd", 1, 0, (int)ap.GetStorage(tab, "filter", FilterTypes.None), DropdownOptions, null, 0, DropdownShow);

			const float topbarYScale = 0.1f;
			cui.CreateText(container, topbar, null, "1 1 1 1", plugins.Count > 0 ? $"/ {maxPages + 1:n0}" : "NONE", plugins.Count > 0 ? 10 : 8, xMin: plugins.Count > 0 ? 0.925f : 0.92f, xMax: 0.996f, align: TextAnchor.MiddleLeft);
			if (plugins.Count != 0) cui.CreateProtectedInputField(container, topbar, null, "1 1 1 1", $"{ap.GetStorage(tab, "page", 0) + 1}", 10, 3, false, xMin: 0.8f, xMax: 0.92f, align: TextAnchor.MiddleRight, command: $"pluginbrowser.page ");
			cui.CreateProtectedButton(container, topbar, null, "0.4 0.7 0.3 0.8", "1 1 1 0.6", "<", 10, xMin: 0.86f, xMax: 0.886f, yMin: topbarYScale, yMax: 1f - topbarYScale, command: "pluginbrowser.page -1");
			cui.CreateProtectedButton(container, topbar, null, "0.4 0.7 0.3 0.8", "1 1 1 0.6", ">", 10, xMin: 0.97f, xMax: 0.996f, yMin: topbarYScale, yMax: 1f - topbarYScale, command: "pluginbrowser.page +1");

			var filterSectionOffset = 0.45f;
			var filterSectionOffsetSize = 0.1f;
			var tagsOption = cui.CreatePanel(container, sidebar, null, "0 0 0 0", yMin: filterSectionOffset - filterSectionOffsetSize, yMax: filterSectionOffset - 0.05f);
			var rowNumber = 0;
			var count = 0;
			var countPerRow = 3;
			var buttonSize = 1f / countPerRow;
			var buttonOffset = 0f;
			var rows = 7;
			foreach (var tag in PopularTags.Take(countPerRow * rows))
			{
				count++;

				cui.CreateProtectedButton(container, tagsOption, null, TagFilter.Contains(tag) ? "#b08d2e" : "0.2 0.2 0.2 0.5", "1 1 1 0.6", tag.ToUpper(), 8, buttonOffset, buttonOffset += buttonSize, OyMin: rowNumber * -25, OyMax: rowNumber * -25, command: $"pluginbrowser.tagfilter {tag}");

				if (count % countPerRow == 0)
				{
					buttonOffset = 0;
					rowNumber++;
				}
			}

			var auth = vendor as IVendorAuthenticated;

			if (auth != null)
			{
				var user = cui.CreatePanel(container, topbar, null, "0 0 0 0", xMax: 0.275f);

				if (auth.IsLoggedIn) cui.CreateClientImage(container, user, null, auth.User.AvatarUrl, "1 1 1 0.8", xMin: 0.02f, xMax: 0.12f, yMin: 0.1f, yMax: 0.9f);
				cui.CreateText(container, user, null, "1 1 1 0.9", auth.IsLoggedIn ? auth.User.DisplayName : "Not logged-in", 10, xMin: auth.IsLoggedIn ? 0.14f : 0.025f, yMax: 0.9f, align: TextAnchor.UpperLeft);
				cui.CreateText(container, user, null, "1 1 0.3 0.9", auth.IsLoggedIn ? $"as {auth.User.Authority}" : "Login to explore premium plugins!", 8, xMin: auth.IsLoggedIn ? 0.14f : 0.025f, yMin: 0.1f, align: TextAnchor.LowerLeft);
				cui.CreateProtectedButton(container, user, null, auth.IsLoggedIn ? "0.8 0.1 0 0.8" : "0.1 0.8 0 0.8", "1 1 1 0.5", auth.IsLoggedIn ? "<b>LOGOUT</b>" : "<b>LOGIN</b>", 8, xMin: 0.75f, xMax: 0.975f, command: "pluginbrowser.login");
			}

			var isLocal = vendor is Local;
			var searchQuery = ap.GetStorage<string>(tab, "search");
			var search = cui.CreatePanel(container, topbar, null, "0 0 0 0", xMin: 0.6f, xMax: 0.855f, yMin: 0f, OyMax: -0.5f);
			cui.CreateProtectedInputField(container, search, null, string.IsNullOrEmpty(searchQuery) ? "0.8 0.8 0.8 0.6" : "1 1 1 1", string.IsNullOrEmpty(searchQuery) ? "Search..." : searchQuery, 10, 20, false, xMin: 0.06f, align: TextAnchor.MiddleLeft, needsKeyboard: Singleton.HandleEnableNeedsKeyboard(ap), command: "pluginbrowser.search  ");
			cui.CreateProtectedButton(container, search, null, string.IsNullOrEmpty(searchQuery) ? "0.2 0.2 0.2 0.8" : "#d43131", "1 1 1 0.6", "X", 10, xMin: 0.95f, yMin: topbarYScale, yMax: 1f - topbarYScale, OxMin: -30, OxMax: -22.5f, command: "pluginbrowser.search  ");

			var reloadButton = cui.CreateProtectedButton(container, search, null, isLocal ? "0.2 0.2 0.2 0.4" : "0.2 0.2 0.2 0.8", "1 1 1 0.6", string.Empty, 0, xMin: 0.9f, xMax: 1, yMin: topbarYScale, yMax: 1f - topbarYScale, command: "pluginbrowser.refreshvendor");
			cui.CreateImage(container, reloadButton, null, "reload", "1 1 1 0.4", xMin: 0.225f, xMax: 0.775f, yMin: 0.25f, yMax: 0.75f);

			if (TagFilter.Contains("peanus")) cui.CreateClientImage(container, grid, null, "https://media.discordapp.net/attachments/1078801277565272104/1085062151221293066/15ox1d_1.jpg?width=827&height=675", "1 1 1 1", xMax: 0.8f);
			if (TagFilter.Contains("banan")) cui.CreateClientImage(container, grid, null, "https://cf-images.us-east-1.prod.boltdns.net/v1/static/507936866/2cd498e2-da08-4305-a86e-f9711ac41615/eac8316f-0061-40ed-b289-aac0bab35da0/1280x720/match/image.jpg", "1 1 1 1", xMax: 0.8f);

			var selectedPlugin = ap.GetStorage<Plugin>(tab, "selectedplugin");
			if (selectedPlugin != null)
			{
				vendor.CheckMetadata(selectedPlugin.Id, () => { Singleton.Draw(ap.Player); });

				var mainPanel = cui.CreatePanel(container, parent, null, "0.15 0.15 0.15 0.35", blur: true);
				cui.CreatePanel(container, mainPanel, null, "0 0 0 0.9");

				var image = cui.CreatePanel(container, parent, null, "0 0 0 0.5", xMin: 0.08f, xMax: 0.45f, yMin: 0.15f, yMax: 0.85f);

				if (selectedPlugin.NoImage()) cui.CreateImage(container, image, null, vendor.Logo, "0.2 0.2 0.2 0.4", xMin: 0.2f, xMax: 0.8f, yMin: 0.2f + vendor.LogoRatio, yMax: 0.8f - vendor.LogoRatio);
				{
					if (Singleton.ImageDatabase.GetImage(selectedPlugin.Image) == 0) cui.CreateClientImage(container, image, null, selectedPlugin.Image, "1 1 1 1", xMin: 0.05f, xMax: 0.95f, yMin: 0.05f, yMax: 0.95f);
					else cui.CreateImage(container, image, null, selectedPlugin.Image, selectedPlugin.HasInvalidImage() ? vendor.SafeIconScale : vendor.IconScale, "1 1 1 1", xMin: 0.05f, xMax: 0.95f, yMin: 0.05f, yMax: 0.95f);
				}
				cui.CreateText(container, mainPanel, null, "1 1 1 1", selectedPlugin.Name, 25, xMin: 0.505f, yMax: 0.8f, align: TextAnchor.UpperLeft, font: CUI.Handler.FontTypes.RobotoCondensedBold);
				cui.CreateText(container, mainPanel, null, "1 1 1 0.5", $"by <b>{selectedPlugin.Author}</b>  <b>•</b>  v{selectedPlugin.Version}  <b>•</b>  Updated on {selectedPlugin.UpdateDate}  <b>•</b>  {selectedPlugin.DownloadCount:n0} downloads", 11, xMin: 0.48f, yMax: 0.74f, align: TextAnchor.UpperLeft);
				cui.CreateText(container, mainPanel, null, "1 1 1 0.3", $"{(!selectedPlugin.HasLookup ? "Fetching metdata..." : $"{selectedPlugin.Description}\n\n{selectedPlugin.Changelog}")}", 11, xMin: 0.48f, xMax: 0.85f, yMax: 0.635f, align: TextAnchor.UpperLeft);

				cui.CreateProtectedButton(container, mainPanel, null, "0 0 0 0", "0 0 0 0", string.Empty, 0, align: TextAnchor.MiddleCenter, command: "pluginbrowser.deselectplugin");

				var favouriteButton = cui.CreateProtectedButton(container, mainPanel, null, "0 0 0 0", "0 0 0 0", string.Empty, 0, xMin: 0.48f, xMax: 0.495f, yMin: 0.755f, yMax: 0.785f, command: $"pluginbrowser.interact 10 {selectedPlugin.Name}");
				cui.CreateImage(container, favouriteButton, null, "star", ServerOwner.Singleton.FavouritePlugins.Contains(selectedPlugin.Name) ? "0.9 0.8 0.4 0.95" : "0.2 0.2 0.2 0.4");

				#region Tags

				var tagOffset = 0f;
				var tagSpacing = 0.012f;
				var tags = cui.CreatePanel(container, mainPanel, null, "0 0 0 0", xMin: 0.48f, xMax: 0.8f, yMin: 0.66f, yMax: 0.7f);
				var tempTags = Facepunch.Pool.GetList<string>();
				var counter = 0;

				foreach (var tag in selectedPlugin.Tags)
				{
					counter += tag.Length;
					tempTags.Add(tag);

					if (counter > 50)
					{
						break;
					}
				}

				foreach (var tag in tempTags)
				{
					if (string.IsNullOrEmpty(tag)) continue;

					var size = ((float)tag.Length).Scale(0, 5, 0.03f, 0.125f);
					var bg = cui.CreateProtectedButton(container, tags, null, TagFilter.Contains(tag) ? "0.8 0.3 0.3 0.8" : "0.2 0.2 0.2 0.4", "1 1 1 1", tag.ToUpper(), 8, xMin: tagOffset, xMax: tagOffset + size, command: $"pluginbrowser.tagfilter {tag}");

					tagOffset += size + tagSpacing;
				}

				Facepunch.Pool.FreeList(ref tempTags);

				#endregion

				cui.CreateProtectedButton(container, mainPanel, null, "#2f802f", "1 1 1 1", "<", 10, align: TextAnchor.MiddleCenter, command: "pluginbrowser.changeselectedplugin -1", xMin: 0f, xMax: 0.02f, yMin: 0.45f, yMax: 0.55f);
				cui.CreateProtectedButton(container, mainPanel, null, "#2f802f", "1 1 1 1", ">", 10, align: TextAnchor.MiddleCenter, command: "pluginbrowser.changeselectedplugin 1", xMin: 0.98f, xMax: 1f, yMin: 0.45f, yMax: 0.55f);

				cui.CreateProtectedButton(container, parent: mainPanel, id: null,
					color: "0.6 0.2 0.2 0.9",
					textColor: "1 0.5 0.5 1",
					text: "X", 9,
					xMin: 0.965f, xMax: 0.99f, yMin: 0.955f, yMax: 0.99f,
					command: "pluginbrowser.deselectplugin",
					font: CUI.Handler.FontTypes.DroidSansMono);

				if (Singleton.HasAccessLevel(ap.Player, 3))
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

					if (isAdmin || selectedPlugin.Owned || !selectedPlugin.IsPaid() || selectedPlugin.IsInstalled())
					{
						var button = cui.CreateProtectedButton(container, mainPanel, null, buttonColor, "0 0 0 0", string.Empty, 0, xMin: 0.48f, xMax: scale, yMin: 0.175f, yMax: 0.235f, align: TextAnchor.MiddleRight, command: selectedPlugin.IsBusy ? "" : $"pluginbrowser.interact {callMode} {selectedPlugin.Id}");
						cui.CreateText(container, button, null, "1 1 1 0.7", status, 11, xMax: 0.88f, align: TextAnchor.MiddleRight);
						cui.CreateImage(container, button, null, icon, elementColor, xMin: 0.1f, xMax: 0.3f, yMin: 0.2f, yMax: 0.8f);
					}
					if (selectedPlugin.IsInstalled())
					{
						var path = Path.Combine(Defines.GetConfigsFolder(), selectedPlugin.ExistentPlugin.Config.Filename);

						if (OsEx.File.Exists(path)) cui.CreateProtectedButton(container, mainPanel, null, "0.1 0.1 0.1 0.8", "1 1 1 0.7", "EDIT CONFIG", 11, xMin: 0.48f, xMax: 0.564f, yMin: 0.175f, yMax: 0.235f, OyMin: 35, OyMax: 35, command: selectedPlugin.IsBusy ? "" : $"pluginbrowser.interact 3 {selectedPlugin.Id}");
					}
				}
			}

			if (auth != null)
			{
				if (auth.IsLoggedIn && auth.User.PendingAccessToken)
				{
					var mainPanel = cui.CreatePanel(container, parent, null, "0.15 0.15 0.15 0.35", blur: true);
					cui.CreatePanel(container, mainPanel, null, "0 0 0 0.9");

					var image = cui.CreatePanel(container, parent, null, "1 1 1 1", xMin: 0.12f, xMax: 0.45f, yMin: 0.2f, yMax: 0.8f);

					cui.QueueImages(vendor.Logo);
					var code = string.Format(auth.AuthRequestEndpoint, auth.AuthCode);
					var qr = cui.CreateQRCodeImage(container, image, null, code,
						brandUrl: vendor.Logo,
						brandColor: "0 0 0 1",
						brandBgColor: "1 1 1 1", 15, true, true, "0 0 0 1", xMin: 0, xMax: 1, yMin: 0, yMax: 1);
					var authUrl = cui.CreatePanel(container, image, null, "0.1 0.1 0.1 0.8", yMax: 0, OyMin: -20);
					cui.CreateInputField(container, authUrl, null, "1 1 1 1", auth.AuthRequestEndpointPreview, 9, 0, true);

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
							cui.CreatePanel(container, image, null, "0 0 0 0.4", blur: true);
							cui.CreateImage(container, image, null, icon, color, xMin: 0.3f, xMax: 0.7f, yMin: 0.3f, yMax: 0.7f);
						}
					}

					cui.CreateText(container, parent, null, "1 1 1 1", $"{vendor.Type} Auth", 25, xMin: 0.51f, yMax: 0.75f, align: TextAnchor.UpperLeft, font: CUI.Handler.FontTypes.RobotoCondensedBold);
					cui.CreateText(container, parent, null, "1 1 1 0.5", $"Securely log into your {vendor.Type} account through OAuth-based login!\n\nScan the QR code or go to the URL, log into {vendor.Type} and type in the provided authentication code below to complete the login process.", 15, xMin: 0.51f, xMax: 0.9f, yMax: 0.67f, align: TextAnchor.UpperLeft);

					cui.CreateText(container, parent, null, "1 1 1 1", "Authorization code:", 13, xMin: 0.51f, yMax: 0.35f, align: TextAnchor.UpperLeft);
					var pan = cui.CreatePanel(container, parent, null, "0.1 0.1 0.1 1",
						xMin: 0.5f, xMax: 0.8f,
						yMin: 0.21f, yMax: 0.31f);

					cui.CreateInputField(container, pan, null, "1 1 1 1", auth.AuthCode, 30, 0, true,
						xMin: 0.05f,
						align: TextAnchor.MiddleLeft,
						font: CUI.Handler.FontTypes.RobotoCondensedBold);

					cui.CreateProtectedButton(container, parent: mainPanel, id: null,
						color: "0.6 0.2 0.2 0.9",
						textColor: "1 0.5 0.5 1",
						text: "X", 9,
						xMin: 0.965f, xMax: 0.99f, yMin: 0.955f, yMax: 0.99f,
						command: "pluginbrowser.closelogin",
						font: CUI.Handler.FontTypes.DroidSansMono);
				}
			}

			Facepunch.Pool.FreeList(ref plugins);
		}

		#region Vendors

		[ProtoContract]
		[ProtoInclude(100, typeof(Codefling))]
		[ProtoInclude(101, typeof(uMod))]
		[ProtoInclude(102, typeof(Lone_Design))]
		public abstract class Vendor
		{
			public virtual string Type { get; }
			public virtual string Url { get; }
			public virtual string Logo { get; }
			public virtual float LogoRatio { get; }

			public virtual float IconScale { get; }
			public virtual float SafeIconScale { get; }

			public virtual string BarInfo { get; }

			public Plugin[] PriceData { get; set; }
			public Plugin[] AuthorData { get; set; }
			public Plugin[] InstalledData { get; set; }
			public Plugin[] OutOfDateData { get; set; }
			public Plugin[] OwnedData { get; set; }
			public string[] PopularTags { get; set; }

			public virtual string ListEndpoint { get; }
			public virtual string DownloadEndpoint { get; }
			public virtual string PluginLookupEndpoint { get; }

			[ProtoMember(1)]
			public List<Plugin> FetchedPlugins { get; set; } = new();

			[ProtoMember(2)]
			public long LastTick { get; set; }

			public abstract void Refresh();
			public abstract void FetchList(Action<Vendor> callback = null);
			public abstract void Download(string id, Action onTimeout = null);
			public abstract void Uninstall(string id);
			public abstract void CheckMetadata(string id, Action onMetadataRetrieved);
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
			public int Id { get; set; }
			[ProtoMember(2)]
			public string Authority { get; set; }
			[ProtoMember(3)]
			public string DisplayName { get; set; }
			[ProtoMember(4)]
			public string AvatarUrl { get; set; }
			[ProtoMember(5)]
			public string AccessTokenEncoded { get; set; }
			[ProtoMember(6)]
			public string CoverUrl { get; set; }

			[ProtoMember(500)]
			public bool PendingAccessToken { get; set; }
			[ProtoMember(501)]
			public RequestResult PendingResult { get; set; }
			[ProtoMember(502)]
			public bool IsAdmin { get; set; }

			[ProtoMember(600)]
			public List<string> OwnedFiles { get; } = new();

			public string AccessToken { get; set; }

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

			public override float IconScale => 0.4f;
			public override float SafeIconScale => 0.2f;

			public override string BarInfo => $"{FetchedPlugins.Count(x => !x.IsPaid()):n0} free, {FetchedPlugins.Count(x => x.IsPaid()):n0} paid";

			public override string ListEndpoint => "https://codefling.com/capi/category-2/?do=apicall";
			public override string DownloadEndpoint => "https://codefling.com/files/file/[ID]-a?do=download";

			public override void Refresh()
			{
				if (FetchedPlugins == null) return;

				var plugins = Community.Runtime.CorePlugin.plugins.GetAll();
				var auth = this as IVendorAuthenticated;

				foreach (var plugin in FetchedPlugins)
				{
					var name = plugin.File;
					var length = 0;

					if (!string.IsNullOrEmpty(name) && (length = name.LastIndexOf('.')) != 0)
					{
						name = name.Substring(0, length);
					}

					plugin.Owned = auth.User != null && auth.User.OwnedFiles.Contains(plugin.Id);

					foreach (var existentPlugin in plugins)
					{
						if (existentPlugin.FileName == name)
						{
							plugin.ExistentPlugin = (RustPlugin)existentPlugin;
							break;
						}
					}
				}

				Array.Clear(plugins, 0, plugins.Length);
				plugins = null;

				if (PriceData != null)
				{
					Array.Clear(PriceData, 0, PriceData.Length);
					Array.Clear(AuthorData, 0, AuthorData.Length);
					Array.Clear(InstalledData, 0, InstalledData.Length);
					Array.Clear(OwnedData, 0, OwnedData.Length);
					PriceData = AuthorData = InstalledData = OwnedData = null;
				}

				PriceData = FetchedPlugins.Where(x => x.Status == Status.Approved).OrderBy(x => x.OriginalPrice).ToArray();
				AuthorData = FetchedPlugins.Where(x => x.Status == Status.Approved).OrderBy(x => x.Author).ToArray();
				InstalledData = FetchedPlugins.Where(x => x.IsInstalled()).ToArray();
				OutOfDateData = FetchedPlugins.Where(x => x.Status == Status.Approved).Where(x => x.IsInstalled() && !x.IsUpToDate()).ToArray();
				OwnedData = FetchedPlugins.Where(x => x.Owned).ToArray();

				var tags = Facepunch.Pool.GetList<string>();
				foreach (var plugin in FetchedPlugins)
				{
					if (plugin.Tags == null || plugin.Tags.Length == 0) continue;

					foreach (var tag in plugin.Tags)
					{
						var processedTag = tag?.ToLower()?.Trim();

						if (!string.IsNullOrEmpty(processedTag) && !tags.Contains(processedTag) && processedTag != "{}")
						{
							tags.Add(processedTag);
						}
					}
				}
				PopularTags = tags.ToArray();
				Facepunch.Pool.FreeList(ref tags);
			}
			public override void FetchList(Action<Vendor> callback = null)
			{
				Community.Runtime.CorePlugin.webrequest.Enqueue(ListEndpoint, null, (error, data) =>
				{
					try
					{
						var list = JObject.Parse(data);

						FetchedPlugins.Clear();

						var plugins = Community.Runtime.CorePlugin.plugins.GetAll();
						var file = list["file"];
						foreach (var token in file)
						{
							var fileStatus = token["file_status"]?.ToString();
							var plugin = new Plugin
							{
								Id = token["file_id"]?.ToString(),
								Name = token["file_name"]?.ToString(),
								Author = token["file_author"]?.ToString(),
								Description = token["file_description"]?.ToString(),
								Version = token["file_version"]?.ToString(),
								OriginalPrice = token["file_price"]?.ToString(),
								UpdateDate = token["file_updated"]?.ToString(),
								Changelog = token["file_changelogs"]?.ToString(),
								File = token["file_file_1"]?.ToString(),
								Image = token["file_image"]["url"]?.ToString(),
								ImageSize = (token["file_image"]["size"]?.ToString().ToInt()).GetValueOrDefault(),
								Tags = token["file_tags"]?.ToString().Split(','),
								DownloadCount = (token["file_downloads"]?.ToString().ToInt()).GetValueOrDefault(),
								Dependencies = token["file_depends"]?.ToString().Split(),
								CarbonCompatible = (token["file_compatibility"]?.ToString().ToBool()).GetValueOrDefault(),
								Rating = (token["file_rating"]?.ToString().ToFloat()).GetValueOrDefault(0),
								Status = string.IsNullOrEmpty(fileStatus) ? Status.Approved : (Status)fileStatus.ToInt(),
								HasLookup = true
							};

							var date = DateTimeOffset.FromUnixTimeSeconds(plugin.UpdateDate.ToLong());
							plugin.UpdateDate = date.UtcDateTime.ToString();

							try { plugin.Description = plugin.Description.TrimStart('\t').Replace("\t", "\n").Split('\n')[0]; } catch { }

							if (plugin.OriginalPrice == "{}") plugin.OriginalPrice = "FREE";
							try { plugin.ExistentPlugin = plugins.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.FilePath) == Path.GetFileNameWithoutExtension(plugin.File)) as RustPlugin; } catch { }

							FetchedPlugins.Add(plugin);
						}

						callback?.Invoke(this);
						Logger.Log($"[{Type}] Downloaded JSON");

						Save();
					}
					catch (Exception ex)
					{
						Logger.Error($" Couldn't fetch Codefling API to get the plugins list. Most likely because it's down.", ex);
					}
				}, Community.Runtime.CorePlugin);
			}
			public override void Download(string id, Action onTimeout = null)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				plugin.IsBusy = true;
				plugin.DownloadCount++;

				var core = Community.Runtime.CorePlugin;

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
					var headers = new Dictionary<string, string>
					{
						[AuthHeader.Key.ToString()] = string.Format(AuthHeader.Value, User.AccessToken)
					};
					var extension = Path.GetExtension(plugin.File);

					switch (extension)
					{
						case ".zip":
							core.webrequest.Enqueue(string.Format(AuthDownloadFileEndpoint, plugin.Id), null, (error, source) =>
							{
								var jobject = JObject.Parse(source);
								var name = jobject["files"][0]["name"].ToString();
								var file = jobject["files"][0]["url"].ToString();
								var path = Path.Combine(Defines.GetScriptFolder(), name);
								jobject = null;

								core.webrequest.EnqueueData(file, null, (error, source) =>
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

													OsEx.File.Create(Path.Combine(Defines.GetScriptFolder(), file.Name), fileSource);
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
								}, core, headers: headers);

							}, core, headers: headers);
							break;

						case ".cs":
							core.webrequest.Enqueue(string.Format(AuthDownloadFileEndpoint, plugin.Id), null, (error, source) =>
							{
								var jobject = JObject.Parse(source);
								var name = jobject["files"][0]["name"].ToString();
								var file = jobject["files"][0]["url"].ToString();
								var path = Path.Combine(Defines.GetScriptFolder(), name);
								jobject = null;

								core.webrequest.Enqueue(file, null, (error, source) =>
								{
									plugin.IsBusy = false;

									Singleton.Puts($"Downloaded {plugin.Name}");
									OsEx.File.Create(path, source);
								}, core, headers: headers);

							}, core, headers: headers);
							break;
					}
				}
				else
				{
					var path = Path.Combine(Defines.GetScriptFolder(), plugin.File);
					var url = DownloadEndpoint.Replace("[ID]", id);

					core.webrequest.Enqueue(url, null, (error, source) =>
					{
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
				OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Defines.GetScriptFolder(), "backups", $"{plugin.ExistentPlugin.FileName}.cs"), true);
				plugin.ExistentPlugin = null;
			}
			public override void CheckMetadata(string id, Action onMetadataRetrieved)
			{

			}

			#region Auth

			[ProtoBuf.ProtoMember(50, IsRequired = false)]
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
				var core = Community.Runtime.CorePlugin;

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
						var jobject = JObject.Parse(result);
						User.AccessToken = jobject["accesstoken"].ToString();
						User.AccessTokenEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(User.AccessToken));
						ValidationTimer.Destroy();
						ValidationTimer = null;
						jobject = null;

						User.PendingResult = LoggedInUser.RequestResult.Complete;
						onComplete?.Invoke();
					}, null, onException: (code, str, ex) =>
					{
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
						}
					});
				});
			}
			public void RefreshUser(PlayerSession session)
			{
				if (!IsLoggedIn) return;

				var core = Community.Runtime.CorePlugin;
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
					}
				}, core, headers: headers, onException: (code, result, ex) =>
				{
					User = null;
					Refresh();
					Save();
					Singleton.Draw(session.Player);
				});
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

					using var file = File.OpenRead(path);
					var value = Serializer.Deserialize<Codefling>(file);

					LastTick = value.LastTick;
					FetchedPlugins.Clear();
					FetchedPlugins.AddRange(value.FetchedPlugins);
					User = value.User;

					if (User != null && !string.IsNullOrEmpty(User.AccessTokenEncoded))
					{
						User.AccessToken = Encoding.UTF8.GetString(Convert.FromBase64String(User.AccessTokenEncoded));
					}

					if ((DateTime.Now - new DateTime(value.LastTick)).TotalHours >= 24)
					{
						Singleton.Puts($"Invalidated {Type} database. Fetching...");
						return false;
					}

					Singleton.Puts($"Loaded {Type} from file: {path}");
					Refresh();
				}
				catch (Exception ex)
				{
					Logger.Error($"{Type}.Load error", ex);
				}
				return true;
			}
			public void Save()
			{
				try
				{
					var path = Path.Combine(Defines.GetDataFolder(), "vendordata_cf.db");
					using var file = File.OpenWrite(path);
					LastTick = DateTime.Now.Ticks;
					Serializer.Serialize(file, this);
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

			public override float IconScale => 1f;
			public override float SafeIconScale => 1f;

			public override string BarInfo => $"{FetchedPlugins.Count:n0} free";

			public override string ListEndpoint => "https://umod.org/plugins/search.json?page=[ID]&sort=title&sortdir=asc&categories%5B0%5D=universal&categories%5B1%5D=rust";
			public override string DownloadEndpoint => "https://umod.org/plugins/[ID].cs";
			public override string PluginLookupEndpoint => "https://umod.org/plugins/[ID]/latest.json";

			public override void Refresh()
			{
				if (FetchedPlugins == null) return;

				var plugins = Community.Runtime.CorePlugin.plugins.GetAll();

				foreach (var plugin in FetchedPlugins)
				{
					var name = plugin.File.Substring(0, plugin.File.IndexOf(".cs"));

					foreach (var existentPlugin in plugins)
					{
						if (existentPlugin.FileName == name)
						{
							plugin.ExistentPlugin = (RustPlugin)existentPlugin;
							break;
						}
					}
				}

				Array.Clear(plugins, 0, plugins.Length);
				plugins = null;

				if (PriceData != null)
				{
					Array.Clear(PriceData, 0, PriceData.Length);
					Array.Clear(AuthorData, 0, AuthorData.Length);
					Array.Clear(InstalledData, 0, InstalledData.Length);
					Array.Clear(OwnedData, 0, OwnedData.Length);
					PriceData = AuthorData = InstalledData = OwnedData = null;
				}

				PriceData = FetchedPlugins.OrderBy(x => x.OriginalPrice).ToArray();
				AuthorData = FetchedPlugins.OrderBy(x => x.Author).ToArray();
				InstalledData = FetchedPlugins.Where(x => x.IsInstalled()).ToArray();
				OutOfDateData = FetchedPlugins.Where(x => x.IsInstalled() && !x.IsUpToDate()).ToArray();
				OwnedData = FetchedPlugins.Where(x => x.Owned).ToArray();

				var tags = Facepunch.Pool.GetList<string>();
				foreach (var plugin in FetchedPlugins)
				{
					foreach (var tag in plugin.Tags)
					{
						var processedTag = tag.ToLower().Trim();

						if (!tags.Contains(processedTag))
						{
							tags.Add(processedTag);
						}
					}
				}
				PopularTags = tags.ToArray();
				Facepunch.Pool.FreeList(ref tags);
			}
			public override void FetchList(Action<Vendor> callback = null)
			{
				FetchedPlugins.Clear();

				Community.Runtime.CorePlugin.webrequest.Enqueue(ListEndpoint.Replace("[ID]", "0"), null, (error, data) =>
				{
					var list = JObject.Parse(data);

					FetchPage(0, (list["last_page"]?.ToString().ToInt()).GetValueOrDefault(), callback);
					list = null;
				}, Community.Runtime.CorePlugin);
			}
			public override void Download(string id, Action onTimeout = null)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				var path = Path.Combine(Defines.GetScriptFolder(), plugin.File);
				var url = DownloadEndpoint.Replace("[ID]", plugin.Name);

				plugin.IsBusy = true;

				Community.Runtime.CorePlugin.timer.In(2f, () =>
				{
					if (plugin.IsBusy)
					{
						plugin.IsBusy = false;
						onTimeout?.Invoke();
					}
				});

				Community.Runtime.CorePlugin.webrequest.Enqueue(url, null, (error, source) =>
				{
					Singleton.Puts($"Downloaded {plugin.Name}");
					OsEx.File.Create(path, source);

					plugin.IsBusy = false;
					plugin.DownloadCount++;

				}, Community.Runtime.CorePlugin, headers: new Dictionary<string, string>
				{
					["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.63",
					["accept"] = "ext/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
				});
			}
			public override void Uninstall(string id)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Defines.GetScriptFolder(), "backups", $"{plugin.ExistentPlugin.FileName}.cs"), true);
				plugin.ExistentPlugin = null;
			}
			public override void CheckMetadata(string id, Action onMetadataRetrieved)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				if (plugin.HasLookup) return;

				Community.Runtime.CorePlugin.webrequest.Enqueue(PluginLookupEndpoint.Replace("[ID]", plugin.Name.ToLower().Trim()), null, (error, data) =>
				{
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
				}, Community.Runtime.CorePlugin);
			}

			public void FetchPage(int page, int maxPage, Action<Vendor> callback = null)
			{
				if (page > maxPage)
				{
					Save();
					callback?.Invoke(this);
					return;
				}

				var plugins = Community.Runtime.CorePlugin.plugins.GetAll();

				Community.Runtime.CorePlugin.webrequest.Enqueue(ListEndpoint.Replace("[ID]", $"{page}"), null, (error, data) =>
				{
					var list = JObject.Parse(data);
					var file = list["data"];
					foreach (var plugin in file)
					{
						var p = new Plugin
						{
							Id = plugin["url"]?.ToString(),
							Name = plugin["name"]?.ToString(),
							Author = plugin["author"]?.ToString(),
							Version = plugin["latest_release_version"]?.ToString(),
							Description = plugin["description"]?.ToString(),
							OriginalPrice = "FREE",
							File = $"{plugin["name"]?.ToString()}.cs",
							Image = plugin["icon_url"]?.ToString(),
							ImageSize = 0,
							DownloadCount = (plugin["downloads"]?.ToString().ToInt()).GetValueOrDefault(),
							UpdateDate = plugin["updated_at"]?.ToString(),
							Tags = plugin["tags_all"]?.ToString().Split(',')
						};

						if (!string.IsNullOrEmpty(p.Description) && !p.Description.EndsWith(".")) p.Description += ".";

						if (string.IsNullOrEmpty(p.Author.Trim())) p.Author = "Unmaintained";
						if (p.OriginalPrice == "{}") p.OriginalPrice = "FREE";
						try { p.ExistentPlugin = plugins.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.FilePath) == Path.GetFileNameWithoutExtension(p.File)) as RustPlugin; } catch { }

						if (!FetchedPlugins.Any(x => x.Name == p.Name)) FetchedPlugins.Add(p);
					}

					if (page % (maxPage / 4) == 0 || page == maxPage - 1)
					{
						Logger.Log($"[{Type}] Downloaded {page} out of {maxPage}");
					}

					list = null;
				}, Community.Runtime.CorePlugin);
				Community.Runtime.CorePlugin.timer.In(5f, () => FetchPage(page + 1, maxPage, callback));
			}

			public bool Load()
			{
				try
				{
					var path = Path.Combine(Defines.GetDataFolder(), "vendordata_umod.db");
					if (!OsEx.File.Exists(path)) return false;

					using var file = File.OpenRead(path);
					var value = Serializer.Deserialize<uMod>(file);

					LastTick = value.LastTick;
					FetchedPlugins.Clear();
					FetchedPlugins.AddRange(value.FetchedPlugins);

					if ((DateTime.Now - new DateTime(value.LastTick)).TotalHours >= 24)
					{
						Singleton.Puts($"Invalidated {Type} database. Fetching...");
						return false;
					}

					Singleton.Puts($"Loaded {Type} from file: {path}");
					Refresh();
				}
				catch { Save(); }
				return true;
			}
			public void Save()
			{
				try
				{
					var path = Path.Combine(Defines.GetDataFolder(), "vendordata_umod.db");
					using var file = File.OpenWrite(path);

					LastTick = DateTime.Now.Ticks;
					Serializer.Serialize(file, this);
					Singleton.Puts($"Stored {Type} to file: {path}");
				}
				catch (Exception ex)
				{
					Singleton.PutsError($" Couldn't store uMod plugins list.", ex);
				}
			}
		}

		[ProtoContract]
		public class Lone_Design : Vendor, IVendorStored
		{
			public override string Type => "Lone.Design";
			public override string Url => "https://lone.design";
			public override string Logo => "lonelogo";
			public override float LogoRatio => 0f;

			public override float IconScale => 0.4f;
			public override float SafeIconScale => 0.2f;

			public override string BarInfo => $"{FetchedPlugins.Count(x => !x.IsPaid()):n0} free, {FetchedPlugins.Count(x => x.IsPaid()):n0} paid";

			public override string ListEndpoint => "https://api.lone.design/carbon.json";
			public override string DownloadEndpoint => "https://codefling.com/files/file/[ID]-a?do=download";

			public override void Refresh()
			{
				if (FetchedPlugins == null) return;

				foreach (var plugin in FetchedPlugins)
				{
					foreach (var existentPlugin in Community.Runtime.CorePlugin.plugins.GetAll())
					{
						if (existentPlugin.FileName == plugin.File)
						{
							plugin.ExistentPlugin = (RustPlugin)existentPlugin;
							break;
						}
					}
				}

				if (PriceData != null)
				{
					Array.Clear(PriceData, 0, PriceData.Length);
					Array.Clear(AuthorData, 0, AuthorData.Length);
					Array.Clear(InstalledData, 0, InstalledData.Length);
					Array.Clear(OwnedData, 0, OwnedData.Length);
					PriceData = AuthorData = InstalledData = OwnedData = null;
				}

				PriceData = FetchedPlugins.OrderBy(x => x.OriginalPrice).ToArray();
				AuthorData = FetchedPlugins.OrderBy(x => x.Author).ToArray();
				InstalledData = FetchedPlugins.Where(x => x.IsInstalled()).ToArray();
				OutOfDateData = FetchedPlugins.Where(x => x.IsInstalled() && !x.IsUpToDate()).ToArray();
				OwnedData = FetchedPlugins.Where(x => x.Owned).ToArray();

				var tags = Facepunch.Pool.GetList<string>();
				foreach (var plugin in FetchedPlugins)
				{
					if (plugin.Tags == null) continue;

					foreach (var tag in plugin.Tags)
					{
						var processedTag = tag?.ToLower().Trim();

						if (!tags.Contains(processedTag))
						{
							tags.Add(processedTag);
						}
					}
				}
				PopularTags = tags.ToArray();
				Facepunch.Pool.FreeList(ref tags);
			}
			public override void FetchList(Action<Vendor> callback = null)
			{
				var plugins = Community.Runtime.CorePlugin.plugins.GetAll();

				Community.Runtime.CorePlugin.webrequest.Enqueue(ListEndpoint, null, (error, data) =>
				{
					var list = JToken.Parse(data);

					FetchedPlugins.Clear();

					foreach (var token in list)
					{
						var plugin = new Plugin
						{
							Id = token["url"]?.ToString(),
							Name = token["name"]?.ToString(),
							Author = token["author"]?.ToString(),
							Description = token["description"]?.ToString(),
							Version = token["version"]?.ToString(),
							OriginalPrice = $"${token["price"]?.ToString()}",
							SalePrice = $"${token["salePrice"]?.ToString()}",
							File = token["filename"]?.ToString(),
							Image = token["images"][0]["src"]?.ToString(),
							Tags = token["tags"]?.Select(x => x["name"]?.ToString())?.ToArray(),
							HasLookup = true
						};

						if (plugin.OriginalPrice == "$" || plugin.OriginalPrice == "$0") plugin.OriginalPrice = "FREE";
						if (plugin.SalePrice == "$" || plugin.SalePrice == "$0") plugin.SalePrice = "FREE";

						var date = DateTimeOffset.FromUnixTimeSeconds(plugin.UpdateDate.ToLong());
						plugin.UpdateDate = date.UtcDateTime.ToString();

						try { plugin.Description = plugin.Description.TrimStart('\t').Replace("\t", "\n").Split('\n')[0]; } catch { }

						try { plugin.ExistentPlugin = plugins.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.FilePath) == Path.GetFileNameWithoutExtension(plugin.File)) as RustPlugin; } catch { }

						FetchedPlugins.Add(plugin);
					}

					callback?.Invoke(this);
					Logger.Log($"[{Type}] Downloaded JSON");

					Save();
				}, Community.Runtime.CorePlugin);
			}
			public override void Download(string id, Action onTimeout = null)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				plugin.IsBusy = true;
				plugin.DownloadCount++;

				var path = Path.Combine(Defines.GetScriptFolder(), plugin.File);
				var url = DownloadEndpoint.Replace("[ID]", id);

				Community.Runtime.CorePlugin.timer.In(2f, () =>
				{
					if (plugin.IsBusy)
					{
						plugin.IsBusy = false;
						onTimeout?.Invoke();
					}
				});

				Community.Runtime.CorePlugin.webrequest.Enqueue(url, null, (error, source) =>
				{
					plugin.IsBusy = false;

					if (!source.StartsWith("<!DOCTYPE html>"))
					{
						Singleton.Puts($"Downloaded {plugin.Name}");
						OsEx.File.Create(path, source);
					}
				}, Community.Runtime.CorePlugin, headers: new Dictionary<string, string>
				{
					["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.63",
					["accept"] = "ext/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
				});
			}
			public override void Uninstall(string id)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Defines.GetScriptFolder(), "backups", $"{plugin.ExistentPlugin.FileName}.cs"), true);
				plugin.ExistentPlugin = null;
			}
			public override void CheckMetadata(string id, Action onMetadataRetrieved)
			{

			}

			public bool Load()
			{
				try
				{
					var path = Path.Combine(Defines.GetDataFolder(), "vendordata_lone.db");
					if (!OsEx.File.Exists(path)) return false;

					using var file = File.OpenRead(path);
					var value = Serializer.Deserialize<Codefling>(file);

					LastTick = value.LastTick;
					FetchedPlugins.Clear();
					FetchedPlugins.AddRange(value.FetchedPlugins);

					if ((DateTime.Now - new DateTime(value.LastTick)).TotalHours >= 24)
					{
						Singleton.Puts($"Invalidated {Type} database. Fetching...");
						return false;
					}

					Singleton.Puts($"Loaded {Type} from file: {path}");
					Refresh();
				}
				catch { Save(); }
				return true;
			}
			public void Save()
			{
				try
				{
					var path = Path.Combine(Defines.GetDataFolder(), "vendordata_lone.db");
					using var file = File.OpenWrite(path);
					LastTick = DateTime.Now.Ticks;
					Serializer.Serialize(file, this);
					Singleton.Puts($"Stored {Type} to file: {path}");
				}
				catch { }
			}
		}

		#endregion

		[ProtoContract]
		public class Local : Vendor
		{
			public override string Type => "All";
			public override string Url => "none";
			public override string Logo => "carbonw";

			public override float LogoRatio => 0.23f;
			public override string ListEndpoint => string.Empty;
			public override string DownloadEndpoint => string.Empty;
			public override string BarInfo => $"{FetchedPlugins.Count:n0} loaded";
			public override float IconScale => 0.4f;
			public override float SafeIconScale => 0.2f;


			internal string[] _defaultTags = new string[] { "carbon", "oxide" };

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

				foreach (var package in ModLoader.LoadedPackages)
				{
					foreach (var plugin in package.Plugins)
					{
						if (plugin.IsCorePlugin) continue;

						var existent = FetchedPlugins.FirstOrDefault(x => x.ExistentPlugin == plugin);

						if (existent == null) FetchedPlugins.Add(CodeflingInstance.FetchedPlugins.FirstOrDefault(x => x.ExistentPlugin == plugin) ??
							uModInstance.FetchedPlugins.FirstOrDefault(x => x.ExistentPlugin == plugin)
							?? (existent = new Plugin
							{
								Name = plugin.Name,
								Author = plugin.Author,
								Version = plugin.Version.ToString(),
								ExistentPlugin = plugin,
								Description = "This is an unlisted plugin.",
								Tags = _defaultTags,
								File = plugin.FileName,
								Id = plugin.Name,
								UpdateDate = DateTime.UtcNow.ToString()
							}));
					}
				}

				FetchedPlugins = FetchedPlugins.OrderBy(x => x.Name).ToList();
				PriceData = FetchedPlugins.OrderBy(x => x.OriginalPrice).ToArray();
				AuthorData = FetchedPlugins.OrderBy(x => x.Author).ToArray();
				InstalledData = FetchedPlugins.Where(x => x.IsInstalled()).ToArray();
				OutOfDateData = FetchedPlugins.Where(x => x.IsInstalled() && !x.IsUpToDate()).ToArray();
				OwnedData = FetchedPlugins.OrderBy(x => x.Owned).ToArray();
			}

			public void Save()
			{
			}

			public override void Uninstall(string id)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Defines.GetScriptFolder(), "backups", $"{plugin.ExistentPlugin.FileName}.cs"), true);
				plugin.ExistentPlugin = null;
			}
		}

		[ProtoContract]
		public class ServerOwner
		{
			public static ServerOwner Singleton { get; internal set; } = new ServerOwner();

			[ProtoMember(1)]
			public List<string> FavouritePlugins { get; set; } = new();

			[ProtoMember(2)]
			public List<string> AutoUpdate { get; set; } = new();

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

					using var file = File.OpenRead(path);
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
					using var file = File.OpenWrite(path);

					Serializer.Serialize(file, Singleton);
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
			public string Id { get; set; }
			public string Name { get; set; }
			public string Author { get; set; }
			public string Version { get; set; }
			public string Description { get; set; }
			public string Changelog { get; set; }
			public string OriginalPrice { get; set; }
			public string SalePrice { get; set; }
			public string[] Dependencies { get; set; }
			public string File { get; set; }
			public string Image { get; set; }
			public int ImageSize { get; set; }
			public string[] Tags { get; set; }
			public int DownloadCount { get; set; }
			public float Rating { get; set; }
			public string UpdateDate { get; set; }
			public bool HasLookup { get; set; } = false;
			public Status Status { get; set; } = Status.Approved;
			public bool CarbonCompatible { get; set; } = false;
			public bool Owned { get; set; }

			internal RustPlugin ExistentPlugin { get; set; }
			internal bool IsBusy { get; set; }

			public bool HasInvalidImage()
			{
				return ImageSize >= 2504304 || Image.EndsWith(".gif");
			}
			public bool NoImage()
			{
				return string.IsNullOrEmpty(Image) || Image.EndsWith(".gif");
			}
			public bool IsInstalled()
			{
				return ExistentPlugin != null;
			}
			public string CurrentVersion()
			{
				if (ExistentPlugin == null) return "N/A";

				return ExistentPlugin.Version.ToString();
			}
			public bool IsPaid()
			{
				return OriginalPrice != "FREE";
			}
			public bool IsUpToDate()
			{
				if (!IsInstalled()) return false;

				return ExistentPlugin.Version.ToString() == Version;
			}
		}
	}

	#region Commands

	[ProtectedCommand("pluginbrowser.changetab")]
	private void PluginBrowserChange(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor2 = ap.SetStorage(tab, "vendor", args.Args[0]);

		var vendor = GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), vendor2));
		vendor.Refresh();
		TagFilter.Clear();

		DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));
		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.interact")]
	private void PluginBrowserInteract(Arg args)
	{
		var player = args.Player();

		if (!Singleton.HasAccessLevel(player, 3)) return;

		var ap = Singleton.GetPlayerSession(player);
		var tab = Singleton.GetTab(ap.Player);

		var vendor = GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage(tab, "vendor", "Local")));
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
				var plugin = vendor.FetchedPlugins.FirstOrDefault(x => x.Id == args.Args[1]).ExistentPlugin;
				var path = Path.Combine(Defines.GetConfigsFolder(), plugin.Config.Filename);
				Singleton.SetTab(ap.Player, ConfigEditor.Make(OsEx.File.ReadText(path),
					(ap, jobject) =>
					{
						Community.Runtime.CorePlugin.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
					},
					(ap, jobject) =>
					{
						OsEx.File.Create(path, jobject.ToString(Formatting.Indented));
						Community.Runtime.CorePlugin.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
					},
					(ap, jobject) =>
					{
						OsEx.File.Create(path, jobject.ToString(Formatting.Indented));
						plugin.ProcessorInstance.SetDirty();
						Community.Runtime.CorePlugin.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
					}));
				Array.Clear(arg, 0, arg.Length);
				break;

			case "10":
				{
					var pluginName = arg.Skip(1).ToString(" ");
					if (ServerOwner.Singleton.FavouritePlugins.Contains(pluginName))
					{
						ServerOwner.Singleton.FavouritePlugins.Remove(pluginName);
						Logger.Log($" [{vendor.Type}] Unfavorited plugin '{pluginName}'");
					}
					else
					{
						ServerOwner.Singleton.FavouritePlugins.Add(pluginName);
						Logger.Log($" [{vendor.Type}] Favorited plugin '{pluginName}'");
					}
					Array.Clear(arg, 0, arg.Length);
				}
				break;

			case "11":
				{
					var pluginName = arg.Skip(1).ToString(" ");
					if (ServerOwner.Singleton.AutoUpdate.Contains(pluginName))
					{
						ServerOwner.Singleton.AutoUpdate.Remove(pluginName);
						Logger.Log($" [{vendor.Type}] Marked plugin '{pluginName}' for auto-update");
					}
					else
					{
						ServerOwner.Singleton.AutoUpdate.Add(pluginName);
						Logger.Log($" [{vendor.Type}] Unmarked plugin '{pluginName}' for auto-update");
					}
					Array.Clear(arg, 0, arg.Length);
				}
				break;

		}

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.page")]
	private void PluginBrowserPage(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);

		var vendor = GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage(tab, "vendor", "Local")));
		vendor.Refresh();
		GetPlugins(vendor, tab, ap, out var maxPages);

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

		DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.tagfilter")]
	private void PluginBrowserTagFilter(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);

		var vendor = GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage(tab, "vendor", "Local")));
		vendor.Refresh();

		var filter = args.Args.ToString(" ");

		if (TagFilter.Contains(filter)) TagFilter.Remove(filter);
		else TagFilter.Add(filter);

		DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.search")]
	private void PluginBrowserSearch(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);

		var vendor = GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage(tab, "vendor", "Local")));
		vendor.Refresh();

		var search = ap.SetStorage(tab, "search", args.Args.ToString(" "));
		var page = ap.SetStorage(tab, "page", 0);

		if (search == "Search...") ap.SetStorage(tab, "search", string.Empty);

		DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.refreshvendor")]
	private void PluginBrowserRefreshVendor(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);

		var vendor = GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage(tab, "vendor", "Local")));

		if (vendor is PluginsTab.Local) return;

		tab.CreateDialog("Are you sure you want to redownload the plugin list?\nThis might take a while.", ap =>
		{
			var id = string.Empty;
			switch (vendor)
			{
				case Codefling:
					id = "cf";
					break;

				case uMod:
					id = "umod";
					break;
			}

			var dataPath = Path.Combine(Defines.GetDataFolder(), $"vendordata_{id}.db");
			OsEx.File.Delete(dataPath);

			if (vendor is IVendorStored stored && !stored.Load())
			{
				vendor.FetchList();
				vendor.Refresh();
			}
			if (vendor is IVendorAuthenticated auth)
			{
				auth.RefreshUser(ap);
			}

			Singleton.Draw(args.Player());
		}, null);

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.selectplugin")]
	private void PluginBrowserSelectPlugin(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);

		var vendor = GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage(tab, "vendor", "Local")));
		vendor.Refresh();

		ap.SetStorage(tab, "selectedplugin", vendor.FetchedPlugins.FirstOrDefault(x => x.Id == args.Args[0]));

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.deselectplugin")]
	private void PluginBrowserDeselectPlugin(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);

		var vendor = GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage(tab, "vendor", "Local")));
		vendor.Refresh();

		ap.SetStorage(tab, "selectedplugin", (PluginsTab.Plugin)null);

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.changeselectedplugin")]
	private void PluginBrowserChangeSelected(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);

		var vendor = GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage(tab, "vendor", "Local")));
		vendor.Refresh();

		var plugins = GetPlugins(vendor, tab, ap);
		var nextPage = plugins.IndexOf(ap.GetStorage<PluginsTab.Plugin>(tab, "selectedplugin")) + args.Args[0].ToInt();
		ap.SetStorage(tab, "selectedplugin", plugins[nextPage > plugins.Count - 1 ? 0 : nextPage < 0 ? plugins.Count - 1 : nextPage]);
		Facepunch.Pool.FreeList(ref plugins);

		DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.changesetting")]
	private void PluginBrowserChangeSetting(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage(tab, "vendor", "Local")));

		switch (args.Args[0])
		{
			case "filter_dd":
				DropdownShow = !DropdownShow;

				if (args.HasArgs(4))
				{
					var index = args.Args[3].ToInt();
					var filter = ap.GetStorage(tab, "filter", FilterTypes.None);
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

		DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		vendor.Refresh();
		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.login")]
	private void PluginBrowserLogin(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = ap.GetStorage<string>(tab, "vendor");

		var vendor2 = GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), vendor));
		if (vendor2 is PluginsTab.IVendorAuthenticated auth)
		{
			if (auth.IsLoggedIn)
			{
				tab.CreateDialog("Are you sure you want to log out?", onConfirm: ap =>
				{
					auth.User = null;
					vendor2.Refresh();
					Singleton.Draw(args.Player());
					if (vendor2 is IVendorStored store) store.Save();
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
				var core = Community.Runtime.CorePlugin;
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
	[ProtectedCommand("pluginbrowser.closelogin")]
	private void PluginBrowserCloseLogin(Arg args)
	{
		var ap = Singleton.GetPlayerSession(args.Player());
		var tab = Singleton.GetTab(ap.Player);
		var vendor = ap.GetStorage<string>(tab, "vendor");

		var vendor2 = GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), vendor));
		if (vendor2 is PluginsTab.IVendorAuthenticated auth)
		{
			auth.User = null;
			auth.ValidationTimer?.Destroy();

			Singleton.Draw(args.Player());
		}
	}

	[ConsoleCommand("adminmodule.downloadplugin", "Downloads a plugin from a vendor (if available). Syntax: adminmodule.downloadplugin <codefling|umod> <plugin>")]
	[AuthLevel(2)]
	private void DownloadPlugin(Arg args)
	{
		var vendor = GetVendor(args.Args[0] == "codefling" ? VendorTypes.Codefling : VendorTypes.uMod);
		if (vendor == null)
		{
			Singleton.PutsWarn($"Couldn't find that vendor.");
			return;
		}
		var plugin = vendor.FetchedPlugins.FirstOrDefault(x => x.Name.ToLower().Trim().Contains(args.Args[1].ToLower().Trim()));
		if (plugin == null)
		{
			Singleton.PutsWarn($"Cannot find that plugin.");
			return;
		}
		vendor.Download(plugin.Id, () => { Singleton.PutsWarn($"Couldn't download {plugin.Name}."); });
	}

	[ConsoleCommand("adminmodule.updatevendor", "Downloads latest vendor information. Syntax: adminmodule.updatevendor <codefling|umod>")]
	[AuthLevel(2)]
	private void UpdateVendor(Arg args)
	{
		var vendor = GetVendor(args.Args[0] == "codefling" ? VendorTypes.Codefling : VendorTypes.uMod);
		if (vendor == null)
		{
			Singleton.PutsWarn($"Couldn't find that vendor.");
			return;
		}

		var id = string.Empty;
		switch (vendor)
		{
			case Codefling:
				id = "cf";
				break;

			case uMod:
				id = "umod";
				break;
		}

		var dataPath = Path.Combine(Defines.GetDataFolder(), $"vendordata_{id}.db");
		OsEx.File.Delete(dataPath);

		if (vendor is IVendorStored stored && !stored.Load())
		{
			vendor.FetchList();
			vendor.Refresh();
		}
	}

	#endregion
}

#endif
