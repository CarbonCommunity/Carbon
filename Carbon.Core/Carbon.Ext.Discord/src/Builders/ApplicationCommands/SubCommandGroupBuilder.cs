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
	// Token: 0x02000132 RID: 306
	public class SubCommandGroupBuilder : IApplicationCommandBuilder
	{
		// Token: 0x06000B1C RID: 2844 RVA: 0x0001949C File Offset: 0x0001769C
		internal SubCommandGroupBuilder(string name, string description, ApplicationCommandBuilder builder)
		{
			this._option = new CommandOption
			{
				Name = name,
				Description = description,
				Type = CommandOptionType.SubCommandGroup,
				Options = new List<CommandOption>()
			};
			builder.Command.Options.Add(this._option);
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x000194F8 File Offset: 0x000176F8
		public SubCommandBuilder AddSubCommand(string name, string description)
		{
			return new SubCommandBuilder(this._option.Options, name, description, this);
		}

		// Token: 0x0400070A RID: 1802
		private readonly CommandOption _option;
	}
}
