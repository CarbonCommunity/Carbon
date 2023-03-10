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
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Applications;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Channels.Threads;
using Oxide.Ext.Discord.Entities.Emojis;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Interactions.MessageComponents;
using Oxide.Ext.Discord.Entities.Messages.Embeds;
using Oxide.Ext.Discord.Entities.Stickers;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Helpers;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Ext.Discord.Interfaces;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Messages
{
	// Token: 0x02000060 RID: 96
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordMessage : IFileAttachments
	{
		// Token: 0x170000AD RID: 173
		// (get) Token: 0x0600030D RID: 781 RVA: 0x0000FE80 File Offset: 0x0000E080
		// (set) Token: 0x0600030E RID: 782 RVA: 0x0000FE88 File Offset: 0x0000E088
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600030F RID: 783 RVA: 0x0000FE91 File Offset: 0x0000E091
		// (set) Token: 0x06000310 RID: 784 RVA: 0x0000FE99 File Offset: 0x0000E099
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000311 RID: 785 RVA: 0x0000FEA2 File Offset: 0x0000E0A2
		// (set) Token: 0x06000312 RID: 786 RVA: 0x0000FEAA File Offset: 0x0000E0AA
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000313 RID: 787 RVA: 0x0000FEB3 File Offset: 0x0000E0B3
		// (set) Token: 0x06000314 RID: 788 RVA: 0x0000FEBB File Offset: 0x0000E0BB
		[JsonProperty("author")]
		public DiscordUser Author { get; set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000315 RID: 789 RVA: 0x0000FEC4 File Offset: 0x0000E0C4
		// (set) Token: 0x06000316 RID: 790 RVA: 0x0000FECC File Offset: 0x0000E0CC
		[JsonProperty("member")]
		public GuildMember Member { get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000317 RID: 791 RVA: 0x0000FED5 File Offset: 0x0000E0D5
		// (set) Token: 0x06000318 RID: 792 RVA: 0x0000FEDD File Offset: 0x0000E0DD
		[JsonProperty("content")]
		public string Content { get; set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000319 RID: 793 RVA: 0x0000FEE6 File Offset: 0x0000E0E6
		// (set) Token: 0x0600031A RID: 794 RVA: 0x0000FEEE File Offset: 0x0000E0EE
		[JsonProperty("timestamp")]
		public DateTime Timestamp { get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x0600031B RID: 795 RVA: 0x0000FEF7 File Offset: 0x0000E0F7
		// (set) Token: 0x0600031C RID: 796 RVA: 0x0000FEFF File Offset: 0x0000E0FF
		[JsonProperty("edited_timestamp")]
		public DateTime? EditedTimestamp { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600031D RID: 797 RVA: 0x0000FF08 File Offset: 0x0000E108
		// (set) Token: 0x0600031E RID: 798 RVA: 0x0000FF10 File Offset: 0x0000E110
		[JsonProperty("tts")]
		public bool Tts { get; set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x0600031F RID: 799 RVA: 0x0000FF19 File Offset: 0x0000E119
		// (set) Token: 0x06000320 RID: 800 RVA: 0x0000FF21 File Offset: 0x0000E121
		[JsonProperty("mention_everyone")]
		public bool MentionEveryone { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000321 RID: 801 RVA: 0x0000FF2A File Offset: 0x0000E12A
		// (set) Token: 0x06000322 RID: 802 RVA: 0x0000FF32 File Offset: 0x0000E132
		[JsonConverter(typeof(HashListConverter<DiscordUser>))]
		[JsonProperty("mentions")]
		public Hash<Snowflake, DiscordUser> Mentions { get; set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000323 RID: 803 RVA: 0x0000FF3B File Offset: 0x0000E13B
		// (set) Token: 0x06000324 RID: 804 RVA: 0x0000FF43 File Offset: 0x0000E143
		[JsonProperty("mention_roles")]
		public List<Snowflake> MentionRoles { get; set; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000325 RID: 805 RVA: 0x0000FF4C File Offset: 0x0000E14C
		// (set) Token: 0x06000326 RID: 806 RVA: 0x0000FF54 File Offset: 0x0000E154
		[JsonConverter(typeof(HashListConverter<ChannelMention>))]
		[JsonProperty("mention_channels")]
		public Hash<Snowflake, ChannelMention> MentionsChannels { get; set; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000327 RID: 807 RVA: 0x0000FF5D File Offset: 0x0000E15D
		// (set) Token: 0x06000328 RID: 808 RVA: 0x0000FF65 File Offset: 0x0000E165
		[JsonConverter(typeof(HashListConverter<MessageAttachment>))]
		[JsonProperty("attachments")]
		public Hash<Snowflake, MessageAttachment> Attachments { get; set; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000329 RID: 809 RVA: 0x0000FF6E File Offset: 0x0000E16E
		// (set) Token: 0x0600032A RID: 810 RVA: 0x0000FF76 File Offset: 0x0000E176
		[JsonProperty("embeds")]
		public List<DiscordEmbed> Embeds { get; set; }

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x0600032B RID: 811 RVA: 0x0000FF7F File Offset: 0x0000E17F
		// (set) Token: 0x0600032C RID: 812 RVA: 0x0000FF87 File Offset: 0x0000E187
		[JsonProperty("reactions")]
		public List<MessageReaction> Reactions { get; set; }

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x0600032D RID: 813 RVA: 0x0000FF90 File Offset: 0x0000E190
		// (set) Token: 0x0600032E RID: 814 RVA: 0x0000FF98 File Offset: 0x0000E198
		[JsonProperty("nonce")]
		public string Nonce { get; set; }

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x0600032F RID: 815 RVA: 0x0000FFA1 File Offset: 0x0000E1A1
		// (set) Token: 0x06000330 RID: 816 RVA: 0x0000FFA9 File Offset: 0x0000E1A9
		[JsonProperty("pinned")]
		public bool Pinned { get; set; }

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000331 RID: 817 RVA: 0x0000FFB2 File Offset: 0x0000E1B2
		// (set) Token: 0x06000332 RID: 818 RVA: 0x0000FFBA File Offset: 0x0000E1BA
		[JsonProperty("webhook_id")]
		public Snowflake? WebhookId { get; set; }

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000333 RID: 819 RVA: 0x0000FFC3 File Offset: 0x0000E1C3
		// (set) Token: 0x06000334 RID: 820 RVA: 0x0000FFCB File Offset: 0x0000E1CB
		[JsonProperty("type")]
		public MessageType? Type { get; set; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000335 RID: 821 RVA: 0x0000FFD4 File Offset: 0x0000E1D4
		// (set) Token: 0x06000336 RID: 822 RVA: 0x0000FFDC File Offset: 0x0000E1DC
		[JsonProperty("activity")]
		public MessageActivity Activity { get; set; }

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000337 RID: 823 RVA: 0x0000FFE5 File Offset: 0x0000E1E5
		// (set) Token: 0x06000338 RID: 824 RVA: 0x0000FFED File Offset: 0x0000E1ED
		[JsonProperty("application")]
		public DiscordApplication Application { get; set; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000339 RID: 825 RVA: 0x0000FFF6 File Offset: 0x0000E1F6
		// (set) Token: 0x0600033A RID: 826 RVA: 0x0000FFFE File Offset: 0x0000E1FE
		[JsonProperty("application_id")]
		public Snowflake? ApplicationId { get; set; }

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x0600033B RID: 827 RVA: 0x00010007 File Offset: 0x0000E207
		// (set) Token: 0x0600033C RID: 828 RVA: 0x0001000F File Offset: 0x0000E20F
		[JsonProperty("message_reference")]
		public MessageReference MessageReference { get; set; }

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600033D RID: 829 RVA: 0x00010018 File Offset: 0x0000E218
		// (set) Token: 0x0600033E RID: 830 RVA: 0x00010020 File Offset: 0x0000E220
		[JsonProperty("flags")]
		public MessageFlags Flags { get; set; }

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x0600033F RID: 831 RVA: 0x00010029 File Offset: 0x0000E229
		// (set) Token: 0x06000340 RID: 832 RVA: 0x00010031 File Offset: 0x0000E231
		[JsonProperty("referenced_message")]
		public DiscordMessage ReferencedMessage { get; internal set; }

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000341 RID: 833 RVA: 0x0001003A File Offset: 0x0000E23A
		// (set) Token: 0x06000342 RID: 834 RVA: 0x00010042 File Offset: 0x0000E242
		[JsonProperty("interaction")]
		public MessageInteraction Interaction { get; set; }

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000343 RID: 835 RVA: 0x0001004B File Offset: 0x0000E24B
		// (set) Token: 0x06000344 RID: 836 RVA: 0x00010053 File Offset: 0x0000E253
		[JsonProperty("thread")]
		public DiscordChannel Thread { get; set; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000345 RID: 837 RVA: 0x0001005C File Offset: 0x0000E25C
		// (set) Token: 0x06000346 RID: 838 RVA: 0x00010064 File Offset: 0x0000E264
		[JsonProperty("components")]
		public List<ActionRowComponent> Components { get; set; }

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000347 RID: 839 RVA: 0x0001006D File Offset: 0x0000E26D
		// (set) Token: 0x06000348 RID: 840 RVA: 0x00010075 File Offset: 0x0000E275
		[JsonConverter(typeof(HashListConverter<DiscordSticker>))]
		[JsonProperty("sticker_items")]
		public Hash<Snowflake, DiscordSticker> StickerItems { get; set; }

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000349 RID: 841 RVA: 0x0001007E File Offset: 0x0000E27E
		// (set) Token: 0x0600034A RID: 842 RVA: 0x00010086 File Offset: 0x0000E286
		public List<MessageFileAttachment> FileAttachments { get; set; }

		// Token: 0x0600034B RID: 843 RVA: 0x00010090 File Offset: 0x0000E290
		public static void CreateMessage(DiscordClient client, Snowflake channelId, MessageCreate message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			message.Validate();
			message.ValidateChannelMessage();
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/channels/{0}/messages", channelId), RequestMethod.POST, message, callback, error);
		}

		// Token: 0x0600034C RID: 844 RVA: 0x000100EC File Offset: 0x0000E2EC
		public static void CreateMessage(DiscordClient client, Snowflake channelId, string message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			MessageCreate message2 = new MessageCreate
			{
				Content = message
			};
			DiscordMessage.CreateMessage(client, channelId, message2, callback, error);
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00010130 File Offset: 0x0000E330
		public static void CreateMessage(DiscordClient client, Snowflake channelId, DiscordEmbed embed, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			MessageCreate message = new MessageCreate
			{
				Embeds = new List<DiscordEmbed>
				{
					embed
				}
			};
			DiscordMessage.CreateMessage(client, channelId, message, callback, error);
		}

		// Token: 0x0600034E RID: 846 RVA: 0x00010180 File Offset: 0x0000E380
		public static void CreateMessage(DiscordClient client, Snowflake channelId, List<DiscordEmbed> embeds, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			MessageCreate message = new MessageCreate
			{
				Embeds = embeds
			};
			DiscordMessage.CreateMessage(client, channelId, message, callback, error);
		}

		// Token: 0x0600034F RID: 847 RVA: 0x000101C4 File Offset: 0x0000E3C4
		public static void GetChannelMessage(DiscordClient client, Snowflake channelId, Snowflake messageId, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			bool flag2 = !messageId.IsValid();
			if (flag2)
			{
				throw new InvalidSnowflakeException("messageId");
			}
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/channels/{0}/messages/{1}", channelId, messageId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000350 RID: 848 RVA: 0x00010230 File Offset: 0x0000E430
		public void Reply(DiscordClient client, MessageCreate message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			bool flag = message.MessageReference == null;
			if (flag)
			{
				message.MessageReference = new MessageReference
				{
					MessageId = this.Id,
					GuildId = this.GuildId
				};
			}
			message.Validate();
			message.ValidateChannelMessage();
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/channels/{0}/messages", this.ChannelId), RequestMethod.POST, message, callback, error);
		}

		// Token: 0x06000351 RID: 849 RVA: 0x000102AC File Offset: 0x0000E4AC
		public void Reply(DiscordClient client, string message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			MessageCreate message2 = new MessageCreate
			{
				Content = message
			};
			this.Reply(client, message2, callback, error);
		}

		// Token: 0x06000352 RID: 850 RVA: 0x000102D4 File Offset: 0x0000E4D4
		public void Reply(DiscordClient client, DiscordEmbed embed, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			this.Reply(client, new List<DiscordEmbed>
			{
				embed
			}, callback, error);
		}

		// Token: 0x06000353 RID: 851 RVA: 0x000102F0 File Offset: 0x0000E4F0
		public void Reply(DiscordClient client, List<DiscordEmbed> embeds, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			MessageCreate message = new MessageCreate
			{
				Embeds = embeds
			};
			this.Reply(client, message, callback, error);
		}

		// Token: 0x06000354 RID: 852 RVA: 0x00010318 File Offset: 0x0000E518
		public void CrossPostMessage(DiscordClient client, Snowflake messageId, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			bool flag = !messageId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("messageId");
			}
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/channels/{0}/messages/{1}/crosspost", this.Id, messageId), RequestMethod.POST, null, callback, error);
		}

		// Token: 0x06000355 RID: 853 RVA: 0x00010370 File Offset: 0x0000E570
		public void CrossPostMessage(DiscordClient client, DiscordMessage message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			this.CrossPostMessage(client, message.Id, callback, error);
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00010384 File Offset: 0x0000E584
		public void CreateReaction(DiscordClient client, DiscordEmoji emoji, Action callback = null, Action<RestError> error = null)
		{
			this.CreateReaction(client, emoji.ToDataString(), callback, error);
		}

		// Token: 0x06000357 RID: 855 RVA: 0x00010398 File Offset: 0x0000E598
		public void CreateReaction(DiscordClient client, string emoji, Action callback = null, Action<RestError> error = null)
		{
			Validation.ValidateEmoji(emoji);
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/messages/{1}/reactions/{2}/@me", this.ChannelId, this.Id, emoji), RequestMethod.PUT, null, callback, error);
		}

		// Token: 0x06000358 RID: 856 RVA: 0x000103E4 File Offset: 0x0000E5E4
		public void DeleteOwnReaction(DiscordClient client, DiscordEmoji emoji, Action callback = null, Action<RestError> error = null)
		{
			this.DeleteOwnReaction(client, emoji.ToDataString(), callback, error);
		}

		// Token: 0x06000359 RID: 857 RVA: 0x000103F8 File Offset: 0x0000E5F8
		public void DeleteOwnReaction(DiscordClient client, string emoji, Action callback = null, Action<RestError> error = null)
		{
			Validation.ValidateEmoji(emoji);
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/messages/{1}/reactions/{2}/@me", this.ChannelId, this.Id, emoji), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x0600035A RID: 858 RVA: 0x00010444 File Offset: 0x0000E644
		public void DeleteUserReaction(DiscordClient client, DiscordEmoji emoji, Snowflake userId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			this.DeleteUserReaction(client, emoji.ToDataString(), userId, callback, error);
		}

		// Token: 0x0600035B RID: 859 RVA: 0x00010480 File Offset: 0x0000E680
		public void DeleteUserReaction(DiscordClient client, string emoji, Snowflake userId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			Validation.ValidateEmoji(emoji);
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/messages/{1}/reactions/{2}/{3}", new object[]
			{
				this.ChannelId,
				this.Id,
				emoji,
				userId
			}), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x0600035C RID: 860 RVA: 0x000104FE File Offset: 0x0000E6FE
		public void GetReactions(DiscordClient client, DiscordEmoji emoji, Action<List<DiscordUser>> callback = null, Action<RestError> error = null)
		{
			this.GetReactions(client, emoji.ToDataString(), callback, error);
		}

		// Token: 0x0600035D RID: 861 RVA: 0x00010514 File Offset: 0x0000E714
		public void GetReactions(DiscordClient client, string emoji, Action<List<DiscordUser>> callback = null, Action<RestError> error = null)
		{
			Validation.ValidateEmoji(emoji);
			client.Bot.Rest.DoRequest<List<DiscordUser>>(string.Format("/channels/{0}/messages/{1}/reactions/{2}", this.ChannelId, this.Id, emoji), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600035E RID: 862 RVA: 0x00010560 File Offset: 0x0000E760
		public void DeleteAllReactions(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/messages/{1}/reactions", this.ChannelId, this.Id), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x0600035F RID: 863 RVA: 0x00010598 File Offset: 0x0000E798
		public void DeleteAllReactionsForEmoji(DiscordClient client, DiscordEmoji emoji, Action callback = null, Action<RestError> error = null)
		{
			this.DeleteAllReactionsForEmoji(client, emoji.ToDataString(), callback, error);
		}

		// Token: 0x06000360 RID: 864 RVA: 0x000105AC File Offset: 0x0000E7AC
		public void DeleteAllReactionsForEmoji(DiscordClient client, string emoji, Action callback = null, Action<RestError> error = null)
		{
			Validation.ValidateEmoji(emoji);
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/messages/{1}/reactions/{2}", this.ChannelId, this.Id, emoji), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000361 RID: 865 RVA: 0x000105F8 File Offset: 0x0000E7F8
		public void EditMessage(DiscordClient client, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/channels/{0}/messages/{1}", this.ChannelId, this.Id), RequestMethod.PATCH, this, callback, error);
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00010630 File Offset: 0x0000E830
		public void DeleteMessage(DiscordClient client, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/channels/{0}/messages/{1}", this.ChannelId, this.Id), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00010668 File Offset: 0x0000E868
		public void PinMessage(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/pins/{1}", this.ChannelId, this.Id), RequestMethod.PUT, null, callback, error);
		}

		// Token: 0x06000364 RID: 868 RVA: 0x000106A0 File Offset: 0x0000E8A0
		public void UnpinMessage(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/pins/{1}", this.ChannelId, this.Id), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000365 RID: 869 RVA: 0x000106D8 File Offset: 0x0000E8D8
		public void StartPublicThread(DiscordClient client, ThreadCreate create, Action<DiscordChannel> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordChannel>(string.Format("/channels/{0}/messages/{1}/threads", this.ChannelId, this.Id), RequestMethod.POST, create, callback, error);
		}
	}
}
