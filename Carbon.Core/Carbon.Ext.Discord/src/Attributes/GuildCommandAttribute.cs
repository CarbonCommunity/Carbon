/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Attributes
{
	// Token: 0x02000136 RID: 310
	[AttributeUsage(AttributeTargets.Method)]
	public class GuildCommandAttribute : BaseCommandAttribute
	{
		// Token: 0x06000B23 RID: 2851 RVA: 0x00019545 File Offset: 0x00017745
		public GuildCommandAttribute(string name, bool isLocalized = false) : base(name, isLocalized)
		{
		}
	}
}
