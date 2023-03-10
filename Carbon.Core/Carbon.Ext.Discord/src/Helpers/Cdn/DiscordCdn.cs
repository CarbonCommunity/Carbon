/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Ext.Discord.Entities;

namespace Oxide.Ext.Discord.Helpers.Cdn
{
	// Token: 0x0200002F RID: 47
	public static class DiscordCdn
	{
		// Token: 0x0600018E RID: 398 RVA: 0x0000CE5C File Offset: 0x0000B05C
		public static string GetCustomEmojiUrl(Snowflake emojiId, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.Gif)
			{
				throw new ArgumentException("ImageFormat is not valid for Custom Emoji. Valid types are (Auto, Png, Jpeg, WebP, Gif)", "format");
			}
			return "https://cdn.discordapp.com/emojis/" + emojiId.ToString() + "." + DiscordCdn.GetExtension(format, emojiId.ToString());
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000CEB8 File Offset: 0x0000B0B8
		public static string GetGuildIconUrl(Snowflake guildId, string guildIcon, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.Gif)
			{
				throw new ArgumentException("ImageFormat is not valid for Guild Icon. Valid types are (Auto, Png, Jpeg, WebP, Gif)", "format");
			}
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/icons/",
				guildId.ToString(),
				"/",
				guildIcon,
				".",
				DiscordCdn.GetExtension(format, guildIcon)
			});
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000CF28 File Offset: 0x0000B128
		public static string GetGuildSplashUrl(Snowflake guildId, string guildSplash, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.WebP)
			{
				throw new ArgumentException("ImageFormat is not valid for Guild Splash. Valid types are (Auto, Png, Jpeg, WebP)", "format");
			}
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/splashes/",
				guildId.ToString(),
				"/",
				guildSplash,
				".",
				DiscordCdn.GetExtension(format, guildSplash)
			});
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000CF98 File Offset: 0x0000B198
		public static string GetGuildDiscoverySplashUrl(Snowflake guildId, string guildDiscoverySplash, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.WebP)
			{
				throw new ArgumentException("ImageFormat is not valid for Guild Discovery Splash. Valid types are (Auto, Png, Jpeg, WebP)", "format");
			}
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/discovery-splashes/",
				guildId.ToString(),
				"/",
				guildDiscoverySplash,
				".",
				DiscordCdn.GetExtension(format, guildDiscoverySplash)
			});
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000D008 File Offset: 0x0000B208
		public static string GetGuildBannerUrl(Snowflake guildId, string guildBanner, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.WebP)
			{
				throw new ArgumentException("ImageFormat is not valid for Guild Banner. Valid types are (Auto, Png, Jpeg, WebP)", "format");
			}
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/banners/",
				guildId.ToString(),
				"/",
				guildBanner,
				".",
				DiscordCdn.GetExtension(format, guildBanner)
			});
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000D078 File Offset: 0x0000B278
		public static string GetUserBanner(Snowflake userId, string userBanner, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.WebP)
			{
				throw new ArgumentException("ImageFormat is not valid for Guild Banner. Valid types are (Auto, Png, Jpeg, WebP)", "format");
			}
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/banners/",
				userId.ToString(),
				"/",
				userBanner,
				".",
				DiscordCdn.GetExtension(format, userBanner)
			});
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000D0E8 File Offset: 0x0000B2E8
		public static string GetChannelIcon(Snowflake channelId, string icon)
		{
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/channel-icons/",
				channelId.ToString(),
				"/",
				icon,
				".png"
			});
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000D134 File Offset: 0x0000B334
		public static string GetUserDefaultAvatarUrl(Snowflake userId, string userDiscriminator)
		{
			uint num = uint.Parse(userDiscriminator) % 5U;
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/embed/avatars/",
				userId.ToString(),
				"/",
				num.ToString(),
				".png"
			});
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000D18C File Offset: 0x0000B38C
		public static string GetUserAvatarUrl(Snowflake userId, string userAvatar, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.Gif)
			{
				throw new ArgumentException("ImageFormat is not valid for User Avatar. Valid types are (Auto, Png, Jpeg, WebP, Gif)", "format");
			}
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/avatars/",
				userId.ToString(),
				"/",
				userAvatar,
				".",
				DiscordCdn.GetExtension(format, userAvatar)
			});
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000D1FC File Offset: 0x0000B3FC
		public static string GetGuildMemberAvatar(Snowflake guildId, Snowflake userId, string memberAvatar, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.Gif)
			{
				throw new ArgumentException("ImageFormat is not valid for Guild Member Avatar. Valid types are (Auto, Png, Jpeg, WebP, Gif)", "format");
			}
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/guilds/",
				guildId.ToString(),
				"/users/",
				userId.ToString(),
				"/avatars/",
				memberAvatar,
				".",
				DiscordCdn.GetExtension(format, memberAvatar)
			});
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000D284 File Offset: 0x0000B484
		public static string GetApplicationIconUrl(Snowflake applicationId, string icon, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.WebP)
			{
				throw new ArgumentException("ImageFormat is not valid for Application Icon. Valid types are (Auto, Png, Jpeg, WebP)", "format");
			}
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/app-icons/",
				applicationId.ToString(),
				"/",
				icon,
				".",
				DiscordCdn.GetExtension(format, icon)
			});
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000D2F4 File Offset: 0x0000B4F4
		public static string GetApplicationAssetUrl(Snowflake applicationId, string assetId, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.WebP)
			{
				throw new ArgumentException("ImageFormat is not valid for Application Asset. Valid types are (Auto, Png, Jpeg, WebP)", "format");
			}
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/app-assets/",
				applicationId.ToString(),
				"/",
				assetId,
				".",
				DiscordCdn.GetExtension(format, assetId.ToString())
			});
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000D368 File Offset: 0x0000B568
		public static string GetAchievementIconUrl(Snowflake applicationId, Snowflake achievementId, string iconHash, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.WebP)
			{
				throw new ArgumentException("ImageFormat is not valid for Achievement Icon. Valid types are (Auto, Png, Jpeg, WebP)", "format");
			}
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/app-assets/",
				applicationId.ToString(),
				"/achievements/",
				achievementId.ToString(),
				"/icons/",
				iconHash,
				".",
				DiscordCdn.GetExtension(format, iconHash)
			});
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000D3F0 File Offset: 0x0000B5F0
		public static string GetTeamIconUrl(Snowflake teamId, string teamIcon, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.WebP)
			{
				throw new ArgumentException("ImageFormat is not valid for Team Icon. Valid types are (Auto, Png, Jpeg, WebP)", "format");
			}
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/team-icons/",
				teamId.ToString(),
				"/",
				teamIcon,
				".",
				DiscordCdn.GetExtension(format, teamIcon)
			});
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000D460 File Offset: 0x0000B660
		public static string GetStickerPackBanner(Snowflake applicationId, Snowflake bannerAssetId, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.WebP)
			{
				throw new ArgumentException("ImageFormat is not valid for Sticker Pack Banner. Valid types are (Auto, Png, Jpeg, WebP)", "format");
			}
			return string.Concat(new string[]
			{
				"https://cdn.discordapp.com/app-assets/",
				applicationId.ToString(),
				"/store/",
				bannerAssetId.ToString(),
				".",
				DiscordCdn.GetExtension(format, bannerAssetId)
			});
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000D4E0 File Offset: 0x0000B6E0
		public static string GetSticker(Snowflake stickerId, ImageFormat format = ImageFormat.Auto)
		{
			if (format != ImageFormat.Auto && format != ImageFormat.Png && format != ImageFormat.Lottie)
			{
				throw new ArgumentException("ImageFormat is not valid for Sticker Pack Banner. Valid types are (Auto, Png, Jpeg, WebP)", "format");
			}
			return "https://cdn.discordapp.com/stickers/" + stickerId.ToString() + "." + DiscordCdn.GetExtension(format, stickerId);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000D540 File Offset: 0x0000B740
		public static string GetRoleIcon(Snowflake roleId, ImageFormat format = ImageFormat.Auto)
		{
			if (format > ImageFormat.WebP)
			{
				throw new ArgumentException("ImageFormat is not valid for Sticker Pack Banner. Valid types are (Auto, Png, Jpeg, WebP)", "format");
			}
			return "https://cdn.discordapp.com/role-icons/" + roleId.ToString() + "." + DiscordCdn.GetExtension(format, roleId);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000D594 File Offset: 0x0000B794
		public static string GetExtension(ImageFormat format, string image)
		{
			bool flag = format == ImageFormat.Auto;
			if (flag)
			{
				format = (image.StartsWith("a_") ? ImageFormat.Gif : ImageFormat.Png);
			}
			string result;
			switch (format)
			{
			case ImageFormat.Jpg:
				result = "jpeg";
				break;
			case ImageFormat.Png:
				result = "png";
				break;
			case ImageFormat.WebP:
				result = "webp";
				break;
			case ImageFormat.Gif:
				result = "gif";
				break;
			case ImageFormat.Lottie:
				result = "json";
				break;
			default:
				throw new ArgumentOutOfRangeException("format", format.ToString(), "Format is not a valid ImageFormat");
			}
			return result;
		}

		// Token: 0x040000FF RID: 255
		public const string CdnUrl = "https://cdn.discordapp.com";
	}
}
