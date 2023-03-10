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

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000AB RID: 171
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildMemberUpdate
	{
		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06000691 RID: 1681 RVA: 0x00014526 File Offset: 0x00012726
		// (set) Token: 0x06000692 RID: 1682 RVA: 0x0001452E File Offset: 0x0001272E
		[JsonProperty("nick")]
		public string Nick { get; set; }

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06000693 RID: 1683 RVA: 0x00014537 File Offset: 0x00012737
		// (set) Token: 0x06000694 RID: 1684 RVA: 0x0001453F File Offset: 0x0001273F
		[JsonProperty("roles")]
		public List<Snowflake> Roles { get; set; }

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000695 RID: 1685 RVA: 0x00014548 File Offset: 0x00012748
		// (set) Token: 0x06000696 RID: 1686 RVA: 0x00014550 File Offset: 0x00012750
		[JsonProperty("deaf")]
		public bool? Deaf { get; set; }

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000697 RID: 1687 RVA: 0x00014559 File Offset: 0x00012759
		// (set) Token: 0x06000698 RID: 1688 RVA: 0x00014561 File Offset: 0x00012761
		[JsonProperty("mute")]
		public bool? Mute { get; set; }

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000699 RID: 1689 RVA: 0x0001456A File Offset: 0x0001276A
		// (set) Token: 0x0600069A RID: 1690 RVA: 0x00014572 File Offset: 0x00012772
		[JsonProperty("channel_id")]
		public Snowflake? ChannelId { get; set; }

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x0600069B RID: 1691 RVA: 0x0001457B File Offset: 0x0001277B
		// (set) Token: 0x0600069C RID: 1692 RVA: 0x00014583 File Offset: 0x00012783
		[JsonProperty("communication_disabled_until")]
		public DateTime? CommunicationDisabledUntil { get; set; }
	}
}
