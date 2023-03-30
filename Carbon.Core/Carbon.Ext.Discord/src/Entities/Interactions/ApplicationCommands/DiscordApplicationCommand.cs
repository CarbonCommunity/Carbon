using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-structure">ApplicationCommand</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DiscordApplicationCommand
    {
        /// <summary>
        /// Unique id of the command
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// The type of command, defaults 1 if not set
        /// </summary>
        [JsonProperty("type")]
        public ApplicationCommandType? Type { get; set; }
        
        /// <summary>
        /// Unique id of the parent application
        /// </summary>
        [JsonProperty("application_id")]
        public Snowflake ApplicationId { get; set; }
        
        /// <summary>
        /// Guild ID of the command, if not global
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake? GuildId { get; set; }
        
        /// <summary>
        /// 1-32 lowercase character name matching ^[\w-]{1,32}$
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Description of the command (1-100 characters)
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// The parameters for the command
        /// See <see cref="CommandOption"/>
        /// </summary>
        [JsonProperty("options")]
        public List<CommandOption> Options { get; set; }
        
        /// <summary>
        /// Whether the command is enabled by default when the app is added to a guild
        /// </summary>
        [JsonProperty("default_permission")]
        public bool? DefaultPermissions { get; set; }
        
        /// <summary>
        /// Auto incrementing version identifier updated during substantial record changes
        /// </summary>
        [JsonProperty("version")]
        public Snowflake Version { get; set; }
        
        /// <summary>
        /// Edit a command.
        /// Updates will be available in all guilds after 1 hour.
        /// See <a href="https://discord.com/developers/docs/interactions/application-commands#edit-global-application-command">Edit Global Application Command</a>
        /// See <a href="https://discord.com/developers/docs/interactions/application-commands#edit-guild-application-command">Edit Guild Application Command</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="update">Command Update</param>
        /// <param name="callback">Callback with updated command</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void Edit(DiscordClient client, CommandUpdate update, Action<DiscordApplicationCommand> callback = null, Action<RestError> error = null)
        {
            if (GuildId.HasValue)
            {
                client.Bot.Rest.DoRequest($"/applications/{ApplicationId}/guilds/{GuildId}/commands/{Id}", RequestMethod.PATCH, update, callback, error);
                return;
            }
            
            client.Bot.Rest.DoRequest($"/applications/{ApplicationId}/commands", RequestMethod.PATCH, update, callback, error);
        }
        
        /// <summary>
        /// Deletes a command
        /// See <a href="https://discord.com/developers/docs/interactions/application-commands#delete-global-application-command">Delete Global Application Command</a>
        /// See <a href="https://discord.com/developers/docs/interactions/application-commands#delete-guild-application-command">Delete Guild Application Command</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void Delete(DiscordClient client, Action callback = null, Action<RestError> error = null)
        {
            if (GuildId.HasValue)
            {
                client.Bot.Rest.DoRequest($"/applications/{ApplicationId}/guilds/{GuildId}/commands/{Id}", RequestMethod.DELETE, null, callback, error);
                return;
            }
            
            client.Bot.Rest.DoRequest($"/applications/{ApplicationId}/commands/{Id}", RequestMethod.DELETE, null, callback, error);
        }

        /// <summary>
        /// Fetches command permissions for a specific command for your application in a guild. Returns a GuildApplicationCommandPermissions object.
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild ID of the guild to get permissions for</param>
        /// <param name="callback">Callback with the permissions for the command</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetPermissions(DiscordClient client, Snowflake guildId, Action<GuildCommandPermissions> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            client.Bot.Rest.DoRequest($"/applications/{ApplicationId}/guilds/{guildId}/commands/{Id}/permissions", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Edits command permissions for a specific command for your application in a guild.
        /// Warning: This endpoint will overwrite existing permissions for the command in that guild
        /// Warning: Deleting or renaming a command will permanently delete all permissions for that command
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild ID of the guild to edit permissions for</param>
        /// <param name="permissions">List of permissions for the command</param>
        /// <param name="callback">Callback with the list of permissions</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void EditPermissions(DiscordClient client, Snowflake guildId, List<CommandPermissions> permissions, Action callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                ["permissions"] = permissions
            };
            
            client.Bot.Rest.DoRequest($"/applications/{ApplicationId}/guilds/{guildId}/commands/{Id}/permissions", RequestMethod.PUT, data, callback, error);
        }
    }
}