using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Messages.Embeds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/channel#embed-object-embed-field-structure">Embed Field Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class EmbedField
    {
        /// <summary>
        /// Name of the field
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Value of the field
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Whether or not this field should display inline
        /// </summary>
        [JsonProperty("inline")]
        public bool Inline { get; set; }

        /// <summary>
        /// Embed Field constructor
        /// </summary>
        public EmbedField()
        {
            
        }
        
        /// <summary>
        /// Embed Field constructor
        /// </summary>
        /// <param name="name">Field Name</param>
        /// <param name="value">Field Value</param>
        /// <param name="inline">Should field be inlined</param>
        public EmbedField(string name, string value, bool inline)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }
    }
}