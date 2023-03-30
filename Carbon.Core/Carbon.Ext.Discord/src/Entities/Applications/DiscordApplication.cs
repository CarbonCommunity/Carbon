using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;
using Oxide.Ext.Discord.Entities.Teams;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Helpers.Cdn;

namespace Oxide.Ext.Discord.Entities.Applications
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/application#application-object">Application Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DiscordApplication
    {
        /// <summary>
        /// The id of the app
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// The name of the app
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// The icon hash of the app
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }
        
        /// <summary>
        /// The description of the app
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// An array of rpc origin urls, if rpc is enabled
        /// </summary>
        [JsonProperty("rpc_origins")]
        public List<string> RpcOrigins { get; set; }
        
        /// <summary>
        /// When false only app owner can join the app's bot to guilds
        /// </summary>
        [JsonProperty("bot_public")]
        public bool BotPublic { get; set; }
        
        /// <summary>
        /// When true the app's bot will only join upon completion of the full oauth2 code grant flow
        /// </summary>
        [JsonProperty("bot_require_code_grant")]
        public bool BotRequireCodeGrant { get; set; }
        
        /// <summary>
        /// The url of the app's terms of service
        /// </summary>
        [JsonProperty("terms_of_service_url")]
        public string TermsOfServiceUrl { get; set; }
        
        /// <summary>
        /// The url of the app's privacy policy
        /// </summary>
        [JsonProperty("privacy_policy_url")]
        public string PrivacyPolicyUrl { get; set; }
        
        /// <summary>
        /// Partial user object containing info on the owner of the application
        /// </summary>
        [JsonProperty("owner")]
        public DiscordUser Owner { get; set; }
        
        /// <summary>
        /// If this application is a game sold on Discord, this field will be the summary field for the store page of its primary sku
        /// </summary>
        [JsonProperty("summary")]
        public string Summary { get; set; }
        
        /// <summary>
        /// The hex encoded key for verification in interactions and the GameSDK's GetTicket
        /// </summary>
        [JsonProperty("verify_key")]
        public string Verify { get; set; }
        
        /// <summary>
        /// If the application belongs to a team, this will be a list of the members of that team
        /// </summary>
        [JsonProperty("team")]
        public DiscordTeam Team { get; set; }
        
        /// <summary>
        /// If this application is a game sold on Discord, this field will be the guild to which it has been linked
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake? GuildId { get; set; }
        
        /// <summary>
        /// If this application is a game sold on Discord, this field will be the id of the "Game SKU" that is created, if exists
        /// </summary>
        [JsonProperty("primary_sku_id")]
        public string PrimarySkuId { get; set; }
        
        /// <summary>
        /// If this application is a game sold on Discord, this field will be the URL slug that links to the store page
        /// </summary>
        [JsonProperty("slug")]
        public string Slug { get; set; }
        
        /// <summary>
        /// If this application is a game sold on Discord, this field will be the hash of the image on store embeds
        /// </summary>
        [JsonProperty("cover_image")]
        public string CoverImage { get; set; } 
        
        /// <summary>
        /// The application's public flags
        /// </summary>
        [JsonProperty("flags")]
        public ApplicationFlags? Flags { get; set; }

        /// <summary>
        /// Returns the URL for the applications Icon
        /// </summary>
        public string GetApplicationIconUrl => DiscordCdn.GetApplicationIconUrl(Id, Icon);
        
        /// <summary>
        /// Returns if the given application has the passed in application flag
        /// If Flags is null false is returned
        /// </summary>
        /// <param name="flag">Flag to compare against</param>
        /// <returns>True of application has flag; False Otherwise</returns>
        public bool HasApplicationFlag(ApplicationFlags flag)
        {
            return Flags.HasValue && (Flags.Value & flag) == flag;
        }

        /// <summary>
        /// Fetch all of the global commands for your application.
        /// Returns a list of ApplicationCommand.
        /// See <a href="https://discord.com/developers/docs/interactions/application-commands#get-global-application-commands">Get Global Application Commands</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with list of application commands</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGlobalCommands(DiscordClient client, Action<List<DiscordApplicationCommand>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/applications/{Id}/commands", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Fetch global command by ID
        /// See <a href="https://discord.com/developers/docs/interactions/application-commands#get-global-application-command">Get Global Application Command</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="commandId">ID of command to get</param>
        /// <param name="callback">Callback with list of application commands</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGlobalCommand(DiscordClient client, Snowflake commandId, Action<DiscordApplicationCommand> callback = null, Action<RestError> error = null)
        {
            if (!commandId.IsValid()) throw new InvalidSnowflakeException(nameof(commandId));
            client.Bot.Rest.DoRequest($"/applications/{Id}/commands/{commandId}", RequestMethod.GET, null, callback, error);
        }
        
        /// <summary>
        /// Create a new global command.
        /// New global commands will be available in all guilds after 1 hour.
        /// Note: Creating a command with the same name as an existing command for your application will overwrite the old command.
        /// See <a href="https://discord.com/developers/docs/interactions/application-commands#create-global-application-command">Create Global Application Command</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="create">Command to create</param>
        /// <param name="callback">Callback with the created command</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void CreateGlobalCommand(DiscordClient client, CommandCreate create, Action<DiscordApplicationCommand> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/applications/{Id}/commands", RequestMethod.POST, create, callback, error);
        }

        /// <summary>
        /// Takes a list of application commands, overwriting existing commands that are registered globally for this application. Updates will be available in all guilds after 1 hour.
        /// See <a href="https://discord.com/developers/docs/interactions/application-commands#bulk-overwrite-global-application-commands">Bulk Overwrite Global Application Commands</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="commands">List of commands to overwrite</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void BulkOverwriteGlobalCommands(DiscordClient client, List<DiscordApplicationCommand> commands, Action<List<DiscordApplicationCommand>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/applications/{Id}/commands", RequestMethod.PUT, commands, callback, error);
        }

        /// <summary>
        /// Fetch all of the guild commands for your application for a specific guild.
        /// See <a href="https://discord.com/developers/docs/interactions/application-commands#get-guild-application-commands">Get Guild Application Commands</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">ID of the guild to get commands for</param>
        /// <param name="callback">Callback with a list of guild application commands</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildCommands(DiscordClient client, Snowflake guildId, Action<List<DiscordApplicationCommand>> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            client.Bot.Rest.DoRequest($"/applications/{Id}/guilds/{guildId}/commands", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Get guild command by Id
        /// See <a href="https://discord.com/developers/docs/interactions/application-commands#get-guild-application-command">Get Guild Application Command</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">ID of the guild to get commands for</param>
        /// <param name="commandId">ID of the command to get</param>
        /// <param name="callback">Callback with a list of guild application commands</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildCommand(DiscordClient client, Snowflake guildId, Snowflake commandId, Action<DiscordApplicationCommand> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            if (!commandId.IsValid()) throw new InvalidSnowflakeException(nameof(commandId));
            client.Bot.Rest.DoRequest($"/applications/{Id}/guilds/{guildId}/commands/{commandId}", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Create a new guild command.
        /// New guild commands will be available in the guild immediately.
        /// See <a href="https://discord.com/developers/docs/interactions/application-commands#create-guild-application-command">Create Guild Application Command</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild ID to create the command in</param>
        /// <param name="create">Command to create</param>
        /// <param name="callback">Callback with the created command</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void CreateGuildCommand(DiscordClient client, Snowflake guildId, CommandCreate create, Action<DiscordApplicationCommand> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            client.Bot.Rest.DoRequest($"/applications/{Id}/guilds/{guildId}/commands", RequestMethod.POST, create, callback, error);
        }

        /// <summary>
        /// Fetches command permissions for all commands for your application in a guild. Returns an array of GuildApplicationCommandPermissions objects.
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild ID to get the permissions from</param>
        /// <param name="callback">Callback with the list of permissions</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildCommandPermissions(DiscordClient client, Snowflake guildId, Action<List<GuildCommandPermissions>> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            client.Bot.Rest.DoRequest($"/applications/{Id}/guilds/{guildId}/commands/permissions", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Batch edits permissions for all commands in a guild.
        /// Warning: This endpoint will overwrite all existing permissions for all commands in a guild
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild ID to update the permissions for</param>
        /// <param name="permissions">List of permissions for the commands</param>
        /// <param name="callback">Callback with the list of permissions</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void BatchEditCommandPermissions(DiscordClient client, Snowflake guildId, List<GuildCommandPermissions> permissions, Action callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            client.Bot.Rest.DoRequest($"/applications/{Id}/guilds/{guildId}/commands/permissions", RequestMethod.PUT, permissions, callback, error);
        }
    }
}