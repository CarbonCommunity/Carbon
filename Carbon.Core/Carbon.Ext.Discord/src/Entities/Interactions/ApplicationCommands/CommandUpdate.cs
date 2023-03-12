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

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
	// Token: 0x02000097 RID: 151
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class CommandUpdate
	{
		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000534 RID: 1332 RVA: 0x00011FC5 File Offset: 0x000101C5
		// (set) Token: 0x06000535 RID: 1333 RVA: 0x00011FCD File Offset: 0x000101CD
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000536 RID: 1334 RVA: 0x00011FD6 File Offset: 0x000101D6
		// (set) Token: 0x06000537 RID: 1335 RVA: 0x00011FDE File Offset: 0x000101DE
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000538 RID: 1336 RVA: 0x00011FE7 File Offset: 0x000101E7
		// (set) Token: 0x06000539 RID: 1337 RVA: 0x00011FEF File Offset: 0x000101EF
		[JsonProperty("options")]
		public List<CommandOption> Options { get; set; }

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x0600053A RID: 1338 RVA: 0x00011FF8 File Offset: 0x000101F8
		// (set) Token: 0x0600053B RID: 1339 RVA: 0x00012000 File Offset: 0x00010200
		[JsonProperty("default_permission")]
		public bool? DefaultPermissions { get; set; }
	}
}
