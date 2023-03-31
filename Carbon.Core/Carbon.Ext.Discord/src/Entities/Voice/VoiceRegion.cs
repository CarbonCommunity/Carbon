using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;

namespace Oxide.Ext.Discord.Entities.Voice
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/voice#voice-region-object">Voice Region Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class VoiceRegion
    {
        /// <summary>
        /// Unique ID for the region
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }

        /// <summary>
        /// Name of the region
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// True if this is a vip-only server
        /// </summary>
        [Obsolete("This field is no longer sent by discord")]
        [JsonProperty("vip")]
        public bool Vip { get; set; }

        /// <summary>
        /// True for a single server that is closest to the current user's client
        /// </summary>
        [JsonProperty("optimal")]
        public bool Optimal { get; set; }

        /// <summary>
        /// Whether this is a deprecated voice region (avoid switching to these)
        /// </summary>
        [JsonProperty("deprecated")]
        public bool Deprecated { get; set; }

        /// <summary>
        /// Whether this is a custom voice region (used for events/etc)
        /// </summary>
        [JsonProperty("custom")]
        public bool Custom { get; set; }

        /// <summary>
        /// Returns an array of voice region objects that can be used when creating servers.
        /// See <a href="https://discord.com/developers/docs/resources/voice#list-voice-regions">List Voice Regions</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with a list of voice regions</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void ListVoiceRegions(DiscordClient client, Action<List<VoiceRegion>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/voice/regions", RequestMethod.GET, null, callback, error);
        }
    }
}
