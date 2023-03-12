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
	// Token: 0x020000BD RID: 189
	public class ScheduledEventLookup
	{
		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06000728 RID: 1832 RVA: 0x00014E06 File Offset: 0x00013006
		// (set) Token: 0x06000729 RID: 1833 RVA: 0x00014E0E File Offset: 0x0001300E
		public bool? WithUserCount { get; set; }

		// Token: 0x0600072A RID: 1834 RVA: 0x00014E18 File Offset: 0x00013018
		internal string ToQueryString()
		{
			QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
			bool flag = this.WithUserCount != null;
			if (flag)
			{
				queryStringBuilder.Add("with_user_count", this.WithUserCount.Value.ToString());
			}
			return queryStringBuilder.ToString();
		}
	}
}
