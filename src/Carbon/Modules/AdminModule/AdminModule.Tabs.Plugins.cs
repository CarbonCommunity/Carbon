#if !MINIMAL

using System.IO.Compression;
using System.Net;
using System.Text;
using Facepunch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Game.Rust.Cui;
using ProtoBuf;
using UnityEngine.UI;
using static ConsoleSystem;
using Exception = System.Exception;
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

		public static Tab TabInstance;

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

			Tab tab = null;
			tab = new Tab("plugins", "Plugins", Community.Runtime.Core, access: "plugins.use", onChange:
				(session, tab1) =>
				{
					tab.AddColumn(0, true);
					tab.AddColumn(1, true);

					tab.Override = (tab, cui, container, panel, ap) =>
					{
						cui.CreatePanel(container, panel, Cache.CUI.BlackColor);

						const string starEmpty = "☆";
						const string starFull = "★";
						const float optionsHeight = 0.07f;
						const float optionsWidth = 100f;
						const float optionsSpacing = 2f;
						var optionsOffset = 0f;
						var selectedVendor = GetVendor(ap.GetStorage(tab, "vendor", VendorTypes.Installed));
						var isInstalledVendor = selectedVendor == LocalInstance;
						var options = cui.CreatePanel(container, panel, "#1f1f1d", yMin: 1f - optionsHeight);

						foreach (var vendor in Enum.GetNames(typeof(VendorTypes)))
						{
							var disabled = GetVendor((VendorTypes)Enum.Parse(typeof(VendorTypes), vendor)) == null;
							var isSelected =  selectedVendor.GetType().Name == vendor;
							CreateTabButton(cui, container, options, vendor, selectedVendor?.BarInfo.ToUpper(),
								isSelected, ref optionsOffset, disabled);
						}

						static void CreateTabButton(CUI cui, CuiElementContainer container, string panel, string text,
							string subtext, bool isSelected, ref float optionsOffset, bool disabled)
						{
							var btn = cui.CreateProtectedButton(container, panel,
								isSelected ? "#af3726" : CUI.HexToRustColor("#454239", 0.5f), Cache.CUI.BlankColor, string.Empty, 0,
								xMin: 0.05f, xMax: 0.05f, OxMin: optionsOffset, OxMax: optionsOffset + optionsWidth,
								command: disabled ? string.Empty : $"pluginbrowser.changetab {text}");
							cui.CreateImage(container, btn, "fade", Cache.CUI.WhiteColor);
							cui.CreateText(container, btn, !isSelected ? "1 1 1 0.4" : "1 0.8 0.8 1",
								text.ToUpper(), 12, font: CUI.Handler.FontTypes.RobotoCondensedBold,
								yMin: isSelected || disabled ? 0.3f : 0f);

							if (isSelected)
							{
								cui.CreateText(container, btn, "1 0.8 0.8 0.5", subtext, 10, yMax: 0.5f);
							}

							if (disabled)
							{
								cui.CreateText(container, btn, "1 0.8 0.8 0.5", "DISABLED", 10, yMax: 0.5f);
							}

							optionsOffset += optionsWidth + optionsSpacing;
						}

						var scroll = cui.CreateScrollView(container, panel, true, false,
							ScrollRect.MovementType.Elastic, 0.1f, true, 0.1f, 150,
							out var content, out _, out var verticalBar, yMax: 1f - optionsHeight);

						const float cardWidth = 150;
						const float cardWidthMargin = 50;
						const float cardHeight = 190;
						const float cardHeightMargin = 150;
						const float cardSpacing = 10;
						const float cardAnimDuration = 0.2f;
						const string accentReadableColor = "0.8 0.8 0.8 0.6";

						const string pageButtonColor = "0.2 0.2 0.2 0.9";
						const string pageButtonColorDark = "0.2 0.2 0.2 0.5";
						const string pageButtonColorLightDark = "0.4 0.4 0.4 0.8";

						var searchInput = ap.GetStorage(tab, "search", string.Empty);
						var plugins = GetPlugins(selectedVendor, tab, ap, out var maxPages, 75);
						var page = ap.GetOrCreatePage(230);
						var count = (float)plugins.Count();
						var rows = Mathf.Ceil(count / 5f) - 2;
						var height = (cardHeightMargin + (rows * (cardHeight + cardSpacing))).Clamp(30f, int.MaxValue);
						page.TotalPages = maxPages;
						page.Check();

						content.AnchorMin = "0 0";
						content.AnchorMax = "1 1";
						content.OffsetMin = $"0 -{height}";
						content.OffsetMax = $"0 0";
						verticalBar.Size = 7;
						verticalBar.AutoHide = false;

						cui.CreateImage(container, scroll, selectedVendor.Hero, Cache.CUI.WhiteColor,
							yMin: 1, yMax: 1, OyMin: -500);

						var contentPanel = cui.CreatePanel(container, scroll, "0 0.1 0.3 0.4", yMin: 1,
							yMax: 1, OyMin: -(height + 450));

						if (!plugins.Any())
						{
							cui.CreateText(container, contentPanel, "0.4 0.4 0.4 0.5", "No plugins available", 10);
						}

						cui.CreateText(container, contentPanel, "0.8 0.8 0.8 0.9",
							selectedVendor.Type.ToUpper(), 32, xMin: 0.04f, yMin: 1, yMax: 1, OyMin: -100,
							OyMax: -30, font: CUI.Handler.FontTypes.RobotoCondensedBold,
							align: TextAnchor.UpperLeft);
						cui.CreateText(container, contentPanel, "0.6 0.6 0.6 0.9",
							selectedVendor.Tagline, 15, xMin: 0.04f,
							yMin: 1, yMax: 1, OyMin: -100, OyMax: -70,
							font: CUI.Handler.FontTypes.RobotoCondensedRegular,
							align: TextAnchor.UpperLeft);

						if (selectedVendor.CanRefresh)
						{
							var refresh = cui.CreateProtectedButton(container, contentPanel,
								"0.2 0.3 0.5 0.9", Cache.CUI.BlankColor, string.Empty, 00,
								xMin: 0.26f, xMax: 0.295f, yMin: 1, yMax: 1, OyMin: -130, OyMax: -100,
								command: "pluginbrowser.refreshvendor");
							cui.CreateImage(container, refresh, "reload", "0.3 0.5 0.8 1", xMin: 0.2f,
								xMax: 0.8f, yMin: 0.2f, yMax: 0.8f);
							cui.CreateImage(container, refresh, "fade", Cache.CUI.WhiteColor);
						}

						if (selectedVendor is IVendorAuthenticated authVendor)
						{
							var defaultProfile = Singleton.ImageDatabase.GetKeyImage("default_profile");
							cui.CreateClientImage(container, contentPanel,
								authVendor != null && authVendor.IsLoggedIn
									? authVendor.User.AvatarUrl
									: defaultProfile,
								"1 1 1 0.7", xMin: 0.91f, xMax: 0.96f, yMin: 1, yMax: 1, OyMin: -65,
								OyMax: -20);
							cui.CreateText(container, contentPanel, "0.8 0.8 0.8 0.9",
								authVendor != null && authVendor.IsLoggedIn
									? authVendor.User?.DisplayName?.ToUpper()
									: "GUEST", 15, xMax: 0.90f, yMin: 1, yMax: 1, OyMin: -100, OyMax: -30,
								font: CUI.Handler.FontTypes.RobotoCondensedBold,
								align: TextAnchor.UpperRight);
							cui.CreateText(container, contentPanel, "0.85 0.8 0.1 0.9",
								authVendor != null && authVendor.IsLoggedIn
									? authVendor.User?.Authority?.ToUpper()
									: "NOT AUTHENTICATED", 10, xMax: 0.90f, yMin: 1, yMax: 1, OyMin: -100,
								OyMax: -50, font: CUI.Handler.FontTypes.RobotoCondensedRegular,
								align: TextAnchor.UpperRight);

							if (authVendor != null)
							{
								cui.CreateProtectedButton(container, contentPanel,
									authVendor != null && authVendor.IsLoggedIn
										? "0.55 0.1 0.1 1"
										: "0.1 0.55 0.1 1",
									authVendor != null && authVendor.IsLoggedIn
										? "1 0.5 0.5 1"
										: "0.5 1 0.5 1",
									authVendor != null && authVendor.IsLoggedIn ? "LOG OUT" : "LOG IN", 10,
									xMin: 0.85f, xMax: 0.9f, yMin: 1, yMax: 1, OyMin: -90, OyMax: -70,
									font: CUI.Handler.FontTypes.RobotoCondensedBold,
									command: "pluginbrowser.login");
							}
						}

						var currentWidth = cardWidthMargin;
						var currentHeight = cardHeightMargin;
						var index = 0;
						var currentAnim = 1f;
						var vendorLogo = Singleton.ImageDatabase.GetKeyImage(selectedVendor.Logo);

						for (int p = 0; p < plugins.Count; p++)
						{
							index++;

							var plugin = plugins[p];
							var originalPlugin = plugin;
							if (isInstalledVendor && plugin.PreferredVendorPlugin != null)
							{
								plugin = plugin.PreferredVendorPlugin;
							}

							var displayVendor = Singleton.DataInstance.HidePluginIcons || plugin.HasNoImage() ||
							                    plugin.HasInvalidImage();
							var card = cui.CreateProtectedButton(container, contentPanel, "0.1 0.1 0.1 0.7",
								Cache.CUI.BlankColor, null, 0,
								command:
								$"pluginbrowser.selectplugin \"{Path.GetFileNameWithoutExtension(plugin.File)}\"",
								xMin: 0,
								xMax: 0, yMin: 1, yMax: 1,
								OxMin: currentWidth, OxMax: currentWidth + cardWidth,
								OyMin: -(currentHeight + cardHeight), OyMax: -currentHeight);
							var image = cui.CreateClientImage(container, card, plugin.ImageThumbnail,
								displayVendor ? "0 0 0 0" : "1 1 1 0.95", yMin: 1, OyMin: -cardWidth,
								fadeIn: currentAnim);
							if (displayVendor)
							{
								cui.CreatePanel(container, image, "1 1 1 0.15");
								cui.CreateImage(container, image, vendorLogo, "0.15 0.15 0.15 0.9",
									xMin: 0.2f, xMax: 0.8f, yMin: 0.2f + selectedVendor.LogoRatio,
									yMax: 0.8f - selectedVendor.LogoRatio);
							}

							cui.CreateImage(container, card, "fade", Cache.CUI.WhiteColor);

							// cui.CreateText(container, card, pageButtonColorLightDark, (index * (page.CurrentPage + 1)).ToString(), 8, yMin: 0, yMax: 0, OyMin: -10);

							if (plugin.IsInstalled())
							{
								var outOfDate = !plugin.IsUpToDate();
								var installed = cui.CreatePanel(container, image,
									outOfDate ? "0.9 0.5 0.1 0.6" : "0.5 0.9 0.1 0.6", yMin: 0.9f);
								cui.CreateText(container, installed,
									outOfDate ? "1 0.75 0.5 1" : "0.75 1 0.5 1",
									(outOfDate ? "OUTDATED" : "INSTALLED").SpacedString(1), 8,
									font: CUI.Handler.FontTypes.RobotoCondensedBold);
							}

							if (selectedVendor == LocalInstance && originalPlugin.AvailableOn != null &&
							    originalPlugin.AvailableOn.Count > 1)
							{
								cui.CreateProtectedButton(container, card, Cache.CUI.BlankColor, "0.75 1 0.5 0.8",
									originalPlugin.PreferredVendor.ToString().ToUpper().SpacedString(1), 8,
									font: CUI.Handler.FontTypes.RobotoCondensedBold, yMin: 0.83f, yMax: 0.92f,
									command:
									$"pluginbrowser.interact 12 \"{Path.GetFileNameWithoutExtension(plugin.File)}\"");
							}

							var isFavourited = ServerOwner.Singleton.FavouritePlugins.Contains(plugin.Name);
							var favourite = cui.CreateProtectedButton(container, card, Cache.CUI.BlankColor,
								Cache.CUI.BlankColor, string.Empty, 0,
								xMin: 0, xMax: 0, yMin: 1, OxMax: 30, OyMin: -30,
								command:
								$"pluginbrowser.interact 10 \"{Path.GetFileNameWithoutExtension(plugin.File)}\"");
							cui.CreateImage(container, favourite, "top_left",
								isFavourited ? "0.7 0.2 0.2 0.6" : "0.1 0.1 0.1 0.2");
							cui.CreateImage(container, favourite, "star",
								isFavourited ? "0.9 0.5 0.4" : "0.1 0.1 0.1 0.5", xMin: 0.075f, xMax: 0.5f,
								yMin: 0.5f, yMax: 0.9f);

							cui.CreateText(container, card, Cache.CUI.WhiteColor,
								plugin.Name.Truncate(17, elipsis: "...", countElipsisLength: false), 12,
								xMin: 0.05f, yMax: 0.165f,
								font: CUI.Handler.FontTypes.RobotoCondensedBold,
								align: TextAnchor.UpperLeft, fadeIn: currentAnim);
							cui.CreateText(container, card, accentReadableColor, $"by {plugin.Author}", 8,
								xMin: 0.05f, yMax: 0.085f,
								font: CUI.Handler.FontTypes.RobotoCondensedRegular,
								align: TextAnchor.UpperLeft, fadeIn: currentAnim);

							if (plugin.HasPrice)
							{
								cui.CreateText(container, card, plugin.IsPaid() ? "#e0e344" : "#44e3db",
									plugin.IsPaid() ? $"${plugin.OriginalPrice.ToFloat():0.00}" : "FREE",
									10, xMax: 0.95f, yMax: 0.155f,
									font: CUI.Handler.FontTypes.RobotoCondensedBold,
									align: TextAnchor.UpperRight, fadeIn: currentAnim);
							}

							if (!Mathf.Approximately(plugin.Rating, -1))
							{
								var builder = Facepunch.Pool.Get<StringBuilder>();

								for (int i = 0; i < 5; i++)
								{
									builder.Append(plugin.Rating <= i ? starEmpty : starFull);
								}

								cui.CreateText(container, card, accentReadableColor, builder.ToString(), 12,
									xMin: 0.565f, xMax: 0.96f, yMin: 0.02f, yMax: 0.08f,
									align: TextAnchor.UpperRight);
								Facepunch.Pool.FreeUnmanaged(ref builder);
							}

							if (plugin.Owned)
							{
								var badge = cui.CreatePanel(container, image, "0 0.4 0.8 0.9", yMax: 0.1f);
								cui.CreateText(container, badge, "0.5 0.75 1 1",
									"PURCHASED".SpacedString(1), 8,
									font: CUI.Handler.FontTypes.RobotoCondensedBold);
							}

							currentWidth += cardWidth + cardSpacing;

							if (index % 5 == 0)
							{
								currentHeight += cardHeight + cardSpacing;
								currentWidth = cardWidthMargin;
							}

							currentAnim += cardAnimDuration;
						}

						var search = cui.CreatePanel(container, contentPanel, pageButtonColor, xMin: 0.04f,
							xMax: 0.25f, yMin: 1, yMax: 1, OyMin: -130, OyMax: -100);
						cui.CreateImage(container, search, "fade", Cache.CUI.WhiteColor);
						cui.CreateImage(container, search, "magnifying-glass", accentReadableColor,
							xMin: 0.05f, xMax: 0.14f, yMin: 0.2f, yMax: 0.8f);
						cui.CreateProtectedInputField(container, search, accentReadableColor,
							string.IsNullOrEmpty(searchInput) ? "Search..." : searchInput, 13, 50, false,
							xMin: 0.17f, command: "pluginbrowser.search", align: TextAnchor.MiddleLeft,
							needsKeyboard: true);
						var clearSearch = cui.CreateProtectedButton(container, search, Cache.CUI.BlankColor,
							Cache.CUI.BlankColor, string.Empty, 0, xMin: 0.9f, command: "pluginbrowser.search ");
						cui.CreateImage(container, clearSearch, "close", pageButtonColorLightDark,
							xMin: 0.5f, xMax: 0.5f, yMin: 0.5f, yMax: 0.5f, OxMin: -5, OxMax: 5, OyMin: -5,
							OyMax: 5);

						var sortId = (int)ap.GetStorage(tab, "filter", FilterTypes.None);
						var sort = cui.CreateProtectedButton(container, contentPanel, pageButtonColor,
							Cache.CUI.BlankColor, string.Empty, 0, xMin: 0.7f, xMax: 0.9f, yMin: 1, yMax: 1,
							OyMin: -130, OyMax: -100, command: "pluginbrowser.changesetting filter_dd");
						cui.CreateImage(container, sort, "fade", Cache.CUI.WhiteColor);
						cui.CreateImage(container, sort, "sort", accentReadableColor, xMin: 0.05f,
							xMax: 0.14f, yMin: 0.2f, yMax: 0.8f);
						cui.CreateText(container, sort, accentReadableColor, DropdownOptions[sortId], 13,
							xMin: 0.17f, align: TextAnchor.MiddleLeft);
						if (DropdownShow)
						{
							const float optionHeight = 0.85f;
							float currentOptionHeight = 0;
							for (int i = 0; i < DropdownOptions.Length; i++)
							{
								var min = currentOptionHeight;
								var max = currentOptionHeight - optionHeight;
								var button = cui.CreateProtectedButton(container, sort,
									sortId == i ? pageButtonColorLightDark : pageButtonColor,
									Cache.CUI.BlankColor, string.Empty, 0, yMin: max, yMax: min,
									command: $"pluginbrowser.changesetting filter_dd true call {i}");
								cui.CreateText(container, button, accentReadableColor, DropdownOptions[i],
									13, xMin: 0.07f, align: TextAnchor.MiddleLeft);
								currentOptionHeight -= optionHeight;
							}
						}

						PaginationButtons(cui, container, contentPanel, page, yMin: 1, yMax: 1, oYMin: -130,
							oYMax: -100);

						if (plugins.Any())
						{
							PaginationButtons(cui, container, contentPanel, page);
						}

						static void PaginationButtons(CUI cui, CuiElementContainer container, string parent,
							PlayerSession.Page page, float xMin = 0.5f, float xMax = 0.5f, float yMin = 0,
							float yMax = 0, float oXMin = -100, float oXMax = 100, float oYMin = 10,
							float oYMax = 40)
						{
							var pageOffset = 0f;
							var pagination = cui.CreatePanel(container, parent, Cache.CUI.BlankColor,
								xMin: xMin, xMax: xMax, yMin: yMin, yMax: yMax, OxMin: -100, OxMax: 100,
								OyMin: oYMin, OyMax: oYMax);

							PaginationButton(ref pageOffset, cui, container, pagination, "<<",
								command: "pluginbrowser.page -2");
							PaginationButton(ref pageOffset, cui, container, pagination, "<",
								command: "pluginbrowser.page -1");
							var pageInput = cui.CreatePanel(container, pagination, pageButtonColorDark,
								xMax: 0, OxMin: pageOffset, OxMax: pageOffset + 70f);
							cui.CreateImage(container, pageInput, "fade", Cache.CUI.WhiteColor);
							cui.CreateText(container, pageInput, pageButtonColorLightDark,
								$"/ {page.TotalPages:n0}", 10, xMin: 0.5f, align: TextAnchor.MiddleLeft);
							cui.CreateProtectedInputField(container, pageInput, Cache.CUI.WhiteColor,
								$"{page.CurrentPage + 1}", 10, 60, false, align: TextAnchor.MiddleRight,
								xMax: 0.45f, command: "pluginbrowser.page");
							pageOffset += 75f;
							PaginationButton(ref pageOffset, cui, container, pagination, ">",
								command: "pluginbrowser.page 1");
							PaginationButton(ref pageOffset, cui, container, pagination, ">>",
								command: "pluginbrowser.page -3");
						}

						static void PaginationButton(ref float pageOffset, CUI cui,
							CuiElementContainer container, string parent, string text,
							bool disabled = false, string command = null)
						{
							var paginationLeft = cui.CreateProtectedButton(container, parent,
								pageButtonColor, "0.7 0.7 0.7 0.4", text, 10, command: command, xMin: 0,
								xMax: 0, OxMin: pageOffset, OxMax: pageOffset + 30f);
							cui.CreateImage(container, paginationLeft, "fade", Cache.CUI.WhiteColor);

							pageOffset += 35f;
						}

						var selectedPlugin = cui.CreatePanel(container, panel, Cache.CUI.BlankColor,
							xMin: 0, xMax: 0, yMin: 0, yMax: 0, id: "selectedpluginpnl");
						{
							cui.CreatePanel(container, selectedPlugin, "0 0 0 0.6", blur: true);
							cui.CreatePanel(container, selectedPlugin, "0 0 0 0.6");

							var selectedScroll = cui.CreateScrollView(container, selectedPlugin, true,
								false, ScrollRect.MovementType.Elastic, 0.1f, true, 0.1f, 50,
								out var selectedContent, out _, out var selectedVerticalBar);

							const float detailBarThickness = 2;
							const float detailBarOffset = 475;

							selectedContent.AnchorMin = "0 0";
							selectedContent.AnchorMax = "1 1";
							selectedContent.OffsetMin = $"0 -250";
							selectedContent.OffsetMax = $"0 0";
							selectedVerticalBar.Size = 7;
							selectedVerticalBar.AutoHide = false;

							var icon = cui.CreateClientImage(container, selectedScroll, string.Empty,
								Cache.CUI.BlankColor, id: "selectedpluginicn");
							cui.CreateImage(container, icon, "hero_fade", "0 0.1 0.2 1");
							cui.CreatePanel(container, selectedScroll, "0 0.1 0.2 1", OyMax: -1000);
							cui.CreatePanel(container, icon, "0 0 0 0.2");

							cui.CreateText(container, selectedScroll, Cache.CUI.WhiteColor, string.Empty, 0,
								align: TextAnchor.UpperLeft, id: "selectedpluginname");
							cui.CreateText(container, selectedScroll, Cache.CUI.WhiteColor, string.Empty, 0,
								align: TextAnchor.UpperLeft, id: "selectedpluginprice");
							cui.CreateText(container, selectedScroll, Cache.CUI.WhiteColor, string.Empty, 0,
								align: TextAnchor.UpperLeft, id: "selectedplugindesc");
							cui.CreateText(container, selectedScroll, accentReadableColor, string.Empty, 0,
								align: TextAnchor.UpperLeft, id: "selectedplugininfo");
							cui.CreatePanel(container, selectedScroll, "0.7 0.7 0.7 0.3", xMin: 0.05f,
								xMax: 0.95f, yMin: 1, yMax: 1,
								OyMin: -(detailBarOffset + detailBarThickness), OyMax: -detailBarOffset);

							cui.CreateProtectedButton(container, selectedScroll, Cache.CUI.BlankColor,
								Cache.CUI.BlankColor, string.Empty, 0, command: "pluginbrowser.deselectplugin");

							var changelog = cui.CreatePanel(container, selectedScroll, "0.2 0.2 0.2 0.9",
								xMin: 0.05f, xMax: 0.95f, yMin: 1, OyMin: -700, OyMax: -500);
							cui.CreateImage(container, changelog, "fade", Cache.CUI.WhiteColor);
							cui.CreateText(container, changelog, "1 1 1 0.8", "VERSION CHANGES", 13,
								font: CUI.Handler.FontTypes.RobotoCondensedBold, xMin: 0.02f, yMax: 0.95f,
								align: TextAnchor.UpperLeft);
							cui.CreatePanel(container, changelog, "0.7 0.7 0.7 0.2", xMin: 0.78f,
								xMax: 0.78f, yMin: 0.05f, yMax: 0.95f, OxMin: -detailBarThickness);

							var version = cui.CreatePanel(container, changelog, Cache.CUI.BlankColor,
								xMin: 0.78f);
							cui.CreateText(container, version, "1 1 1 0.2", "RELEASE DATE", 12, yMax: 0.7f,
								align: TextAnchor.MiddleCenter,
								font: CUI.Handler.FontTypes.RobotoCondensedBold);
							cui.CreateText(container, version, accentReadableColor, "14 June", 15,
								yMax: 0.8f, align: TextAnchor.MiddleCenter,
								font: CUI.Handler.FontTypes.RobotoCondensedRegular,
								id: "selectedpluginrdate");
							cui.CreatePanel(container, version, "0.7 0.7 0.7 0.2", xMin: 0.3f, xMax: 0.7f,
								yMin: 0.55f, yMax: 0.55f, OyMin: -detailBarThickness);

							cui.CreateText(container, version, "1 1 1 0.2", "RATING", 12, yMin: 0.65f,
								align: TextAnchor.MiddleCenter,
								font: CUI.Handler.FontTypes.RobotoCondensedBold);
							cui.CreateText(container, version, Cache.CUI.BlankColor, string.Empty, 0,
								xMin: 0.3f, xMax: 0.7f, yMin: 0.7f, yMax: 0.75f,
								align: TextAnchor.UpperCenter, id: "selectedpluginrating");

							var changelogScroll = cui.CreateScrollView(container, changelog, true, false,
								ScrollRect.MovementType.Elastic, 0.1f, true, 0.1f, 50,
								out var changelogContent, out _, out var changelogVerticalBar, xMin: 0.02f,
								xMax: 0.72f, yMin: 0.1f, yMax: 0.8f);

							changelogContent.AnchorMin = "0 0";
							changelogContent.AnchorMax = "1 1";
							changelogContent.OffsetMin = $"0 -100";
							changelogContent.OffsetMax = $"0 0";
							changelogVerticalBar.HandleColor =
								changelogVerticalBar.TrackColor = Cache.CUI.BlankColor;
							changelogVerticalBar.Size = 0;
							changelogVerticalBar.AutoHide = true;

							cui.CreateText(container, changelogScroll, "1 1 1 0.7", "lorem ipsum", 13,
								align: TextAnchor.UpperLeft, id: "selectedpluginchlog");
							cui.CreateImage(container, selectedPlugin, "fade_flip", "0 0 0 1", yMin: 1,
								OyMin: -75);

							var closeButton = cui.CreateProtectedButton(container, selectedPlugin,
								"0.2 0.2 0.2 0.9", Cache.CUI.BlankColor, string.Empty, 0,
								command: "pluginbrowser.deselectplugin", xMin: 0.05f, xMax: 0.08f, yMin: 0.9f,
								yMax: 0.955f);
							cui.CreateImage(container, closeButton, "close", "0.7 0.7 0.7 0.4", xMin: 0.3f,
								xMax: 0.7f, yMin: 0.3f, yMax: 0.7f);
							cui.CreateImage(container, closeButton, "fade", Cache.CUI.WhiteColor);

							var buttonOffset = 0f;
							DrawButton(cui, container, selectedPlugin, "selectedplugin_b1",
								"selectedplugin_b1_icn", "selectedplugin_b1_txt", "selectedplugin_b1_fade",
								100, ref buttonOffset);
							DrawButton(cui, container, selectedPlugin, "selectedplugin_b2",
								"selectedplugin_b2_icn", "selectedplugin_b2_txt", "selectedplugin_b2_fade",
								90, ref buttonOffset);
							DrawButton(cui, container, selectedPlugin, "selectedplugin_b3",
								"selectedplugin_b3_icn", "selectedplugin_b3_txt", "selectedplugin_b3_fade",
								80, ref buttonOffset);
							DrawButton(cui, container, selectedPlugin, "selectedplugin_b4",
								"selectedplugin_b4_icn", "selectedplugin_b4_txt", "selectedplugin_b4_fade",
								75, ref buttonOffset);
							DrawButton(cui, container, selectedPlugin, "selectedplugin_b5",
								"selectedplugin_b5_icn", "selectedplugin_b5_txt", "selectedplugin_b5_fade",
								80, ref buttonOffset);

							static void DrawButton(CUI cui, CuiElementContainer container, string parent,
								string btn, string icn, string txt, string fade, float width,
								ref float offset)
							{
								var button = cui.CreateProtectedButton(container, parent, "0.5 0.5 0.5 0.5",
									Cache.CUI.BlankColor, string.Empty, 0, command: null, xMin: 0.09f,
									xMax: 0.09f, OxMin: offset, OxMax: offset + width, yMin: 0.9f,
									yMax: 0.955f, id: btn);
								cui.CreateImage(container, button, "graph", Cache.CUI.BlankColor, xMin: 0,
									xMax: 0, yMin: 0.5f, yMax: 0.5f, OxMin: 5, OxMax: 25, OyMin: -10,
									OyMax: 10, id: icn);
								cui.CreateText(container, button, Cache.CUI.BlankColor, string.Empty, 0,
									align: TextAnchor.MiddleCenter, xMin: 0.2f,
									font: CUI.Handler.FontTypes.RobotoCondensedBold, id: txt);
								cui.CreateImage(container, button, "fade", Cache.CUI.BlankColor, id: fade);
								offset += width + 5;
							}
						}

						var auth = selectedVendor as IVendorAuthenticated;

						if (auth != null)
						{
							if (auth.IsLoggedIn && auth.User != null && auth.User.PendingAccessToken)
							{
								var mainPanel = cui.CreatePanel(container, panel, "0.15 0.15 0.15 0.35",
									blur: true);
								cui.CreatePanel(container, mainPanel, "0 0 0 0.9");

								cui.CreateText(container, panel, "1 1 1 1", $"{selectedVendor.Type} Auth",
									25, xMin: 0.51f, yMax: 0.75f, align: TextAnchor.UpperLeft,
									font: CUI.Handler.FontTypes.RobotoCondensedBold);
								cui.CreateText(container, panel, "1 1 1 0.5",
									$"Securely log into your {selectedVendor.Type} account through OAuth-based login!\n\nScan the QR code or go to the URL, log into {selectedVendor.Type} and type in the provided authentication code below to complete the login process.",
									15, xMin: 0.51f, xMax: 0.9f, yMax: 0.67f, align: TextAnchor.UpperLeft);
								cui.CreateText(container, panel, "1 1 1 1", "Authorization code:", 13,
									xMin: 0.51f, yMax: 0.35f, align: TextAnchor.UpperLeft);

								cui.CreateProtectedButton(container, parent: panel,
									color: Cache.CUI.BlankColor,
									textColor: Cache.CUI.BlankColor,
									text: string.Empty, 0,
									command: "pluginbrowser.closelogin");

								var image = cui.CreatePanel(container, panel, "1 1 1 1", xMin: 0.12f,
									xMax: 0.45f, yMin: 0.2f, yMax: 0.8f);
								var code = string.Format(auth.AuthRequestEndpoint, auth.AuthCode);

								cui.CreateQRCodeImage(container, image, code,
									brandUrl: selectedVendor.Logo,
									brandColor: "0 0 0 1",
									brandBgColor: "1 1 1 1", 15, true, true, "0 0 0 1", xMin: 0, xMax: 1,
									yMin: 0, yMax: 1);
								var pan = cui.CreatePanel(container, panel, "0.1 0.1 0.1 1", xMin: 0.5f,
									xMax: 0.8f, yMin: 0.21f, yMax: 0.31f);
								var authUrl = cui.CreatePanel(container, image, "0.1 0.1 0.1 0.8", yMax: 0,
									OyMin: -20);
								cui.CreateInputField(container, authUrl, "1 1 1 1",
									auth.AuthRequestEndpointPreview, 9, 0, true);
								cui.CreateInputField(container, pan, "1 1 1 1", auth.AuthCode, 30, 0, true,
									xMin: 0.05f,
									align: TextAnchor.MiddleLeft,
									font: CUI.Handler.FontTypes.RobotoCondensedBold);

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
										cui.CreateImage(container, image, icon, color, xMin: 0.3f,
											xMax: 0.7f, yMin: 0.3f, yMax: 0.7f);
									}
								}
							}
						}

						Facepunch.Pool.FreeUnmanaged(ref plugins);
					};
				});

			CodeflingInstance = new Codefling();
			if (CodeflingInstance is IVendorStored cfStored && !cfStored.Load())
			{
				CodeflingInstance.FetchList((vendor) =>
				{
					CodeflingInstance.Refresh();
					CodeflingInstance.VersionCheck();
				});
			}

			InstallUModTab();

			LocalInstance = new Installed();
			LocalInstance.Refresh();

			ServerOwner.Load();
			return TabInstance = tab;
		}

		public static void InstallUModTab()
		{
			if (Singleton.DataInstance.DisableUMod)
			{
				if (uModInstance is uMod umod)
				{
					umod.Dispose();
				}
				uModInstance = null;
				return;
			}

			uModInstance = new uMod();
			if (uModInstance is IVendorStored umodStored && !umodStored.Load())
			{
				uModInstance.FetchList(_ =>
				{
					uModInstance.Refresh();
					uModInstance.VersionCheck();
				});
			}
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
				if (Singleton.DataInstance.HidePluginIcons || element.HasNoImage()) continue;

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
			public virtual string Tagline { get; }

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
					if (plugin.IsInstalled() && !plugin.IsUpToDate())
					{
						// 3156772569 aka OnPluginOutdated
						HookCaller.CallStaticHook(3156772569, plugin.Name, new VersionNumber(plugin.CurrentVersion()), new VersionNumber(plugin.Version), plugin.ExistentPlugin, Type);
					}
				}
			}

			public override string ToString()
			{
				return Type + " Vendor";
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
			public override string Tagline => "The largest marketplace for Rust community-driven content.";

			public override string BarInfo => $"{FetchedPlugins.Count(x => !x.IsPaid()):n0} free, {FetchedPlugins.Count(x => x.IsPaid()):n0} paid";

			public override string ListEndpoint => "https://codefling.com/db/?category=2";
			public string List2Endpoint => "https://codefling.com/db/?category=21";
			public override string DownloadEndpoint => "https://codefling.com/files/file/[ID]-a?do=download";

			private Dictionary<string, string> _headers = new();
			private static readonly string _backSlashes = "\\";

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
						plugin.SetOwned(auth.User != null && (auth.User.IsAdmin || auth.User.OwnedFiles.Contains(plugin.Id)));

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
									Image = $"https://codefling.com/cdn-cgi/image/width=1250,height=1250,quality=100,blur=25,fit=cover,format=jpeg/{token["primaryScreenshot"]}",
									ImageThumbnail = $"https://codefling.com/cdn-cgi/image/width=246,height=246,quality=75,fit=cover,format=jpeg/{token["primaryScreenshot"]}",
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
								plugin.PreferredVendor = VendorTypes.Codefling;

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

								Logger.Log($"[{vendor} Tab] Fetched latest plugin information.");

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
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase) ||
				                                                x.Name.Equals(id, StringComparison.CurrentCultureIgnoreCase) ||
				                                                Path.GetFileNameWithoutExtension(x.File).Equals(id, StringComparison.CurrentCultureIgnoreCase));
				if (plugin == null)
				{
					Logger.Error($"Couldn't find '{id}' on {Type}");
				}
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
								var path = plugin.ExistentPlugin == null ? Path.Combine(Defines.GetScriptsFolder(), name) : plugin.ExistentPlugin.FilePath;

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

										OsEx.File.Move(path, Path.Combine(Defines.GetScriptsFolder(), "backups", Path.GetFileName(path)));
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

													OsEx.File.Move(Path.Combine(Defines.GetScriptsFolder(), file.Name), Path.Combine(Defines.GetScriptsFolder(), "backups", file.Name));
													OsEx.File.Create(Path.Combine(Defines.GetScriptsFolder(), file.Name), fileSource);
													Singleton.Puts($" Extracted plugin file {file.Name}");
												}
												break;

											case dllExtension:
												{
													StoreFile(file, Path.Combine(Defines.GetLibFolder(), $"{file.Name}"), "extension");
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
								var file = jobject["files"][0]["url"].ToString();
								var path = plugin.ExistentPlugin == null ? Path.Combine(Defines.GetScriptsFolder(), plugin.File) : plugin.ExistentPlugin.FilePath;

								core.webrequest.Enqueue(file, null, (_, source) =>
								{
									plugin.IsBusy = false;

									Singleton.Puts($"Downloaded {plugin.Name}");
									OsEx.File.Move(path, Path.Combine(Defines.GetScriptsFolder(), "backups", plugin.File));
									OsEx.File.Create(path, source);
								}, core, headers: _headers);

							}, core, headers: _headers);
							break;
					}
				}
				else
				{
					var path = plugin.ExistentPlugin == null ? Path.Combine(Defines.GetScriptsFolder(), plugin.File) : plugin.ExistentPlugin.FilePath;
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
							OsEx.File.Move(path, Path.Combine(Defines.GetScriptsFolder(), "backups", plugin.File));
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
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase) ||
				                                                x.Name.Equals(id, StringComparison.CurrentCultureIgnoreCase) ||
				                                                Path.GetFileNameWithoutExtension(x.File).Equals(id, StringComparison.CurrentCultureIgnoreCase));
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
			public override string Tagline => "A large platform for free plugins curated by the Oxide team.";

			public override string BarInfo => $"{FetchedPlugins.Count:n0} free";

			public override string ListEndpoint => "https://umod.org/plugins/search.json?page=[ID]&sort=title&sortdir=asc&categories%5B0%5D=universal&categories%5B1%5D=rust";
			public override string DownloadEndpoint => "https://umod.org/plugins/[ID].cs";
			public override string PluginLookupEndpoint => "https://umod.org/plugins/[ID]/latest.json";

			public global::Oxide.Core.Libraries.WebRequests.WebRequest FetchingRequest;
			public global::Oxide.Core.Libraries.WebRequests.WebRequest FetchingPageRequest;
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
						p.PreferredVendor = VendorTypes.uMod;

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
				FetchingTimer = Community.Runtime.Core.timer.In(5f, () => FetchPage(page + 1, maxPage, callback));
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
			public override string Tagline => "All actively loaded plugins. Items with no metadata most likely don't exist on the public vendors.";

			public override float LogoRatio => 0.23f;
			public override string ListEndpoint => string.Empty;
			public override string DownloadEndpoint => string.Empty;
			public override string BarInfo => $"{FetchedPlugins.Count:n0} loaded";

			public override bool CanRefresh => false;

			private string[] _defaultTags = ["carbon", "oxide"];

			public override void CheckMetadata(string id, Action callback)
			{
			}

			public override void Download(string id, Action onTimeout = null)
			{
			}

			public override void Uninstall(string id)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase) ||
				                                                x.Name.Equals(id, StringComparison.CurrentCultureIgnoreCase) ||
				                                                Path.GetFileNameWithoutExtension(x.File).Equals(id, StringComparison.CurrentCultureIgnoreCase));
				ModLoader.UninitializePlugin(plugin.ExistentPlugin);
				OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Defines.GetScriptsFolder(), "backups", plugin.ExistentPlugin.FileName));
				plugin.ExistentPlugin = null;
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
				using var temp = Pool.Get<PooledList<Plugin>>();
				temp.AddRange(FetchedPlugins);

				foreach (var plugin in temp)
				{
					if (plugin.ExistentPlugin == null || !plugin.ExistentPlugin.HasInitialized)
					{
						FetchedPlugins.Remove(plugin);
					}
				}

				foreach (var package in ModLoader.Packages)
				{
					foreach (var plugin in package.Plugins)
					{
						if (plugin.IsCorePlugin) continue;

						var codefling = CodeflingInstance.FetchedPlugins.FirstOrDefault(x => x.ExistentPlugin == plugin);
						var umod = uModInstance?.FetchedPlugins.FirstOrDefault(x => x.ExistentPlugin == plugin);
						var installed = FetchedPlugins.FirstOrDefault(x => x.ExistentPlugin == plugin);

						if (installed == null)
						{
							installed = new Plugin
							{
								Name = plugin.Name,
								Author = plugin.Author,
								Version = plugin.Version.ToString(),
								ExistentPlugin = plugin,
								Description = "This is an unlisted plugin.",
								Tags = _defaultTags,
								File = plugin.FileName,
								Id = plugin.Name,
								UpdateDate = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
								Rating = -1
							};
							FetchedPlugins.Add(installed);
						}

						installed.TryMarkFoundOn(umod);
						installed.TryMarkFoundOn(codefling);
						if (installed.PreferredVendor == VendorTypes.Installed && installed.AvailableOn != null && installed.AvailableOn.Count > 0)
						{
							var initialVendorPlugin = installed.AvailableOn[0];
							installed.PreferredVendor = initialVendorPlugin.PreferredVendor;
							installed.PreferredVendorPlugin = initialVendorPlugin;
						}
					}
				}

				PriceData = FetchedPlugins.OrderBy(x => x.OriginalPrice);
				AuthorData = FetchedPlugins.OrderBy(x => x.Author);
				InstalledData = FetchedPlugins.Where(x => x.IsInstalled());
				OutOfDateData = FetchedPlugins.Where(x => x.IsInstalled() && !x.IsUpToDate());
				OwnedData = FetchedPlugins.OrderBy(x => x.Owned);
			}

			public void Save()
			{
			}
		}

		[ProtoContract]
		public class ServerOwner
		{
			public static ServerOwner Singleton { get; internal set; } = new();

			[ProtoMember(1)]
			public List<string> FavouritePlugins = new();

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
			public VendorTypes PreferredVendor;

			[ProtoIgnore]
			public List<Plugin> AvailableOn;

			[ProtoIgnore]
			public RustPlugin ExistentPlugin;

			internal Plugin PreferredVendorPlugin;
			internal bool IsBusy;

			[ProtoIgnore]
			public bool HasRating => Rating != -1;

			[ProtoIgnore]
			public bool HasPrice => OriginalPrice != "Null";

			public Vendor GetPreferredVendor()
			{
				return GetVendor(PreferredVendor);
			}
			public void SetPreferredVendor(VendorTypes vendor)
			{
				PreferredVendor = vendor;
				PreferredVendorPlugin = GetVendor(vendor).FetchedPlugins.FirstOrDefault(x => x.Name.Equals(Name));
			}
			public void TryMarkFoundOn(Plugin plugin)
			{
				if (plugin == null)
				{
					return;
				}
				AvailableOn ??= new();
				if (!AvailableOn.Contains(plugin) && !AvailableOn.Contains(this))
				{
					AvailableOn.Add(plugin);
				}
			}

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
				return !string.IsNullOrEmpty(OriginalPrice) && OriginalPrice != "FREE" && OriginalPrice != "Null";
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
		var player = ap.Player;
		var tab = Singleton.GetTab(ap.Player);
		var vendorName = args.GetString(0);

		if (vendorName == "LIST")
		{
			ap.SetStorage(tab, "listview", true);
			tab.OnChange.Invoke(ap, tab);
			Singleton.Draw(player);
			return;
		}

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
		var vendorType = ap.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed);
		var vendor = PluginsTab.GetVendor(vendorType);
		var pluginName = args.Args.Skip(1).ToString(" ").Replace("\"", string.Empty).Trim();
		var tabPlugin = ap.GetStorage<PluginsTab.Plugin>(tab, "plugin") ?? vendor.FetchedPlugins.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.File).Equals(pluginName));
		var mainTabPlugin = tabPlugin;
		if (tabPlugin.PreferredVendorPlugin != null)
		{
			tabPlugin = tabPlugin.PreferredVendorPlugin;
		}
		var plugin = tabPlugin.ExistentPlugin;
		var arg = new string[args.Args.Length];
		Array.Copy(args.Args, arg, args.Args.Length);

		switch (arg[0])
		{
			case "0":
				tabPlugin.GetPreferredVendor().Download(pluginName, () => Singleton.Draw(args.Player()));
				Array.Clear(arg, 0, arg.Length);
				break;
			case "1":
				tab.CreateDialog($"Are you sure you want to update '{ap.GetStorage<PluginsTab.Plugin>(tab, "selectedplugin").Name}'?", ap =>
				{
					tabPlugin.GetPreferredVendor().Download(pluginName, () => Singleton.Draw(args.Player()));
					Array.Clear(arg, 0, arg.Length);
				}, null);
				break;
			case "2":
				tab.CreateDialog($"Are you sure you want to uninstall '{ap.GetStorage<PluginsTab.Plugin>(tab, "selectedplugin").Name}'?", ap =>
				{
					Singleton.Puts($"Uninstalling {pluginName} on {vendor?.GetType().Name}");
					tabPlugin.GetPreferredVendor().Uninstall(pluginName);
					Array.Clear(arg, 0, arg.Length);
				}, null);
				break;
			case "3":
			{
				plugin ??= vendor.FetchedPlugins.FirstOrDefault(x => x.Id.Equals(pluginName))?.ExistentPlugin;
				var path = Path.Combine(Defines.GetConfigsFolder(), plugin.Config.Filename);
				if (OsEx.File.Exists(path))
				{
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
				}

				else
				{
					args.ReplyWith($"Config file not found at '{path}'");
				}
				Array.Clear(arg, 0, arg.Length);
				break;
			}
			case "4":
			{
				plugin ??= vendor.FetchedPlugins.FirstOrDefault(x => x.Id.Equals(pluginName)).ExistentPlugin;
				if (plugin != null)
				{
					plugin.ProcessorProcess.MarkDirty();
					Community.Runtime.Core.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
				}
				break;
			}
			case "5":
			{
				plugin ??= vendor.FetchedPlugins.FirstOrDefault(x => x.Id.Equals(pluginName)).ExistentPlugin;
				Singleton.SetTab(ap.Player, LangEditor.Make(plugin,
					(ap) =>
					{
						Community.Runtime.Core.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
					}));
				break;
			}
			case "10":
			{
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
				break;
			}
			case "11":
			{
				var fileName = Path.GetFileNameWithoutExtension(tabPlugin.File);
				var isLoadable = !string.IsNullOrEmpty(CorePlugin.GetPluginFile(fileName).Path);
				if (tabPlugin.IsInstalled())
				{
					Run(Option.Server, $"c.unload \"{fileName}\"");
					Singleton.Draw(player);
				}
				else if(isLoadable)
				{
					Run(Option.Server, $"c.load \"{fileName}\"");
					Singleton.Draw(player);
				}
				break;
			}
			case "12":
			{
				mainTabPlugin.SetPreferredVendor(mainTabPlugin.AvailableOn.FirstOrDefault(x => x.PreferredVendor != mainTabPlugin.PreferredVendor)!.PreferredVendor);
				Singleton.Draw(player);
				break;
			}
		}

		Singleton.Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.page")]
	private void PluginBrowserPage(Arg arg)
	{
		var player = arg.Player();
		var ap = Singleton.GetPlayerSession(player);
		var page = ap.GetOrCreatePage(230);
		var offset = arg.GetInt(0);
		var currentPage = page.CurrentPage;

		switch (offset)
		{
			case 0:
			{
				page.CurrentPage = 0;
				break;
			}

			case 1:
			{
				page.CurrentPage++;

				if(page.CurrentPage > page.TotalPages - 1)
				{
					page.CurrentPage = 0;
				}
				break;
			}

			case -1:
			{
				page.CurrentPage--;

				if (page.CurrentPage < 0)
				{
					page.CurrentPage = page.TotalPages - 1;
				}
				break;
			}

			case -2:
			{
				page.CurrentPage = 0;
				break;
			}

			case -3:
			{
				page.CurrentPage = page.TotalPages - 1;
				break;
			}

			default:
			{
				page.CurrentPage = offset - 1;
				break;
			}
		}

		if (page.CurrentPage <= 0)
		{
			page.CurrentPage = 0;
		}
		else if (page.CurrentPage > page.TotalPages)
		{
			page.CurrentPage = page.TotalPages - 1;
		}

		ap.SetStorage(ap.SelectedTab, "page", page.CurrentPage);
		PluginsTab.DropdownShow = false;

		if(currentPage != page.CurrentPage)
		{
			Singleton.Draw(player);
		}
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
		ap.SetStorage(tab, "page", 0);

		if (search == "Search...")
		{
			ap.SetStorage(tab, "search", string.Empty);
		}

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

		tab.CreateDialog($"Are you sure you want to fetch the {vendor.Type} plugin list?", ap =>
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

			Singleton.Draw(ap.Player);
		}, null);
		Singleton.Draw(ap.Player);
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.selectplugin")]
	private void PluginBrowserSelectPlugin(Arg arg)
	{
		var session = Singleton.GetPlayerSession(arg.Player());
		var tab = session.SelectedTab;
		var vendor = PluginsTab.GetVendor(session.GetStorage(tab, "vendor", PluginsTab.VendorTypes.Installed));
		var pluginName = arg.FullString.Replace("\"", string.Empty).Trim();
		var plugin = vendor.FetchedPlugins.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.File).Equals(pluginName, StringComparison.CurrentCultureIgnoreCase));
		try
		{
			if (plugin.PreferredVendorPlugin != null)
			{
				plugin = plugin.PreferredVendorPlugin;
			}
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed {pluginName}", ex);
			return;
		}
		session.SetStorage(tab, "selectedplugin", plugin);

		const string accentReadableColor = "0.8 0.8 0.8 0.6";
		const float fadeTime = 1f;
		const string starEmpty = "☆";
		const string starFull = "★";

		using var cui = new CUI(Singleton.Handler);
		using var update = cui.UpdatePool();

		if (!DateTime.TryParse(plugin.Date, out var date))
		{
			date = DateTime.Now;
		}

		update.Add(cui.UpdatePanel("selectedpluginpnl", "0.1 0.1 0.1 0.5", xMin: 0.0001f, xMax: 1f, yMin: 0.0001f, yMax: 1, blur: false));
		update.Add(cui.UpdateClientImage("selectedpluginicn", url: plugin.Image, "1 1 1 0.85", xMin: 0, xMax: 1, yMin: 1, yMax: 1, OyMin: -1000, fadeIn: fadeTime));
		update.Add(cui.UpdateText("selectedpluginname", Cache.CUI.WhiteColor, text: plugin.Name, 35, xMin: 0.05f, yMin: 1, OyMin: -400, font: CUI.Handler.FontTypes.RobotoCondensedBold, align: TextAnchor.LowerLeft, fadeIn: fadeTime));
		update.Add(cui.UpdateText("selectedpluginprice", plugin.IsPaid() ? "#e0e344" : "#44e3db", text: plugin.IsPaid() ? $"${plugin.OriginalPrice.ToFloat():0.00}" : "FREE", 25, xMax: 0.925f, yMin: 1, OyMin: -400, font: CUI.Handler.FontTypes.RobotoCondensedBold, align: TextAnchor.LowerRight, fadeIn: fadeTime));
		update.Add(cui.UpdateText("selectedpluginchlog", accentReadableColor, text: plugin.Changelog, 13, align: TextAnchor.UpperLeft, fadeIn: fadeTime));
		update.Add(cui.UpdateText("selectedplugindesc", accentReadableColor, text: plugin.Description, 12, xMin: 0.05f, xMax: 0.95f, yMin: 1, OyMin: -470, OyMax: -420, font: CUI.Handler.FontTypes.RobotoCondensedRegular, align: TextAnchor.UpperLeft, fadeIn: fadeTime));
		update.Add(cui.UpdateText("selectedpluginrdate", accentReadableColor, text: $"{date.Day} {date:MMMM}, {date.Year:0000}", 15, yMax: 0.5f, align: TextAnchor.MiddleCenter, font: CUI.Handler.FontTypes.RobotoCondensedRegular, fadeIn: fadeTime));
		update.Add(cui.UpdateText("selectedplugininfo", color: accentReadableColor, text: $"by <b>{(plugin.Author)}</b>  <b>•</b>  v{plugin.Version}  <b>•</b>  Updated on {plugin.UpdateDate}  <b>•</b>  {plugin.DownloadCount:n0} downloads", 12, xMin: 0.05f, xMax: 0.95f, yMin: 1, OyMin: -450, OyMax: -400, align: TextAnchor.UpperLeft, fadeIn: fadeTime));

		var builder = Facepunch.Pool.Get<StringBuilder>();

		for (int i = 0; i < 5; i++)
		{
			builder.Append(plugin.Rating <= i ? starEmpty : starFull);
		}

		update.Add(cui.UpdateText("selectedpluginrating", accentReadableColor, builder.ToString(), 18, xMin: 0.3f, xMax: 0.7f, yMin: 0.7f, yMax: 0.75f, align: TextAnchor.UpperCenter));
		Facepunch.Pool.FreeUnmanaged(ref builder);

		if (plugin.IsInstalled())
		{
			update.Add(cui.UpdateProtectedButton("selectedplugin_b1", "#b84242", Cache.CUI.BlankColor, text: string.Empty, 0, align: TextAnchor.LowerLeft, command: $"pluginbrowser.interact 2 \"{Path.GetFileNameWithoutExtension(plugin.File)}\""));
			update.Add(cui.UpdateText("selectedplugin_b1_txt", "#f7a3a3", text: "UNINSTALL", 12, align: TextAnchor.MiddleCenter, xMin: 0.2f, font: CUI.Handler.FontTypes.RobotoCondensedBold));
			update.Add(cui.UpdateImage("selectedplugin_b1_icn", "trashcan", "#f7a3a3"));
			update.Add(cui.UpdateImage("selectedplugin_b1_fade", "fade", Cache.CUI.WhiteColor));

			update.Add(cui.UpdateProtectedButton("selectedplugin_b2", "#b84242", Cache.CUI.BlankColor, text: string.Empty, 0, align: TextAnchor.LowerLeft, command: $"pluginbrowser.interact 11 \"{Path.GetFileNameWithoutExtension(plugin.File)}\""));
			update.Add(cui.UpdateText("selectedplugin_b2_txt", "#f7a3a3", text: "UNLOAD", 12, align: TextAnchor.MiddleCenter, xMin: 0.2f, font: CUI.Handler.FontTypes.RobotoCondensedBold));
			update.Add(cui.UpdateImage("selectedplugin_b2_icn", "installed", "#f7a3a3"));
			update.Add(cui.UpdateImage("selectedplugin_b2_fade", "fade", Cache.CUI.WhiteColor));

			update.Add(cui.UpdateProtectedButton("selectedplugin_b3", "0.2 0.2 0.2 0.8", Cache.CUI.BlankColor, text: string.Empty, 0, align: TextAnchor.LowerLeft, command: $"pluginbrowser.interact 3 \"{Path.GetFileNameWithoutExtension(plugin.File)}\""));
			update.Add(cui.UpdateText("selectedplugin_b3_txt", "0.8 0.8 0.8 0.8", text: "CONFIG", 12, align: TextAnchor.MiddleCenter, xMin: 0.2f, font: CUI.Handler.FontTypes.RobotoCondensedBold));
			update.Add(cui.UpdateImage("selectedplugin_b3_icn", "file", "0.8 0.8 0.8 0.8"));
			update.Add(cui.UpdateImage("selectedplugin_b3_fade", "fade", Cache.CUI.WhiteColor));

			update.Add(cui.UpdateProtectedButton("selectedplugin_b4", "0.2 0.2 0.2 0.8", Cache.CUI.BlankColor, text: string.Empty, 0, align: TextAnchor.LowerLeft, command: $"pluginbrowser.interact 5 \"{Path.GetFileNameWithoutExtension(plugin.File)}\""));
			update.Add(cui.UpdateText("selectedplugin_b4_txt", "0.8 0.8 0.8 0.8", text: "LANG", 12, align: TextAnchor.MiddleCenter, xMin: 0.2f, font: CUI.Handler.FontTypes.RobotoCondensedBold));
			update.Add(cui.UpdateImage("selectedplugin_b4_icn", "translate", "0.8 0.8 0.8 0.8"));
			update.Add(cui.UpdateImage("selectedplugin_b4_fade", "fade", Cache.CUI.WhiteColor));

			var isOutdated = !plugin.IsUpToDate();
			if (plugin.IsPaid() && !plugin.Owned)
			{
				isOutdated = false;
			}
			update.Add(cui.UpdateProtectedButton("selectedplugin_b5", $"0.2 0.2 0.2 {(isOutdated ? 0.8f : 0.2f)}", Cache.CUI.BlankColor, text: string.Empty, 0, align: TextAnchor.LowerLeft, command: isOutdated ? $"pluginbrowser.interact 1 \"{Path.GetFileNameWithoutExtension(plugin.File)}\"" : string.Empty));
			update.Add(cui.UpdateText("selectedplugin_b5_txt", "0.8 0.8 0.8 0.8", text: "UPDATE", 12, align: TextAnchor.MiddleCenter, xMin: 0.2f, font: CUI.Handler.FontTypes.RobotoCondensedBold));
			update.Add(cui.UpdateImage("selectedplugin_b5_icn", "clouddl", "0.8 0.8 0.8 0.8"));
			update.Add(cui.UpdateImage("selectedplugin_b5_fade", "fade", Cache.CUI.WhiteColor));
		}
		else
		{
			var canDownload = plugin.GetPreferredVendor() != PluginsTab.LocalInstance && !plugin.IsPaid() || plugin.Owned || (plugin.AvailableOn != null && plugin.AvailableOn.Count > 1);
			update.Add(cui.UpdateProtectedButton("selectedplugin_b1", !canDownload ? CUI.HexToRustColor("#8db842", 0.4f) : "#8db842", Cache.CUI.BlankColor, text: string.Empty, 0, align: TextAnchor.LowerLeft, command: canDownload ? $"pluginbrowser.interact 0 \"{Path.GetFileNameWithoutExtension(plugin.File)}\"" : string.Empty));
			update.Add(cui.UpdateText("selectedplugin_b1_txt", "#d9f7a3", text: canDownload ? "DOWNLOAD" : "CAN'T DOWNLOAD", 12, align: TextAnchor.MiddleCenter, xMin: 0.2f, font: CUI.Handler.FontTypes.RobotoCondensedBold));
			update.Add(cui.UpdateImage("selectedplugin_b1_icn", "clouddl", "#d9f7a3"));
			update.Add(cui.UpdateImage("selectedplugin_b1_fade", "fade", Cache.CUI.BlankColor));

			var isLoadable = !string.IsNullOrEmpty(CorePlugin.GetPluginFile(Path.GetFileNameWithoutExtension(plugin.File)).Path);

			update.Add(cui.UpdateProtectedButton("selectedplugin_b2", !isLoadable ? CUI.HexToRustColor("#8db842", 0.4f) : "#8db842", Cache.CUI.BlankColor, text: string.Empty, 0, align: TextAnchor.LowerLeft, command: $"pluginbrowser.interact 11 \"{Path.GetFileNameWithoutExtension(plugin.File)}\""));
			update.Add(cui.UpdateText("selectedplugin_b2_txt", "#d9f7a3", text: "LOAD", 12, align: TextAnchor.MiddleCenter, xMin: 0.2f, font: CUI.Handler.FontTypes.RobotoCondensedBold));
			update.Add(cui.UpdateImage("selectedplugin_b2_icn", "installed", "#d9f7a3"));
			update.Add(cui.UpdateImage("selectedplugin_b2_fade", "fade", Cache.CUI.WhiteColor));

			update.Add(cui.UpdateProtectedButton("selectedplugin_b3", Cache.CUI.BlankColor, Cache.CUI.BlankColor, text: string.Empty, 0, align: TextAnchor.LowerLeft));
			update.Add(cui.UpdateText("selectedplugin_b3_txt", Cache.CUI.BlankColor, text: string.Empty, 0));
			update.Add(cui.UpdateImage("selectedplugin_b3_icn", "trashcan", Cache.CUI.BlankColor));
			update.Add(cui.UpdateImage("selectedplugin_b3_fade", "fade", Cache.CUI.BlankColor));

			update.Add(cui.UpdateProtectedButton("selectedplugin_b4", Cache.CUI.BlankColor, Cache.CUI.BlankColor, text: string.Empty, 0, align: TextAnchor.LowerLeft));
			update.Add(cui.UpdateText("selectedplugin_b4_txt", Cache.CUI.BlankColor, text: string.Empty, 0));
			update.Add(cui.UpdateImage("selectedplugin_b4_icn", "trashcan", Cache.CUI.BlankColor));
			update.Add(cui.UpdateImage("selectedplugin_b4_fade", "fade", Cache.CUI.BlankColor));

			update.Add(cui.UpdateProtectedButton("selectedplugin_b5", Cache.CUI.BlankColor, Cache.CUI.BlankColor, text: string.Empty, 0, align: TextAnchor.LowerLeft));
			update.Add(cui.UpdateText("selectedplugin_b5_txt", Cache.CUI.BlankColor, text: string.Empty, 0));
			update.Add(cui.UpdateImage("selectedplugin_b5_icn", "trashcan", Cache.CUI.BlankColor));
			update.Add(cui.UpdateImage("selectedplugin_b5_fade", "fade", Cache.CUI.BlankColor));
		}

		update.Send(arg.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("pluginbrowser.deselectplugin")]
	private void PluginBrowserDeselectPlugin(Arg arg)
	{
		using var cui = new CUI(Singleton.Handler);
		using var update = cui.UpdatePool();
		update.Add(cui.UpdatePanel("selectedpluginpnl", Cache.CUI.BlankColor, xMin: 0, xMax: 0, yMin: -1000, yMax: -1000, blur: false));
		update.Send(arg.Player());
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
		var player = ap.Player;
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

					Singleton.Draw(player);

					if (vendor is PluginsTab.IVendorStored store)
					{
						store.Save();
					}
				}, null);
			}
			else
			{
				auth.AuthCode = Extensions.StringEx.Truncate(Guid.NewGuid().ToString(), 6).ToUpper();
				auth.User = new PluginsTab.LoggedInUser { PendingAccessToken = true };

				var currentCode = auth.AuthCode;
				var core = Community.Runtime.Core;

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
							Singleton.Draw(player);

							auth.RefreshUser(ap);
						});

						Singleton.Draw(player);
					});
				});
			}

			Singleton.Draw(args.Player());
		}
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
