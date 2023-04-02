using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Api
{
    /// <summary>
    /// Represents RateLimit entity when the rate limit is hit
    /// <a href="https://discord.com/developers/docs/topics/rate-limits#exceeding-a-rate-limit-rate-limit-response-structure">Rate Limit Structure</a>
    /// </summary>
    public class RateLimit
    {
        /// <summary>
        /// A message saying you are being rate limited.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// The number of seconds to wait before submitting another request.
        /// </summary>
        [JsonProperty("retry_after")]
        public float RetryAfter { get; set; }

        /// <summary>
        /// A value indicating if you are being globally rate limited or not
        /// </summary>
        [JsonProperty("global")]
        public bool Global { get; set; }
    }
}
