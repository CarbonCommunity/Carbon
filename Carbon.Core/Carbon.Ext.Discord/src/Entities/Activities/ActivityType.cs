namespace Oxide.Ext.Discord.Entities.Activities
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#activity-object-activity-types">Activity Types</a>
    /// </summary>
    public enum ActivityType
    {
        /// Playing {name}
        Game = 0,       
        
        /// Streaming {name}
        Streaming = 1,  
        
        /// Listening {name}
        Listening = 2,  
        
        /// Watching {name}
        Watching = 3,   
        
        ///{emoji} {name} EX: ":smiley: I am cool"
        Custom = 4,     
        
        /// Competing in {name}
        Competing = 5   
    }
}
