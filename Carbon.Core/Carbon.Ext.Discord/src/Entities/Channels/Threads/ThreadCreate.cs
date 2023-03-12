/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Channels.Threads
{
	// Token: 0x02000105 RID: 261
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ThreadCreate
	{
		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06000976 RID: 2422 RVA: 0x00017042 File Offset: 0x00015242
		// (set) Token: 0x06000977 RID: 2423 RVA: 0x0001704A File Offset: 0x0001524A
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06000978 RID: 2424 RVA: 0x00017053 File Offset: 0x00015253
		// (set) Token: 0x06000979 RID: 2425 RVA: 0x0001705B File Offset: 0x0001525B
		[JsonProperty("auto_archive_duration")]
		public int? AutoArchiveDuration { get; set; }

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x0600097A RID: 2426 RVA: 0x00017064 File Offset: 0x00015264
		// (set) Token: 0x0600097B RID: 2427 RVA: 0x0001706C File Offset: 0x0001526C
		[JsonProperty("type")]
		public ChannelType Type { get; set; } = ChannelType.GuildPrivateThread;

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x0600097C RID: 2428 RVA: 0x00017075 File Offset: 0x00015275
		// (set) Token: 0x0600097D RID: 2429 RVA: 0x0001707D File Offset: 0x0001527D
		[JsonProperty("invitable")]
		public bool? Invitable { get; set; }
	}
}
