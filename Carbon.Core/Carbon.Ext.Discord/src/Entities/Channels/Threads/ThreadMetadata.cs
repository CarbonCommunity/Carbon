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
	// Token: 0x02000108 RID: 264
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ThreadMetadata
	{
		// Token: 0x17000353 RID: 851
		// (get) Token: 0x06000990 RID: 2448 RVA: 0x0001711E File Offset: 0x0001531E
		// (set) Token: 0x06000991 RID: 2449 RVA: 0x00017126 File Offset: 0x00015326
		[JsonProperty("archived")]
		public bool Archived { get; set; }

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06000992 RID: 2450 RVA: 0x0001712F File Offset: 0x0001532F
		// (set) Token: 0x06000993 RID: 2451 RVA: 0x00017137 File Offset: 0x00015337
		[JsonProperty("auto_archive_duration")]
		public int AutoArchiveDuration { get; set; }

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06000994 RID: 2452 RVA: 0x00017140 File Offset: 0x00015340
		// (set) Token: 0x06000995 RID: 2453 RVA: 0x00017148 File Offset: 0x00015348
		[JsonProperty("archive_timestamp")]
		public DateTime ArchiveTimestamp { get; set; }

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x06000996 RID: 2454 RVA: 0x00017151 File Offset: 0x00015351
		// (set) Token: 0x06000997 RID: 2455 RVA: 0x00017159 File Offset: 0x00015359
		[JsonProperty("locked")]
		public bool? Locked { get; set; }

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06000998 RID: 2456 RVA: 0x00017162 File Offset: 0x00015362
		// (set) Token: 0x06000999 RID: 2457 RVA: 0x0001716A File Offset: 0x0001536A
		[JsonProperty("invitable")]
		public bool? Invitable { get; set; }

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x0600099A RID: 2458 RVA: 0x00017173 File Offset: 0x00015373
		// (set) Token: 0x0600099B RID: 2459 RVA: 0x0001717B File Offset: 0x0001537B
		[JsonProperty("create_timestamp")]
		public DateTime? CreateTimestamp { get; set; }
	}
}
