/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Builders.ApplicationCommands
{
	// Token: 0x0200012F RID: 303
	public class CommandOptionBuilder : IApplicationCommandBuilder
	{
		// Token: 0x06000B0A RID: 2826 RVA: 0x00018F14 File Offset: 0x00017114
		internal CommandOptionBuilder(List<CommandOption> parent, CommandOptionType type, string name, string description, IApplicationCommandBuilder builder)
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
			bool flag3 = type == CommandOptionType.SubCommand || type == CommandOptionType.SubCommandGroup;
			if (flag3)
			{
				throw new InvalidApplicationCommandException(string.Format("{0} is not allowed to be used here. Valid types are any non command type.", type));
			}
			this._option = new CommandOption
			{
				Name = name,
				Description = description,
				Type = type
			};
			parent.Add(this._option);
			this._builder = builder;
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x00018FC0 File Offset: 0x000171C0
		public CommandOptionBuilder Required(bool required = true)
		{
			this._option.Required = new bool?(required);
			return this;
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x00018FE8 File Offset: 0x000171E8
		public CommandOptionBuilder AutoComplete(bool autoComplete = true)
		{
			this._option.Autocomplete = new bool?(autoComplete);
			return this;
		}

		// Token: 0x06000B0D RID: 2829 RVA: 0x00019010 File Offset: 0x00017210
		public CommandOptionBuilder SetMinValue(int minValue)
		{
			bool flag = this._option.Type != CommandOptionType.Integer && this._option.Type != CommandOptionType.Number;
			if (flag)
			{
				throw new InvalidApplicationCommandException("Can only set min value for Integer or Number Type");
			}
			this._option.MinValue = new double?((double)minValue);
			return this;
		}

		// Token: 0x06000B0E RID: 2830 RVA: 0x0001906C File Offset: 0x0001726C
		public CommandOptionBuilder SetMinValue(double minValue)
		{
			bool flag = this._option.Type != CommandOptionType.Number;
			if (flag)
			{
				throw new InvalidApplicationCommandException("Can only set min value for Number Type");
			}
			this._option.MinValue = new double?(minValue);
			return this;
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x000190B4 File Offset: 0x000172B4
		public CommandOptionBuilder SetMaxValue(int maxValue)
		{
			bool flag = this._option.Type != CommandOptionType.Integer && this._option.Type != CommandOptionType.Number;
			if (flag)
			{
				throw new InvalidApplicationCommandException("Can only set max value for Integer or Number Type");
			}
			this._option.MaxValue = new double?((double)maxValue);
			return this;
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x00019110 File Offset: 0x00017310
		public CommandOptionBuilder SetMaxValue(double maxValue)
		{
			bool flag = this._option.Type != CommandOptionType.Integer && this._option.Type != CommandOptionType.Number;
			if (flag)
			{
				throw new InvalidApplicationCommandException("Can only set max value for Number Type");
			}
			this._option.MaxValue = new double?(maxValue);
			return this;
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x00019168 File Offset: 0x00017368
		public CommandOptionBuilder SetChannelTypes(List<ChannelType> types)
		{
			bool flag = this._option.Type != CommandOptionType.Channel;
			if (flag)
			{
				throw new InvalidApplicationCommandException("Can only set ChannelTypes for CommandOptionType.Channel");
			}
			this._option.ChannelTypes = types;
			return this;
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x000191AC File Offset: 0x000173AC
		public CommandOptionBuilder AddChoice(string name, string value)
		{
			bool flag = string.IsNullOrEmpty(name);
			if (flag)
			{
				throw new ArgumentException("Value cannot be null or empty.", "name");
			}
			bool flag2 = string.IsNullOrEmpty(value);
			if (flag2)
			{
				throw new ArgumentException("Value cannot be null or empty.", "value");
			}
			bool flag3 = this._option.Type != CommandOptionType.String;
			if (flag3)
			{
				throw new InvalidApplicationCommandException(string.Format("Cannot add a string choice to non string type: {0}", this._option.Type));
			}
			return this.AddChoice(name, value);
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x00019230 File Offset: 0x00017430
		public CommandOptionBuilder AddChoice(string name, int value)
		{
			bool flag = string.IsNullOrEmpty(name);
			if (flag)
			{
				throw new ArgumentException("Value cannot be null or empty.", "name");
			}
			bool flag2 = this._option.Type != CommandOptionType.Integer;
			if (flag2)
			{
				throw new InvalidApplicationCommandException(string.Format("Cannot add a integer choice to non integer type: {0}", this._option.Type));
			}
			return this.AddChoice(name, value);
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x000192A0 File Offset: 0x000174A0
		public CommandOptionBuilder AddChoice(string name, double value)
		{
			bool flag = string.IsNullOrEmpty(name);
			if (flag)
			{
				throw new ArgumentException("Value cannot be null or empty.", "name");
			}
			bool flag2 = this._option.Type != CommandOptionType.Number;
			if (flag2)
			{
				throw new InvalidApplicationCommandException(string.Format("Cannot add a number choice to non number type: {0}", this._option.Type));
			}
			return this.AddChoice(name, value);
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x00019310 File Offset: 0x00017510
		private CommandOptionBuilder AddChoice(string name, object value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			bool flag2 = string.IsNullOrEmpty(name);
			if (flag2)
			{
				throw new ArgumentException("Value cannot be null or empty.", "name");
			}
			bool flag3 = this._option.Choices == null;
			if (flag3)
			{
				this._option.Choices = new List<CommandOptionChoice>();
			}
			this._option.Choices.Add(new CommandOptionChoice
			{
				Name = name,
				Value = value
			});
			return this;
		}

		// Token: 0x06000B16 RID: 2838 RVA: 0x0001939C File Offset: 0x0001759C
		public ApplicationCommandBuilder BuildForApplicationCommand()
		{
			return (ApplicationCommandBuilder)this._builder;
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x000193BC File Offset: 0x000175BC
		public SubCommandBuilder BuildForSubCommand()
		{
			return (SubCommandBuilder)this._builder;
		}

		// Token: 0x04000706 RID: 1798
		private readonly CommandOption _option;

		// Token: 0x04000707 RID: 1799
		private readonly IApplicationCommandBuilder _builder;
	}
}
