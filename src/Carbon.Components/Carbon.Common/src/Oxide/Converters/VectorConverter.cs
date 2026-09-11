using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Converters;

/*
 *
 * Copyright (c) 2024-2026 Carbon Community, under the GNU v3 license rights
 *
 * Mirrors the converter of the same name shipped in Oxide's patched Newtonsoft.Json.
 * See HashSetConverter for why these live here.
 *
 */

[Preserve]
public class VectorConverter : JsonConverter
{
	private static readonly Type V2 = typeof(Vector2);
	private static readonly Type V3 = typeof(Vector3);
	private static readonly Type V4 = typeof(Vector4);

	public bool EnableVector2 { get; set; }
	public bool EnableVector3 { get; set; }
	public bool EnableVector4 { get; set; }

	public VectorConverter()
	{
		EnableVector2 = true;
		EnableVector3 = true;
		EnableVector4 = true;
	}

	public VectorConverter(bool enableVector2, bool enableVector3, bool enableVector4) : this()
	{
		EnableVector2 = enableVector2;
		EnableVector3 = enableVector3;
		EnableVector4 = enableVector4;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		if (value == null)
		{
			writer.WriteNull();
			return;
		}

		var type = value.GetType();

		if (type == V2)
		{
			var vector = (Vector2)value;
			WriteVector(writer, vector.x, vector.y, null, null);
		}
		else if (type == V3)
		{
			var vector = (Vector3)value;
			WriteVector(writer, vector.x, vector.y, vector.z, null);
		}
		else if (type == V4)
		{
			var vector = (Vector4)value;
			WriteVector(writer, vector.x, vector.y, vector.z, vector.w);
		}
		else
		{
			writer.WriteNull();
		}
	}

	private static void WriteVector(JsonWriter writer, float x, float y, float? z, float? w)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(x);
		writer.WritePropertyName("y");
		writer.WriteValue(y);

		if (z.HasValue)
		{
			writer.WritePropertyName("z");
			writer.WriteValue(z.Value);

			if (w.HasValue)
			{
				writer.WritePropertyName("w");
				writer.WriteValue(w.Value);
			}
		}

		writer.WriteEndObject();
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (objectType == V2)
		{
			return PopulateVector2(reader);
		}

		if (objectType == V3)
		{
			return PopulateVector3(reader);
		}

		return PopulateVector4(reader);
	}

	public override bool CanConvert(Type objectType)
	{
		return (EnableVector2 && objectType == V2) || (EnableVector3 && objectType == V3) || (EnableVector4 && objectType == V4);
	}

	private static Vector2 PopulateVector2(JsonReader reader)
	{
		var result = default(Vector2);

		if (reader.TokenType != JsonToken.Null)
		{
			var jObject = JObject.Load(reader);
			result.x = jObject["x"].Value<float>();
			result.y = jObject["y"].Value<float>();
		}

		return result;
	}

	private static Vector3 PopulateVector3(JsonReader reader)
	{
		var result = default(Vector3);

		if (reader.TokenType != JsonToken.Null)
		{
			var jObject = JObject.Load(reader);
			result.x = jObject["x"].Value<float>();
			result.y = jObject["y"].Value<float>();
			result.z = jObject["z"].Value<float>();
		}

		return result;
	}

	private static Vector4 PopulateVector4(JsonReader reader)
	{
		var result = default(Vector4);

		if (reader.TokenType != JsonToken.Null)
		{
			var jObject = JObject.Load(reader);
			result.x = jObject["x"].Value<float>();
			result.y = jObject["y"].Value<float>();
			result.z = jObject["z"].Value<float>();
			result.w = jObject["w"].Value<float>();
		}

		return result;
	}
}
