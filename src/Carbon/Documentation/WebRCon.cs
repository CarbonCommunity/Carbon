namespace Carbon.Documentation;

public static partial class WebRCon
{
	private static object[] args = [1];
	private static Dictionary<uint, DocsRpcAttribute> rpcs = [];

	internal static void Initialize()
	{
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
			rpc.Method.Invoke(null, args);
		}
		catch(Exception ex)
		{
			Logger.Error($"Failed WebRCon.Run", ex);
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
