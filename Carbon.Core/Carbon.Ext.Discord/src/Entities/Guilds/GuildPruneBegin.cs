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

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000AF RID: 175
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildPruneBegin : GuildPruneGet
	{
		// Token: 0x17000226 RID: 550
		// (get) Token: 0x060006B5 RID: 1717 RVA: 0x00014647 File Offset: 0x00012847
		// (set) Token: 0x060006B6 RID: 1718 RVA: 0x0001464F File Offset: 0x0001284F
		[JsonProperty("compute_prune_count")]
		public bool ComputePruneCount { get; set; }

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x060006B7 RID: 1719 RVA: 0x00014658 File Offset: 0x00012858
		// (set) Token: 0x060006B8 RID: 1720 RVA: 0x00014660 File Offset: 0x00012860
		[JsonProperty("reason")]
		public string Reason { get; set; }

		// Token: 0x060006B9 RID: 1721 RVA: 0x0001466C File Offset: 0x0001286C
		public override string ToQueryString()
		{
			QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
			queryStringBuilder.Add("compute_prune_count", this.ComputePruneCount.ToString());
			bool flag = !string.IsNullOrEmpty(this.Reason);
			if (flag)
			{
				queryStringBuilder.Add("reason", this.Reason);
			}
			return queryStringBuilder.ToString();
		}
	}
}
