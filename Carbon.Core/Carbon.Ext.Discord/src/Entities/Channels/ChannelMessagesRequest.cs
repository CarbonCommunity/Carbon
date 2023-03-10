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

namespace Oxide.Ext.Discord.Entities.Channels
{
	// Token: 0x020000FA RID: 250
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ChannelMessagesRequest
	{
		// Token: 0x1700030A RID: 778
		// (get) Token: 0x060008CC RID: 2252 RVA: 0x00015E0F File Offset: 0x0001400F
		// (set) Token: 0x060008CD RID: 2253 RVA: 0x00015E17 File Offset: 0x00014017
		public Snowflake? Around { get; set; }

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x060008CE RID: 2254 RVA: 0x00015E20 File Offset: 0x00014020
		// (set) Token: 0x060008CF RID: 2255 RVA: 0x00015E28 File Offset: 0x00014028
		public Snowflake? Before { get; set; }

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060008D0 RID: 2256 RVA: 0x00015E31 File Offset: 0x00014031
		// (set) Token: 0x060008D1 RID: 2257 RVA: 0x00015E39 File Offset: 0x00014039
		public Snowflake? After { get; set; }

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060008D2 RID: 2258 RVA: 0x00015E42 File Offset: 0x00014042
		// (set) Token: 0x060008D3 RID: 2259 RVA: 0x00015E4A File Offset: 0x0001404A
		public int? Limit { get; set; }

		// Token: 0x060008D4 RID: 2260 RVA: 0x00015E54 File Offset: 0x00014054
		public string ToQueryString()
		{
			QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
			bool flag = this.Around != null;
			if (flag)
			{
				queryStringBuilder.Add("around", this.Around.Value.ToString());
			}
			else
			{
				bool flag2 = this.Before != null;
				if (flag2)
				{
					queryStringBuilder.Add("before", this.Before.Value.ToString());
				}
				else
				{
					bool flag3 = this.After != null;
					if (flag3)
					{
						queryStringBuilder.Add("after", this.After.Value.ToString());
					}
				}
			}
			bool flag4 = this.Limit != null;
			if (flag4)
			{
				queryStringBuilder.Add("limit", this.Limit.Value.ToString());
			}
			return queryStringBuilder.ToString();
		}
	}
}
