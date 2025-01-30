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

		private static string[] _sortTypeNames = Enum.GetNames(typeof(SortTypes));

		private static string[] _configBlacklist = new[]
		{
			"Version"
		};

		public static Tab Get()
		{
			var tab = (Tab)null;

			tab = new Tab("modules", "Modules", Community.Runtime.Core, access: "modules.use", onChange: (ap, tab) =>
			{
				Draw(tab, ap);
			});

			return tab;
		}

		static void Draw(Tab tab, PlayerSession ap)
		{
			tab.AddColumn(0, true);
			tab.AddColumn(1, true);

			var searchInput = ap.GetStorage<string>(tab, "search")?.ToLower();

			tab.AddInput(0, "Search", ap => searchInput, (ap, args) =>
			{
				ap.SetStorage(tab, "search", args.ToString(" "));
				Draw(tab, ap);
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

				Draw(tab, ap);
			}, _sortTypeNames);

			tab.AddName(0, "Core Modules");
			Generate(sort, sortFlip, tab,
				x => x.ForceEnabled && (ap.HasStorage(tab, "search") && !string.IsNullOrEmpty(searchInput)
					? x.Name.ToLower().Contains(searchInput)
					: true));

			tab.AddName(0, "Other Modules");
			Generate(sort, sortFlip, tab,
				x => !x.ForceEnabled && (ap.HasStorage(tab, "search") && !string.IsNullOrEmpty(searchInput)
					? x.Name.ToLower().Contains(searchInput)
					: true));

			static void Generate(SortTypes sort, bool sortFlip, Tab tab, Func<BaseModule, bool> condition)
			{
				IEnumerable<BaseHookable> modules = sort switch
				{
					SortTypes.Name => Community.Runtime.ModuleProcessor.Modules.OrderBy(x => x.Name),
					SortTypes.Enabled => Community.Runtime.ModuleProcessor.Modules.OrderByDescending(x =>
						x is BaseModule module && module.IsEnabled()),
					_ => Community.Runtime.ModuleProcessor.Modules
				};

				if (sortFlip)
				{
					modules = modules.Reverse();
				}

				foreach (var hookable in modules)
				{
					if (hookable is not BaseModule module) continue;
					if (!condition(module)) continue;

					var moduleConfigFile = Path.Combine(Defines.GetModulesFolder(), module.Name, "config.json");
					var exists = OsEx.File.Exists(moduleConfigFile);

					tab.AddButtonArray(0,
						new Tab.OptionButton(hookable.Name, ap =>
							{
								Draw(tab, ap);
							},
							type: _ => Tab.OptionButton.Types.None),
						new Tab.OptionButton(
							$"{(module.ForceEnabled ? "Always Enabled" : module.IsEnabled() ? "Enabled" : "Disabled")}",
							ap =>
							{
								if (module.ForceEnabled) return;

								module.SetEnabled(!module.IsEnabled());
								module.Save();
								Draw(tab, ap);
							},
							type: ap => module.ForceEnabled ? Tab.OptionButton.Types.Warned :
								module.IsEnabled() ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
						new Tab.OptionButton("Edit Config", ap =>
							{
								if (!exists)
								{
									return;
								}

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
							},
							type: ap => exists ? Tab.OptionButton.Types.Warned : Tab.OptionButton.Types.None));
				}
			}
		}
	}
}

#endif
