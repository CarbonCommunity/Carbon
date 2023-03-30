using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#ban-object-ban-structure">Guild Ban Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildBan
    {
        /// <summary>
        /// The reason for the ban
        /// </summary>
        [JsonProperty("reason")]
        public string Reason { get; set; }

        /// <summary>
        /// The banned user
        /// </summary>
        [JsonProperty("user")]
        public DiscordUser User { get; set; }
    }
}
