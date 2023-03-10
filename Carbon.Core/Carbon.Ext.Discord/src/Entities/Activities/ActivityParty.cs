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

namespace Oxide.Ext.Discord.Entities.Activities
{
	// Token: 0x02000121 RID: 289
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ActivityParty
	{
		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06000AA0 RID: 2720 RVA: 0x00017E52 File Offset: 0x00016052
		// (set) Token: 0x06000AA1 RID: 2721 RVA: 0x00017E5A File Offset: 0x0001605A
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06000AA2 RID: 2722 RVA: 0x00017E63 File Offset: 0x00016063
		// (set) Token: 0x06000AA3 RID: 2723 RVA: 0x00017E6B File Offset: 0x0001606B
		[JsonProperty("size")]
		public List<int> Size { get; set; }

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06000AA4 RID: 2724 RVA: 0x00017E74 File Offset: 0x00016074
		public int CurrentSize
		{
			get
			{
				return this.Size[0];
			}
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06000AA5 RID: 2725 RVA: 0x00017E82 File Offset: 0x00016082
		public int MaxSize
		{
			get
			{
				return this.Size[1];
			}
		}
	}
}
