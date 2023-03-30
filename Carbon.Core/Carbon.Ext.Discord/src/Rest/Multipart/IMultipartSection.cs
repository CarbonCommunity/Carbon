namespace Oxide.Ext.Discord.Rest.Multipart
{
    /// <summary>
    /// Represents a Multipart section for MultiPart requests
    /// </summary>
    internal interface IMultipartSection
    {
        /// <summary>
        /// Name of the file for the section
        /// </summary>
        string FileName
        {
            get;
        }
        
        /// <summary>
        /// Content type for the section
        /// </summary>
        string ContentType
        {
            get;
        }

        /// <summary>
        /// Data for the section
        /// </summary>
        byte[] Data
        {
            get;
        }

        /// <summary>
        /// Name of the section
        /// </summary>
        string SectionName
        {
            get;
        }
    }
}