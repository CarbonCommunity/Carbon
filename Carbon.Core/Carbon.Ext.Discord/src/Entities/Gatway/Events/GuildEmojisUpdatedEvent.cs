/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Emojis;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000CD RID: 205
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildEmojisUpdatedEvent
	{
		// Token: 0x17000279 RID: 633
		// (get) Token: 0x0600077D RID: 1917 RVA: 0x00015241 File Offset: 0x00013441
		// (set) Token: 0x0600077E RID: 1918 RVA: 0x00015249 File Offset: 0x00013449
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x0600077F RID: 1919 RVA: 0x00015252 File Offset: 0x00013452
		// (set) Token: 0x06000780 RID: 1920 RVA: 0x0001525A File Offset: 0x0001345A
		[JsonConverter(typeof(HashListConverter<DiscordEmoji>))]
		[JsonProperty("emojis")]
		public Hash<Snowflake, DiscordEmoji> Emojis { get; set; }
	}
}
