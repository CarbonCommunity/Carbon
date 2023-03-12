/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Security.Authentication;
using System.Timers;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Ext.Discord.Entities.Gatway;
using Oxide.Ext.Discord.Entities.Gatway.Commands;
using Oxide.Ext.Discord.Logging;
using Oxide.Ext.Discord.WebSockets.Handlers;
using WebSocketSharp;

namespace Oxide.Ext.Discord.WebSockets
{
	// Token: 0x02000007 RID: 7
	public class Socket
	{
		// Token: 0x0600003E RID: 62 RVA: 0x000032A4 File Offset: 0x000014A4
		public Socket(BotClient client, ILogger logger)
		{
			this._client = client;
			this._logger = logger;
			this._commands = new SocketCommandHandler(this, logger);
			this._listener = new SocketListener(this._client, this, this._logger, this._commands);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003304 File Offset: 0x00001504
		public void Connect()
		{
			string websocketUrl = Gateway.WebsocketUrl;
			bool flag = string.IsNullOrEmpty(websocketUrl);
			if (flag)
			{
				Gateway.UpdateGatewayUrl(this._client, new Action(this.Connect));
			}
			else
			{
				object @lock = this._lock;
				lock (@lock)
				{
					bool flag3 = this.IsConnected() || this.IsConnecting();
					if (flag3)
					{
						throw new Exception("Socket is already running. Please disconnect before attempting to connect.");
					}
					this.SocketState = SocketState.Connecting;
				}
				this.RequestedReconnect = false;
				this.ShouldAttemptResume = false;
				this._socket = new WebSocket(websocketUrl, Array.Empty<string>());
				this._socket.SslConfiguration.EnabledSslProtocols |= SslProtocols.Tls12;
				this._socket.OnOpen += this._listener.SocketOpened;
				this._socket.OnClose += this._listener.SocketClosed;
				this._socket.OnError += this._listener.SocketErrored;
				this._socket.OnMessage += this._listener.SocketMessage;
				this._socket.ConnectAsync();
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003458 File Offset: 0x00001658
		public void Disconnect(bool attemptReconnect, bool shouldResume, bool requested = false)
		{
			this.RequestedReconnect = attemptReconnect;
			this.ShouldAttemptResume = shouldResume;
			this._commands.OnSocketDisconnected();
			bool flag = this._reconnectTimer != null;
			if (flag)
			{
				this._reconnectTimer.Stop();
				this._reconnectTimer.Dispose();
				this._reconnectTimer = null;
			}
			object @lock = this._lock;
			lock (@lock)
			{
				bool flag3 = this.IsDisconnected();
				if (flag3)
				{
					this.DisposeSocket();
					return;
				}
				if (requested)
				{
					this._socket.CloseAsync(4199, "Discord Requested Reconnect");
				}
				else
				{
					this._socket.CloseAsync(1000);
				}
				this.DisposeSocket();
				this.SocketState = SocketState.Disconnected;
			}
			bool requestedReconnect = this.RequestedReconnect;
			if (requestedReconnect)
			{
				this.Reconnect();
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000354C File Offset: 0x0000174C
		internal bool IsCurrentSocket(WebSocket socket)
		{
			return this._socket != null && this._socket == socket;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003572 File Offset: 0x00001772
		public void Shutdown()
		{
			this.Disconnect(false, false, false);
			SocketListener listener = this._listener;
			if (listener != null)
			{
				listener.Shutdown();
			}
			this._listener = null;
			this._socket = null;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000035A0 File Offset: 0x000017A0
		public void DisposeSocket()
		{
			bool flag = this._socket != null;
			if (flag)
			{
				this._socket.OnOpen -= this._listener.SocketOpened;
				this._socket.OnError -= this._listener.SocketErrored;
				this._socket.OnMessage -= this._listener.SocketMessage;
				this._socket = null;
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000361C File Offset: 0x0000181C
		public void Send(GatewayCommandCode opCode, object data)
		{
			CommandPayload command = new CommandPayload
			{
				OpCode = opCode,
				Payload = data
			};
			this._commands.Enqueue(command);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x0000364C File Offset: 0x0000184C
		internal bool Send(CommandPayload payload)
		{
			string text = JsonConvert.SerializeObject(payload, DiscordExtension.ExtensionSerializeSettings);
			bool flag = this._logger.IsLogging(DiscordLogLevel.Verbose);
			if (flag)
			{
				this._logger.Verbose("Socket.Send Payload: " + text);
			}
			bool flag2 = this._socket == null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				this._socket.SendAsync(text, null);
				result = true;
			}
			return result;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000036B4 File Offset: 0x000018B4
		public bool IsConnected()
		{
			return this.SocketState == SocketState.Connected;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000036D0 File Offset: 0x000018D0
		public bool IsConnecting()
		{
			return this.SocketState == SocketState.Connecting;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000036EC File Offset: 0x000018EC
		public bool IsPendingReconnect()
		{
			return this.SocketState == SocketState.PendingReconnect;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003708 File Offset: 0x00001908
		public bool IsDisconnected()
		{
			return this.SocketState == SocketState.Disconnected;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003724 File Offset: 0x00001924
		public void Reconnect()
		{
			bool flag = !this._client.Initialized;
			if (!flag)
			{
				object @lock = this._lock;
				lock (@lock)
				{
					bool flag3 = this.SocketState > SocketState.Disconnected;
					if (flag3)
					{
						return;
					}
					this.SocketState = SocketState.PendingReconnect;
				}
				this._reconnectRetries++;
				bool flag4 = this._reconnectRetries == 1;
				if (flag4)
				{
					Interface.Oxide.NextTick(new Action(this.Connect));
				}
				else
				{
					float num = (this._reconnectRetries <= 3) ? 1f : 15f;
					this._reconnectTimer = new Timer
					{
						Interval = (double)(num * 1000f),
						AutoReset = false
					};
					this._reconnectTimer.Elapsed += this.ReconnectWebsocket;
					this._logger.Warning("Attempting to reconnect to Discord... [Retry=" + this._reconnectRetries.ToString() + "]");
					this._reconnectTimer.Start();
				}
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003854 File Offset: 0x00001A54
		private void ReconnectWebsocket(object sender, ElapsedEventArgs e)
		{
			bool flag = this._reconnectRetries > 3;
			if (flag)
			{
				Gateway.UpdateGatewayUrl(this._client, new Action(this.Connect));
			}
			else
			{
				this.Connect();
			}
			this._reconnectTimer = null;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000389B File Offset: 0x00001A9B
		internal void ResetRetries()
		{
			this._reconnectRetries = 0;
		}

		// Token: 0x0400005C RID: 92
		public bool RequestedReconnect;

		// Token: 0x0400005D RID: 93
		public bool ShouldAttemptResume;

		// Token: 0x0400005E RID: 94
		internal SocketState SocketState = SocketState.Disconnected;

		// Token: 0x0400005F RID: 95
		private Timer _reconnectTimer;

		// Token: 0x04000060 RID: 96
		private int _reconnectRetries;

		// Token: 0x04000061 RID: 97
		private readonly BotClient _client;

		// Token: 0x04000062 RID: 98
		private WebSocket _socket;

		// Token: 0x04000063 RID: 99
		private SocketListener _listener;

		// Token: 0x04000064 RID: 100
		private readonly SocketCommandHandler _commands;

		// Token: 0x04000065 RID: 101
		private readonly ILogger _logger;

		// Token: 0x04000066 RID: 102
		private readonly object _lock = new object();
	}
}
