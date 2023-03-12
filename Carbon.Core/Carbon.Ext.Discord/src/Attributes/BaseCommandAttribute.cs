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
	// Token: 0x02000133 RID: 307
	[AttributeUsage(AttributeTargets.Method)]
	public class BaseCommandAttribute : Attribute
	{
		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06000B1E RID: 2846 RVA: 0x0001951D File Offset: 0x0001771D
		public string Name { get; }

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06000B1F RID: 2847 RVA: 0x00019525 File Offset: 0x00017725
		public bool IsLocalized { get; }

		// Token: 0x06000B20 RID: 2848 RVA: 0x0001952D File Offset: 0x0001772D
		protected BaseCommandAttribute(string name, bool isLocalized = false)
		{
			this.Name = name;
			this.IsLocalized = isLocalized;
		}
	}
}
