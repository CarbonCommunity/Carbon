using Oxide.Ext.Discord.Entities;

namespace Oxide.Ext.Discord.Interfaces
{
    /// <summary>
    /// Interface used to get the entity ID from an entity
    /// </summary>
    public interface ISnowflakeEntity
    {
        /// <summary>
        /// Returns the unique ID for this entity
        /// </summary>
        /// <returns></returns>
        Snowflake Id { get; }
    }
}