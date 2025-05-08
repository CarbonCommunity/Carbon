using System.Net.WebSockets;
using Facepunch;
using Facepunch.Rcon;
using Facepunch.Rust.Profiling;
using Fleck;
using Network;

namespace Carbon.Components;

public class Bridge
{
	public static readonly int MAX_BUFFER_SIZE = 8192;

	public static void Send(NetWrite write)
	{
		if (RCon.listenerNew == null || RCon.listenerNew.server == null)
		{
			return;
		}

		var dictionary = RCon.listenerNew.clients;
		lock (dictionary)
		{
			RCon.listenerNew.deadClients.Clear();
			foreach (var client in RCon.listenerNew.clients)
			{
				if (client.Value.Socket.IsAvailable)
				{
					var buffer = write.GetBuffer();
					client.Value.Socket.Send(new MemoryBuffer(buffer.Buffer, buffer.Length, false));
					RconConnection value = client.Value;
					value.Stats.BroadcastedMessages += 1;
				}
				else
				{
					RCon.listenerNew.deadClients.Add(client.Key);
				}
			}

			foreach (int num in RCon.listenerNew.deadClients)
			{
				if (RCon.listenerNew.clients.TryGetValue(num, out var rconConnection))
				{
					rconConnection.Socket.Close();
					RCon.listenerNew._subscribedRconClients.Remove(num);
					RCon.listenerNew.clients.Remove(num);
				}
			}

			RconProfiler.UpdateClientCount(RCon.listenerNew.clients.Count);
		}
	}
}

public class BridgeConnection(string connectionString)
{
	public event EventHandler<NetRead> OnRead;

	private readonly Uri uri = new(connectionString);
	private ClientWebSocket socket;
	private CancellationTokenSource tokenSource;

	public BridgeConnection(string ip, string port, string password) : this($"ws://{ip}:{port}/{Vault.ApplyReplacement(password) ?? password}") { }

	public async ValueTask Connect()
	{
		socket = new ClientWebSocket();
		tokenSource = new CancellationTokenSource();

		try
		{
			await socket.ConnectAsync(uri, tokenSource.Token);
			Task.Run(async () => await ReceiveLoop(tokenSource.Token));
		}
		catch (Exception ex)
		{
			Logger.Error($"Carbon.Bridge connection attempt to '{connectionString}' failed", ex);
		}
	}

	public async ValueTask Disconnect()
	{
		try
		{
			if (socket?.State == WebSocketState.Open)
			{
				await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", tokenSource.Token);
			}
		}
		finally
		{
			tokenSource?.Cancel();
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
							OnRead?.Invoke(result, read);
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
}
