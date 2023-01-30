using System;
using System.Collections.Generic;
using System.Linq;
using Carbon.Base;
using Carbon.Extensions;
using Facepunch;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Game.Rust.Cui;
using Oxide.Plugins;
using UnityEngine;
using static ConsoleSystem;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public class AdminModule : CarbonModule<AdminConfig, AdminData>
{
	public override string Name => "Admin";
	public override Type Type => typeof(AdminModule);
	public override bool EnabledByDefault => true;

	internal List<Tab> Tabs = new();
	internal Dictionary<BasePlayer, AdminPlayer> Instances = new();
	internal CUI.Handler Handler { get; } = new();

	const string PanelId = "carbonmodularui";

	private void OnServerInitialized()
	{
		Community.Runtime.CorePlugin.cmd.AddChatCommand(Config.OpenCommand, this, (player, cmd, args) => { Draw(player); });

		RegisterTab(PermissionsTab.Get(Community.Runtime.CorePlugin.permission));
	}

	#region Option Elements

	internal void TabButton(CUI cui, CuiElementContainer container, string parent, string text, string command, float width, float offset, bool highlight = false)
	{
		cui.CreateProtectedButton(container, parent: parent, id: $"{parent}btn",
			color: highlight ? "0.4 0.7 0.2 0.7" : "0.3 0.3 0.3 0.1",
			textColor: "1 1 1 0.5",
			text: text, 11,
			xMin: offset, xMax: offset + width, yMin: 0, yMax: 1,
			command: command,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		if (highlight)
		{
			cui.CreatePanel(container, $"{parent}btn", null,
				color: "1 1 1 0.4",
				xMin: 0, xMax: 1f, yMin: 0f, yMax: 0.03f);
		}
	}

	public void TabColumnPagination(CUI cui, CuiElementContainer container, string parent, int column, AdminPlayer.Page page, float height, float offset)
	{
		var id = $"{parent}{column}";

		cui.CreatePanel(container, parent, id,
			color: "0.3 0.3 0.3 0.3",
			xMin: 0.025f, xMax: 0.98f, yMin: offset, yMax: offset + height);

		cui.CreateText(container, parent: id, id: null,
			color: "1 1 1 0.5",
			text: $"{page.CurrentPage + 1:n0} / {page.TotalPages + 1:n0}", 9,
			xMin: 0, xMax: 1f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleCenter,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: id, id: null,
			color: page.CurrentPage > 0 ? "0.4 0.7 0.2 0.7" : "0.3 0.3 0.3 0.1",
			textColor: "1 1 1 0.5",
			text: "<", 8,
			xMin: 0, xMax: 0.1f, yMin: 0f, yMax: 1f,
			command: page.CurrentPage > 0 ? PanelId + $".changecolumnpage {column} true" : "",
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: id, id: null,
			color: page.CurrentPage < page.TotalPages ? "0.4 0.7 0.2 0.7" : "0.3 0.3 0.3 0.1",
			textColor: "1 1 1 0.5",
			text: ">", 8,
			xMin: 0.9f, xMax: 1f, yMin: 0f, yMax: 1f,
			command: page.CurrentPage < page.TotalPages ? PanelId + $".changecolumnpage {column} false" : "",
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);
	}
	public void TabPanelName(CUI cui, CuiElementContainer container, string parent, string text, float height, float offset, TextAnchor align)
	{
		cui.CreateText(container, parent: parent, id: $"{parent}text",
			color: "1 1 1 0.7",
			text: text.ToUpper(), 12,
			xMin: 0.025f, xMax: 0.98f, yMin: offset, yMax: offset + height,
			align: align,
			font: CUI.Handler.FontTypes.RobotoCondensedBold);

		cui.CreatePanel(container, $"{parent}text", null,
			color: "1 1 1 0.7",
			xMin: 0, xMax: 1, yMin: 0f, yMax: 0.025f);
	}
	public void TabPanelText(CUI cui, CuiElementContainer container, string parent, string text, int size, string color, float height, float offset, TextAnchor align, CUI.Handler.FontTypes font)
	{
		cui.CreateText(container, parent: parent, id: $"{parent}text",
			color: color,
			text: text, size,
			xMin: 0.025f, xMax: 0.98f, yMin: offset, yMax: offset + height,
			align: align,
			font: font);
	}
	public void TabPanelButton(CUI cui, CuiElementContainer container, string parent, string text, string command, float height, float offset, Tab.OptionButton.Types type = Tab.OptionButton.Types.None)
	{
		var color = "0 0 0 0";

		switch (type)
		{
			case Tab.OptionButton.Types.Selected:
				color = "0.4 0.7 0.2 0.7";
				break;

			case Tab.OptionButton.Types.Warned:
				color = "0.8 0.7 0.2 0.7";
				break;

			case Tab.OptionButton.Types.Important:
				color = "0.97 0.2 0.1 0.7";
				break;

			default:
				color = "0.2 0.2 0.2 0.5";
				break;
		}

		cui.CreateProtectedButton(container, parent: parent, id: $"{parent}btn",
			color: color,
			textColor: "1 1 1 0.5",
			text: text, 11,
			xMin: 0.015f, xMax: 0.985f, yMin: offset, yMax: offset + height,
			command: command,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		// Death hates this
		// if (type != Tab.RowButton.Types.None)
		// {
		// 	cui.CreatePanel(container, $"{parent}btn", null,
		// 		color: "1 1 1 0.4",
		// 		xMin: 0, xMax: 1f, yMin: 0f, yMax: 0.03f);
		// }
	}
	public void TabPanelInput(CUI cui, CuiElementContainer container, string parent, string text, string placeholder, string command, float height, float offset, Tab.OptionButton.Types type = Tab.OptionButton.Types.None)
	{
		var color = "0 0 0 0";

		switch (type)
		{
			case Tab.OptionButton.Types.Selected:
				color = "0.4 0.7 0.2 0.7";
				break;

			case Tab.OptionButton.Types.Warned:
				color = "0.8 0.7 0.2 0.7";
				break;

			case Tab.OptionButton.Types.Important:
				color = "0.97 0.2 0.1 0.7";
				break;

			default:
				color = "0.2 0.2 0.2 0.5";
				break;
		}

		cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
			color: "1 1 1 0.7",
			text: text, 12,
			xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleLeft,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreatePanel(container, $"{parent}panel", $"{parent}inppanel",
			color: color,
			xMin: Config.OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		cui.CreateProtectedInputField(container, parent: $"{parent}inppanel", id: null,
			color: "1 1 1 1",
			text: placeholder, 11,
			xMin: 0.03f, xMax: 1, yMin: 0, yMax: 1,
			command: command,
			align: TextAnchor.MiddleLeft,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);
	}
	public void TabPanelEnum(CUI cui, CuiElementContainer container, string parent, string text, string value, string command, float height, float offset, Tab.OptionButton.Types type = Tab.OptionButton.Types.Selected)
	{
		var color = "0 0 0 0";

		switch (type)
		{
			case Tab.OptionButton.Types.Selected:
				color = "0.4 0.7 0.2 0.7";
				break;

			case Tab.OptionButton.Types.Warned:
				color = "0.8 0.7 0.2 0.7";
				break;

			case Tab.OptionButton.Types.Important:
				color = "0.97 0.2 0.1 0.7";
				break;

			default:
				color = "0.2 0.2 0.2 0.5";
				break;
		}

		cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
			color: "1 1 1 0.7",
			text: text, 12,
			xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleLeft,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreatePanel(container, $"{parent}panel", $"{parent}inppanel",
			color: "0.2 0.2 0.2 0.5",
			xMin: Config.OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		cui.CreateText(container, parent: $"{parent}inppanel", id: null,
			color: "1 1 1 0.7",
			text: value, 11,
			xMin: 0, xMax: 1, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleCenter,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: $"{parent}inppanel", id: null,
			color: color,
			textColor: "1 1 1 0.7",
			text: "<", 10,
			xMin: 0f, xMax: 0.15f, yMin: 0, yMax: 1,
			command: $"{command} true",
			align: TextAnchor.MiddleCenter,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: $"{parent}inppanel", id: null,
			color: color,
			textColor: "1 1 1 0.7",
			text: ">", 10,
			xMin: 0.85f, xMax: 1f, yMin: 0, yMax: 1,
			command: $"{command} false",
			align: TextAnchor.MiddleCenter,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);
	}

	#endregion

	#region UI Commands

	[UiCommand(PanelId + ".changetab")]
	private void ChangeTab(Arg args)
	{
		var player = args.Player();
		var instance = GetOrCreateInstance(player);
		var previous = instance.TabIndex;

		var tab = GetTab(player);
		tab?.OnChange?.Invoke(instance, tab);

		instance.Clear();

		if (int.TryParse(args.Args[0], out int index))
		{
			instance.TabIndex = index;
		}
		else
		{
			instance.TabIndex += args.Args[0] == "up" ? 1 : -1;

			if (instance.TabIndex > Tabs.Count - 1) instance.TabIndex = 0;
			else if (instance.TabIndex < 0) instance.TabIndex = Tabs.Count - 1;
		}

		if (instance.TabIndex != previous) Draw(player);
	}

	[UiCommand(PanelId + ".callaction")]
	private void CallAction(Arg args)
	{
		var player = args.Player();

		CallColumnRow(player, args.Args[0].ToInt(), args.Args[1].ToInt(), args.Args.Skip(2).ToArray());
		Draw(player);
	}

	[UiCommand(PanelId + ".changecolumnpage")]
	private void ChangeColumnPage(Arg args)
	{
		var player = args.Player();
		var instance = GetOrCreateInstance(player);
		var page = instance.GetOrCreatePage(args.Args[0].ToInt());
		var back = args.Args[1].ToBool();

		if (back) page.CurrentPage--;
		else page.CurrentPage++;

		Draw(player);
	}

	[UiCommand(PanelId + ".closetest")]
	private void CloseUI(Arg args)
	{
		Handler.Destroy(PanelId, args.Player());
	}

	#endregion

	#region Methods

	public void Draw(BasePlayer player)
	{
		var instance = GetOrCreateInstance(player);
		var tab = GetTab(player);

		using (var cui = new CUI(Handler))
		{
			cui.Destroy(PanelId, player);

			var container = cui.CreateContainer(PanelId,
				color: "0 0 0 0.75",
				xMin: 0, xMax: 1, yMin: 0, yMax: 1,
				useCursor: true);

			cui.CreatePanel(container, parent: PanelId, id: "color",
				color: "0 0 0 0.6",
				xMin: 0.15f, xMax: 0.85f, yMin: 0.1f, yMax: 0.9f);
			cui.CreatePanel(container, "color", "main",
				color: "0 0 0 0.5",
				blur: true);

			#region Title

			cui.CreateText(container, parent: "main", id: null,
				color: "1 1 1 0.8",
				text: "<b>Admin Settings</b>", 18,
				xMin: 0.0175f, yMin: 0.8f, xMax: 1f, yMax: 0.98f,
				align: TextAnchor.UpperLeft,
				font: CUI.Handler.FontTypes.RobotoCondensedBold);
			cui.CreateText(container, parent: "main", id: null,
				color: "1 1 1 0.5",
				text: $"Carbon {Community.InformationalVersion}", 11,
				xMin: 0.0175f, yMin: 0.8f, xMax: 1f, yMax: 0.95f,
				align: TextAnchor.UpperLeft,
				font: CUI.Handler.FontTypes.RobotoCondensedRegular);

			#endregion

			#region Exit

			cui.CreateProtectedButton(container, parent: "main", id: null,
				color: "0.6 0.2 0.2 0.9",
				textColor: "1 0.5 0.5 1",
				text: "X", 13,
				xMin: 0.96f, xMax: 0.99f, yMin: 0.95f, yMax: 0.99f,
				command: PanelId + ".closetest",
				font: CUI.Handler.FontTypes.DroidSansMono);

			#endregion

			#region Tabs

			cui.CreatePanel(container, parent: "main", id: "tab_buttons",
				color: "0 0 0 0.6",
				xMin: 0.01f, xMax: 0.99f, yMin: 0.875f, yMax: 0.92f);

			TabButton(cui, container, "tab_buttons", "<", PanelId + ".changetab down", 0.03f, 0);
			TabButton(cui, container, "tab_buttons", ">", PanelId + ".changetab up", 0.03f, 0.97f);

			var tabIndex = 0.03f;
			var amount = Tabs.Count;
			var tabWidth = amount == 0 ? 0f : 0.94f / amount;

			for (int i = instance.TabSkip; i < amount; i++)
			{
				var _tab = Tabs[instance.TabSkip + i];
				TabButton(cui, container, "tab_buttons", $"{_tab.Name}", PanelId + $".changetab {i}", tabWidth, tabIndex, instance.TabIndex == i);
				tabIndex += tabWidth;
			}

			#endregion

			#region Panels

			cui.CreatePanel(container, "main", "panels",
				color: "0 0 0 0",
				xMin: 0.01f, xMax: 0.99f, yMin: 0.02f, yMax: 0.86f);

			if (tab != null)
			{
				#region Override

				tab.Override?.Invoke(tab, container, "panels");

				#endregion

				#region Columns

				var panelIndex = 0f;
				var spacing = 0.005f;
				var panelWidth = (tab.Columns.Count == 0 ? 0f : 1f / tab.Columns.Count) - spacing;

				for (int i = 0; i < tab.Columns.Count; i++)
				{
					var panel = $"sub{i}";
					var rows = tab.Columns[i];

					cui.CreatePanel(container, "panels", panel,
						color: "0 0 0 0.5",
						xMin: panelIndex, xMax: panelIndex + panelWidth - spacing, yMin: 0, yMax: 1);

					#region Rows

					var columnPage = instance.GetOrCreatePage(i);
					var contentsPerPage = 19;
					var rowSpacing = 0.01f;
					var rowHeight = 0.04f;
					var rowIndex = 1f - rowSpacing - rowHeight;
					var rowPage = rows.Skip(contentsPerPage * columnPage.CurrentPage).Take(contentsPerPage);
					columnPage.TotalPages = (int)Math.Ceiling((double)rows.Count / contentsPerPage - 1);
					columnPage.Check();

					for (int r = 0; r < rowPage.Count(); r++)
					{
						var actualI = r + (columnPage.CurrentPage * contentsPerPage);
						var row = rows.ElementAt(actualI);

						switch (row)
						{
							case Tab.OptionName name:
								TabPanelName(cui, container, panel, name.Name, rowHeight, rowIndex, name.Align);
								break;

							case Tab.OptionButton button:
								TabPanelButton(cui, container, panel, button.Name, PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex, button.Type == null ? Tab.OptionButton.Types.None : button.Type.Invoke(instance));
								break;

							case Tab.OptionText text:
								TabPanelText(cui, container, panel, text.Name, text.Size, text.Color, rowHeight, rowIndex, text.Align, text.Font);
								break;

							case Tab.OptionInput input:
								TabPanelInput(cui, container, panel, input.Name, input.Placeholder, PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex);
								break;

							case Tab.OptionEnum @enum:
								TabPanelEnum(cui, container, panel, @enum.Name, @enum.Text?.Invoke(), PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex);
								break;

							default:
								break;
						}

						rowIndex -= rowHeight + rowSpacing;
					}

					if (columnPage.TotalPages > 0)
					{
						TabColumnPagination(cui, container, panel, i, columnPage, rowHeight, rowIndex);
						TabColumnPagination(cui, container, panel, i, columnPage, rowHeight, rowIndex);
					}

					#endregion

					panelIndex += panelWidth + spacing;
				}

				#endregion
			}

			#endregion

			cui.Send(container, player);
		}
	}

	public void RegisterTab(Tab tab)
	{
		var existentTab = Tabs.FirstOrDefault(x => x.Name == tab.Name);
		if (existentTab != null)
		{
			var index = Tabs.IndexOf(existentTab);
			Tabs.RemoveAt(index);
			Pool.Free(ref existentTab);

			Tabs.Insert(index, tab);
		}
		else
		{
			Tabs.Add(tab);
		}
	}
	public AdminPlayer GetOrCreateInstance(BasePlayer player)
	{
		if (Instances.TryGetValue(player, out AdminPlayer instance)) return instance;

		instance = new AdminPlayer(player);
		Instances.Add(player, instance);
		return instance;
	}
	public Tab GetTab(BasePlayer player)
	{
		if (Tabs.Count == 0) return null;

		var instance = GetOrCreateInstance(player);
		if (instance.TabIndex > Tabs.Count - 1 || instance.TabIndex < 0) return null;

		return Tabs[instance.TabIndex];
	}
	public void CallColumnRow(BasePlayer player, int column, int row, string[] args)
	{
		var instance = GetOrCreateInstance(player);
		var tab = GetTab(player);

		instance.LastPressedColumn = column;
		instance.LastPressedRow = row;

		switch (tab.Columns[column][row])
		{
			case Tab.OptionButton button:
				button.Callback?.Invoke(instance);
				break;

			case Tab.OptionInput input:
				input.Callback?.Invoke(instance, args);
				break;

			case Tab.OptionEnum @enum:
				@enum.Callback?.Invoke(args[0].ToBool());
				break;
		}
	}

	#endregion

	public class AdminPlayer : IDisposable
	{
		public static AdminPlayer Blank { get; } = new AdminPlayer(null);

		public BasePlayer Player;
		public Dictionary<int, Page> ColumnPages = new Dictionary<int, Page>();
		public Dictionary<string, object> LocalStorage = new Dictionary<string, object>();

		public int TabIndex;
		public int TabSkip;
		public int LastPressedColumn;
		public int LastPressedRow;

		public AdminPlayer(BasePlayer player)
		{
			Player = player;
		}

		public T GetStorage<T>(string id)
		{
			if (LocalStorage.TryGetValue(id, out var storage)) return (T)storage;

			return default;
		}
		public object GetStorage(string id)
		{
			if (LocalStorage.TryGetValue(id, out var storage)) return storage;

			return default;
		}
		public void SetStorage<T>(string id, T value)
		{
			LocalStorage[id] = value;
		}
		public void ClearStorage(string id)
		{
			LocalStorage[id] = null;
		}
		public void Clear()
		{
			foreach (var page in ColumnPages)
			{
				var value = ColumnPages[page.Key];
				Pool.Free(ref value);
			}

			ColumnPages.Clear();
			LocalStorage.Clear();
		}
		public Page GetOrCreatePage(int column)
		{
			if (ColumnPages.TryGetValue(column, out var page)) return page;
			else
			{
				ColumnPages[column] = page = new Page();
				return page;
			}
		}

		public void Dispose()
		{
			Clear();
		}

		public class Page
		{
			public int CurrentPage { get; set; }
			public int TotalPages { get; set; }

			public void Check()
			{
				if (CurrentPage < 0) CurrentPage = TotalPages;
				else if (CurrentPage > TotalPages) CurrentPage = 0;
			}
		}
	}
	public class Tab : IDisposable
	{
		public string Name;
		public RustPlugin Plugin;
		public Action<Tab, CuiElementContainer, string> Override;
		public Dictionary<int, List<Option>> Columns = new Dictionary<int, List<Option>>();
		public Action<AdminPlayer, Tab> OnChange;

		public Tab(string name, Action<AdminPlayer, Tab> onChange = null)
		{
			Name = name;
			OnChange = onChange;
		}

		public void ClearColumn(int column, bool erase = false)
		{
			if (Columns.TryGetValue(column, out var rows))
			{
				rows.Clear();

				if (erase)
				{
					Columns[column] = null;
					Columns.Remove(column);
				}
			}
		}
		public void ClearAfter(int index)
		{
			for (int i = 0; i < Columns.Count; i++)
			{
				if (i >= index) ClearColumn(i);
			}
		}
		public Tab AddColumn(int column)
		{
			if (!Columns.TryGetValue(column, out var options))
			{
				Columns[column] = new List<Option>();
			}

			return this;

		}
		public Tab AddRow(int column, Option row)
		{
			if (Columns.TryGetValue(column, out var options))
			{
				options.Add(row);
			}
			else
			{

				Columns[column] = options = new List<Option>();
				options.Add(row);
			}

			return this;
		}
		public Tab AddName(int column, string name, TextAnchor align)
		{
			AddRow(column, new OptionName(name, align));
			return this;
		}
		public Tab AddText(int column, string name, int size, string color, TextAnchor align, CUI.Handler.FontTypes font)
		{
			AddRow(column, new OptionText(name, size, color, align, font));
			return this;
		}
		public Tab AddInput(int column, string name, string placeholder, Action<AdminPlayer, string[]> callback)
		{
			AddRow(column, new OptionInput(name, placeholder, callback));
			return this;
		}
		public Tab AddEnum(int column, string name, Action<bool> callback, Func<string> text)
		{
			AddRow(column, new OptionEnum(name, callback, text));
			return this;
		}

		public void Dispose()
		{
			foreach (var column in Columns)
			{
				column.Value.Clear();
				Columns[column.Key] = null;
			}

			Columns.Clear();
			Columns = null;
		}

		public class Option
		{
			public string Name;

			public Option(string name)
			{
				Name = name;
			}
		}
		public class OptionName : Option
		{
			public TextAnchor Align;

			public OptionName(string name, TextAnchor align) : base(name) { Align = align; }
		}
		public class OptionText : Option
		{
			public int Size;
			public string Color;
			public TextAnchor Align;
			public CUI.Handler.FontTypes Font;

			public OptionText(string name, int size, string color, TextAnchor align, CUI.Handler.FontTypes font) : base(name) { Align = align; Size = size; Color = color; Font = font; }
		}
		public class OptionInput : Option
		{
			public string Placeholder;
			public Action<AdminPlayer, string[]> Callback;

			public OptionInput(string name, string placeholder, Action<AdminPlayer, string[]> args) : base(name) { Placeholder = placeholder; Callback = args; }
		}
		public class OptionButton : Option
		{
			public Func<AdminPlayer, Types> Type;
			public Action<AdminPlayer> Callback;

			public enum Types
			{
				None,
				Selected,
				Warned,
				Important
			}

			public OptionButton(string name, Action<AdminPlayer> callback, Func<AdminPlayer, Types> type = null) : base(name) { Callback = callback; Type = type; }
		}
		public class OptionEnum : Option
		{
			public Func<string> Text;
			public Action<bool> Callback;

			public OptionEnum(string name, Action<bool> callback, Func<string> text) : base(name) { Callback = callback; Text = text; }
		}
		public class OptionRange : Option
		{
			public float Value;
			public Action<float> Callback;

			public OptionRange(string name, float value, Action<float> callback, Func<float> text) : base(name) { Callback = callback; Value = value; }
		}
	}

	#region Core Tabs

	public class PermissionsTab
	{
		public static Tab Get(Permission permission)
		{
			var perms = new Tab("Permissions", (instance, tab) =>
			{
				tab.ClearColumn(1);
				tab.ClearColumn(2);
				tab.ClearColumn(3);
				GeneratePlayers(tab, permission, instance);
			});
			{
				perms.AddName(0, "Options", TextAnchor.MiddleLeft);

				perms.AddRow(0, new Tab.OptionButton($"Players", instance =>
				{
					perms.ClearColumn(1);
					perms.ClearColumn(2);
					perms.ClearColumn(3);
					instance.Clear();

					instance.SetStorage("option", 0);

					GeneratePlayers(perms, permission, instance);
				}, type: (instance) => instance.GetStorage<int>("option") == 0 ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));

				GeneratePlayers(perms, permission, AdminPlayer.Blank);

				perms.AddRow(0, new Tab.OptionButton($"Groups", instance =>
				{
					perms.ClearColumn(1);
					perms.ClearColumn(2);
					perms.ClearColumn(3);
					instance.Clear();

					instance.ClearStorage("player");
					instance.ClearStorage("plugin");

					instance.SetStorage("option", 1);

					perms.AddName(1, "Groups", TextAnchor.MiddleLeft);
					{
						foreach (var group in permission.GetGroups())
						{
							perms.AddRow(1, new Tab.OptionButton($"{group}", instance2 =>
							{
								instance.SetStorage("group", group);
								instance.SetStorage("groupr", instance2.LastPressedRow);
								instance.SetStorage("groupc", instance2.LastPressedColumn);

								instance.ClearStorage("plugin");

								perms.ClearColumn(2);
								perms.ClearColumn(3);

								perms.AddName(2, "Plugins", TextAnchor.MiddleLeft);
								{
									foreach (var plugin in Community.Runtime.Plugins.Plugins.Where(x => x.permission.GetPermissions().Any(y => y.StartsWith(x.Name.ToLower()))))
									{
										perms.AddRow(2, new Tab.OptionButton($"{plugin.Name} ({plugin.Version})", instance3 =>
										{
											instance.SetStorage("plugin", plugin);
											instance.SetStorage("pluginr", instance3.LastPressedRow);
											instance.SetStorage("pluginc", instance3.LastPressedColumn);

											perms.ClearColumn(3);

											perms.AddName(3, "Permissions", TextAnchor.MiddleLeft);
											foreach (var perm in permission.GetPermissions().Where(x => x.StartsWith(plugin.Name.ToLower())))
											{
												perms.AddRow(3, new Tab.OptionButton($"{perm}", instance5 =>
												{
													if (permission.GroupHasPermission(group, perm))
														permission.RevokeGroupPermission(group, perm);
													else permission.GrantGroupPermission(group, perm, plugin);
												}, type: (_instance) => permission.GroupHasPermission(group, perm) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
											}
										}, type: (_instance) => instance.GetStorage("plugin") == plugin ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
									}
								}
							}, type: (_instance) => instance.GetStorage<string>("group") == group ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
						}
					}
					perms.AddRow(1, new Tab.OptionButton("Add Group", null, (_instance) => Tab.OptionButton.Types.Warned));
				}, type: (instance) => instance.GetStorage<int>("option") == 1 ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
				perms.AddColumn(1);
				perms.AddColumn(2);
				perms.AddColumn(3);
			}

			return perms;
		}

		public static void GeneratePlayers(Tab perms, Permission permission, AdminPlayer instance)
		{
			perms.AddName(1, "Players", TextAnchor.MiddleLeft);
			{
				foreach (var player in BasePlayer.allPlayerList)
				{
					perms.AddRow(1, new Tab.OptionButton($"{player.displayName} ({player.userID})", instance2 =>
					{
						instance.SetStorage("player", player);
						instance.SetStorage("playerr", instance2.LastPressedRow);
						instance.SetStorage("playerc", instance2.LastPressedColumn);

						instance.ClearStorage("plugin");

						perms.ClearColumn(2);
						perms.ClearColumn(3);

						perms.AddName(2, "Plugins", TextAnchor.MiddleLeft);
						{
							foreach (var plugin in Community.Runtime.Plugins.Plugins.Where(x => x.permission.GetPermissions().Any(y => y.StartsWith(x.Name.ToLower()))))
							{
								perms.AddRow(2, new Tab.OptionButton($"{plugin.Name} ({plugin.Version})", instance3 =>
								{
									instance.SetStorage("plugin", plugin);
									instance.SetStorage("pluginr", instance3.LastPressedRow);
									instance.SetStorage("pluginc", instance3.LastPressedColumn);

									perms.ClearColumn(3);

									perms.AddName(3, "Permissions", TextAnchor.MiddleLeft);
									foreach (var perm in permission.GetPermissions().Where(x => x.StartsWith(plugin.Name.ToLower())))
									{
										var isInherited = false;
										var list = "";

										foreach (var group in permission.GetUserGroups(player.UserIDString))
											if (permission.GroupHasPermission(group, perm))
											{
												isInherited = true;
												list += $"<b>{group}</b>, ";
											}

										perms.AddRow(3, new Tab.OptionButton($"{perm}", instance5 =>
										{
											if (permission.UserHasPermission(player.UserIDString, perm))
												permission.RevokeUserPermission(player.UserIDString, perm);
											else permission.GrantUserPermission(player.UserIDString, perm, plugin);
										}, type: (_instance) => isInherited ? Tab.OptionButton.Types.Important : permission.UserHasPermission(player.UserIDString, perm) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));

										if (isInherited)
										{
											perms.AddText(3, $"Inherited by the following groups: {list.TrimEnd(',', ' ')}", 8, "1 1 1 0.6", TextAnchor.UpperLeft, CUI.Handler.FontTypes.RobotoCondensedRegular);
										}
									}
								}, type: (_instance) => instance.GetStorage("plugin") == plugin ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
							}
						}
					}, type: (_instance) => instance.GetStorage<BasePlayer>("player") == player ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
				}
			}
		}
	}

	#endregion
}

public class AdminConfig
{
	public string OpenCommand = "cadmin";
	public float OptionWidth = 0.55f;
}
public class AdminData
{

}
