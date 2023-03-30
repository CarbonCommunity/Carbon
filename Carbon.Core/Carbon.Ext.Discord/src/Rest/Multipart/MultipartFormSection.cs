using System.Text;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Rest.Multipart
{
    /// <summary>
    /// Represents a MultiPartFormSection
    /// </summary>
    internal class MultipartFormSection : IMultipartSection
    {
        /// <summary>
        /// Name of the file being sent
        /// </summary>
        public string FileName => null;
        
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
        /// Constructor for byte form data
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        internal MultipartFormSection(string sectionName, byte[] data, string contentType)
        {
            ContentType = contentType;
            Data = data;
            SectionName = sectionName;
        }
        
        /// <summary>
        /// Constructor for string form data
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        internal MultipartFormSection(string sectionName, string data, string contentType) : this(contentType, Encoding.UTF8.GetBytes(data), sectionName)
        {

        }
        
        /// <summary>
        /// Constructor for object form data
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        internal MultipartFormSection(string sectionName, object data, string contentType) : this(contentType, JsonConvert.SerializeObject(data, DiscordExtension.ExtensionSerializeSettings), sectionName)
        {

        }
    }
}