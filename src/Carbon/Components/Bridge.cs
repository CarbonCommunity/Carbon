using System.Net.WebSockets;
using Fleck;
using Network;
using Facepunch;
using Facepunch.Rcon;

namespace Carbon.Components;

/// <summary>
/// A self-managing bridge system dedicated to allow server owners and plugin developers communicate between the server network, directly.
/// It uses Rust's network reading/writing with the only difference being, this allows you to communicate with one or multiple servers at once.
/// </summary>
public static class Bridge
{
	public static BridgeServer Server = new();

	/// <summary>
	/// Opens the connection to a local or external Bridge server. The other server must have a Bridge.Server initialized. Think of it RCon but not really.
	/// </summary>
	/// <param name="ip">Bridge localhost or external IP address</param>
	/// <param name="port">Bridge server port</param>
	/// <param name="password">Bridge server password</param>
	/// <param name="messages">Bridge messages set of channels</param>
	/// <param name="maxBufferSize">Bridge maximum buffer size. Expand or lower if you know what you're doing, depending on the types of data sizes you work with.</param>
	/// <returns></returns>
	public static async ValueTask<BridgeClient> StartClient(string ip, int port, string password, BridgeMessages messages, int maxBufferSize = 8192)
	{
		return await new BridgeClient().Connect(ip, port, password, messages, maxBufferSize);
	}
}

/// <summary>
/// Meant to be overriden to handle custom logic. Use BridgeRead.Connection.Send to respond back using BridgeWrite.
/// </summary>
public abstract class BridgeMessages
{
	protected abstract void OnRpc(BridgeRead read);
	protected abstract void OnCommand(BridgeRead read);
	protected abstract void OnCustom(BridgeRead read);
	protected abstract void OnUnhandled(BridgeRead read);

	public void HandleChannelRead(BridgeRead read)
	{
		switch (read.BridgeMessage())
		{
			case Channels.Rpc:
				OnRpc(read);
				break;
			case Channels.Command:
				OnCommand(read);
				break;
			case Channels.Custom:
				OnCustom(read);
				break;
			default:
				OnUnhandled(read);
				break;
		}
	}

	public enum Channels
	{
		Rpc,
		Command,
		Custom
	}
}

/// <summary>
/// A default message set for the bridge events and channels.
/// </summary>
public class DefaultBridgeMessages : BridgeMessages
{
	protected override void OnRpc(BridgeRead read)
	{
	}

	protected override void OnCommand(BridgeRead read)
	{
	}

	protected override void OnCustom(BridgeRead read)
	{
	}

	protected override void OnUnhandled(BridgeRead read)
	{
	}
}

/// <summary>
/// At its core, it uses Fleck (AKA Facepunch RCon's listener). It's entirely independent to Rust's RCon system, it just uses the core components at base (connection and memory management, etc.).
/// </summary>
public sealed class BridgeServer
{
	public Listener Listener;
	public Action<BridgeConnection> OnNewConnection;
	public Action<BridgeConnection> OnClosedConnection;
	public BridgeMessages Messages;
	public readonly Dictionary<int, BridgeConnection> Connections = [];

	public void Start(int port, string password, string ip = null, BridgeMessages messages = null)
	{
		if (password is null or "unset" or "password")
		{
			return;
		}

		Messages = messages ?? new DefaultBridgeMessages();

		Listener = new Listener();
		if (!string.IsNullOrEmpty(ip))
		{
			Listener.Address = ip;
		}

		Listener.Password = Vault.ApplyReplacement(password) ?? password;
		Listener.Port = port;
		Listener.Start();
		Listener.server._config = socket =>
		{
			lock (Listener.clients)
			{
				if (socket.ConnectionInfo.Path != $"/{Listener.Password}")
				{
					socket.Close();
				}
				else
				{
					var connectionId = Interlocked.Increment(ref Listener.nextClientId);
					var bridgeConnection = Pool.Get<BridgeConnection>().Init(socket, Messages);
					socket.OnOpen = () =>
					{
						lock (Listener.clients)
						{
							Listener.clients.Add(connectionId, new RconConnection(socket, connectionId));
							Connections[connectionId] = bridgeConnection;
							OnNewConnection?.Invoke(bridgeConnection);
						}
					};
					socket.OnClose = () =>
					{
						lock (Listener.clients)
						{
							Listener._subscribedRconClients.Remove(connectionId);
							Listener.clients.Remove(connectionId);
							if (Connections.TryGetValue(connectionId, out var bridgeConnection))
							{
								Pool.Free(ref bridgeConnection);
								Connections.Remove(connectionId);
							}

							OnClosedConnection?.Invoke(bridgeConnection);
						}
					};
					socket.OnBinary += data =>
					{
						var buffer = BufferStream.RentBuffer(data.Length);
						using var stream = Pool.Get<BufferStream>().Initialize();
						stream._buffer = buffer;
						stream._length = buffer.Length;
						stream._isBufferOwned = true;
						for (var i = 0; i < data.Length; i++)
						{
							stream._buffer[i] = data[i];
						}
						var read = BridgeRead.Rent(stream, bridgeConnection);
						Messages.HandleChannelRead(read);
						BridgeRead.Return(ref read);
					};
					socket.OnError = e =>
					{
						Logger.Error("Socket failure", e);
					};
				}
			}
		};
		Logger.Log($"Carbon.Bridge Started on {port}");
	}

	public void Shutdown()
	{
		Listener.Shutdown();
		Listener = null;
		OnNewConnection = null;
		OnClosedConnection = null;
		Messages = null;
	}
}

/// <summary>
/// A simple client websocket connector. Designed to connect to BridgeServer infrastructure and easily understand/communicate Bridge messages back and forth.
/// </summary>
public sealed class BridgeClient
{
	public ClientWebSocket Socket;
	public CancellationTokenSource CancellationToken;
	public BridgeMessages Messages;
	public int MaxBufferSize;

	public async ValueTask<BridgeClient> Connect(string ip, int port, string password, BridgeMessages messages, int maxBufferSize = 8192)
	{
		MaxBufferSize = maxBufferSize;
		Messages = messages;
		Socket = new ClientWebSocket();
		CancellationToken = new CancellationTokenSource();

		try
		{
			await Socket.ConnectAsync(new Uri($"ws://{ip}:{port}/{Vault.ApplyReplacement(password) ?? password}"), CancellationToken.Token);

			Task.Run(async () => await ReceiveLoop());
		}
		catch (Exception ex)
		{
			Logger.Error($"Carbon.Bridge Client connection attempt to '{ip}:{port}' failed", ex);
		}

		return this;
	}

	public async ValueTask Send(BridgeWrite write)
	{
		await Socket.SendAsync(new ArraySegment<byte>(write.GetBuffer().Buffer), WebSocketMessageType.Binary, true, CancellationToken.Token);
	}

	public async ValueTask Disconnect()
	{
		await Socket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown", CancellationToken.Token)!;

		CancellationToken.Cancel();
		CancellationToken = null;
		Socket?.Dispose();
		Socket = null;
	}

	private async ValueTask ReceiveLoop()
	{
		while (Socket.State == WebSocketState.Open && !CancellationToken.IsCancellationRequested)
		{
			using var stream = Pool.Get<BufferStream>().Initialize();
			stream._isBufferOwned = true;
			stream._buffer = BufferStream.RentBuffer(MaxBufferSize);
			stream._length = stream._buffer.Length;
			stream._position = 0;
			try
			{
				var result = (WebSocketReceiveResult)null;
				do result = await Socket.ReceiveAsync(new ArraySegment<byte>(stream._buffer), CancellationToken.Token);
				while (!result.EndOfMessage);

				switch (result.MessageType)
				{
					case WebSocketMessageType.Binary:
						var read = BridgeRead.Rent(stream);
						read.stream = stream;
						try
						{
							Messages.HandleChannelRead(read);
						}
						catch (Exception ex)
						{
							Logger.Error("Carbon.Bridge.ReceiveLoop[OnRead] failure", ex);
						}
						BridgeRead.Return(ref read);
						break;

					case WebSocketMessageType.Close:
						await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed", CancellationToken.Token);
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Carbon.Bridge.ReceiveLoop failure", ex);
			}
		}

		await Disconnect();
	}
}

/// <summary>
/// Base Bridge.Server connection. It uses pooling as connections can turn on and off frequently so we're reusing memory up in here. Use BridgeConnection.Send to respond back to the other Bridge server.
/// </summary>
public sealed class BridgeConnection : Pool.IPooled
{
	public IWebSocketConnection Socket;
	public BridgeMessages Messages;

	public BridgeConnection Init(IWebSocketConnection connection, BridgeMessages messages)
	{
		this.Socket = connection;
		this.Messages = messages;
		return this;
	}

	public void Send(BridgeWrite write)
	{
		Socket.Send(write.GetMemory());
	}

	public void EnterPool()
	{
		Messages = null;
		Socket = null;
	}
	public void LeavePool()
	{

	}
}

/// <summary>
/// A small wrapper of Rust's NetRead. Use BridgeRead.Rent and BridgeRead.Return to properly initialize it.
/// </summary>
public sealed class BridgeRead : NetRead
{
	public BridgeConnection Connection;

	public static BridgeRead Rent(BufferStream stream, BridgeConnection conn = null)
	{
		var read = Pool.Get<BridgeRead>();
		read.Init(stream, conn);
		return read;
	}

	public static void Return(ref BridgeRead read)
	{
		read.stream = null;
		Pool.Free(ref read);
	}

	public void Init(BufferStream stream, BridgeConnection conn = null)
	{
		this.stream = stream;
		this.Connection = conn;
	}

	public BridgeMessages.Channels PeekBridgeMessage() => Peek<BridgeMessages.Channels>();

	public BridgeMessages.Channels BridgeMessage() => (BridgeMessages.Channels)Int32();

	public new void EnterPool()
	{
		Connection = null;
		base.EnterPool();
	}
}

/// <summary>
/// A small wrapper of Rust's NetWrite. Use BridgeWrite.Rent and BridgeWrite.Return to properly initialize it.
/// </summary>
public sealed class BridgeWrite : NetWrite
{
	public static BridgeWrite Rent()
	{
		var write = Pool.Get<BridgeWrite>();
		write.Start(Net.sv);
		return write;
	}

	public static void Return(ref BridgeWrite write)
	{
		Pool.Free(ref write);
	}

	public MemoryBuffer GetMemory(bool fromPool = false)
	{
		var buffer = GetBuffer();
		return new MemoryBuffer(buffer.Buffer, buffer.Length, fromPool);
	}

	public void BridgeMessage(BridgeMessages.Channels message)
	{
		Int32((int)message);
	}
}
