namespace Carbon;

public static partial class WebControlPanel
{
	public static bool TryFindAccount(string password, out Account account)
	{
		return (account = FindAccount(password)) != null;
	}

	public static Account FindAccount(string password)
	{
		for (int i = 0; i < config.accounts.Length; i++)
		{
			var account = config.accounts[i];
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
	}

	public class Permissions(bool enabled)
	{
		public bool console_view = enabled;
		public bool console_input = enabled;
		public bool chat_view = enabled;
		public bool chat_input = enabled;
		public bool serverinfo = enabled;
		public bool players_view = enabled;
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
			write.WriteObject(serverinfo);
			write.WriteObject(players_view);
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
		ServerInfo,
		PlayersView,
		PlayersInventory,
		EntitiesView,
		EntitiesEdit,
		PermissionsView,
		PermissionsEdit,
	}
}
