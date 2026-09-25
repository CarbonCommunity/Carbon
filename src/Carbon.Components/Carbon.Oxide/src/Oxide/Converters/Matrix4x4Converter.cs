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

public class Matrix4x4Converter : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		if (value == null)
		{
			writer.WriteNull();
			return;
		}

		var matrix = (Matrix4x4)value;

		writer.WriteStartObject();

		for (int row = 0; row < 4; row++)
		{
			for (int column = 0; column < 4; column++)
			{
				writer.WritePropertyName($"m{row}{column}");
				writer.WriteValue(matrix[row, column]);
			}
		}

		writer.WriteEnd();
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.Null)
		{
			return default(Matrix4x4);
		}

		var jObject = JObject.Load(reader);
		var matrix = default(Matrix4x4);

		// Oxide's build never reads m10..m13 back, silently zeroing the second row of every matrix it
		// wrote. The written JSON does carry them, so reading all sixteen is a strict improvement.
		for (int row = 0; row < 4; row++)
		{
			for (int column = 0; column < 4; column++)
			{
				matrix[row, column] = (float)jObject[$"m{row}{column}"];
			}
		}

		return matrix;
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Matrix4x4);
	}
}
