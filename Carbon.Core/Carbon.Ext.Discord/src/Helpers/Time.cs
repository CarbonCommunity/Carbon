/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Helpers
{
	// Token: 0x02000026 RID: 38
	public static class Time
	{
		// Token: 0x0600016B RID: 363 RVA: 0x0000C4A8 File Offset: 0x0000A6A8
		public static int TimeSinceEpoch()
		{
			return (int)(DateTime.UtcNow - Time.Epoch).TotalSeconds;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000C4D0 File Offset: 0x0000A6D0
		public static DateTime ToDateTimeOffsetFromMilliseconds(this long milliseconds)
		{
			return Time.Epoch.AddMilliseconds((double)milliseconds);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000C4EC File Offset: 0x0000A6EC
		public static double TimeSinceEpoch(DateTime time)
		{
			return (time - Time.Epoch).TotalSeconds;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000C50C File Offset: 0x0000A70C
		public static DateTime ToDateTime(this int timestamp)
		{
			return Time.Epoch.AddSeconds((double)timestamp);
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000C528 File Offset: 0x0000A728
		public static int ToUnixTimeStamp(this DateTime date)
		{
			return (int)(date - Time.Epoch).TotalSeconds;
		}

		// Token: 0x040000EF RID: 239
		public static readonly DateTimeOffset DiscordEpoch = new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero);

		// Token: 0x040000F0 RID: 240
		public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}
