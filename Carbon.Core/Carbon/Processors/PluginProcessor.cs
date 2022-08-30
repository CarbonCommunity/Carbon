using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Facepunch;
using Humanlights.Extensions;

namespace Carbon.Core.Processors
{
    public class PluginProcessor : FacepunchBehaviour
    {
        private Dictionary<string, AutoUpdatePlugin> Plugins { get; } = new Dictionary<string, AutoUpdatePlugin> ();
        public List<string> IgnoredPlugins { get; } = new List<string> ();

        public void Prepare ( string file )
        {
            Prepare ( Path.GetFileNameWithoutExtension ( file ), file );
        }
        public void Prepare ( string id, string file )
        {
            if ( IgnoredPlugins.Contains ( file ) ) return;

            DebugEx.Log ( $" Loading plugin '{id}'..." );

            Remove ( id );

            var plugin = AutoUpdatePlugin.Create ();
            Plugins.Add ( id, plugin );

            plugin.File = file;
            plugin.Process ();
        }
        public void Remove ( string id )
        {
            var existent = !Plugins.ContainsKey ( id ) ? null : Plugins [ id ];
            existent?.Loader?.Clear ();

            if ( Plugins.ContainsKey ( id ) ) Plugins.Remove ( id );
        }
        public void Clear ()
        {
            foreach ( var plugin in Plugins )
            {
                try
                {
                    plugin.Value.Loader?.Clear ();
                    plugin.Value.Loader = null;
                }
                catch ( Exception ex ) { CarbonCore.Error ( $" Plugin error for '{plugin.Key}':", ex ); }
            }

            Plugins.Clear ();
        }
        public void Ignore ( string file )
        {
            IgnoredPlugins.Add ( file );
        }

        public void ClearIgnore ( string file )
        {
            IgnoredPlugins.RemoveAll ( x => x == file );      
        }

        public void Start ()
        {
            StartCoroutine ( CompileCheck () );
        }
        public void Clear ( string id, AutoUpdatePlugin plugin )
        {
            plugin?.Loader?.Clear ();
            Pool.Free ( ref plugin );
            Remove ( id );
        }
        public void Process ( string id, AutoUpdatePlugin plugin )
        {
            var file = plugin.File;

            Clear ( id, plugin );
            Prepare ( id, file );
        }

        private IEnumerator CompileCheck ()
        {
            var temp = new Dictionary<string, AutoUpdatePlugin> ();

            while ( true )
            {
                foreach ( var plugin in Plugins ) temp.Add ( plugin.Key, plugin.Value );

                foreach ( var plugin in temp )
                {
                    if ( plugin.Value.IsRemoved )
                    {
                        Clear ( plugin.Key, plugin.Value );
                        yield return null;
                        break;
                    }

                    if ( plugin.Value.IsDirty )
                    {
                        Process ( plugin.Key, plugin.Value );
                        yield return null;
                    }

                }

                temp.Clear ();

                yield return null;
            }
        }

        [Serializable]
        public class AutoUpdatePlugin : IDisposable
        {
            public string File { get; set; }
            public List<string> Files { get; set; }
            public List<FileSystemWatcher> Watchers { get; set; }

            internal bool HasChanged;
            internal bool HasRemoved;
            internal PluginLoader Loader;

            public static AutoUpdatePlugin Create ()
            {
                var plugin = new AutoUpdatePlugin
                {
                    Watchers = new List<FileSystemWatcher> (),
                    Files = new List<string> (),
                };
                return plugin;
            }

            public void AddWatcher ( string path )
            {
                var watcher = new FileSystemWatcher ( Path.GetDirectoryName ( path ) ?? throw new InvalidOperationException() )
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.LastAccess,
                    Filter = Path.GetFileName ( path )
                };
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Renamed += OnRemoved;
                watcher.Deleted += OnRemoved;
                watcher.EnableRaisingEvents = true;
                Watchers.Add ( watcher );

                Files.Add ( path );
            }

            private void OnChanged ( object sender, FileSystemEventArgs e )
            {
                HasChanged = true;
            }
            private void OnRemoved ( object sender, FileSystemEventArgs e )
            {
                HasRemoved = true;
            }

            public void Dispose ()
            {
                Loader?.Clear ();
                Loader = null;
            }
            public void Process ()
            {
                try
                {
                    for ( int i = 0; i < Watchers.Count; i++ )
                    {
                        Watchers [ i ].EnableRaisingEvents = false;
                        Watchers [ i ].Dispose ();
                        Watchers [ i ] = null;
                    }
                    Watchers.Clear ();
                    Files.Clear ();
                    AddWatcher ( File );

                    var folder = Path.GetDirectoryName ( File );
                    var plugin = Path.GetFileNameWithoutExtension ( File );
                    var requiredPlugins = OsEx.File.ReadTextLines ( File ).Where ( x => x.Contains ( "// Required:" ) );
                    foreach ( var requiredPlugin in requiredPlugins )
                    {
                        var actualPlugin = requiredPlugin.Replace ( "// Required:", "" ).Trim ();
                        if (folder == null) continue;
                        var path = Path.Combine ( folder, $"{actualPlugin}.cs" );
                        if ( !OsEx.File.Exists ( path ) )
                        {
                            CarbonCore.Warn ( $"Couldn't find required plugin: {actualPlugin} for {plugin}" );
                            return;
                        }

                        AddWatcher ( path );
                    }

                    Loader = new PluginLoader ();
                    Loader.Files = Files;
                    Loader.Load ( true );
                }
                catch ( Exception ex )
                {
                    CarbonCore.Warn ( $"Failed processing {Path.GetFileNameWithoutExtension ( File )}:\n{ex}" );
                }
            }

            public bool IsDirty => HasChanged;
            public bool IsRemoved => HasRemoved;
        }
    }
}
