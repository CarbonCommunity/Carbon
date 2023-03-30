using System;
using Newtonsoft.Json;
using Oxide.Core.Configuration;

namespace Oxide.Ext.Discord.Configuration
{
    /// <summary>
    /// Represents Discord Extension Config
    /// </summary>
    public class DiscordConfig : ConfigFile
    {
        /// <summary>
        /// Discord Config commands options
        /// </summary>
        [JsonProperty("Commands")]
        public DiscordCommandsConfig Commands { get; set; }
        
        /// <summary>
        /// Constructor for discord config
        /// </summary>
        /// <param name="filename">Filename to use</param>
        public DiscordConfig(string filename) : base(filename)
        {
            
        }

        /// <summary>
        /// Load the config file and populate it.
        /// </summary>
        /// <param name="filename"></param>
        public override void Load(string filename = null)
        {
            try
            {
                base.Load(filename);

                Commands = new DiscordCommandsConfig
                {
                    CommandPrefixes = Commands?.CommandPrefixes ?? new[] {'/', '!'}
                };
            }
            catch (Exception ex)
            {
                DiscordExtension.GlobalLogger.Error($"Failed to load config file. Generating new Config.\n{ex}");
                Commands = new DiscordCommandsConfig
                {
                    CommandPrefixes = new[] {'/', '!'}
                };
                Save(filename);
            }
        }
    }
}