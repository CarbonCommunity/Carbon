/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Ext.Discord.Entities.Emojis;
using Oxide.Ext.Discord.Entities.Interactions.MessageComponents;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Builders.MessageComponents
{
	// Token: 0x0200012D RID: 301
	public class SelectMenuComponentBuilder
	{
		// Token: 0x06000B01 RID: 2817 RVA: 0x00018B10 File Offset: 0x00016D10
		internal SelectMenuComponentBuilder(SelectMenuComponent menu, MessageComponentBuilder builder)
		{
			this._menu = menu;
			this._builder = builder;
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x00018B28 File Offset: 0x00016D28
		public SelectMenuComponentBuilder AddOption(string label, string value, string description, bool @default = false, DiscordEmoji emoji = null)
		{
			bool flag = string.IsNullOrEmpty(label);
			if (flag)
			{
				throw new ArgumentException("Value cannot be null or empty.", "label");
			}
			bool flag2 = string.IsNullOrEmpty(value);
			if (flag2)
			{
				throw new ArgumentException("Value cannot be null or empty.", "value");
			}
			bool flag3 = this._menu.Options.Count >= 25;
			if (flag3)
			{
				throw new InvalidMessageComponentException("Select Menu Options cannot have more than 25 options");
			}
			this._menu.Options.Add(new SelectMenuOption
			{
				Label = label,
				Value = value,
				Description = description,
				Default = new bool?(@default),
				Emoji = emoji
			});
			return this;
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x00018BDC File Offset: 0x00016DDC
		public MessageComponentBuilder Build()
		{
			return this._builder;
		}

		// Token: 0x04000701 RID: 1793
		private readonly SelectMenuComponent _menu;

		// Token: 0x04000702 RID: 1794
		private readonly MessageComponentBuilder _builder;
	}
}
