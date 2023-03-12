/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Helpers.Converters;

namespace Oxide.Ext.Discord.WebSockets
{
	// Token: 0x02000006 RID: 6
	[JsonConverter(typeof(DiscordEnumConverter))]
	public enum DispatchCode : byte
	{
		// Token: 0x04000022 RID: 34
		Unknown,
		// Token: 0x04000023 RID: 35
		[System.ComponentModel.Description("READY")]
		Ready,
		// Token: 0x04000024 RID: 36
		[System.ComponentModel.Description("RESUMED")]
		Resumed,
		// Token: 0x04000025 RID: 37
		[System.ComponentModel.Description("CHANNEL_CREATE")]
		ChannelCreated,
		// Token: 0x04000026 RID: 38
		[System.ComponentModel.Description("CHANNEL_UPDATE")]
		ChannelUpdated,
		// Token: 0x04000027 RID: 39
		[System.ComponentModel.Description("CHANNEL_DELETE")]
		ChannelDeleted,
		// Token: 0x04000028 RID: 40
		[System.ComponentModel.Description("CHANNEL_PINS_UPDATE")]
		ChannelPinsUpdate,
		// Token: 0x04000029 RID: 41
		[System.ComponentModel.Description("GUILD_CREATE")]
		GuildCreated,
		// Token: 0x0400002A RID: 42
		[System.ComponentModel.Description("GUILD_UPDATE")]
		GuildUpdated,
		// Token: 0x0400002B RID: 43
		[System.ComponentModel.Description("GUILD_DELETE")]
		GuildDeleted,
		// Token: 0x0400002C RID: 44
		[System.ComponentModel.Description("GUILD_BAN_ADD")]
		GuildBanAdded,
		// Token: 0x0400002D RID: 45
		[System.ComponentModel.Description("GUILD_BAN_REMOVE")]
		GuildBanRemoved,
		// Token: 0x0400002E RID: 46
		[System.ComponentModel.Description("GUILD_EMOJIS_UPDATE")]
		GuildEmojisUpdated,
		// Token: 0x0400002F RID: 47
		[System.ComponentModel.Description("GUILD_STICKERS_UPDATE")]
		GuildStickersUpdate,
		// Token: 0x04000030 RID: 48
		[System.ComponentModel.Description("GUILD_INTEGRATIONS_UPDATE")]
		GuildIntegrationsUpdated,
		// Token: 0x04000031 RID: 49
		[System.ComponentModel.Description("GUILD_MEMBER_ADD")]
		GuildMemberAdded,
		// Token: 0x04000032 RID: 50
		[System.ComponentModel.Description("GUILD_MEMBER_REMOVE")]
		GuildMemberRemoved,
		// Token: 0x04000033 RID: 51
		[System.ComponentModel.Description("GUILD_MEMBER_UPDATE")]
		GuildMemberUpdated,
		// Token: 0x04000034 RID: 52
		[System.ComponentModel.Description("GUILD_MEMBERS_CHUNK")]
		GuildMembersChunk,
		// Token: 0x04000035 RID: 53
		[System.ComponentModel.Description("GUILD_ROLE_CREATE")]
		GuildRoleCreated,
		// Token: 0x04000036 RID: 54
		[System.ComponentModel.Description("GUILD_ROLE_UPDATE")]
		GuildRoleUpdated,
		// Token: 0x04000037 RID: 55
		[System.ComponentModel.Description("GUILD_ROLE_DELETE")]
		GuildRoleDeleted,
		// Token: 0x04000038 RID: 56
		[System.ComponentModel.Description("GUILD_SCHEDULED_EVENT_CREATE")]
		GuildScheduledEventCreate,
		// Token: 0x04000039 RID: 57
		[System.ComponentModel.Description("GUILD_SCHEDULED_EVENT_UPDATE")]
		GuildScheduledEventUpdate,
		// Token: 0x0400003A RID: 58
		[System.ComponentModel.Description("GUILD_SCHEDULED_EVENT_DELETE")]
		GuildScheduledEventDelete,
		// Token: 0x0400003B RID: 59
		[System.ComponentModel.Description("GUILD_SCHEDULED_EVENT_USER_ADD")]
		GuildScheduledEventUserAdd,
		// Token: 0x0400003C RID: 60
		[System.ComponentModel.Description("GUILD_SCHEDULED_EVENT_USER_REMOVE")]
		GuildScheduledEventUserRemove,
		// Token: 0x0400003D RID: 61
		[System.ComponentModel.Description("INTEGRATION_CREATE")]
		IntegrationCreated,
		// Token: 0x0400003E RID: 62
		[System.ComponentModel.Description("INTEGRATION_UPDATE")]
		IntegrationUpdated,
		// Token: 0x0400003F RID: 63
		[System.ComponentModel.Description("INTEGRATION_DELETE")]
		IntegrationDeleted,
		// Token: 0x04000040 RID: 64
		[System.ComponentModel.Description("MESSAGE_CREATE")]
		MessageCreated,
		// Token: 0x04000041 RID: 65
		[System.ComponentModel.Description("MESSAGE_UPDATE")]
		MessageUpdated,
		// Token: 0x04000042 RID: 66
		[System.ComponentModel.Description("MESSAGE_DELETE")]
		MessageDeleted,
		// Token: 0x04000043 RID: 67
		[System.ComponentModel.Description("MESSAGE_DELETE_BULK")]
		MessageBulkDeleted,
		// Token: 0x04000044 RID: 68
		[System.ComponentModel.Description("MESSAGE_REACTION_ADD")]
		MessageReactionAdded,
		// Token: 0x04000045 RID: 69
		[System.ComponentModel.Description("MESSAGE_REACTION_REMOVE")]
		MessageReactionRemoved,
		// Token: 0x04000046 RID: 70
		[System.ComponentModel.Description("MESSAGE_REACTION_REMOVE_ALL")]
		MessageReactionAllRemoved,
		// Token: 0x04000047 RID: 71
		[System.ComponentModel.Description("MESSAGE_REACTION_REMOVE_EMOJI")]
		MessageReactionEmojiRemoved,
		// Token: 0x04000048 RID: 72
		[System.ComponentModel.Description("PRESENCE_UPDATE")]
		PresenceUpdated,
		// Token: 0x04000049 RID: 73
		[System.ComponentModel.Description("PRESENCES_REPLACE")]
		PresenceReplace,
		// Token: 0x0400004A RID: 74
		[System.ComponentModel.Description("TYPING_START")]
		TypingStarted,
		// Token: 0x0400004B RID: 75
		[System.ComponentModel.Description("USER_UPDATE")]
		UserUpdated,
		// Token: 0x0400004C RID: 76
		[System.ComponentModel.Description("VOICE_STATE_UPDATE")]
		VoiceStateUpdated,
		// Token: 0x0400004D RID: 77
		[System.ComponentModel.Description("VOICE_SERVER_UPDATE")]
		VoiceServerUpdated,
		// Token: 0x0400004E RID: 78
		[System.ComponentModel.Description("WEBHOOKS_UPDATE")]
		WebhooksUpdated,
		// Token: 0x0400004F RID: 79
		[System.ComponentModel.Description("INVITE_CREATE")]
		InviteCreated,
		// Token: 0x04000050 RID: 80
		[System.ComponentModel.Description("INVITE_DELETE")]
		InviteDeleted,
		// Token: 0x04000051 RID: 81
		[System.ComponentModel.Description("INTERACTION_CREATE")]
		InteractionCreated,
		// Token: 0x04000052 RID: 82
		[System.ComponentModel.Description("GUILD_JOIN_REQUEST_DELETE")]
		GuildJoinRequestDeleted,
		// Token: 0x04000053 RID: 83
		[System.ComponentModel.Description("THREAD_CREATE")]
		ThreadCreated,
		// Token: 0x04000054 RID: 84
		[System.ComponentModel.Description("THREAD_UPDATE")]
		ThreadUpdated,
		// Token: 0x04000055 RID: 85
		[System.ComponentModel.Description("THREAD_DELETE")]
		ThreadDeleted,
		// Token: 0x04000056 RID: 86
		[System.ComponentModel.Description("THREAD_LIST_SYNC")]
		ThreadListSync,
		// Token: 0x04000057 RID: 87
		[System.ComponentModel.Description("THREAD_MEMBER_UPDATE")]
		ThreadMemberUpdated,
		// Token: 0x04000058 RID: 88
		[System.ComponentModel.Description("THREAD_MEMBERS_UPDATE")]
		ThreadMembersUpdated,
		// Token: 0x04000059 RID: 89
		[System.ComponentModel.Description("STAGE_INSTANCE_CREATE")]
		StageInstanceCreated,
		// Token: 0x0400005A RID: 90
		[System.ComponentModel.Description("STAGE_INSTANCE_UPDATE")]
		StageInstanceUpdated,
		// Token: 0x0400005B RID: 91
		[System.ComponentModel.Description("STAGE_INSTANCE_DELETE")]
		StageInstanceDeleted
	}
}
