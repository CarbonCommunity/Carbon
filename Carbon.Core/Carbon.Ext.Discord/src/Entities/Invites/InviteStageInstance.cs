using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Guilds;

namespace Oxide.Ext.Discord.Entities.Invites
{
    /// <summary>
    /// Represents an <a href="https://discord.com/developers/docs/resources/invite#invite-stage-instance-object">Invite Stage Instance</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class InviteStageInstance
    {
        /// <summary>
        /// The members speaking in the Stage
        /// </summary>
        [JsonProperty("members")]
        public List<GuildMember> Members { get; set; }
        
        /// <summary>
        /// The number of users in the Stage
        /// </summary>
        [JsonProperty("participant_count")]
        public int ParticipantCount { get; set; }
        
        /// <summary>
        /// The number of users speaking in the Stage
        /// </summary>
        [JsonProperty("speaker_count")]
        public int SpeakerCount { get; set; }
        
        /// <summary>
        /// The topic of the Stage instance (1-120 characters)
        /// </summary>
        [JsonProperty("topic")]
        public string Topic { get; set; }
    }
}