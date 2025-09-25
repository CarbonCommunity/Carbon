namespace Carbon;

public static partial class WebControlPanel
{
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
	}
}
