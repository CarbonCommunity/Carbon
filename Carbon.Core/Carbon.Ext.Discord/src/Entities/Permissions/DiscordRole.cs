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
using Oxide.Ext.Discord.Helpers.Cdn;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Permissions
{
	// Token: 0x0200005D RID: 93
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordRole : ISnowflakeEntity
	{
		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060002EA RID: 746 RVA: 0x0000FC62 File Offset: 0x0000DE62
		// (set) Token: 0x060002EB RID: 747 RVA: 0x0000FC6A File Offset: 0x0000DE6A
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060002EC RID: 748 RVA: 0x0000FC73 File Offset: 0x0000DE73
		// (set) Token: 0x060002ED RID: 749 RVA: 0x0000FC7B File Offset: 0x0000DE7B
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060002EE RID: 750 RVA: 0x0000FC84 File Offset: 0x0000DE84
		// (set) Token: 0x060002EF RID: 751 RVA: 0x0000FC8C File Offset: 0x0000DE8C
		[JsonProperty("color")]
		public DiscordColor Color { get; set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x0000FC95 File Offset: 0x0000DE95
		// (set) Token: 0x060002F1 RID: 753 RVA: 0x0000FC9D File Offset: 0x0000DE9D
		[JsonProperty("hoist")]
		public bool? Hoist { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060002F2 RID: 754 RVA: 0x0000FCA6 File Offset: 0x0000DEA6
		// (set) Token: 0x060002F3 RID: 755 RVA: 0x0000FCAE File Offset: 0x0000DEAE
		[JsonProperty("icon")]
		public string Icon { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060002F4 RID: 756 RVA: 0x0000FCB7 File Offset: 0x0000DEB7
		// (set) Token: 0x060002F5 RID: 757 RVA: 0x0000FCBF File Offset: 0x0000DEBF
		[JsonProperty("unicode_emoji")]
		public string UnicodeEmoji { get; set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060002F6 RID: 758 RVA: 0x0000FCC8 File Offset: 0x0000DEC8
		// (set) Token: 0x060002F7 RID: 759 RVA: 0x0000FCD0 File Offset: 0x0000DED0
		[JsonProperty("position")]
		public int Position { get; set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060002F8 RID: 760 RVA: 0x0000FCD9 File Offset: 0x0000DED9
		// (set) Token: 0x060002F9 RID: 761 RVA: 0x0000FCE1 File Offset: 0x0000DEE1
		[JsonProperty("permissions")]
		public PermissionFlags Permissions { get; set; }

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060002FA RID: 762 RVA: 0x0000FCEA File Offset: 0x0000DEEA
		// (set) Token: 0x060002FB RID: 763 RVA: 0x0000FCF2 File Offset: 0x0000DEF2
		[JsonProperty("managed")]
		public bool Managed { get; set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060002FC RID: 764 RVA: 0x0000FCFB File Offset: 0x0000DEFB
		// (set) Token: 0x060002FD RID: 765 RVA: 0x0000FD03 File Offset: 0x0000DF03
		[JsonProperty("mentionable")]
		public bool Mentionable { get; set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060002FE RID: 766 RVA: 0x0000FD0C File Offset: 0x0000DF0C
		// (set) Token: 0x060002FF RID: 767 RVA: 0x0000FD14 File Offset: 0x0000DF14
		[JsonProperty("tags")]
		public RoleTags Tags { get; set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000300 RID: 768 RVA: 0x0000FD1D File Offset: 0x0000DF1D
		public string Mention
		{
			get
			{
				return DiscordFormatting.MentionRole(this.Id);
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000301 RID: 769 RVA: 0x0000FD2A File Offset: 0x0000DF2A
		public string RoleIcon
		{
			get
			{
				return (!string.IsNullOrEmpty(this.Icon)) ? DiscordCdn.GetRoleIcon(this.Id, ImageFormat.Auto) : string.Empty;
			}
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0000FD4C File Offset: 0x0000DF4C
		public bool HasPermission(PermissionFlags perm)
		{
			return (this.Permissions & perm) == perm;
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0000FD6C File Offset: 0x0000DF6C
		public bool IsBoosterRole()
		{
			return this.Managed && this.Tags != null && this.Tags.BotId == null;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0000FDA8 File Offset: 0x0000DFA8
		internal DiscordRole UpdateRole(DiscordRole role)
		{
			DiscordRole result = (DiscordRole)base.MemberwiseClone();
			bool flag = role.Name != null;
			if (flag)
			{
				this.Name = role.Name;
			}
			this.Color = role.Color;
			this.Hoist = role.Hoist;
			this.Position = role.Position;
			this.Permissions = role.Permissions;
			this.Managed = role.Managed;
			this.Mentionable = role.Mentionable;
			bool flag2 = role.Tags != null;
			if (flag2)
			{
				this.Tags = role.Tags;
			}
			return result;
		}
	}
}
