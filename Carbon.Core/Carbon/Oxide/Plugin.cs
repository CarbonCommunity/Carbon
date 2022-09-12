using System;
using Oxide.Core;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using Carbon.Core;
using Newtonsoft.Json;
using Harmony;
using System.Linq;
using Carbon.Core.Processors;

namespace Oxide.Plugins
{
    [JsonObject ( MemberSerialization.OptIn )]
    public class Plugin : IDisposable
    {
        public Dictionary<string, MethodInfo> HookCache { get; private set; } = new Dictionary<string, MethodInfo> ();
        public Dictionary<string, MethodInfo> HookMethodAttributeCache { get; private set; } = new Dictionary<string, MethodInfo> ();
        public List<string> IgnoredHooks { get; private set; } = new List<string> ();

        public bool IsCorePlugin { get; set; }
        public Type Type { get; set; }

        [JsonProperty]
        public string Title { get; set; } = "Rust";
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Description { get; set; }
        [JsonProperty]
        public string Author { get; set; }
        [JsonProperty]
        public VersionNumber Version { get; set; }
        public int ResourceId { get; set; }
        public bool HasConfig { get; set; }
        public bool HasMessages { get; set; }
        public double TotalHookTime { get; internal set; }

        public CarbonLoader.CarbonMod carbon { get; set; }

        public Plugin [] Requires { get; internal set; }

        internal Stopwatch _trackStopwatch = new Stopwatch ();
        internal BaseProcessor _processor;

        public HarmonyInstance Harmony;

        public static implicit operator bool ( Plugin other )
        {
            return other != null;
        }

        public void TrackStart ()
        {
            if ( !CarbonCore.IsServerFullyInitialized || IsCorePlugin )
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
        public void TrackEnd ()
        {
            if ( !CarbonCore.IsServerFullyInitialized || IsCorePlugin )
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

        public virtual void IInit ()
        {
            foreach ( var method in GetType ().GetMethods ( BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public ) )
            {
                var attribute = method.GetCustomAttribute<HookMethodAttribute> ();
                if ( attribute == null ) continue;

                var name = string.IsNullOrEmpty ( attribute.Name ) ? method.Name : attribute.Name;
                HookMethodAttributeCache.Add ( name + method.GetParameters ().Length, method );
            }

            foreach ( var field in GetType ().GetFields ( BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public ) )
            {
                var attribute = field.GetCustomAttribute<PluginReferenceAttribute> ();
                if ( attribute == null ) continue;

                var plugin = CarbonCore.Instance.CorePlugin.plugins.Find ( field.Name );
                if ( plugin != null ) field.SetValue ( this, plugin );
            }
        }
        public virtual void ILoad ()
        {
            IsLoaded = true;
            CallHook ( "OnLoaded" );
        }
        public virtual void IUnload ()
        {
            foreach ( var plugin in CarbonCore.Instance.CorePlugin.plugins.GetAll () )
            {
                if ( plugin == null || plugin.Requires == null || plugin.Requires.Length == 0 ) continue;

                if ( plugin.Requires.Contains ( this ) )
                {
                    plugin.InternalUnload ();
                }
            }

            IgnoredHooks.Clear ();
            HookCache.Clear ();
            HookMethodAttributeCache.Clear ();

            IgnoredHooks = null;
            HookCache = null;
            HookMethodAttributeCache = null;
        }
        internal void InternalUnload ()
        {
            switch ( _processor )
            {
                case ScriptProcessor script:
                    _processor.Clear ( Name, script.InstanceBuffer [ Name ] );
                    break;
            }
        }

        public void PatchPlugin ( Assembly assembly = null )
        {
            if ( assembly == null ) assembly = Assembly.GetExecutingAssembly ();

            Harmony = null;
            Harmony = HarmonyInstance.Create ( Name + "Patches" );
            Harmony.PatchAll ( assembly );
        }
        public void UnpatchPlugin ()
        {
            Harmony?.UnpatchAll ( Harmony.Id );
            Harmony = null;
        }

        public void SetProcessor ( BaseProcessor processor )
        {
            _processor = processor;
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
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 );
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
            return ( T )HookExecutor.CallHook ( this, hook );
        }
        public T CallHook<T> ( string hook, object arg1 )
        {
            return ( T )HookExecutor.CallHook ( this, hook, arg1 );
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
            return ( T )HookExecutor.CallHook ( this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 );
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

        #endregion

        public void NextTick ( Action action )
        {
            action?.Invoke ();
        }

        public bool IsLoaded { get; set; }

        public virtual void Dispose ()
        {
            IsLoaded = false;
        }
    }
}