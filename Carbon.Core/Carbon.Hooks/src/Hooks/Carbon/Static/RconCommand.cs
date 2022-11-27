using System;
using System.Linq;
using Carbon.Extensions;
using Facepunch;
using Oxide.Core;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Static
{
	public partial class Static_RCon
	{
		/*
		[Hook.AlwaysPatched]
		[Hook("OnRconCommand"), Hook.Category(Hook.Category.Enum.Server)]
		[Hook.Parameter("address", typeof(IPAddress))]
		[Hook.Parameter("command", typeof(string))]
		[Hook.Parameter("args", typeof(string[]))]
		[Hook.Info("Called when an RCON command is run.")]
		[Hook.Patch(typeof(RCon), "OnCommand")]
		*/

		public class Static_RCon_OnCommand_ccce0832a0eb4c28bc2372f5e0812c7e
		{
			public static Metadata metadata = new Metadata("OnRconCommand",
				typeof(RCon), "OnCommand", new System.Type[] { typeof(Facepunch.RCon.Command) });

			static Static_RCon_OnCommand_ccce0832a0eb4c28bc2372f5e0812c7e()
			{
				metadata.SetIdentifier("ccce0832a0eb4c28bc2372f5e0812c7e");
				metadata.SetAlwaysPatch(true);
			}

			public static bool Prefix(RCon.Command cmd)
			{
				if (Community.Runtime == null) return true;

				try
				{
					var split = cmd.Message.Split(ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries);
					var command = split[0].Trim();

					var arguments = Pool.GetList<string>();
					foreach (var arg in split.Skip(1)) arguments.Add(arg.Trim());
					var args2 = arguments.ToArray();
					Pool.FreeList(ref arguments);

					if (Interface.CallHook("OnRconCommand", cmd.Ip, command, args2) != null)
					{
						return false;
					}

					foreach (var carbonCommand in Community.Runtime.AllConsoleCommands)
					{
						if (carbonCommand.Command == command)
						{
							try
							{
								global::Command._fromRcon = true; // FIXME : Had to change protection from internal to public
								carbonCommand.Callback?.Invoke(null, command, args2);
								return !carbonCommand.SkipOriginal;
							}
							catch (Exception ex)
							{
								Carbon.Logger.Error("RconCommand_OnCommand", ex);
							}

							break;
						}
					}
				}
				catch { }

				return true;
			}
		}
	}
}