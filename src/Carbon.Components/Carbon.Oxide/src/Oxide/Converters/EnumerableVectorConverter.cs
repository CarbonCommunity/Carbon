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

public class EnumerableVectorConverter<T> : JsonConverter
{
	private static readonly VectorConverter VectorConverter = new();

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		var array = (value as IEnumerable<T>)?.ToArray();

		if (array == null)
		{
			writer.WriteNull();
			return;
		}

		writer.WriteStartArray();

		for (int i = 0; i < array.Length; i++)
		{
			VectorConverter.WriteJson(writer, array[i], serializer);
		}

		writer.WriteEndArray();
	}

	public override bool CanConvert(Type objectType)
	{
		return typeof(IEnumerable<Vector2>).IsAssignableFrom(objectType) ||
		       typeof(IEnumerable<Vector3>).IsAssignableFrom(objectType) ||
		       typeof(IEnumerable<Vector4>).IsAssignableFrom(objectType);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.Null)
		{
			return null;
		}

		// Oxide's build reads this back as a JObject, which can't round-trip the array WriteJson emits.
		var array = JArray.Load(reader);
		var list = new List<T>(array.Count);

		for (int i = 0; i < array.Count; i++)
		{
			list.Add(JsonConvert.DeserializeObject<T>(array[i].ToString()));
		}

		return list;
	}
}
