/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Timers;
using Oxide.Core;
using Oxide.Ext.Discord.Logging;
using Random = Oxide.Core.Random;

namespace Oxide.Ext.Discord.WebSockets.Handlers
{
	// Token: 0x0200000B RID: 11
	public class HeartbeatHandler
	{
		// Token: 0x06000099 RID: 153 RVA: 0x00008707 File Offset: 0x00006907
		public HeartbeatHandler(BotClient client, Socket socket, SocketListener listener, ILogger logger)
		{
			this._client = client;
			this._socket = socket;
			this._listener = listener;
			this._logger = logger;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00008730 File Offset: 0x00006930
		internal void SetupHeartbeat(float interval)
		{
			bool flag = this._timer != null;
			if (flag)
			{
				this._logger.Debug("HeartbeatHandler.SetupHeartbeat Previous heartbeat timer exists.");
				this.DestroyHeartbeat();
			}
			this.HeartbeatAcknowledged = true;
			this._interval = interval;
			this._initial = true;
			this._timer = new Timer((double)(this._interval * Random.Range(0f, 1f)));
			this._timer.Elapsed += this.HeartbeatElapsed;
			this._timer.Start();
			this._logger.Debug("HeartbeatHandler.SetupHeartbeat Creating heartbeat with interval " + interval.ToString() + "ms.");
			this._client.CallHook("OnDiscordSetupHeartbeat", new object[]
			{
				interval
			});
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00008804 File Offset: 0x00006A04
		public void DestroyHeartbeat()
		{
			bool flag = this._timer != null;
			if (flag)
			{
				this._logger.Debug("HeartbeatHandler.DestroyHeartbeat Destroy Heartbeat");
				this._timer.Dispose();
				this._timer = null;
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00008848 File Offset: 0x00006A48
		private void HeartbeatElapsed(object sender, ElapsedEventArgs e)
		{
			this._logger.Debug("HeartbeatHandler.HeartbeatElapsed Heartbeat Elapsed");
			bool initial = this._initial;
			if (initial)
			{
				this._timer.Interval = (double)this._interval;
				this._initial = false;
			}
			bool flag = !this._listener.SocketHasConnected;
			if (flag)
			{
				this._logger.Debug("HeartbeatHandler.HeartbeatElapsed Websocket has not yet connected successfully. Skipping Heartbeat.");
			}
			else
			{
				bool flag2 = this._socket.IsPendingReconnect();
				if (flag2)
				{
					this._logger.Debug("HeartbeatHandler.HeartbeatElapsed Websocket is offline and is waiting to connect.");
				}
				else
				{
					bool flag3 = this._socket.IsDisconnected();
					if (flag3)
					{
						this._logger.Debug("HeartbeatHandler.HeartbeatElapsed Websocket is offline and is NOT connecting. Attempt Reconnect.");
						this._socket.Reconnect();
					}
					else
					{
						bool flag4 = !this.HeartbeatAcknowledged;
						if (flag4)
						{
							bool flag5 = this._socket.IsConnected();
							if (flag5)
							{
								this._logger.Debug("HeartbeatHandler.HeartbeatElapsed Heartbeat Elapsed and WebSocket is connected. Forcing reconnect.");
								this._socket.Disconnect(true, true, true);
							}
							else
							{
								bool flag6 = !this._socket.IsConnecting() && !this._socket.IsPendingReconnect();
								if (flag6)
								{
									this._logger.Debug("HeartbeatHandler.HeartbeatElapsed Heartbeat Elapsed and bot is not online or connecting.");
									this._socket.Reconnect();
								}
								else
								{
									this._logger.Debug("HeartbeatHandler.HeartbeatElapsed Heartbeat Elapsed and bot is not online but is waiting to connecting or waiting to reconnect.");
								}
							}
						}
						else
						{
							this.SendHeartbeat();
						}
					}
				}
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000089B4 File Offset: 0x00006BB4
		public void SendHeartbeat()
		{
			this.HeartbeatAcknowledged = false;
			this._listener.SendHeartbeat();
			this._client.CallHook("OnDiscordHeartbeatSent", Array.Empty<object>());
			this._logger.Debug("HeartbeatHandler.SendHeartbeat Heartbeat sent - " + this._timer.Interval.ToString() + "ms interval.");
		}

		// Token: 0x04000084 RID: 132
		public bool HeartbeatAcknowledged;

		// Token: 0x04000085 RID: 133
		private readonly BotClient _client;

		// Token: 0x04000086 RID: 134
		private readonly Socket _socket;

		// Token: 0x04000087 RID: 135
		private readonly SocketListener _listener;

		// Token: 0x04000088 RID: 136
		private readonly ILogger _logger;

		// Token: 0x04000089 RID: 137
		private Timer _timer;

		// Token: 0x0400008A RID: 138
		private float _interval;

		// Token: 0x0400008B RID: 139
		private bool _initial;
	}
}
