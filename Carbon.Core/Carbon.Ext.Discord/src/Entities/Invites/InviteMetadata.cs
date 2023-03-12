using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Invites
{
	// Token: 0x02000077 RID: 119
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class InviteMetadata : DiscordInvite
	{
		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x00011146 File Offset: 0x0000F346
		// (set) Token: 0x06000441 RID: 1089 RVA: 0x0001114E File Offset: 0x0000F34E
		[JsonProperty("uses")]
		public int Uses { get; set; }

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x00011157 File Offset: 0x0000F357
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x0001115F File Offset: 0x0000F35F
		[JsonProperty("max_uses")]
		public int MaxUses { get; set; }

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x00011168 File Offset: 0x0000F368
		// (set) Token: 0x06000445 RID: 1093 RVA: 0x00011170 File Offset: 0x0000F370
		[JsonProperty("max_age")]
		public int MaxAge { get; set; }

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x00011179 File Offset: 0x0000F379
		// (set) Token: 0x06000447 RID: 1095 RVA: 0x00011181 File Offset: 0x0000F381
		[JsonProperty("temporary")]
		public bool Temporary { get; set; }

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x0001118A File Offset: 0x0000F38A
		// (set) Token: 0x06000449 RID: 1097 RVA: 0x00011192 File Offset: 0x0000F392
		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }
	}
}
