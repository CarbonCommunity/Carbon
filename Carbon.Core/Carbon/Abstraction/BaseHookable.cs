using Newtonsoft.Json;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Core
{
    public class BaseHookable
    {
        public Dictionary<string, List<MethodInfo>> HookCache { get; internal set; } = new Dictionary<string, List<MethodInfo>> ();
        public Dictionary<string, List<MethodInfo>> HookMethodAttributeCache { get; internal set; } = new Dictionary<string, List<MethodInfo>> ();

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public VersionNumber Version { get; set; }

        [JsonProperty]
        public double TotalHookTime { get; internal set; }

        public Type Type { get; set; }

        #region Tracking

        internal Stopwatch _trackStopwatch = new Stopwatch ();

        public virtual void TrackStart ()
        {
            if ( !CarbonCore.IsServerFullyInitialized )
            {
                return;
            }

            var stopwatch = _trackStopwatch;
            if ( stopwatch.IsRunning )
            {
                return;
            }
            stopwatch.Start ();
        }
        public virtual void TrackEnd ()
        {
            if ( !CarbonCore.IsServerFullyInitialized )
            {
                return;
            }

            var stopwatch = _trackStopwatch;
            if ( !stopwatch.IsRunning )
            {
                return;
            }
            stopwatch.Stop ();
            TotalHookTime += stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset ();
        }

        #endregion

        public T To<T> ()
        {
            if ( this is T result )
            {
                return result;
            }

            return default;
        }
    }
}
