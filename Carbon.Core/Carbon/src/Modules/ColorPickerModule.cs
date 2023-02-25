using System;
using Carbon.Base;
using Carbon.Extensions;
using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;
using static ConsoleSystem;
using UnityEngine;
using System.Linq;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public class ColorPickerModule : CarbonModule<ColorPickerConfig, ColorPickerData>
{
	public override string Name => "Color Picker";
	public override Type Type => typeof(ColorPickerModule);
	public override bool EnabledByDefault => true;

	public static float Brightness = 1f;
	public static Action<string, string> OnColorPicked;

	public const string PanelId = "carbonuicolorpicker";
	public const string PanelCursorLockId = "carbonuicolorpickercurlock";

	internal CUI.Handler Handler = new();
	internal float AnimationLength = 0.005f;
	internal float CurrentAnimation = 0f;
	internal bool FirstOpen = false;
	internal int BrightnessIndicator = 8;

	public void Open(BasePlayer player, Action<string, string> onColorPicked)
	{
		DrawCursorLocker(player);
		Draw(player, onColorPicked);
		FirstOpen = true;
	}
	public void Close(BasePlayer player)
	{
		Handler.Destroy(PanelId, player);
		Handler.Destroy(PanelCursorLockId, player);

		FirstOpen = false;
		Brightness = 1f;
		BrightnessIndicator = 8;
	}

	internal void Draw(BasePlayer player, Action<string, string> onColorPicked)
	{
		if (player == null) return;

		OnColorPicked = onColorPicked;

		using var cui = new CUI(Handler);

		var container = cui.CreateContainer(PanelId,
			color: "0 0 0 0.75",
			xMin: 0, xMax: 1, yMin: 0, yMax: 1,
			needsCursor: true, destroyUi: PanelId);

		cui.CreatePanel(container, parent: PanelId, id: PanelId + ".color",
			color: "0 0 0 0.6",
			xMin: 0.3f, xMax: 0.7f, yMin: 0.275f, yMax: 0.825f);
		cui.CreatePanel(container, PanelId + ".color", PanelId + ".main",
			color: "0 0 0 0.5",
			blur: true);

		cui.CreateText(container, parent: PanelId + ".main", id: null,
			color: "1 1 1 0.8",
			text: "<b>Color Picker</b>", 18,
			xMin: 0f, yMin: 0.8f, xMax: 1f, yMax: 0.98f,
			align: TextAnchor.UpperCenter,
			font: CUI.Handler.FontTypes.RobotoCondensedBold);

		#region Main

		var scale = 20f;
		var offset = scale * 0.770f;
		var total = (scale * 2) - 8f;

		var topRightColor = Color.blue;
		var bottomRightColor = Color.green;
		var topLeftColor = Color.red;
		var bottomLeftColor = Color.yellow;

		cui.CreateText(container, parent: PanelId + ".main", id: null,
			color: "1 1 1 0.3",
			text: "------------------------------------------------------------------------------------------------------------------------------------- BRIGHTNESS", 8,
			xMin: 0f, xMax: 0.775f, yMin: 0.01f, yMax: 0.98f,
			align: TextAnchor.LowerRight,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateText(container, parent: PanelId + ".main", id: null,
			color: "1 1 1 0.3",
			text: "SHADES ---------------", 8,
			xMin: 0.805f, xMax: 1, yMin: 0.085f, yMax: 1f,
			align: TextAnchor.LowerLeft,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreatePanel(container, PanelId + ".main", PanelId + ".picker",
			color: "0 0 0 0",
			xMin: 0.175f, xMax: 0.8f, yMin: 0.1f, yMax: 0.9f);

		for (float y = 0; y < scale; y += 1f)
		{
			var heightColor = Color.Lerp(topRightColor, bottomRightColor, y.Scale(0f, scale, 0f, 1f));

			for (float x = 0; x < scale; x += 1f)
			{
				var widthColor = Color.Lerp(topLeftColor, bottomLeftColor, (x + y).Scale(0f, total, 0f, 1f));
				var color = Color.Lerp(widthColor, heightColor, x.Scale(0f, scale, 0f, 1f)) * Brightness;
				DrawColor(cui, container, scale, color, PanelId + ".picker", offset * x, -(offset * y), fade: !FirstOpen ? CurrentAnimation : 0);

				CurrentAnimation += AnimationLength;
			}
		}

		//
		// Brightness
		//
		var counter = 0;
		for (float x = 0; x < scale; x += 1f)
		{
			var color = Color.Lerp(Color.black, Color.white, x.Scale(0f, scale, 0f, 1f));
			DrawColor(cui, container, scale, color, PanelId + ".picker", offset * x, -(offset * (scale + 1f)), "brightness", fade: !FirstOpen ? CurrentAnimation : 0, index: counter);

			CurrentAnimation += AnimationLength;
			counter++;
		}

		//
		// Saturation
		//
		for (float y = 0; y < scale; y += 1f)
		{
			var color = Color.Lerp(Color.white, Color.black, y.Scale(0f, scale, 0f, 1f));
			DrawColor(cui, container, scale, color, PanelId + ".picker", offset * (scale + 1f), -(offset * y), fade: !FirstOpen ? CurrentAnimation : 0);

			CurrentAnimation += AnimationLength;
		}

		#endregion

		cui.CreateProtectedButton(container, parent: PanelId + ".main", id: null,
			color: "0.6 0.2 0.2 0.9",
			textColor: "1 0.5 0.5 1",
			text: "X", 8,
			xMin: 0.96f, xMax: 0.99f, yMin: 0.95f, yMax: 0.99f,
			command: PanelId + ".close",
			font: CUI.Handler.FontTypes.DroidSansMono);

		cui.Send(container, player);

		CurrentAnimation = 0;
	}
	internal void DrawColor(CUI cui, CuiElementContainer container, float scale, Color color, string parent, float xOffset, float yOffset, string mode = "color", float fade = 0f, int index = -1)
	{
		var size = Carbon.Extensions.MathEx.Scale(1f, 0, scale, 0f, 1f);

		var id = cui.CreateProtectedButton(container, parent, null,
			color: $"{color.r} {color.g} {color.b} 1",
			textColor: "0 0 0 0",
			text: string.Empty, 0,
			xMin: 0, yMin: size * (scale - 1f), xMax: 1f - (size * (scale - 1f)), yMax: 1f,
			OxMin: xOffset, OyMin: yOffset, OxMax: xOffset, OyMax: yOffset,
			fadeIn: fade,
			command: PanelId + $".pickcolor {mode} {ColorUtility.ToHtmlStringRGBA(color)} {color.r} {color.g} {color.b}");

		if (mode == "brightness" && index == BrightnessIndicator)
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

	[UiCommand(PanelId + ".close")]
	private void CloseUI(Arg args)
	{
		Close(args.Player());
	}

	[UiCommand(PanelId + ".pickcolor")]
	private void PickColorUI(Arg args)
	{
		var player = args.Player();
		var mode = args.Args[0];
		var hex = args.Args[1];
		var rawColor = args.Args.Skip(2).ToArray().ToString(", ", ", ");
		ColorUtility.TryParseHtmlString($"#{hex}", out var color);

		switch (mode)
		{
			case "brightness":
				Brightness = color.r.Scale(0f, 1f, 0f, 2.5f);
				BrightnessIndicator = (int)color.r.Scale(0f, 1f, 0f, 20.5f);
				Draw(player, OnColorPicked);
				return;
		}

		OnColorPicked?.Invoke(hex, rawColor);
		OnColorPicked = null;
		Close(args.Player());
	}
}

public class ColorPickerConfig
{

}
public class ColorPickerData
{

}
