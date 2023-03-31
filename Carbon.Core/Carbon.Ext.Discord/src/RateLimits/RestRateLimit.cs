using System;
using Oxide.Ext.Discord.Helpers;
namespace Oxide.Ext.Discord.RateLimits
{
    /// <summary>
    /// Represents a rate limit for rest requests
    /// </summary>
    public class RestRateLimit : BaseRateLimit
    {
        private double _retryAfter;

        /// <summary>
        /// Constructor for RestRateLimit
        /// </summary>
        public RestRateLimit() : base(110, 60)
        {
            
        }
        
        /// <summary>
        /// Called if we receive a header notifying us of hitting the rate limit
        /// </summary>
        /// <param name="retryAfter">How long until we should retry API request again</param>
        public void ReachedRateLimit(double retryAfter)
        {
            NumRequests = MaxRequests;
            _retryAfter = retryAfter;
        }

        /// <summary>
        /// Returns how long until the current rate limit period will expire
        /// </summary>
        public override double NextReset => Time.TimeSinceEpoch() - Math.Max(LastReset + ResetInterval, Time.TimeSinceEpoch() + _retryAfter) ;
    }
}