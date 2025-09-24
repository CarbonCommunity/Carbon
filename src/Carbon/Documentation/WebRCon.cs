using Facepunch;

namespace Carbon.Documentation;

public static partial class WebRCon
{
	public static Dictionary<uint, DocsRpcAttribute> rpcs = [];

	internal static object[] args = [1];

	internal static void Init()
	{
		LoadConfig();
		rpcs.Clear();
		var methods = typeof(WebRCon).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
		for (int i = 0; i < methods.Length; i++)
		{
			var method = methods[i];
			if (method.GetCustomAttribute<DocsRpcAttribute>() is not DocsRpcAttribute rpc)
			{
				continue;
			}
			rpc.Setup(method);
			rpcs[rpc.MethodId] = rpc;
		}
		StartServer();
	}

	internal static void Run(ConsoleSystem.Arg arg)
	{
		var rpcId = arg.GetUInt(0);
		if(!rpcs.TryGetValue(rpcId, out DocsRpcAttribute rpc))
		{
			return;
		}
		try
		{
			args[0] = arg;

			var response = rpc.Method.Invoke(null, args) as DocsRpcResponse;
			response.RpcId = rpcId;
			arg.ReplyWithObject(response);
			Pool.Free(ref response);
		}
		catch(Exception ex)
		{
			Logger.Error($"Failed WebRCon.Run", ex);
		}
	}

	public static DocsRpcResponse Response(object value = null)
	{
		var response = Pool.Get<DocsRpcResponse>();
		response.Value = value;
		return response;
	}

	public class DocsRpcResponse : Pool.IPooled
	{
		public uint RpcId;
		public object Value;

		public void EnterPool()
		{
			RpcId = 0;
			Value = null;
		}
		public void LeavePool()
		{
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class DocsRpcAttribute : Attribute
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
