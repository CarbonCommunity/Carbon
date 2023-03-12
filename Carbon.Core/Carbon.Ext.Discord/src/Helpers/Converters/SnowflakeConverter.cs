using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities;

namespace Oxide.Ext.Discord.Helpers.Converters
{
	// Token: 0x0200002E RID: 46
	public class SnowflakeConverter : JsonConverter
	{
		// Token: 0x0600018A RID: 394 RVA: 0x0000CCE8 File Offset: 0x0000AEE8
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			switch (reader.TokenType)
			{
			case ( JsonToken )7:
				return new Snowflake(ulong.Parse(reader.Value.ToString()));
			case (JsonToken)9:
			{
				string value = reader.Value.ToString();
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					return default(Snowflake);
				}
				Snowflake snowflake;
				bool flag2 = Snowflake.TryParse(value, out snowflake);
				if (flag2)
				{
					return snowflake;
				}
				throw new JsonException(string.Format("Snowflake string JSON token failed to parse to snowflake: '{0}' Path: {1}", reader.Value, reader.Path));
			}
			case (JsonToken)11:
			{
				bool flag3 = Nullable.GetUnderlyingType(objectType) != null;
				if (flag3)
				{
					return null;
				}
				DiscordExtension.GlobalLogger.Warning("Snowflake tried to parse null to non nullable field: " + reader.Path + ". Please give this message to the discord extension authors.");
				return default(Snowflake);
			}
			}
			throw new JsonException("Token type " + reader.TokenType.ToString() + " does not match snowflake valid types of string or integer. Path: " + reader.Path);
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000CE25 File Offset: 0x0000B025
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000CE38 File Offset: 0x0000B038
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Snowflake);
		}
	}
}
