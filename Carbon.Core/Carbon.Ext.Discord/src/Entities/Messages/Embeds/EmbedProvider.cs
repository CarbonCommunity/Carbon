using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Messages.Embeds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/channel#embed-object-embed-provider-structure">Embed Provider Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class EmbedProvider
    {
        /// <summary>
        /// Name of provider
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Url of provider
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Embed Provider Constructor
        /// </summary>
        public EmbedProvider()
        {
            
        }
        
        /// <summary>
        /// Embed Provider Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        public EmbedProvider(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}