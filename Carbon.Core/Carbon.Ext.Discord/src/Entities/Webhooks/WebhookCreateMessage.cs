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
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Entities.Webhooks
{
	// Token: 0x02000043 RID: 67
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class WebhookCreateMessage : MessageCreate
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x0000E4DE File Offset: 0x0000C6DE
		// (set) Token: 0x060001F9 RID: 505 RVA: 0x0000E4E6 File Offset: 0x0000C6E6
		[JsonProperty("username")]
		public string Username { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060001FA RID: 506 RVA: 0x0000E4EF File Offset: 0x0000C6EF
		// (set) Token: 0x060001FB RID: 507 RVA: 0x0000E4F7 File Offset: 0x0000C6F7
		[JsonProperty("avatar_url")]
		public string AvatarUrl { get; set; }

		// Token: 0x060001FC RID: 508 RVA: 0x0000E500 File Offset: 0x0000C700
		internal void ValidateWebhookMessage()
		{
			bool flag = base.Flags == null;
			if (!flag)
			{
				bool flag2 = (base.Flags.Value & ~MessageFlags.SuppressEmbeds) > MessageFlags.None;
				if (flag2)
				{
					throw new InvalidMessageException("Invalid Message Flags Used for Webhook Message. Only supported flags are MessageFlags.SuppressEmbeds");
				}
			}
		}
	}
}
