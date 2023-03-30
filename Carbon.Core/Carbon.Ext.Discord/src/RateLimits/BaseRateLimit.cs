using System.Timers;
using Oxide.Ext.Discord.Helpers;

namespace Oxide.Ext.Discord.RateLimits
{
    /// <summary>
    /// Represents a base rate limit for websocket and rest api requests
    /// </summary>
    public class BaseRateLimit
    {
        /// <summary>
        /// The number of requests that have executed since the last reset
        /// </summary>
        protected int NumRequests;
        
        /// <summary>
        /// The UNIX timestamp of the last reset
        /// </summary>
        protected double LastReset;
        
        /// <summary>
        /// The max number of requests this rate limit can handle per interval
        /// </summary>
        protected readonly int MaxRequests;
        
        /// <summary>
        /// The interval in which this resets at
        /// </summary>
        protected readonly double ResetInterval;
        
        private Timer _timer;
        
        private readonly object _syncRoot = new object();

        /// <summary>
        /// Base Rate Limit Constructor
        /// </summary>
        /// <param name="maxRequests">Max requests per interval</param>
        /// <param name="interval">Reset Interval</param>
        protected BaseRateLimit(int maxRequests, double interval)
        {
            MaxRequests = maxRequests;
            ResetInterval = interval;
            
            _timer = new Timer(interval);
            _timer.Elapsed += ResetRateLimit;
            _timer.Start();
            LastReset = Time.TimeSinceEpoch();
        }
        
        private void ResetRateLimit(object sender, ElapsedEventArgs e)
        {
            lock (_syncRoot)
            {
                NumRequests = 0;
                LastReset = Time.TimeSinceEpoch();
            }
        }
        
        /// <summary>
        /// Called when an API request is fired
        /// </summary>
        public void FiredRequest()
        {
            lock (_syncRoot)
            {
                NumRequests += 1;
            }
        }
        
        /// <summary>
        /// Returns true if we have reached the global rate limit 
        /// </summary>
        public bool HasReachedRateLimit => NumRequests >= MaxRequests;

        /// <summary>
        /// Returns how long until the current rate limit period will expire
        /// </summary>
        public virtual double NextReset => Time.TimeSinceEpoch() + ResetInterval * 1000 - LastReset;

        /// <summary>
        /// Called when a bot is shutting down
        /// </summary>
        public void Shutdown()
        {
            if (_timer == null)
            {
                return;
            }
            
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }
    }
}