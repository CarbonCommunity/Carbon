/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using Oxide.Game.Rust.Cui;
using static Carbon.Components.CUI;
using static Carbon.Modules.AdminModule;
using static ConsoleSystem;

namespace Carbon.Modules;

public class ColorPickerModule : CarbonModule<EmptyModuleConfig, EmptyModuleData>
{
	public AdminModule Admin { get; internal set; }
	public readonly Handler Handler = new();

	public override string Name => "ColorPicker";
	public override VersionNumber Version => new(1, 0, 0);
	public override Type Type => typeof(ColorPickerModule);
	public override bool EnabledByDefault => true;
	public override bool ForceEnabled => true;

	public const string Brightness = "colorpicker_brightness";
	public const string BrightnessIndicator = "colorpicker_brightnessindicator";
	public const string FirstOpen = "colorpicker_firstopen";
	public const string Alpha = "colorpicker_alpha";
	public const string OnColorPicked = "colorpicker_oncolorpicked";

	public const string PanelId = "carbonuicolorpicker";
	public const string PanelCursorLockId = "carbonuicolorpickercurlock";

	internal static float AnimationLength = 0.005f;
	internal static float CurrentAnimation = 0f;

	public override bool InitEnd()
	{
		Admin = GetModule<AdminModule>();
		return base.InitEnd();
	}

	public void Open(BasePlayer player, Action<string, string, float> onColorPicked)
	{
		var ap = Admin.GetPlayerSession(player);
		ap.SetStorage(ap.SelectedTab, Alpha, 1f);

		if (!ModuleConfiguration.Enabled)
		{
			var empty = string.Empty;
			onColorPicked?.Invoke(empty, empty, 1f);
			return;
		}

		DrawCursorLocker(player);
		Draw(player, onColorPicked);
		ap.SetStorage(ap.SelectedTab, FirstOpen, true);
	}
	public void Close(BasePlayer player)
	{
		var ap = Admin.GetPlayerSession(player);

		Handler.Destroy(PanelId, player);
		Handler.Destroy(PanelCursorLockId, player);

		ap.SetStorage(ap.SelectedTab, FirstOpen, false);
	}

	internal void Draw(BasePlayer player, Action<string, string, float> onColorPicked)
	{
		if (player == null) return;

		var ap = Admin.GetPlayerSession(player);

		ap.SetStorage(ap.SelectedTab, OnColorPicked, onColorPicked);
		var alphaValue = ap.GetStorage(ap.SelectedTab, Alpha, 1f);

		var brightness = ap.GetStorage(ap.SelectedTab, Brightness, 1f);
		var firstOpen = ap.GetStorage(ap.SelectedTab, FirstOpen, false);

		using var cui = new CUI(Handler);

		var container = cui.CreateContainer(PanelId,
			color: "0 0 0 0.75",
			xMin: 0, xMax: 1, yMin: 0, yMax: 1,
			needsCursor: true, destroyUi: PanelId);

		var color = cui.CreatePanel(container, parent: PanelId, id: PanelId + ".color",
			color: "0 0 0 0.6",
			xMin: 0.3f, xMax: 0.7f, yMin: 0.275f, yMax: 0.825f);
		var main = cui.CreatePanel(container, parent: PanelId + ".color", id: PanelId + ".main",
			color: "0 0 0 0.5",
			blur: true);

		cui.CreateText(container, parent: main, id: null,
			color: "1 1 1 0.8",
			text: "<b>Color Picker</b>", 18,
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

		cui.CreateText(container, parent: main, id: null,
			color: "1 1 1 0.3",
			text: "------------------------------------------------------------------------------------------------------------------------------------- BRIGHTNESS", 8,
			xMin: 0f, xMax: 0.775f, yMin: 0.01f, yMax: 0.98f,
			align: TextAnchor.LowerRight,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateText(container, parent: main, id: null,
			color: "1 1 1 0.3",
			text: "SHADES ---------------", 8,
			xMin: 0.805f, xMax: 1, yMin: 0.085f, yMax: 1f,
			align: TextAnchor.LowerLeft,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateText(container, parent: main, id: null,
			color: "1 1 1 0.3",
			text: "------------------- ALPHA", 8,
			xMin: 0, xMax: 0.14f, yMin: 0.085f, yMax: 1f,
			align: TextAnchor.LowerRight,
			font: Handler.FontTypes.RobotoCondensedRegular);

		//
		// Hex input field
		//
		var input = cui.CreatePanel(container, parent: main, id: null, "0.1 0.1 0.1 0.5",
			xMin: 0.805f, xMax: 0.94f, yMin: 0.085f, yMax: 0.15f, OyMin: -30, OyMax: -30);
		cui.CreateProtectedInputField(container, input, null, "1 1 1 1", "#", 10, 0, false,
			xMin: 0.075f, command: PanelId + ".pickhexcolor ", align: TextAnchor.MiddleLeft, needsKeyboard: true);

		//
		// Alpha input field
		//
		var alpha = cui.CreatePanel(container, parent: main, id: null, "0.1 0.1 0.1 0.5",
			xMin: 0.015f, xMax: 0.14f, yMin: 0.085f, yMax: 0.15f, OyMin: -30, OyMax: -30);
		cui.CreateProtectedInputField(container, alpha, null, "1 1 1 1", $"{alphaValue}", 10, 0, false,
			xMin: 0.075f, command: PanelId + ".pickalpha", align: TextAnchor.MiddleRight, needsKeyboard: true);

		var picker = cui.CreatePanel(container, parent: main, id: PanelId + ".picker",
			color: "0 0 0 0",
			xMin: 0.175f, xMax: 0.8f, yMin: 0.1f, yMax: 0.9f);

		for (var y = 0f; y < scale; y += 1f)
		{
			var heightColor = Color.Lerp(topRightColor, bottomRightColor, y.Scale(0f, scale, 0f, 1f));

			for (float x = 0; x < scale; x += 1f)
			{
				var widthColor = Color.Lerp(topLeftColor, bottomLeftColor, (x + y).Scale(0f, total, 0f, 1f));
				var _color = Color.Lerp(widthColor, heightColor, x.Scale(0f, scale, 0f, 1f)) * brightness;
				DrawColor(cui, container, ap, scale, _color, picker, offset * x, -(offset * y), fade: !firstOpen ? CurrentAnimation : 0);

				CurrentAnimation += AnimationLength;
			}
		}

		//
		// Brightness
		//
		var counter = 0;
		for (var y = 0f; y < scale; y += 1f)
		{
			var _color = Color.Lerp(Color.black, Color.white, y.Scale(0f, scale, 0f, 1f));
			DrawColor(cui, container, ap, scale, _color, picker, offset * y, -(offset * (scale + 1f)), "brightness", fade: !firstOpen ? CurrentAnimation : 0, index: counter);

			CurrentAnimation += AnimationLength;
			counter++;
		}

		//
		// Saturation
		//
		for (var y = 0f; y < scale; y += 1f)
		{
			var _color = Color.Lerp(Color.white, Color.black, y.Scale(0f, scale, 0f, 1f));
			DrawColor(cui, container, ap, scale, _color, picker, offset * (scale + 1f), -(offset * y), fade: !firstOpen ? CurrentAnimation : 0);

			CurrentAnimation += AnimationLength;
		}

		#endregion

		cui.CreateProtectedButton(container, parent: main, id: null,
			color: "0.6 0.2 0.2 0.9",
			textColor: "1 0.5 0.5 1",
			text: "X", 8,
			xMin: 0.96f, xMax: 0.99f, yMin: 0.95f, yMax: 0.99f,
			command: PanelId + ".close",
			font: Handler.FontTypes.DroidSansMono);

		cui.Send(container, player);

		CurrentAnimation = 0;
	}
	internal static void DrawColor(CUI cui, CuiElementContainer container, PlayerSession ap, float scale, Color color, string parent, float xOffset, float yOffset, string mode = "color", float fade = 0f, int index = -1)
	{
		var size = Extensions.MathEx.Scale(1f, 0, scale, 0f, 1f);

		var id = cui.CreateProtectedButton(container, parent, null,
			color: $"{color.r} {color.g} {color.b} 1",
			textColor: "0 0 0 0",
			text: string.Empty, 0,
			xMin: 0, yMin: size * (scale - 1f), xMax: 1f - (size * (scale - 1f)), yMax: 1f,
			OxMin: xOffset, OyMin: yOffset, OxMax: xOffset, OyMax: yOffset,
			fadeIn: fade,
			command: PanelId + $".pickcolor {mode} {ColorUtility.ToHtmlStringRGBA(color)} {color.r} {color.g} {color.b}");

		if (mode == "brightness" && index == ap.GetStorage(ap.SelectedTab, BrightnessIndicator, 8))
		{
			cui.CreatePanel(container, id, null, "0.75 0.75 0.2 0.8", yMin: 0.85f, yMax: 1, OyMin: 4, OyMax: 8.5f);
		}
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
	private void CloseColorPickerUI(Arg args)
	{
		Close(args.Player());
	}

	[ProtectedCommand(PanelId + ".pickcolor")]
	private void PickColorPickerUI(Arg args)
	{
		var player = args.Player();
		var ap = Admin.GetPlayerSession(player);
		var mode = args.Args[0];
		var hex = args.Args[1];
		var alpha = ap.GetStorage(ap.SelectedTab, Alpha, 1f);
		var rawColor = args.Args.Skip(2).ToArray().ToString(" ", " ");
		ColorUtility.TryParseHtmlString($"#{hex}", out var color);

		var brightness = ap.GetStorage(ap.SelectedTab, Brightness, 1f);
		var brightnessIndicator = ap.GetStorage(ap.SelectedTab, BrightnessIndicator, 8);
		var onColorPicked = ap.GetStorage<Action<string, string, float>>(ap.SelectedTab, OnColorPicked );

		switch (mode)
		{
			case "brightness":
				ap.SetStorage(ap.SelectedTab, Brightness, color.r.Scale(0f, 1f, 0f, 2.5f));
				ap.SetStorage(ap.SelectedTab, BrightnessIndicator, (int)color.r.Scale(0f, 1f, 0f, 20.5f));
				ap.SetStorage(ap.SelectedTab, FirstOpen, true);
				Draw (player, onColorPicked);
				return;
		}

		onColorPicked?.Invoke(hex, rawColor, alpha);
		Close (args.Player());
	}

	[ProtectedCommand(PanelId + ".pickhexcolor")]
	private void PickHexColorPickerUI(Arg args)
	{
		var player = args.Player();
		var ap = Admin.GetPlayerSession (player);
		var hex = args.Args[0];
		var alpha = ap.GetStorage(ap.SelectedTab, Alpha, 1f);

		if (args.Args.Length == 0 || string.IsNullOrEmpty(hex) || hex == "#")
		{
			return;
		}

		var onColorPicked = ap.GetStorage<Action<string, string, float>>(ap.SelectedTab, OnColorPicked );

		if (!hex.StartsWith("#")) hex = $"#{hex}";
		var rawColor = HexToRustColor(hex, includeAlpha: false);
		onColorPicked?.Invoke(hex, rawColor, alpha);
		Close (args.Player());
	}

	[ProtectedCommand(PanelId + ".pickalpha")]
	private void PickAlphaColorPickerUI(Arg args)
	{
		var player = args.Player();
		var ap = Admin.GetPlayerSession (player);
		var alpha = args.Args[0].ToFloat().Clamp(0f, 1f);
		ap.SetStorage(ap.SelectedTab, Alpha, alpha);
	}

	#endregion
}
