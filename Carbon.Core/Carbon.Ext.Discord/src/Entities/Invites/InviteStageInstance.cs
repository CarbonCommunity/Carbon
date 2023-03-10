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
using Oxide.Ext.Discord.Entities.Guilds;

namespace Oxide.Ext.Discord.Entities.Invites
{
	// Token: 0x02000078 RID: 120
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class InviteStageInstance
	{
		// Token: 0x1700012D RID: 301
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x000111A4 File Offset: 0x0000F3A4
		// (set) Token: 0x0600044C RID: 1100 RVA: 0x000111AC File Offset: 0x0000F3AC
		[JsonProperty("members")]
		public List<GuildMember> Members { get; set; }

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x0600044D RID: 1101 RVA: 0x000111B5 File Offset: 0x0000F3B5
		// (set) Token: 0x0600044E RID: 1102 RVA: 0x000111BD File Offset: 0x0000F3BD
		[JsonProperty("participant_count")]
		public int ParticipantCount { get; set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x0600044F RID: 1103 RVA: 0x000111C6 File Offset: 0x0000F3C6
		// (set) Token: 0x06000450 RID: 1104 RVA: 0x000111CE File Offset: 0x0000F3CE
		[JsonProperty("speaker_count")]
		public int SpeakerCount { get; set; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000451 RID: 1105 RVA: 0x000111D7 File Offset: 0x0000F3D7
		// (set) Token: 0x06000452 RID: 1106 RVA: 0x000111DF File Offset: 0x0000F3DF
		[JsonProperty("topic")]
		public string Topic { get; set; }
	}
}
