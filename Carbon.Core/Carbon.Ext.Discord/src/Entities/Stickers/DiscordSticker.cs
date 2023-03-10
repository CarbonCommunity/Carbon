/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Helpers.Cdn;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Stickers
{
	// Token: 0x02000056 RID: 86
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordSticker : ISnowflakeEntity
	{
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060002A6 RID: 678 RVA: 0x0000F402 File Offset: 0x0000D602
		// (set) Token: 0x060002A7 RID: 679 RVA: 0x0000F40A File Offset: 0x0000D60A
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060002A8 RID: 680 RVA: 0x0000F413 File Offset: 0x0000D613
		// (set) Token: 0x060002A9 RID: 681 RVA: 0x0000F41B File Offset: 0x0000D61B
		[JsonProperty("pack_id")]
		public Snowflake? PackId { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060002AA RID: 682 RVA: 0x0000F424 File Offset: 0x0000D624
		// (set) Token: 0x060002AB RID: 683 RVA: 0x0000F42C File Offset: 0x0000D62C
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060002AC RID: 684 RVA: 0x0000F435 File Offset: 0x0000D635
		// (set) Token: 0x060002AD RID: 685 RVA: 0x0000F43D File Offset: 0x0000D63D
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060002AE RID: 686 RVA: 0x0000F446 File Offset: 0x0000D646
		// (set) Token: 0x060002AF RID: 687 RVA: 0x0000F44E File Offset: 0x0000D64E
		[JsonProperty("tags")]
		public string Tags { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x0000F457 File Offset: 0x0000D657
		// (set) Token: 0x060002B1 RID: 689 RVA: 0x0000F45F File Offset: 0x0000D65F
		[JsonProperty("type")]
		public StickerType Type { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x0000F468 File Offset: 0x0000D668
		// (set) Token: 0x060002B3 RID: 691 RVA: 0x0000F470 File Offset: 0x0000D670
		[JsonProperty("format_type")]
		public StickerFormatType FormatType { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x0000F479 File Offset: 0x0000D679
		// (set) Token: 0x060002B5 RID: 693 RVA: 0x0000F481 File Offset: 0x0000D681
		[JsonProperty("available")]
		public bool? Available { get; set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060002B6 RID: 694 RVA: 0x0000F48A File Offset: 0x0000D68A
		// (set) Token: 0x060002B7 RID: 695 RVA: 0x0000F492 File Offset: 0x0000D692
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x0000F49B File Offset: 0x0000D69B
		// (set) Token: 0x060002B9 RID: 697 RVA: 0x0000F4A3 File Offset: 0x0000D6A3
		[JsonProperty("user")]
		public DiscordUser User { get; set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060002BA RID: 698 RVA: 0x0000F4AC File Offset: 0x0000D6AC
		// (set) Token: 0x060002BB RID: 699 RVA: 0x0000F4B4 File Offset: 0x0000D6B4
		[JsonProperty("sort_value")]
		public int? SortValue { get; set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060002BC RID: 700 RVA: 0x0000F4BD File Offset: 0x0000D6BD
		public string StickerUrl
		{
			get
			{
				return DiscordCdn.GetSticker(this.Id, ImageFormat.Auto);
			}
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0000F4CC File Offset: 0x0000D6CC
		public static void GetSticker(DiscordClient client, Snowflake stickerId, Action<DiscordSticker> callback, Action<RestError> error = null)
		{
			bool flag = !stickerId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("stickerId");
			}
			client.Bot.Rest.DoRequest<DiscordSticker>(string.Format("/stickers/{0}", stickerId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0000F518 File Offset: 0x0000D718
		public void ModifyGuildSticker(DiscordClient client, Action<DiscordSticker> callback = null, Action<RestError> error = null)
		{
			bool flag = this.Type != StickerType.Guild;
			if (flag)
			{
				throw new Exception("This endpoint can only be used for guild stickers");
			}
			client.Bot.Rest.DoRequest<DiscordSticker>(string.Format("/guilds/{0}/stickers/{1}", this.GuildId, this.Id), RequestMethod.PATCH, this, callback, error);
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0000F578 File Offset: 0x0000D778
		public void DeleteGuildSticker(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			bool flag = this.Type != StickerType.Guild;
			if (flag)
			{
				throw new Exception("This endpoint can only be used for guild stickers");
			}
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/stickers/{1}", this.GuildId, this.Id), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0000F5D8 File Offset: 0x0000D7D8
		internal void Update(DiscordSticker sticker)
		{
			bool flag = sticker.Name != null;
			if (flag)
			{
				this.Name = sticker.Name;
			}
			bool flag2 = sticker.Description != null;
			if (flag2)
			{
				this.Description = sticker.Description;
			}
			bool flag3 = sticker.Tags != null;
			if (flag3)
			{
				this.Tags = sticker.Tags;
			}
		}
	}
}
