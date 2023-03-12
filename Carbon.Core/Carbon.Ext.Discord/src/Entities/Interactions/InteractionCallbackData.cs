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
using Oxide.Ext.Discord.Entities.Interactions.MessageComponents;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Entities.Messages.AllowedMentions;
using Oxide.Ext.Discord.Entities.Messages.Embeds;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Entities.Interactions
{
	// Token: 0x0200007B RID: 123
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class InteractionCallbackData
	{
		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000476 RID: 1142 RVA: 0x000114B6 File Offset: 0x0000F6B6
		// (set) Token: 0x06000477 RID: 1143 RVA: 0x000114BE File Offset: 0x0000F6BE
		[JsonProperty("tts")]
		public bool? Tts { get; set; }

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000478 RID: 1144 RVA: 0x000114C7 File Offset: 0x0000F6C7
		// (set) Token: 0x06000479 RID: 1145 RVA: 0x000114CF File Offset: 0x0000F6CF
		[JsonProperty("content")]
		public string Content { get; set; }

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x0600047A RID: 1146 RVA: 0x000114D8 File Offset: 0x0000F6D8
		// (set) Token: 0x0600047B RID: 1147 RVA: 0x000114E0 File Offset: 0x0000F6E0
		[JsonProperty("embeds")]
		public List<DiscordEmbed> Embeds { get; set; }

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x0600047C RID: 1148 RVA: 0x000114E9 File Offset: 0x0000F6E9
		// (set) Token: 0x0600047D RID: 1149 RVA: 0x000114F1 File Offset: 0x0000F6F1
		[JsonProperty("allowed_mentions")]
		public AllowedMention AllowedMentions { get; set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x0600047E RID: 1150 RVA: 0x000114FA File Offset: 0x0000F6FA
		// (set) Token: 0x0600047F RID: 1151 RVA: 0x00011502 File Offset: 0x0000F702
		[JsonProperty("custom_id")]
		public string CustomId { get; set; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000480 RID: 1152 RVA: 0x0001150B File Offset: 0x0000F70B
		// (set) Token: 0x06000481 RID: 1153 RVA: 0x00011513 File Offset: 0x0000F713
		[JsonProperty("title")]
		public string Title { get; set; }

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000482 RID: 1154 RVA: 0x0001151C File Offset: 0x0000F71C
		// (set) Token: 0x06000483 RID: 1155 RVA: 0x00011524 File Offset: 0x0000F724
		[JsonProperty("flags")]
		public MessageFlags? Flags { get; set; }

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000484 RID: 1156 RVA: 0x0001152D File Offset: 0x0000F72D
		// (set) Token: 0x06000485 RID: 1157 RVA: 0x00011535 File Offset: 0x0000F735
		[JsonProperty("components")]
		public List<ActionRowComponent> Components { get; set; }

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000486 RID: 1158 RVA: 0x0001153E File Offset: 0x0000F73E
		// (set) Token: 0x06000487 RID: 1159 RVA: 0x00011546 File Offset: 0x0000F746
		[JsonProperty("attachments")]
		public List<MessageAttachment> Attachments { get; set; }

		// Token: 0x06000488 RID: 1160 RVA: 0x00011550 File Offset: 0x0000F750
		internal void Validate()
		{
			bool flag = this.Flags == null;
			if (!flag)
			{
				bool flag2 = (this.Flags.Value & ~(MessageFlags.SuppressEmbeds | MessageFlags.Ephemeral)) > MessageFlags.None;
				if (flag2)
				{
					throw new InvalidInteractionResponseException("Invalid Message Flags Used for Interaction Message. Only supported flags are MessageFlags.SuppressEmbeds or MessageFlags.Ephemeral");
				}
			}
		}
	}
}
