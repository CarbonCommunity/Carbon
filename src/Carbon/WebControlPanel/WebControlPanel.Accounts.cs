namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	private static void RPC_AccountPermissions(BridgeRead read)
	{
		if (read.Connection.Reference is not Account { permissions: Permissions permissions })
		{
			return;
		}
		var write = StartRpcResponse();
		permissions.Serialize(write);
		SendRpcResponse(read.Connection, write);
	}

	public static bool TryFindAccount(string password, out Account account)
	{
		if (string.IsNullOrEmpty(password))
		{
			account = null;
			return false;
		}
		return (account = FindAccount(password)) != null;
	}

	public static Account FindAccount(string password)
	{
		for (int i = 0; i < config.WebAccounts.Length; i++)
		{
			var account = config.WebAccounts[i];
			if (account.password == password)
			{
				return account;
			}
		}
		return null;
	}

	public class Account
	{
		public string id;
		public string password;
		public Permissions permissions = new(true);

		public static bool HasPermission(BridgeConnection connection, PermissionTypes permission)
		{
			if (connection.Reference is not Account account)
			{
				return false;
			}
			return permission switch
			{
				PermissionTypes.ConsoleView => account.permissions.console_view,
				PermissionTypes.ConsoleInput => account.permissions.console_input,
				PermissionTypes.ChatView => account.permissions.chat_view,
				PermissionTypes.ChatInput => account.permissions.chat_input,
				PermissionTypes.PlayersView => account.permissions.players_view,
				PermissionTypes.PlayersIp => account.permissions.players_ip,
				PermissionTypes.PlayersInventory => account.permissions.players_inventory,
				PermissionTypes.EntitiesView => account.permissions.entities_view,
				PermissionTypes.EntitiesEdit => account.permissions.entities_edit,
				PermissionTypes.PermissionsView => account.permissions.permissions_view,
				PermissionTypes.PermissionsEdit => account.permissions.permissions_edit,
				_ => false
			};
		}
	}

	public class Permissions(bool enabled)
	{
		public bool console_view = enabled;
		public bool console_input = enabled;
		public bool chat_view = enabled;
		public bool chat_input = enabled;
		public bool players_view = enabled;
		public bool players_ip = enabled;
		public bool players_inventory = enabled;
		public bool entities_view = enabled;
		public bool entities_edit = enabled;
		public bool permissions_view = enabled;
		public bool permissions_edit = enabled;

		public void Serialize(BridgeWrite write)
		{
			write.WriteObject(console_view);
			write.WriteObject(console_input);
			write.WriteObject(chat_view);
			write.WriteObject(chat_input);
			write.WriteObject(players_view);
			write.WriteObject(players_ip);
			write.WriteObject(players_inventory);
			write.WriteObject(entities_view);
			write.WriteObject(entities_edit);
			write.WriteObject(permissions_view);
			write.WriteObject(permissions_edit);
		}
	}

	public enum PermissionTypes
	{
		None,
		ConsoleView,
		ConsoleInput,
		ChatView,
		ChatInput,
		PlayersView,
		PlayersIp,
		PlayersInventory,
		EntitiesView,
		EntitiesEdit,
		PermissionsView,
		PermissionsEdit,
	}
}
