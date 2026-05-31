using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using API.Hooks;
using Carbon.Components;
using Carbon.Extensions;
using Facepunch;
using Facepunch.Extend;
using static ConsoleSystem;
using Command = Oxide.Game.Rust.Libraries.Command;
using Exception = System.Exception;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Static
{
	public partial class Static_RCon
	{
		[HookAttribute.Patch("OnRconCommand", "OnRconCommand", typeof(RCon), "OnCommand", new System.Type[] { typeof(Facepunch.RCon.Command) })]
		[HookAttribute.Options(HookFlags.Static | HookFlags.IgnoreChecksum)]

		[MetadataAttribute.Info("Called when an RCON command is run.")]
		[MetadataAttribute.Parameter("ip", typeof(IPAddress))]
		[MetadataAttribute.Parameter("command", typeof(string))]
		[MetadataAttribute.Parameter("arguments", typeof(string[]))]
		[MetadataAttribute.OxideCompatible]

		public class IOnRconCommand : Patch
		{
			internal static string Space = " ";

			public static bool Prefix(RCon.Command cmd)
			{
				if (Community.Runtime == null)
				{
					return true;
				}

				RCon.responseIdentifier = cmd.Identifier;
				RCon.responseConnection = cmd.ConnectionId;
				RCon.isInput = false;

				try
				{
					ConsoleArgEx.TryParseCommand(cmd.Message, out var command, out var parsedArguments);

					var temp = Pool.Get<List<string>>();
					temp.AddRange(parsedArguments);
					var arguments = temp.ToArray();
					Pool.FreeUnmanaged(ref temp);

					if (Community.Runtime.Config.Aliases.TryGetValue(command, out var alias))
					{
						command = alias;
						cmd.Message = $"{alias} {arguments.ToString(Space)}";
					}

					var consoleArg = new Arg(Option.Server.Quiet().FromRconConnection(cmd.ConnectionId, cmd.Ip.ToString(), cmd.Name), cmd.Message);

					if (HookCaller.CallStaticHook(3740958730, cmd.Ip, command, arguments) != null)
					{
						return false;
					}

					try
					{
						if (Community.Runtime.CommandManager.Contains(Community.Runtime.CommandManager.RCon, command, out var outCommand))
						{
							Command.FromRcon = API.Commands.Command.FromRcon = true;

							var commandArgs = Pool.Get<API.Commands.Command.Args>();
							commandArgs.Token = consoleArg;
							commandArgs.Type = outCommand.Type;
							commandArgs.Arguments = arguments;
							commandArgs.IsRCon = true;
							commandArgs.IsServer = true;
							commandArgs.PrintOutput = consoleArg.Option.PrintOutput;
							consoleArg.Args = arguments;
							consoleArg.cmd = outCommand.RustCommand;

							Community.Runtime.CommandManager.Execute(outCommand, commandArgs);
							Pool.Free(ref commandArgs);

							Community.Runtime.Core.NextFrame(() => Command.FromRcon = API.Commands.Command.FromRcon = false);
							return false;
						}
					}
					catch (Exception ex)
					{
						Logger.Error("RconCommand_OnCommand", ex);
					}
				}
				finally
				{
					RCon.responseIdentifier = 0;
					RCon.responseConnection = -1;
				}

				return true;
			}
		}
	}
}
