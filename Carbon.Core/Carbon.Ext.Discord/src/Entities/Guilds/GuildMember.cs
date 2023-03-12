/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000A9 RID: 169
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildMember : ISnowflakeEntity
	{
		// Token: 0x17000203 RID: 515
		// (get) Token: 0x0600066A RID: 1642 RVA: 0x00014270 File Offset: 0x00012470
		public Snowflake Id
		{
			get
			{
				DiscordUser user = this.User;
				return (user != null) ? user.Id : default(Snowflake);
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x0600066B RID: 1643 RVA: 0x00014297 File Offset: 0x00012497
		// (set) Token: 0x0600066C RID: 1644 RVA: 0x0001429F File Offset: 0x0001249F
		[JsonProperty("user")]
		public DiscordUser User { get; set; }

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x0600066D RID: 1645 RVA: 0x000142A8 File Offset: 0x000124A8
		// (set) Token: 0x0600066E RID: 1646 RVA: 0x000142B0 File Offset: 0x000124B0
		[JsonProperty("nick")]
		public string Nickname { get; set; }

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x0600066F RID: 1647 RVA: 0x000142B9 File Offset: 0x000124B9
		// (set) Token: 0x06000670 RID: 1648 RVA: 0x000142C1 File Offset: 0x000124C1
		[JsonProperty("avatar")]
		public string Avatar { get; set; }

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06000671 RID: 1649 RVA: 0x000142CA File Offset: 0x000124CA
		// (set) Token: 0x06000672 RID: 1650 RVA: 0x000142D2 File Offset: 0x000124D2
		[JsonProperty("roles")]
		public List<Snowflake> Roles { get; set; }

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06000673 RID: 1651 RVA: 0x000142DB File Offset: 0x000124DB
		// (set) Token: 0x06000674 RID: 1652 RVA: 0x000142E3 File Offset: 0x000124E3
		[JsonProperty("joined_at")]
		public DateTime? JoinedAt { get; set; }

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06000675 RID: 1653 RVA: 0x000142EC File Offset: 0x000124EC
		// (set) Token: 0x06000676 RID: 1654 RVA: 0x000142F4 File Offset: 0x000124F4
		[JsonProperty("premium_since")]
		public DateTime? PremiumSince { get; set; }

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06000677 RID: 1655 RVA: 0x000142FD File Offset: 0x000124FD
		// (set) Token: 0x06000678 RID: 1656 RVA: 0x00014305 File Offset: 0x00012505
		[JsonProperty("permissions")]
		public string Permissions { get; set; }

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06000679 RID: 1657 RVA: 0x0001430E File Offset: 0x0001250E
		// (set) Token: 0x0600067A RID: 1658 RVA: 0x00014316 File Offset: 0x00012516
		[JsonProperty("deaf")]
		public bool Deaf { get; set; }

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x0600067B RID: 1659 RVA: 0x0001431F File Offset: 0x0001251F
		// (set) Token: 0x0600067C RID: 1660 RVA: 0x00014327 File Offset: 0x00012527
		[JsonProperty("mute")]
		public bool Mute { get; set; }

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x0600067D RID: 1661 RVA: 0x00014330 File Offset: 0x00012530
		// (set) Token: 0x0600067E RID: 1662 RVA: 0x00014338 File Offset: 0x00012538
		[JsonProperty("pending")]
		public bool? Pending { get; set; }

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x0600067F RID: 1663 RVA: 0x00014341 File Offset: 0x00012541
		// (set) Token: 0x06000680 RID: 1664 RVA: 0x00014349 File Offset: 0x00012549
		[JsonProperty("communication_disabled_until")]
		public DateTime? CommunicationDisabledUntil { get; set; }

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06000681 RID: 1665 RVA: 0x00014352 File Offset: 0x00012552
		public string DisplayName
		{
			get
			{
				string result;
				if (!string.IsNullOrEmpty(this.Nickname))
				{
					result = this.Nickname;
				}
				else
				{
					DiscordUser user = this.User;
					result = ((user != null) ? user.Username : null);
				}
				return result;
			}
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x0001437C File Offset: 0x0001257C
		public bool HasRole(DiscordRole role)
		{
			bool flag = role == null;
			if (flag)
			{
				throw new ArgumentNullException("role");
			}
			return this.HasRole(role.Id);
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x000143B0 File Offset: 0x000125B0
		public bool HasRole(Snowflake roleId)
		{
			return this.Roles.Contains(roleId);
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x000143D0 File Offset: 0x000125D0
		internal GuildMember Update(GuildMember update)
		{
			GuildMember guildMember = (GuildMember)base.MemberwiseClone();
			bool flag = update.User != null;
			if (flag)
			{
				guildMember.User = this.User.Update(update.User);
			}
			bool flag2 = update.Nickname != null;
			if (flag2)
			{
				this.Nickname = update.Nickname;
			}
			bool flag3 = update.Roles != null;
			if (flag3)
			{
				this.Roles = update.Roles;
			}
			bool flag4 = update.PremiumSince != null;
			if (flag4)
			{
				this.PremiumSince = update.PremiumSince;
			}
			this.Deaf = update.Deaf;
			this.Mute = update.Mute;
			bool flag5 = update.Pending != null;
			if (flag5)
			{
				this.Pending = update.Pending;
			}
			bool flag6 = update.Permissions != null;
			if (flag6)
			{
				this.Permissions = update.Permissions;
			}
			this.CommunicationDisabledUntil = update.CommunicationDisabledUntil;
			return guildMember;
		}
	}
}
