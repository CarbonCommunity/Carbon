/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
	// Token: 0x020000F1 RID: 241
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ResumeSessionCommand
	{
		// Token: 0x040004FE RID: 1278
		[JsonProperty("token")]
		public string Token;

		// Token: 0x040004FF RID: 1279
		[JsonProperty("session_id")]
		public string SessionId;

		// Token: 0x04000500 RID: 1280
		[JsonProperty("seq")]
		public int Sequence;
	}
}
