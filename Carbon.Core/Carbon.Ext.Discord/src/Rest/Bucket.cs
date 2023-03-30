using System.Collections.Generic;
using System.Threading;
using Oxide.Ext.Discord.Helpers;
using Oxide.Ext.Discord.Logging;
namespace Oxide.Ext.Discord.Rest
{
    /// <summary>
    /// Represents a discord API bucket for a group of requests
    /// </summary>
    public class Bucket
    {
        /// <summary>
        /// The ID of this bucket which is based on the route
        /// </summary>
        public readonly string BucketId;
        
        /// <summary>
        /// The number of requests that can be made
        /// </summary>
        public int RateLimit;

        /// <summary>
        /// How many requests are remaining before hitting the rate limit for the bucket
        /// </summary>
        public int RateLimitRemaining;

        /// <summary>
        /// How long until the rate limit resets
        /// </summary>
        public double RateLimitReset;

        /// <summary>
        /// How long to wait before retrying request since there was a web exception
        /// </summary>
        public double ErrorDelayUntil;

        /// <summary>
        /// Rest Handler for the bucket
        /// </summary>
        public readonly RestHandler Handler;

        private readonly ILogger _logger;
        private readonly List<Request> _requests = new List<Request>();
        private readonly object _syncRoot = new object();
        
        private Thread _thread;

        /// <summary>
        /// Creates a new bucket for the given rest handler and bucket ID
        /// </summary>
        /// <param name="handler">Rest Handler for the bucket</param>
        /// <param name="bucketId">ID of the bucket</param>
        /// <param name="logger">Logger for the client</param>
        public Bucket(RestHandler handler, string bucketId, ILogger logger)
        {
            Handler = handler;
            BucketId = bucketId;
            _logger = logger;
            _logger.Debug($"New Bucket Created with id: {bucketId}");
        }

        /// <summary>
        /// Close the bucket and abort the thread
        /// </summary>
        public void Close()
        {
            _thread?.Abort();
            _thread = null;
        }

        /// <summary>
        /// Returns if this bucket is ready to be cleaned up.
        /// Should be cleaned up if the thread is not null and the RateLimitReset has expired
        /// </summary>
        /// <returns>True if we should cleanup the bucket; false otherwise</returns>
        public bool ShouldCleanup() => (_thread == null || !_thread.IsAlive) && Time.TimeSinceEpoch() > RateLimitReset;

        /// <summary>
        /// Queues a new request for the buck
        /// </summary>
        /// <param name="request">Request to be queued</param>
        public void QueueRequest(Request request)
        {
            request.Bucket = this;
            lock (_syncRoot)
            {
                _requests.Add(request);
            }

            if (_thread == null || !_thread.IsAlive)
            {
                _thread = new Thread(RunThread) {IsBackground = true};
                _thread.Start();
            }
        }

        /// <summary>
        /// Removes the request from the queue.
        /// Either the request completed successfully or there was an error and failed to succeed after 3 attempts
        /// </summary>
        /// <param name="request">Request to remove</param>
        public void DequeueRequest(Request request)
        {
            lock (_syncRoot)
            {
                _requests.Remove(request);
            }
        }

        private void RunThread()
        {
            try
            {
                while (RequestCount() > 0)
                {
                    FireRequests();
                }
            }
            catch (ThreadAbortException)
            {
                _logger.Debug("Bucket thread has been aborted.");
            }
        }

        private void FireRequests()
        {
            double timeSince = Time.TimeSinceEpoch();
            if (Handler.RateLimit.HasReachedRateLimit)
            {
                int resetIn = (int)(Handler.RateLimit.NextReset * 1000) + 1;
                _logger.Debug($"Global Rate limit hit. Sleeping until Reset: {resetIn.ToString()}ms");
                Thread.Sleep(resetIn);
                return;
            }
            
            if (RateLimitRemaining == 0 && RateLimitReset > timeSince)
            {
                int resetIn = (int) ((RateLimitReset - timeSince) * 1000);
                _logger.Debug($"Bucket Rate limit hit. Sleeping until Reset: {resetIn.ToString()}ms");
                Thread.Sleep(resetIn);
                return;
            }

            if (ErrorDelayUntil > timeSince)
            {
                int resetIn = (int) ((ErrorDelayUntil - timeSince) * 1000);
                _logger.Debug($"Web request error occured delaying next send until: {resetIn.ToString()}ms ");
                Thread.Sleep(resetIn);
                return;
            }

            for (int index = 0; index < RequestCount(); index++)
            {
                Request request = GetRequest(index);
                if (request.HasTimedOut())
                {
                    request.Close(false);
                }
            }

            //It's possible we removed a request that has timed out.
            if (RequestCount() == 0)
            {
                return;
            }
            
            Handler.RateLimit.FiredRequest();
            GetRequest(0).Fire();
        }

        private int RequestCount()
        {
            lock (_syncRoot)
            {
                return _requests.Count;
            }
        }

        private Request GetRequest(int index)
        {
            lock (_syncRoot)
            {
                return _requests[index];
            }
        }
    }
}
