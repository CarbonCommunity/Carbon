using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Extensions
{
    /// <summary>
    /// Adds extension methods to Discord User to allow sending server chat commands to the player
    /// </summary>
    public static class DiscordUserExt
    {
        /// <summary>
        /// Send chat message to the user if they're connected
        /// </summary>
        /// <param name="user">User to send the message to on the server</param>
        /// <param name="message">Message to send</param>
        public static void SendChatMessage(this DiscordUser user, string message)
        {
            IPlayer player = user.Player;
            if (player != null && player.IsConnected)
            {
                player.Message(message);
            }
        }

        /// <summary>
        /// Send chat message to the user if they're connected
        /// </summary>
        /// <param name="user">User to send the message to on the server</param>
        /// <param name="message">Message to send</param>
        /// <param name="prefix">Message Prefix</param>
        /// <param name="args">Message Args</param>
        public static void SendChatMessage(this DiscordUser user, string message, string prefix, params object[] args)
        {
            IPlayer player = user.Player;
            if (player != null && player.IsConnected)
            {
                player.Message(message, prefix, args);
            }
        }

        /// <summary>
        /// Return if the discord user has the given oxide permission.
        /// If the user is not linked this will return false
        /// </summary>
        /// <param name="user">User to check for permission</param>
        /// <param name="permission">Permission to check for</param>
        /// <returns>True if use is linked and has permission; False otherwise</returns>
        public static bool HasPermission(this DiscordUser user, string permission)
        {
            return user.Player?.HasPermission(permission) ?? false;
        }
        
        /// <summary>
        /// Returns true if the player is linked
        /// </summary>
        /// <param name="user">Discord user to check if they're linked</param>
        /// <returns>True if linked; False otherwise</returns>
        public static bool IsLinked(this DiscordUser user)
        {
            return DiscordExtension.DiscordLink.IsLinked(user.Id);
        }
    }
}