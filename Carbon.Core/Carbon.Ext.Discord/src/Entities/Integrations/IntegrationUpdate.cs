/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Integrations
{
	// Token: 0x0200009F RID: 159
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class IntegrationUpdate
	{
		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000588 RID: 1416 RVA: 0x00012448 File Offset: 0x00010648
		// (set) Token: 0x06000589 RID: 1417 RVA: 0x00012450 File Offset: 0x00010650
		[JsonProperty("enable_emoticons")]
		public bool? EnableEmoticons { get; set; }

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x0600058A RID: 1418 RVA: 0x00012459 File Offset: 0x00010659
		// (set) Token: 0x0600058B RID: 1419 RVA: 0x00012461 File Offset: 0x00010661
		[JsonProperty("expire_behaviour")]
		public IntegrationExpireBehaviors? ExpireBehaviour { get; set; }

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x0600058C RID: 1420 RVA: 0x0001246A File Offset: 0x0001066A
		// (set) Token: 0x0600058D RID: 1421 RVA: 0x00012472 File Offset: 0x00010672
		[JsonProperty("expire_grace_period")]
		public int? ExpireGracePeriod { get; set; }
	}
}
