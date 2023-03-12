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

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
	// Token: 0x02000098 RID: 152
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordApplicationCommand
	{
		// Token: 0x17000193 RID: 403
		// (get) Token: 0x0600053D RID: 1341 RVA: 0x00012009 File Offset: 0x00010209
		// (set) Token: 0x0600053E RID: 1342 RVA: 0x00012011 File Offset: 0x00010211
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x0600053F RID: 1343 RVA: 0x0001201A File Offset: 0x0001021A
		// (set) Token: 0x06000540 RID: 1344 RVA: 0x00012022 File Offset: 0x00010222
		[JsonProperty("type")]
		public ApplicationCommandType? Type { get; set; }

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000541 RID: 1345 RVA: 0x0001202B File Offset: 0x0001022B
		// (set) Token: 0x06000542 RID: 1346 RVA: 0x00012033 File Offset: 0x00010233
		[JsonProperty("application_id")]
		public Snowflake ApplicationId { get; set; }

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000543 RID: 1347 RVA: 0x0001203C File Offset: 0x0001023C
		// (set) Token: 0x06000544 RID: 1348 RVA: 0x00012044 File Offset: 0x00010244
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000545 RID: 1349 RVA: 0x0001204D File Offset: 0x0001024D
		// (set) Token: 0x06000546 RID: 1350 RVA: 0x00012055 File Offset: 0x00010255
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000547 RID: 1351 RVA: 0x0001205E File Offset: 0x0001025E
		// (set) Token: 0x06000548 RID: 1352 RVA: 0x00012066 File Offset: 0x00010266
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000549 RID: 1353 RVA: 0x0001206F File Offset: 0x0001026F
		// (set) Token: 0x0600054A RID: 1354 RVA: 0x00012077 File Offset: 0x00010277
		[JsonProperty("options")]
		public List<CommandOption> Options { get; set; }

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x0600054B RID: 1355 RVA: 0x00012080 File Offset: 0x00010280
		// (set) Token: 0x0600054C RID: 1356 RVA: 0x00012088 File Offset: 0x00010288
		[JsonProperty("default_permission")]
		public bool? DefaultPermissions { get; set; }

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x0600054D RID: 1357 RVA: 0x00012091 File Offset: 0x00010291
		// (set) Token: 0x0600054E RID: 1358 RVA: 0x00012099 File Offset: 0x00010299
		[JsonProperty("version")]
		public Snowflake Version { get; set; }

		// Token: 0x0600054F RID: 1359 RVA: 0x000120A4 File Offset: 0x000102A4
		public void Edit(DiscordClient client, CommandUpdate update, Action<DiscordApplicationCommand> callback = null, Action<RestError> error = null)
		{
			bool flag = this.GuildId != null;
			if (flag)
			{
				client.Bot.Rest.DoRequest<DiscordApplicationCommand>(string.Format("/applications/{0}/guilds/{1}/commands/{2}", this.ApplicationId, this.GuildId, this.Id), RequestMethod.PATCH, update, callback, error);
			}
			else
			{
				client.Bot.Rest.DoRequest<DiscordApplicationCommand>(string.Format("/applications/{0}/commands", this.ApplicationId), RequestMethod.PATCH, update, callback, error);
			}
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x00012134 File Offset: 0x00010334
		public void Delete(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			bool flag = this.GuildId != null;
			if (flag)
			{
				client.Bot.Rest.DoRequest(string.Format("/applications/{0}/guilds/{1}/commands/{2}", this.ApplicationId, this.GuildId, this.Id), RequestMethod.DELETE, null, callback, error);
			}
			else
			{
				client.Bot.Rest.DoRequest(string.Format("/applications/{0}/commands/{1}", this.ApplicationId, this.Id), RequestMethod.DELETE, null, callback, error);
			}
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x000121CC File Offset: 0x000103CC
		public void GetPermissions(DiscordClient client, Snowflake guildId, Action<GuildCommandPermissions> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			client.Bot.Rest.DoRequest<GuildCommandPermissions>(string.Format("/applications/{0}/guilds/{1}/commands/{2}/permissions", this.ApplicationId, guildId, this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x00012230 File Offset: 0x00010430
		public void EditPermissions(DiscordClient client, Snowflake guildId, List<CommandPermissions> permissions, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["permissions"] = permissions;
			Dictionary<string, object> data = dictionary;
			client.Bot.Rest.DoRequest(string.Format("/applications/{0}/guilds/{1}/commands/{2}/permissions", this.ApplicationId, guildId, this.Id), RequestMethod.PUT, data, callback, error);
		}
	}
}
