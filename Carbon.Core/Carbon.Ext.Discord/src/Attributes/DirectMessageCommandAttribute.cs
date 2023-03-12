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
	// Token: 0x02000134 RID: 308
	[AttributeUsage(AttributeTargets.Method)]
	public class DirectMessageCommandAttribute : BaseCommandAttribute
	{
		// Token: 0x06000B21 RID: 2849 RVA: 0x00019545 File Offset: 0x00017745
		public DirectMessageCommandAttribute(string name, bool isLocalized = false) : base(name, isLocalized)
		{
		}
	}
}
