/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Core;

namespace Oxide.Ext.Discord.Logging
{
	// Token: 0x02000019 RID: 25
	public class Logger : ILogger
	{
		// Token: 0x06000101 RID: 257 RVA: 0x0000A7DC File Offset: 0x000089DC
		public Logger(DiscordLogLevel logLevel)
		{
			this._logLevel = logLevel;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000A7ED File Offset: 0x000089ED
		public void Verbose(string message)
		{
			this.Log(DiscordLogLevel.Verbose, message, null);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000A7FA File Offset: 0x000089FA
		public void Debug(string message)
		{
			this.Log(DiscordLogLevel.Debug, message, null);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000A807 File Offset: 0x00008A07
		public void Info(string message)
		{
			this.Log(DiscordLogLevel.Info, message, null);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x0000A814 File Offset: 0x00008A14
		public void Warning(string message)
		{
			this.Log(DiscordLogLevel.Warning, message, null);
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0000A821 File Offset: 0x00008A21
		public void Error(string message)
		{
			this.Log(DiscordLogLevel.Error, message, null);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000A82E File Offset: 0x00008A2E
		public void Exception(string message, Exception ex)
		{
			this.Log(DiscordLogLevel.Exception, message, ex);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000A83C File Offset: 0x00008A3C
		private void Log(DiscordLogLevel level, string message, object data = null)
		{
			bool flag = !this.IsLogging(level);
			if (!flag)
			{
				string text = "[Discord Extension] [" + level.ToString() + "]: " + message;
				switch (level)
				{
				case DiscordLogLevel.Debug:
				case DiscordLogLevel.Warning:
					Interface.Oxide.LogWarning(text, Array.Empty<object>());
					return;
				case DiscordLogLevel.Error:
					Interface.Oxide.LogError(text, Array.Empty<object>());
					return;
				case DiscordLogLevel.Exception:
					Interface.Oxide.LogException(string.Format("{0}\n{1}", text, data), (Exception)data);
					return;
				}
				Interface.Oxide.LogInfo(text, Array.Empty<object>());
			}
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000A8F6 File Offset: 0x00008AF6
		public void UpdateLogLevel(DiscordLogLevel level)
		{
			this._logLevel = level;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000A900 File Offset: 0x00008B00
		public bool IsLogging(DiscordLogLevel level)
		{
			return level >= this._logLevel;
		}

		// Token: 0x040000D7 RID: 215
		private DiscordLogLevel _logLevel;
	}
}
