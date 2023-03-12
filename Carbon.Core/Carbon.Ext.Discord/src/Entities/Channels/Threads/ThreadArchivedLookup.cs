/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Builders;

namespace Oxide.Ext.Discord.Entities.Channels.Threads
{
	// Token: 0x02000104 RID: 260
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ThreadArchivedLookup
	{
		// Token: 0x17000346 RID: 838
		// (get) Token: 0x06000970 RID: 2416 RVA: 0x00016F87 File Offset: 0x00015187
		// (set) Token: 0x06000971 RID: 2417 RVA: 0x00016F8F File Offset: 0x0001518F
		[JsonProperty("before")]
		public DateTime? Before { get; set; }

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06000972 RID: 2418 RVA: 0x00016F98 File Offset: 0x00015198
		// (set) Token: 0x06000973 RID: 2419 RVA: 0x00016FA0 File Offset: 0x000151A0
		[JsonProperty("limit")]
		public int? Limit { get; set; }

		// Token: 0x06000974 RID: 2420 RVA: 0x00016FAC File Offset: 0x000151AC
		internal string ToQueryString()
		{
			QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
			bool flag = this.Before != null;
			if (flag)
			{
				queryStringBuilder.Add("before", this.Before.Value.ToString("o"));
			}
			bool flag2 = this.Limit != null;
			if (flag2)
			{
				queryStringBuilder.Add("limit", this.Limit.Value.ToString());
			}
			return queryStringBuilder.ToString();
		}
	}
}
