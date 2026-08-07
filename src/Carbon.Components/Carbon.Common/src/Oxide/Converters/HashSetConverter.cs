using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Converters;

/*
 *
 * Copyright (c) 2024-2026 Carbon Community, under the GNU v3 license rights
 *
 * Oxide ships a patched Newtonsoft.Json 8.0.0.0 that adds converters stock Newtonsoft doesn't have.
 * Rust moved to the official, strong-named 13.0.0.0 build, so plugins and extensions written against
 * Oxide would fail to resolve these types. They're mirrored here, under their original namespace, so
 * both plugin sources and Compat-converted extensions keep working.
 *
 */

public class HashSetConverter : JsonConverter
{
	public override bool CanWrite => false;

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var replace = serializer.ObjectCreationHandling == ObjectCreationHandling.Replace;

		if (reader.TokenType == JsonToken.Null)
		{
			return replace ? null : existingValue;
		}

		var set = !replace && existingValue != null ? existingValue : Activator.CreateInstance(objectType);
		var elementType = objectType.GetGenericArguments()[0];
		var add = objectType.GetMethod("Add");
		var array = JArray.Load(reader);

		for (int i = 0; i < array.Count; i++)
		{
			add.Invoke(set, [serializer.Deserialize(array[i].CreateReader(), elementType)]);
		}

		return set;
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(HashSet<>);
	}
}
