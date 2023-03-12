/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Globalization;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Entities.Permissions
{
	// Token: 0x0200005B RID: 91
	[JsonConverter(typeof(DiscordColorConverter))]
	public struct DiscordColor
	{
		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060002DC RID: 732 RVA: 0x0000F7BB File Offset: 0x0000D9BB
		public uint Color { get; }

		// Token: 0x060002DD RID: 733 RVA: 0x0000F7C4 File Offset: 0x0000D9C4
		public DiscordColor(uint color)
		{
			bool flag = color > 16777215U;
			if (flag)
			{
				throw new InvalidDiscordColorException(string.Format("Color '{0}' is greater than the max color of 0xFFFFFF", color));
			}
			Color = color;
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0000F7FC File Offset: 0x0000D9FC
		public DiscordColor(string color)
		{
			this = new DiscordColor(uint.Parse(color.TrimStart(new char[]
			{
				'#'
			}), NumberStyles.AllowHexSpecifier));
		}

		// Token: 0x060002DF RID: 735 RVA: 0x0000F821 File Offset: 0x0000DA21
		public DiscordColor(byte red, byte green, byte blue)
		{
			Color = (uint)(((int)red << 16) + ((int)green << 8) + (int)blue);
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0000F834 File Offset: 0x0000DA34
		public DiscordColor(int red, int green, int blue)
		{
			bool flag = red < 0 || red > 255;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("red", "Value must be between 0 - 255");
			}
			bool flag2 = green < 0 || green > 255;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("green", "Value must be between 0 - 255");
			}
			bool flag3 = blue < 0 || blue > 255;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("blue", "Value must be between 0 - 255");
			}
			Color = (uint)((red << 16) + (green << 8) + blue);
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0000F8C0 File Offset: 0x0000DAC0
		public DiscordColor(uint red, uint green, uint blue)
		{
			bool flag = red > 255U;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("red", "Value must be < 255");
			}
			bool flag2 = green > 255U;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("green", "Value must be < 255");
			}
			bool flag3 = blue > 255U;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("blue", "Value must be < 255");
			}
			Color = (red << 16) + (green << 8) + blue;
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000F938 File Offset: 0x0000DB38
		public DiscordColor(float red, float green, float blue)
		{
			bool flag = red < 0f || red > 1f;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("red", "Value must be between 0 - 1");
			}
			bool flag2 = green < 0f || green > 1f;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("green", "Value must be between 0 - 1");
			}
			bool flag3 = blue < 0f || blue > 1f;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("blue", "Value must be between 0 - 1");
			}
			Color = ((uint)(red * 255f) << 16) + ((uint)(green * 255f) << 8) + (uint)(blue * 255f);
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000F9E3 File Offset: 0x0000DBE3
		public DiscordColor(double red, double green, double blue)
		{
			this = new DiscordColor((float)red, (float)green, (float)blue);
		}

		// Token: 0x040001A5 RID: 421
		public static readonly DiscordColor Default = new DiscordColor(0U);

		// Token: 0x040001A6 RID: 422
		public static readonly DiscordColor Teal = new DiscordColor(1752220U);

		// Token: 0x040001A7 RID: 423
		public static readonly DiscordColor DarkTeal = new DiscordColor(1146986U);

		// Token: 0x040001A8 RID: 424
		public static readonly DiscordColor Green = new DiscordColor(3066993U);

		// Token: 0x040001A9 RID: 425
		public static readonly DiscordColor DarkGreen = new DiscordColor(2067276U);

		// Token: 0x040001AA RID: 426
		public static readonly DiscordColor Blue = new DiscordColor(3447003U);

		// Token: 0x040001AB RID: 427
		public static readonly DiscordColor DarkBlue = new DiscordColor(2123412U);

		// Token: 0x040001AC RID: 428
		public static readonly DiscordColor Purple = new DiscordColor(10181046U);

		// Token: 0x040001AD RID: 429
		public static readonly DiscordColor DarkPurple = new DiscordColor(7419530U);

		// Token: 0x040001AE RID: 430
		public static readonly DiscordColor Magenta = new DiscordColor(15277667U);

		// Token: 0x040001AF RID: 431
		public static readonly DiscordColor DarkMagenta = new DiscordColor(11342935U);

		// Token: 0x040001B0 RID: 432
		public static readonly DiscordColor Gold = new DiscordColor(15844367U);

		// Token: 0x040001B1 RID: 433
		public static readonly DiscordColor LightOrange = new DiscordColor(12745742U);

		// Token: 0x040001B2 RID: 434
		public static readonly DiscordColor Orange = new DiscordColor(15105570U);

		// Token: 0x040001B3 RID: 435
		public static readonly DiscordColor DarkOrange = new DiscordColor(11027200U);

		// Token: 0x040001B4 RID: 436
		public static readonly DiscordColor Red = new DiscordColor(15158332U);

		// Token: 0x040001B5 RID: 437
		public static readonly DiscordColor DarkRed = new DiscordColor(10038562U);

		// Token: 0x040001B6 RID: 438
		public static readonly DiscordColor LightGrey = new DiscordColor(9936031U);

		// Token: 0x040001B7 RID: 439
		public static readonly DiscordColor LighterGrey = new DiscordColor(9807270U);

		// Token: 0x040001B8 RID: 440
		public static readonly DiscordColor DarkGrey = new DiscordColor(6323595U);

		// Token: 0x040001B9 RID: 441
		public static readonly DiscordColor DarkerGrey = new DiscordColor(5533306U);
	}
}
