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
using Oxide.Ext.Discord.Helpers.Cdn;

namespace Oxide.Ext.Discord.Entities.Teams
{
	// Token: 0x02000053 RID: 83
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordTeam
	{
		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000291 RID: 657 RVA: 0x0000F355 File Offset: 0x0000D555
		// (set) Token: 0x06000292 RID: 658 RVA: 0x0000F35D File Offset: 0x0000D55D
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000293 RID: 659 RVA: 0x0000F366 File Offset: 0x0000D566
		// (set) Token: 0x06000294 RID: 660 RVA: 0x0000F36E File Offset: 0x0000D56E
		[JsonProperty("icon")]
		public string Icon { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000295 RID: 661 RVA: 0x0000F377 File Offset: 0x0000D577
		// (set) Token: 0x06000296 RID: 662 RVA: 0x0000F37F File Offset: 0x0000D57F
		[JsonProperty("members")]
		public List<TeamMember> Members { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000297 RID: 663 RVA: 0x0000F388 File Offset: 0x0000D588
		// (set) Token: 0x06000298 RID: 664 RVA: 0x0000F390 File Offset: 0x0000D590
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000299 RID: 665 RVA: 0x0000F399 File Offset: 0x0000D599
		// (set) Token: 0x0600029A RID: 666 RVA: 0x0000F3A1 File Offset: 0x0000D5A1
		[JsonProperty("owner_user_id")]
		public Snowflake OwnerUserId { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600029B RID: 667 RVA: 0x0000F3AA File Offset: 0x0000D5AA
		public string GetTeamIconUrl
		{
			get
			{
				return DiscordCdn.GetTeamIconUrl(this.Id, this.Icon, ImageFormat.Auto);
			}
		}
	}
}
