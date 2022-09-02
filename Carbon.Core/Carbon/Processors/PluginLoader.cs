using Carbon.Core;
using Facepunch;
using Humanlights.Extensions;
using Humanlights.Unity.Compiler;
using Oxide.Core;
using Oxide.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Carbon.Core
{
    public class PluginLoader : IDisposable
    {
        public static List<Plugin> All { get; } = new List<Plugin> ();

        public List<Plugin> Plugins { get; set; } = new List<Plugin> ();

        public List<string> Files { get; set; } = new List<string> ();
        public List<string> Sources { get; set; } = new List<string> ();
        public List<string> Namespaces { get; set; } = new List<string> ();
        public string Source { get; set; }
        public bool IsCore { get; set; }

        public AsyncPluginLoader AsyncLoader { get; set; } = new AsyncPluginLoader ();

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

        public static void LoadAll ()
        {
            var files = OsEx.Folder.GetFilesWithExtension ( CarbonCore.GetPluginsFolder (), "cs" );

            foreach(var file in files )
            {
                CarbonCore.Instance.PluginProcessor.Prepare ( file );
            }
        }

        public void Clear ()
        {
            AsyncLoader?.Abort ();
            AsyncLoader = null;

            for ( int i = 0; i < Plugins.Count; i++ )
            {
                var plugin = Plugins [ i ];
                if ( plugin.IsCore ) continue;

                All.RemoveAll ( x => x.Name == Plugins [ i ].Name );

                if(plugin.Instance != null) plugin.Instance.CallHook ( "OnUnload" );
                
                if ( plugin.Instance != null ) DebugEx.Log ( $"Unloaded plugin {plugin.Instance.Name} v{plugin.Instance.Version} by {plugin.Instance.Author}" );

                // if ( plugin.Instance != null ) UnityEngine.Object.DestroyImmediate ( plugin.Instance );
                UnityEngine.Object.Destroy ( plugin.GameObject );

                plugin.Dispose ();
                if ( plugin.Instance != null ) Pool.Free ( ref plugin.Instance );
            }

            Plugins.RemoveAll ( x => !x.IsCore );
        }
        protected void GetSources ()
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
        protected void GetNamespaces ()
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
        protected void GetFullSource ()
        {
            Source = StringArrayEx.ToString ( Namespaces.ToArray (), "\n", "\n" ) + "\n\n";

            foreach ( var source in Sources )
            {
                var usingLines = source.Split ( '\n' ).Where ( x => x.Trim ().ToLower ().StartsWith ( "using" ) && x.Trim ().ToLower ().EndsWith ( ";" ) ).ToArray();
                var usingLinesString = usingLines.Length == 0 ? "" : StringArrayEx.ToString ( usingLines, "\n", "\n" );

                var fixedSource = string.IsNullOrEmpty ( usingLinesString ) ? source.Trim () : source.Replace ( usingLinesString, "" ).Trim ();
                Source += fixedSource + "\n\n";
            }
        }

        public IEnumerator Compile ( GameObject target = null )
        {
            OsEx.File.Create ( "test.cs", Source );

            Debug.Log ( $"WAAAAT" );
            if ( string.IsNullOrEmpty ( Source ) )
            {
                CarbonCore.Warn ( "Attempted to compile an empty string of source code." );
                yield break;
            }
            Debug.Log ( $"WAAAAT2" );

            AsyncLoader.Source = Source;
            AsyncLoader.Start ();
            Debug.Log ( $"WAAAAT3" );

            while ( AsyncLoader != null && !AsyncLoader.IsDone ) { yield return null; }

            Debug.Log ( $"WAAAAT4 {AsyncLoader.Assembly == null} {AsyncLoader.Exception}" );

            if ( AsyncLoader == null ) yield break;

            if ( AsyncLoader.Exception != null )
            {
                CarbonCore.Error ( $"Failed compiling '{( Files.Count == 0 ? "<custom source>" : Path.GetFileNameWithoutExtension ( Files [ 0 ] ) )}':", AsyncLoader.Exception );
                yield break;
            }

            Debug.Log ( $"WAAAAT5" );

            try
            {
                var assembly = AsyncLoader.Assembly;
                var pluginIndex = 0;

                Debug.Log ( $"WAAAAT6" );

                foreach ( var type in assembly.GetTypes () )
                {
                    var attributes = ( type.GetCustomAttributes ( typeof ( InfoAttribute ), true ) as InfoAttribute [] );
                    var info = attributes.Length > 0 ? type.GetCustomAttribute ( typeof ( InfoAttribute ), true ) as InfoAttribute : null;
                    var desc = attributes.Length > 0 ? type.GetCustomAttribute ( typeof ( DescriptionAttribute ), true ) as DescriptionAttribute : null;
                    var plugin = Plugin.Create ( Sources [ pluginIndex ], assembly, type );

                    Debug.Log ( $"WAAAAT6 {type.FullName}" );

                    if ( info != null )
                    {
                        plugin.Name = info.Title;
                        plugin.Author = info.Author;
                        plugin.Version = info.Version;
                        plugin.Description = desc?.Description;
                        Debug.Log ( $"WAAAAT7 {plugin.Name} {plugin.Author} {plugin.Version}" );

                        plugin.GameObject = target != null ? target : new GameObject ( $"{info.Title}_{info.Version}" );
                        plugin.Instance = Activator.CreateInstance ( type ) as RustPlugin;
                        plugin.IsCore = IsCore;
                        plugin.Required = Plugins.Count != 0 ? Plugins [ 0 ] : null;
                        plugin.Instance.Init ();
                        plugin.Instance.CallHook ( "OnLoaded" );
                        plugin.Instance.CallHook ( "OnServerInitialized" );
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

            yield break;
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