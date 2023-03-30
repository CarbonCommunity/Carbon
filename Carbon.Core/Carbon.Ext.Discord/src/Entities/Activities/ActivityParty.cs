using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Activities
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#activity-object-activity-party">Activity Party</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ActivityParty
    {
        /// <summary>
        /// The id of the party
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// Used to show the party's current and maximum size
        /// </summary>
        [JsonProperty("size")]
        public List<int> Size { get; set; }

        /// <summary>
        /// The current party size
        /// </summary>
        public int CurrentSize => Size[0];

        /// <summary>
        /// The maximum party size
        /// </summary>
        public int MaxSize => Size[1];
    }
}