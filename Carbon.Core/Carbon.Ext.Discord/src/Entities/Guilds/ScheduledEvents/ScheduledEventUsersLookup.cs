/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Ext.Discord.Builders;

namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
	// Token: 0x020000C2 RID: 194
	public class ScheduledEventUsersLookup
	{
		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x00014F3A File Offset: 0x0001313A
		// (set) Token: 0x06000747 RID: 1863 RVA: 0x00014F42 File Offset: 0x00013142
		public int? Limit { get; set; }

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06000748 RID: 1864 RVA: 0x00014F4B File Offset: 0x0001314B
		// (set) Token: 0x06000749 RID: 1865 RVA: 0x00014F53 File Offset: 0x00013153
		public bool? WithMember { get; set; }

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x0600074A RID: 1866 RVA: 0x00014F5C File Offset: 0x0001315C
		// (set) Token: 0x0600074B RID: 1867 RVA: 0x00014F64 File Offset: 0x00013164
		public Snowflake? Before { get; set; }

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x0600074C RID: 1868 RVA: 0x00014F6D File Offset: 0x0001316D
		// (set) Token: 0x0600074D RID: 1869 RVA: 0x00014F75 File Offset: 0x00013175
		public Snowflake? After { get; set; }

		// Token: 0x0600074E RID: 1870 RVA: 0x00014F80 File Offset: 0x00013180
		internal string ToQueryString()
		{
			QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
			bool flag = this.Limit != null;
			if (flag)
			{
				queryStringBuilder.Add("limit", this.Limit.Value.ToString());
			}
			bool flag2 = this.WithMember != null;
			if (flag2)
			{
				queryStringBuilder.Add("with_member", this.WithMember.Value.ToString());
			}
			bool flag3 = this.Before != null;
			if (flag3)
			{
				queryStringBuilder.Add("before", this.Before.Value.ToString());
			}
			bool flag4 = this.After != null;
			if (flag4)
			{
				queryStringBuilder.Add("after", this.After.Value.ToString());
			}
			return queryStringBuilder.ToString();
		}
	}
}
