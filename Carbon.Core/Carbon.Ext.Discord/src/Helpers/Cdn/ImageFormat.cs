namespace Oxide.Ext.Discord.Helpers.Cdn
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/reference#image-formatting-image-formats">Image Formats</a>
    /// </summary>
    public enum ImageFormat
    {
        /// <summary>
        /// Automatically pick the image format
        /// </summary>
        Auto,
        
        /// <summary>
        /// Return image as a JPG
        /// </summary>
        Jpg,
        
        /// <summary>
        /// Return image as PNG
        /// </summary>
        Png,
        
        /// <summary>
        /// Return image as WebP
        /// </summary>
        WebP,
        
        /// <summary>
        /// Return image as GIF
        /// </summary>
        Gif,
        
        /// <summary>
        /// Lottie Image
        /// </summary>
        Lottie
    }
}