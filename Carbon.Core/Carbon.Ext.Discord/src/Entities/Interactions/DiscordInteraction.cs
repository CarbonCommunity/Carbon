using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Entities.Webhooks;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Entities.Interactions
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-interaction-structure">Interaction Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DiscordInteraction
    {
        /// <summary>
        /// Id of the interaction
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// ID of the application this interaction is for
        /// </summary>
        [JsonProperty("application_id")]
        public Snowflake ApplicationId { get; set; }
        
        /// <summary>
        /// The type of interaction
        /// See <see cref="InteractionType"/>
        /// </summary>
        [JsonProperty("type")]
        public InteractionType Type { get; set; }
        
        /// <summary>
        /// The command data payload
        /// See <see cref="InteractionData"/>
        /// </summary>
        [JsonProperty("data")]
        public InteractionData Data { get; set; }
        
        /// <summary>
        /// The guild it was sent from
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake? GuildId { get; set; }    
        
        /// <summary>
        /// The channel it was sent from
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake? ChannelId { get; set; }
        
        /// <summary>
        /// Guild member data for the invoking user
        /// </summary>
        [JsonProperty("member")]
        public GuildMember Member { get; set; }
        
        /// <summary>
        /// User object for the invoking user, if invoked in a DM
        /// </summary>
        [JsonProperty("user")]
        public DiscordUser User { get; set; }
        
        /// <summary>
        /// A continuation token for responding to the interaction
        /// Interaction tokens are valid for 15 minutes and can be used to send followup messages but you must send an initial response within 3 seconds of receiving the event.
        /// If the 3 second deadline is exceeded, the token will be invalidated.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; } 
        
        /// <summary>
        /// Read-only property, always 1
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; } 
        
        /// <summary>
        ///  For components, the message they were attached to
        /// </summary>
        [JsonProperty("message")]
        public DiscordMessage Message { get; set; }
        
        /// <summary>
        /// The selected language of the invoking user
        /// <a href="https://discord.com/developers/docs/dispatch/field-values#predefined-field-values-accepted-locales">Discord Locale Values</a>
        /// </summary>
        [JsonProperty("locale")]
        public string Locale { get; set; }
        
        /// <summary>
        /// The guild's preferred locale, if invoked in a guild
        /// <a href="https://discord.com/developers/docs/dispatch/field-values#predefined-field-values-accepted-locales">Discord Locale Values</a>
        /// </summary>
        [JsonProperty("guild_locale")]
        public string GuildLocale { get; set; }

        private InteractionDataParsed _parsed;

        /// <summary>
        /// Returns the interaction parsed args to make it easier to process that interaction.
        /// </summary>
        public InteractionDataParsed Parsed => _parsed ?? (_parsed = new InteractionDataParsed(this));
        
        /// <summary>
        /// Create a response to an Interaction from the gateway.
        /// See <a href="https://discord.com/developers/docs/interactions/receiving-and-responding#create-interaction-response">Create Interaction Response</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="response">Response to respond with</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void CreateInteractionResponse(DiscordClient client, InteractionResponse response, Action callback = null, Action<RestError> error = null)
        {
            response.Data?.Validate();
            
            client.Bot.Rest.DoRequest($"/interactions/{Id}/{Token}/callback", RequestMethod.POST, response, callback, error);
        }
        
        /// <summary>
        /// Edits the initial Interaction response
        /// See <a href="https://discord.com/developers/docs/interactions/receiving-and-responding#edit-original-interaction-response">Edit Original Interaction Response</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// /// <param name="message">Updated message</param>
        /// <param name="callback">Callback with the created message</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void EditOriginalInteractionResponse(DiscordClient client, DiscordMessage message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/webhooks/{ApplicationId}/{Token}/messages/@original", RequestMethod.PATCH, message, callback, error);
        }

        /// <summary>
        /// Deletes the initial Interaction response
        /// See <a href="https://discord.com/developers/docs/interactions/receiving-and-responding#delete-original-interaction-response">Delete Original Interaction Response</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void DeleteOriginalInteractionResponse(DiscordClient client, Action callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/webhooks/{ApplicationId}/{Token}/messages/@original", RequestMethod.DELETE, null, callback, error);
        }

        /// <summary>
        /// Create a followup message for an Interaction
        /// See <a href="https://discord.com/developers/docs/interactions/receiving-and-responding#create-followup-message">Create Followup Message</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="message">Message to follow up with</param>
        /// <param name="callback">Callback with the message</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void CreateFollowUpMessage(DiscordClient client, WebhookCreateMessage message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
        {
            message.Validate();
            message.ValidateInteractionMessage();
            client.Bot.Rest.DoRequest($"/webhooks/{ApplicationId}/{Token}", RequestMethod.POST, message, callback, error);
        }

        /// <summary>
        /// Edits a followup message for an Interaction
        /// See <a href="https://discord.com/developers/docs/interactions/receiving-and-responding#edit-followup-message">Edit Followup Message</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="messageId">Message ID of the follow up message</param>
        /// <param name="edit">Updated message</param>
        /// <param name="callback">Callback with the updated message</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void EditFollowUpMessage(DiscordClient client, Snowflake messageId, CommandFollowupUpdate edit, Action<DiscordMessage> callback = null, Action<RestError> error = null)
        {
            if (!messageId.IsValid()) throw new InvalidSnowflakeException(nameof(messageId));
            client.Bot.Rest.DoRequest($"/webhooks/{ApplicationId}/{Token}/messages/{messageId}", RequestMethod.PATCH, edit, callback, error);
        }

        /// <summary>
        /// Deletes a followup message for an Interaction
        /// See <a href="https://discord.com/developers/docs/interactions/receiving-and-responding#delete-followup-message">Delete Followup Message</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="messageId">Message ID to delete</param>
        /// <param name="callback">Callback with the updated message</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void DeleteFollowUpMessage(DiscordClient client, Snowflake messageId, Action callback = null, Action<RestError> error = null)
        {
            if (!messageId.IsValid()) throw new InvalidSnowflakeException(nameof(messageId));
            client.Bot.Rest.DoRequest($"/webhooks/{ApplicationId}/{Token}/messages/{messageId}", RequestMethod.DELETE, null, callback, error);
        }
    }
}