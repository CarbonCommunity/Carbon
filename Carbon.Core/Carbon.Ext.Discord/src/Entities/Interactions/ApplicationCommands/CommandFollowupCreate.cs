/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Webhooks;

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
	// Token: 0x02000090 RID: 144
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	[Obsolete("Replaced with WebhookCreateMessage. This will be removed in the May 2022 update.")]
	public class CommandFollowupCreate : WebhookCreateMessage
	{
	}
}
