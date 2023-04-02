using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Integrations
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#integration-account-object">Integration Account Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Account
    {
        /// <summary>
        /// ID of the account
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }

        /// <summary>
        /// Name of the account
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
