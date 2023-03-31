using System.Text.RegularExpressions;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Helpers
{
    /// <summary>
    /// Provides helper methods for validation
    /// </summary>
    public static class Validation
    {
        private static readonly Regex EmojiValidation = new Regex("^.+:[0-9]+$", RegexOptions.Compiled);
        private static readonly Regex FilenameValidation = new Regex("^[a-zA-Z0-9_.-]*$", RegexOptions.Compiled);
        
        /// <summary>
        /// Validates that the emoji string entered is valid.
        /// </summary>
        /// <param name="emoji"></param>
        /// <returns></returns>
        public static void ValidateEmoji(string emoji)
        {
            if (string.IsNullOrEmpty(emoji))
            {
                throw new InvalidEmojiException(emoji, "Emoji string cannot be null or empty.");
            }

            if (emoji.Length == 2 && !char.IsSurrogatePair(emoji[0], emoji[1]))
            {
                throw new InvalidEmojiException(emoji, "Emoji of length 2 must be a surrogate pair");
            }

            if (emoji.Length > 2 && !EmojiValidation.IsMatch(emoji))
            {
                throw new InvalidEmojiException(emoji, "Emoji string is not in the correct format.\n" +
                                                       "If using a normal emoji please use the unicode character for that emoji.\n" +
                                                       "If using a custom emoji the format must be emojiName:emojiId\n" +
                                                       "If using a custom animated emoji the format must be a:emojiName:emojiId");
            }
        }

        /// <summary>
        /// Checks if the filename is a valid discord filename.
        /// </summary>
        /// <param name="filename">Filename to validate</param>
        public static void ValidateFilename(string filename)
        {
            if (!FilenameValidation.IsMatch(filename))
            {
                throw new InvalidFileNameException(filename);
            }
        }
    }
}