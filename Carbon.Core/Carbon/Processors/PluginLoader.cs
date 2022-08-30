using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Facepunch;
using Humanlights.Extensions;
using Humanlights.Unity.Compiler;
using Oxide.Core;
using Oxide.Plugins;
using UnityEngine;

namespace Carbon.Core.Processors
{
    public class PluginLoader : IDisposable
    {
        private static List<Plugin> All { get; } = new List<Plugin> ();

        private List<Plugin> Plugins { get; set; } = new List<Plugin> ();

        public List<string> Files { get; set; } = new List<string> ();
        private List<string> Sources { get; set; } = new List<string> ();
        private List<string> Namespaces { get; set; } = new List<string> ();
        private string Source { get; set; }
        private bool IsCore { get; set; }

        private AsyncPluginLoader AsyncLoader { get; set; } = new AsyncPluginLoader ();

        public void Load ( bool customFiles = false, bool customSources = false, GameObject target = null )
        {
            if ( !customFiles )
            {
                if ( Files.Count == 0 ) return;
            }

            if ( !customSources ) GetSources ();
            GetNamespaces ();
            GetFullSource ();

            OsEx.File.Create ( "master.cs", Source );

            CarbonCore.Instance.PluginProcessor.StartCoroutine ( Compile ( target ) );
        }

        private bool ExecuteMethod ( Assembly assembly, Type type, GameObject target, string method, Plugin plugin, bool skipAttributes )
        {
            var hasInfoAttribute = type.GetCustomAttributes ( true ).Any ( x => x.GetType () == typeof ( InfoAttribute ) );

            if ( !hasInfoAttribute && !skipAttributes )
            {
                DebugEx.Log ( $"The plugin {type.FullName} does not have Info attribute. Fix that by adding [Info(pluginName, authorName, version)] on top of the class." );
                return false;
            }
            else if ( type.BaseType == typeof ( RustPlugin ) )
            {
                try { CompilerManager.RunMethod ( assembly, type.FullName, method, target ); return true; } catch ( Exception exception ) { CarbonCore.Error ( $"Plugin Error:{plugin.Name}.{plugin.Version}", exception ); return false; }
            }
            else
            {
                DebugEx.Log ( $"The plugin {type.Name} is not inherited by {nameof(RustPlugin)}, so it cannot be executed." );
                return false;
            }
        }
        private bool ExecuteMethodNon ( Assembly assembly, Type type, GameObject target, string method )
        {
            try { CompilerManager.RunMethod ( assembly, type.FullName, method, target ); return true; } catch ( Exception exception ) { CarbonCore.Error ( $"Error: {exception}", exception ); return false; }
        }

        public void Clear ()
        {
            AsyncLoader?.Abort ();
            AsyncLoader = null;

            foreach (var plugin in Plugins)
            {
                if ( plugin.IsCore ) continue;

                All.RemoveAll ( x => x.Name == plugin.Name );

                if ( plugin.Instance != null ) plugin.Instance.CallHook ( "OnUnload" );
                if ( plugin.Instance != null ) DebugEx.Log ( $"Unloaded plugin {plugin.Instance.Name} v{plugin.Instance.Version} by {plugin.Instance.Author}" );

                if ( plugin.Instance != null ) UnityEngine.Object.DestroyImmediate ( plugin.Instance );
                UnityEngine.Object.Destroy ( plugin.GameObject );

                plugin.Dispose ();
                if ( plugin.Instance != null ) Pool.Free ( ref plugin.Instance );
            }

            Plugins.RemoveAll ( x => !x.IsCore );
        }

        private void GetSources ()
        {
            Sources.Clear ();

            foreach ( var file in Files )
            {
                if ( !OsEx.File.Exists ( file ) )
                {
                    CarbonCore.Warn ( $"Plugin \"{file}\" does not exist or the path is misspelled." );
                    continue;
                }

                var source = OsEx.File.ReadText ( file );
                Sources.Add ( source.Trim () );
            }
        }

        private void GetNamespaces ()
        {
            Namespaces.Clear ();

            foreach ( var source in Sources )
            {
                var usingLines = source.Split ( '\n' ).Where ( x => x.Trim ().ToLower ().StartsWith ( "using" ) && x.Trim ().ToLower ().EndsWith ( ";" ) );

                foreach ( var usingLine in usingLines )
                {
                    if ( Namespaces.Exists ( x => x == usingLine ) )
                    {
                        continue;
                    }

                    Namespaces.Add ( usingLine );
                }
            }

            Namespaces = Namespaces.OrderBy ( x => x ).ToList ();

            // OsEx.File.Create ( $"{TempFolder}/namespaces.txt", StringArrayEx.ToString ( Namespaces.ToArray (), "\n", "\n" ) );
        }

        private void GetFullSource ()
        {
            Source = Namespaces.ToArray ().ToString ("\n", "\n" ) + "\n\n";

            foreach ( var source in Sources )
            {
                var usingLines = source.Split ( '\n' ).Where ( x => x.Trim ().ToLower ().StartsWith ( "using" ) && x.Trim ().ToLower ().EndsWith ( ";" ) );
                var usingLinesArray = usingLines as string[] ?? usingLines.ToArray();
                var usingLinesString = usingLinesArray.Length == 0 ? "" : usingLinesArray.ToArray ().ToString ("\n", "\n" );

                var fixedSource = string.IsNullOrEmpty ( usingLinesString ) ? source.Trim () : source.Replace ( usingLinesString, "" ).Trim ();
                Source += fixedSource + "\n\n";
            }
        }

        private IEnumerator Compile ( GameObject target = null )
        {
            if ( string.IsNullOrEmpty ( Source ) )
            {
                CarbonCore.Warn ( "Attempted to compile an empty string of source code." );
                yield break;
            }

            AsyncLoader.Source = Source;
            AsyncLoader.Start ();

            while ( AsyncLoader != null && !AsyncLoader.IsDone ) { yield return null; }

            if ( AsyncLoader == null ) yield break;

            if ( AsyncLoader.Exception != null )
            {
                CarbonCore.Error ( $"Failed compiling '{( Files.Count == 0 ? "<custom source>" : Path.GetFileNameWithoutExtension ( Files [ 0 ] ) )}':", AsyncLoader.Exception );
                yield break;
            }

            try
            {
                var assembly = AsyncLoader.Assembly;
                var pluginIndex = 0;

                foreach ( var type in assembly.GetTypes () )
                {
                    var attributes = ( type.GetCustomAttributes ( typeof ( InfoAttribute ), true ) as InfoAttribute [] );
                    var info = attributes != null && attributes.Length > 0 ? type.GetCustomAttribute ( typeof ( InfoAttribute ), true ) as InfoAttribute : null;
                    var desc = attributes != null && attributes.Length > 0 ? type.GetCustomAttribute ( typeof ( DescriptionAttribute ), true ) as DescriptionAttribute : null;
                    var plugin = Plugin.Create ( Sources [ pluginIndex ], assembly, type );

                    if ( info != null )
                    {
                        plugin.Name = info.Title;
                        plugin.Author = info.Author;
                        plugin.Version = info.Version;
                        plugin.Description = desc?.Description;
                        plugin.GameObject = target != null ? target : new GameObject ( $"{info.Title}_{info.Version}" );
                        plugin.Instance = plugin.GameObject.AddComponent ( type ) as RustPlugin;
                        plugin.IsCore = IsCore;
                        plugin.Required = Plugins.Count != 0 ? Plugins [ 0 ] : null;
                        if (plugin.Instance != null)
                        {
                            plugin.Instance.Init();
                            plugin.Instance.CallHook("OnLoaded");
                            plugin.Instance.CallHook("OnServerInitialized");
                        }
                    }
                    else
                    {
                        plugin.GameObject = target != null ? target : new GameObject ( $"Dynamic" );
                    }
                    plugin.Loader = this;

                    if ( info != null )
                    {
                        DebugEx.Log ( $"Loaded plugin {info.Title} v{info.Version} by {info.Author}" );
                        All.Add ( plugin );
                    }
                    Plugins.Add ( plugin );
                }
            }
            catch ( Exception exception )
            {
                CarbonCore.Error ( $"Failed to compile:", exception );
            }
        }

        public void Dispose ()
        {

        }

        [Serializable]
        public class Plugin : IDisposable
        {
            public Assembly Assembly { get; set; }
            public Type Type { get; set; }

            public string Name;
            public string Author;
            public VersionNumber Version;
            public string Description;
            public Plugin Required;
            [TextArea ( 4, 15 )] public string Source;
            public PluginLoader Loader;
            public GameObject GameObject;
            public RustPlugin Instance;
            public bool IsCore;

            public static Plugin Create ( string source, Assembly assembly, Type type )
            {
                return new Plugin
                {
                    Source = source,
                    Assembly = assembly,
                    Type = type,

                    Name = null,
                    Author = null,
                    Version = new VersionNumber ( 1, 0, 0 ),
                    Description = null,
                };
            }

            public bool Execute ( string method, GameObject target, bool skipAttributes, PluginLoader manager, bool non = false )
            {
                return non ? manager.ExecuteMethodNon ( Assembly, Type, target, method ) : manager.ExecuteMethod ( Assembly, Type, target, method, this, skipAttributes );
            }

            public void Dispose ()
            {
                Assembly = null;
                Type = null;
            }

            public override string ToString ()
            {
                return $"{Name} v{Version}";
            }
        }
    }
}