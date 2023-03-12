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
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Channels.Stages
{
	// Token: 0x0200010A RID: 266
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class StageInstance : ISnowflakeEntity
	{
		// Token: 0x17000359 RID: 857
		// (get) Token: 0x0600099D RID: 2461 RVA: 0x00017184 File Offset: 0x00015384
		// (set) Token: 0x0600099E RID: 2462 RVA: 0x0001718C File Offset: 0x0001538C
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x0600099F RID: 2463 RVA: 0x00017195 File Offset: 0x00015395
		// (set) Token: 0x060009A0 RID: 2464 RVA: 0x0001719D File Offset: 0x0001539D
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x060009A1 RID: 2465 RVA: 0x000171A6 File Offset: 0x000153A6
		// (set) Token: 0x060009A2 RID: 2466 RVA: 0x000171AE File Offset: 0x000153AE
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x060009A3 RID: 2467 RVA: 0x000171B7 File Offset: 0x000153B7
		// (set) Token: 0x060009A4 RID: 2468 RVA: 0x000171BF File Offset: 0x000153BF
		[JsonProperty("privacy_level")]
		public PrivacyLevel PrivacyLevel { get; set; }

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x060009A5 RID: 2469 RVA: 0x000171C8 File Offset: 0x000153C8
		// (set) Token: 0x060009A6 RID: 2470 RVA: 0x000171D0 File Offset: 0x000153D0
		[Obsolete("Deprecated by discord")]
		[JsonProperty("discoverable_disabled")]
		public bool DiscoverableDisabled { get; set; }

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x060009A7 RID: 2471 RVA: 0x000171D9 File Offset: 0x000153D9
		// (set) Token: 0x060009A8 RID: 2472 RVA: 0x000171E1 File Offset: 0x000153E1
		[JsonProperty("topic")]
		public string Topic { get; set; }

		// Token: 0x060009A9 RID: 2473 RVA: 0x000171EC File Offset: 0x000153EC
		public static void CreateStageInstance(DiscordClient client, Snowflake channelId, string topic, PrivacyLevel privacyLevel = PrivacyLevel.GuildOnly, Action<StageInstance> callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["channel_id"] = channelId;
			dictionary["topic"] = topic;
			string key = "privacy_level";
			int num = (int)privacyLevel;
			dictionary[key] = num.ToString();
			Dictionary<string, string> data = dictionary;
			client.Bot.Rest.DoRequest<StageInstance>("/stage-instances", RequestMethod.POST, data, callback, error);
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x0001726C File Offset: 0x0001546C
		public static void GetStageInstance(DiscordClient client, Snowflake channelId, Action<StageInstance> callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			client.Bot.Rest.DoRequest<StageInstance>(string.Format("/stage-instances/{0}", channelId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x000172B8 File Offset: 0x000154B8
		public void ModifyStageInstance(DiscordClient client, string topic = null, PrivacyLevel? privacyLevel = null, Action<StageInstance> callback = null, Action<RestError> error = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			bool flag = !string.IsNullOrEmpty(topic);
			if (flag)
			{
				dictionary["topic"] = topic;
			}
			bool flag2 = privacyLevel != null;
			if (flag2)
			{
				dictionary["privacy_level"] = ((int)privacyLevel.Value).ToString();
			}
			client.Bot.Rest.DoRequest<StageInstance>(string.Format("/stage-instances/{0}", this.ChannelId), RequestMethod.PATCH, dictionary, callback, error);
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x0001733C File Offset: 0x0001553C
		public void DeleteStageInstance(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/stage-instances/{0}", this.ChannelId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x0001736C File Offset: 0x0001556C
		internal StageInstance Update(StageInstance stage)
		{
			StageInstance result = (StageInstance)base.MemberwiseClone();
			bool flag = stage.Topic != null;
			if (flag)
			{
				this.Topic = stage.Topic;
			}
			this.PrivacyLevel = stage.PrivacyLevel;
			return result;
		}
	}
}
