using Facepunch;

namespace Carbon;

public static partial class WebControlPanel
{
	public static Dictionary<uint, RpcAttribute> rpcs = [];

	internal static object[] args = [1];

	internal static void Init()
	{
		LoadConfig();
		rpcs.Clear();
		var methods = typeof(WebControlPanel).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
		for (int i = 0; i < methods.Length; i++)
		{
			var method = methods[i];
			if (method.GetCustomAttribute<RpcAttribute>() is not RpcAttribute rpc)
			{
				continue;
			}
			rpc.Setup(method);
			rpcs[rpc.MethodId] = rpc;
		}
	}

	internal static void Run(ConsoleSystem.Arg arg)
	{
		var rpcId = arg.GetUInt(0);
		if(!rpcs.TryGetValue(rpcId, out RpcAttribute rpc))
		{
			return;
		}
		try
		{
			args[0] = arg;
			arg.ReplyWithObject(((Response)rpc.Method.Invoke(null, args)).WithRpcId(rpcId));
		}
		catch(Exception ex)
		{
			Logger.Error($"Failed WebRCon.Run", ex);
		}
	}

	public static Response GetResponse(object value = null)
	{
		Response response = default;
		response.value = value;
		return response.WithValue(value);
	}

	public struct Response
	{
		public uint rpcId;
		public object value;

		public Response WithRpcId(uint rpcId)
		{
			this.rpcId = rpcId;
			return this;
		}
		public Response WithValue(object value)
		{
			this.value = value;
			return this;
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class RpcAttribute : Attribute
	{
		public MethodInfo Method;
		public uint MethodId;

		public void Setup(MethodInfo method)
		{
			Method = method;
			MethodId = Vault.Pool.Get(method.Name);
		}
	}
}
