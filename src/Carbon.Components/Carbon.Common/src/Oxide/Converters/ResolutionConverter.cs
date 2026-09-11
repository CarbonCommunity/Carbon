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

public class ResolutionConverter : JsonConverter
{
	// Resolution.refreshRate is obsolete in favour of refreshRateRatio, but the property name is part
	// of the serialized shape Oxide plugins already have on disk.
#pragma warning disable CS0618

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		var resolution = (Resolution)value;

		writer.WriteStartObject();
		writer.WritePropertyName("height");
		writer.WriteValue(resolution.height);
		writer.WritePropertyName("width");
		writer.WriteValue(resolution.width);
		writer.WritePropertyName("refreshRate");
		writer.WriteValue(resolution.refreshRate);
		writer.WriteEndObject();
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Resolution);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var jObject = JObject.Load(reader);
		var resolution = default(Resolution);

		resolution.height = (int)jObject["height"];
		resolution.width = (int)jObject["width"];
		resolution.refreshRate = (int)jObject["refreshRate"];

		return resolution;
	}

#pragma warning restore CS0618
}
