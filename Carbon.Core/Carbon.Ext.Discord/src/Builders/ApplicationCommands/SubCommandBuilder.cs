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

namespace Oxide.Ext.Discord.Builders.ApplicationCommands
{
	// Token: 0x02000131 RID: 305
	public class SubCommandBuilder : IApplicationCommandBuilder
	{
		// Token: 0x06000B18 RID: 2840 RVA: 0x000193DC File Offset: 0x000175DC
		internal SubCommandBuilder(List<CommandOption> parent, string name, string description, IApplicationCommandBuilder builder)
		{
			this._builder = builder;
			this._options = new List<CommandOption>();
			parent.Add(new CommandOption
			{
				Name = name,
				Description = description,
				Type = CommandOptionType.SubCommand,
				Options = this._options
			});
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x00019438 File Offset: 0x00017638
		public CommandOptionBuilder AddOption(CommandOptionType type, string name, string description)
		{
			return new CommandOptionBuilder(this._options, type, name, description, this);
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x0001945C File Offset: 0x0001765C
		public ApplicationCommandBuilder BuildForApplicationCommand()
		{
			return (ApplicationCommandBuilder)this._builder;
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x0001947C File Offset: 0x0001767C
		public SubCommandGroupBuilder BuildForSubCommandGroup()
		{
			return (SubCommandGroupBuilder)this._builder;
		}

		// Token: 0x04000708 RID: 1800
		private readonly IApplicationCommandBuilder _builder;

		// Token: 0x04000709 RID: 1801
		private readonly List<CommandOption> _options;
	}
}
