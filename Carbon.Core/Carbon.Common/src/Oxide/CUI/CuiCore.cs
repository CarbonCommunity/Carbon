using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

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

	[JsonProperty( "command", NullValueHandling = NullValueHandling.Ignore)]
	public string Command { get; set; }

	[JsonProperty("close", NullValueHandling = NullValueHandling.Ignore)]
	public string Close { get; set; }

	[DefaultValue("Assets/Content/UI/UI.Background.Tile.psd")]
	[JsonProperty("sprite", NullValueHandling = NullValueHandling.Ignore)]
	public string Sprite { get; set; } = "Assets/Content/UI/UI.Background.Tile.psd";

	[DefaultValue("Assets/Icons/IconMaterial.mat")]
	[JsonProperty("material")]
	public string Material { get; set; } = "Assets/Icons/IconMaterial.mat";

	[DefaultValue("1 1 1 1")]
	[JsonProperty("color", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Color { get; set; } = "1 1 1 1";

	[DefaultValue(Image.Type.Simple)]
	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("imagetype")]
	public Image.Type ImageType { get; set; }

	[JsonProperty("fadeIn", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
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
				},
				DestroyUi = destroyUi
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

	[JsonProperty("sprite", NullValueHandling = NullValueHandling.Ignore)]
	public string Sprite { get; set; }

	[JsonProperty("material", NullValueHandling = NullValueHandling.Ignore)]
	public string Material { get; set; }

	[DefaultValue("1 1 1 1")]
	[JsonProperty("color", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Color { get; set; } = "1 1 1 1";

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("imagetype")]
	public Image.Type ImageType { get; set; }

	[JsonProperty("png", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Png { get; set; }

	[JsonProperty("fadeIn", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public float FadeIn { get; set; }

	[JsonProperty("itemid", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public int ItemId { get; set; }

	[JsonProperty("skinid", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public ulong SkinId { get; set; }
}
public class CuiInputFieldComponent : ICuiComponent, ICuiColor
{
	public string Type => "UnityEngine.UI.InputField";

	[DefaultValue("")]
	[JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
	public string Text { get; set; } = "";

	[DefaultValue(14)]
	[JsonProperty("fontSize")]
	public int FontSize { get; set; } = 14;

	[DefaultValue("RobotoCondensed-Bold.ttf")]
	[JsonProperty("font")]
	public string Font { get; set; } = "RobotoCondensed-Bold.ttf";

	[DefaultValue(TextAnchor.UpperLeft)]
	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("align")]
	public TextAnchor Align { get; set; }

	[DefaultValue("1 1 1 1")]
	[JsonProperty("color", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Color { get; set; } = "1 1 1 1";

	[DefaultValue(100)]
	[JsonProperty("characterLimit", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public int CharsLimit { get; set; } = 100;

	[JsonProperty("command", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Command { get; set; }

	[DefaultValue(false)]
	[JsonProperty("password", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public bool IsPassword { get; set; }

	[JsonProperty("readOnly", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public bool ReadOnly { get; set; }

	[DefaultValue(false)]
	[JsonProperty("needsCursor", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public bool NeedsCursor { get; set; }

	[DefaultValue(false)]
	[JsonProperty("needsKeyboard", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public bool NeedsKeyboard { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("lineType", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public InputField.LineType LineType { get; set; }

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

	[DefaultValue("1 1 1 1")]
	[JsonProperty("color", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Color { get; set; } = "1 1 1 1";

	[DefaultValue("1.0 -1.0")]
	[JsonProperty("distance", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Distance { get; set; } = "1.0 -1.0";

	[DefaultValue(false)]
	[JsonProperty("useGraphicAlpha", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public bool UseGraphicAlpha { get; set; }
}
public class CuiRawImageComponent : ICuiComponent, ICuiColor
{
	public string Type => "UnityEngine.UI.RawImage";

	[JsonProperty("sprite", NullValueHandling = NullValueHandling.Ignore)]
	public string Sprite { get; set; } = "assets/content/textures/generic/fulltransparent.tga";

	[DefaultValue("1 1 1 1")]
	[JsonProperty("color", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Color { get; set; } = "1 1 1 1";

	[JsonProperty("material", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Material { get; set; }

	[JsonProperty("url", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Url { get; set; }

	[JsonProperty("png", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Png { get; set; }

	[JsonProperty("fadeIn", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public float FadeIn { get; set; }
}
public class CuiRectTransformComponent : ICuiComponent
{
	public string Type => "RectTransform";

	[DefaultValue("0 0")]
	[JsonProperty("anchormin", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string AnchorMin { get; set; } = "0 0";

	[DefaultValue("1 1")]
	[JsonProperty("anchormax", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string AnchorMax { get; set; } = "1 1";

	[DefaultValue("0 0")]
	[JsonProperty("offsetmin", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string OffsetMin { get; set; } = "0 0";

	[DefaultValue("0 0")]
	[JsonProperty("offsetmax")]
	public string OffsetMax { get; set; } = "0 0";
}
public class CuiCountdownComponent : ICuiComponent
{
	public string Type => "Countdown";

	[JsonProperty("endTime", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public int EndTime { get; set; }

	[JsonProperty("startTime", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public int StartTime { get; set; }

	[JsonProperty("step", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public int Step { get; set; }

	[JsonProperty("command", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Command { get; set; }

	[JsonProperty("fadeIn", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public float FadeIn { get; set; }
}
public class CuiTextComponent : ICuiComponent, ICuiColor
{
	public string Type => "UnityEngine.UI.Text";

	[DefaultValue("")]
	[JsonProperty("text", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Text { get; set; } = "";

	[DefaultValue(14)]
	[JsonProperty("fontSize", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public int FontSize { get; set; } = 14;

	[DefaultValue("RobotoCondensed-Bold.ttf")]
	[JsonProperty("font")]
	public string Font { get; set; } = "RobotoCondensed-Bold.ttf";

	[DefaultValue(TextAnchor.UpperLeft)]
	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("align", NullValueHandling = NullValueHandling.Ignore)]
	public TextAnchor Align { get; set; }

	[DefaultValue("1 1 1 1")]
	[JsonProperty("color", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string Color { get; set; } = "1 1 1 1";

	[JsonProperty("fadeIn", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public float FadeIn { get; set; }
}

#endregion

public class CuiButton
{
	public CuiButtonComponent Button { get; } = new CuiButtonComponent();
	public CuiRectTransformComponent RectTransform { get; } = new CuiRectTransformComponent();
	public CuiTextComponent Text { get; } = new CuiTextComponent();
	public float FadeOut { get; set; }
}
public class CuiElement
{
	[DefaultValue("AddUI CreatedPanel")]
	[JsonProperty("name")]
	public string Name { get; set; } = "AddUI CreatedPanel";

	[JsonProperty("parent")]
	public string Parent { get; set; } = "Hud";

	[JsonProperty("components")]
	public List<ICuiComponent> Components { get; } = new List<ICuiComponent>();

	[JsonProperty("destroyUi", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string DestroyUi { get; set; }

	[JsonProperty("fadeOut", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	public float FadeOut { get; set; }
}
public class CuiLabel
{
	public CuiTextComponent Text { get; } = new CuiTextComponent();
	public CuiRectTransformComponent RectTransform { get; } = new CuiRectTransformComponent();
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
