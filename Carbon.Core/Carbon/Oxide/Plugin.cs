using Carbon.Core.Harmony;
using System;
using Oxide.Core;
using System.Diagnostics;

namespace Oxide.Plugins
{
    public class Plugin
    {
        public bool IsCorePlugin { get; set; }
        public Type Type { get; set; }

        public string Title { get; set; } = "Rust";
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public VersionNumber Version { get; set; }
        public int ResourceId { get; set; }
        public bool HasConfig { get; set; }
        public bool HasMessages { get; set; }

        private Stopwatch trackStopwatch = new Stopwatch ();
        public double TotalHookTime { get; internal set; }

        public void TrackStart ()
        {
            if ( this.IsCorePlugin )
            {
                return;
            }

            Stopwatch stopwatch = this.trackStopwatch;
            if ( stopwatch.IsRunning )
            {
                return;
            }
            stopwatch.Start ();
        }
        public void TrackEnd ()
        {
            if ( this.IsCorePlugin )
            {
                return;
            }
            Stopwatch stopwatch = this.trackStopwatch;
            if ( !stopwatch.IsRunning )
            {
                return;
            }
            stopwatch.Stop ();
            this.TotalHookTime += stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset ();
        }

        public T Call<T> ( string hook,  params object [] args )
        {
            return ( T )HookExecutor.CallHook ( this, hook, args );
        }
        public object Call ( string hook, params object [] args )
        {
            return Call<object> ( hook, args );
        }

        public object CallHook ( string name, params object [] args )
        {
            return Call ( name, args );
        }
        public object CallPublicHook ( string name, params object [] args )
        {
            return HookExecutor.CallPublicHook ( this, name, args );
        }

        public bool IsLoaded => true;
    }
}