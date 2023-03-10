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

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000A7 RID: 167
	[JsonConverter(typeof(DiscordEnumConverter))]
	public enum GuildFeatures
	{
		// Token: 0x040003BA RID: 954
		Unknown,
		// Token: 0x040003BB RID: 955
		[System.ComponentModel.Description("ANIMATED_ICON")]
		AnimatedIcon,
		// Token: 0x040003BC RID: 956
		[System.ComponentModel.Description("BANNER")]
		Banner,
		// Token: 0x040003BD RID: 957
		[System.ComponentModel.Description("COMMERCE")]
		Commerce,
		// Token: 0x040003BE RID: 958
		[System.ComponentModel.Description("COMMUNITY")]
		Community,
		// Token: 0x040003BF RID: 959
		[System.ComponentModel.Description("DISCOVERABLE")]
		Discoverable,
		// Token: 0x040003C0 RID: 960
		[System.ComponentModel.Description("FEATURABLE")]
		Featurable,
		// Token: 0x040003C1 RID: 961
		[System.ComponentModel.Description("INVITE_SPLASH")]
		InviteSplash,
		// Token: 0x040003C2 RID: 962
		[System.ComponentModel.Description("MEMBER_VERIFICATION_GATE_ENABLED")]
		MemberVerificationGateEnabled,
		// Token: 0x040003C3 RID: 963
		[System.ComponentModel.Description("MONETIZATION_ENABLED")]
		MonetizationEnabled,
		// Token: 0x040003C4 RID: 964
		[System.ComponentModel.Description("MORE_STICKERS")]
		MoreStickers,
		// Token: 0x040003C5 RID: 965
		[System.ComponentModel.Description("NEWS")]
		News,
		// Token: 0x040003C6 RID: 966
		[System.ComponentModel.Description("PARTNERED")]
		Partnered,
		// Token: 0x040003C7 RID: 967
		[System.ComponentModel.Description("PREVIEW_ENABLED")]
		PreviewEnabled,
		// Token: 0x040003C8 RID: 968
		[System.ComponentModel.Description("PRIVATE_THREADS")]
		PrivateThreads,
		// Token: 0x040003C9 RID: 969
		[System.ComponentModel.Description("ROLE_ICONS")]
		RoleIcons,
		// Token: 0x040003CA RID: 970
		[System.ComponentModel.Description("SEVEN_DAY_THREAD_ARCHIVE")]
		SevenDayThreadArchive,
		// Token: 0x040003CB RID: 971
		[System.ComponentModel.Description("THREE_DAY_THREAD_ARCHIVE")]
		ThreeDayThreadArchive,
		// Token: 0x040003CC RID: 972
		[System.ComponentModel.Description("TICKETED_EVENTS_ENABLED")]
		TicketedEventsEnabled,
		// Token: 0x040003CD RID: 973
		[System.ComponentModel.Description("VANITY_URL")]
		VanityUrl,
		// Token: 0x040003CE RID: 974
		[System.ComponentModel.Description("VERIFIED")]
		Verified,
		// Token: 0x040003CF RID: 975
		[System.ComponentModel.Description("VIP_REGIONS")]
		VipRegions,
		// Token: 0x040003D0 RID: 976
		[System.ComponentModel.Description("WELCOME_SCREEN_ENABLED")]
		WelcomeScreenEnabled
	}
}
