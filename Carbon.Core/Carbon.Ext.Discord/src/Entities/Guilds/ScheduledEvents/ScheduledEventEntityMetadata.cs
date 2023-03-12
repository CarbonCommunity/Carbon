/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
	// Token: 0x020000BB RID: 187
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ScheduledEventEntityMetadata
	{
		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06000724 RID: 1828 RVA: 0x00014DC9 File Offset: 0x00012FC9
		// (set) Token: 0x06000725 RID: 1829 RVA: 0x00014DD1 File Offset: 0x00012FD1
		[JsonProperty("location")]
		public string Location { get; set; }

		// Token: 0x06000726 RID: 1830 RVA: 0x00014DDC File Offset: 0x00012FDC
		internal void Update(ScheduledEventEntityMetadata metadata)
		{
			bool flag = metadata.Location != null;
			if (flag)
			{
				this.Location = metadata.Location;
			}
		}
	}
}
