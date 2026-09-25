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

public class QuaternionConverter : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		var quaternion = (Quaternion)value;

		writer.WriteStartObject();
		writer.WritePropertyName("w");
		writer.WriteValue(quaternion.w);
		writer.WritePropertyName("x");
		writer.WriteValue(quaternion.x);
		writer.WritePropertyName("y");
		writer.WriteValue(quaternion.y);
		writer.WritePropertyName("z");
		writer.WriteValue(quaternion.z);
		writer.WritePropertyName("eulerAngles");
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(quaternion.eulerAngles.x);
		writer.WritePropertyName("y");
		writer.WriteValue(quaternion.eulerAngles.y);
		writer.WritePropertyName("z");
		writer.WriteValue(quaternion.eulerAngles.z);
		writer.WriteEndObject();
		writer.WriteEndObject();
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Quaternion);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var jObject = JObject.Load(reader);
		var properties = jObject.Properties().ToList();
		var quaternion = default(Quaternion);

		if (properties.Any(p => p.Name == "w"))
		{
			quaternion.w = (float)jObject["w"];
		}

		if (properties.Any(p => p.Name == "x"))
		{
			quaternion.x = (float)jObject["x"];
		}

		if (properties.Any(p => p.Name == "y"))
		{
			quaternion.y = (float)jObject["y"];
		}

		if (properties.Any(p => p.Name == "z"))
		{
			quaternion.z = (float)jObject["z"];
		}

		if (properties.Any(p => p.Name == "eulerAngles"))
		{
			var eulerAngles = jObject["eulerAngles"];

			quaternion.eulerAngles = new Vector3((float)eulerAngles["x"], (float)eulerAngles["y"], (float)eulerAngles["z"]);
		}

		return quaternion;
	}
}
