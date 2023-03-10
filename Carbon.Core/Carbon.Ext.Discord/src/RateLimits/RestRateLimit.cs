/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Ext.Discord.Helpers;

namespace Oxide.Ext.Discord.RateLimits
{
	// Token: 0x02000015 RID: 21
	public class RestRateLimit : BaseRateLimit
	{
		// Token: 0x060000F5 RID: 245 RVA: 0x0000A789 File Offset: 0x00008989
		public RestRateLimit() : base(110, 60.0)
		{
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000A79E File Offset: 0x0000899E
		public void ReachedRateLimit(double retryAfter)
		{
			this.NumRequests = this.MaxRequests;
			this._retryAfter = retryAfter;
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x0000A7B4 File Offset: 0x000089B4
		public override double NextReset
		{
			get
			{
				return (double)Time.TimeSinceEpoch() - Math.Max(this.LastReset + this.ResetInterval, (double)Time.TimeSinceEpoch() + this._retryAfter);
			}
		}

		// Token: 0x040000CE RID: 206
		private double _retryAfter;
	}
}
