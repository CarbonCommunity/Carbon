namespace Oxide.Ext.Discord.Entities.Messages
{
    /// <summary>
    /// Represents a file attachment for a discord message
    /// </summary>
    public class MessageFileAttachment
    {
        /// <summary>
        /// Name of the file attachment
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// Data for the file attachment
        /// </summary>
        public byte[] Data { get; set; }
        
        /// <summary>
        /// Web Content Type for the file attachment
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MessageFileAttachment()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">File Attachment Name</param>
        /// <param name="data">Data for the file</param>
        /// <param name="contentType">Web Content Type for the file attachment</param>
        public MessageFileAttachment(string fileName, byte[] data, string contentType)
        {
            FileName = fileName;
            Data = data;
            ContentType = contentType;
        }
    }
}