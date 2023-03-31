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
    /// <summary>
    /// Represents a websocket that connects to discord
    /// </summary>
    public class Socket
    {
        /// <summary>
        /// If we should attempt to reconnect to discord on disconnect
        /// </summary>
        public bool RequestedReconnect;
        
        /// <summary>
        /// If we should attempt to resume our previous session after connecting
        /// </summary>
        public bool ShouldAttemptResume;
        
        internal SocketState SocketState = SocketState.Disconnected;
        
        /// <summary>
        /// Timer to use when attempting to reconnect to discord due to an error
        /// </summary>
        private Timer _reconnectTimer;
        private int _reconnectRetries;

        private readonly BotClient _client;
        private WebSocket _socket;
        private SocketListener _listener;
        private readonly SocketCommandHandler _commands;
        private readonly ILogger _logger;

        private readonly object _lock = new object();

        /// <summary>
        /// Socket used by the BotClient
        /// </summary>
        /// <param name="client">Client using the socket</param>
        /// <param name="logger">Logger for the bot client</param>
        public Socket(BotClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
            _commands = new SocketCommandHandler(this, logger);
            _listener = new SocketListener(_client, this, _logger, _commands);
        }

        /// <summary>
        /// Connect to the websocket
        /// </summary>
        /// <exception cref="Exception">Thrown if the socket still exists. Must call disconnect before trying to connect</exception>
        public void Connect()
        {
            string url = Gateway.WebsocketUrl;
            
            //We haven't gotten the websocket url. Get url then attempt to connect.
            if (string.IsNullOrEmpty(url))
            {
                Gateway.UpdateGatewayUrl(_client, Connect, _ => Reconnect());
                return;
            }

            lock (_lock)
            {
                if (IsConnected() || IsConnecting())
                {
                    throw new Exception("Socket is already running. Please disconnect before attempting to connect.");
                }

                SocketState = SocketState.Connecting;
            }

            RequestedReconnect = false;
            ShouldAttemptResume = false;

            _socket = new WebSocket(url);

            _socket.SslConfiguration.EnabledSslProtocols |= SslProtocols.Tls12;
            _socket.OnOpen += _listener.SocketOpened;
            _socket.OnClose += _listener.SocketClosed;
            _socket.OnError += _listener.SocketErrored;
            _socket.OnMessage += _listener.SocketMessage;
            _socket.ConnectAsync();
        }

        /// <summary>
        /// Disconnects the websocket from discord
        /// </summary>
        /// <param name="attemptReconnect">Should we attempt to reconnect to discord after disconnecting</param>
        /// <param name="shouldResume">Should we attempt to resume our previous session</param>
        /// <param name="requested">If discord requested that we reconnect to discord</param>
        public void Disconnect(bool attemptReconnect, bool shouldResume, bool requested = false)
        {
            RequestedReconnect = attemptReconnect;
            ShouldAttemptResume = shouldResume;
            _commands.OnSocketDisconnected();

            if (_reconnectTimer != null)
            {
                _reconnectTimer.Stop();
                _reconnectTimer.Dispose();
                _reconnectTimer = null;
            }
            
            lock (_lock)
            {
                if (IsDisconnected())
                {
                    DisposeSocket();
                    return;
                }

                if (requested)
                {
                    _socket.CloseAsync(4199, "Discord Requested Reconnect");
                }
                else
                {
                    _socket.CloseAsync(CloseStatusCode.Normal);
                }

                DisposeSocket();
                SocketState = SocketState.Disconnected;
            }

            if (RequestedReconnect)
            {
                Reconnect();
            }
        }

        /// <summary>
        /// Returns if the given websocket matches our current websocket.
        /// If socket is null we return false
        /// </summary>
        /// <param name="socket">Socket to compare</param>
        /// <returns>True if current socket is not null and socket matches current socket; False otherwise.</returns>
        internal bool IsCurrentSocket(WebSocket socket)
        {
            return _socket != null && _socket == socket;
        }

        /// <summary>
        /// Shutdowns the websocket completely. Used when bot is being shutdown
        /// </summary>
        public void Shutdown()
        {
            Disconnect(false, false);

            _listener?.Shutdown();
            _listener = null;
            _socket = null;
        }

        /// <summary>
        /// Called when a websocket is closed to remove previous socket
        /// </summary>
        public void DisposeSocket()
        {
            if (_socket != null)
            {
                _socket.OnOpen -= _listener.SocketOpened;
                _socket.OnError -= _listener.SocketErrored;
                _socket.OnMessage -= _listener.SocketMessage;
                _socket = null;
            }
        }

        /// <summary>
        /// Send a command to discord over the websocket
        /// </summary>
        /// <param name="opCode">Command code to send</param>
        /// <param name="data">Data to send</param>
        public void Send(GatewayCommandCode opCode, object data)
        {
            CommandPayload payload = new CommandPayload
            {
                OpCode = opCode,
                Payload = data
            };

            _commands.Enqueue(payload);
        }
        
        internal bool Send(CommandPayload payload)
        {
            string payloadData = JsonConvert.SerializeObject(payload, DiscordExtension.ExtensionSerializeSettings);
            if (_logger.IsLogging(DiscordLogLevel.Verbose))
            {
                _logger.Verbose($"{nameof(Socket)}.{nameof(Send)} Payload: {payloadData}");
            }

            if (_socket == null)
            {
                return false;
            }
            
            _socket.SendAsync(payloadData, null);
            return true;
        }

        /// <summary>
        /// Returns if the websocket is in the open state
        /// </summary>
        /// <returns>Returns if the websocket is in open state</returns>
        public bool IsConnected()
        {
            return SocketState == SocketState.Connected;
        }

        /// <summary>
        /// Returns if the websocket is in the connecting state
        /// </summary>
        /// <returns>Returns if the websocket is in connecting state</returns>
        public bool IsConnecting()
        {
            return SocketState == SocketState.Connecting;
        }
        
        /// <summary>
        /// Returns if the socket is waiting to reconnect
        /// </summary>
        /// <returns>Returns if the socket is waiting to reconnect</returns>
        public bool IsPendingReconnect()
        {
            return SocketState == SocketState.PendingReconnect;
        }

        /// <summary>
        /// Returns if the websocket is null or is currently closing / closed
        /// </summary>
        /// <returns>Returns if the websocket is null or is currently closing / closed</returns>
        public bool IsDisconnected()
        {
            return SocketState == SocketState.Disconnected;
        }

        /// <summary>
        /// Reconnects the socket to the gateway.
        /// </summary>
        public void Reconnect()
        {
            if (!_client.Initialized)
            {
                return;
            }

            lock (_lock)
            {
                if (SocketState != SocketState.Disconnected)
                {
                    return;
                }

                SocketState = SocketState.PendingReconnect;
            }
            
            _reconnectRetries++;
            
            //If we haven't had any errors reconnect to the gateway
            if (_reconnectRetries == 1)
            {
                Interface.Oxide.NextTick(Connect);
                return;
            }

            //We had an error trying to reconnect. Perform Delayed Reconnects
            _reconnectTimer = new Timer
            {
                Interval = GetReconnectDelay(),
                AutoReset = false
            };

            _reconnectTimer.Elapsed += ReconnectWebsocket;

            _logger.Warning($"Attempting to reconnect to Discord... [Retry={_reconnectRetries.ToString()}]");
            _reconnectTimer.Start();
        }

        private void ReconnectWebsocket(object sender, ElapsedEventArgs e)
        {
            //There has been more than 3 tries to reconnect. Discord suggests trying to update gateway url.
            if (_reconnectRetries > 3)
            {
                Gateway.UpdateGatewayUrl(_client, Connect, _ => Reconnect());
            }
            else
            {
                Connect();
            }
            _reconnectTimer = null;
        }

        internal void ResetRetries()
        {
            _reconnectRetries = 0;
        }
        
        private int GetReconnectDelay()
        {
            if (_reconnectRetries <= 3) return 1 * 1000;
            if (_reconnectRetries <= 25) return 15 * 1000;
            return 60 * 1000;
        }
    }
}
