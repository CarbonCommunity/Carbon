/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Permissions
{
	// Token: 0x0200005E RID: 94
	[Flags]
	public enum PermissionFlags : ulong
	{
		// Token: 0x040001C7 RID: 455
		[System.ComponentModel.Description("CREATE_INSTANT_INVITE")]
		CreateInstantInvite = 1UL,
		// Token: 0x040001C8 RID: 456
		[System.ComponentModel.Description("KICK_MEMBERS")]
		KickMembers = 2UL,
		// Token: 0x040001C9 RID: 457
		[System.ComponentModel.Description("BAN_MEMBERS")]
		BanMembers = 4UL,
		// Token: 0x040001CA RID: 458
		[System.ComponentModel.Description("ADMINISTRATOR")]
		Administrator = 8UL,
		// Token: 0x040001CB RID: 459
		[System.ComponentModel.Description("MANAGE_CHANNELS")]
		ManageChannels = 16UL,
		// Token: 0x040001CC RID: 460
		[System.ComponentModel.Description("MANAGE_GUILD")]
		ManageGuild = 32UL,
		// Token: 0x040001CD RID: 461
		[System.ComponentModel.Description("ADD_REACTIONS")]
		AddReactions = 64UL,
		// Token: 0x040001CE RID: 462
		[System.ComponentModel.Description("VIEW_AUDIT_LOG")]
		ViewAuditLog = 128UL,
		// Token: 0x040001CF RID: 463
		[System.ComponentModel.Description("PRIORITY_SPEAKER")]
		PrioritySpeaker = 256UL,
		// Token: 0x040001D0 RID: 464
		[System.ComponentModel.Description("STREAM")]
		Stream = 512UL,
		// Token: 0x040001D1 RID: 465
		[System.ComponentModel.Description("VIEW_CHANNEL")]
		ViewChannel = 1024UL,
		// Token: 0x040001D2 RID: 466
		[System.ComponentModel.Description("SEND_MESSAGES")]
		SendMessages = 2048UL,
		// Token: 0x040001D3 RID: 467
		[System.ComponentModel.Description("SEND_TTS_MESSAGES")]
		SendTtsMessages = 4096UL,
		// Token: 0x040001D4 RID: 468
		[System.ComponentModel.Description("MANAGE_MESSAGES")]
		ManageMessages = 8192UL,
		// Token: 0x040001D5 RID: 469
		[System.ComponentModel.Description("EMBED_LINKS")]
		EmbedLinks = 16384UL,
		// Token: 0x040001D6 RID: 470
		[System.ComponentModel.Description("ATTACH_FILES")]
		AttachFiles = 32768UL,
		// Token: 0x040001D7 RID: 471
		[System.ComponentModel.Description("READ_MESSAGE_HISTORY")]
		ReadMessageHistory = 65536UL,
		// Token: 0x040001D8 RID: 472
		[System.ComponentModel.Description("MENTION_EVERYONE")]
		MentionEveryone = 131072UL,
		// Token: 0x040001D9 RID: 473
		[System.ComponentModel.Description("USE_EXTERNAL_EMOJIS")]
		UseExternalEmojis = 262144UL,
		// Token: 0x040001DA RID: 474
		[System.ComponentModel.Description("VIEW_GUILD_INSIGHTS")]
		ViewGuildInsights = 524288UL,
		// Token: 0x040001DB RID: 475
		[System.ComponentModel.Description("CONNECT")]
		Connect = 1048576UL,
		// Token: 0x040001DC RID: 476
		[System.ComponentModel.Description("SPEAK")]
		Speak = 2097152UL,
		// Token: 0x040001DD RID: 477
		[System.ComponentModel.Description("MUTE_MEMBERS")]
		MuteMembers = 4194304UL,
		// Token: 0x040001DE RID: 478
		[System.ComponentModel.Description("DEAFEN_MEMBERS")]
		DeafanMembers = 8388608UL,
		// Token: 0x040001DF RID: 479
		[System.ComponentModel.Description("MOVE_MEMBERS")]
		MoveMembers = 16777216UL,
		// Token: 0x040001E0 RID: 480
		[System.ComponentModel.Description("USE_VAD")]
		UseVad = 33554432UL,
		// Token: 0x040001E1 RID: 481
		[System.ComponentModel.Description("CHANGE_NICKNAME")]
		ChangeNickname = 67108864UL,
		// Token: 0x040001E2 RID: 482
		[System.ComponentModel.Description("MANAGE_NICKNAMES")]
		ManageNicknames = 134217728UL,
		// Token: 0x040001E3 RID: 483
		[System.ComponentModel.Description("MANAGE_ROLES")]
		ManageRoles = 268435456UL,
		// Token: 0x040001E4 RID: 484
		[System.ComponentModel.Description("MANAGE_WEBHOOKS")]
		ManageWebhooks = 536870912UL,
		// Token: 0x040001E5 RID: 485
		[System.ComponentModel.Description("MANAGE_EMOJIS_AND_STICKERS")]
		ManageEmojisAndStickers = 1073741824UL,
		// Token: 0x040001E6 RID: 486
		[System.ComponentModel.Description("USE_APPLICATION_COMMANDS")]
		UseSlashCommands = 2147483648UL,
		// Token: 0x040001E7 RID: 487
		[System.ComponentModel.Description("REQUEST_TO_SPEAK")]
		RequestToSpeak = 4294967296UL,
		// Token: 0x040001E8 RID: 488
		[System.ComponentModel.Description("MANAGE_EVENTS")]
		ManageEvents = 8589934592UL,
		// Token: 0x040001E9 RID: 489
		[System.ComponentModel.Description("MANAGE_THREADS")]
		ManageThreads = 17179869184UL,
		// Token: 0x040001EA RID: 490
		[Obsolete("This flag has been deprecated and will be removed in a future update. This flag is replaced by CreatePublicThreads")]
		[System.ComponentModel.Description("USE_PUBLIC_THREADS")]
		UsePublicThreads = 34359738368UL,
		// Token: 0x040001EB RID: 491
		[System.ComponentModel.Description("CREATE_PUBLIC_THREADS")]
		CreatePublicThreads = 34359738368UL,
		// Token: 0x040001EC RID: 492
		[Obsolete("This flag has been deprecated and will be removed in a future update. This flag is replaced by CreatePrivateThreads")]
		[System.ComponentModel.Description("USE_PRIVATE_THREADS")]
		UsePrivateThreads = 68719476736UL,
		// Token: 0x040001ED RID: 493
		[System.ComponentModel.Description("CREATE_PRIVATE_THREADS")]
		CreatePrivateThreads = 68719476736UL,
		// Token: 0x040001EE RID: 494
		[System.ComponentModel.Description("USE_EXTERNAL_STICKERS")]
		UseExternalStickers = 137438953472UL,
		// Token: 0x040001EF RID: 495
		[System.ComponentModel.Description("SEND_MESSAGES_IN_THREADS")]
		SendMessagesInThreads = 274877906944UL,
		// Token: 0x040001F0 RID: 496
		[System.ComponentModel.Description("START_EMBEDDED_ACTIVITIES")]
		StartEmbeddedActivities = 549755813888UL,
		// Token: 0x040001F1 RID: 497
		[System.ComponentModel.Description("MODERATE_MEMBERS")]
		ModerateMembers = 1099511627776UL
	}
}
