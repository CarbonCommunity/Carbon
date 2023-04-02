using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Channels.Stages
{
    /// <summary>
    /// Represents a channel <a href="https://discord.com/developers/docs/resources/stage-instance#stage-instance-object-stage-instance-structure">Stage Instance</a> within Discord.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class StageInstance : ISnowflakeEntity
    {
        /// <summary>
        /// The ID of this Stage instance
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// The guild ID of the associated Stage channel
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }
        
        /// <summary>
        /// The ID of the associated Stage channel
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake ChannelId { get; set; }
        
        /// <summary>
        /// The privacy level of the Stage instance
        /// </summary>
        [JsonProperty("privacy_level")]
        public PrivacyLevel PrivacyLevel { get; set; }
        
        /// <summary>
        /// Whether or not Stage discovery is disabled (deprecated)   
        /// </summary>
        [Obsolete("Deprecated by discord")]
        [JsonProperty("discoverable_disabled")]
        public bool DiscoverableDisabled { get; set; }
        
        /// <summary>
        /// The topic of the Stage instance (1-120 characters)
        /// </summary>
        [JsonProperty("topic")]
        public string Topic { get; set; }

        /// <summary>
        /// Creates a new Stage instance associated to a Stage channel.
        /// Requires the user to be a moderator of the Stage channel.
        /// See <a href="https://discord.com/developers/docs/resources/stage-instance#create-stage-instance">Create Stage Instance</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="channelId">Channel ID to create a stage in</param>
        /// <param name="topic">The topic for the stage instance</param>
        /// <param name="privacyLevel">Privacy level for the stage instance</param>
        /// <param name="callback">Callback with the new stage instance</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void CreateStageInstance(DiscordClient client, Snowflake channelId, string topic, PrivacyLevel privacyLevel = PrivacyLevel.GuildOnly, Action<StageInstance> callback = null, Action<RestError> error = null)
        {
            if (!channelId.IsValid()) throw new InvalidSnowflakeException(nameof(channelId));
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                ["channel_id"] = channelId,
                ["topic"] = topic,
                ["privacy_level"] = ((int)privacyLevel).ToString()
            };
            
            client.Bot.Rest.DoRequest($"/stage-instances", RequestMethod.POST, data, callback, error);
        }
        
        /// <summary>
        /// Gets the stage instance associated with the Stage channel, if it exists.
        /// See <a href="https://discord.com/developers/docs/resources/stage-instance#get-stage-instance">Get Stage Instance</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="channelId">Channel ID to get the stage instance for</param>
        /// <param name="callback">Callback with the new stage instance</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void GetStageInstance(DiscordClient client, Snowflake channelId, Action<StageInstance> callback = null, Action<RestError> error = null)
        {
            if (!channelId.IsValid()) throw new InvalidSnowflakeException(nameof(channelId));
            client.Bot.Rest.DoRequest($"/stage-instances/{channelId}", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Modifies fields of an existing Stage instance.
        /// Requires the user to be a moderator of the Stage channel.
        /// See <a href="https://discord.com/developers/docs/resources/stage-instance#modify-stage-instance">Update Stage Instance</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="topic">The new topic for the stage instance</param>
        /// <param name="privacyLevel">Privacy level for the stage instance</param>
        /// <param name="callback">Callback when the updated stage instance</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyStageInstance(DiscordClient client, string topic = null, PrivacyLevel? privacyLevel = null, Action<StageInstance> callback = null, Action<RestError> error = null)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(topic))
            {
                data["topic"] = topic;
            }

            if (privacyLevel.HasValue)
            {
                data["privacy_level"] = ((int)privacyLevel).ToString();
            }

            client.Bot.Rest.DoRequest($"/stage-instances/{ChannelId}", RequestMethod.PATCH, data, callback, error);
        }
        
        /// <summary>
        /// Deletes the Stage instance.
        /// Requires the user to be a moderator of the Stage channel.
        /// See <a href="https://discord.com/developers/docs/resources/stage-instance#delete-stage-instance">Delete Stage Instance</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback when the stage instance is deleted</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void DeleteStageInstance(DiscordClient client, Action callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/stage-instances/{ChannelId}", RequestMethod.DELETE, null, callback, error);
        }

        internal StageInstance Update(StageInstance stage)
        {
            StageInstance previous = (StageInstance)MemberwiseClone();
            if (stage.Topic != null)
                Topic = stage.Topic;

            PrivacyLevel = stage.PrivacyLevel;
            
            return previous;
        }
    }
}