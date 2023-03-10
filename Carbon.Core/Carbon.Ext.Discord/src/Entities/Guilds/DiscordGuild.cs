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
using Newtonsoft.Json.Linq;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Channels.Stages;
using Oxide.Ext.Discord.Entities.Channels.Threads;
using Oxide.Ext.Discord.Entities.Emojis;
using Oxide.Ext.Discord.Entities.Gatway.Events;
using Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents;
using Oxide.Ext.Discord.Entities.Integrations;
using Oxide.Ext.Discord.Entities.Invites;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Entities.Stickers;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Entities.Voice;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Helpers.Cdn;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Ext.Discord.Interfaces;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000A1 RID: 161
	[JsonObject(MemberSerialization = ( MemberSerialization )1 )]
	public class DiscordGuild : ISnowflakeEntity
	{
		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x0600058F RID: 1423 RVA: 0x0001247B File Offset: 0x0001067B
		// (set) Token: 0x06000590 RID: 1424 RVA: 0x00012483 File Offset: 0x00010683
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000591 RID: 1425 RVA: 0x0001248C File Offset: 0x0001068C
		// (set) Token: 0x06000592 RID: 1426 RVA: 0x00012494 File Offset: 0x00010694
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000593 RID: 1427 RVA: 0x0001249D File Offset: 0x0001069D
		// (set) Token: 0x06000594 RID: 1428 RVA: 0x000124A5 File Offset: 0x000106A5
		[JsonProperty("icon")]
		public string Icon { get; set; }

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000595 RID: 1429 RVA: 0x000124AE File Offset: 0x000106AE
		// (set) Token: 0x06000596 RID: 1430 RVA: 0x000124B6 File Offset: 0x000106B6
		[JsonProperty("icon_Hash")]
		public string IconHash { get; set; }

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06000597 RID: 1431 RVA: 0x000124BF File Offset: 0x000106BF
		// (set) Token: 0x06000598 RID: 1432 RVA: 0x000124C7 File Offset: 0x000106C7
		[JsonProperty("splash")]
		public string Splash { get; set; }

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06000599 RID: 1433 RVA: 0x000124D0 File Offset: 0x000106D0
		// (set) Token: 0x0600059A RID: 1434 RVA: 0x000124D8 File Offset: 0x000106D8
		[JsonProperty("discovery_splash")]
		public string DiscoverySplash { get; set; }

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x0600059B RID: 1435 RVA: 0x000124E1 File Offset: 0x000106E1
		// (set) Token: 0x0600059C RID: 1436 RVA: 0x000124E9 File Offset: 0x000106E9
		[JsonProperty("owner")]
		public bool? Owner { get; set; }

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x0600059D RID: 1437 RVA: 0x000124F2 File Offset: 0x000106F2
		// (set) Token: 0x0600059E RID: 1438 RVA: 0x000124FA File Offset: 0x000106FA
		[JsonProperty("owner_id")]
		public Snowflake OwnerId { get; set; }

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x00012503 File Offset: 0x00010703
		// (set) Token: 0x060005A0 RID: 1440 RVA: 0x0001250B File Offset: 0x0001070B
		[JsonProperty("permissions")]
		public string Permissions { get; set; }

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x060005A1 RID: 1441 RVA: 0x00012514 File Offset: 0x00010714
		// (set) Token: 0x060005A2 RID: 1442 RVA: 0x0001251C File Offset: 0x0001071C
		[JsonProperty("afk_channel_id")]
		public Snowflake? AfkChannelId { get; set; }

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x00012525 File Offset: 0x00010725
		// (set) Token: 0x060005A4 RID: 1444 RVA: 0x0001252D File Offset: 0x0001072D
		[JsonProperty("afk_timeout")]
		public int? AfkTimeout { get; set; }

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x060005A5 RID: 1445 RVA: 0x00012536 File Offset: 0x00010736
		// (set) Token: 0x060005A6 RID: 1446 RVA: 0x0001253E File Offset: 0x0001073E
		[JsonProperty("widget_enabled")]
		public bool? WidgetEnabled { get; set; }

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x00012547 File Offset: 0x00010747
		// (set) Token: 0x060005A8 RID: 1448 RVA: 0x0001254F File Offset: 0x0001074F
		[JsonProperty("widget_channel_id")]
		public Snowflake? WidgetChannelId { get; set; }

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x060005A9 RID: 1449 RVA: 0x00012558 File Offset: 0x00010758
		// (set) Token: 0x060005AA RID: 1450 RVA: 0x00012560 File Offset: 0x00010760
		[JsonProperty("verification_level")]
		public GuildVerificationLevel? VerificationLevel { get; set; }

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x060005AB RID: 1451 RVA: 0x00012569 File Offset: 0x00010769
		// (set) Token: 0x060005AC RID: 1452 RVA: 0x00012571 File Offset: 0x00010771
		[JsonProperty("default_message_notifications")]
		public DefaultNotificationLevel? DefaultMessageNotifications { get; set; }

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x060005AD RID: 1453 RVA: 0x0001257A File Offset: 0x0001077A
		// (set) Token: 0x060005AE RID: 1454 RVA: 0x00012582 File Offset: 0x00010782
		[JsonProperty("explicit_content_filter")]
		public ExplicitContentFilterLevel? ExplicitContentFilter { get; set; }

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x060005AF RID: 1455 RVA: 0x0001258B File Offset: 0x0001078B
		// (set) Token: 0x060005B0 RID: 1456 RVA: 0x00012593 File Offset: 0x00010793
		[JsonConverter(typeof(HashListConverter<DiscordRole>))]
		[JsonProperty("roles")]
		public Hash<Snowflake, DiscordRole> Roles { get; set; }

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x060005B1 RID: 1457 RVA: 0x0001259C File Offset: 0x0001079C
		// (set) Token: 0x060005B2 RID: 1458 RVA: 0x000125A4 File Offset: 0x000107A4
		[JsonConverter(typeof(HashListConverter<DiscordEmoji>))]
		[JsonProperty("emojis")]
		public Hash<Snowflake, DiscordEmoji> Emojis { get; set; }

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x060005B3 RID: 1459 RVA: 0x000125AD File Offset: 0x000107AD
		// (set) Token: 0x060005B4 RID: 1460 RVA: 0x000125B5 File Offset: 0x000107B5
		[JsonProperty("features")]
		public List<GuildFeatures> Features { get; set; }

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x060005B5 RID: 1461 RVA: 0x000125BE File Offset: 0x000107BE
		// (set) Token: 0x060005B6 RID: 1462 RVA: 0x000125C6 File Offset: 0x000107C6
		[JsonProperty("mfa_level")]
		public GuildMFALevel? MfaLevel { get; set; }

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x060005B7 RID: 1463 RVA: 0x000125CF File Offset: 0x000107CF
		// (set) Token: 0x060005B8 RID: 1464 RVA: 0x000125D7 File Offset: 0x000107D7
		[JsonProperty("application_id")]
		public Snowflake? ApplicationId { get; set; }

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x060005B9 RID: 1465 RVA: 0x000125E0 File Offset: 0x000107E0
		// (set) Token: 0x060005BA RID: 1466 RVA: 0x000125E8 File Offset: 0x000107E8
		[JsonProperty("system_channel_id")]
		public Snowflake? SystemChannelId { get; set; }

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x060005BB RID: 1467 RVA: 0x000125F1 File Offset: 0x000107F1
		// (set) Token: 0x060005BC RID: 1468 RVA: 0x000125F9 File Offset: 0x000107F9
		[JsonProperty("system_channel_flags")]
		public SystemChannelFlags SystemChannelFlags { get; set; }

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x060005BD RID: 1469 RVA: 0x00012602 File Offset: 0x00010802
		// (set) Token: 0x060005BE RID: 1470 RVA: 0x0001260A File Offset: 0x0001080A
		[JsonProperty("rules_channel_id")]
		public Snowflake? RulesChannelId { get; set; }

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x00012613 File Offset: 0x00010813
		// (set) Token: 0x060005C0 RID: 1472 RVA: 0x0001261B File Offset: 0x0001081B
		[JsonProperty("joined_at")]
		public DateTime? JoinedAt { get; set; }

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x00012624 File Offset: 0x00010824
		// (set) Token: 0x060005C2 RID: 1474 RVA: 0x0001262C File Offset: 0x0001082C
		[JsonProperty("large")]
		public bool? Large { get; set; }

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x060005C3 RID: 1475 RVA: 0x00012635 File Offset: 0x00010835
		// (set) Token: 0x060005C4 RID: 1476 RVA: 0x0001263D File Offset: 0x0001083D
		[JsonProperty("unavailable")]
		public bool? Unavailable { get; set; }

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x060005C5 RID: 1477 RVA: 0x00012646 File Offset: 0x00010846
		// (set) Token: 0x060005C6 RID: 1478 RVA: 0x0001264E File Offset: 0x0001084E
		[JsonProperty("member_count")]
		public int? MemberCount { get; set; }

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x060005C7 RID: 1479 RVA: 0x00012657 File Offset: 0x00010857
		// (set) Token: 0x060005C8 RID: 1480 RVA: 0x0001265F File Offset: 0x0001085F
		[JsonConverter(typeof(HashListConverter<VoiceState>))]
		[JsonProperty("voice_states")]
		public Hash<Snowflake, VoiceState> VoiceStates { get; set; }

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x060005C9 RID: 1481 RVA: 0x00012668 File Offset: 0x00010868
		// (set) Token: 0x060005CA RID: 1482 RVA: 0x00012670 File Offset: 0x00010870
		[JsonConverter(typeof(HashListConverter<GuildMember>))]
		[JsonProperty("members")]
		public Hash<Snowflake, GuildMember> Members { get; set; }

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x060005CB RID: 1483 RVA: 0x00012679 File Offset: 0x00010879
		// (set) Token: 0x060005CC RID: 1484 RVA: 0x00012681 File Offset: 0x00010881
		[JsonConverter(typeof(HashListConverter<DiscordChannel>))]
		[JsonProperty("channels")]
		public Hash<Snowflake, DiscordChannel> Channels { get; set; }

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x060005CD RID: 1485 RVA: 0x0001268A File Offset: 0x0001088A
		// (set) Token: 0x060005CE RID: 1486 RVA: 0x00012692 File Offset: 0x00010892
		[JsonConverter(typeof(HashListConverter<DiscordChannel>))]
		[JsonProperty("threads")]
		public Hash<Snowflake, DiscordChannel> Threads { get; set; }

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x060005CF RID: 1487 RVA: 0x0001269B File Offset: 0x0001089B
		// (set) Token: 0x060005D0 RID: 1488 RVA: 0x000126A3 File Offset: 0x000108A3
		[JsonProperty("presences")]
		public List<PresenceUpdatedEvent> Presences { get; set; }

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x060005D1 RID: 1489 RVA: 0x000126AC File Offset: 0x000108AC
		// (set) Token: 0x060005D2 RID: 1490 RVA: 0x000126B4 File Offset: 0x000108B4
		[JsonProperty("max_presences")]
		public int? MaxPresences { get; set; }

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x060005D3 RID: 1491 RVA: 0x000126BD File Offset: 0x000108BD
		// (set) Token: 0x060005D4 RID: 1492 RVA: 0x000126C5 File Offset: 0x000108C5
		[JsonProperty("max_members")]
		public int? MaxMembers { get; set; }

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x060005D5 RID: 1493 RVA: 0x000126CE File Offset: 0x000108CE
		// (set) Token: 0x060005D6 RID: 1494 RVA: 0x000126D6 File Offset: 0x000108D6
		[JsonProperty("vanity_url_code")]
		public string VanityUrlCode { get; set; }

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x060005D7 RID: 1495 RVA: 0x000126DF File Offset: 0x000108DF
		// (set) Token: 0x060005D8 RID: 1496 RVA: 0x000126E7 File Offset: 0x000108E7
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x060005D9 RID: 1497 RVA: 0x000126F0 File Offset: 0x000108F0
		// (set) Token: 0x060005DA RID: 1498 RVA: 0x000126F8 File Offset: 0x000108F8
		[JsonProperty("banner")]
		public string Banner { get; set; }

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x060005DB RID: 1499 RVA: 0x00012701 File Offset: 0x00010901
		// (set) Token: 0x060005DC RID: 1500 RVA: 0x00012709 File Offset: 0x00010909
		[JsonProperty("premium_tier")]
		public GuildPremiumTier? PremiumTier { get; set; }

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x060005DD RID: 1501 RVA: 0x00012712 File Offset: 0x00010912
		// (set) Token: 0x060005DE RID: 1502 RVA: 0x0001271A File Offset: 0x0001091A
		[JsonProperty("premium_subscription_count")]
		public int? PremiumSubscriptionCount { get; set; }

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x060005DF RID: 1503 RVA: 0x00012723 File Offset: 0x00010923
		// (set) Token: 0x060005E0 RID: 1504 RVA: 0x0001272B File Offset: 0x0001092B
		[JsonProperty("preferred_locale")]
		public string PreferredLocale { get; set; }

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x060005E1 RID: 1505 RVA: 0x00012734 File Offset: 0x00010934
		// (set) Token: 0x060005E2 RID: 1506 RVA: 0x0001273C File Offset: 0x0001093C
		[JsonProperty("public_updates_channel_id")]
		public Snowflake? PublicUpdatesChannelId { get; set; }

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x060005E3 RID: 1507 RVA: 0x00012745 File Offset: 0x00010945
		// (set) Token: 0x060005E4 RID: 1508 RVA: 0x0001274D File Offset: 0x0001094D
		[JsonProperty("max_video_channel_users")]
		public int? MaxVideoChannelUsers { get; set; }

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x060005E5 RID: 1509 RVA: 0x00012756 File Offset: 0x00010956
		// (set) Token: 0x060005E6 RID: 1510 RVA: 0x0001275E File Offset: 0x0001095E
		[JsonProperty("approximate_member_count")]
		public int? ApproximateMemberCount { get; set; }

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x060005E7 RID: 1511 RVA: 0x00012767 File Offset: 0x00010967
		// (set) Token: 0x060005E8 RID: 1512 RVA: 0x0001276F File Offset: 0x0001096F
		[JsonProperty("approximate_presence_count")]
		public int? ApproximatePresenceCount { get; set; }

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x060005E9 RID: 1513 RVA: 0x00012778 File Offset: 0x00010978
		// (set) Token: 0x060005EA RID: 1514 RVA: 0x00012780 File Offset: 0x00010980
		[JsonProperty("welcome_screen")]
		public GuildWelcomeScreen WelcomeScreen { get; set; }

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x060005EB RID: 1515 RVA: 0x00012789 File Offset: 0x00010989
		// (set) Token: 0x060005EC RID: 1516 RVA: 0x00012791 File Offset: 0x00010991
		[JsonProperty("nsfw_level")]
		public GuildNsfwLevel NsfwLevel { get; set; }

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x060005ED RID: 1517 RVA: 0x0001279A File Offset: 0x0001099A
		// (set) Token: 0x060005EE RID: 1518 RVA: 0x000127A2 File Offset: 0x000109A2
		[JsonConverter(typeof(HashListConverter<StageInstance>))]
		[JsonProperty("stage_instances")]
		public Hash<Snowflake, StageInstance> StageInstances { get; set; }

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x060005EF RID: 1519 RVA: 0x000127AB File Offset: 0x000109AB
		// (set) Token: 0x060005F0 RID: 1520 RVA: 0x000127B3 File Offset: 0x000109B3
		[JsonConverter(typeof(HashListConverter<DiscordSticker>))]
		[JsonProperty("stickers")]
		public Hash<Snowflake, DiscordSticker> Stickers { get; set; }

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x060005F1 RID: 1521 RVA: 0x000127BC File Offset: 0x000109BC
		// (set) Token: 0x060005F2 RID: 1522 RVA: 0x000127C4 File Offset: 0x000109C4
		[JsonConverter(typeof(HashListConverter<GuildScheduledEvent>))]
		[JsonProperty("guild_scheduled_events")]
		public Hash<Snowflake, GuildScheduledEvent> ScheduledEvents { get; set; }

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x060005F3 RID: 1523 RVA: 0x000127CD File Offset: 0x000109CD
		// (set) Token: 0x060005F4 RID: 1524 RVA: 0x000127D5 File Offset: 0x000109D5
		[JsonProperty("premium_progress_bar_enabled")]
		public bool PremiumProgressBarEnabled { get; set; }

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x060005F5 RID: 1525 RVA: 0x000127DE File Offset: 0x000109DE
		// (set) Token: 0x060005F6 RID: 1526 RVA: 0x000127E6 File Offset: 0x000109E6
		public bool HasLoadedAllMembers { get; internal set; }

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x060005F7 RID: 1527 RVA: 0x000127F0 File Offset: 0x000109F0
		public bool IsAvailable
		{
			get
			{
				return this.Unavailable != null && !this.Unavailable.Value;
			}
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x060005F8 RID: 1528 RVA: 0x00012821 File Offset: 0x00010A21
		public string IconUrl
		{
			get
			{
				return DiscordCdn.GetGuildIconUrl(this.Id, this.Icon, ImageFormat.Auto);
			}
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x060005F9 RID: 1529 RVA: 0x00012835 File Offset: 0x00010A35
		public string SplashUrl
		{
			get
			{
				return DiscordCdn.GetGuildSplashUrl(this.Id, this.Splash, ImageFormat.Auto);
			}
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x060005FA RID: 1530 RVA: 0x00012849 File Offset: 0x00010A49
		public string DiscoverySplashUrl
		{
			get
			{
				return DiscordCdn.GetGuildDiscoverySplashUrl(this.Id, this.DiscoverySplash, ImageFormat.Auto);
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x060005FB RID: 1531 RVA: 0x0001285D File Offset: 0x00010A5D
		public string BannerUrl
		{
			get
			{
				return DiscordCdn.GetGuildBannerUrl(this.Id, this.Banner, ImageFormat.Auto);
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x060005FC RID: 1532 RVA: 0x00012871 File Offset: 0x00010A71
		public DiscordRole EveryoneRole
		{
			get
			{
				return this.Roles[this.Id];
			}
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x00012884 File Offset: 0x00010A84
		public DiscordChannel GetChannel(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			foreach (DiscordChannel discordChannel in this.Channels.Values)
			{
				bool flag2 = discordChannel.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					return discordChannel;
				}
			}
			return null;
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x00012908 File Offset: 0x00010B08
		public DiscordChannel GetParentChannel(DiscordChannel channel)
		{
			bool flag = channel.ParentId == null || !channel.ParentId.Value.IsValid();
			DiscordChannel result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this.Channels[channel.ParentId.Value];
			}
			return result;
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x00012968 File Offset: 0x00010B68
		public DiscordRole GetRole(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			foreach (DiscordRole discordRole in this.Roles.Values)
			{
				bool flag2 = discordRole.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					return discordRole;
				}
			}
			return null;
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x000129EC File Offset: 0x00010BEC
		public DiscordRole GetBoosterRole()
		{
			foreach (DiscordRole discordRole in this.Roles.Values)
			{
				bool flag = discordRole.IsBoosterRole();
				if (flag)
				{
					return discordRole;
				}
			}
			return null;
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x00012A50 File Offset: 0x00010C50
		public GuildMember GetMember(string userName)
		{
			bool flag = userName == null;
			if (flag)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag2 = userName.Contains("#");
			GuildMember result;
			if (flag2)
			{
				string[] array = userName.Split(new char[]
				{
					'#'
				});
				userName = array[0];
				string b = array[1];
				foreach (GuildMember guildMember in this.Members.Values)
				{
					bool flag3 = guildMember.User.Username.Equals(userName, StringComparison.OrdinalIgnoreCase) && guildMember.User.Discriminator == b;
					if (flag3)
					{
						return guildMember;
					}
				}
				result = null;
			}
			else
			{
				foreach (GuildMember guildMember2 in this.Members.Values)
				{
					bool flag4 = guildMember2.User.Username.Equals(userName, StringComparison.OrdinalIgnoreCase);
					if (flag4)
					{
						return guildMember2;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x00012B90 File Offset: 0x00010D90
		public DiscordEmoji GetEmoji(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			foreach (DiscordEmoji discordEmoji in this.Emojis.Values)
			{
				bool flag2 = discordEmoji.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					return discordEmoji;
				}
			}
			return null;
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x00012C14 File Offset: 0x00010E14
		public static void CreateGuild(DiscordClient client, GuildCreate create, Action<DiscordGuild> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordGuild>("/guilds", RequestMethod.POST, create, callback, error);
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x00012C34 File Offset: 0x00010E34
		public static void GetGuild(DiscordClient client, Snowflake guildId, Action<DiscordGuild> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			client.Bot.Rest.DoRequest<DiscordGuild>(string.Format("/guilds/{0}", guildId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x00012C80 File Offset: 0x00010E80
		public static void GetGuildPreview(DiscordClient client, Snowflake guildId, Action<GuildPreview> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			client.Bot.Rest.DoRequest<GuildPreview>(string.Format("/guilds/{0}/preview", guildId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x00012CCC File Offset: 0x00010ECC
		public void ModifyGuild(DiscordClient client, Action<DiscordGuild> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordGuild>(string.Format("/guilds/{0}", this.Id), RequestMethod.PATCH, this, callback, error);
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x00012CF9 File Offset: 0x00010EF9
		public void DeleteGuild(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}", this.Id), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x00012D26 File Offset: 0x00010F26
		public void GetGuildChannels(DiscordClient client, Action<List<DiscordChannel>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<DiscordChannel>>(string.Format("/guilds/{0}/channels", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x00012D53 File Offset: 0x00010F53
		public void CreateGuildChannel(DiscordClient client, ChannelCreate channel, Action<DiscordChannel> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordChannel>(string.Format("/guilds/{0}/channels", this.Id), RequestMethod.POST, channel, callback, error);
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x00012D81 File Offset: 0x00010F81
		public void ModifyGuildChannelPositions(DiscordClient client, List<GuildChannelPosition> positions, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/channels", this.Id), RequestMethod.PATCH, positions, callback, error);
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00012DAF File Offset: 0x00010FAF
		public void ListActiveThreads(DiscordClient client, Action<ThreadList> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<ThreadList>(string.Format("/guilds/{0}/threads/active", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x00012DDC File Offset: 0x00010FDC
		public void GetGuildMember(DiscordClient client, Snowflake userId, Action<GuildMember> callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			client.Bot.Rest.DoRequest<GuildMember>(string.Format("/guilds/{0}/members/{1}", this.Id, userId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x00012E34 File Offset: 0x00011034
		public void ListGuildMembers(DiscordClient client, int limit = 1000, string afterSnowflake = "0", Action<List<GuildMember>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<GuildMember>>(string.Format("/guilds/{0}/members?limit={1}&after={2}", this.Id, limit, afterSnowflake), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x00012E6A File Offset: 0x0001106A
		public void SearchGuildMembers(DiscordClient client, string search, int limit = 1, Action<List<GuildMember>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<GuildMember>>(string.Format("/guilds/{0}/members/search?query={1}&limit={2}", this.Id, search, limit), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x00012EA0 File Offset: 0x000110A0
		public void AddGuildMember(DiscordClient client, GuildMember member, string accessToken, List<DiscordRole> roles, Action<GuildMember> callback = null, Action<RestError> error = null)
		{
			GuildMemberAdd guildMemberAdd = new GuildMemberAdd
			{
				Deaf = member.Deaf,
				Mute = member.Mute,
				Nick = member.Nickname,
				AccessToken = accessToken,
				Roles = new List<Snowflake>()
			};
			foreach (DiscordRole discordRole in roles)
			{
				guildMemberAdd.Roles.Add(discordRole.Id);
			}
			this.AddGuildMember(client, member.User.Id, guildMemberAdd, callback, error);
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x00012F58 File Offset: 0x00011158
		public void AddGuildMember(DiscordClient client, Snowflake userId, GuildMemberAdd member, Action<GuildMember> callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			client.Bot.Rest.DoRequest<GuildMember>(string.Format("/guilds/{0}/members/{1}", this.Id, userId), RequestMethod.PUT, member, callback, error);
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x00012FB4 File Offset: 0x000111B4
		public void ModifyGuildMember(DiscordClient client, Snowflake userId, GuildMemberUpdate update, Action<GuildMember> callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			client.Bot.Rest.DoRequest<GuildMember>(string.Format("/guilds/{0}/members/{1}", this.Id, userId), RequestMethod.PATCH, update, callback, error);
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x00013010 File Offset: 0x00011210
		public void ModifyUsersNick(DiscordClient client, Snowflake userId, string nick, Action<GuildMember> callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			GuildMemberUpdate update = new GuildMemberUpdate
			{
				Nick = nick
			};
			this.ModifyGuildMember(client, userId, update, callback, error);
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x00013054 File Offset: 0x00011254
		public void ModifyCurrentMember(DiscordClient client, string nick, Action<GuildMember> callback, Action<RestError> error)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["nick"] = nick;
			Dictionary<string, object> data = dictionary;
			client.Bot.Rest.DoRequest<GuildMember>(string.Format("/guilds/{0}/members/@me", this.Id), RequestMethod.PATCH, data, callback, error);
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x000130A0 File Offset: 0x000112A0
		[Obsolete("Please use ModifyCurrentMember Instead. This will be removed in April 2022 Update")]
		public void ModifyCurrentUsersNick(DiscordClient client, string nick, Action<string> callback = null, Action<RestError> error = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["nick"] = nick;
			Dictionary<string, object> data = dictionary;
			client.Bot.Rest.DoRequest<string>(string.Format("/guilds/{0}/members/@me/nick", this.Id), RequestMethod.PATCH, data, callback, error);
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x000130EC File Offset: 0x000112EC
		public void AddGuildMemberRole(DiscordClient client, DiscordUser user, DiscordRole role, Action callback = null, Action<RestError> error = null)
		{
			this.AddGuildMemberRole(client, user.Id, role.Id, callback, error);
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00013108 File Offset: 0x00011308
		public void AddGuildMemberRole(DiscordClient client, Snowflake userId, Snowflake roleId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			bool flag2 = !roleId.IsValid();
			if (flag2)
			{
				throw new InvalidSnowflakeException("roleId");
			}
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/members/{1}/roles/{2}", this.Id, userId, roleId), RequestMethod.PUT, null, callback, error);
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x00013180 File Offset: 0x00011380
		public void RemoveGuildMemberRole(DiscordClient client, DiscordUser user, DiscordRole role, Action callback = null, Action<RestError> error = null)
		{
			this.RemoveGuildMemberRole(client, user.Id, role.Id, callback, error);
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x0001319C File Offset: 0x0001139C
		public void RemoveGuildMemberRole(DiscordClient client, Snowflake userId, Snowflake roleId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			bool flag2 = !roleId.IsValid();
			if (flag2)
			{
				throw new InvalidSnowflakeException("roleId");
			}
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/members/{1}/roles/{2}", this.Id, userId, roleId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x00013214 File Offset: 0x00011414
		public void RemoveGuildMember(DiscordClient client, GuildMember member, Action callback = null, Action<RestError> error = null)
		{
			this.RemoveGuildMember(client, member.User.Id, callback, error);
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x0001322C File Offset: 0x0001142C
		public void RemoveGuildMember(DiscordClient client, Snowflake userId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/members/{1}", this.Id, userId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x00013284 File Offset: 0x00011484
		public void GetGuildBans(DiscordClient client, Action<List<GuildBan>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<GuildBan>>(string.Format("/guilds/{0}/bans", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x000132B4 File Offset: 0x000114B4
		public void GetGuildBan(DiscordClient client, Snowflake userId, Action<GuildBan> callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			client.Bot.Rest.DoRequest<GuildBan>(string.Format("/guilds/{0}/bans/{1}", this.Id, userId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x0001330C File Offset: 0x0001150C
		public void CreateGuildBan(DiscordClient client, GuildMember member, GuildBanCreate ban, Action callback = null, Action<RestError> error = null)
		{
			this.CreateGuildBan(client, member.User.Id, ban, callback, error);
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x00013328 File Offset: 0x00011528
		public void CreateGuildBan(DiscordClient client, Snowflake userId, GuildBanCreate ban, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/bans/{1}", this.Id, userId), RequestMethod.PUT, ban, callback, error);
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00013384 File Offset: 0x00011584
		public void RemoveGuildBan(DiscordClient client, Snowflake userId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/bans/{1}", this.Id, userId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x000133DC File Offset: 0x000115DC
		public void GetGuildRoles(DiscordClient client, Action<List<DiscordRole>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<DiscordRole>>(string.Format("/guilds/{0}/roles", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x00013409 File Offset: 0x00011609
		public void CreateGuildRole(DiscordClient client, DiscordRole role, Action<DiscordRole> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordRole>(string.Format("/guilds/{0}/roles", this.Id), RequestMethod.POST, role, callback, error);
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x00013437 File Offset: 0x00011637
		public void ModifyGuildRolePositions(DiscordClient client, List<GuildRolePosition> positions, Action<List<DiscordRole>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<DiscordRole>>(string.Format("/guilds/{0}/roles", this.Id), RequestMethod.PATCH, positions, callback, error);
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x00013465 File Offset: 0x00011665
		public void ModifyGuildRole(DiscordClient client, DiscordRole role, Action<DiscordRole> callback = null, Action<RestError> error = null)
		{
			this.ModifyGuildRole(client, role.Id, role, callback, error);
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x0001347C File Offset: 0x0001167C
		public void ModifyGuildRole(DiscordClient client, Snowflake roleId, DiscordRole role, Action<DiscordRole> callback = null, Action<RestError> error = null)
		{
			bool flag = !roleId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("roleId");
			}
			client.Bot.Rest.DoRequest<DiscordRole>(string.Format("/guilds/{0}/roles/{1}", this.Id, roleId), RequestMethod.PATCH, role, callback, error);
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x000134D5 File Offset: 0x000116D5
		public void DeleteGuildRole(DiscordClient client, DiscordRole role, Action callback = null, Action<RestError> error = null)
		{
			this.DeleteGuildRole(client, role.Id, callback, error);
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x000134E8 File Offset: 0x000116E8
		public void DeleteGuildRole(DiscordClient client, Snowflake roleId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !roleId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("roleId");
			}
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/roles/{1}", this.Id, roleId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x00013540 File Offset: 0x00011740
		public void GetGuildPruneCount(DiscordClient client, GuildPruneGet prune, Action<int?> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<JObject>(string.Format("/guilds/{0}/prune?{1}", this.Id, prune.ToQueryString()), RequestMethod.GET, null, delegate(JObject returnValue)
			{
				int? obj = returnValue.GetValue("pruned").ToObject<int?>();
				Action<int?> callback2 = callback;
				if (callback2 != null)
				{
					callback2(obj);
				}
			}, error);
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x00013598 File Offset: 0x00011798
		public void BeginGuildPrune(DiscordClient client, GuildPruneBegin prune, Action<int?> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<JObject>(string.Format("/guilds/{0}/prune?{1}", this.Id, prune.ToQueryString()), RequestMethod.POST, null, delegate(JObject returnValue)
			{
				int? obj = returnValue.GetValue("pruned").ToObject<int?>();
				Action<int?> callback2 = callback;
				if (callback2 != null)
				{
					callback2(obj);
				}
			}, error);
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x000135EF File Offset: 0x000117EF
		public void GetGuildVoiceRegions(DiscordClient client, Action<List<VoiceRegion>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<VoiceRegion>>(string.Format("/guilds/{0}/regions", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x0001361C File Offset: 0x0001181C
		public void GetGuildInvites(DiscordClient client, Action<List<InviteMetadata>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<InviteMetadata>>(string.Format("/guilds/{0}/invites", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x00013649 File Offset: 0x00011849
		public void GetGuildIntegrations(DiscordClient client, Action<List<Integration>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<Integration>>(string.Format("/guilds/{0}/integrations", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x00013676 File Offset: 0x00011876
		public void DeleteGuildIntegration(DiscordClient client, Integration integration, Action callback = null, Action<RestError> error = null)
		{
			this.DeleteGuildIntegration(client, integration.Id, callback, error);
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x0001368C File Offset: 0x0001188C
		public void DeleteGuildIntegration(DiscordClient client, Snowflake integrationId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !integrationId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("integrationId");
			}
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/integrations/{1}", this.Id, integrationId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x000136E4 File Offset: 0x000118E4
		public void GetGuildWidgetSettings(DiscordClient client, Action<GuildWidgetSettings> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<GuildWidgetSettings>(string.Format("/guilds/{0}/widget", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x00013711 File Offset: 0x00011911
		public void ModifyGuildWidget(DiscordClient client, GuildWidget widget, Action<GuildWidget> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<GuildWidget>(string.Format("/guilds/{0}/widget", this.Id), RequestMethod.PATCH, widget, callback, error);
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x0001373F File Offset: 0x0001193F
		public void GetGuildWidget(DiscordClient client, Action<GuildWidget> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<GuildWidget>(string.Format("/guilds/{0}/widget.json", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x0001376C File Offset: 0x0001196C
		public void GetGuildWelcomeScreen(DiscordClient client, Action<GuildWelcomeScreen> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<GuildWelcomeScreen>(string.Format("/guilds/{0}/welcome-screen", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x00013799 File Offset: 0x00011999
		public void ModifyWelcomeScreen(DiscordClient client, WelcomeScreenUpdate update, Action<GuildWelcomeScreen> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<GuildWelcomeScreen>(string.Format("/guilds/{0}/welcome-screen", this.Id), RequestMethod.PATCH, update, callback, error);
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x000137C7 File Offset: 0x000119C7
		public void GetGuildVanityUrl(DiscordClient client, Action<InviteMetadata> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<InviteMetadata>(string.Format("/guilds/{0}/vanity-url", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x000137F4 File Offset: 0x000119F4
		public void ListGuildEmojis(DiscordClient client, Action<List<DiscordEmoji>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<DiscordEmoji>>(string.Format("/guilds/{0}/emojis", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x00013821 File Offset: 0x00011A21
		public void GetGuildEmoji(DiscordClient client, string emjoiId, Action<DiscordEmoji> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordEmoji>(string.Format("/guilds/{0}/emojis/{1}", this.Id, emjoiId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x00013850 File Offset: 0x00011A50
		public void CreateGuildEmoji(DiscordClient client, EmojiCreate emoji, Action<DiscordEmoji> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordEmoji>(string.Format("/guilds/{0}/emojis", this.Id), RequestMethod.POST, emoji, callback, error);
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x0001387E File Offset: 0x00011A7E
		public void UpdateGuildEmoji(DiscordClient client, string emojiId, EmojiUpdate emoji, Action<DiscordEmoji> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordEmoji>(string.Format("/guilds/{0}/emojis/{1}", this.Id, emojiId), RequestMethod.PATCH, emoji, callback, error);
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x000138AE File Offset: 0x00011AAE
		public void DeleteGuildEmoji(DiscordClient client, string emojiId, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/emojis/{1}", this.Id, emojiId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x000138E0 File Offset: 0x00011AE0
		public void ModifyCurrentUserVoiceState(DiscordClient client, Snowflake channelId, bool? suppress = null, DateTime? requestToSpeak = null, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["channel_id"] = channelId.ToString();
			Dictionary<string, object> dictionary2 = dictionary;
			bool flag2 = suppress != null;
			if (flag2)
			{
				dictionary2["suppress"] = suppress.Value;
			}
			bool flag3 = requestToSpeak != null;
			if (flag3)
			{
				dictionary2["request_to_speak_timestamp"] = requestToSpeak;
			}
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/voice-states/@me", this.Id), RequestMethod.PATCH, dictionary2, callback, error);
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x00013998 File Offset: 0x00011B98
		public void ModifyUserVoiceState(DiscordClient client, Snowflake userId, Snowflake channelId, bool? suppress = null, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			bool flag2 = !channelId.IsValid();
			if (flag2)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["channel_id"] = channelId.ToString();
			Dictionary<string, object> dictionary2 = dictionary;
			bool flag3 = suppress != null;
			if (flag3)
			{
				dictionary2["suppress"] = suppress.Value;
			}
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/voice-states/{1}", this.Id, userId), RequestMethod.PATCH, dictionary2, callback, error);
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x00013A4E File Offset: 0x00011C4E
		public void ListGuildStickers(DiscordClient client, Action<List<DiscordSticker>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<DiscordSticker>>(string.Format("/guilds/{0}/stickers", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x00013A7C File Offset: 0x00011C7C
		public void GetGuildSticker(DiscordClient client, Snowflake stickerId, Action<DiscordSticker> callback = null, Action<RestError> error = null)
		{
			bool flag = !stickerId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("stickerId");
			}
			client.Bot.Rest.DoRequest<DiscordSticker>(string.Format("/guilds/{0}/stickers/{1}", this.Id, stickerId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x00013AD4 File Offset: 0x00011CD4
		public void CreateGuildSticker(DiscordClient client, GuildStickerCreate sticker, Action<DiscordSticker> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordSticker>(string.Format("/guilds/{0}/stickers", this.Id), RequestMethod.POST, sticker, callback, error);
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x00013B02 File Offset: 0x00011D02
		public void ModifyGuildSticker(DiscordClient client, DiscordSticker sticker, Action<DiscordSticker> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordSticker>(string.Format("/guilds/{0}/stickers/{1}", this.Id, sticker.Id), RequestMethod.PATCH, sticker, callback, error);
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x00013B3C File Offset: 0x00011D3C
		public void DeleteGuildSticker(DiscordClient client, Snowflake stickerId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !stickerId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("stickerId");
			}
			client.Bot.Rest.DoRequest(string.Format("/guilds/{0}/stickers/{1}", this.Id, stickerId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x00013B94 File Offset: 0x00011D94
		internal DiscordGuild Update(DiscordGuild updatedGuild)
		{
			DiscordGuild result = (DiscordGuild)base.MemberwiseClone();
			bool flag = updatedGuild.Name != null;
			if (flag)
			{
				this.Name = updatedGuild.Name;
			}
			bool flag2 = updatedGuild.Icon != null;
			if (flag2)
			{
				this.Icon = updatedGuild.Icon;
			}
			bool flag3 = updatedGuild.IconHash != null;
			if (flag3)
			{
				this.IconHash = updatedGuild.IconHash;
			}
			bool flag4 = updatedGuild.Splash != null;
			if (flag4)
			{
				this.Splash = updatedGuild.Splash;
			}
			bool flag5 = updatedGuild.DiscoverySplash != null;
			if (flag5)
			{
				this.DiscoverySplash = updatedGuild.DiscoverySplash;
			}
			bool flag6 = updatedGuild.OwnerId.IsValid();
			if (flag6)
			{
				this.OwnerId = updatedGuild.OwnerId;
			}
			bool flag7 = updatedGuild.AfkChannelId != null;
			if (flag7)
			{
				this.AfkChannelId = updatedGuild.AfkChannelId;
			}
			bool flag8 = updatedGuild.AfkTimeout != null;
			if (flag8)
			{
				this.AfkTimeout = updatedGuild.AfkTimeout;
			}
			bool flag9 = updatedGuild.WidgetEnabled != null;
			if (flag9)
			{
				this.WidgetEnabled = updatedGuild.WidgetEnabled;
			}
			bool flag10 = updatedGuild.WidgetChannelId != null;
			if (flag10)
			{
				this.WidgetChannelId = updatedGuild.WidgetChannelId;
			}
			this.VerificationLevel = updatedGuild.VerificationLevel;
			this.DefaultMessageNotifications = updatedGuild.DefaultMessageNotifications;
			this.ExplicitContentFilter = updatedGuild.ExplicitContentFilter;
			bool flag11 = updatedGuild.Roles != null;
			if (flag11)
			{
				this.Roles = updatedGuild.Roles;
			}
			bool flag12 = updatedGuild.Emojis != null;
			if (flag12)
			{
				this.Emojis = updatedGuild.Emojis;
			}
			bool flag13 = updatedGuild.Features != null;
			if (flag13)
			{
				this.Features = updatedGuild.Features;
			}
			bool flag14 = updatedGuild.MfaLevel != null;
			if (flag14)
			{
				this.MfaLevel = updatedGuild.MfaLevel;
			}
			bool flag15 = updatedGuild.ApplicationId != null;
			if (flag15)
			{
				this.ApplicationId = updatedGuild.ApplicationId;
			}
			bool flag16 = updatedGuild.SystemChannelId != null;
			if (flag16)
			{
				this.SystemChannelId = updatedGuild.SystemChannelId;
			}
			this.SystemChannelFlags = updatedGuild.SystemChannelFlags;
			bool flag17 = this.RulesChannelId != null;
			if (flag17)
			{
				this.RulesChannelId = updatedGuild.RulesChannelId;
			}
			bool flag18 = updatedGuild.JoinedAt != null;
			if (flag18)
			{
				this.JoinedAt = updatedGuild.JoinedAt;
			}
			bool flag19 = updatedGuild.Large != null;
			if (flag19)
			{
				this.Large = updatedGuild.Large;
			}
			bool flag20 = updatedGuild.Unavailable != null && (this.Unavailable == null || this.Unavailable.Value);
			if (flag20)
			{
				this.Unavailable = updatedGuild.Unavailable;
			}
			bool flag21 = updatedGuild.MemberCount != null;
			if (flag21)
			{
				this.MemberCount = updatedGuild.MemberCount;
			}
			bool flag22 = updatedGuild.VoiceStates != null;
			if (flag22)
			{
				this.VoiceStates = updatedGuild.VoiceStates;
			}
			bool flag23 = updatedGuild.Members != null;
			if (flag23)
			{
				this.Members = updatedGuild.Members;
			}
			bool flag24 = updatedGuild.Channels != null;
			if (flag24)
			{
				this.Channels = updatedGuild.Channels;
			}
			bool flag25 = updatedGuild.Threads != null;
			if (flag25)
			{
				this.Threads = updatedGuild.Threads;
			}
			bool flag26 = updatedGuild.Presences != null;
			if (flag26)
			{
				this.Presences = updatedGuild.Presences;
			}
			bool flag27 = updatedGuild.MaxPresences != null;
			if (flag27)
			{
				this.MaxPresences = updatedGuild.MaxPresences;
			}
			bool flag28 = updatedGuild.MaxMembers != null;
			if (flag28)
			{
				this.MaxMembers = updatedGuild.MaxMembers;
			}
			bool flag29 = updatedGuild.VanityUrlCode != null;
			if (flag29)
			{
				this.VanityUrlCode = updatedGuild.VanityUrlCode;
			}
			bool flag30 = updatedGuild.Description != null;
			if (flag30)
			{
				this.Description = updatedGuild.Description;
			}
			bool flag31 = updatedGuild.Banner != null;
			if (flag31)
			{
				this.Banner = updatedGuild.Banner;
			}
			bool flag32 = updatedGuild.PremiumTier != null;
			if (flag32)
			{
				this.PremiumTier = updatedGuild.PremiumTier;
			}
			bool flag33 = updatedGuild.PremiumSubscriptionCount != null;
			if (flag33)
			{
				this.PremiumSubscriptionCount = updatedGuild.PremiumSubscriptionCount;
			}
			bool flag34 = updatedGuild.PreferredLocale != null;
			if (flag34)
			{
				this.PreferredLocale = updatedGuild.PreferredLocale;
			}
			bool flag35 = updatedGuild.PublicUpdatesChannelId != null;
			if (flag35)
			{
				this.PublicUpdatesChannelId = updatedGuild.PublicUpdatesChannelId;
			}
			bool flag36 = updatedGuild.MaxVideoChannelUsers != null;
			if (flag36)
			{
				this.MaxVideoChannelUsers = updatedGuild.MaxVideoChannelUsers;
			}
			bool flag37 = updatedGuild.ApproximateMemberCount != null;
			if (flag37)
			{
				this.ApproximateMemberCount = updatedGuild.ApproximateMemberCount;
			}
			bool flag38 = updatedGuild.ApproximatePresenceCount != null;
			if (flag38)
			{
				this.ApproximatePresenceCount = updatedGuild.ApproximatePresenceCount;
			}
			bool flag39 = updatedGuild.WelcomeScreen != null;
			if (flag39)
			{
				this.WelcomeScreen = updatedGuild.WelcomeScreen;
			}
			this.NsfwLevel = updatedGuild.NsfwLevel;
			bool flag40 = updatedGuild.StageInstances != null;
			if (flag40)
			{
				this.StageInstances = updatedGuild.StageInstances;
			}
			bool flag41 = updatedGuild.Stickers != null;
			if (flag41)
			{
				this.Stickers = updatedGuild.Stickers;
			}
			bool flag42 = updatedGuild.ScheduledEvents != null;
			if (flag42)
			{
				this.ScheduledEvents = updatedGuild.ScheduledEvents;
			}
			this.PremiumProgressBarEnabled = updatedGuild.PremiumProgressBarEnabled;
			return result;
		}
	}
}
