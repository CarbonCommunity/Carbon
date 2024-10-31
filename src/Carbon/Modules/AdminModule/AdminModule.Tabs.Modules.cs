#if !MINIMAL

using Newtonsoft.Json;

namespace Carbon.Modules;

public partial class AdminModule
{
	public class ModulesTab
	{
		public enum SortTypes
		{
			Loaded,
			Name,
			Enabled
		}

		public static string[] SortTypeNames = Enum.GetNames(typeof(SortTypes));

		public static Tab Get()
		{
			var tab = (Tab)null;
			void Draw(PlayerSession ap)
			{
				tab.AddColumn(0, true);
				tab.AddColumn(1, true);

				var searchInput = ap.GetStorage<string>(tab, "search")?.ToLower();

				tab.AddInput(0, "Search", ap => searchInput, (ap, args) =>
				{
					ap.SetStorage(tab, "search", args.ToString(" "));
					Draw(ap);
				});

				var sort = (SortTypes)ap.GetStorage(tab, "sorttype", 0);
				var sortFlip = ap.GetStorage(tab, "sortflip", false);

				tab.AddDropdown(0, "Sorting", ap => (int)sort, (ap, index) =>
				{
					if ((int)sort != index)
					{
						ap.SetStorage(tab, "sortflip", false);
						ap.SetStorage(tab, "sorttype", index);
					}
					else
					{
						ap.SetStorage(tab, "sortflip", !sortFlip);
					}

					Draw(ap);
				}, SortTypeNames);

				tab.AddName(0, "Core Modules");
				Generate(x => x.ForceEnabled && (ap.HasStorage(tab, "search") && !string.IsNullOrEmpty(searchInput) ? x.Name.ToLower().Contains(searchInput) : true));

				tab.AddName(0, "Other Modules");
				Generate(x => !x.ForceEnabled && (ap.HasStorage(tab, "search") && !string.IsNullOrEmpty(searchInput) ? x.Name.ToLower().Contains(searchInput) : true));

				void Generate(Func<BaseModule, bool> condition)
				{
					IEnumerable<BaseHookable> modules = sort switch
					{
						SortTypes.Name =>  Community.Runtime.ModuleProcessor.Modules.OrderBy(x => x.Name),
						SortTypes.Enabled =>  Community.Runtime.ModuleProcessor.Modules.OrderByDescending(x => x is BaseModule module && module.IsEnabled()),
						_ => Community.Runtime.ModuleProcessor.Modules
					};

					if (sortFlip)
					{
						modules = modules.Reverse();
					}

					foreach (var hookable in modules)
					{
						if (hookable is BaseModule module)
						{
							if (!condition(module)) continue;

							tab.AddButtonArray(0,
								new Tab.OptionButton(hookable.Name, ap =>
								{
									ap.SetStorage(tab, "selectedmodule", module);
									Draw(ap);
									DrawModuleSettings(tab, module, ap);
								}, type: ap => ap.GetStorage<BaseModule>(tab, "selectedmodule") == module ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
								new Tab.OptionButton($"{(module.ForceEnabled ? "Always Enabled" : module.IsEnabled() ? "Enabled" : "Disabled")}", ap =>
								{
									if (module.ForceEnabled) return;

									module.SetEnabled(!module.IsEnabled());
									module.Save();
									ap.SetStorage(tab, "selectedmodule", module);
									Draw(ap);
									DrawModuleSettings(tab, module, ap);
								}, type: ap => module.ForceEnabled ? Tab.OptionButton.Types.Warned : module.IsEnabled() ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
						}
					}
				}
			}

			tab = new Tab("modules", "Modules", Community.Runtime.Core, access: "modules.use", onChange: (ap, tab) =>
			{
				ap.ClearStorage(tab, "selectedmodule");
				Draw(ap);
			});

			return tab;
		}

		internal static string[] _configBlacklist = new[]
		{
			"Version"
		};

		internal static void DrawModuleSettings(Tab tab, BaseModule module, PlayerSession ap)
		{
			tab.ClearColumn(1);

			tab.AddInput(1, "Name", ap => module.Name, null);

			if (!module.ForceEnabled)
			{
				tab.AddToggle(1, "Enabled", ap2 => { module.SetEnabled(!module.IsEnabled()); module.Save(); DrawModuleSettings(tab, module, ap); }, ap2 => module.IsEnabled());
			}

			tab.AddButtonArray(1,
				new Tab.OptionButton("Save", ap => { module.Save(); }),
				new Tab.OptionButton("Load", ap => { module.Load(); }));

			if (Singleton.HasAccess(ap.Player, "modules.config_edit"))
			{
				tab.AddButton(1, "Edit Config", ap =>
				{
					var moduleConfigFile = Path.Combine(Defines.GetModulesFolder(), module.Name, "config.json");
					ap.SelectedTab = ConfigEditor.Make(OsEx.File.ReadText(moduleConfigFile),
						(ap, _) =>
						{
							Singleton.SetTab(ap.Player, "modules");
							Singleton.Draw(ap.Player);
						},
						(ap, jobject) =>
						{
							var wasEnabled = module.IsEnabled();
							OsEx.File.Create(moduleConfigFile, jobject.ToString(Formatting.Indented));
							module.SetEnabled(false);
							module.OnUnload();
							module.Load();
							if(wasEnabled) module.SetEnabled(wasEnabled);

							Singleton.SetTab(ap.Player, "modules");
							Singleton.Draw(ap.Player);
						}, null, blacklist: _configBlacklist);
				});
			}
		}
	}
}

#endif
