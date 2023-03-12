/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using System.Timers;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Gatway;
using Oxide.Ext.Discord.Entities.Gatway.Commands;
using Oxide.Ext.Discord.Logging;
using Oxide.Ext.Discord.RateLimits;

namespace Oxide.Ext.Discord.WebSockets.Handlers
{
	// Token: 0x0200000C RID: 12
	public class SocketCommandHandler
	{
		// Token: 0x0600009E RID: 158 RVA: 0x00008A1C File Offset: 0x00006C1C
		public SocketCommandHandler(Socket webSocket, ILogger logger)
		{
			this._webSocket = webSocket;
			this._logger = logger;
			this._rateLimitTimer = new Timer(1000.0);
			this._rateLimitTimer.AutoReset = false;
			this._rateLimitTimer.Elapsed += this.RateLimitElapsed;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00008A9C File Offset: 0x00006C9C
		public void Enqueue(CommandPayload command)
		{
			bool flag = this._logger.IsLogging(DiscordLogLevel.Debug);
			if (flag)
			{
				this._logger.Debug("SocketCommandHandler.Enqueue Queuing command " + command.OpCode.ToString());
			}
			bool flag2 = this._webSocket.IsConnected() && (command.OpCode == GatewayCommandCode.Identify || command.OpCode == GatewayCommandCode.Resume);
			if (flag2)
			{
				this._webSocket.Send(command);
			}
			else
			{
				bool flag3 = !this._socketCanSendCommands;
				if (flag3)
				{
					bool flag4 = command.OpCode == GatewayCommandCode.PresenceUpdate;
					if (flag4)
					{
						this.RemoveByType(GatewayCommandCode.PresenceUpdate);
					}
					else
					{
						bool flag5 = command.OpCode == GatewayCommandCode.VoiceStateUpdate;
						if (flag5)
						{
							this.RemoveByType(GatewayCommandCode.VoiceStateUpdate);
						}
					}
					this.AddCommand(command);
				}
				else
				{
					this.AddCommand(command);
					this.SendCommands();
				}
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00008B75 File Offset: 0x00006D75
		internal void OnSocketConnected()
		{
			this._logger.Debug("SocketCommandHandler.OnSocketConnected Socket Connected. Sending queued commands.");
			this._socketCanSendCommands = true;
			this.SendCommands();
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00008B97 File Offset: 0x00006D97
		internal void OnSocketDisconnected()
		{
			this._logger.Debug("SocketCommandHandler.OnSocketConnected Socket Disconnected. Queuing Commands.");
			this._socketCanSendCommands = false;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00008BB4 File Offset: 0x00006DB4
		private void RateLimitElapsed(object sender, ElapsedEventArgs e)
		{
			this._logger.Debug("SocketCommandHandler.RateLimitElapsed Rate Limit has elapsed. Send Queued Commands");
			this._rateLimitTimer.Stop();
			bool flag = !this._socketCanSendCommands;
			if (flag)
			{
				this._rateLimitTimer.Interval = 1000.0;
				this._logger.Debug("SocketCommandHandler.RateLimitElapsed Can't send commands right now. Trying again in 1 second");
				this._rateLimitTimer.Start();
			}
			else
			{
				this.SendCommands();
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00008C28 File Offset: 0x00006E28
		private void SendCommands()
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				while (this._pendingCommands.Count != 0)
				{
					CommandPayload commandPayload = this._pendingCommands[0];
					bool hasReachedRateLimit = this._rateLimit.HasReachedRateLimit;
					if (hasReachedRateLimit)
					{
						bool flag2 = !this._rateLimitTimer.Enabled;
						if (flag2)
						{
							this._rateLimitTimer.Interval = this._rateLimit.NextReset;
							this._rateLimitTimer.Stop();
							this._rateLimitTimer.Start();
							this._logger.Warning(string.Format("{0}.{1} Rate Limit Hit! Retrying in {2} seconds\nOpcode: {3}\nPayload: {4}", new object[]
							{
								"SocketCommandHandler",
								"SendCommands",
								this._rateLimit.NextReset.ToString(),
								commandPayload.OpCode,
								JsonConvert.SerializeObject(commandPayload.Payload, DiscordExtension.ExtensionSerializeSettings)
							}));
						}
						break;
					}
					bool flag3 = this._logger.IsLogging(DiscordLogLevel.Debug);
					if (flag3)
					{
						this._logger.Debug("SocketCommandHandler.SendCommands Sending Command " + commandPayload.OpCode.ToString());
					}
					bool flag4 = !this._webSocket.Send(commandPayload);
					if (flag4)
					{
						break;
					}
					this._pendingCommands.RemoveAt(0);
					this._rateLimit.FiredRequest();
				}
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00008DD0 File Offset: 0x00006FD0
		private void AddCommand(CommandPayload command)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				bool flag2 = command.OpCode == GatewayCommandCode.Identify || command.OpCode == GatewayCommandCode.Resume;
				if (flag2)
				{
					this._pendingCommands.Insert(0, command);
				}
				else
				{
					this._pendingCommands.Add(command);
				}
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00008E48 File Offset: 0x00007048
		private void RemoveByType(GatewayCommandCode code)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				this._pendingCommands.RemoveAll((CommandPayload c) => c.OpCode == code);
			}
		}

		// Token: 0x0400008C RID: 140
		private readonly Socket _webSocket;

		// Token: 0x0400008D RID: 141
		private readonly ILogger _logger;

		// Token: 0x0400008E RID: 142
		private readonly List<CommandPayload> _pendingCommands = new List<CommandPayload>();

		// Token: 0x0400008F RID: 143
		private readonly WebsocketRateLimit _rateLimit = new WebsocketRateLimit();

		// Token: 0x04000090 RID: 144
		private readonly Timer _rateLimitTimer;

		// Token: 0x04000091 RID: 145
		private readonly object _syncRoot = new object();

		// Token: 0x04000092 RID: 146
		private bool _socketCanSendCommands;
	}
}
