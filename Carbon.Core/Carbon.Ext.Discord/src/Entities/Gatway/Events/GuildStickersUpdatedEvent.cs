/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Stickers;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000D9 RID: 217
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildStickersUpdatedEvent
	{
		// Token: 0x17000295 RID: 661
		// (get) Token: 0x060007C1 RID: 1985 RVA: 0x00015426 File Offset: 0x00013626
		// (set) Token: 0x060007C2 RID: 1986 RVA: 0x0001542E File Offset: 0x0001362E
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x060007C3 RID: 1987 RVA: 0x00015437 File Offset: 0x00013637
		// (set) Token: 0x060007C4 RID: 1988 RVA: 0x0001543F File Offset: 0x0001363F
		[JsonConverter(typeof(HashListConverter<DiscordSticker>))]
		[JsonProperty("stickers")]
		public Hash<Snowflake, DiscordSticker> Stickers { get; set; }
	}
}
