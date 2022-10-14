///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Carbon;
using Carbon.Base;
using Oxide.Core;
using Oxide.Plugins;
using static ConsoleSystem;
using Pool = Facepunch.Pool;

public class Command
{
	public void AddChatCommand(string command, BaseHookable plugin, Action<BasePlayer, string, string[]> callback, bool skipOriginal = true, string help = null, object reference = null)
	{
		if (Community.Runtime.AllChatCommands.Count(x => x.Command == command) == 0)
		{
			Community.Runtime.AllChatCommands.Add(new OxideCommand
			{
				Command = command,
				Plugin = plugin,
				SkipOriginal = skipOriginal,
				Callback = (player, cmd, args) =>
				{
					try { callback.Invoke(player, cmd, args); }
					catch (Exception ex) { if (plugin is RustPlugin rustPlugin) rustPlugin.LogError("Error", ex); }
				},
				Help = help,
				Reference = reference
			});
		}
		else Carbon.Logger.Warn($"Chat command '{command}' already exists.");
	}
	public void AddChatCommand(string command, BaseHookable plugin, string method, bool skipOriginal = true, string help = null, object reference = null)
	{
		AddChatCommand(command, plugin, (player, cmd, args) =>
		{
			var argData = Pool.GetList<object>();
			var result = (object[])null;
			try
			{
				var m = plugin.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				switch (m.GetParameters().Length)
				{
					case 1:
						{
							argData.Add(player);
							result = argData.ToArray();
							break;
						}

					case 2:
						{
							argData.Add(player);
							argData.Add(cmd);
							result = argData.ToArray();
							break;
						}

					case 3:
						{
							argData.Add(player);
							argData.Add(cmd);
							argData.Add(args);
							result = argData.ToArray();
							break;
						}
				}

				m?.Invoke(plugin, result);
			}
			catch (Exception ex) { if (plugin is RustPlugin rustPlugin) rustPlugin.LogError("Error", ex); }

			if (argData != null) Pool.FreeList(ref argData);
			if (result != null) Pool.Free(ref result);
		}, skipOriginal, help, reference);
	}
	public void AddConsoleCommand(string command, BaseHookable plugin, Action<BasePlayer, string, string[]> callback, bool skipOriginal = true, string help = null, object reference = null)
	{
		if (Community.Runtime.AllConsoleCommands.Count(x => x.Command == command) == 0)
		{
			Community.Runtime.AllConsoleCommands.Add(new OxideCommand
			{
				Command = command,
				Plugin = plugin,
				SkipOriginal = skipOriginal,
				Callback = callback,
				Help = help,
				Reference = reference
			});
		}
		else Carbon.Logger.Warn($"Console command '{command}' already exists.");
	}
	public void AddConsoleCommand(string command, BaseHookable plugin, string method, bool skipOriginal = true, string help = null, object reference = null)
	{
		AddConsoleCommand(command, plugin, (player, cmd, args) =>
		{
			var arguments = Pool.GetList<object>();
			var result = (object[])null;

			try
			{
				var fullString = args == null || args.Length == 0 ? cmd : $"{cmd} {string.Join(" ", args)}";
				var value = new object[] { fullString };
				var client = player == null ? Option.Unrestricted : Option.Client;
				var arg = FormatterServices.GetUninitializedObject(typeof(Arg)) as Arg;
				if (player != null) client = client.FromConnection(player.net.connection);
				arg.Option = client;
				arg.FullString = fullString;
				arg.Args = args;

				arguments.Add(arg);

				try
				{
					var methodInfo = plugin.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					var parameters = methodInfo.GetParameters();

					if (parameters.Length > 0)
					{
						for (int i = 1; i < parameters.Length; i++)
						{
							arguments.Add(null);
						}
					}

					result = arguments.ToArray();

					if (Interface.CallHook("OnCarbonCommand", arg) == null)
					{
						methodInfo?.Invoke(plugin, result);
					}
				}
				catch (Exception ex) { if (plugin is RustPlugin rustPlugin) rustPlugin.LogError("Error", ex); }
			}
			catch (TargetParameterCountException) { }
			catch (Exception ex) { if (plugin is RustPlugin rustPlugin) rustPlugin.LogError("Error", ex); }

			Pool.FreeList(ref arguments);
			if (result != null) Pool.Free(ref result);
		}, skipOriginal, help, reference);
	}
	public void AddConsoleCommand(string command, BaseHookable plugin, Func<Arg, bool> callback, bool skipOriginal = true, string help = null, object reference = null)
	{
		AddConsoleCommand(command, plugin, (player, cmd, args) =>
		{
			var arguments = Pool.GetList<object>();
			var result = (object[])null;

			try
			{
				var fullString = args == null || args.Length == 0 ? cmd : $"{cmd} {string.Join(" ", args)}";
				var value = new object[] { fullString };
				var client = player == null ? Option.Unrestricted : Option.Client;
				var arg = FormatterServices.GetUninitializedObject(typeof(Arg)) as Arg;
				if (player != null) client = client.FromConnection(player.net.connection);
				arg.Option = client;
				arg.FullString = fullString;
				arg.Args = args;

				arguments.Add(arg);
				result = arguments.ToArray();

				if (Interface.CallHook("OnCarbonCommand", arg) == null)
				{
					callback.Invoke(arg);
				}
			}
			catch (TargetParameterCountException) { }
			catch (Exception ex) { if (plugin is RustPlugin rustPlugin) rustPlugin.LogError("Error", ex); }

			Pool.FreeList(ref arguments);
			if (result != null) Pool.Free(ref result);
		}, skipOriginal, help, reference);
	}
	public void AddCovalenceCommand(string command, BaseHookable plugin, string method, bool skipOriginal = true, string help = null, object reference = null)
	{
		AddChatCommand(command, plugin, method, skipOriginal, help, reference);
		AddConsoleCommand(command, plugin, method, skipOriginal, help, reference);
	}
	public void AddCovalenceCommand(string command, BaseHookable plugin, Action<BasePlayer, string, string[]> callback, bool skipOriginal = true, string help = null, object reference = null)
	{
		AddChatCommand(command, plugin, callback, skipOriginal, help, reference);
		AddConsoleCommand(command, plugin, callback, skipOriginal, help, reference);
	}
}
