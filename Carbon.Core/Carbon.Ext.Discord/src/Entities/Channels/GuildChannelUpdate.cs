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

namespace Oxide.Ext.Discord.Entities.Channels
{
	// Token: 0x020000FF RID: 255
	public class GuildChannelUpdate
	{
		// Token: 0x1700032F RID: 815
		// (get) Token: 0x0600093F RID: 2367 RVA: 0x00016E00 File Offset: 0x00015000
		// (set) Token: 0x06000940 RID: 2368 RVA: 0x00016E08 File Offset: 0x00015008
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06000941 RID: 2369 RVA: 0x00016E11 File Offset: 0x00015011
		// (set) Token: 0x06000942 RID: 2370 RVA: 0x00016E19 File Offset: 0x00015019
		[JsonProperty("type")]
		public ChannelType Type { get; set; }

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06000943 RID: 2371 RVA: 0x00016E22 File Offset: 0x00015022
		// (set) Token: 0x06000944 RID: 2372 RVA: 0x00016E2A File Offset: 0x0001502A
		[JsonProperty("position")]
		public int? Position { get; set; }

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06000945 RID: 2373 RVA: 0x00016E33 File Offset: 0x00015033
		// (set) Token: 0x06000946 RID: 2374 RVA: 0x00016E3B File Offset: 0x0001503B
		[JsonProperty("topic")]
		public string Topic { get; set; }

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06000947 RID: 2375 RVA: 0x00016E44 File Offset: 0x00015044
		// (set) Token: 0x06000948 RID: 2376 RVA: 0x00016E4C File Offset: 0x0001504C
		[JsonProperty("nsfw")]
		public bool? Nsfw { get; set; }

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06000949 RID: 2377 RVA: 0x00016E55 File Offset: 0x00015055
		// (set) Token: 0x0600094A RID: 2378 RVA: 0x00016E5D File Offset: 0x0001505D
		[JsonProperty("rate_limit_per_user")]
		public int? RateLimitPerUser { get; set; }

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x0600094B RID: 2379 RVA: 0x00016E66 File Offset: 0x00015066
		// (set) Token: 0x0600094C RID: 2380 RVA: 0x00016E6E File Offset: 0x0001506E
		[JsonProperty("bitrate")]
		public int? Bitrate { get; set; }

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x0600094D RID: 2381 RVA: 0x00016E77 File Offset: 0x00015077
		// (set) Token: 0x0600094E RID: 2382 RVA: 0x00016E7F File Offset: 0x0001507F
		[JsonProperty("user_limit")]
		public int? UserLimit { get; set; }

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x0600094F RID: 2383 RVA: 0x00016E88 File Offset: 0x00015088
		// (set) Token: 0x06000950 RID: 2384 RVA: 0x00016E90 File Offset: 0x00015090
		[JsonProperty("permission_overwrites")]
		public List<Overwrite> PermissionOverwrites { get; set; }

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06000951 RID: 2385 RVA: 0x00016E99 File Offset: 0x00015099
		// (set) Token: 0x06000952 RID: 2386 RVA: 0x00016EA1 File Offset: 0x000150A1
		[JsonProperty("parent_id")]
		public Snowflake? ParentId { get; set; }

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06000953 RID: 2387 RVA: 0x00016EAA File Offset: 0x000150AA
		// (set) Token: 0x06000954 RID: 2388 RVA: 0x00016EB2 File Offset: 0x000150B2
		[JsonProperty("rtc_region")]
		public string RtcRegion { get; set; }

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06000955 RID: 2389 RVA: 0x00016EBB File Offset: 0x000150BB
		// (set) Token: 0x06000956 RID: 2390 RVA: 0x00016EC3 File Offset: 0x000150C3
		[JsonProperty("video_quality_mode")]
		public VideoQualityMode? VideoQualityMode { get; set; }

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06000957 RID: 2391 RVA: 0x00016ECC File Offset: 0x000150CC
		// (set) Token: 0x06000958 RID: 2392 RVA: 0x00016ED4 File Offset: 0x000150D4
		[JsonProperty("default_auto_archive_duration")]
		public int? DefaultAutoArchiveDuration { get; set; }
	}
}
