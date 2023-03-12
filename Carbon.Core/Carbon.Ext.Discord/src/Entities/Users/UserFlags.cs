/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Entities.Users
{
	// Token: 0x0200004C RID: 76
	[Flags]
	public enum UserFlags
	{
		// Token: 0x04000146 RID: 326
		None = 0,
		// Token: 0x04000147 RID: 327
		Staff = 1,
		// Token: 0x04000148 RID: 328
		Partner = 2,
		// Token: 0x04000149 RID: 329
		HypeSquad = 4,
		// Token: 0x0400014A RID: 330
		BugHunterLevel1 = 8,
		// Token: 0x0400014B RID: 331
		HypeSquadOnlineHouse1 = 64,
		// Token: 0x0400014C RID: 332
		HypeSquadOnlineHouse2 = 128,
		// Token: 0x0400014D RID: 333
		HypeSquadOnlineHouse3 = 256,
		// Token: 0x0400014E RID: 334
		PremiumEarlySupporter = 512,
		// Token: 0x0400014F RID: 335
		TeamPseudoUser = 1024,
		// Token: 0x04000150 RID: 336
		BugHunterLevel2 = 16384,
		// Token: 0x04000151 RID: 337
		VerifiedBot = 65536,
		// Token: 0x04000152 RID: 338
		VerifiedDeveloper = 131072,
		// Token: 0x04000153 RID: 339
		CertifiedModerator = 262144,
		// Token: 0x04000154 RID: 340
		BotHttpInteractions = 524288,
		// Token: 0x04000155 RID: 341
		[Obsolete("Replaced with Staff. Will be removed April 2022")]
		DiscordEmployee = 1,
		// Token: 0x04000156 RID: 342
		[Obsolete("Replaced with Partner. Will be removed April 2022")]
		PartneredServerOwner = 2,
		// Token: 0x04000157 RID: 343
		[Obsolete("Replaced with HypeSquad. Will be removed April 2022")]
		HyperSquadEvents = 4,
		// Token: 0x04000158 RID: 344
		[Obsolete("Replaced with HypeSquadOnlineHouse1. Will be removed April 2022")]
		HouseBravery = 64,
		// Token: 0x04000159 RID: 345
		[Obsolete("Replaced with HypeSquadOnlineHouse2. Will be removed April 2022")]
		HouseBrilliance = 128,
		// Token: 0x0400015A RID: 346
		[Obsolete("Replaced with HypeSquadOnlineHouse3. Will be removed April 2022")]
		HouseBalance = 256,
		// Token: 0x0400015B RID: 347
		[Obsolete("Replaced with PremiumEarlySupporter. Will be removed April 2022")]
		EarlySupporter = 512,
		// Token: 0x0400015C RID: 348
		[Obsolete("Replaced with TeamPseudoUser. Will be removed April 2022")]
		TeamUser = 1024,
		// Token: 0x0400015D RID: 349
		[Obsolete("Replaced with VerifiedDeveloper. Will be removed April 2022")]
		EarlyVerifiedBotDeveloper = 131072,
		// Token: 0x0400015E RID: 350
		[Obsolete("Replaced with CertifiedModerator. Will be removed April 2022")]
		DiscordCertifiedModerator = 262144
	}
}
