/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Exceptions
{
	// Token: 0x0200003B RID: 59
	public class InvalidEmojiException : BaseDiscordException
	{
		// Token: 0x060001B5 RID: 437 RVA: 0x0000DB33 File Offset: 0x0000BD33
		internal InvalidEmojiException(string emojiValue, string validationError) : base("'" + emojiValue + "' failed emoji validation with error: " + validationError)
		{
		}
	}
}
