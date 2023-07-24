/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using API.Abstracts;
using ConVar;
using static Carbon.Components.CUI;

namespace Carbon.Modules;

public class DatePickerModule : CarbonModule<EmptyModuleConfig, EmptyModuleData>
{
	public AdminModule Admin { get; internal set; }
	public readonly Handler Handler = new();

	public override string Name => "DatePicker";
	public override VersionNumber Version => new(1, 0, 0);
	public override Type Type => typeof(DatePickerModule);
	public override bool EnabledByDefault => true;
	public override bool ForceEnabled => true;

	public const string OnDatePicked = "datepicker_ondatepicked";
	public const string PanelId = "carbonuidatepicker";
	public const string PanelCursorLockId = "carbonuidatepickercurlock";

	internal static int Year = DateTime.UtcNow.Year;
	internal static int Month = DateTime.UtcNow.Month;
	internal static int Day = DateTime.UtcNow.Day;
	internal static float AnimationLength = 0.005f;
	internal static float CurrentAnimation = 0f;

	internal static readonly string[] Months = new string[]
	{
			"January", "February", "March", "April", "May", "June",
			"July", "August", "September", "October", "November", "December"
	};

	public override bool InitEnd()
	{
		Admin = GetModule<AdminModule>();
		return base.InitEnd();
	}

	public void Open(BasePlayer player, Action<DateTime> onDatePicked)
	{
		var ap = Admin.GetPlayerSession(player);

		if (!ModuleConfiguration.Enabled)
		{
			var empty = string.Empty;
			onDatePicked?.Invoke(default);
			return;
		}

		DrawCursorLocker(player);
		Draw(player, onDatePicked);
	}
	public void Close(BasePlayer player)
	{
		Handler.Destroy(PanelId, player);
		Handler.Destroy(PanelCursorLockId, player);
	}

	internal void Draw(BasePlayer player, Action<DateTime> onDatePicked)
	{
		if (player == null) return;

		var ap = Admin.GetPlayerSession(player);

		ap.SetStorage(ap.SelectedTab, OnDatePicked, onDatePicked);

		using var cui = new CUI(Handler);
	
		var container = cui.CreateContainer(PanelId,
			color: "0 0 0 0.75",
			xMin: 0, xMax: 1, yMin: 0, yMax: 1,
			needsCursor: true, destroyUi: PanelId);

		var color = cui.CreatePanel(container, parent: PanelId, null,
			color: "0 0 0 0.6",
			xMin: 0.3f, xMax: 0.7f, yMin: 0.275f, yMax: 0.825f);
		var main = cui.CreatePanel(container, parent: color, null,
			color: "0 0 0 0.5",
			blur: true);

		cui.CreateText(container, parent: main, id: null,
			color: "1 1 1 0.8",
			text: "<b>Date Picker</b>", 18,
			xMin: 0f, yMin: 0.8f, xMax: 1f, yMax: 0.98f,
			align: TextAnchor.UpperCenter,
			font: Handler.FontTypes.RobotoCondensedBold);

		#region Main

		var scale = 20f;
		var offset = scale * 0.770f;
		var total = (scale * 2) - 8f;

		var topRightColor = Color.blue;
		var bottomRightColor = Color.green;
		var topLeftColor = Color.red;
		var bottomLeftColor = Color.yellow;

		var year = cui.CreatePanel(container, main, null,
			color: "1 1 1 0.1",
			xMin: 0.15f, xMax: 0.85f, yMin: 0.86f, yMax: 0.92f);
		cui.CreateProtectedInputField(container, year, null, "1 1 1 0.7", $"{Year}", 10, 4, false, command: PanelId + $".action yearchange ");
		cui.CreateProtectedButton(container, year, null, "1 1 1 0.2", "1 1 1 0.7", "<", 8,
			xMin: 0f, xMax: 0.1f, command: PanelId + $".action yearappend -1");
		cui.CreateProtectedButton(container, year, null, "1 1 1 0.2", "1 1 1 0.7", ">", 8,
			xMin: 0.9f, xMax: 1f, command: PanelId + $".action yearappend 1");

		var months = cui.CreatePanel(container, parent: main, null,
			color: "0 0 0 0",
			xMin: 0.15f, xMax: 0.85f, yMin: 0.71f, yMax: 0.85f);
		var monthHeight = 1f;
		var monthOffset = 0f;
		var monthWidth = 1f / (Months.Length / 2);
		for (float i = 0; i < Months.Length; i++)
		{
			cui.CreateProtectedButton(container, months, null, $"1 1 1 {(Month == i + 1 ? 0.2 : 0.1)}", "1 1 1 0.7", Months[(int)i], 8,
				monthOffset, monthOffset + monthWidth, monthHeight - 0.5f, monthHeight, command: PanelId + $".action monthchange {i + 1}");
			monthOffset += monthWidth;

			if (i == (Months.Length - 1) / 2)
			{
				monthOffset = 0f;
				monthHeight = 0.5f;
			}
		}

		var days = cui.CreatePanel(container, parent: main, null,
			color: "0 0 0 0",
			xMin: 0.15f, xMax: 0.85f, yMin: 0.15f, yMax: 0.7f);
		var totalDaysInMonth = DateTime.DaysInMonth(Year, Month);
		var daysOffset = 0f;
		var daysHeight = 1f / (31 / 6);
		var dayWidth = 1f / 7f;
		var daysHeightOffset = 1f;
		for (float i = 0; i < totalDaysInMonth; i++)
		{
			cui.CreateProtectedButton(container, days, null, $"1 1 1 {(Day == i + 1 ? 0.2 : 0.05)}", "1 1 1 0.7", $"{i + 1}", 8, daysOffset, daysOffset + dayWidth, daysHeightOffset - daysHeight, daysHeightOffset, command: PanelId + $".action daychange {i + 1}");
			daysOffset += dayWidth;

			if ((i + 1) % 7 == 0)
			{
				daysOffset = 0f;
				daysHeightOffset -= daysHeight;
			}
		}

		#endregion

		cui.CreateProtectedButton(container, parent: main, id: null,
			color: "0.2 0.6 0.2 0.5",
			textColor: "0.5 1 0.5 1",
			text: "NOW", 9,
			xMin: 0.9f, xMax: 0.95f, yMin: 0.95f, yMax: 0.99f,
			command: PanelId + ".action reset");

		cui.CreateProtectedButton(container, parent: main, id: null,
			color: "0.6 0.2 0.2 0.9",
			textColor: "1 0.5 0.5 1",
			text: "X", 8,
			xMin: 0.96f, xMax: 0.99f, yMin: 0.95f, yMax: 0.99f,
			command: PanelId + ".close",
			font: Handler.FontTypes.DroidSansMono);

		cui.CreateProtectedButton(container, main, null, "0.3 1 0.3 0.2", "0.8 1 0.8 1", "CONFIRM".SpacedString(1), 8,
			xMin: 0.4f, xMax: 0.6f, yMin: 0.05f, yMax: 0.12f,
			command: PanelId + $".action confirm");

		cui.Send(container, player);

		CurrentAnimation = 0;
	}
	internal void DrawCursorLocker(BasePlayer player)
	{
		using var cui = new CUI(Handler);

		var container = cui.CreateContainer(PanelCursorLockId,
			color: "0 0 0 0",
			xMin: 0, xMax: 0, yMin: 0, yMax: 0,
			fadeIn: 0.005f,
			needsCursor: true);

		cui.Send(container, player);
	}

	#region Commands

	[ProtectedCommand(PanelId + ".close")]
	private void CloseDatePickerUI(ConsoleSystem.Arg args)
	{
		Close(args.Player());
	}
	[ProtectedCommand(PanelId + ".action")]
	private void ActionDatePickerUI(ConsoleSystem.Arg args)
	{
		var player = args.Player();
		var ap = Admin.GetPlayerSession(player);
		var onDatePicked = ap.GetStorage<Action<DateTime>>(ap.SelectedTab, OnDatePicked);

		switch (args.Args[0])
		{
			case "yearchange":
				Year = args.GetInt(1, DateTime.UtcNow.Year).Clamp(1977, 2100);
				break;

			case "monthchange":
				Month = args.GetInt(1, 1);
				break;

			case "daychange":
				Day = args.GetInt(1, 1);
				break;

			case "yearappend":
				Year += args.GetInt(1, 0);
				break;

			case "reset":
				Year = DateTime.UtcNow.Year;
				Month = DateTime.UtcNow.Month;
				Day = DateTime.UtcNow.Day;
				break;

			case "confirm":
				onDatePicked?.Invoke(new DateTime(Year, Month, Day));
				Close(player);
				return;
		}

		Draw(player, onDatePicked);
	}

	#endregion
}
