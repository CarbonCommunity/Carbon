using Carbon.Core.Harmony;
using System;
using Oxide.Core;
using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace Oxide.Plugins
{
    public class Plugin
    {
        public Dictionary<string, MethodInfo> HookCache { get; } = new Dictionary<string, MethodInfo> ();

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
            TotalHookTime += stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset ();
        }

        #region Calls

        public T Call<T> ( string hook )
        {
            return ( T )HookExecutor.CallHook ( this, hook );
        }
        public T Call<T> ( string hook, object arg1 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1 );
        }
        public T Call<T> ( string hook, object arg1, object arg2 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2 );
        }
        public T Call<T> ( string hook, object arg1, object arg2, object arg3 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3 );
        }
        public T Call<T> ( string hook, object arg1, object arg2, object arg3, object arg4 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4 );
        }
        public T Call<T> ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5 );
        }
        public T Call<T> ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6 );
        }
        public T Call<T> ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7 );
        }
        public T Call<T> ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7 , arg8 );
        }
        public T Call<T> ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 );
        }

        public object Call ( string hook )
        {
            return HookExecutor.CallHook ( this, hook );
        }
        public object Call ( string hook, object arg1 )
        {
            return HookExecutor.CallHook ( this, hook, arg1 );
        }
        public object Call ( string hook, object arg1, object arg2 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2 );
        }
        public object Call ( string hook, object arg1, object arg2, object arg3 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3 );
        }
        public object Call ( string hook, object arg1, object arg2, object arg3, object arg4 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4 );
        }
        public object Call ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5 );
        }
        public object Call ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6 );
        }
        public object Call ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7 );
        }
        public object Call ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 );
        }
        public object Call ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 );
        }

        public T CallHook<T> ( string hook )
        {
            return (T)HookExecutor.CallHook ( this, hook );
        }
        public T CallHook<T> ( string hook, object arg1 )
        {
            return (T)HookExecutor.CallHook ( this, hook, arg1 );
        }
        public T CallHook<T> ( string hook, object arg1, object arg2 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2 );
        }
        public T CallHook<T> ( string hook, object arg1, object arg2, object arg3 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3 );
        }
        public T CallHook<T> ( string hook, object arg1, object arg2, object arg3, object arg4 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4 );
        }
        public T CallHook<T> ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5 );
        }
        public T CallHook<T> ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6 );
        }
        public T CallHook<T> ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7 );
        }
        public T CallHook<T> ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8 )
        {
            return ( T )    HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 );
        }
        public T CallHook<T> ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 );
        }

        public object CallHook ( string hook )
        {
            return HookExecutor.CallHook ( this, hook );
        }
        public object CallHook ( string hook, object arg1 )
        {
            return HookExecutor.CallHook ( this, hook, arg1 );
        }
        public object CallHook ( string hook, object arg1, object arg2 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2 );
        }
        public object CallHook ( string hook, object arg1, object arg2, object arg3 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3 );
        }
        public object CallHook ( string hook, object arg1, object arg2, object arg3, object arg4 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4 );
        }
        public object CallHook ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5 );
        }
        public object CallHook ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6 );
        }
        public object CallHook ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7 );
        }
        public object CallHook ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 );
        }
        public object CallHook ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9 )
        {
            return HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 );
        }

        public object CallPublicHook ( string hook )
        {
            return HookExecutor.CallPublicHook ( this, hook );
        }
        public object CallPublicHook ( string hook, object arg1 )
        {
            return HookExecutor.CallPublicHook ( this, hook, arg1 );
        }
        public object CallPublicHook ( string hook, object arg1, object arg2 )
        {
            return HookExecutor.CallPublicHook ( this, hook, arg1, arg2 );
        }
        public object CallPublicHook ( string hook, object arg1, object arg2, object arg3 )
        {
            return HookExecutor.CallPublicHook ( this, hook, arg1, arg2, arg3 );
        }
        public object CallPublicHook ( string hook, object arg1, object arg2, object arg3, object arg4 )
        {
            return HookExecutor.CallPublicHook ( this, hook, arg1, arg2, arg3, arg4 );
        }
        public object CallPublicHook ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5 )
        {
            return HookExecutor.CallPublicHook ( this, hook, arg1, arg2, arg3, arg4, arg5 );
        }
        public object CallPublicHook ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6 )
        {
            return HookExecutor.CallPublicHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6 );
        }
        public object CallPublicHook ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7 )
        {
            return HookExecutor.CallPublicHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7 );
        }
        public object CallPublicHook ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8 )
        {
            return HookExecutor.CallPublicHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 );
        }
        public object CallPublicHook ( string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9 )
        {
            return HookExecutor.CallPublicHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 );
        }

        #endregion

        public bool IsLoaded => true;
    }
}