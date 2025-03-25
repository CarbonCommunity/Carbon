using Logger = Carbon.Logger;
using Player = Oxide.Game.Rust.Libraries.Player;

namespace Oxide.Plugins;

public class RustPlugin : Plugin
{
	public bool IsPrecompiled { get; set; }
	public bool IsExtension { get; set; }

	public Lang lang;
	public Server server;
	public Oxide.Core.Libraries.Plugins plugins;
	public Timers timer;
	public OxideMod mod;
	public WebRequests webrequest;
	public Oxide.Game.Rust.Libraries.Rust rust;
	public Covalence covalence;

	public Player Player { get { return rust.Player; } private set { } }
	public Server Server { get { return rust.Server; } private set { } }

	public virtual void SetupMod(ModLoader.Package mod, string name, string author, VersionNumber version, string description)
	{
		Package = mod;
		Setup(name, author, version, description);
	}

	public virtual void Setup(string name, string author, VersionNumber version, string description)
	{
		Name = GetType().Name;
		Title = name.Replace(":", string.Empty);
		Version = version;
		Author = author;
		Description = description;

		permission = Interface.Oxide.Permission;
		cmd = new Command();
		server = new Server();
		Manager = new PluginManager();
		plugins = new Oxide.Core.Libraries.Plugins(Manager);
		timer = new Timers(this);
		lang = new Lang(this);
		mod = Interface.Oxide;
		rust = new Game.Rust.Libraries.Rust();
		webrequest = new WebRequests();
		persistence = new GameObject($"Script_{name}").AddComponent<Persistence>();
		UnityEngine.Object.DontDestroyOnLoad(persistence.gameObject);
		covalence = new Covalence();

		HookableType = GetType();
	}

	public override void Dispose()
	{
		permission.UnregisterPermissions(this);

		timer?.Clear();
		timer = null;

		if (persistence != null)
		{
			var go = persistence.gameObject;
			UnityEngine.Object.DestroyImmediate(persistence);
			UnityEngine.Object.Destroy(go);
		}

		base.Dispose();
	}

	public static T Singleton<T>()
	{
		foreach (var mod in ModLoader.Packages)
		{
			foreach (var plugin in mod.Plugins)
			{
				if (plugin is T result)
				{
					return result;
				}
			}
		}

		return default;
	}

	#region Logging

	/// <summary>
	/// Outputs to the game's console a message with severity level 'NOTICE'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	public void Puts(object message) => Logger.Log($"[{Title}] {message}");

	/// <summary>
	/// Outputs to the game's console a message with severity level 'NOTICE'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="args"></param>
	public void Puts(object message, params object[] args) => Logger.Log($"[{Title}] {(args == null || args.Length == 0 ? message : string.Format(message?.ToString() ?? string.Empty, args))}");

	/// <summary>
	/// Outputs to the game's console a message with severity level 'NOTICE'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	public void Log(object message) => Logger.Log($"[{Title}] {message}");

	/// <summary>
	/// Outputs to the game's console a message with severity level 'NOTICE'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	public void Log(object message, params object[] args) => Logger.Log($"[{Title}] {(args == null || args.Length == 0 ? message : string.Format(message?.ToString() ?? string.Empty, args))}");

	/// <summary>
	/// Outputs to the game's console a message with severity level 'WARNING'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	public void LogWarning(object message) => Logger.Warn($"[{Title}] {message}");

	/// <summary>
	/// Outputs to the game's console a message with severity level 'WARNING'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	public void LogWarning(object message, params object[] args) => Logger.Warn($"[{Title}] {(args == null || args.Length == 0 ? message : string.Format(message?.ToString() ?? string.Empty, args))}");

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="ex"></param>
	public void LogError(object message, Exception ex) => Logger.Error($"[{Title}] {message}", ex);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="ex"></param>
	public void LogError(object message, Exception ex, params object[] args) => Logger.Error($"[{Title}] {(args == null || args.Length == 0 ? message : string.Format(message?.ToString() ?? string.Empty, args))}", ex);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	public void LogError(object message) => Logger.Error($"[{Title}] {message}");

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	public void LogError(object message, params object[] args) => Logger.Error($"[{Title}] {(args == null || args.Length == 0 ? message : string.Format(message?.ToString() ?? string.Empty, args))}");

	/// <summary>
	/// Outputs to the game's console a message with severity level 'WARNING'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="args"></param>
	public void PrintWarning(object format, params object[] args) => Logger.Warn($"[{Title}] {(args == null || args.Length == 0 ? format : string.Format(format?.ToString() ?? string.Empty, args))}");

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="args"></param>
	public void PrintError(object format, params object[] args) => Logger.Error($"[{Title}] {(args == null || args.Length == 0 ? format : string.Format(format?.ToString() ?? string.Empty, args))}");

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	public void RaiseError(object message) => Logger.Error($"[{Title}] {message}", null);

	protected void LogToFile(string filename, string text, Plugin plugin = null, bool timeStamp = true, bool anotherBool = false)
	{
		if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(text))
			return;

		DateTime now = DateTime.Now;

		string logFolder, finalFileName;

		if (plugin == null)
		{
			string subFolder = Path.GetDirectoryName(filename);
			string fileOnly = Path.GetFileNameWithoutExtension(filename);

			logFolder = string.IsNullOrEmpty(subFolder)
				? Defines.GetLogsFolder()
				: Path.Combine(Defines.GetLogsFolder(), subFolder);

			finalFileName = timeStamp
				? string.Concat(fileOnly, "-", now.ToString("yyyy-MM-dd"), ".txt")
				: string.Concat(fileOnly, ".txt");
		}
		else
		{
			logFolder = Path.Combine(Defines.GetLogsFolder(), plugin.Name);
			finalFileName = (timeStamp
				? string.Concat(plugin.Name, "_", filename, "-", now.ToString("yyyy-MM-dd"), ".txt")
				: string.Concat(plugin.Name, "_", filename, ".txt")).ToLower();
		}

		OsEx.Folder.Create(logFolder);

		string fullPath = Path.Combine(logFolder, Utility.CleanPath(finalFileName));

		string logEntry = timeStamp
			? string.Concat("[", now.ToString("yyyy-MM-dd HH:mm:ss"), "] ", text, Environment.NewLine)
			: string.Concat(text, Environment.NewLine);

		OsEx.File.Append(fullPath, logEntry);
	}

	#endregion

	#region Library

	public static T GetLibrary<T>(string name = null) where T : Library
	{
		return Interface.Oxide.GetLibrary<T>();
	}

	#endregion

	public void ILoadConfig()
	{
		try
		{
			LoadConfig();
		}
		catch (Exception ex)
		{
			LogError($"Failed ILoadConfig", ex);
		}
	}

	private bool loadedDefaultMessages;

	public void ILoadDefaultMessages()
	{
		if (loadedDefaultMessages)
		{
			return;
		}

		CallHook("LoadDefaultMessages");

		loadedDefaultMessages = true;
	}

	public override string ToPrettyString()
	{
		return $"{Title} v{Version} by {Author}";
	}

	#region Printing

	protected void PrintWarning(object message)
	{
		LogWarning(message);
	}

	protected void PrintWarning(string format, params object[] args)
	{
		LogWarning(format, args);
	}

	protected void PrintToConsole(BasePlayer player, string format, params object[] args)
	{
		if (player == null || player.net == null)
		{
			return;
		}
		player.SendConsoleCommand("echo " + ((args.Length != 0) ? string.Format(format, args) : format));
	}

	protected void PrintToConsole(string format, params object[] args)
	{
		if (BasePlayer.activePlayerList.Count >= 1)
		{
			ConsoleNetwork.BroadcastToAllClients("echo " + ((args.Length != 0) ? string.Format(format, args) : format));
		}
	}

	protected void PrintToChat(BasePlayer player, string format, params object[] args)
	{
		if (player == null || player.net == null)
		{
			return;
		}

#if !MINIMAL
		player.SendConsoleCommand("chat.add", 2, Community.Runtime.Core.DefaultServerChatId, (args.Length != 0) ? string.Format(format, args) : format);
#else
		player.SendConsoleCommand("chat.add", 2, 0, (args.Length != 0) ? string.Format(format, args) : format);
#endif
	}

	protected void PrintToChat(string format, params object[] args)
	{
		if (BasePlayer.activePlayerList.Count == 0)
		{
			return;
		}
#if !MINIMAL
			ConsoleNetwork.BroadcastToAllClients("chat.add", 2, Community.Runtime.Core.DefaultServerChatId, (args.Length != 0) ? string.Format(format, args) : format);
#else
			ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, (args.Length != 0) ? string.Format(format, args) : format);
#endif
	}

	protected void PrintToChat(BasePlayer player, string format, long chatId, params object[] args)
	{
		if (player == null || player.net == null)
		{
			return;
		}
		player.SendConsoleCommand("chat.add", 2, chatId, (args.Length != 0) ? string.Format(format, args) : format);
	}

	protected void PrintToChat(string format, long chatId, params object[] args)
	{
		if (BasePlayer.activePlayerList.Count > 0)
		{
			ConsoleNetwork.BroadcastToAllClients("chat.add", 2, chatId, (args.Length != 0) ? string.Format(format, args) : format);
		}
	}

	protected void SendReply(ConsoleSystem.Arg arg, string format, params object[] args)
	{
		if (arg != null || arg.Connection != null)
		{
			var connection = arg.Connection;
			var basePlayer = connection?.player as BasePlayer;

			if (basePlayer != null && basePlayer.net != null)
			{
				basePlayer.SendConsoleCommand($"echo {((args != null && args.Length != 0) ? string.Format(format, args) : format)}");
				return;
			}
		}
		Puts(format, args);
	}

	protected void SendReply(BasePlayer player, string format, params object[] args)
	{
		PrintToChat(player, format, args);
	}

	protected void SendWarning(ConsoleSystem.Arg arg, string format, params object[] args)
	{
		var connection = arg.Connection;
		var basePlayer = connection?.player as BasePlayer;

		if (basePlayer != null && basePlayer.net != null)
		{
			basePlayer.SendConsoleCommand($"echo {((args != null && args.Length != 0) ? string.Format(format, args) : format)}");
			return;
		}
		PrintWarning(format, args);;
	}

	protected void SendError(ConsoleSystem.Arg arg, string format, params object[] args)
	{
		var connection = arg.Connection;
		var basePlayer = connection?.player as BasePlayer;
		if (basePlayer != null && basePlayer.net != null)
		{
			basePlayer.SendConsoleCommand($"echo {((args != null && args.Length != 0) ? string.Format(format, args) : format)}");
			return;
		}
		PrintError(format, args);;
	}

	#endregion

	protected void ForcePlayerPosition(BasePlayer player, Vector3 destination)
	{
		player.MovePosition(destination);

		if (!player.IsSpectating() || (double)Vector3.Distance(player.transform.position, destination) > 25.0)
		{
			player.ClientRPC(RpcTarget.Player("ForcePositionTo", player), destination);
			return;
		}

		player.SendNetworkUpdate(BasePlayer.NetworkQueue.UpdateDistance);
	}

	#region Covalence

	protected void AddCovalenceCommand(string command, string callback, params string[] perms)
	{
		cmd.AddCovalenceCommand(command, this, callback, permissions: perms);

		if (perms != null)
		{
			foreach (var permission in perms)
			{
				if (!this.permission.PermissionExists(permission))
				{
					this.permission.RegisterPermission(permission, this);
				}
			}
		}
	}

	protected void AddCovalenceCommand(string[] commands, string callback, params string[] perms)
	{
		foreach (var command in commands)
		{
			cmd.AddCovalenceCommand(command, this, callback, permissions: perms);
		}

		if (perms != null)
		{
			foreach (var permission in perms)
			{
				if (!this.permission.PermissionExists(permission))
				{
					this.permission.RegisterPermission(permission, this);
				}
			}
		}
	}

	protected void AddUniversalCommand(string command, string callback, params string[] perms)
	{
		cmd.AddCovalenceCommand(command, this, callback, permissions: perms);

		if (perms != null)
		{
			foreach (var permission in perms)
			{
				if (!this.permission.PermissionExists(permission))
				{
					this.permission.RegisterPermission(permission, this);
				}
			}
		}
	}

	protected void AddUniversalCommand(string[] commands, string callback, params string[] perms)
	{
		foreach (var command in commands)
		{
			cmd.AddCovalenceCommand(command, this, callback, permissions: perms);
		}

		if (perms != null)
		{
			foreach (var permission in perms)
			{
				if (!this.permission.PermissionExists(permission))
				{
					this.permission.RegisterPermission(permission, this);
				}
			}
		}
	}

	#endregion
}
