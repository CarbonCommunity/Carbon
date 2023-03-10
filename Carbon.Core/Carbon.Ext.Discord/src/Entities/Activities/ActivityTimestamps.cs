/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Helpers;

namespace Oxide.Ext.Discord.Entities.Activities
{
	// Token: 0x02000123 RID: 291
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ActivityTimestamps
	{
		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06000AAE RID: 2734 RVA: 0x00017EC3 File Offset: 0x000160C3
		// (set) Token: 0x06000AAF RID: 2735 RVA: 0x00017ECB File Offset: 0x000160CB
		[JsonProperty("start")]
		public int Start { get; set; }

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06000AB0 RID: 2736 RVA: 0x00017ED4 File Offset: 0x000160D4
		// (set) Token: 0x06000AB1 RID: 2737 RVA: 0x00017EDC File Offset: 0x000160DC
		[JsonProperty("end")]
		public int End { get; set; }

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06000AB2 RID: 2738 RVA: 0x00017EE5 File Offset: 0x000160E5
		public DateTime StartDateTime
		{
			get
			{
				return this.Start.ToDateTime();
			}
		}

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06000AB3 RID: 2739 RVA: 0x00017EE5 File Offset: 0x000160E5
		public DateTime EndDateTime
		{
			get
			{
				return this.Start.ToDateTime();
			}
		}
	}
}
