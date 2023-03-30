using System;
using System.Text;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Extensions;
using Oxide.Ext.Discord.Logging;
using Oxide.Ext.Discord.RateLimits;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Rest
{
    /// <summary>
    /// Represents a REST handler for a bot
    /// </summary>
    public class RestHandler
    {
        /// <summary>
        /// Global Rate Limit for the bot
        /// </summary>
        public readonly RestRateLimit RateLimit = new RestRateLimit();
        
        /// <summary>
        /// The request buckets for the bot
        /// </summary>
        public readonly Hash<string, Bucket> Buckets = new Hash<string, Bucket>();

        /// <summary>
        /// The authorization header value
        /// </summary>
        private readonly string _authorization;
        
        private readonly ILogger _logger;
        private readonly object _bucketSyncObject = new object();

        /// <summary>
        /// Creates a new REST handler for a bot client
        /// </summary>
        /// <param name="client">Client the request is for</param>
        /// <param name="logger">Logger from the client</param>
        public RestHandler(BotClient client, ILogger logger)
        {
            _authorization = $"Bot {client.Settings.ApiToken}";
            _logger = logger;
        }

        /// <summary>
        /// Creates a new request and queues it to be ran
        /// </summary>
        /// <param name="url">URL of the request</param>
        /// <param name="method">HTTP method of the request</param>
        /// <param name="data">Data to be sent with the request</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Error callback if an error occurs</param>
        public void DoRequest(string url, RequestMethod method, object data, Action callback, Action<RestError> error)
        {
            CreateRequest(method, url, data, response => callback?.Invoke(), error);
        }

        /// <summary>
        /// Creates a new request and queues it to be ran
        /// </summary>
        /// <param name="url">URL of the request</param>
        /// <param name="method">HTTP method of the request</param>
        /// <param name="data">Data to be sent with the request</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Error callback if an error occurs</param>
        /// <typeparam name="T">The type that is expected to be returned</typeparam>
        public void DoRequest<T>(string url, RequestMethod method, object data, Action<T> callback, Action<RestError> error)
        {
            CreateRequest(method, url, data, response =>
            {
                callback?.Invoke(response.ParseData<T>());
            }, error);
        }

        /// <summary>
        /// Creates a new request and queues it to be ran
        /// </summary>
        /// <param name="url">URL of the request</param>
        /// <param name="method">HTTP method of the request</param>
        /// <param name="data">Data to be sent with the request</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Error callback if an error occurs</param>
        private void CreateRequest(RequestMethod method, string url, object data, Action<RestResponse> callback, Action<RestError> error)
        {
            Request request = new Request(method, url, data, _authorization, callback, error, _logger);
            QueueRequest(request, _logger);
            CleanupExpired();
        }
        
        /// <summary>
        /// Removed buckets that are old and not being used
        /// </summary>
        public void CleanupExpired()
        {
            lock (_bucketSyncObject)
            {
                Buckets.RemoveAll(b => b.ShouldCleanup());
            }
        }

        /// <summary>
        /// Queues the request
        /// </summary>
        /// <param name="request">Request to queue</param>
        /// <param name="logger">Logger to use</param>
        public void QueueRequest(Request request, ILogger logger)
        {
            string bucketId = GetBucketId(request.Route);
            lock (_bucketSyncObject)
            {
                Bucket bucket = Buckets[bucketId];
                if (bucket == null)
                {
                    bucket = new Bucket(this, bucketId, logger);
                    Buckets[bucketId] = bucket;
                }

                bucket.QueueRequest(request);
            }
        }
        
        /// <summary>
        /// Shutdown the REST handler
        /// </summary>
        public void Shutdown()
        {
            lock (_bucketSyncObject)
            {
                foreach (Bucket bucket in Buckets.Values)
                {
                    bucket.Close();
                }
                Buckets.Clear();
            }
            
            RateLimit.Shutdown();
        }
        
        /// <summary>
        /// Returns the Rate Limit Bucket for the given route
        /// https://discord.com/developers/docs/topics/rate-limits#rate-limits
        /// </summary>
        /// <param name="route">API Route</param>
        /// <returns>Bucket ID for route</returns>
        private static string GetBucketId(string route)
        {
            string[] routeSegments = route.Split('/');
            StringBuilder bucket = new StringBuilder(routeSegments[0]);
            bucket.Append('/');
            string previousSegment = routeSegments[0];
            for (int index = 1; index < routeSegments.Length; index++)
            {
                string segment = routeSegments[index];
                switch (previousSegment)
                {
                    // Reactions routes and sub-routes all share the same bucket
                    case "reactions":
                        return bucket.ToString();
                        
                    // Literal IDs should only be taken account if they are the Major ID (Channel ID / Guild ID / Webhook ID)
                    case "guilds":
                    case "channels": 
                    case "webhooks":
                        break;

                    default:
                        if (ulong.TryParse(segment, out ulong _))
                        {
                            bucket.Append("id/");
                            previousSegment = segment;
                            continue;
                        }

                        break;
                }
                
                // All other parts of the route should be considered as part of the bucket identifier
                bucket.Append(previousSegment = segment);
                bucket.Append("/");
            }

            return bucket.ToString();
        }
    }
}
