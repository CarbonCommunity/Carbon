using System;
using System.Net;

namespace Oxide.Ext.Discord.Entities.Api
{
    /// <summary>
    /// Error object that is returned to the caller when a request fails
    /// </summary>
    public class RestError
    {
        /// <summary>
        /// HTTP Status code
        /// </summary>
        public int HttpStatusCode { get; internal set; }
        
        /// <summary>
        /// The request method that was called
        /// </summary>
        public RequestMethod RequestMethod { get; } 
        
        /// <summary>
        /// The web exception from the request
        /// </summary>
        public WebException Exception { get; }
        
        /// <summary>
        /// The URI that was called
        /// </summary>
        public Uri Url { get; }
        
        /// <summary>
        /// What data was passed to the request
        /// </summary>
        public object Data { get; }
        
        /// <summary>
        /// If discord returned an error this will contain that error message
        /// </summary>
        public DiscordApiError DiscordError { get; internal set; }
        
        /// <summary>
        /// Full string response if we received one
        /// </summary>
        public string Message { get; internal set; }
        
        /// <summary>
        /// Creates a new rest error
        /// </summary>
        /// <param name="exception">The web exception we received</param>
        /// <param name="url">Url that was called</param>
        /// <param name="requestMethod">Request method that was used</param>
        /// <param name="data">Data passed to the request</param>
        public RestError(WebException exception, Uri url, RequestMethod requestMethod, object data)
        {
            Exception = exception;
            Url = url;
            RequestMethod = requestMethod;
            Data = data;
        }
    }
}