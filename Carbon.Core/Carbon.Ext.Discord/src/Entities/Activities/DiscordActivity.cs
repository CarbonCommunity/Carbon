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
using Oxide.Ext.Discord.Entities.Emojis;
using Oxide.Ext.Discord.Helpers;
using Oxide.Ext.Discord.Helpers.Cdn;

namespace Oxide.Ext.Discord.Entities.Activities
{
	// Token: 0x02000125 RID: 293
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordActivity
	{
		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06000AB5 RID: 2741 RVA: 0x00017EF2 File Offset: 0x000160F2
		// (set) Token: 0x06000AB6 RID: 2742 RVA: 0x00017EFA File Offset: 0x000160FA
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06000AB7 RID: 2743 RVA: 0x00017F03 File Offset: 0x00016103
		// (set) Token: 0x06000AB8 RID: 2744 RVA: 0x00017F0B File Offset: 0x0001610B
		[JsonProperty("type")]
		public ActivityType Type { get; set; }

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06000AB9 RID: 2745 RVA: 0x00017F14 File Offset: 0x00016114
		// (set) Token: 0x06000ABA RID: 2746 RVA: 0x00017F1C File Offset: 0x0001611C
		[JsonProperty("url")]
		public string Url { get; set; }

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06000ABB RID: 2747 RVA: 0x00017F25 File Offset: 0x00016125
		// (set) Token: 0x06000ABC RID: 2748 RVA: 0x00017F2D File Offset: 0x0001612D
		[JsonProperty("created_at")]
		public long CreatedAt { get; set; }

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06000ABD RID: 2749 RVA: 0x00017F36 File Offset: 0x00016136
		public DateTime CreatedAtDateTime
		{
			get
			{
				return this.CreatedAt.ToDateTimeOffsetFromMilliseconds();
			}
		}

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06000ABE RID: 2750 RVA: 0x00017F43 File Offset: 0x00016143
		// (set) Token: 0x06000ABF RID: 2751 RVA: 0x00017F4B File Offset: 0x0001614B
		[JsonProperty("timestamps")]
		public List<ActivityTimestamps> Timestamps { get; set; }

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06000AC0 RID: 2752 RVA: 0x00017F54 File Offset: 0x00016154
		// (set) Token: 0x06000AC1 RID: 2753 RVA: 0x00017F5C File Offset: 0x0001615C
		[JsonProperty("application_id")]
		public Snowflake? ApplicationId { get; set; }

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x06000AC2 RID: 2754 RVA: 0x00017F65 File Offset: 0x00016165
		// (set) Token: 0x06000AC3 RID: 2755 RVA: 0x00017F6D File Offset: 0x0001616D
		[JsonProperty("details")]
		public string Details { get; set; }

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06000AC4 RID: 2756 RVA: 0x00017F76 File Offset: 0x00016176
		// (set) Token: 0x06000AC5 RID: 2757 RVA: 0x00017F7E File Offset: 0x0001617E
		[JsonProperty("state")]
		public string State { get; set; }

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06000AC6 RID: 2758 RVA: 0x00017F87 File Offset: 0x00016187
		// (set) Token: 0x06000AC7 RID: 2759 RVA: 0x00017F8F File Offset: 0x0001618F
		[JsonProperty("emoji")]
		public DiscordEmoji Emoji { get; set; }

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06000AC8 RID: 2760 RVA: 0x00017F98 File Offset: 0x00016198
		// (set) Token: 0x06000AC9 RID: 2761 RVA: 0x00017FA0 File Offset: 0x000161A0
		[JsonProperty("party")]
		public ActivityParty Party { get; set; }

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06000ACA RID: 2762 RVA: 0x00017FA9 File Offset: 0x000161A9
		// (set) Token: 0x06000ACB RID: 2763 RVA: 0x00017FB1 File Offset: 0x000161B1
		[JsonProperty("assets")]
		public ActivityAssets Assets { get; set; }

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06000ACC RID: 2764 RVA: 0x00017FBA File Offset: 0x000161BA
		// (set) Token: 0x06000ACD RID: 2765 RVA: 0x00017FC2 File Offset: 0x000161C2
		[JsonProperty("secrets")]
		public ActivitySecrets Secrets { get; set; }

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06000ACE RID: 2766 RVA: 0x00017FCB File Offset: 0x000161CB
		// (set) Token: 0x06000ACF RID: 2767 RVA: 0x00017FD3 File Offset: 0x000161D3
		[JsonProperty("instance")]
		public bool? Instance { get; set; }

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06000AD0 RID: 2768 RVA: 0x00017FDC File Offset: 0x000161DC
		// (set) Token: 0x06000AD1 RID: 2769 RVA: 0x00017FE4 File Offset: 0x000161E4
		[JsonProperty("flags")]
		public ActivityFlags? Flags { get; set; }

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06000AD2 RID: 2770 RVA: 0x00017FED File Offset: 0x000161ED
		// (set) Token: 0x06000AD3 RID: 2771 RVA: 0x00017FF5 File Offset: 0x000161F5
		[JsonProperty("buttons")]
		public List<ActivityButton> Buttons { get; set; }

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06000AD4 RID: 2772 RVA: 0x00018000 File Offset: 0x00016200
		public string GetLargeImageUrl
		{
			get
			{
				return (this.ApplicationId != null) ? DiscordCdn.GetApplicationAssetUrl(this.ApplicationId.Value, this.Assets.LargeImage, ImageFormat.Auto) : null;
			}
		}

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06000AD5 RID: 2773 RVA: 0x00018040 File Offset: 0x00016240
		public string GetSmallImageUrl
		{
			get
			{
				return (this.ApplicationId != null) ? DiscordCdn.GetApplicationAssetUrl(this.ApplicationId.Value, this.Assets.SmallImage, ImageFormat.Auto) : null;
			}
		}
	}
}
