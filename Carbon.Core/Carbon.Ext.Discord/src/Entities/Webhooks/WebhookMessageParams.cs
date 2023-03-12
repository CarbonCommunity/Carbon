/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Ext.Discord.Builders;

namespace Oxide.Ext.Discord.Entities.Webhooks
{
	// Token: 0x02000046 RID: 70
	public class WebhookMessageParams
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600020F RID: 527 RVA: 0x0000E6E7 File Offset: 0x0000C8E7
		// (set) Token: 0x06000210 RID: 528 RVA: 0x0000E6EF File Offset: 0x0000C8EF
		public Snowflake? ThreadId { get; set; }

		// Token: 0x06000211 RID: 529 RVA: 0x0000E6F8 File Offset: 0x0000C8F8
		public string ToQueryString()
		{
			QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
			bool flag = this.ThreadId != null;
			if (flag)
			{
				queryStringBuilder.Add("thread_id", this.ThreadId.Value.ToString());
			}
			return queryStringBuilder.ToString();
		}
	}
}
