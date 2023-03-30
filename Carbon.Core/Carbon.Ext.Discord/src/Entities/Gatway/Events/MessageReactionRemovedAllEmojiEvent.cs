using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Emojis;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#message-reaction-remove-emoji-message-reaction-remove-emoji">Message Reaction Remove All</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class MessageReactionRemovedAllEmojiEvent : MessageReactionRemovedAllEvent
    {
        /// <summary>
        /// Emoji that was removed from the message
        /// </summary>
       public DiscordEmoji Emoji { get; set; }
    }
}
