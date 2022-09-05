using Carbon.Core;
using Facepunch;
using Oxide.Plugins;
using System;
using System.Collections.Generic;
using static Oxide.Plugins.RustPlugin;

namespace Oxide.Core.Libraries
{
    public class Timers
    {
        public RustPlugin Plugin { get; }
        internal List<Timer> _timers { get; set; } = new List<Timer> ();

        public Timers () { }
        public Timers ( RustPlugin plugin )
        {
            Plugin = plugin;
        }

        public bool IsValid ()
        {
            return Plugin != null && Plugin.persistence != null;
        }
        public void Clear ()
        {
            foreach ( var timer in _timers )
            {
                timer.Destroy ();
            }

            _timers.Clear ();
            _timers = null;
        }

        public Persistence Persistence => Plugin.persistence;

        public Timer In ( float time, Action action )
        {
            if ( !IsValid () ) return null;

            var timer = new Timer ( Persistence );
            var activity = new Action ( () =>
            {
                try { action?.Invoke (); } catch ( Exception ex ) { Plugin.Error ( $" Timer {time}s has failed:", ex ); }

                timer.Destroy ();
                Pool.Free ( ref timer );
            } );

            timer.Callback = activity;
            Persistence.Invoke ( activity, time );
            return timer;
        }
        public Timer Once ( float time, Action action )
        {
            return In ( time, action );
        }
        public Timer Every ( float time, Action action )
        {
            if ( !IsValid () ) return null;

            var timer = new Timer ( Persistence );
            var activity = new Action ( () =>
            {
                try { action?.Invoke (); }
                catch ( Exception ex )
                {
                    Plugin.Error ( $" Timer {time}s has failed:", ex );

                    timer.Destroy ();
                    Pool.Free ( ref timer );
                }
            } );

            timer.Callback = activity;
            Persistence.InvokeRepeating ( activity, time, time );
            return timer;
        }
    }

    public class Timer : IDisposable
    {
        public Action Callback { get; set; }
        public Persistence Persistence { get; set; }

        public Timer () { }
        public Timer ( Persistence persistence )
        {
            Persistence = persistence;
        }

        public void Destroy ()
        {
            if ( Persistence != null )
            {
                Persistence.CancelInvoke ( Callback );
                Persistence.CancelInvokeFixedTime ( Callback );
            }

            if ( Callback != null )
            {
                Callback = null;
            }
        }

        public void Dispose ()
        {
            Destroy ();
        }
    }
}