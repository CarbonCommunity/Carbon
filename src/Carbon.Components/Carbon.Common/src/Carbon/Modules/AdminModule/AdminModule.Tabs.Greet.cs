#if !MINIMAL

namespace Carbon.Modules;

public partial class AdminModule
{
	public class Greet
	{
		public static Tab Make()
		{
			var tab = new Tab("greet", "Greet", Community.Runtime.Core) { IsFullscreen = true };
			tab.Override = (_, cui, container, panel, _) =>
			{
				cui.CreateImage(container, panel,
					url: "carbonws",
					color: "1 1 1 0.7",
					xMin: 0.2f, xMax: 0.8f,
					yMin: 0.52f, yMax: 0.71f,
					OyMin: -20, OyMax: -20);
				cui.CreateText(container, panel,
					color: "1 1 1 0.5",
					text: "Welcome to <b>Carbon</b>!\n\n<size=12><color=grey>If you've seen this panel again, your existent settings have not been reset.\nFor more information, go to <color=orange>carbonmod.gg</color>.</color></size>",
					18,
					yMax: 0.495f, OyMin: -20, OyMax: -20, align: TextAnchor.UpperCenter);
				cui.CreateProtectedButton(container, panel, "#7d8f32", "1 1 1 1", "Continue ▶", 9,
					xMin: 0.5f, xMax: 0.5f, yMin: 0.25f, yMax: 0.25f, OxMin: -30, OxMax: 30, OyMin: -12.5f, OyMax: 12.5f, command: "greet.continue");
			};
			return tab;
		}
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("greet.continue")]
	private void ChangePage(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var tab = GetTab(ap.Player);

		Analytics.admin_module_greet_continue();

		ap.SetStorage(tab, "page", 0);
		Singleton.DataInstance.GreetDisplayed = true;
		Singleton.GenerateTabs();
		Community.Runtime.Core.NextTick(() =>
		{
			Save();
			Singleton.SetTab(ap.Player, "carbon");
			Draw(ap.Player);
		});
	}
}

#endif
