/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Helpers;
using Oxide.Ext.Discord.Helpers.Cdn;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Emojis
{
	// Token: 0x020000F4 RID: 244
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordEmoji : EmojiUpdate, ISnowflakeEntity
	{
		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06000885 RID: 2181 RVA: 0x00015A24 File Offset: 0x00013C24
		public Snowflake Id
		{
			get
			{
				return this.EmojiId.GetValueOrDefault();
			}
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06000886 RID: 2182 RVA: 0x00015A3F File Offset: 0x00013C3F
		// (set) Token: 0x06000887 RID: 2183 RVA: 0x00015A47 File Offset: 0x00013C47
		[JsonProperty("id")]
		public Snowflake? EmojiId { get; set; }

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06000888 RID: 2184 RVA: 0x00015A50 File Offset: 0x00013C50
		// (set) Token: 0x06000889 RID: 2185 RVA: 0x00015A58 File Offset: 0x00013C58
		[JsonProperty("user")]
		public DiscordUser User { get; set; }

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x0600088A RID: 2186 RVA: 0x00015A61 File Offset: 0x00013C61
		// (set) Token: 0x0600088B RID: 2187 RVA: 0x00015A69 File Offset: 0x00013C69
		[JsonProperty("require_colons")]
		public bool? RequireColons { get; set; }

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x0600088C RID: 2188 RVA: 0x00015A72 File Offset: 0x00013C72
		// (set) Token: 0x0600088D RID: 2189 RVA: 0x00015A7A File Offset: 0x00013C7A
		[JsonProperty("managed")]
		public bool? Managed { get; set; }

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x0600088E RID: 2190 RVA: 0x00015A83 File Offset: 0x00013C83
		// (set) Token: 0x0600088F RID: 2191 RVA: 0x00015A8B File Offset: 0x00013C8B
		[JsonProperty("animated")]
		public bool? Animated { get; set; }

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06000890 RID: 2192 RVA: 0x00015A94 File Offset: 0x00013C94
		// (set) Token: 0x06000891 RID: 2193 RVA: 0x00015A9C File Offset: 0x00013C9C
		[JsonProperty("available")]
		public bool? Available { get; set; }

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06000892 RID: 2194 RVA: 0x00015AA8 File Offset: 0x00013CA8
		public string Url
		{
			get
			{
				return (this.EmojiId != null) ? DiscordCdn.GetCustomEmojiUrl(this.EmojiId.Value, (this.Animated != null && this.Animated.Value) ? ImageFormat.Gif : ImageFormat.Png) : null;
			}
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x00015B00 File Offset: 0x00013D00
		public static DiscordEmoji FromCharacter(string emoji)
		{
			return new DiscordEmoji
			{
				Name = emoji
			};
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x00015B20 File Offset: 0x00013D20
		public string ToDataString()
		{
			bool flag = this.EmojiId == null;
			string result;
			if (flag)
			{
				result = base.Name;
			}
			else
			{
				result = DiscordFormatting.CustomEmojiDataString(this.EmojiId.Value, base.Name, this.Animated.GetValueOrDefault());
			}
			return result;
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x00015B78 File Offset: 0x00013D78
		internal void Update(DiscordEmoji emoji)
		{
			bool flag = emoji.Name != null;
			if (flag)
			{
				base.Name = emoji.Name;
			}
			bool flag2 = emoji.Roles != null;
			if (flag2)
			{
				base.Roles = emoji.Roles;
			}
			bool flag3 = emoji.User != null;
			if (flag3)
			{
				this.User = emoji.User;
			}
			bool flag4 = emoji.RequireColons != null;
			if (flag4)
			{
				this.RequireColons = emoji.RequireColons;
			}
			bool flag5 = emoji.Managed != null;
			if (flag5)
			{
				this.Managed = emoji.Managed;
			}
			bool flag6 = emoji.Animated != null;
			if (flag6)
			{
				this.Animated = emoji.Animated;
			}
			bool flag7 = emoji.Available != null;
			if (flag7)
			{
				this.Available = emoji.Available;
			}
		}
	}
}
