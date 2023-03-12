/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Oxide.Ext.Discord.Entities.Messages;

namespace Oxide.Ext.Discord.Interfaces
{
	// Token: 0x02000022 RID: 34
	public interface IFileAttachments
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600014E RID: 334
		// (set) Token: 0x0600014F RID: 335
		List<MessageFileAttachment> FileAttachments { get; set; }
	}
}
