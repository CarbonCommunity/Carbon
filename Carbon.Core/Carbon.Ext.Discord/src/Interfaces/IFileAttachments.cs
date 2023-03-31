using System.Collections.Generic;
using Oxide.Ext.Discord.Entities.Messages;

namespace Oxide.Ext.Discord.Interfaces
{
    /// <summary>
    /// Represents and interface for entities that can upload files
    /// </summary>
    public interface IFileAttachments
    {
        /// <summary>
        /// File attachments for an entity
        /// </summary>
        List<MessageFileAttachment> FileAttachments { get; set; }
    }
}