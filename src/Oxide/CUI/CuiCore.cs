using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine.UI;

namespace Oxide.Game.Rust.Cui;

public class ComponentConverter : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { }

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var jObject = JObject.Load(reader);
		var typeName = jObject["type"].ToString();
		var type = (Type)null;

		switch (typeName)
		{
			case "UnityEngine.UI.Text":
				type = typeof(CuiTextComponent);
				break;

			case "UnityEngine.UI.Image":
				type = typeof(CuiImageComponent);
				break;

			case "UnityEngine.UI.RawImage":
				type = typeof(CuiRawImageComponent);
				break;

			case "UnityEngine.UI.Button":
				type = typeof(CuiButtonComponent);
				break;

			case "UnityEngine.UI.Outline":
				type = typeof(CuiOutlineComponent);
				break;

			case "UnityEngine.UI.InputField":
				type = typeof(CuiInputFieldComponent);
				break;

			case "Countdown":
				type = typeof(CuiCountdownComponent);
				break;

			case "NeedsCursor":
				type = typeof(CuiNeedsCursorComponent);
				break;

			case "NeedsKeyboard":
				type = typeof(CuiNeedsKeyboardComponent);
				break;

			case "RectTransform":
				type = typeof(CuiRectTransformComponent);
				break;

			case "UnityEngine.UI.ScrollView":
				type = typeof(CuiScrollViewComponent);
				break;

			default:
				return null;
		}

		var target = Activator.CreateInstance(type);
		serializer.Populate(jObject.CreateReader(), target);
		return target;
	}

	public override bool CanConvert(Type objectType) => objectType == typeof(ICuiComponent);

	public override bool CanWrite => false;
}

#region Components

public class CuiButtonComponent : ICuiComponent, ICuiColor
{
	public string Type => "UnityEngine.UI.Button";

	[JsonProperty("command")]
	public string Command { get; set; }

	[JsonProperty("close")]
	public string Close { get; set; }

	[JsonProperty("sprite")]
	public string Sprite { get; set; }

	[JsonProperty("material")]
	public string Material { get; set; }

	public string Color { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("imagetype")]
	public Image.Type ImageType { get; set; }

	[JsonProperty("fadeIn")]
	public float FadeIn { get; set; }
}
public class CuiElementContainer : List<CuiElement>
{
	public string Name { get; set; }

	public string Add(CuiButton button, string parent = "Hud", string name = null, string destroyUi = null)
	{
		if (string.IsNullOrEmpty(name))
		{
			name = CuiHelper.GetGuid();
		}
		Add(new CuiElement
		{
			Name = name,
			Parent = parent,
			FadeOut = button.FadeOut,
			Components =
			{
				button.Button,
				button.RectTransform
			},
			DestroyUi = destroyUi
		});
		if (!string.IsNullOrEmpty(button.Text.Text))
		{
			Add(new CuiElement
			{
				Parent = name,
				FadeOut = button.FadeOut,
				Components =
				{
					button.Text,
					new CuiRectTransformComponent()
				}
			});
		}
		return name;
	}

	public string Add(CuiLabel label, string parent = "Hud", string name = null, string destroyUi = null)
	{
		if (string.IsNullOrEmpty(name))
		{
			name = CuiHelper.GetGuid();
		}

		Add(new CuiElement
		{
			Name = name,
			Parent = parent,
			FadeOut = label.FadeOut,
			Components =
			{
				label.Text,
				label.RectTransform
			},
			DestroyUi = destroyUi
		});
		return name;
	}

	public string Add(CuiPanel panel, string parent = "Hud", string name = null, string destroyUi = null)
	{
		if (string.IsNullOrEmpty(name))
		{
			name = CuiHelper.GetGuid();
		}

		var cuiElement = new CuiElement
		{
			Name = name,
			Parent = parent,
			FadeOut = panel.FadeOut,
			DestroyUi = destroyUi
		};
		if (panel.Image != null)
		{
			cuiElement.Components.Add(panel.Image);
		}
		if (panel.RawImage != null)
		{
			cuiElement.Components.Add(panel.RawImage);
		}

		cuiElement.Components.Add(panel.RectTransform);

		if (panel.CursorEnabled)
		{
			cuiElement.Components.Add(new CuiNeedsCursorComponent());
		}

		if (panel.KeyboardEnabled)
		{
			cuiElement.Components.Add(new CuiNeedsKeyboardComponent());
		}

		Add(cuiElement);
		return name;
	}

	public string ToJson()
	{
		return this.ToString();
	}

	public override string ToString()
	{
		return CuiHelper.ToJson(this, false);
	}
}
public class CuiImageComponent : ICuiComponent, ICuiColor
{
	public string Type => "UnityEngine.UI.Image";

	[JsonProperty("sprite")]
	public string Sprite { get; set; }

	[JsonProperty("material")]
	public string Material { get; set; }

	public string Color { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("imagetype")]
	public Image.Type ImageType { get; set; }

	[JsonProperty("png")]
	public string Png { get; set; }

	[JsonProperty("fadeIn")]
	public float FadeIn { get; set; }

	[JsonProperty("itemid")]
	public int ItemId { get; set; }

	[JsonProperty("skinid")]
	public ulong SkinId { get; set; }
}
public class CuiInputFieldComponent : ICuiComponent, ICuiColor
{
	public string Type => "UnityEngine.UI.InputField";

	[JsonProperty("text")]
	public string Text { get; set; } = string.Empty;

	[JsonProperty("fontSize")]
	public int FontSize { get; set; }

	[JsonProperty("font")]
	public string Font { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("align")]
	public TextAnchor Align { get; set; }

	public string Color { get; set; }

	[JsonProperty("characterLimit")]
	public int CharsLimit { get; set; }

	[JsonProperty("command")]
	public string Command { get; set; }

	[JsonProperty("password")]
	public bool IsPassword { get; set; }

	[JsonProperty("readOnly")]
	public bool ReadOnly { get; set; }

	[JsonProperty("needsKeyboard")]
	public bool NeedsKeyboard { get; set; }

	[JsonProperty("needsCursor")]
	public bool NeedsCursor { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("lineType")]
	public InputField.LineType LineType { get; set; }

	[JsonProperty("autofocus")]
	public bool Autofocus { get; set; }

	[JsonProperty("hudMenuInput")]
	public bool HudMenuInput { get; set; }
}
public class CuiNeedsCursorComponent : ICuiComponent
{
	public string Type => "NeedsCursor";
}
public class CuiNeedsKeyboardComponent : ICuiComponent
{
	public string Type => "NeedsKeyboard";
}
public class CuiOutlineComponent : ICuiComponent, ICuiColor
{
	public string Type => "UnityEngine.UI.Outline";

	public string Color { get; set; }

	[JsonProperty("distance")]
	public string Distance { get; set; }

	[JsonProperty("useGraphicAlpha")]
	public bool UseGraphicAlpha { get; set; }
}
public class CuiRawImageComponent : ICuiComponent, ICuiColor
{
	public string Type => "UnityEngine.UI.RawImage";

	[JsonProperty("sprite")]
	public string Sprite { get; set; }

	public string Color { get; set; }

	[JsonProperty("material")]
	public string Material { get; set; }

	[JsonProperty("url")]
	public string Url { get; set; }

	[JsonProperty("png")]
	public string Png { get; set; }

	[JsonProperty("fadeIn")]
	public float FadeIn { get; set; }

	[JsonProperty("steamid")]
	public string SteamId { get; set; }
}
public class CuiRectTransformComponent : CuiRectTransform, ICuiComponent
{
	public string Type => "RectTransform";

}
public class CuiRectTransform
{
	[JsonProperty("anchormin")]
	public string AnchorMin { get; set; }

	[JsonProperty("anchormax")]
	public string AnchorMax { get; set; }

	[JsonProperty("offsetmin")]
	public string OffsetMin { get; set; }

	[JsonProperty("offsetmax")]
	public string OffsetMax { get; set; }
}
public class CuiCountdownComponent : ICuiComponent
{
	public string Type => "Countdown";

	[JsonProperty("endTime")]
	public float EndTime { get; set; }

	[JsonProperty("startTime")]
	public float StartTime { get; set; }

	[JsonProperty("step")]
	public float Step { get; set; }

	[JsonProperty("interval")]
	public float Interval { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("timerFormat")]
	public TimerFormat TimerFormat { get; set; }

	[JsonProperty("numberFormat")]
	public string NumberFormat { get; set; }

	[JsonProperty("destroyIfDone")]
	public bool DestroyIfDone { get; set; }

	[JsonProperty("command")]
	public string Command { get; set; }

	[JsonProperty("fadeIn")]
	public float FadeIn { get; set; }
}

public enum TimerFormat
{
	None,
	SecondsHundreth,
	MinutesSeconds,
	MinutesSecondsHundreth,
	HoursMinutes,
	HoursMinutesSeconds,
	HoursMinutesSecondsMilliseconds,
	HoursMinutesSecondsTenths,
	DaysHoursMinutes,
	DaysHoursMinutesSeconds,
	Custom
}

public class CuiTextComponent : ICuiComponent, ICuiColor
{
	public string Type => "UnityEngine.UI.Text";

	[JsonProperty("text")]
	public string Text { get; set; }

	[JsonProperty("fontSize")]
	public int FontSize { get; set; }

	[JsonProperty("font")]
	public string Font { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("align")]
	public TextAnchor Align { get; set; }

	public string Color { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("verticalOverflow")]
	public VerticalWrapMode VerticalOverflow { get; set; }

	[JsonProperty("fadeIn")]
	public float FadeIn { get; set; }
}
public class CuiScrollViewComponent : ICuiComponent
{
	public string Type => "UnityEngine.UI.ScrollView";

	[JsonProperty("vertical")]
	public bool Vertical { get; set; }

	[JsonProperty("horizontal")]
	public bool Horizontal { get; set; }

	[JsonProperty("movementType")]
	[JsonConverter(typeof(StringEnumConverter))]
	public ScrollRect.MovementType MovementType { get; set; }

	[JsonProperty("elasticity")]
	public float Elasticity { get; set; }

	[JsonProperty("inertia")]
	public bool Inertia { get; set; }

	[JsonProperty("decelerationRate")]
	public float DecelerationRate { get; set; }

	[JsonProperty("scrollSensitivity")]
	public float ScrollSensitivity { get; set; }

	[JsonProperty("contentTransform")]
	public CuiRectTransform ContentTransform { get; set; }

	[JsonProperty("horizontalScrollbar")]
	public CuiScrollbar HorizontalScrollbar { get; set; }

	[JsonProperty("verticalScrollbar")]
	public CuiScrollbar VerticalScrollbar { get; set; }
}
public class CuiScrollbar : ICuiComponent
{
	public string Type => "UnityEngine.UI.Scrollbar";

	[JsonProperty("invert")]
	public bool Invert { get; set; }

	[JsonProperty("autoHide")]
	public bool AutoHide { get; set; }

	[JsonProperty("handleSprite")]
	public string HandleSprite { get; set; } = "assets/content/ui/ui.rounded.tga";

	[DefaultValue(20)]
	[JsonProperty("size")]
	public float Size { get; set; } = 20;

	[JsonProperty("handleColor")]
	public string HandleColor { get; set; } = "0.15 0.15 0.15 1";

	[JsonProperty("highlightColor")]
	public string HighlightColor { get; set; } = "0.17 0.17 0.17 1";

	[JsonProperty("pressedColor")]
	public string PressedColor { get; set; } = "0.2 0.2 0.2 1";

	[JsonProperty("trackSprite")]
	public string TrackSprite { get; set; } = "assets/content/ui/ui.background.tile.psd";

	[JsonProperty("trackColor")]
	public string TrackColor { get; set; } = "0.09 0.09 0.09 1";
}

#endregion

public class CuiButton
{
	public CuiButtonComponent Button { get; } = new();
	public CuiRectTransformComponent RectTransform { get; } = new();
	public CuiTextComponent Text { get; } = new();
	public float FadeOut { get; set; }
}
public class CuiElement
{
	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("parent")]
	public string Parent { get; set; }

	[JsonProperty("components")]
	public List<ICuiComponent> Components { get; } = new();

	[JsonProperty("destroyUi")]
	public string DestroyUi { get; set; }

	[JsonProperty("fadeOut")]
	public float FadeOut { get; set; }

	[JsonProperty("update")]
	public bool Update { get; set; }
}
public class CuiLabel
{
	public CuiTextComponent Text { get; } = new();
	public CuiRectTransformComponent RectTransform { get; } = new();
	public float FadeOut { get; set; }
}
public class CuiPanel
{
	public CuiImageComponent Image { get; set; } = new CuiImageComponent();
	public CuiRawImageComponent RawImage { get; set; }
	public CuiRectTransformComponent RectTransform { get; } = new CuiRectTransformComponent();
	public bool KeyboardEnabled { get; set; }
	public bool CursorEnabled { get; set; }
	public float FadeOut { get; set; }
}
