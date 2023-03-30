using System;
using Oxide.Ext.Discord.Entities;

namespace Oxide.Ext.Discord.Helpers
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/reference#message-formatting-formats">Message text formatting options</a>
    /// </summary>
    public class DiscordFormatting
    {
        /// <summary>
        /// Mention the user with the given user ID
        /// </summary>
        /// <param name="userId">User ID to mention</param>
        /// <returns>Mention user formatted string</returns>
        public static string MentionUser(Snowflake userId) => $"<@{userId.ToString()}>";
        
        /// <summary>
        /// Mention the user displaying their user name
        /// </summary>
        /// <param name="userId">User ID to mention</param>
        /// <returns>Ping user formatted string</returns>
        public static string MentionUserNickname(Snowflake userId) => $"<@!{userId.ToString()}>";
        
        /// <summary>
        /// Mention the the channel with the given ID
        /// </summary>
        /// <param name="channelId">Channel ID to mention</param>
        /// <returns>Mention channel formatted string</returns>
        public static string MentionChannel(Snowflake channelId) => $"<#{channelId.ToString()}>";

        /// <summary>
        /// Mention the the role with the given ID
        /// </summary>
        /// <param name="roleId">Role ID to mention</param>
        /// <returns>Mention role formatted string</returns>
        public static string MentionRole(Snowflake roleId) => $"<@&{roleId.ToString()}>";

        /// <summary>
        /// Returns formatting string for custom emoji to be used in a message
        /// </summary>
        /// <param name="name">Name of the custom emoji</param>
        /// <param name="id">ID of the custom emoji</param>
        /// <param name="animated">If the emoji is animated</param>
        /// <returns>Custom emoji formatted string</returns>
        public static string CustomEmojiMessageString(Snowflake id, string name, bool animated) => $"<{CustomEmojiDataString(id, name, animated)}>";
        
        /// <summary>
        /// Returns formatting string for custom emoji to be used in a url
        /// </summary>
        /// <param name="name">Name of the custom emoji</param>
        /// <param name="id">ID of the custom emoji</param>
        /// <param name="animated">If the emoji is animated</param>
        /// <returns>Custom emoji formatted string</returns>
        public static string CustomEmojiDataString(Snowflake id, string name, bool animated) => $"{(animated ? "a" : "")}:{name}:{id.ToString()}";

        /// <summary>
        /// Displays a timestamp 
        /// </summary>
        /// <param name="timestamp">UNIX Timestamp</param>
        /// <param name="style">Display style for the timestamp</param>
        /// <returns></returns>
        public static string UnixTimestamp(int timestamp, TimestampStyles style = TimestampStyles.ShortDateTime)
        {
            return $"<t:{timestamp.ToString()}:{GetTimestampFlag(style)}>";
        }

        private static string GetTimestampFlag(TimestampStyles style)
        {
            switch (style)
            {
                case TimestampStyles.ShortTime:
                    return "t";
                case TimestampStyles.LongTime:
                    return "T";
                case TimestampStyles.ShortDate:
                    return "d";
                case TimestampStyles.LongDate:
                    return "D";
                case TimestampStyles.ShortDateTime:
                    return "f";
                case TimestampStyles.LongDateTime:
                    return "F";
                case TimestampStyles.RelativeTime:
                    return "R";
            }

            return "f";
        }
        
        /// <summary>
        /// Will display the message in italics
        /// </summary>
        /// <param name="message">Message to make italics</param>
        /// <returns>Italics formatted message</returns>
        public static string Italics(string message) => $"*{message}*";
        
        /// <summary>
        /// Will display the message in bold
        /// </summary>
        /// <param name="message">Message to make bold</param>
        /// <returns>Bold formatted message</returns>
        public static string Bold(string message) => $"**{message}**";
        
        /// <summary>
        /// Will display the message in italics and bold
        /// </summary>
        /// <param name="message">Message to make italics and bold</param>
        /// <returns>Bold and Italics formatted message</returns>
        public static string ItalicsBold(string message) => $"***{message}***";
        
        /// <summary>
        /// Will display the message in underline
        /// </summary>
        /// <param name="message">Message to make underline</param>
        /// <returns>Underline formatted message</returns>
        public static string Underline(string message) => $"__{message}__";
        
        /// <summary>
        /// Will display the message in underline and italics
        /// </summary>
        /// <param name="message">Message to make underline and italics</param>
        /// <returns>Underline and Italics formatted message</returns>
        public static string UnderlineItalics(string message) => $"__*{message}*__";
        
        /// <summary>
        /// Will display the message in underline and bold
        /// </summary>
        /// <param name="message">Message to make underline and bold</param>
        /// <returns>Underline and bold formatted message</returns>
        public static string UnderlineBold(string message) => $"__**{message}**__";
        
        /// <summary>
        /// Will display the message in underline and bold and italics
        /// </summary>
        /// <param name="message">Message to make underline and bold and italics</param>
        /// <returns>Underline and Bold and Italics formatted message</returns>
        public static string UnderlineBoldItalics(string message) => $"__***{message}***__";
        
        /// <summary>
        /// Will display the message with a strikethrough
        /// </summary>
        /// <param name="message">Message to make strikethrough</param>
        /// <returns>Strikethrough formatted message</returns>
        public static string Strikethrough(string message) => $"~~{message}~~";
        
        /// <summary>
        /// Will display the message as a one line code block
        /// </summary>
        /// <param name="message">Message to make code block</param>
        /// <returns>Code block formatted message</returns>
        public static string CodeBlockOneLine(string message) => $"`{message}`";
        
        /// <summary>
        /// Will display the message as a multiline code block
        /// </summary>
        /// <param name="message">Message to make multiline code block</param>
        /// <returns>Code block formatted message</returns>
        public static string CodeBlockMultiLine(string message) => $"```\n{message}\n```";

        /// <summary>
        /// Will display a multiline code bloc with the specified language
        /// </summary>
        /// <param name="message">Message to make code block with language</param>
        /// <param name="language">Language to display the code block as</param>
        /// <returns>Language code block formatted message</returns>
        public static string CodeBlockLanguage(string message, string language) => $"```{language}\n{message}\n```";
        
        /// <summary>
        /// Will display the message in single line block quote
        /// </summary>
        /// <param name="message">Message to make block quote</param>
        /// <returns>Block Quote formatted message</returns>
        public static string BlockQuoteSingleLine(string message) => $"> {message}";
        
        /// <summary>
        /// Will display the message in multiline block quote
        /// </summary>
        /// <param name="message">Message to make block quote</param>
        /// <returns>Multiline block quote formatted message</returns>
        public static string BlockQuoteMultiLine(string message) => $">>> {message}";
    }
}