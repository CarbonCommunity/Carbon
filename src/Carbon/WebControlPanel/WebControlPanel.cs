using Facepunch;

namespace Carbon;

public static partial class WebControlPanel
{
	public static Dictionary<uint, WebCall> rpcs = [];
	public static Server server;
	public static ServerMessages serverMessages = new();
	public static Config config;

	private static uint currentRpcId;
	private static object[] args = [1];

	public static void Init()
	{
		LoadConfig();
		rpcs.Clear();
		var methods = typeof(WebControlPanel).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
		for (int i = 0; i < methods.Length; i++)
		{
			var method = methods[i];
			if (method.GetCustomAttribute<WebCall>() is not WebCall rpc)
			{
				continue;
			}
			rpc.Setup(method);
			rpc.Conditions = [.. method.GetCustomAttributes<WebCall.Condition>()];
			rpcs[rpc.MethodId] = rpc;
		}
		Output.OnPostMessage += OnLog;
	}

	public static void Shutdown()
	{
		Output.OnPostMessage -= OnLog;
		if (server == null)
		{
			return;
		}
		server.Shutdown();
		server = null;
	}

	private static void RunRpc(BridgeRead read)
	{
		currentRpcId = read.UInt32();
		if(!rpcs.TryGetValue(currentRpcId, out WebCall rpc))
		{
			return;
		}
		if (rpc.Conditions != null)
		{
			for (int i = 0; i < rpc.Conditions.Length; i++)
			{
				var condition = rpc.Conditions[i];
				if (!condition.Test(read.Connection))
				{
					return;
				}
			}
		}
		try
		{
			args[0] = read;
			rpc.Method.Invoke(null, args);
		}
		catch(Exception ex)
		{
			Logger.Error($"Failed WebControlPanel.RunRpc", ex.InnerException);
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class WebCall : Attribute
	{
		public MethodInfo Method;
		public uint MethodId;
		public Condition[] Conditions;

		public void Setup(MethodInfo method)
		{
			Method = method;
			MethodId = Vault.Pool.Get(method.Name);
		}

		[AttributeUsage(AttributeTargets.Method)]
		public class Condition : Attribute
		{
			public virtual bool Test(BridgeConnection connection)
			{
				return true;
			}

			[AttributeUsage(AttributeTargets.Method)]
			public class Permission(PermissionTypes permission) : Condition
			{
				public PermissionTypes PermissionType = permission;

				public override bool Test(BridgeConnection connection)
				{
					return Account.HasPermission(connection, PermissionType);
				}
			}
		}
	}
}
