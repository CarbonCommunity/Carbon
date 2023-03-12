/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Builders;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000B0 RID: 176
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildPruneGet
	{
		// Token: 0x17000228 RID: 552
		// (get) Token: 0x060006BB RID: 1723 RVA: 0x000146D3 File Offset: 0x000128D3
		// (set) Token: 0x060006BC RID: 1724 RVA: 0x000146DB File Offset: 0x000128DB
		[JsonProperty("days")]
		public int Days { get; set; }

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x060006BD RID: 1725 RVA: 0x000146E4 File Offset: 0x000128E4
		// (set) Token: 0x060006BE RID: 1726 RVA: 0x000146EC File Offset: 0x000128EC
		[JsonProperty("include_roles")]
		public List<Snowflake> IncludeRoles { get; set; }

		// Token: 0x060006BF RID: 1727 RVA: 0x000146F8 File Offset: 0x000128F8
		public virtual string ToQueryString()
		{
			QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
			queryStringBuilder.Add("days", this.Days.ToString());
			bool flag = this.IncludeRoles != null;
			if (flag)
			{
				queryStringBuilder.AddList<Snowflake>("include_roles", this.IncludeRoles, ",");
			}
			return queryStringBuilder.ToString();
		}
	}
}
