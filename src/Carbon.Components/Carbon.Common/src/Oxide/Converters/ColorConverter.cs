using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Converters;

/*
 *
 * Copyright (c) 2024-2026 Carbon Community, under the GNU v3 license rights
 *
 * Mirrors the converter of the same name shipped in Oxide's patched Newtonsoft.Json.
 * See HashSetConverter for why these live here.
 *
 */

public class ColorConverter : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		if (value == null)
		{
			writer.WriteNull();
			return;
		}

		var color = (Color)value;

		writer.WriteStartObject();
		writer.WritePropertyName("a");
		writer.WriteValue(color.a);
		writer.WritePropertyName("r");
		writer.WriteValue(color.r);
		writer.WritePropertyName("g");
		writer.WriteValue(color.g);
		writer.WritePropertyName("b");
		writer.WriteValue(color.b);
		writer.WriteEndObject();
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Color) || objectType == typeof(Color32);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.Null)
		{
			return default(Color);
		}

		var jObject = JObject.Load(reader);

		if (objectType == typeof(Color32))
		{
			return new Color32((byte)jObject["r"], (byte)jObject["g"], (byte)jObject["b"], (byte)jObject["a"]);
		}

		return new Color((float)jObject["r"], (float)jObject["g"], (float)jObject["b"], (float)jObject["a"]);
	}
}
