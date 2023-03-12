/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;

namespace Oxide.Ext.Discord.Entities.Stickers
{
	// Token: 0x02000057 RID: 87
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordStickerPack
	{
		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x0000F634 File Offset: 0x0000D834
		// (set) Token: 0x060002C3 RID: 707 RVA: 0x0000F63C File Offset: 0x0000D83C
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x0000F645 File Offset: 0x0000D845
		// (set) Token: 0x060002C5 RID: 709 RVA: 0x0000F64D File Offset: 0x0000D84D
		[JsonProperty("stickers")]
		public List<DiscordSticker> Stickers { get; set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x0000F656 File Offset: 0x0000D856
		// (set) Token: 0x060002C7 RID: 711 RVA: 0x0000F65E File Offset: 0x0000D85E
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x0000F667 File Offset: 0x0000D867
		// (set) Token: 0x060002C9 RID: 713 RVA: 0x0000F66F File Offset: 0x0000D86F
		[JsonProperty("sku_id")]
		public Snowflake SkuId { get; set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060002CA RID: 714 RVA: 0x0000F678 File Offset: 0x0000D878
		// (set) Token: 0x060002CB RID: 715 RVA: 0x0000F680 File Offset: 0x0000D880
		[JsonProperty("cover_sticker_id")]
		public Snowflake? CoverStickerId { get; set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060002CC RID: 716 RVA: 0x0000F689 File Offset: 0x0000D889
		// (set) Token: 0x060002CD RID: 717 RVA: 0x0000F691 File Offset: 0x0000D891
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060002CE RID: 718 RVA: 0x0000F69A File Offset: 0x0000D89A
		// (set) Token: 0x060002CF RID: 719 RVA: 0x0000F6A2 File Offset: 0x0000D8A2
		[JsonProperty("banner_asset_id")]
		public Snowflake? BannerAssetId { get; set; }

		// Token: 0x060002D0 RID: 720 RVA: 0x0000F6AB File Offset: 0x0000D8AB
		public static void GetNitroStickerPacks(DiscordClient client, Action<List<DiscordStickerPack>> callback, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<DiscordStickerPack>>("/sticker-packs", RequestMethod.GET, null, callback, error);
		}
	}
}
