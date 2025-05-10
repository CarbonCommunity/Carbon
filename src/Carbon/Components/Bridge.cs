using System.Net.WebSockets;
using Fleck;
using Network;
using Facepunch;
using Facepunch.Rcon;

namespace Carbon.Components;

public static class Bridge
{
	public static BridgeServer Server = new();

	public static async ValueTask<BridgeClient> StartClient(string ip, int port, string password, Action<BridgeRead> onRead, int maxBufferSize = 8192)
	{
		var client = new BridgeClient();
		await client.Connect(ip, port, password, onRead, maxBufferSize);
		return client;
	}
}

public class BridgeServer
{
	public Listener Listener;
	public Action<BridgeConnection> OnNewConnection;
	public Action<BridgeConnection> OnClosedConnection;
	public readonly Dictionary<int, BridgeConnection> Connections = [];

	public void Start(int port, string password, string ip = null)
	{
		if (password is null or "unset")
		{
			return;
		}

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
					var bridgeConnection = Pool.Get<BridgeConnection>().Init(socket);
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

						var read = BridgeRead.Rent(bridgeConnection, stream);
						bridgeConnection.OnRead(read);
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
}

public class BridgeClient
{
	public ClientWebSocket Socket;
	public CancellationTokenSource CancellationToken;
	public Action<BridgeRead> OnRead;
	public int MaxBufferSize;

	public async ValueTask Connect(string ip, int port, string password, Action<BridgeRead> onRead, int maxBufferSize = 8192)
	{
		MaxBufferSize = maxBufferSize;
		OnRead = onRead;
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
	}
	public async ValueTask Disconnect()
	{
		await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown", CancellationToken.Token);

		CancellationToken.Cancel();
		CancellationToken = null;
		Socket.Dispose();
		Socket = null;
		OnRead = null;
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
						var read = BridgeRead.Rent(null, stream);
						read.stream = stream;
						try
						{
							OnRead?.Invoke(read);
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
	}
}

public class BridgeConnection : Pool.IPooled
{
	public IWebSocketConnection Socket;
	public Action<BridgeRead> OnRead;

	public BridgeConnection Init(IWebSocketConnection connection)
	{
		this.Socket = connection;
		this.OnRead = OnRead;
		return this;
	}

	public void Send(BridgeWrite write)
	{
		Socket.Send(write.GetMemory());
	}

	public void EnterPool()
	{
		OnRead = null;
		Socket = null;
	}

	public void LeavePool()
	{

	}
}

public enum BridgeMessages
{
	Handshake,
	Rpc
}

public sealed class BridgeRead : NetRead
{
	public BridgeConnection bridgeConnection;

	public static BridgeRead Rent(BridgeConnection conn, BufferStream stream)
	{
		var read = Pool.Get<BridgeRead>();
		read.Init(conn, stream);
		return read;
	}

	public static void Return(ref BridgeRead read)
	{
		read.stream = null;
		Pool.Free(ref read);
	}

	public void Init(BridgeConnection conn, BufferStream stream)
	{
		this.stream = stream;
		this.bridgeConnection = conn;
	}

	public BridgeMessages PeekBridgeMessage() => Peek<BridgeMessages>();

	public BridgeMessages BridgeMessage() => (BridgeMessages)Int32();

	public new void EnterPool()
	{
		bridgeConnection = null;
		base.EnterPool();
	}
}

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

	public void BridgeMessage(BridgeMessages message)
	{
		Int32((int)message);
	}
}
