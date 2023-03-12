/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Ext.Discord.Entities;

namespace Oxide.Ext.Discord.Helpers
{
	// Token: 0x02000024 RID: 36
	public class DiscordFormatting
	{
		// Token: 0x06000151 RID: 337 RVA: 0x0000BF7C File Offset: 0x0000A17C
		public static string MentionUser(Snowflake userId)
		{
			return "<@" + userId.ToString() + ">";
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000BF9A File Offset: 0x0000A19A
		public static string MentionUserNickname(Snowflake userId)
		{
			return "<@!" + userId.ToString() + ">";
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000BFB8 File Offset: 0x0000A1B8
		public static string MentionChannel(Snowflake channelId)
		{
			return "<#" + channelId.ToString() + ">";
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000BFD6 File Offset: 0x0000A1D6
		public static string MentionRole(Snowflake roleId)
		{
			return "<@&" + roleId.ToString() + ">";
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000BFF4 File Offset: 0x0000A1F4
		public static string CustomEmojiMessageString(Snowflake id, string name, bool animated)
		{
			return "<" + DiscordFormatting.CustomEmojiDataString(id, name, animated) + ">";
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000C010 File Offset: 0x0000A210
		public static string CustomEmojiDataString(Snowflake id, string name, bool animated)
		{
			return string.Concat(new string[]
			{
				animated ? "a" : "",
				":",
				name,
				":",
				id.ToString()
			});
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000C060 File Offset: 0x0000A260
		public static string UnixTimestamp(int timestamp, TimestampStyles style = TimestampStyles.ShortDateTime)
		{
			return string.Concat(new string[]
			{
				"<t:",
				timestamp.ToString(),
				":",
				DiscordFormatting.GetTimestampFlag(style),
				">"
			});
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000C0A8 File Offset: 0x0000A2A8
		private static string GetTimestampFlag(TimestampStyles style)
		{
			string result;
			switch (style)
			{
			case TimestampStyles.ShortTime:
				result = "t";
				break;
			case TimestampStyles.LongTime:
				result = "T";
				break;
			case TimestampStyles.ShortDate:
				result = "d";
				break;
			case TimestampStyles.LongDate:
				result = "D";
				break;
			case TimestampStyles.ShortDateTime:
				result = "f";
				break;
			case TimestampStyles.LongDateTime:
				result = "F";
				break;
			case TimestampStyles.RelativeTime:
				result = "R";
				break;
			default:
				result = "f";
				break;
			}
			return result;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000C11F File Offset: 0x0000A31F
		public static string Italics(string message)
		{
			return "*" + message + "*";
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000C131 File Offset: 0x0000A331
		public static string Bold(string message)
		{
			return "**" + message + "**";
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000C143 File Offset: 0x0000A343
		public static string ItalicsBold(string message)
		{
			return "***" + message + "***";
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000C155 File Offset: 0x0000A355
		public static string Underline(string message)
		{
			return "__" + message + "__";
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000C167 File Offset: 0x0000A367
		public static string UnderlineItalics(string message)
		{
			return "__*" + message + "*__";
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000C179 File Offset: 0x0000A379
		public static string UnderlineBold(string message)
		{
			return "__**" + message + "**__";
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000C18B File Offset: 0x0000A38B
		public static string UnderlineBoldItalics(string message)
		{
			return "__***" + message + "***__";
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000C19D File Offset: 0x0000A39D
		public static string Strikethrough(string message)
		{
			return "~~" + message + "~~";
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000C1AF File Offset: 0x0000A3AF
		public static string CodeBlockOneLine(string message)
		{
			return "`" + message + "`";
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000C1C1 File Offset: 0x0000A3C1
		public static string CodeBlockMultiLine(string message)
		{
			return "```\n" + message + "\n```";
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000C1D3 File Offset: 0x0000A3D3
		public static string CodeBlockLanguage(string message, string language)
		{
			return string.Concat(new string[]
			{
				"```",
				language,
				"\n",
				message,
				"\n```"
			});
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000C200 File Offset: 0x0000A400
		public static string BlockQuoteSingleLine(string message)
		{
			return "> " + message;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000C20D File Offset: 0x0000A40D
		public static string BlockQuoteMultiLine(string message)
		{
			return ">>> " + message;
		}
	}
}
