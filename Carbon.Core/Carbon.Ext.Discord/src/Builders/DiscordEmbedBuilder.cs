/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using Oxide.Ext.Discord.Entities.Messages.Embeds;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Builders
{
	// Token: 0x0200012A RID: 298
	public class DiscordEmbedBuilder
	{
		// Token: 0x06000ADE RID: 2782 RVA: 0x0001815C File Offset: 0x0001635C
		public DiscordEmbedBuilder AddTitle(string title)
		{
			bool flag = title != null && title.Length > 256;
			if (flag)
			{
				throw new InvalidEmbedException("Title cannot be more than 256 characters");
			}
			this._embed.Title = title;
			return this;
		}

		// Token: 0x06000ADF RID: 2783 RVA: 0x000181A0 File Offset: 0x000163A0
		public DiscordEmbedBuilder AddDescription(string description)
		{
			bool flag = description == null;
			if (flag)
			{
				throw new ArgumentNullException("description");
			}
			bool flag2 = description.Length > 4096;
			if (flag2)
			{
				throw new InvalidEmbedException("Description cannot be more than 4096 characters");
			}
			this._embed.Description = description;
			return this;
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x000181F0 File Offset: 0x000163F0
		public DiscordEmbedBuilder AddUrl(string url)
		{
			this._embed.Url = url;
			return this;
		}

		// Token: 0x06000AE1 RID: 2785 RVA: 0x00018210 File Offset: 0x00016410
		public DiscordEmbedBuilder AddAuthor(string name, string iconUrl = null, string url = null, string proxyIconUrl = null)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = name.Length > 256;
			if (flag2)
			{
				throw new InvalidEmbedException("Author name cannot be more than 256 characters");
			}
			this._embed.Author = new EmbedAuthor(name, iconUrl, url, proxyIconUrl);
			return this;
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x0001826C File Offset: 0x0001646C
		public DiscordEmbedBuilder AddFooter(string text, string iconUrl = null, string proxyIconUrl = null)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			bool flag2 = text.Length > 2048;
			if (flag2)
			{
				throw new InvalidEmbedException("Footer text cannot be more than 2048 characters");
			}
			this._embed.Footer = new EmbedFooter(text, iconUrl, proxyIconUrl);
			return this;
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x000182C4 File Offset: 0x000164C4
		public DiscordEmbedBuilder AddColor(DiscordColor color)
		{
			this._embed.Color = color;
			return this;
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x000182E4 File Offset: 0x000164E4
		public DiscordEmbedBuilder AddColor(uint color)
		{
			this._embed.Color = new DiscordColor(color);
			return this;
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x0001830C File Offset: 0x0001650C
		public DiscordEmbedBuilder AddColor(string color)
		{
			this._embed.Color = new DiscordColor(uint.Parse(color.TrimStart(new char[]
			{
				'#'
			}), NumberStyles.AllowHexSpecifier));
			return this;
		}

		// Token: 0x06000AE6 RID: 2790 RVA: 0x0001834C File Offset: 0x0001654C
		public DiscordEmbedBuilder AddColor(byte red, byte green, byte blue)
		{
			this._embed.Color = new DiscordColor(red, green, blue);
			return this;
		}

		// Token: 0x06000AE7 RID: 2791 RVA: 0x00018374 File Offset: 0x00016574
		public DiscordEmbedBuilder AddColor(int red, int green, int blue)
		{
			this._embed.Color = new DiscordColor(red, green, blue);
			return this;
		}

		// Token: 0x06000AE8 RID: 2792 RVA: 0x0001839C File Offset: 0x0001659C
		public DiscordEmbedBuilder AddColor(float red, float green, float blue)
		{
			this._embed.Color = new DiscordColor(red, green, blue);
			return this;
		}

		// Token: 0x06000AE9 RID: 2793 RVA: 0x000183C4 File Offset: 0x000165C4
		public DiscordEmbedBuilder AddColor(double red, double green, double blue)
		{
			this._embed.Color = new DiscordColor(red, green, blue);
			return this;
		}

		// Token: 0x06000AEA RID: 2794 RVA: 0x000183EC File Offset: 0x000165EC
		public DiscordEmbedBuilder AddNowTimestamp()
		{
			this._embed.Timestamp = new DateTime?(DateTime.UtcNow);
			return this;
		}

		// Token: 0x06000AEB RID: 2795 RVA: 0x00018418 File Offset: 0x00016618
		public DiscordEmbedBuilder AddTimestamp(DateTime timestamp)
		{
			this._embed.Timestamp = new DateTime?(timestamp);
			return this;
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x00018440 File Offset: 0x00016640
		public DiscordEmbedBuilder AddBlankField(bool inline)
		{
			bool flag = this._embed.Fields == null;
			if (flag)
			{
				this._embed.Fields = new List<EmbedField>();
			}
			bool flag2 = this._embed.Fields.Count >= 25;
			if (flag2)
			{
				throw new InvalidEmbedException("Embeds cannot have more than 25 fields");
			}
			this._embed.Fields.Add(new EmbedField("​", "​", inline));
			return this;
		}

		// Token: 0x06000AED RID: 2797 RVA: 0x000184C0 File Offset: 0x000166C0
		public DiscordEmbedBuilder AddField(string name, string value, bool inline)
		{
			bool flag = this._embed.Fields == null;
			if (flag)
			{
				this._embed.Fields = new List<EmbedField>();
			}
			bool flag2 = this._embed.Fields.Count >= 25;
			if (flag2)
			{
				throw new InvalidEmbedException("Embeds cannot have more than 25 fields");
			}
			bool flag3 = string.IsNullOrEmpty(name);
			if (flag3)
			{
				throw new InvalidEmbedException("Embed Fields cannot have a null or empty name");
			}
			bool flag4 = string.IsNullOrEmpty(value);
			if (flag4)
			{
				throw new InvalidEmbedException("Embed Fields cannot have a null or empty value");
			}
			bool flag5 = name.Length > 256;
			if (flag5)
			{
				throw new InvalidEmbedException("Field name cannot be more than 256 characters");
			}
			bool flag6 = value.Length > 1024;
			if (flag6)
			{
				throw new InvalidEmbedException("Field value cannot be more than 1024 characters");
			}
			this._embed.Fields.Add(new EmbedField(name, value, inline));
			return this;
		}

		// Token: 0x06000AEE RID: 2798 RVA: 0x000185A4 File Offset: 0x000167A4
		public DiscordEmbedBuilder AddImage(string url, int? width = null, int? height = null, string proxyUrl = null)
		{
			bool flag = url == null;
			if (flag)
			{
				throw new ArgumentNullException("url");
			}
			this._embed.Image = new EmbedImage(url, width, height, proxyUrl);
			return this;
		}

		// Token: 0x06000AEF RID: 2799 RVA: 0x000185E0 File Offset: 0x000167E0
		public DiscordEmbedBuilder AddThumbnail(string url, int? width = null, int? height = null, string proxyUrl = null)
		{
			bool flag = url == null;
			if (flag)
			{
				throw new ArgumentNullException("url");
			}
			this._embed.Thumbnail = new EmbedThumbnail(url, width, height, proxyUrl);
			return this;
		}

		// Token: 0x06000AF0 RID: 2800 RVA: 0x0001861C File Offset: 0x0001681C
		public DiscordEmbedBuilder AddVideo(string url, int? width = null, int? height = null, string proxyUrl = null)
		{
			bool flag = url == null;
			if (flag)
			{
				throw new ArgumentNullException("url");
			}
			this._embed.Video = new EmbedVideo(url, width, height, proxyUrl);
			return this;
		}

		// Token: 0x06000AF1 RID: 2801 RVA: 0x00018658 File Offset: 0x00016858
		public DiscordEmbedBuilder AddProvider(string name, string url)
		{
			this._embed.Provider = new EmbedProvider(name, url);
			return this;
		}

		// Token: 0x06000AF2 RID: 2802 RVA: 0x00018680 File Offset: 0x00016880
		public DiscordEmbed Build()
		{
			return this._embed;
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x00018698 File Offset: 0x00016898
		public List<DiscordEmbed> BuildList()
		{
			return new List<DiscordEmbed>
			{
				this._embed
			};
		}

		// Token: 0x040006FD RID: 1789
		private readonly DiscordEmbed _embed = new DiscordEmbed();
	}
}
