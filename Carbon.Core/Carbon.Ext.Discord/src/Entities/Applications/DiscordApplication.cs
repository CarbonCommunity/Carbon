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
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;
using Oxide.Ext.Discord.Entities.Teams;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Helpers.Cdn;

namespace Oxide.Ext.Discord.Entities.Applications
{
	// Token: 0x02000118 RID: 280
	[JsonObject(MemberSerialization = ( MemberSerialization )1 )]
	public class DiscordApplication
	{
		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06000A46 RID: 2630 RVA: 0x00017880 File Offset: 0x00015A80
		// (set) Token: 0x06000A47 RID: 2631 RVA: 0x00017888 File Offset: 0x00015A88
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06000A48 RID: 2632 RVA: 0x00017891 File Offset: 0x00015A91
		// (set) Token: 0x06000A49 RID: 2633 RVA: 0x00017899 File Offset: 0x00015A99
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06000A4A RID: 2634 RVA: 0x000178A2 File Offset: 0x00015AA2
		// (set) Token: 0x06000A4B RID: 2635 RVA: 0x000178AA File Offset: 0x00015AAA
		[JsonProperty("icon")]
		public string Icon { get; set; }

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06000A4C RID: 2636 RVA: 0x000178B3 File Offset: 0x00015AB3
		// (set) Token: 0x06000A4D RID: 2637 RVA: 0x000178BB File Offset: 0x00015ABB
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06000A4E RID: 2638 RVA: 0x000178C4 File Offset: 0x00015AC4
		// (set) Token: 0x06000A4F RID: 2639 RVA: 0x000178CC File Offset: 0x00015ACC
		[JsonProperty("rpc_origins")]
		public List<string> RpcOrigins { get; set; }

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06000A50 RID: 2640 RVA: 0x000178D5 File Offset: 0x00015AD5
		// (set) Token: 0x06000A51 RID: 2641 RVA: 0x000178DD File Offset: 0x00015ADD
		[JsonProperty("bot_public")]
		public bool BotPublic { get; set; }

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06000A52 RID: 2642 RVA: 0x000178E6 File Offset: 0x00015AE6
		// (set) Token: 0x06000A53 RID: 2643 RVA: 0x000178EE File Offset: 0x00015AEE
		[JsonProperty("bot_require_code_grant")]
		public bool BotRequireCodeGrant { get; set; }

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06000A54 RID: 2644 RVA: 0x000178F7 File Offset: 0x00015AF7
		// (set) Token: 0x06000A55 RID: 2645 RVA: 0x000178FF File Offset: 0x00015AFF
		[JsonProperty("terms_of_service_url")]
		public string TermsOfServiceUrl { get; set; }

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x06000A56 RID: 2646 RVA: 0x00017908 File Offset: 0x00015B08
		// (set) Token: 0x06000A57 RID: 2647 RVA: 0x00017910 File Offset: 0x00015B10
		[JsonProperty("privacy_policy_url")]
		public string PrivacyPolicyUrl { get; set; }

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06000A58 RID: 2648 RVA: 0x00017919 File Offset: 0x00015B19
		// (set) Token: 0x06000A59 RID: 2649 RVA: 0x00017921 File Offset: 0x00015B21
		[JsonProperty("owner")]
		public DiscordUser Owner { get; set; }

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06000A5A RID: 2650 RVA: 0x0001792A File Offset: 0x00015B2A
		// (set) Token: 0x06000A5B RID: 2651 RVA: 0x00017932 File Offset: 0x00015B32
		[JsonProperty("summary")]
		public string Summary { get; set; }

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06000A5C RID: 2652 RVA: 0x0001793B File Offset: 0x00015B3B
		// (set) Token: 0x06000A5D RID: 2653 RVA: 0x00017943 File Offset: 0x00015B43
		[JsonProperty("verify_key")]
		public string Verify { get; set; }

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06000A5E RID: 2654 RVA: 0x0001794C File Offset: 0x00015B4C
		// (set) Token: 0x06000A5F RID: 2655 RVA: 0x00017954 File Offset: 0x00015B54
		[JsonProperty("team")]
		public DiscordTeam Team { get; set; }

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06000A60 RID: 2656 RVA: 0x0001795D File Offset: 0x00015B5D
		// (set) Token: 0x06000A61 RID: 2657 RVA: 0x00017965 File Offset: 0x00015B65
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06000A62 RID: 2658 RVA: 0x0001796E File Offset: 0x00015B6E
		// (set) Token: 0x06000A63 RID: 2659 RVA: 0x00017976 File Offset: 0x00015B76
		[JsonProperty("primary_sku_id")]
		public string PrimarySkuId { get; set; }

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06000A64 RID: 2660 RVA: 0x0001797F File Offset: 0x00015B7F
		// (set) Token: 0x06000A65 RID: 2661 RVA: 0x00017987 File Offset: 0x00015B87
		[JsonProperty("slug")]
		public string Slug { get; set; }

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06000A66 RID: 2662 RVA: 0x00017990 File Offset: 0x00015B90
		// (set) Token: 0x06000A67 RID: 2663 RVA: 0x00017998 File Offset: 0x00015B98
		[JsonProperty("cover_image")]
		public string CoverImage { get; set; }

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06000A68 RID: 2664 RVA: 0x000179A1 File Offset: 0x00015BA1
		// (set) Token: 0x06000A69 RID: 2665 RVA: 0x000179A9 File Offset: 0x00015BA9
		[JsonProperty("flags")]
		public ApplicationFlags? Flags { get; set; }

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06000A6A RID: 2666 RVA: 0x000179B2 File Offset: 0x00015BB2
		public string GetApplicationIconUrl
		{
			get
			{
				return DiscordCdn.GetApplicationIconUrl(this.Id, this.Icon, ImageFormat.Auto);
			}
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x000179C8 File Offset: 0x00015BC8
		public bool HasApplicationFlag(ApplicationFlags flag)
		{
			return this.Flags != null && (this.Flags.Value & flag) == flag;
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x00017A00 File Offset: 0x00015C00
		public void GetGlobalCommands(DiscordClient client, Action<List<DiscordApplicationCommand>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<DiscordApplicationCommand>>(string.Format("/applications/{0}/commands", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x00017A30 File Offset: 0x00015C30
		public void GetGlobalCommand(DiscordClient client, Snowflake commandId, Action<DiscordApplicationCommand> callback = null, Action<RestError> error = null)
		{
			bool flag = !commandId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("commandId");
			}
			client.Bot.Rest.DoRequest<DiscordApplicationCommand>(string.Format("/applications/{0}/commands/{1}", this.Id, commandId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x00017A88 File Offset: 0x00015C88
		public void CreateGlobalCommand(DiscordClient client, CommandCreate create, Action<DiscordApplicationCommand> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordApplicationCommand>(string.Format("/applications/{0}/commands", this.Id), RequestMethod.POST, create, callback, error);
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x00017AB6 File Offset: 0x00015CB6
		public void BulkOverwriteGlobalCommands(DiscordClient client, List<DiscordApplicationCommand> commands, Action<List<DiscordApplicationCommand>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<DiscordApplicationCommand>>(string.Format("/applications/{0}/commands", this.Id), RequestMethod.PUT, commands, callback, error);
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x00017AE4 File Offset: 0x00015CE4
		public void GetGuildCommands(DiscordClient client, Snowflake guildId, Action<List<DiscordApplicationCommand>> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			client.Bot.Rest.DoRequest<List<DiscordApplicationCommand>>(string.Format("/applications/{0}/guilds/{1}/commands", this.Id, guildId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x00017B3C File Offset: 0x00015D3C
		public void GetGuildCommand(DiscordClient client, Snowflake guildId, Snowflake commandId, Action<DiscordApplicationCommand> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			bool flag2 = !commandId.IsValid();
			if (flag2)
			{
				throw new InvalidSnowflakeException("commandId");
			}
			client.Bot.Rest.DoRequest<DiscordApplicationCommand>(string.Format("/applications/{0}/guilds/{1}/commands/{2}", this.Id, guildId, commandId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000A72 RID: 2674 RVA: 0x00017BB4 File Offset: 0x00015DB4
		public void CreateGuildCommand(DiscordClient client, Snowflake guildId, CommandCreate create, Action<DiscordApplicationCommand> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			client.Bot.Rest.DoRequest<DiscordApplicationCommand>(string.Format("/applications/{0}/guilds/{1}/commands", this.Id, guildId), RequestMethod.POST, create, callback, error);
		}

		// Token: 0x06000A73 RID: 2675 RVA: 0x00017C10 File Offset: 0x00015E10
		public void GetGuildCommandPermissions(DiscordClient client, Snowflake guildId, Action<List<GuildCommandPermissions>> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			client.Bot.Rest.DoRequest<List<GuildCommandPermissions>>(string.Format("/applications/{0}/guilds/{1}/commands/permissions", this.Id, guildId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x00017C68 File Offset: 0x00015E68
		public void BatchEditCommandPermissions(DiscordClient client, Snowflake guildId, List<GuildCommandPermissions> permissions, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			client.Bot.Rest.DoRequest(string.Format("/applications/{0}/guilds/{1}/commands/permissions", this.Id, guildId), RequestMethod.PUT, permissions, callback, error);
		}
	}
}
