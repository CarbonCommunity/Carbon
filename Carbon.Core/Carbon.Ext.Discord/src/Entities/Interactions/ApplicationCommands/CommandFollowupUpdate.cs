/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Entities.Webhooks;

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
	// Token: 0x02000091 RID: 145
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class CommandFollowupUpdate : WebhookEditMessage
	{
		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000510 RID: 1296 RVA: 0x00011EAC File Offset: 0x000100AC
		// (set) Token: 0x06000511 RID: 1297 RVA: 0x00011EB4 File Offset: 0x000100B4
		[JsonProperty("flags")]
		public MessageFlags? Flags { get; set; }
	}
}
