using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Webhooks;

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/interactions/receiving-and-responding#create-followup-message">Command Followup</a> within discord.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [Obsolete("Replaced with WebhookCreateMessage. This will be removed in the May 2022 update.")]
    public class CommandFollowupCreate : WebhookCreateMessage
    {

    }
}