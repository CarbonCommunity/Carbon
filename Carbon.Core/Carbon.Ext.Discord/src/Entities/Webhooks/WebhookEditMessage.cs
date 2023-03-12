/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Messages.AllowedMentions;
using Oxide.Ext.Discord.Entities.Messages.Embeds;

namespace Oxide.Ext.Discord.Entities.Webhooks
{
	// Token: 0x02000044 RID: 68
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class WebhookEditMessage
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060001FE RID: 510 RVA: 0x0000E553 File Offset: 0x0000C753
		// (set) Token: 0x060001FF RID: 511 RVA: 0x0000E55B File Offset: 0x0000C75B
		[JsonProperty("content")]
		public string Content { get; set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000200 RID: 512 RVA: 0x0000E564 File Offset: 0x0000C764
		// (set) Token: 0x06000201 RID: 513 RVA: 0x0000E56C File Offset: 0x0000C76C
		[JsonProperty("embeds")]
		public List<DiscordEmbed> Embeds { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000202 RID: 514 RVA: 0x0000E575 File Offset: 0x0000C775
		// (set) Token: 0x06000203 RID: 515 RVA: 0x0000E57D File Offset: 0x0000C77D
		[JsonProperty("allowed_mentions")]
		public AllowedMention AllowedMentions { get; set; }

		// Token: 0x06000204 RID: 516 RVA: 0x0000E588 File Offset: 0x0000C788
		public WebhookEditMessage AddEmbed(DiscordEmbed embed)
		{
			bool flag = this.Embeds.Count >= 10;
			if (flag)
			{
				throw new IndexOutOfRangeException("Only 10 embed are allowed per message");
			}
			this.Embeds.Add(embed);
			return this;
		}
	}
}
