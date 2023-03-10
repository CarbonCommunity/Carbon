/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Entities.AuditLogs
{
	// Token: 0x0200010C RID: 268
	public enum AuditLogActionType
	{
		// Token: 0x04000594 RID: 1428
		GuildUpdate = 1,
		// Token: 0x04000595 RID: 1429
		ChannelCreate = 10,
		// Token: 0x04000596 RID: 1430
		ChannelUpdate,
		// Token: 0x04000597 RID: 1431
		ChannelDelete,
		// Token: 0x04000598 RID: 1432
		OverwriteCreate,
		// Token: 0x04000599 RID: 1433
		OverwriteUpdate,
		// Token: 0x0400059A RID: 1434
		OverwriteDelete,
		// Token: 0x0400059B RID: 1435
		Kick = 20,
		// Token: 0x0400059C RID: 1436
		Prune,
		// Token: 0x0400059D RID: 1437
		Ban,
		// Token: 0x0400059E RID: 1438
		Unban,
		// Token: 0x0400059F RID: 1439
		MemberUpdate,
		// Token: 0x040005A0 RID: 1440
		MemberRoleUpdate,
		// Token: 0x040005A1 RID: 1441
		MemberMove,
		// Token: 0x040005A2 RID: 1442
		MemberDisconnect,
		// Token: 0x040005A3 RID: 1443
		BotAdd,
		// Token: 0x040005A4 RID: 1444
		RoleCreate = 30,
		// Token: 0x040005A5 RID: 1445
		RoleUpdate,
		// Token: 0x040005A6 RID: 1446
		RoleDelete,
		// Token: 0x040005A7 RID: 1447
		InviteCreate = 40,
		// Token: 0x040005A8 RID: 1448
		InviteUpdate,
		// Token: 0x040005A9 RID: 1449
		InviteDelete,
		// Token: 0x040005AA RID: 1450
		WebhookCreate = 50,
		// Token: 0x040005AB RID: 1451
		WebhookUpdate,
		// Token: 0x040005AC RID: 1452
		WebhookDelete,
		// Token: 0x040005AD RID: 1453
		EmojiCreate = 60,
		// Token: 0x040005AE RID: 1454
		EmojiUpdate,
		// Token: 0x040005AF RID: 1455
		EmojiDelete,
		// Token: 0x040005B0 RID: 1456
		MessageDelete = 72,
		// Token: 0x040005B1 RID: 1457
		MessageBulkDelete,
		// Token: 0x040005B2 RID: 1458
		MessagePin,
		// Token: 0x040005B3 RID: 1459
		MessageUnpin,
		// Token: 0x040005B4 RID: 1460
		IntegrationCreate = 80,
		// Token: 0x040005B5 RID: 1461
		IntegrationUpdate,
		// Token: 0x040005B6 RID: 1462
		IntegrationDelete
	}
}
