/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Voice
{
	// Token: 0x0200004A RID: 74
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class VoiceState : ISnowflakeEntity
	{
		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000221 RID: 545 RVA: 0x0000E7D7 File Offset: 0x0000C9D7
		public Snowflake Id
		{
			get
			{
				return this.UserId;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000222 RID: 546 RVA: 0x0000E7DF File Offset: 0x0000C9DF
		// (set) Token: 0x06000223 RID: 547 RVA: 0x0000E7E7 File Offset: 0x0000C9E7
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000224 RID: 548 RVA: 0x0000E7F0 File Offset: 0x0000C9F0
		// (set) Token: 0x06000225 RID: 549 RVA: 0x0000E7F8 File Offset: 0x0000C9F8
		[JsonProperty("channel_id")]
		public Snowflake? ChannelId { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000226 RID: 550 RVA: 0x0000E801 File Offset: 0x0000CA01
		// (set) Token: 0x06000227 RID: 551 RVA: 0x0000E809 File Offset: 0x0000CA09
		[JsonProperty("user_id")]
		public Snowflake UserId { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000228 RID: 552 RVA: 0x0000E812 File Offset: 0x0000CA12
		// (set) Token: 0x06000229 RID: 553 RVA: 0x0000E81A File Offset: 0x0000CA1A
		[JsonProperty("member")]
		public GuildMember Member { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600022A RID: 554 RVA: 0x0000E823 File Offset: 0x0000CA23
		// (set) Token: 0x0600022B RID: 555 RVA: 0x0000E82B File Offset: 0x0000CA2B
		[JsonProperty("session_id")]
		public string SessionId { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600022C RID: 556 RVA: 0x0000E834 File Offset: 0x0000CA34
		// (set) Token: 0x0600022D RID: 557 RVA: 0x0000E83C File Offset: 0x0000CA3C
		[JsonProperty("deaf")]
		public bool Deaf { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600022E RID: 558 RVA: 0x0000E845 File Offset: 0x0000CA45
		// (set) Token: 0x0600022F RID: 559 RVA: 0x0000E84D File Offset: 0x0000CA4D
		[JsonProperty("mute")]
		public bool Mute { get; set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000230 RID: 560 RVA: 0x0000E856 File Offset: 0x0000CA56
		// (set) Token: 0x06000231 RID: 561 RVA: 0x0000E85E File Offset: 0x0000CA5E
		[JsonProperty("self_deaf")]
		public bool SelfDeaf { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000232 RID: 562 RVA: 0x0000E867 File Offset: 0x0000CA67
		// (set) Token: 0x06000233 RID: 563 RVA: 0x0000E86F File Offset: 0x0000CA6F
		[JsonProperty("self_mute")]
		public bool SelfMute { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000234 RID: 564 RVA: 0x0000E878 File Offset: 0x0000CA78
		// (set) Token: 0x06000235 RID: 565 RVA: 0x0000E880 File Offset: 0x0000CA80
		[JsonProperty("self_stream")]
		public bool? SelfStream { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000236 RID: 566 RVA: 0x0000E889 File Offset: 0x0000CA89
		// (set) Token: 0x06000237 RID: 567 RVA: 0x0000E891 File Offset: 0x0000CA91
		[JsonProperty("self_video")]
		public bool SelfVideo { get; set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000238 RID: 568 RVA: 0x0000E89A File Offset: 0x0000CA9A
		// (set) Token: 0x06000239 RID: 569 RVA: 0x0000E8A2 File Offset: 0x0000CAA2
		[JsonProperty("suppress")]
		public bool Suppress { get; set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600023A RID: 570 RVA: 0x0000E8AB File Offset: 0x0000CAAB
		// (set) Token: 0x0600023B RID: 571 RVA: 0x0000E8B3 File Offset: 0x0000CAB3
		[JsonProperty("request_to_speak_timestamp")]
		public DateTime? RequestToSpeakTimestamp { get; set; }

		// Token: 0x0600023C RID: 572 RVA: 0x0000E8BC File Offset: 0x0000CABC
		internal void Update(VoiceState state)
		{
			bool flag = state.ChannelId != this.ChannelId;
			if (flag)
			{
				this.ChannelId = state.ChannelId;
			}
			bool flag2 = state.SessionId != this.SessionId;
			if (flag2)
			{
				this.SessionId = state.Id;
			}
			this.Deaf = state.Deaf;
			this.Mute = state.Mute;
			this.SelfDeaf = state.SelfDeaf;
			this.SelfMute = state.SelfMute;
			bool? selfStream = state.SelfStream;
			bool? selfStream2 = this.SelfStream;
			bool flag3 = !(selfStream.GetValueOrDefault() == selfStream2.GetValueOrDefault() & selfStream != null == (selfStream2 != null));
			if (flag3)
			{
				this.SelfStream = state.SelfStream;
			}
			this.SelfVideo = state.SelfVideo;
			this.Suppress = state.Suppress;
			bool flag4 = state.RequestToSpeakTimestamp != this.RequestToSpeakTimestamp;
			if (flag4)
			{
				this.RequestToSpeakTimestamp = state.RequestToSpeakTimestamp;
			}
		}
	}
}
