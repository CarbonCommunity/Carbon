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
using Oxide.Ext.Discord.Entities.Messages.AllowedMentions;
using Oxide.Ext.Discord.Entities.Messages.Embeds;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Helpers;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Messages
{
	// Token: 0x02000064 RID: 100
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class MessageCreate : IFileAttachments
	{
		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000381 RID: 897 RVA: 0x000107DD File Offset: 0x0000E9DD
		// (set) Token: 0x06000382 RID: 898 RVA: 0x000107E5 File Offset: 0x0000E9E5
		[JsonProperty("content")]
		public string Content { get; set; }

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000383 RID: 899 RVA: 0x000107EE File Offset: 0x0000E9EE
		// (set) Token: 0x06000384 RID: 900 RVA: 0x000107F6 File Offset: 0x0000E9F6
		[JsonProperty("nonce")]
		public string Nonce { get; set; }

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000385 RID: 901 RVA: 0x000107FF File Offset: 0x0000E9FF
		// (set) Token: 0x06000386 RID: 902 RVA: 0x00010807 File Offset: 0x0000EA07
		[JsonProperty("tts")]
		public bool? Tts { get; set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000387 RID: 903 RVA: 0x00010810 File Offset: 0x0000EA10
		// (set) Token: 0x06000388 RID: 904 RVA: 0x00010818 File Offset: 0x0000EA18
		[JsonProperty("embeds")]
		public List<DiscordEmbed> Embeds { get; set; }

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000389 RID: 905 RVA: 0x00010821 File Offset: 0x0000EA21
		// (set) Token: 0x0600038A RID: 906 RVA: 0x00010829 File Offset: 0x0000EA29
		[JsonProperty("allowed_mentions")]
		public AllowedMention AllowedMention { get; set; }

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x0600038B RID: 907 RVA: 0x00010832 File Offset: 0x0000EA32
		// (set) Token: 0x0600038C RID: 908 RVA: 0x0001083A File Offset: 0x0000EA3A
		[JsonProperty("message_reference")]
		public MessageReference MessageReference { get; set; }

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x0600038D RID: 909 RVA: 0x00010843 File Offset: 0x0000EA43
		// (set) Token: 0x0600038E RID: 910 RVA: 0x0001084B File Offset: 0x0000EA4B
		[JsonProperty("components")]
		public List<ActionRowComponent> Components { get; set; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x0600038F RID: 911 RVA: 0x00010854 File Offset: 0x0000EA54
		// (set) Token: 0x06000390 RID: 912 RVA: 0x0001085C File Offset: 0x0000EA5C
		[JsonProperty("sticker_ids")]
		public List<Snowflake> StickerIds { get; set; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000391 RID: 913 RVA: 0x00010865 File Offset: 0x0000EA65
		// (set) Token: 0x06000392 RID: 914 RVA: 0x0001086D File Offset: 0x0000EA6D
		[JsonProperty("attachments")]
		public List<MessageAttachment> Attachments { get; set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000393 RID: 915 RVA: 0x00010876 File Offset: 0x0000EA76
		// (set) Token: 0x06000394 RID: 916 RVA: 0x0001087E File Offset: 0x0000EA7E
		[JsonProperty("flags ")]
		public MessageFlags? Flags { get; set; }

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000395 RID: 917 RVA: 0x00010887 File Offset: 0x0000EA87
		// (set) Token: 0x06000396 RID: 918 RVA: 0x0001088F File Offset: 0x0000EA8F
		public List<MessageFileAttachment> FileAttachments { get; set; }

		// Token: 0x06000397 RID: 919 RVA: 0x00010898 File Offset: 0x0000EA98
		public void AddAttachment(string filename, byte[] data, string contentType, string description = null)
		{
			Validation.ValidateFilename(filename);
			bool flag = this.FileAttachments == null;
			if (flag)
			{
				this.FileAttachments = new List<MessageFileAttachment>();
			}
			bool flag2 = this.Attachments == null;
			if (flag2)
			{
				this.Attachments = new List<MessageAttachment>();
			}
			this.FileAttachments.Add(new MessageFileAttachment(filename, data, contentType));
			this.Attachments.Add(new MessageAttachment
			{
				Id = new Snowflake((ulong)((long)this.FileAttachments.Count)),
				Filename = filename,
				Description = description
			});
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00010934 File Offset: 0x0000EB34
		internal void Validate()
		{
			bool flag = string.IsNullOrEmpty(this.Content) && (this.Embeds == null || this.Embeds.Count == 0) && (this.FileAttachments == null || this.FileAttachments.Count == 0);
			if (flag)
			{
				throw new InvalidMessageException("Discord Messages require Either Content, An Embed, Or a File");
			}
			bool flag2 = !string.IsNullOrEmpty(this.Content) && this.Content.Length > 2000;
			if (flag2)
			{
				throw new InvalidMessageException("Content cannot be more than 2000 characters");
			}
		}

		// Token: 0x06000399 RID: 921 RVA: 0x000109C4 File Offset: 0x0000EBC4
		internal void ValidateChannelMessage()
		{
			bool flag = this.Flags == null;
			if (!flag)
			{
				bool flag2 = (this.Flags.Value & ~MessageFlags.SuppressEmbeds) > MessageFlags.None;
				if (flag2)
				{
					throw new InvalidMessageException("Invalid Message Flags Used for Channel Message. Only supported flags are MessageFlags.SuppressEmbeds");
				}
			}
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00010A10 File Offset: 0x0000EC10
		internal void ValidateInteractionMessage()
		{
			bool flag = this.Flags == null;
			if (!flag)
			{
				bool flag2 = (this.Flags.Value & ~(MessageFlags.SuppressEmbeds | MessageFlags.Ephemeral)) > MessageFlags.None;
				if (flag2)
				{
					throw new InvalidMessageException("Invalid Message Flags Used for Interaction Message. Only supported flags are MessageFlags.SuppressEmbeds, and MessageFlags.Ephemeral");
				}
			}
		}
	}
}
