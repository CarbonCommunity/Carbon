/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Text.RegularExpressions;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Helpers
{
	// Token: 0x02000028 RID: 40
	public static class Validation
	{
		// Token: 0x06000171 RID: 369 RVA: 0x0000C57C File Offset: 0x0000A77C
		public static void ValidateEmoji(string emoji)
		{
			bool flag = string.IsNullOrEmpty(emoji);
			if (flag)
			{
				throw new InvalidEmojiException(emoji, "Emoji string cannot be null or empty.");
			}
			bool flag2 = emoji.Length == 2 && !char.IsSurrogatePair(emoji[0], emoji[1]);
			if (flag2)
			{
				throw new InvalidEmojiException(emoji, "Emoji of length 2 must be a surrogate pair");
			}
			bool flag3 = emoji.Length > 2 && !Validation.EmojiValidation.IsMatch(emoji);
			if (flag3)
			{
				throw new InvalidEmojiException(emoji, "Emoji string is not in the correct format.\nIf using a normal emoji please use the unicode character for that emoji.\nIf using a custom emoji the format must be emojiName:emojiId\nIf using a custom animated emoji the format must be a:emojiName:emojiId");
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000C600 File Offset: 0x0000A800
		public static void ValidateFilename(string filename)
		{
			bool flag = !Validation.FilenameValidation.IsMatch(filename);
			if (flag)
			{
				throw new InvalidFileNameException(filename);
			}
		}

		// Token: 0x040000F9 RID: 249
		private static readonly Regex EmojiValidation = new Regex("^.+:[0-9]+$", RegexOptions.Compiled);

		// Token: 0x040000FA RID: 250
		private static readonly Regex FilenameValidation = new Regex("^[a-zA-Z0-9_.-]*$", RegexOptions.Compiled);
	}
}
