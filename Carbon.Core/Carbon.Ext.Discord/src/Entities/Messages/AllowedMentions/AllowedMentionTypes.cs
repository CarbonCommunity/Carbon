using System.ComponentModel;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Helpers.Converters;

namespace Oxide.Ext.Discord.Entities.Messages.AllowedMentions
{
    /// <summary>
    ///  Represents a <a href="https://discord.com/developers/docs/resources/channel#allowed-mentions-object-allowed-mention-types">Allowed Mention Types</a> for a message
    /// </summary>
    [JsonConverter(typeof(DiscordEnumConverter))]
    public enum AllowedMentionTypes
    {
        /// <summary>
        /// Discord Extension doesn't currently support this allowed mention type.
        /// </summary>
        Unknown,
        
        /// <summary>
        /// Controls role mentions
        /// </summary>
        [System.ComponentModel.Description ("roles")] 
        Roles,
        
        /// <summary>
        /// 	Controls user mentions
        /// </summary>
        [System.ComponentModel.Description ("users")] 
        Users,
        
        /// <summary>
        /// Controls @everyone and @here mentions
        /// </summary>
        [System.ComponentModel.Description ("everyone")] 
        Everyone,
    }
}
