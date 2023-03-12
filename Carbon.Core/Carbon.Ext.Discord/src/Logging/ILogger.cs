/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Logging
{
	// Token: 0x02000018 RID: 24
	public interface ILogger
	{
		// Token: 0x060000F9 RID: 249
		void Verbose(string message);

		// Token: 0x060000FA RID: 250
		void Debug(string message);

		// Token: 0x060000FB RID: 251
		void Info(string message);

		// Token: 0x060000FC RID: 252
		void Warning(string message);

		// Token: 0x060000FD RID: 253
		void Error(string message);

		// Token: 0x060000FE RID: 254
		void Exception(string message, Exception ex);

		// Token: 0x060000FF RID: 255
		void UpdateLogLevel(DiscordLogLevel level);

		// Token: 0x06000100 RID: 256
		bool IsLogging(DiscordLogLevel level);
	}
}
