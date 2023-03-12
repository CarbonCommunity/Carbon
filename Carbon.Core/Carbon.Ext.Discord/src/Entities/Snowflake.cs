/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Helpers;
using Oxide.Ext.Discord.Helpers.Converters;

namespace Oxide.Ext.Discord.Entities
{
	// Token: 0x02000041 RID: 65
	[JsonConverter(typeof(SnowflakeConverter))]
	public struct Snowflake : IComparable<Snowflake>, IEquatable<Snowflake>, IComparable<ulong>, IEquatable<ulong>
	{
		// Token: 0x060001BB RID: 443 RVA: 0x0000DB7D File Offset: 0x0000BD7D
		public Snowflake(ulong id)
		{
			this.Id = id;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000DB87 File Offset: 0x0000BD87
		public Snowflake(string id)
		{
			this.Id = ulong.Parse(id);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000DB98 File Offset: 0x0000BD98
		public Snowflake(DateTimeOffset offset)
		{
			this.Id = (ulong)(Time.DiscordEpoch - offset).TotalMilliseconds << 22;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000DBC4 File Offset: 0x0000BDC4
		public DateTimeOffset GetCreationDate()
		{
			return Time.DiscordEpoch + TimeSpan.FromMilliseconds(this.Id >> 22);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000DBF0 File Offset: 0x0000BDF0
		public bool IsValid()
		{
			return this.Id > 0UL;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000DC0C File Offset: 0x0000BE0C
		public static bool TryParse(string value, out Snowflake snowflake)
		{
			ulong id;
			bool flag = ulong.TryParse(value, out id);
			bool result;
			if (flag)
			{
				snowflake = new Snowflake(id);
				result = true;
			}
			else
			{
				snowflake = default(Snowflake);
				result = false;
			}
			return result;
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000DC44 File Offset: 0x0000BE44
		public bool Equals(Snowflake other)
		{
			return this.Id == other.Id;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000DC64 File Offset: 0x0000BE64
		public override bool Equals(object obj)
		{
			Snowflake other = default;
			bool flag;
			if (obj is Snowflake)
			{
				other = (Snowflake)obj;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				result = this.Equals(other);
			}
			else
			{
				ulong other2 = default;
				bool flag3;
				if (obj is ulong)
				{
					other2 = (ulong)obj;
					flag3 = true;
				}
				else
				{
					flag3 = false;
				}
				bool flag4 = flag3;
				result = (flag4 && this.Equals(other2));
			}
			return result;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000DCC0 File Offset: 0x0000BEC0
		public bool Equals(ulong other)
		{
			return this.Id == other;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000DCDC File Offset: 0x0000BEDC
		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000DCFC File Offset: 0x0000BEFC
		public override string ToString()
		{
			return this.IsValid() ? this.Id.ToString() : string.Empty;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000DD2C File Offset: 0x0000BF2C
		public int CompareTo(Snowflake num)
		{
			return this.Id.CompareTo(num.Id);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000DD54 File Offset: 0x0000BF54
		public int CompareTo(ulong other)
		{
			return this.Id.CompareTo(other);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000DD78 File Offset: 0x0000BF78
		public static bool operator ==(Snowflake left, Snowflake right)
		{
			return left.Id == right.Id;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000DD98 File Offset: 0x0000BF98
		public static bool operator !=(Snowflake left, Snowflake right)
		{
			return !(left == right);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000DDB4 File Offset: 0x0000BFB4
		public static bool operator <(Snowflake left, Snowflake right)
		{
			return left.CompareTo(right) < 0;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000DDD4 File Offset: 0x0000BFD4
		public static bool operator >(Snowflake left, Snowflake right)
		{
			return left.CompareTo(right) > 0;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000DDF4 File Offset: 0x0000BFF4
		public static bool operator <=(Snowflake left, Snowflake right)
		{
			return left.CompareTo(right) <= 0;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000DE14 File Offset: 0x0000C014
		public static bool operator >=(Snowflake left, Snowflake right)
		{
			return left.CompareTo(right) >= 0;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000DE34 File Offset: 0x0000C034
		public static implicit operator ulong(Snowflake snowflake)
		{
			return snowflake.Id;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000DE3C File Offset: 0x0000C03C
		public static explicit operator Snowflake(ulong id)
		{
			return new Snowflake(id);
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000DE44 File Offset: 0x0000C044
		public static implicit operator string(Snowflake snowflake)
		{
			return snowflake.Id.ToString();
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000DE5F File Offset: 0x0000C05F
		public static explicit operator Snowflake(string id)
		{
			return new Snowflake(id);
		}

		// Token: 0x04000107 RID: 263
		public readonly ulong Id;
	}
}
