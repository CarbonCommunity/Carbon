/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Ext.Discord.Builders;

namespace Oxide.Ext.Discord.Entities.Users
{
	// Token: 0x0200004D RID: 77
	public class UserGuildsRequest
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000276 RID: 630 RVA: 0x0000F1D1 File Offset: 0x0000D3D1
		// (set) Token: 0x06000277 RID: 631 RVA: 0x0000F1D9 File Offset: 0x0000D3D9
		public Snowflake? Before { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000278 RID: 632 RVA: 0x0000F1E2 File Offset: 0x0000D3E2
		// (set) Token: 0x06000279 RID: 633 RVA: 0x0000F1EA File Offset: 0x0000D3EA
		public Snowflake? After { get; set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600027A RID: 634 RVA: 0x0000F1F3 File Offset: 0x0000D3F3
		// (set) Token: 0x0600027B RID: 635 RVA: 0x0000F1FB File Offset: 0x0000D3FB
		public int Limit { get; set; } = 200;

		// Token: 0x0600027C RID: 636 RVA: 0x0000F204 File Offset: 0x0000D404
		public virtual string ToQueryString()
		{
			QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
			queryStringBuilder.Add("limit", this.Limit.ToString());
			bool flag = this.Before != null;
			if (flag)
			{
				queryStringBuilder.Add("before", this.Before.ToString());
			}
			bool flag2 = this.After != null;
			if (flag2)
			{
				queryStringBuilder.Add("after", this.After.ToString());
			}
			return queryStringBuilder.ToString();
		}
	}
}
