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
		SetTab(player, availableTabs.FirstOrDefault(x => x.Id.Equals(value)));
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand(PanelId + ".callaction")]
	private void CallAction(ConsoleSystem.Arg args)
	{
		var player = args.Player();

		var array = Array.Empty<object>();
		if (args.Args.Length - 2 > 0)
		{
			array = HookCaller.Caller.AllocateBuffer(args.Args.Length - 2);
			for (int i = 2; i < args.Args.Length; i++)
			{
				array[i - 2] = args.Args[i];
			}
		}

		if (CallColumnRow(player, args.GetInt(0), args.GetInt(1), array))
			Draw(player);

		if (array.Length > 0)
		{
			HookCaller.Caller.ReturnBuffer(array);
		}
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

		if (GetTab(player) is { Id: "configuration" })
		{
			SetTab(player, "carbon");
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
			SetTab(player, "carbon");
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
