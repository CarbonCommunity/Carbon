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

	internal static void RunCommand(ConsoleSystem.Arg arg)
	{
		currentRpcId = arg.GetUInt(0);
		if(!rpcs.TryGetValue(currentRpcId, out WebCall rpc))
		{
			return;
		}
		try
		{
			args[0] = arg;
			arg.ReplyWithObject(((Response)rpc.Method.Invoke(null, args)).WithRpcId(currentRpcId));
		}
		catch(Exception ex)
		{
			Logger.Error($"Failed WebControlPanel.RunCommand", ex.InnerException);
		}
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
					if (connection.Reference is not Account account)
					{
						return false;
					}
					return PermissionType switch
					{
						PermissionTypes.ConsoleView => account.permissions.console_view,
						PermissionTypes.ConsoleInput => account.permissions.console_input,
						PermissionTypes.ChatView => account.permissions.chat_view,
						PermissionTypes.ChatInput => account.permissions.chat_input,
						PermissionTypes.PlayersView => account.permissions.players_view,
						PermissionTypes.PlayersInventory => account.permissions.players_inventory,
						PermissionTypes.EntitiesView => account.permissions.entities_view,
						PermissionTypes.EntitiesEdit => account.permissions.entities_edit,
						PermissionTypes.PermissionsView => account.permissions.permissions_view,
						PermissionTypes.PermissionsEdit => account.permissions.permissions_edit,
						_ => base.Test(connection)
					};
				}
			}
		}
	}
}
