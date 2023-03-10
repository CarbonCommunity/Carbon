/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
	// Token: 0x02000093 RID: 147
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class CommandOptionChoice
	{
		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000528 RID: 1320 RVA: 0x00011F70 File Offset: 0x00010170
		// (set) Token: 0x06000529 RID: 1321 RVA: 0x00011F78 File Offset: 0x00010178
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x0600052A RID: 1322 RVA: 0x00011F81 File Offset: 0x00010181
		// (set) Token: 0x0600052B RID: 1323 RVA: 0x00011F89 File Offset: 0x00010189
		[JsonProperty("value")]
		public object Value { get; set; }
	}
}
