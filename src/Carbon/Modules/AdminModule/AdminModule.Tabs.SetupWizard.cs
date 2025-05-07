#if !MINIMAL

using API.Commands;
using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;

namespace Carbon.Modules;

public partial class AdminModule
{
	public class SetupWizard : Tab
	{
		public SetupWizard(string id, string name, RustPlugin plugin, Action<PlayerSession, Tab> onChange = null) : base(id, name, plugin, onChange)
		{
		}

		public static SetupWizard Make()
		{
			var tab = new SetupWizard("setupwizard", "Setup Wizard", Community.Runtime.Core) { IsFullscreen = true };
			tab.Override = (Tab tab, CUI cui, CuiElementContainer container, string panel, PlayerSession ap) =>
			{
				cui.CreateImage(container, panel,
					url: "carbonws",
					color: "1 1 1 0.7",
					xMin: 0.2f, xMax: 0.8f,
					yMin: 0.52f, yMax: 0.71f,
					OyMin: -20, OyMax: -20);
				cui.CreateText(container, panel,
					color: "1 1 1 0.5",
					text: "Welcome to <b>Carbon</b> setup wizard!\nIf you've seen this panel again, your existent settings are not reset.",
					13,
					yMax: 0.495f, OyMin: -20, OyMax: -20, align: TextAnchor.UpperCenter);
				tab.DisplayArrows(cui, tab, container, panel, ap, true);
			};
			return tab;
		}

		internal void DisplayArrows(CUI cui, Tab tab, CuiElementContainer container, string panel, PlayerSession ap, bool centerNext = false)
		{
			cui.CreateProtectedButton(container, panel, "#7d8f32", "1 1 1 1", $"Button   ▶", 9,
				xMin: 0.9f, yMin: 0f, yMax: 0.055f, OxMin: -470, OxMax: -470, OyMin: 145f, OyMax: 145f, command: $"wizard.changepage 1");

			cui.CreateProtectedButton(container, panel, "1 1 1 0.3", "1 1 1 1", $"Skip   ▶▶", 9,
				xMin: 0.9f, yMin: 0f, yMax: 0.055f, OxMin: -370, OxMax: -370, OyMin: 145f, OyMax: 145f, command: $"wizard.changepage -2");
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
				Singleton.SetTab(ap.Player, "carbon");
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
		DataInstance.MarkTabHidden(feature, !DataInstance.IsTabHidden(feature));
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
