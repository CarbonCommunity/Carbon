namespace Carbon.Modules;

public partial class AdminModule
{
#if !MINIMAL
	[Conditional("!MINIMAL")]
	[ProtectedCommand(PanelId + ".changetab")]
	private void ChangeTab(ConsoleSystem.Arg args)
	{
		var player = args.Player();
		var ap = GetPlayerSession(player);
		var previous = ap.SelectedTab;
		var value = args.GetString(0);

		ap.Clear();

		var availableTabs = Tabs.Where(x => !DataInstance.IsTabHidden(x.Id));
		switch (value)
		{
			case "next":
			case "prev":
				var count = availableTabs.Count();
				var indexOf = availableTabs.IndexOf(previous);
				indexOf = value == "next" ? indexOf + 1 : indexOf - 1;

				if (indexOf > count - 1) indexOf = 0;
				else if (indexOf < 0) indexOf = count - 1;
				SetTab(player, indexOf);
				break;

			default:
				SetTab(player, availableTabs.FirstOrDefault(x => x.Id.Equals(value)));
				break;
		}
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand(PanelId + ".callaction")]
	private void CallAction(ConsoleSystem.Arg args)
	{
		var player = args.Player();

		if (CallColumnRow(player, args.GetInt(0), args.GetInt(1), args.Args.Skip(2).Any() ? args.Args.Skip(2) : Array.Empty<string>()))
			Draw(player);
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand(PanelId + ".changecolumnpage")]
	private void ChangeColumnPage(ConsoleSystem.Arg args)
	{
		var player = args.Player();
		var instance = GetPlayerSession(player);
		var page = instance.GetOrCreatePage(args.Args[0].ToInt());
		var type = args.Args[1].ToInt();

		switch (type)
		{
			case 0:
				page.CurrentPage--;
				if (page.CurrentPage < 0) page.CurrentPage = page.TotalPages;
				break;

			case 1:
				page.CurrentPage++;
				if (page.CurrentPage > page.TotalPages) page.CurrentPage = 0;
				break;

			case 2:
				page.CurrentPage = 0;
				break;

			case 3:
				page.CurrentPage = page.TotalPages;
				break;

			case 4:
				page.CurrentPage = (args.Args[2].ToInt() - 1).Clamp(0, page.TotalPages);
				break;
		}

		Draw(player);
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand(PanelId + ".config")]
	private void ShowConfig(ConsoleSystem.Arg args)
	{
		var player = args.Player();

		if (GetTab(player).Id == "configuration")
		{
			SetTab(player, 0);
		}
		else
		{
			SetTab(player, ConfigurationTab.GetOrCache());
		}
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand(PanelId + ".profiler")]
	private void ShowProfiler(ConsoleSystem.Arg args)
	{
		var player = args.Player();

		if (GetTab(player).Id == "profiler")
		{
			SetTab(player, 0);
		}
		else
		{
			SetTab(player, ProfilerTab.GetOrCache(GetPlayerSession(player)));
		}
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand(PanelId + ".maximize")]
	private void Maximize(ConsoleSystem.Arg args)
	{
		DataInstance.Maximize = !DataInstance.Maximize;
		Draw(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand(PanelId + ".close")]
	private void CloseUI(ConsoleSystem.Arg args)
	{
		Close(args.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand(PanelId + ".dialogaction")]
	private void Dialog_Action(ConsoleSystem.Arg args)
	{
		var player = args.Player();
		var admin = GetPlayerSession(player);
		var tab = GetTab(player);
		var dialog = tab?.Dialog;
		if (tab != null) tab.Dialog = null;

		switch (args.Args[0])
		{
			case "confirm":
				try { dialog?.OnConfirm(admin); } catch { }
				break;

			case "decline":
				try { dialog?.OnDecline(admin); } catch { }
				break;
		}

		Draw(player);
	}
#endif
}
