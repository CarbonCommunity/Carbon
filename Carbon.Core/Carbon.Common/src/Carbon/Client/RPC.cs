using System.Diagnostics;
using Carbon.Client.Packets;
using Network;
using Debug = UnityEngine.Debug;

namespace Carbon.Client;

public struct RPC
{
	public uint Id;
	public string Name;

	public static readonly bool SERVER = typeof(LocalPlayer).GetMethod("OnInventoryChanged") == null;

	public const string DOMAIN = "carbon.com.";

	public static List<RPC> rpcList = new();

	internal static Dictionary<uint, Func<BasePlayer, Network.Message, object>> _cache = new();
	internal static object[] _argBuffer = new object[2];

	public static implicit operator string(RPC rpc)
	{
		return rpc.Name;
	}
	public static implicit operator uint(RPC rpc)
	{
		return rpc.Id;
	}

	public static void Init()
	{
		Init(typeof(RPC).Assembly.GetTypes());
	}
	public static void Init(params Type[] types )
	{
		foreach (var type in types)
		{
			foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
			{
				var attr = method.GetCustomAttribute<Method>();
				if (attr == null) continue;

				var name = $"{DOMAIN}{attr.Id}";
				var id = StringPool.Add(name);
#if DEBUG
				Console.WriteLine($"Registed client RPC '{name}[{id}]'");
#endif
				rpcList.Add(new RPC
				{
					Id = id,
					Name = name
				});

				_cache.Add(id, (player, msg) =>
				{
					_argBuffer[0] = player;
					_argBuffer[1] = msg;

					var result = (object)null;

					try
					{
						result = method.Invoke(null, _argBuffer);
					}
					catch (Exception ex)
					{
						ex = (ex.InnerException ?? ex).Demystify();

						var rpc = RPC.Get(id);
						Console.WriteLine($"Failed executing RPC call {rpc.Name}[{rpc.Id}] ({ex.Message})\n{(ex.InnerException ?? ex).Demystify().StackTrace}");
					}

					return result;
				});
			}
		}

		Get("ping");
		Get("pong");
	}
	public static RPC Get(string name)
	{
		name = $"{DOMAIN}{name}";

		return new RPC
		{
			Id = StringPool.Add(name),
			Name = name
		};
	}
	public static RPC Get(uint id)
	{
		return new RPC
		{
			Id = id,
			Name = StringPool.Get(id)
		};
	}

	public static object HandleRPCMessage(BasePlayer player, uint rpc, Network.Message message)
	{
		if (_cache.TryGetValue(rpc, out var value))
		{
			return value(player, message);
		}

		return null;
	}

	[Method("pong")]
	private static void Pong(BasePlayer player, Network.Message message)
	{
		var client = CarbonClient.Get(player);

		if (client.HasCarbonClient)
		{
			Logger.Warn($"Player '{player.Connection}' attempted registering twice.");
			return;
		}

		var result = CarbonClient.Receive<RPCList>(message);
		result.Sync();
		result.Dispose();

		client.HasCarbonClient = true;
		client.Send(RPC.Get("clientinfo"));
		Logger.Log($"{client.Connection} joined with Carbon client");
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class Method : Attribute
	{
		public string Id { get; set; }

		public Method(string id)
		{
			Id = id;
		}
	}
}
