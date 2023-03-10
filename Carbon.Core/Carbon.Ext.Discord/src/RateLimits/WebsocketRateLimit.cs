/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.RateLimits
{
	// Token: 0x02000016 RID: 22
	public class WebsocketRateLimit : BaseRateLimit
	{
		// Token: 0x060000F8 RID: 248 RVA: 0x0000A789 File Offset: 0x00008989
		public WebsocketRateLimit() : base(110, 60.0)
		{
		}
	}
}
