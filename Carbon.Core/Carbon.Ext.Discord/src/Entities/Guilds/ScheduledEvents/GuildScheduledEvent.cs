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
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
	// Token: 0x020000B9 RID: 185
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildScheduledEvent : ISnowflakeEntity
	{
		// Token: 0x1700023D RID: 573
		// (get) Token: 0x060006ED RID: 1773 RVA: 0x00014899 File Offset: 0x00012A99
		// (set) Token: 0x060006EE RID: 1774 RVA: 0x000148A1 File Offset: 0x00012AA1
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x060006EF RID: 1775 RVA: 0x000148AA File Offset: 0x00012AAA
		// (set) Token: 0x060006F0 RID: 1776 RVA: 0x000148B2 File Offset: 0x00012AB2
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x060006F1 RID: 1777 RVA: 0x000148BB File Offset: 0x00012ABB
		// (set) Token: 0x060006F2 RID: 1778 RVA: 0x000148C3 File Offset: 0x00012AC3
		[JsonProperty("channel_id")]
		public Snowflake? ChannelId { get; set; }

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x060006F3 RID: 1779 RVA: 0x000148CC File Offset: 0x00012ACC
		// (set) Token: 0x060006F4 RID: 1780 RVA: 0x000148D4 File Offset: 0x00012AD4
		[JsonProperty("creator_id")]
		public Snowflake? CreatorId { get; set; }

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x060006F5 RID: 1781 RVA: 0x000148DD File Offset: 0x00012ADD
		// (set) Token: 0x060006F6 RID: 1782 RVA: 0x000148E5 File Offset: 0x00012AE5
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x060006F7 RID: 1783 RVA: 0x000148EE File Offset: 0x00012AEE
		// (set) Token: 0x060006F8 RID: 1784 RVA: 0x000148F6 File Offset: 0x00012AF6
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x060006F9 RID: 1785 RVA: 0x000148FF File Offset: 0x00012AFF
		// (set) Token: 0x060006FA RID: 1786 RVA: 0x00014907 File Offset: 0x00012B07
		[JsonProperty("scheduled_start_time")]
		public DateTime ScheduledStartTime { get; set; }

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x060006FB RID: 1787 RVA: 0x00014910 File Offset: 0x00012B10
		// (set) Token: 0x060006FC RID: 1788 RVA: 0x00014918 File Offset: 0x00012B18
		[JsonProperty("scheduled_end_time ")]
		public DateTime? ScheduledEndTime { get; set; }

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x060006FD RID: 1789 RVA: 0x00014921 File Offset: 0x00012B21
		// (set) Token: 0x060006FE RID: 1790 RVA: 0x00014929 File Offset: 0x00012B29
		[JsonProperty("privacy_level")]
		public ScheduledEventPrivacyLevel PrivacyLevel { get; set; }

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x060006FF RID: 1791 RVA: 0x00014932 File Offset: 0x00012B32
		// (set) Token: 0x06000700 RID: 1792 RVA: 0x0001493A File Offset: 0x00012B3A
		[JsonProperty("status")]
		public ScheduledEventStatus Status { get; set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000701 RID: 1793 RVA: 0x00014943 File Offset: 0x00012B43
		// (set) Token: 0x06000702 RID: 1794 RVA: 0x0001494B File Offset: 0x00012B4B
		[JsonProperty("entity_type")]
		public ScheduledEventEntityType EntityType { get; set; }

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000703 RID: 1795 RVA: 0x00014954 File Offset: 0x00012B54
		// (set) Token: 0x06000704 RID: 1796 RVA: 0x0001495C File Offset: 0x00012B5C
		[JsonProperty("entity_id")]
		public Snowflake? EntityId { get; set; }

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000705 RID: 1797 RVA: 0x00014965 File Offset: 0x00012B65
		// (set) Token: 0x06000706 RID: 1798 RVA: 0x0001496D File Offset: 0x00012B6D
		[JsonProperty("entity_metadata")]
		public ScheduledEventEntityMetadata EntityMetadata { get; set; }

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000707 RID: 1799 RVA: 0x00014976 File Offset: 0x00012B76
		// (set) Token: 0x06000708 RID: 1800 RVA: 0x0001497E File Offset: 0x00012B7E
		[JsonProperty("creator")]
		public DiscordUser Creator { get; set; }

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000709 RID: 1801 RVA: 0x00014987 File Offset: 0x00012B87
		// (set) Token: 0x0600070A RID: 1802 RVA: 0x0001498F File Offset: 0x00012B8F
		[JsonProperty("user_count")]
		public int? UserCount { get; set; }

		// Token: 0x0600070B RID: 1803 RVA: 0x00014998 File Offset: 0x00012B98
		public static void ListForGuild(DiscordClient client, Snowflake guildId, ScheduledEventLookup lookup = null, Action<List<GuildScheduledEvent>> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			client.Bot.Rest.DoRequest<List<GuildScheduledEvent>>(string.Format("/guilds/{0}/scheduled-events{1}", guildId, (lookup != null) ? lookup.ToQueryString() : null), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x000149F4 File Offset: 0x00012BF4
		public static void Create(DiscordClient client, Snowflake guildId, ScheduledEventCreate create, Action<GuildScheduledEvent> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			client.Bot.Rest.DoRequest<GuildScheduledEvent>(string.Format("/guilds/{0}/scheduled-events", guildId), RequestMethod.POST, create, callback, error);
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x00014A44 File Offset: 0x00012C44
		public static void Get(DiscordClient client, Snowflake guildId, Snowflake eventId, ScheduledEventLookup lookup = null, Action<GuildScheduledEvent> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			bool flag2 = !eventId.IsValid();
			if (flag2)
			{
				throw new InvalidSnowflakeException("eventId");
			}
			client.Bot.Rest.DoRequest<GuildScheduledEvent>(string.Format("/guilds/{0}/scheduled-events/{1}{2}", guildId, eventId, (lookup != null) ? lookup.ToQueryString() : null), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x00014AC0 File Offset: 0x00012CC0
		public void Modify(DiscordClient client, Snowflake guildId, Snowflake eventId, ScheduledEventUpdate update, Action<GuildScheduledEvent> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			bool flag2 = !eventId.IsValid();
			if (flag2)
			{
				throw new InvalidSnowflakeException("eventId");
			}
			client.Bot.Rest.DoRequest<GuildScheduledEvent>(string.Format("/guilds/{0}/scheduled-events/{1}", guildId, eventId), RequestMethod.PATCH, update, callback, error);
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x00014B30 File Offset: 0x00012D30
		public void Delete(DiscordClient client, Snowflake guildId, Snowflake eventId, Action<GuildScheduledEvent> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			bool flag2 = !eventId.IsValid();
			if (flag2)
			{
				throw new InvalidSnowflakeException("eventId");
			}
			client.Bot.Rest.DoRequest<GuildScheduledEvent>(string.Format("/guilds/{0}/scheduled-events/{1}", guildId, eventId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x00014BA0 File Offset: 0x00012DA0
		public static void GetUsers(DiscordClient client, Snowflake guildId, Snowflake eventId, ScheduledEventUsersLookup lookup = null, Action<List<ScheduledEventUser>> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			bool flag2 = !eventId.IsValid();
			if (flag2)
			{
				throw new InvalidSnowflakeException("eventId");
			}
			bool flag3 = lookup != null && lookup.Limit != null && lookup.Limit.Value > 100;
			if (flag3)
			{
				throw new Exception("GuildScheduledEvent.GetUsers Validation Error: ScheduledEventUsersLookup.Limit cannot be greater than 100");
			}
			client.Bot.Rest.DoRequest<List<ScheduledEventUser>>(string.Format("/guilds/{0}/scheduled-events/{1}{2}", guildId, eventId, (lookup != null) ? lookup.ToQueryString() : null), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x00014C54 File Offset: 0x00012E54
		internal void Update(GuildScheduledEvent scheduledEvent)
		{
			bool flag = scheduledEvent.ChannelId != null;
			if (flag)
			{
				this.ChannelId = scheduledEvent.ChannelId;
			}
			bool flag2 = scheduledEvent.EntityMetadata != null;
			if (flag2)
			{
				bool flag3 = this.EntityMetadata == null;
				if (flag3)
				{
					this.EntityMetadata = scheduledEvent.EntityMetadata;
				}
				else
				{
					this.EntityMetadata.Update(scheduledEvent.EntityMetadata);
				}
			}
			bool flag4 = scheduledEvent.Name != null;
			if (flag4)
			{
				this.Name = scheduledEvent.Name;
			}
			bool flag5 = scheduledEvent.Description != null;
			if (flag5)
			{
				this.Description = scheduledEvent.Description;
			}
			this.PrivacyLevel = scheduledEvent.PrivacyLevel;
			this.EntityType = scheduledEvent.EntityType;
			this.Status = scheduledEvent.Status;
			this.ScheduledStartTime = scheduledEvent.ScheduledStartTime;
			this.ScheduledEndTime = scheduledEvent.ScheduledEndTime;
		}
	}
}
