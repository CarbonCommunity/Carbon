/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Oxide.Ext.Discord.Entities.Emojis;
using Oxide.Ext.Discord.Entities.Interactions.MessageComponents;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Builders.MessageComponents
{
	// Token: 0x0200012C RID: 300
	public class MessageComponentBuilder
	{
		// Token: 0x06000AF9 RID: 2809 RVA: 0x000187D2 File Offset: 0x000169D2
		public MessageComponentBuilder()
		{
			this._current = new ActionRowComponent();
			this._components.Add(this._current);
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x00018804 File Offset: 0x00016A04
		public MessageComponentBuilder AddActionButton(ButtonStyle style, string label, string customId, bool disabled = false, DiscordEmoji emoji = null)
		{
			bool flag = style == ButtonStyle.Link;
			if (flag)
			{
				throw new InvalidMessageComponentException("Cannot add link button as action button. Please use AddLinkButton instead");
			}
			this.UpdateActionRow<ButtonComponent>();
			this._current.Components.Add(new ButtonComponent
			{
				Style = style,
				Label = label,
				CustomId = customId,
				Disabled = new bool?(disabled),
				Emoji = emoji
			});
			return this;
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x00018878 File Offset: 0x00016A78
		public MessageComponentBuilder AddDummyButton(string label, bool disabled = true)
		{
			return this.AddActionButton(ButtonStyle.Secondary, label, string.Format("DUMMY_{0}", this._components.Count * 5 + this._current.Components.Count), disabled, null);
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x000188C4 File Offset: 0x00016AC4
		public MessageComponentBuilder AddLinkButton(string label, string url, bool disabled = false, DiscordEmoji emoji = null)
		{
			bool flag = url == null;
			if (flag)
			{
				throw new ArgumentNullException("url");
			}
			this.UpdateActionRow<ButtonComponent>();
			this._current.Components.Add(new ButtonComponent
			{
				Style = ButtonStyle.Link,
				Label = label,
				Url = url,
				Disabled = new bool?(disabled),
				Emoji = emoji
			});
			return this;
		}

		// Token: 0x06000AFD RID: 2813 RVA: 0x00018938 File Offset: 0x00016B38
		public SelectMenuComponentBuilder AddSelectMenu(string customId, string placeholder, int minValues = 1, int maxValues = 1, bool disabled = false)
		{
			bool flag = string.IsNullOrEmpty(customId);
			if (flag)
			{
				throw new ArgumentException("Value cannot be null or empty.", "customId");
			}
			this.UpdateActionRow<SelectMenuComponent>();
			SelectMenuComponent selectMenuComponent = new SelectMenuComponent
			{
				CustomId = customId,
				Placeholder = placeholder,
				MinValues = new int?(minValues),
				MaxValues = new int?(maxValues),
				Disabled = new bool?(disabled)
			};
			this._current.Components.Add(selectMenuComponent);
			return new SelectMenuComponentBuilder(selectMenuComponent, this);
		}

		// Token: 0x06000AFE RID: 2814 RVA: 0x000189C4 File Offset: 0x00016BC4
		public MessageComponentBuilder AddInputText(string customId, string label, InputTextStyles style, string value = null, bool? required = null, string placeholder = null, int? minLength = null, int? maxLength = null)
		{
			bool flag = string.IsNullOrEmpty(customId);
			if (flag)
			{
				throw new ArgumentException("Value cannot be null or empty.", "customId");
			}
			this.UpdateActionRow<InputTextComponent>();
			InputTextComponent item = new InputTextComponent
			{
				CustomId = customId,
				Label = label,
				Style = style,
				Value = value,
				Required = required,
				Placeholder = placeholder,
				MinLength = minLength,
				MaxLength = maxLength
			};
			this._current.Components.Add(item);
			return this;
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x00018A58 File Offset: 0x00016C58
		private void UpdateActionRow<T>() where T : BaseComponent
		{
			bool flag = this._current.Components.Count == 0;
			if (!flag)
			{
				bool flag2 = typeof(T) == typeof(ButtonComponent) && this._current.Components.Count < 5;
				if (!flag2)
				{
					bool flag3 = this._components.Count >= 5;
					if (flag3)
					{
						throw new InvalidMessageComponentException("Cannot have more than 5 action rows");
					}
					this._current = new ActionRowComponent();
					this._components.Add(this._current);
				}
			}
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x00018AF8 File Offset: 0x00016CF8
		public List<ActionRowComponent> Build()
		{
			return this._components;
		}

		// Token: 0x040006FF RID: 1791
		private readonly List<ActionRowComponent> _components = new List<ActionRowComponent>();

		// Token: 0x04000700 RID: 1792
		private ActionRowComponent _current;
	}
}
