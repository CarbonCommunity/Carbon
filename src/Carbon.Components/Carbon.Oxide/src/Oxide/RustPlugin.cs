namespace Oxide.Plugins;

/// <summary>
/// Oxide's plugin base. Everything generic comes from <see cref="CarbonPlugin"/>;
/// this only adds the Oxide-only libraries and helpers.
/// </summary>
public class RustPlugin : CarbonPlugin
{
	public Server server;
	public Oxide.Core.Libraries.Plugins plugins;
	public OxideMod mod;
	public Oxide.Game.Rust.Libraries.Rust rust;
	public Covalence covalence;

	public PluginManager Manager { get; set; }

	public Player Player => rust.Player;
	public Server Server => rust.Server;

	public override void Setup(string name, string author, VersionNumber version, string description)
	{
		base.Setup(name, author, version, description);

		Manager = Interface.Oxide.RootPluginManager ?? new PluginManager();
		server = new Server();
		plugins = new Oxide.Core.Libraries.Plugins(Manager);
		mod = Interface.Oxide;
		rust = new Oxide.Game.Rust.Libraries.Rust();
		covalence = new Covalence();
	}

	public virtual void HandleAddedToManager(PluginManager manager) { }
	public virtual void HandleRemovedFromManager(PluginManager manager) { }

	public static T GetLibrary<T>(string name = null) where T : class
	{
		return Interface.Oxide.GetLibrary<T>(name);
	}

	#region Covalence

	private void RegisterPermissions(IEnumerable<string> perms)
	{
		if (perms == null)
		{
			return;
		}

		foreach (var perm in perms)
		{
			if (!string.IsNullOrEmpty(perm) && !permission.PermissionExists(perm))
			{
				permission.RegisterPermission(perm, this);
			}
		}
	}

	protected void AddCovalenceCommand(string command, string callback, params string[] perms)
	{
		cmd.AddCommand(command, this, callback, permissions: perms);
		RegisterPermissions(perms);
	}

	protected void AddCovalenceCommand(string[] commands, string callback, string perm)
	{
		AddCovalenceCommand(commands, callback, string.IsNullOrEmpty(perm) ? [] : [perm]);
	}

	protected void AddCovalenceCommand(string[] commands, string callback, params string[] perms)
	{
		foreach (var command in commands)
		{
			cmd.AddCommand(command, this, callback, permissions: perms);
		}

		RegisterPermissions(perms);
	}

	protected void AddUniversalCommand(string command, string callback, params string[] perms)
		=> AddCovalenceCommand(command, callback, perms);

	protected void AddUniversalCommand(string[] commands, string callback, params string[] perms)
		=> AddCovalenceCommand(commands, callback, perms);

	#endregion
}
