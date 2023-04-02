using System.Timers;
using Oxide.Core;
using Oxide.Ext.Discord.Constants;
using Oxide.Ext.Discord.Logging;

namespace Oxide.Ext.Discord.WebSockets.Handlers
{
    /// <summary>
    /// Handles the heartbeating for the websocket connection
    /// </summary>
    public class HeartbeatHandler
    {
        /// <summary>
        /// Discord Acknowledged our heartbeat successfully 
        /// </summary>
        public bool HeartbeatAcknowledged;
        
        private readonly BotClient _client;
        private readonly Socket _socket;
        private readonly SocketListener _listener;
        private readonly ILogger _logger;
        private Timer _timer;
        private float _interval;
        private bool _initial;

        /// <summary>
        /// Constructor for Heartbeat Handler
        /// </summary>
        /// <param name="client">Client for the handler</param>
        /// <param name="socket">Socket for the heartbeat</param>
        /// <param name="listener">Socket Listener for the client</param>
        /// <param name="logger">Logger for the bot</param>
        public HeartbeatHandler(BotClient client, Socket socket,  SocketListener listener, ILogger logger)
        {
            _client = client;
            _socket = socket;
            _listener = listener;
            _logger = logger;
        }
        
        #region Heartbeat
        /// <summary>
        /// Setup a heartbeat for this bot with the given interval
        /// </summary>
        /// <param name="interval"></param>
        internal void SetupHeartbeat(float interval)
        {
            if (_timer != null)
            {
                _logger.Debug($"{nameof(HeartbeatHandler)}.{nameof(SetupHeartbeat)} Previous heartbeat timer exists.");
                DestroyHeartbeat();
            }
            
            HeartbeatAcknowledged = true;
            _interval = interval;
            _initial = true;
            _timer = new Timer(_interval * Random.Range(0f, 1f));
            _timer.Elapsed += HeartbeatElapsed;
            _timer.Start();
            _logger.Debug($"{nameof(HeartbeatHandler)}.{nameof(SetupHeartbeat)} Creating heartbeat with interval {interval.ToString()}ms.");
            _client.CallHook(DiscordExtHooks.OnDiscordSetupHeartbeat, interval);
        }

        /// <summary>
        /// Destroy the heartbeat on this bot
        /// </summary>
        public void DestroyHeartbeat()
        {
            if(_timer != null)
            {
                _logger.Debug($"{nameof(HeartbeatHandler)}.{nameof(DestroyHeartbeat)} Destroy Heartbeat");
                _timer.Dispose();
                _timer = null;
            }
        }

        private void HeartbeatElapsed(object sender, ElapsedEventArgs e)
        {
            _logger.Debug($"{nameof(HeartbeatHandler)}.{nameof(HeartbeatElapsed)} Heartbeat Elapsed");

            if (_initial)
            {
                _timer.Interval = _interval;
                _initial = false;
            }

            if (!_listener.SocketHasConnected)
            {
                _logger.Debug($"{nameof(HeartbeatHandler)}.{nameof(HeartbeatElapsed)} Websocket has not yet connected successfully. Skipping Heartbeat.");
                return;
            }
            
            if (_socket.IsPendingReconnect())
            {
                _logger.Debug($"{nameof(HeartbeatHandler)}.{nameof(HeartbeatElapsed)} Websocket is offline and is waiting to connect.");
                return;
            }

            if (_socket.IsDisconnected())
            {
                _logger.Debug($"{nameof(HeartbeatHandler)}.{nameof(HeartbeatElapsed)} Websocket is offline and is NOT connecting. Attempt Reconnect.");
                _socket.Reconnect();
                return;
            }
            
            if(!HeartbeatAcknowledged)
            {
                //Discord did not acknowledge our last sent heartbeat. This is a zombie connection we should reconnect with non 1000 close code.
                if (_socket.IsConnected())
                {
                    _logger.Debug($"{nameof(HeartbeatHandler)}.{nameof(HeartbeatElapsed)} Heartbeat Elapsed and WebSocket is connected. Forcing reconnect.");
                    _socket.Disconnect(true, true, true);
                    return;
                }

                //Websocket isn't connected or waiting to reconnect. We should reconnect.
                if (!_socket.IsConnecting() && !_socket.IsPendingReconnect())
                {
                    _logger.Debug($"{nameof(HeartbeatHandler)}.{nameof(HeartbeatElapsed)} Heartbeat Elapsed and bot is not online or connecting.");
                    _socket.Reconnect();
                    return;
                }

                _logger.Debug($"{nameof(HeartbeatHandler)}.{nameof(HeartbeatElapsed)} Heartbeat Elapsed and bot is not online but is waiting to connecting or waiting to reconnect.");
                return;
            }
            
            SendHeartbeat();
        }
        
        /// <summary>
        /// Sends a heartbeat to discord.
        /// If the previous heartbeat wasn't acknowledged then we will attempt to reconnect
        /// </summary>
        public void SendHeartbeat()
        {
            HeartbeatAcknowledged = false;
            _listener.SendHeartbeat();
            _client.CallHook(DiscordExtHooks.OnDiscordHeartbeatSent);
            _logger.Debug($"{nameof(HeartbeatHandler)}.{nameof(SendHeartbeat)} Heartbeat sent - {_timer.Interval.ToString()}ms interval.");
        }
        #endregion
    }
}