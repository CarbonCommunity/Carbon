using System;
using Oxide.Ext.Discord.Entities;

namespace Oxide.Ext.Discord.Helpers.Cdn
{
    /// <summary>
    /// Represents Discord <a href="https://discord.com/developers/docs/reference#image-formatting-cdn-endpoints">CDN Endpoints</a>
    /// </summary>
    public static class DiscordCdn
    {
        /// <summary>
        /// Base CDN Url
        /// </summary>
        public const string CdnUrl = "https://cdn.discordapp.com";

        /// <summary>
        /// Returns the Url to the custom emoji
        /// </summary>
        /// <param name="emojiId">ID of the emoji</param>
        /// <param name="format">The format the emoji is in</param>
        /// <returns>Url of the emoji</returns>
        /// <exception cref="ArgumentException">Thrown if format is Jpg or WebP</exception>
        public static string GetCustomEmojiUrl(Snowflake emojiId, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                case ImageFormat.Gif:
                    return $"{CdnUrl}/emojis/{emojiId.ToString()}.{GetExtension(format, emojiId.ToString())}";

                default:
                    throw new ArgumentException("ImageFormat is not valid for Custom Emoji. Valid types are (Auto, Png, Jpeg, WebP, Gif)", nameof(format));
            }
        }
        
        /// <summary>
        /// Returns the Url to the Guild Icon
        /// </summary>
        /// <param name="guildId">Guild ID for the icon</param>
        /// <param name="guildIcon">Guild Icon from guild</param>
        /// <param name="format">Format the icon is in</param>
        /// <returns>Url of the guild icon</returns>
        public static string GetGuildIconUrl(Snowflake guildId, string guildIcon, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                case ImageFormat.Gif:
                    return $"{CdnUrl}/icons/{guildId.ToString()}/{guildIcon}.{GetExtension(format, guildIcon)}";

                default:
                    throw new ArgumentException("ImageFormat is not valid for Guild Icon. Valid types are (Auto, Png, Jpeg, WebP, Gif)", nameof(format));
            }
        }
        
        /// <summary>
        /// Returns the Url of the Guild Splash
        /// </summary>
        /// <param name="guildId">Guild ID for the icon</param>
        /// <param name="guildSplash">Guild Splash from guild</param>
        /// <param name="format">Format the icon is in</param>
        /// <returns>Url of the guild splash</returns>
        /// <exception cref="ArgumentException">Thrown if format is Gif</exception>
        public static string GetGuildSplashUrl(Snowflake guildId, string guildSplash, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                    return $"{CdnUrl}/splashes/{guildId.ToString()}/{guildSplash}.{GetExtension(format, guildSplash)}";
                
                default:
                    throw new ArgumentException("ImageFormat is not valid for Guild Splash. Valid types are (Auto, Png, Jpeg, WebP)", nameof(format));
            }
        }
        
        /// <summary>
        /// Return the Url of the Guild Discovery Splash
        /// </summary>
        /// <param name="guildId">Guild ID for the icon</param>
        /// <param name="guildDiscoverySplash">Guild Discovery Splash from guild</param>
        /// <param name="format">Format the icon is in</param>
        /// <returns>Url of the guild discovery splash</returns>
        /// <exception cref="ArgumentException">Thrown if format is Gif</exception>
        public static string GetGuildDiscoverySplashUrl(Snowflake guildId, string guildDiscoverySplash, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                    return $"{CdnUrl}/discovery-splashes/{guildId.ToString()}/{guildDiscoverySplash}.{GetExtension(format, guildDiscoverySplash)}";
                
                default:
                    throw new ArgumentException("ImageFormat is not valid for Guild Discovery Splash. Valid types are (Auto, Png, Jpeg, WebP)", nameof(format));
            }
        }
        
        /// <summary>
        /// Returns the Url of the Guild Banner
        /// </summary>
        /// <param name="guildId">Guild ID for the icon</param>
        /// <param name="guildBanner">Guild Banner from guild</param>
        /// <param name="format">Format the icon is in</param>
        /// <returns>Url of the guild banner</returns>
        /// <exception cref="ArgumentException">Thrown if format is Gif</exception>
        public static string GetGuildBannerUrl(Snowflake guildId, string guildBanner, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                    return $"{CdnUrl}/banners/{guildId.ToString()}/{guildBanner}.{GetExtension(format, guildBanner)}";
                
                default:
                    throw new ArgumentException("ImageFormat is not valid for Guild Banner. Valid types are (Auto, Png, Jpeg, WebP)", nameof(format));
            }
        }
        
        /// <summary>
        /// Returns the Url of the User Banner
        /// </summary>
        /// <param name="userId">User ID for the Banner</param>
        /// <param name="userBanner">User Banner from user</param>
        /// <param name="format">Format the icon is in</param>
        /// <returns>Url of the User banner</returns>
        /// <exception cref="ArgumentException">Thrown if format is Gif</exception>
        public static string GetUserBanner(Snowflake userId, string userBanner, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                    return $"{CdnUrl}/banners/{userId.ToString()}/{userBanner}.{GetExtension(format, userBanner)}";
                
                default:
                    throw new ArgumentException("ImageFormat is not valid for Guild Banner. Valid types are (Auto, Png, Jpeg, WebP)", nameof(format));
            }
        }

        /// <summary>
        /// Returns the icon for a given channel
        /// </summary>
        /// <param name="channelId">Channel ID for the Icon</param>
        /// <param name="icon">Icon hash for the channel</param>
        /// <returns></returns>
        public static string GetChannelIcon(Snowflake channelId, string icon)
        {
            return $"https://cdn.discordapp.com/channel-icons/{channelId.ToString()}/{icon}.png";
        }
        
        /// <summary>
        /// Returns the Url of the users default avatar
        /// </summary>
        /// <param name="userId">Discord User ID</param>
        /// <param name="userDiscriminator">Discord User Discriminator</param>
        /// <returns>Url of the default avatar url</returns>
        public static string GetUserDefaultAvatarUrl(Snowflake userId, string userDiscriminator)
        {
            uint discriminator = uint.Parse(userDiscriminator) % 5;
            return $"{CdnUrl}/embed/avatars/{userId.ToString()}/{discriminator.ToString()}.png";
        }
        
        /// <summary>
        /// Returns the Url of the users avatar
        /// </summary>
        /// <param name="userId">Discord User ID</param>
        /// <param name="userAvatar">User avatar from user</param>
        /// <param name="format">Format the avatar is in</param>
        /// <returns>Url of the users avatar</returns>
        public static string GetUserAvatarUrl(Snowflake userId, string userAvatar, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                case ImageFormat.Gif:
                    return $"{CdnUrl}/avatars/{userId.ToString()}/{userAvatar}.{GetExtension(format, userAvatar)}";

                default:
                    throw new ArgumentException("ImageFormat is not valid for User Avatar. Valid types are (Auto, Png, Jpeg, WebP, Gif)", nameof(format));
            }
        }

        /// <summary>
        /// Returns the Url of the Guild Member avatar
        /// </summary>
        /// <param name="guildId">Guild ID of the Guild Member</param>
        /// <param name="userId">Discord User ID</param>
        /// <param name="memberAvatar">Guild Member avatar</param>
        /// <param name="format">Format the avatar is in</param>
        /// <returns>Url of the Guild Member avatar</returns>
        public static string GetGuildMemberAvatar(Snowflake guildId, Snowflake userId, string memberAvatar, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                case ImageFormat.Gif:
                    return $"{CdnUrl}/guilds/{guildId.ToString()}/users/{userId.ToString()}/avatars/{memberAvatar}.{GetExtension(format, memberAvatar)}";

                default:
                    throw new ArgumentException("ImageFormat is not valid for Guild Member Avatar. Valid types are (Auto, Png, Jpeg, WebP, Gif)", nameof(format));
            }
        }
        
        /// <summary>
        /// Returns the url to the application icon
        /// </summary>
        /// <param name="applicationId">Application ID</param>
        /// <param name="icon">Icon field from application</param>
        /// <param name="format">Format the icon is in</param>
        /// <returns>Url of the application icon</returns>
        /// <exception cref="ArgumentException">Throw if format is Gif</exception>
        public static string GetApplicationIconUrl(Snowflake applicationId, string icon, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                    return $"{CdnUrl}/app-icons/{applicationId.ToString()}/{icon}.{GetExtension(format, icon)}";

                default:
                    throw new ArgumentException("ImageFormat is not valid for Application Icon. Valid types are (Auto, Png, Jpeg, WebP)", nameof(format));
            }
        }
        
        /// <summary>
        /// Returns the applications asset icon url
        /// </summary>
        /// <param name="applicationId">Application ID of the icon</param>
        /// <param name="assetId">Asset ID for the application</param>
        /// <param name="format">Format the icon is in</param>
        /// <returns>Url of the application asset icon</returns>
        /// <exception cref="ArgumentException">Throw if format is Gif</exception>
        public static string GetApplicationAssetUrl(Snowflake applicationId, string assetId, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                    return $"{CdnUrl}/app-assets/{applicationId.ToString()}/{assetId}.{GetExtension(format, assetId.ToString())}";

                default:
                    throw new ArgumentException("ImageFormat is not valid for Application Asset. Valid types are (Auto, Png, Jpeg, WebP)", nameof(format));
            }
        }
        
        /// <summary>
        /// Returns the Url of the Achievement Icon
        /// </summary>
        /// <param name="applicationId">Application ID of the icon</param>
        /// <param name="achievementId">Achievement ID</param>
        /// <param name="iconHash">Achievement Icon Hash</param>
        /// <param name="format">Format the icon is in</param>
        /// <returns>Url of the achievement icon</returns>
        /// <exception cref="ArgumentException">Throw if format is Gif</exception>
        public static string GetAchievementIconUrl(Snowflake applicationId, Snowflake achievementId, string iconHash, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                    return $"{CdnUrl}/app-assets/{applicationId.ToString()}/achievements/{achievementId.ToString()}/icons/{iconHash}.{GetExtension(format, iconHash)}";

                default:
                    throw new ArgumentException("ImageFormat is not valid for Achievement Icon. Valid types are (Auto, Png, Jpeg, WebP)", nameof(format));
            }
        }
        
        /// <summary>
        /// Returns the Url of the Team Icon
        /// </summary>
        /// <param name="teamId">Team ID of the Icon</param>
        /// <param name="teamIcon">Icon field from Team</param>
        /// <param name="format">Format the icon is in</param>
        /// <returns>Url of the achievement icon</returns>
        /// <exception cref="ArgumentException">Throw if format is Gif</exception>
        public static string GetTeamIconUrl(Snowflake teamId, string teamIcon, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                    return $"{CdnUrl}/team-icons/{teamId.ToString()}/{teamIcon}.{GetExtension(format, teamIcon)}";

                default:
                    throw new ArgumentException("ImageFormat is not valid for Team Icon. Valid types are (Auto, Png, Jpeg, WebP)", nameof(format));
            }
        }
        

        /// <summary>
        /// Returns the banner for a given sticker pack
        /// </summary>
        /// <param name="applicationId">Application ID for the stickers</param>
        /// <param name="bannerAssetId">Banner Asset ID for the stickers</param>
        /// <param name="format">Image Formatting for the banner</param>
        /// <returns>Url to the sticker pack banner</returns>
        /// <exception cref="ArgumentException">Thrown if image type is not PNG,JPEG, or WebP</exception>
        public static string GetStickerPackBanner(Snowflake applicationId, Snowflake bannerAssetId, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                    return $"{CdnUrl}/app-assets/{applicationId.ToString()}/store/{bannerAssetId.ToString()}.{GetExtension(format, bannerAssetId)}";

                default:
                    throw new ArgumentException("ImageFormat is not valid for Sticker Pack Banner. Valid types are (Auto, Png, Jpeg, WebP)", nameof(format));
            }
        }
        
        /// <summary>
        /// Returns the sticker url with the given ID
        /// </summary>
        /// <param name="stickerId">ID of the sticker</param>
        /// <param name="format">Format for the sticker to be returned in</param>
        /// <returns>Return url for the sticker</returns>
        /// <exception cref="ArgumentException">Thrown if image type is not PNG or Lottie</exception>
        public static string GetSticker(Snowflake stickerId, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Png:
                case ImageFormat.Lottie:
                    return $"{CdnUrl}/stickers/{stickerId.ToString()}.{GetExtension(format, stickerId)}";

                default:
                    throw new ArgumentException("ImageFormat is not valid for Sticker Pack Banner. Valid types are (Auto, Png, Jpeg, WebP)", nameof(format));
            }
        }
        
        /// <summary>
        /// Returns the sticker url with the given ID
        /// </summary>
        /// <param name="roleId">ID of the role</param>
        /// <param name="format">Format for the icon to be returned in</param>
        /// <returns>Return url for the role icon</returns>
        /// <exception cref="ArgumentException">Thrown if image type is not PNG or Lottie</exception>
        public static string GetRoleIcon(Snowflake roleId, ImageFormat format = ImageFormat.Auto)
        {
            switch (format)
            {
                case ImageFormat.Auto:
                case ImageFormat.Jpg:
                case ImageFormat.Png:
                case ImageFormat.WebP:
                    return $"{CdnUrl}/role-icons/{roleId.ToString()}.{GetExtension(format, roleId)}";

                default:
                    throw new ArgumentException("ImageFormat is not valid for Sticker Pack Banner. Valid types are (Auto, Png, Jpeg, WebP)", nameof(format));
            }
        }

        /// <summary>
        /// Returns the extension to use for the image
        /// </summary>
        /// <param name="format">Image format that is requested</param>
        /// <param name="image">Image data from the field</param>
        /// <returns>Image extension for the image format and image data</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Image Format is out of range</exception>
        public static string GetExtension(ImageFormat format, string image)
        {
            if (format == ImageFormat.Auto)
            {
                format = image.StartsWith("a_") ? ImageFormat.Gif : ImageFormat.Png;
            }

            switch (format)
            {
                case ImageFormat.Jpg:
                    return "jpeg";
                case ImageFormat.Png:
                    return "png";
                case ImageFormat.WebP:
                    return "webp";
                case ImageFormat.Gif:
                    return "gif";
                case ImageFormat.Lottie:
                    return "json";
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format.ToString(), "Format is not a valid ImageFormat");
            }
        }
    }
}