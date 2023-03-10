/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Ext.Discord.Builders;

namespace Oxide.Ext.Discord.Entities.Invites
{
	// Token: 0x02000076 RID: 118
	public class InviteLookup
	{
		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000438 RID: 1080 RVA: 0x0001104A File Offset: 0x0000F24A
		// (set) Token: 0x06000439 RID: 1081 RVA: 0x00011052 File Offset: 0x0000F252
		public bool? WithCounts { get; set; }

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x0001105B File Offset: 0x0000F25B
		// (set) Token: 0x0600043B RID: 1083 RVA: 0x00011063 File Offset: 0x0000F263
		public bool? WithExpiration { get; set; }

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x0001106C File Offset: 0x0000F26C
		// (set) Token: 0x0600043D RID: 1085 RVA: 0x00011074 File Offset: 0x0000F274
		public bool? GuildScheduledEventId { get; set; }

		// Token: 0x0600043E RID: 1086 RVA: 0x00011080 File Offset: 0x0000F280
		internal string ToQueryString()
		{
			QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
			bool flag = this.WithCounts != null;
			if (flag)
			{
				queryStringBuilder.Add("with_counts", this.WithCounts.Value.ToString());
			}
			bool flag2 = this.WithExpiration != null;
			if (flag2)
			{
				queryStringBuilder.Add("with_expiration", this.WithExpiration.Value.ToString());
			}
			bool flag3 = this.GuildScheduledEventId != null;
			if (flag3)
			{
				queryStringBuilder.Add("guild_scheduled_event_id", this.GuildScheduledEventId.Value.ToString());
			}
			return queryStringBuilder.ToString();
		}
	}
}
