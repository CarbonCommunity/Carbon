namespace Oxide.Ext.Discord.Rest.Multipart
{
    /// <summary>
    /// Represents a MultiPartFileSection
    /// </summary>
    internal class MultipartFileSection : IMultipartSection
    {
        /// <summary>
        /// Name of the file being sent
        /// </summary>
        public string FileName { get; }
        
        /// <summary>
        /// Content Type for the file being sent
        /// </summary>
        public string ContentType { get; }
        
        /// <summary>
        /// Data for the file being sent
        /// </summary>
        public byte[] Data { get; }
        
        /// <summary>
        /// Section name for the multipart section
        /// </summary>
        public string SectionName { get; }

        /// <summary>
        /// Constructor for a multipart file
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        internal MultipartFileSection(string sectionName, string fileName, byte[] data, string contentType)
        {
            FileName = fileName;
            ContentType = contentType;
            Data = data;
            SectionName = sectionName;
        }
    }
}