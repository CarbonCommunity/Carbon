using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#create-guild-ban">Guild Ban Create Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildBanCreate
    {
        /// <summary>
        /// Number of days to delete messages for (0-7)
        /// </summary>
        [JsonProperty("delete_message_days")]
        public int? DeleteMessageDays { get; set; }
        
        /// <summary>
        /// Reason for the ban
        /// </summary>
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}