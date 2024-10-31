#if !MINIMAL

using API.Commands;
using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;

namespace Carbon.Modules;

public partial class AdminModule
{
	public class SetupWizard : Tab
	{
		internal List<Page> Pages = new();

		public SetupWizard(string id, string name, RustPlugin plugin, Action<PlayerSession, Tab> onChange = null) : base(id, name, plugin, onChange)
		{
		}

		public static SetupWizard Make()
		{
			var tab = new SetupWizard("setupwizard", "Setup Wizard", Community.Runtime.Core) { IsFullscreen = true };
			tab.Override = tab.Draw;

			tab.Pages.Add(new Page("Main", (cui, t, container, panel, ap) =>
			{
				cui.CreateImage(container, panel,
					url: "carbonws",
					color: "1 1 1 0.7",
					xMin: 0.2f, xMax: 0.8f,
					yMin: 0.52f, yMax: 0.71f,
					OyMin: -20, OyMax: -20);
				cui.CreateText(container, panel,
					color: "1 1 1 0.5",
					text: "Welcome to <b>Carbon</b> setup wizard!\nIf you've seen this panel again, your existent settings are not reset.", 13,
					yMax: 0.495f, OyMin: -20, OyMax: -20, align: TextAnchor.UpperCenter);
				tab.DisplayArrows(cui, tab, container, panel, ap, true);
			}));

			tab.Pages.Add(new Page("Getting Started", (cui, t, container, panel, ap) =>
			{
				tab.InfoTemplate(cui, t, container, panel, ap, "What is Carbon?",
					$"Carbon is a robust self-updating framework designed with performance and stability in mind. It has complete backward compatibility with all Oxide plugins and uses the <b>latest C#</b> and <b>Harmony</b> libraries to give developers the tools to let their creativity flourish!\n\n" +
					$"\n{Header("Features", 1)}" +
					$"\n<b>AUTO-UPDATES</b>" +
					$"\nCarbon updates itself and all dynamic hook DLLs at runtime, so you do not need to restart your Rust server when new builds come out.\n\n" +
					$"<b>C# 10</b>" +
					$"\nCarbon natively supports the latest C# version, with many improvements and optimizations. All plugins get compiled to the latest C# version.\n\n" +
					$"<b>HIGH PERFORMANCE</b>" +
					$"\nThe goal is to make the servers as performant as possible, learn from the community about new ways to do things, and high profiling for tracking any hiccups.\n\n" +
					$"<b>DYNAMIC HOOKS</b>" +
					$"\nA crucial distinction that makes Carbon so much different from other frameworks is that although we support all Oxide hooks (appx. +600), they are not called unless a plugin subscribes to them.\n\n" +
					$"<b>HARMONY 2.0</b>" +
					$"\nCarbon runs on Harmony v2.0, which introduces higher performance Pre-&-Post-fixes and even faster transpiler calls, which 99% of running hooks use.", "");
			}));
			tab.Pages.Add(new Page("Modules", (cui, t, container, panel, ap) =>
			{
				tab.InfoTemplate(cui, t, container, panel, ap,
					"Modules",
					"Modules are built-in and out of the box ready-to-use systems that are useful for most servers." +
					"\nThey're heavily inspired by public free plugins on the Rust modding market providing convenience and maintenance for your most important needs.", "");
			}));
			tab.Pages.Add(new Page("Gather Manager", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"Gather Manager Module",
					"The module allows you to modify the processing modifiers of Quarries, Excavators, Pickup and Gather amounts, globally set or item-specific. This module comes with a variety of useful tools, including custom recycler speed.",
					"", FindModule("GatherManager"));
			}));
			tab.Pages.Add(new Page("Stack Manager", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"Stack Manager Module",
					"High performance will allow to set custom item stacks based on:\n" +
					"\n• Item name" +
					"\n• Item category" +
					"\n• Blacklisted items (useful when using categories)", "", FindModule("StackManager"));
			}));
			tab.Pages.Add(new Page("Vanish", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"Vanish Module",
					"A very lightweight auth-level based system allowing you to become invisible, with various settings in the config file.", "", FindModule("Vanish"));
			}));
			tab.Pages.Add(new Page("Moderation Tools", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"Moderation Tools Module",
					"This module is a bundle of very helpful and often usable moderation tools that can grant the ability to players with regular authority level to use noclip and god-mode and nothing else (use the 'carbon.admin' permission to allow the use of the '/cadmin' command).\n" +
					"There's also a permission ('carbon.cmod') that allows players to kick, ban or mute players with defined reasons.", "", FindModule("ModerationTools"));
			}));
			tab.Pages.Add(new Page("Circular Networking", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"Circular Networking Module",
					"This module modifies Rust's network system to use a more performant circular version. Using this on maximum settings with lower network distance offers an extreme performance boost over vanilla settings, and it has allowed huge server networks to push an extra 100 server pop on wipe with minimal lag.\n" +
					"It's not a catch-all solution, but it works well for network lag with almost no downside.", "", FindModule("CircularNetworking"));

			}));
			tab.Pages.Add(new Page("Plugin Browser", (cui, t, container, panel, ap) =>
			{
				cui.CreateClientImage(container, panel, "https://carbonmod.gg/assets/media/cui/wizard/pluginstab_sample.jpg", "1 1 1 0.7",
					xMin: 0.35f, xMax: 0.65f, yMin: 0.1f, yMax: 0.425f, fadeIn: 1f);

				var cfPaidPluginCount = PluginsTab.CodeflingInstance?.FetchedPlugins.Count(x => x.IsPaid());
				var cfFreePluginCount = PluginsTab.CodeflingInstance?.FetchedPlugins.Count(x => !x.IsPaid());
				var uModFreePluginCount = PluginsTab.uModInstance?.FetchedPlugins.Count(x => !x.IsPaid());

				tab.InternalFeatureInfoTemplate(cui, t, container, panel, ap,
					"Plugin Browser",
					"The plugin browser allows you to explore Codefling and uMod plugins all within the game. Manage, stay up-to-date and download new plugins in the very intuitive UI. Filter all plugins by searching, using tags or sort the lists based on your liking." +
					$"\n\n{Header("Codefling", 1)} ({cfFreePluginCount:n0} free, {cfPaidPluginCount:n0} paid)" +
					$"\nThe Codefling integration allows you to download free available files and it supports an in-game login system which allows you to download or update your paid files in-game." +
					$"\nPurchasing will not be available but you may browse new files you're interested in and add them to cart through the game as well." +
					$"\n\n{Header("uMod", 1)} ({uModFreePluginCount:n0} free)" +
					$"\nBrowse the 1.5K catalogue full of free, Rust- and covalence supported plugins.",
					"A very minimum amount of plugins currently are not compatible, due to them being out of date (on Oxide too) or requiring external DLLs that are Oxide-only compatible; meaning that it's the author's responsability to add Carbon support.",
					"plugins");
			}));
			tab.Pages.Add(new Page("Whitelist", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"Whitelist Module",
					"A very basic system that only grants players access to a server based on the 'whitelist.bypass' permission or 'whitelisted' group.", "",
					FindModule("Whitelist"));
			}));

			tab.Pages.Add(new Page("Finalize", (cui, t, container, panel, ap) =>
			{
				Analytics.admin_module_wizard(Analytics.WizardProgress.Walkthrough);

				Singleton.DataInstance.WizardDisplayed = true;
				Singleton.GenerateTabs();
				Community.Runtime.Core.NextTick(() => Singleton.SetTab(ap.Player, 0));
			}));

			return tab;
		}

		internal void Draw(Tab tab, CUI cui, CuiElementContainer container, string panel, PlayerSession ap)
		{
			var page = ap.GetStorage(tab, "page", 0);
			Pages[page].Draw?.Invoke(cui, tab, container, panel, ap);
		}

		internal void InfoTemplate(CUI cui, Tab tab, CuiElementContainer container, string panel, PlayerSession ap, string title, string content, string hint)
		{
			cui.CreateImage(container, panel, "carbonws", "0 0 0 0.1", xMin: 0.75f, xMax: 0.95f, yMin: 0.875f, yMax: 0.95f);

			var mainTitle = cui.CreatePanel(container, panel, "0 0 0 0.5", xMin: 0.05f, xMax: ((float)title.Length).Scale(0, 7, 0.075f, 0.18f), yMin: 0.875f, yMax: 0.95f);
			cui.CreateText(container, mainTitle,
				color: "1 1 1 1", text: $"<b>{title.ToUpper()}</b>", 25, align: TextAnchor.MiddleCenter, fadeIn: 2f);

			cui.CreatePanel(container, panel, "0 0 0 0.5", xMin: 0.05f, xMax: 0.875f, yMin: 0.1f, yMax: 0.86f, fadeIn: 1f);
			cui.CreateText(container, panel, "1 1 1 0.5", content +
				$"\n\n{(string.IsNullOrEmpty(hint) ? "" : Header("Hint", 2))}" +
				$"\n{hint}", 12, xMin: 0.06f, xMax: 0.85f, yMax: 0.84f, align: TextAnchor.UpperLeft);

			DisplayArrows(cui, tab, container, panel, ap);
		}
		internal void ModuleInfoTemplate(CUI cui, Tab tab, CuiElementContainer container, string panel, PlayerSession player, string title, string content, string hint, BaseModule module)
		{
			if (module == null) return;

			var consoleCommands = Community.Runtime.CommandManager.ClientConsole.Where(x => x.Reference == module && !x.HasFlag(CommandFlags.Hidden));
			var chatCommands = Community.Runtime.CommandManager.Chat.Where(x => x.Reference == module && !x.HasFlag(CommandFlags.Hidden));
			var consoleCommandCount = consoleCommands.Count();
			var chatCommandCount = chatCommands.Count();

			content = $"The <b>{module.Name}</b> uses <b>{module.Hooks.Count:n0}</b> total {module.Hooks.Count.Plural("hook", "hooks")}, with currently <b>{module.IgnoredHooks.Count:n0}</b> ignored {module.IgnoredHooks.Count.Plural("hook", "hooks")}, " +
				$"and so far has used {module.TotalHookTime.TotalMilliseconds:0.0}ms of server time during those hook calls. " +
				$"This module is {(module.EnabledByDefault ? "enabled" : "disabled")} by default. " +
				$"This module has <b>{consoleCommandCount:n0}</b> console and <b>{chatCommandCount:n0}</b> chat {(consoleCommandCount == 1 && chatCommandCount == 1 ? "command" : "commands")} and will{(!module.ForceModded ? " <b>not</b>" : "")} enforce this server to modded when enabled.{((consoleCommandCount + chatCommandCount) == 0 ? "" : "\n\n")}" +
				((consoleCommandCount > 0 ? $"<b>Console commands:</b> {consoleCommands.Select(x => $"{x.Name}").ToString(", ")}\n" : "") +
				(chatCommandCount > 0 ? $"<b>Chat commands:</b> {chatCommands.Select(x => $"{x.Name}").ToString(", ")}\n" : "") +
				$"\n\n{(string.IsNullOrEmpty(content) ? "" : Header("About", 1))}" +
				$"\n{content}");

			InfoTemplate(cui, tab, container, panel, player, title, content, hint);

			cui.CreateProtectedButton(container, panel, "0.3 0.3 0.3 0.5", "1 1 1 1", "OPEN FOLDER".SpacedString(1), 10,
				xMin: 0.9f, yMin: 0.075f, yMax: 0.125f, OyMin: 60, OyMax: 60, OxMin: 8, OxMax: 8, command: $"wizard.openmodulefolder {module.Name}");

			cui.CreateProtectedButton(container, panel, "0.3 0.3 0.3 0.5", "1 1 1 1", "EDIT CONFIG".SpacedString(1), 10,
				xMin: 0.9f, yMin: 0.075f, yMax: 0.125f, OyMin: 30, OyMax: 30, OxMin: 8, OxMax: 8, command: $"wizard.editmoduleconfig {module.Name}");

			cui.CreateProtectedButton(container, panel, module.IsEnabled() ? "0.4 0.9 0.3 0.5" : "0.1 0.1 0.1 0.5", "1 1 1 1", module.IsEnabled() ? "ENABLED".SpacedString(1) : "DISABLED".SpacedString(1), 10,
				xMin: 0.9f, yMin: 0.075f, yMax: 0.125f, OxMin: 8, OxMax: 8, command: $"wizard.togglemodule {module.Name}");
		}
		internal void InternalFeatureInfoTemplate(CUI cui, Tab tab, CuiElementContainer container, string panel, PlayerSession player, string title, string content, string hint, string feature)
		{
			InfoTemplate(cui, tab, container, panel, player, title, content, hint);

			var isEnabled = IsFeatureEnabled(feature);
			cui.CreateProtectedButton(container, panel, isEnabled ? "0.4 0.9 0.3 0.5" : "0.1 0.1 0.1 0.5", "1 1 1 1", isEnabled ? "ENABLED".SpacedString(1) : "DISABLED".SpacedString(1), 10,
				xMin: 0.9f, yMin: 0.075f, yMax: 0.125f, OxMin: 8, OxMax: 8, command: $"wizard.togglefeature {feature}");
		}
		internal void DisplayArrows(CUI cui, Tab tab, CuiElementContainer container, string panel, PlayerSession ap, bool centerNext = false)
		{
			var page = ap.GetStorage(tab, "page", 0);

			if (page < Pages.Count - 1)
			{
				var nextPage = Pages[page + 1];

				if (centerNext)
				{
					cui.CreateProtectedButton(container, panel, "#7d8f32", "1 1 1 1", $"{nextPage.Title}   ▶", 9,
						xMin: 0.9f, yMin: 0f, yMax: 0.055f, OxMin: -470, OxMax: -470, OyMin: 145f, OyMax: 145f, command: $"wizard.changepage 1");

					cui.CreateProtectedButton(container, panel, "1 1 1 0.3", "1 1 1 1", $"Skip   ▶▶", 9,
						xMin: 0.9f, yMin: 0f, yMax: 0.055f, OxMin: -370, OxMax: -370, OyMin: 145f, OyMax: 145f, command: $"wizard.changepage -2");
				}
				else
				{
					cui.CreateProtectedButton(container, panel, "#7d8f32", "1 1 1 1", $"{nextPage.Title} ▶", 9,
						xMin: 0.9f, yMin: 0f, yMax: 0.055f, OxMin: 8, OxMax: 8, command: $"wizard.changepage 1");
				}
			}

			if (page >= 1)
			{
				var backPage = Pages[page - 1];
				cui.CreateProtectedButton(container, panel, "#7d8f32", "1 1 1 1", $"◀ {backPage.Title}", 9,
					xMin: 0, xMax: 0.1f, yMin: 0f, yMax: 0.055f, OxMin: -9, OxMax: -9, command: $"wizard.changepage -1");
			}
		}

		internal static string Header(string value, int level)
		{
			return level switch
			{
				1 => $"<size=20>{value}</size>",
				2 => $"<size=17>{value}</size>",
				3 => $"<size=14>{value}</size>",
				_ => value
			};
		}

		internal bool IsFeatureEnabled(string feature)
		{
			switch (feature)
			{
				case "plugins":
					return !Singleton.ConfigInstance.DisablePluginsTab;
			}

			return false;
		}

		public class Page
		{
			public string Title;
			public Action<CUI, Tab, CuiElementContainer, string, PlayerSession> Draw;

			public Page() { }
			public Page(string title, Action<CUI, Tab, CuiElementContainer, string, PlayerSession> draw)
			{
				Title = title;
				Draw = draw;
			}
		}
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("wizard.changepage")]
	private void ChangePage(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var tab = GetTab(ap.Player);
		var value = arg.GetInt(0);
		var currentPage = ap.GetStorage(tab, "page", 0);

		if (value == -2)
		{
			Analytics.admin_module_wizard(Analytics.WizardProgress.Skipped);

			ap.SetStorage(tab, "page", 0);
			Singleton.DataInstance.WizardDisplayed = true;
			Singleton.GenerateTabs();
			Community.Runtime.Core.NextTick(() =>
			{
				Save();
				Singleton.SetTab(ap.Player, 0);
				Draw(ap.Player);
			});
		}
		else
		{
			currentPage += value;
			ap.SetStorage(tab, "page", currentPage);
			Community.Runtime.Core.NextFrame(() => Draw(ap.Player));
		}
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("wizard.togglemodule")]
	private void ToggleModule(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var module = FindModule(arg.GetString(0));
		var enabled = module.IsEnabled();

		module.SetEnabled(!enabled);

		Draw(ap.Player);
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("wizard.togglefeature")]
	private void ToggleFeature(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var feature = arg.GetString(0);

		switch (feature)
		{
			case "plugins":
				ConfigInstance.DisablePluginsTab = !ConfigInstance.DisablePluginsTab;
				break;
		}

		Draw(ap.Player);
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("wizard.editmoduleconfig")]
	private void EditModuleConfig(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var module = FindModule(arg.GetString(0));
		var moduleConfigFile = Path.Combine(Defines.GetModulesFolder(), module.Name, "config.json");

		ap.SelectedTab = ConfigEditor.Make(OsEx.File.ReadText(moduleConfigFile),
			(ap, _) =>
			{
				SetTab(ap.Player, SetupWizard.Make());
				Draw(ap.Player);
			},
			(ap, jobject) =>
			{
				OsEx.File.Create(moduleConfigFile, jobject.ToString(Formatting.Indented));
				module.Load();
				SetTab(ap.Player, SetupWizard.Make());
				Draw(ap.Player);
			}, null, fullscreen: true);

		Draw(ap.Player);
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("wizard.openmodulefolder")]
	private void OpenModuleFolder(ConsoleSystem.Arg arg)
	{
		var module = FindModule(arg.GetString(0));
		var ap = GetPlayerSession(arg.Player());

		Application.OpenURL(Path.Combine(Carbon.Core.Defines.GetModulesFolder(), module.Name));

		Draw(ap.Player);
	}
}

#endif
