using System.Net.WebSockets;
using Facepunch;
using Network;

namespace Carbon.Components;

public class Bridge
{
	public static readonly int MAX_BUFFER_SIZE = 8192;
	public static readonly Dictionary<uint, BridgeConnection> Connections = [];

	public static BridgeConnection BeginConnection(string connectionString, Action<NetRead> onReceived)
	{
		if (string.IsNullOrEmpty(connectionString))
		{
			return null;
		}
		if (Connections.TryGetValue(Vault.Pool.Get(connectionString), out var connection))
		{
			return connection;
		}
		connection = Pool.Get<BridgeConnection>();
		connection.Init(connectionString, onReceived);
		return connection;
	}
	public static BridgeConnection BeginConnection(string ip, string port, string password, Action<NetRead> onReceive)
	{
		return BeginConnection($"ws://{ip}:{port}/{Vault.ApplyReplacement(password) ?? password}", onReceive);
	}

	public static void Broadcast(NetWrite write)
	{
		foreach (var connection in Connections.Values)
		{
			_ = connection.Send(write);
		}
	}
}

public class BridgeConnection : Pool.IPooled
{
	private Uri uri;
	private ClientWebSocket socket;
	private event Action<NetRead> onReceived;
	private CancellationTokenSource tokenSource;
	private string connectionString;

	public BridgeConnection Init(string connectionString, Action<NetRead> onReceive)
	{
		uri = new Uri(this.connectionString = connectionString);
		this.onReceived = onReceive;
		return this;
	}
	public BridgeConnection Init(string ip, string port, string password, Action<NetRead> onReceive)
	{
		return Init($"ws://{ip}:{port}/{Vault.ApplyReplacement(password) ?? password}", onReceive);
	}

	public async ValueTask Connect()
	{
		socket = new ClientWebSocket();
		tokenSource = new CancellationTokenSource();

		try
		{
			await socket.ConnectAsync(uri, tokenSource.Token);

			Bridge.Connections[Vault.Pool.Get(connectionString)] = this;

			Task.Run(async () => await ReceiveLoop(tokenSource.Token));
		}
		catch (Exception ex)
		{
			Logger.Error($"Carbon.Bridge connection attempt to '{connectionString}' failed", ex);
		}
	}
	public async ValueTask Disconnect(bool sendToPool = true)
	{
		try
		{
			Bridge.Connections.Remove(Vault.Pool.Get(connectionString));
			if (socket?.State == WebSocketState.Open)
			{
				await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", tokenSource.Token);
			}
		}
		finally
		{
			socket?.Dispose();
			socket = null;
			tokenSource?.Cancel();
			tokenSource = null;

			if (sendToPool)
			{
				var self = this;
				Pool.Free(ref self);
			}
		}
	}
	public async ValueTask Send(NetWrite stream)
	{
		if (socket?.State != WebSocketState.Open)
		{
			return;
		}
		try
		{
			await socket.SendAsync(stream.stream.GetBuffer(), WebSocketMessageType.Binary, true, tokenSource.Token);
		}
		catch (Exception ex)
		{
			Logger.Error($"Carbon.Bridge.Send exception", ex);
		}
	}
	private async ValueTask ReceiveLoop(CancellationToken token)
	{
		while (socket.State == WebSocketState.Open && !token.IsCancellationRequested)
		{
			using var stream = Pool.Get<BufferStream>().Initialize();
			stream._isBufferOwned = true;
			stream._buffer = BufferStream.RentBuffer(Bridge.MAX_BUFFER_SIZE);
			stream._length = stream._buffer.Length;
			stream._position = 0;
			try
			{
				var result = (WebSocketReceiveResult)null;
				do result = await socket.ReceiveAsync(new ArraySegment<byte>(stream._buffer), token);
				while (!result.EndOfMessage);

				switch (result.MessageType)
				{
					case WebSocketMessageType.Binary:
						var read = Pool.Get<NetRead>();
						read.stream = stream;
						try
						{
							onReceived?.Invoke(read);
						}
						catch (Exception ex)
						{
							Logger.Error("Carbon.Bridge.ReceiveLoop[OnRead] failure", ex);
						}

						Pool.Free(ref read);
						break;

					case WebSocketMessageType.Close:
						await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed", token);
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Carbon.Bridge.ReceiveLoop failure", ex);
			}
		}
	}

	public void EnterPool()
	{
		onReceived = null;
		uri = null;
		if (socket is { State: WebSocketState.Open })
		{
			socket.Dispose();
		}
		socket = null;
		tokenSource?.Cancel();
		tokenSource = null;
		connectionString = null;
	}
	public void LeavePool()
	{

	}
}
