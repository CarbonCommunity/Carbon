#if !MINIMAL

using Newtonsoft.Json;

namespace Carbon.Modules;

public partial class AdminModule
{
	public class LangEditor : Tab
	{
		internal BaseHookable TargetPlugin;
		internal Action<PlayerSession> OnCancel;
		internal const string Spacing = " ";

		public LangEditor(string id, string name, RustPlugin plugin, Action<PlayerSession, Tab> onChange = null) : base(id, name, plugin, onChange)
		{
		}

		public static LangEditor Make(Plugin plugin, Action<PlayerSession> onCancel)
		{
			var tab = new LangEditor("langeditor", "Lang Editor", Community.Runtime.Core)
			{
				TargetPlugin = plugin,
				OnCancel = onCancel,
			};

			tab._draw();
			return tab;
		}

		internal void _draw()
		{
			AddColumn(0, true);
			AddColumn(1, true);

			AddButton(0, "Cancel", ap => { OnCancel?.Invoke(ap); }, ap => OptionButton.Types.Important);

			foreach (var folder in Directory.GetDirectories(Defines.GetLangFolder()))
			{
				var files = Directory.GetFiles(folder);

				if (files.Length == 0)
				{
					continue;
				}

				var pluginFiles = files.Where(x => x.Contains(TargetPlugin.Name, CompareOptions.OrdinalIgnoreCase));

				if (!pluginFiles.Any())
				{
					continue;
				}

				var file = pluginFiles.FirstOrDefault();

				AddButton(0, Path.GetFileName(folder), ap =>
				{
					Singleton.SetTab(ap.Player, ConfigEditor.Make(OsEx.File.ReadText(file),
						(ap, jobject) =>
						{
							Community.Runtime.Core.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
						},
						(ap, jobject) =>
						{
							OsEx.File.Create(file, jobject.ToString(Formatting.Indented));
							Community.Runtime.Core.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
						},
						(ap, jobject) =>
						{
							OsEx.File.Create(file, jobject.ToString(Formatting.Indented));

							if (TargetPlugin is RustPlugin rustPlugin)
							{
								rustPlugin.ProcessorProcess.MarkDirty();
							}

							Community.Runtime.Core.NextTick(() => Singleton.SetTab(ap.Player, "plugins", false));
						}));
				}, ap => OptionButton.Types.Warned);
			}
		}
	}
}

#endif
