/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Emojis;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000E2 RID: 226
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class MessageReactionRemovedAllEmojiEvent : MessageReactionRemovedAllEvent
	{
		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x0600080C RID: 2060 RVA: 0x00015660 File Offset: 0x00013860
		// (set) Token: 0x0600080D RID: 2061 RVA: 0x00015668 File Offset: 0x00013868
		public DiscordEmoji Emoji { get; set; }
	}
}
