/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Builders.ApplicationCommands
{
	// Token: 0x0200012E RID: 302
	public class ApplicationCommandBuilder : IApplicationCommandBuilder
	{
		// Token: 0x06000B04 RID: 2820 RVA: 0x00018BF4 File Offset: 0x00016DF4
		public ApplicationCommandBuilder(string name, string description, ApplicationCommandType type)
		{
			bool flag = string.IsNullOrEmpty(name);
			if (flag)
			{
				throw new ArgumentException("Value cannot be null or empty.", "name");
			}
			bool flag2 = string.IsNullOrEmpty(description);
			if (flag2)
			{
				throw new ArgumentException("Value cannot be null or empty.", "description");
			}
			this._options = new List<CommandOption>();
			this.Command = new CommandCreate
			{
				Name = name,
				Description = description,
				Type = new ApplicationCommandType?(type),
				Options = this._options
			};
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x00018C80 File Offset: 0x00016E80
		public ApplicationCommandBuilder SetEnabled(bool enabled)
		{
			this.Command.DefaultPermissions = new bool?(enabled);
			return this;
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x00018CA8 File Offset: 0x00016EA8
		public SubCommandGroupBuilder AddSubCommandGroup(string name, string description)
		{
			bool flag = string.IsNullOrEmpty(name);
			if (flag)
			{
				throw new ArgumentException("Value cannot be null or empty.", "name");
			}
			bool flag2 = string.IsNullOrEmpty(description);
			if (flag2)
			{
				throw new ArgumentException("Value cannot be null or empty.", "description");
			}
			bool flag3 = this._chosenType != null && this._chosenType.Value != CommandOptionType.SubCommandGroup && this._chosenType.Value != CommandOptionType.SubCommand;
			if (flag3)
			{
				throw new InvalidApplicationCommandException("Cannot mix sub command / sub command groups with command options");
			}
			ApplicationCommandType? type = this.Command.Type;
			ApplicationCommandType applicationCommandType = ApplicationCommandType.Message;
			bool flag4;
			if (!(type.GetValueOrDefault() == applicationCommandType & type != null))
			{
				type = this.Command.Type;
				applicationCommandType = ApplicationCommandType.User;
				flag4 = (type.GetValueOrDefault() == applicationCommandType & type != null);
			}
			else
			{
				flag4 = true;
			}
			bool flag5 = flag4;
			if (flag5)
			{
				throw new InvalidApplicationCommandException("Message and User commands cannot have sub command groups");
			}
			this._chosenType = new CommandOptionType?(CommandOptionType.SubCommandGroup);
			return new SubCommandGroupBuilder(name, description, this);
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x00018DA0 File Offset: 0x00016FA0
		public SubCommandBuilder AddSubCommand(string name, string description)
		{
			bool flag = string.IsNullOrEmpty(name);
			if (flag)
			{
				throw new ArgumentException("Value cannot be null or empty.", "name");
			}
			bool flag2 = string.IsNullOrEmpty(description);
			if (flag2)
			{
				throw new ArgumentException("Value cannot be null or empty.", "description");
			}
			bool flag3 = this._chosenType != null && this._chosenType.Value != CommandOptionType.SubCommandGroup && this._chosenType.Value != CommandOptionType.SubCommand;
			if (flag3)
			{
				throw new InvalidApplicationCommandException("Cannot mix sub command / sub command groups with command options");
			}
			ApplicationCommandType? type = this.Command.Type;
			ApplicationCommandType applicationCommandType = ApplicationCommandType.Message;
			bool flag4;
			if (!(type.GetValueOrDefault() == applicationCommandType & type != null))
			{
				type = this.Command.Type;
				applicationCommandType = ApplicationCommandType.User;
				flag4 = (type.GetValueOrDefault() == applicationCommandType & type != null);
			}
			else
			{
				flag4 = true;
			}
			bool flag5 = flag4;
			if (flag5)
			{
				throw new InvalidApplicationCommandException("Message and User commands cannot have sub commands");
			}
			this._chosenType = new CommandOptionType?(CommandOptionType.SubCommand);
			return new SubCommandBuilder(this._options, name, description, this);
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x00018E9C File Offset: 0x0001709C
		public CommandOptionBuilder AddOption(CommandOptionType type, string name, string description)
		{
			bool flag = this._chosenType != null && (this._chosenType.Value == CommandOptionType.SubCommandGroup || this._chosenType.Value == CommandOptionType.SubCommand);
			if (flag)
			{
				throw new InvalidApplicationCommandException("Cannot mix sub command / sub command groups with command options");
			}
			return new CommandOptionBuilder(this._options, type, name, description, this);
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x00018EFC File Offset: 0x000170FC
		public CommandCreate Build()
		{
			return this.Command;
		}

		// Token: 0x04000703 RID: 1795
		internal readonly CommandCreate Command;

		// Token: 0x04000704 RID: 1796
		private readonly List<CommandOption> _options;

		// Token: 0x04000705 RID: 1797
		private CommandOptionType? _chosenType;
	}
}
