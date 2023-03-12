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
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Invites
{
	// Token: 0x02000075 RID: 117
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordInvite
	{
		// Token: 0x1700011A RID: 282
		// (get) Token: 0x0600041F RID: 1055 RVA: 0x00010F37 File Offset: 0x0000F137
		// (set) Token: 0x06000420 RID: 1056 RVA: 0x00010F3F File Offset: 0x0000F13F
		[JsonProperty("code")]
		public string Code { get; set; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000421 RID: 1057 RVA: 0x00010F48 File Offset: 0x0000F148
		// (set) Token: 0x06000422 RID: 1058 RVA: 0x00010F50 File Offset: 0x0000F150
		[JsonProperty("guild")]
		public DiscordGuild Guild { get; set; }

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000423 RID: 1059 RVA: 0x00010F59 File Offset: 0x0000F159
		// (set) Token: 0x06000424 RID: 1060 RVA: 0x00010F61 File Offset: 0x0000F161
		[JsonProperty("channel")]
		public DiscordChannel Channel { get; set; }

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000425 RID: 1061 RVA: 0x00010F6A File Offset: 0x0000F16A
		// (set) Token: 0x06000426 RID: 1062 RVA: 0x00010F72 File Offset: 0x0000F172
		[JsonProperty("inviter")]
		public DiscordUser Inviter { get; set; }

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000427 RID: 1063 RVA: 0x00010F7B File Offset: 0x0000F17B
		// (set) Token: 0x06000428 RID: 1064 RVA: 0x00010F83 File Offset: 0x0000F183
		[JsonProperty("target_user")]
		public DiscordUser TargetUser { get; set; }

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x00010F8C File Offset: 0x0000F18C
		// (set) Token: 0x0600042A RID: 1066 RVA: 0x00010F94 File Offset: 0x0000F194
		[JsonProperty("target_user_type")]
		public TargetUserType? UserTargetType { get; set; }

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x0600042B RID: 1067 RVA: 0x00010F9D File Offset: 0x0000F19D
		// (set) Token: 0x0600042C RID: 1068 RVA: 0x00010FA5 File Offset: 0x0000F1A5
		[JsonProperty("approximate_presence_count")]
		public int? ApproximatePresenceCount { get; set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x0600042D RID: 1069 RVA: 0x00010FAE File Offset: 0x0000F1AE
		// (set) Token: 0x0600042E RID: 1070 RVA: 0x00010FB6 File Offset: 0x0000F1B6
		[JsonProperty("approximate_member_count")]
		public int? ApproximateMemberCount { get; set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x0600042F RID: 1071 RVA: 0x00010FBF File Offset: 0x0000F1BF
		// (set) Token: 0x06000430 RID: 1072 RVA: 0x00010FC7 File Offset: 0x0000F1C7
		[JsonProperty("expires_at")]
		public DateTime? ExpiresAt { get; set; }

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x00010FD0 File Offset: 0x0000F1D0
		// (set) Token: 0x06000432 RID: 1074 RVA: 0x00010FD8 File Offset: 0x0000F1D8
		[JsonProperty("stage_instance")]
		public InviteStageInstance StageInstance { get; set; }

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000433 RID: 1075 RVA: 0x00010FE1 File Offset: 0x0000F1E1
		// (set) Token: 0x06000434 RID: 1076 RVA: 0x00010FE9 File Offset: 0x0000F1E9
		[JsonProperty("guild_scheduled_event")]
		public InviteStageInstance GuildScheduledEvent { get; set; }

		// Token: 0x06000435 RID: 1077 RVA: 0x00010FF2 File Offset: 0x0000F1F2
		public static void GetInvite(DiscordClient client, string inviteCode, InviteLookup lookup = null, Action<DiscordInvite> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordInvite>("/invites/" + inviteCode + ((lookup != null) ? lookup.ToQueryString() : null), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x00011022 File Offset: 0x0000F222
		public void DeleteInvite(DiscordClient client, Action<DiscordInvite> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordInvite>("/invites/" + this.Code, RequestMethod.DELETE, null, callback, error);
		}
	}
}
