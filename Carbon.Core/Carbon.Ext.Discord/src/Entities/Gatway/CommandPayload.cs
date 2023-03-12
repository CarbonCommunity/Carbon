/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Gatway.Commands;

namespace Oxide.Ext.Discord.Entities.Gatway
{
	// Token: 0x020000C3 RID: 195
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class CommandPayload
	{
		// Token: 0x04000456 RID: 1110
		[JsonProperty("op")]
		public GatewayCommandCode OpCode;

		// Token: 0x04000457 RID: 1111
		[JsonProperty("d")]
		public object Payload;
	}
}
