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
using Oxide.Ext.Discord.Entities.Interactions.MessageComponents;
using Oxide.Ext.Discord.Helpers;

namespace Oxide.Ext.Discord.Entities.Interactions
{
	// Token: 0x0200007E RID: 126
	public class InteractionDataParsed
	{
		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060004A8 RID: 1192 RVA: 0x00011688 File Offset: 0x0000F888
		// (set) Token: 0x060004A9 RID: 1193 RVA: 0x00011690 File Offset: 0x0000F890
		public string CommandGroup { get; private set; } = string.Empty;

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060004AA RID: 1194 RVA: 0x00011699 File Offset: 0x0000F899
		// (set) Token: 0x060004AB RID: 1195 RVA: 0x000116A1 File Offset: 0x0000F8A1
		public string SubCommand { get; private set; } = string.Empty;

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060004AC RID: 1196 RVA: 0x000116AA File Offset: 0x0000F8AA
		// (set) Token: 0x060004AD RID: 1197 RVA: 0x000116B2 File Offset: 0x0000F8B2
		public InteractionDataArgs Args { get; private set; }

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060004AE RID: 1198 RVA: 0x000116BC File Offset: 0x0000F8BC
		public bool InGuild
		{
			get
			{
				return this.Interaction.GuildId != null;
			}
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x000116DC File Offset: 0x0000F8DC
		public InteractionDataParsed(DiscordInteraction interaction)
		{
			this.Interaction = interaction;
			this.Data = interaction.Data;
			bool flag = this.Data.ComponentType != null;
			if (flag)
			{
				this.TriggeredComponentId = this.Data.CustomId;
				MessageComponentType? componentType = this.Data.ComponentType;
				MessageComponentType messageComponentType = MessageComponentType.SelectMenu;
				bool flag2 = componentType.GetValueOrDefault() == messageComponentType & componentType != null;
				if (flag2)
				{
					this.SelectMenuValues = this.Data.Values;
				}
			}
			else
			{
				this.Type = this.Data.Type;
				this.Command = this.Data.Name;
				ApplicationCommandType? type = this.Type;
				ApplicationCommandType applicationCommandType = ApplicationCommandType.Message;
				bool flag3;
				if (!(type.GetValueOrDefault() == applicationCommandType & type != null))
				{
					type = this.Type;
					applicationCommandType = ApplicationCommandType.User;
					flag3 = (type.GetValueOrDefault() == applicationCommandType & type != null);
				}
				else
				{
					flag3 = true;
				}
				bool flag4 = flag3;
				if (!flag4)
				{
					this.UserOxideLocale = LocaleConverter.GetOxideLocale(interaction.Locale);
					this.GuildOxideLocale = LocaleConverter.GetOxideLocale(interaction.GuildLocale);
					this.ParseCommand(this.Data.Options);
				}
			}
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x00011820 File Offset: 0x0000FA20
		private void ParseCommand(List<InteractionDataOption> options)
		{
			bool flag = options == null || options.Count == 0;
			if (!flag)
			{
				InteractionDataOption interactionDataOption = options[0];
				CommandOptionType type = interactionDataOption.Type;
				CommandOptionType commandOptionType = type;
				if (commandOptionType != CommandOptionType.SubCommand)
				{
					if (commandOptionType != CommandOptionType.SubCommandGroup)
					{
						this.Args = new InteractionDataArgs(this.Interaction, options);
					}
					else
					{
						this.CommandGroup = interactionDataOption.Name;
						this.ParseCommand(interactionDataOption.Options);
					}
				}
				else
				{
					this.SubCommand = interactionDataOption.Name;
					this.ParseCommand(interactionDataOption.Options);
				}
			}
		}

		// Token: 0x040002CD RID: 717
		public readonly DiscordInteraction Interaction;

		// Token: 0x040002CE RID: 718
		public readonly InteractionData Data;

		// Token: 0x040002CF RID: 719
		public readonly ApplicationCommandType? Type;

		// Token: 0x040002D0 RID: 720
		public readonly string Command;

		// Token: 0x040002D4 RID: 724
		public readonly string TriggeredComponentId;

		// Token: 0x040002D5 RID: 725
		public readonly List<string> SelectMenuValues;

		// Token: 0x040002D6 RID: 726
		public readonly string UserOxideLocale;

		// Token: 0x040002D7 RID: 727
		public readonly string GuildOxideLocale;
	}
}
