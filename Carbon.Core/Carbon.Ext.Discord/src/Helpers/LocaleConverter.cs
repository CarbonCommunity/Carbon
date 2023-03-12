/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Helpers
{
	// Token: 0x02000025 RID: 37
	public static class LocaleConverter
	{
		// Token: 0x06000167 RID: 359 RVA: 0x0000C224 File Offset: 0x0000A424
		static LocaleConverter()
		{
			LocaleConverter.AddLocale("en", "en-US");
			LocaleConverter.AddLocale("bg-", "bg");
			LocaleConverter.AddLocale("zh", "zh-CN");
			LocaleConverter.AddLocale("hr", "hr");
			LocaleConverter.AddLocale("cs", "cs");
			LocaleConverter.AddLocale("da", "da");
			LocaleConverter.AddLocale("nl", "nl");
			LocaleConverter.AddLocale("fi", "fi");
			LocaleConverter.AddLocale("fr", "fr");
			LocaleConverter.AddLocale("de", "de");
			LocaleConverter.AddLocale("el", "el");
			LocaleConverter.AddLocale("hi", "hi");
			LocaleConverter.AddLocale("hu", "hu");
			LocaleConverter.AddLocale("it", "it");
			LocaleConverter.AddLocale("ja", "ja");
			LocaleConverter.AddLocale("ko", "ko");
			LocaleConverter.AddLocale("lt", "lt");
			LocaleConverter.AddLocale("no", "no");
			LocaleConverter.AddLocale("pl", "pl");
			LocaleConverter.AddLocale("pt", "pt-BR");
			LocaleConverter.AddLocale("ro", "ro");
			LocaleConverter.AddLocale("ru", "ru");
			LocaleConverter.AddLocale("es", "es-ES");
			LocaleConverter.AddLocale("sv", "sv-SE");
			LocaleConverter.AddLocale("th", "th");
			LocaleConverter.AddLocale("tr", "tr");
			LocaleConverter.AddLocale("uk", "uk");
			LocaleConverter.AddLocale("vi", "vi");
			LocaleConverter.DiscordToOxide["en-GB"] = "en";
			LocaleConverter.DiscordToOxide["zh-TW"] = "zh";
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000C430 File Offset: 0x0000A630
		private static void AddLocale(string oxide, string discord)
		{
			LocaleConverter.DiscordToOxide[discord] = oxide;
			LocaleConverter.OxideToDiscord[oxide] = discord;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000C450 File Offset: 0x0000A650
		public static string GetOxideLocale(string discordLocale)
		{
			return (!string.IsNullOrEmpty(discordLocale)) ? LocaleConverter.DiscordToOxide[discordLocale] : string.Empty;
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000C47C File Offset: 0x0000A67C
		public static string GetDiscordLocale(string oxideLocale)
		{
			return (!string.IsNullOrEmpty(oxideLocale)) ? LocaleConverter.OxideToDiscord[oxideLocale] : string.Empty;
		}

		// Token: 0x040000ED RID: 237
		private static readonly Hash<string, string> DiscordToOxide = new Hash<string, string>();

		// Token: 0x040000EE RID: 238
		private static readonly Hash<string, string> OxideToDiscord = new Hash<string, string>();
	}
}
