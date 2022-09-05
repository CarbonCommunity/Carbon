using Facepunch;
using Humanlights.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Carbon.Core.Processors
{
    public class ScriptProcessor : BaseProcessor, IDisposable
    {
        public Dictionary<string, AutoUpdatePlugin> Plugins { get; } = new Dictionary<string, AutoUpdatePlugin> ();
        public List<string> IgnoredPlugins { get; } = new List<string> ();

        internal FileSystemWatcher _folderWatcher { get; set; }
        internal WaitForSeconds _waitSeconds { get; set; } = new WaitForSeconds ( 0.2f );

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
            existent?._loader?.Clear ();

            if ( Plugins.ContainsKey ( id ) ) Plugins.Remove ( id );
        }
        public void Clear ()
        {
            foreach ( var plugin in Plugins )
            {
                try
                {
                    plugin.Value._loader?.Clear ();
                    plugin.Value._loader = null;
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

        public override void Start ()
        {
            base.Start ();

            StopAllCoroutines ();
            StartCoroutine ( CompileCheck () );

            _folderWatcher?.Dispose ();
            _folderWatcher = null;
            _folderWatcher = new FileSystemWatcher ( CarbonCore.GetPluginsFolder () )
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.FileName,
                Filter = "*.cs"
            };
            _folderWatcher.Created += _onCreated;
            _folderWatcher.Changed += _onChanged;
            _folderWatcher.Renamed += _onRenamed;
            _folderWatcher.Deleted += _onRemoved;

            _folderWatcher.IncludeSubdirectories = true;
            _folderWatcher.EnableRaisingEvents = true;

            CarbonCore.Log ( $" Initialized Script Processor" );
        }
        public override void OnDestroy ()
        {
            base.OnDestroy ();

            CarbonCore.Log ( $"{GetType ().Name} has been unloaded." );
        }

        internal void _onCreated ( object sender, FileSystemEventArgs e )
        {
            Plugins.Add ( Path.GetFileNameWithoutExtension ( e.Name ), null );
        }
        internal void _onChanged ( object sender, FileSystemEventArgs e )
        {
            Plugins.TryGetValue ( Path.GetFileNameWithoutExtension ( e.Name ), out var mod );
            if ( mod != null ) mod.SetDirty ();
        }
        internal void _onRenamed ( object sender, RenamedEventArgs e )
        {
            Plugins.TryGetValue ( Path.GetFileNameWithoutExtension ( e.OldName ), out var mod );
            if ( mod != null ) mod.MarkDeleted ();

            Plugins.Add ( Path.GetFileNameWithoutExtension ( e.Name ), null );
        }
        internal void _onRemoved ( object sender, FileSystemEventArgs e )
        {
            Plugins.TryGetValue ( Path.GetFileNameWithoutExtension ( e.Name ), out var mod );
            if ( mod != null ) mod.MarkDeleted ();
        }
        public void Clear ( string id, AutoUpdatePlugin plugin )
        {
            plugin?._loader?.Clear ();
            Pool.Free ( ref plugin );
            Remove ( id );
        }
        public void Process ( string id, AutoUpdatePlugin plugin )
        {
            var file = plugin.File;

            Clear ( id, plugin );
            Prepare ( id, file );
        }
        public IEnumerator CompileCheck ()
        {
            var temp = new Dictionary<string, AutoUpdatePlugin> ();

            while ( true )
            {
                yield return _waitSeconds;

                foreach ( var plugin in Plugins ) temp.Add ( plugin.Key, plugin.Value );

                foreach ( var plugin in temp )
                {
                    if ( plugin.Value == null )
                    {
                        Plugins [ plugin.Key ] = AutoUpdatePlugin.Create ();
                        Plugins [ plugin.Key ].File = Path.Combine ( CarbonCore.GetPluginsFolder (), $"{plugin.Key}.cs" );
                        Plugins [ plugin.Key ].Process ();
                        continue;
                    }

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

        public void Dispose ()
        {
            Clear ();
        }

        [Serializable]
        public class AutoUpdatePlugin : IDisposable
        {
            public string File { get; set; }
            public List<string> Files { get; set; }
            public List<FileSystemWatcher> Watchers { get; set; }

            internal bool _hasChanged;
            internal bool _hasRemoved;
            internal ScriptLoader _loader;

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
                var watcher = new FileSystemWatcher ( Path.GetDirectoryName ( path ) )
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
                _hasChanged = true;
            }
            private void OnRemoved ( object sender, FileSystemEventArgs e )
            {
                _hasRemoved = true;
            }

            public void Dispose ()
            {
                _loader?.Clear ();
                _loader = null;
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
                        var path = Path.Combine ( folder, $"{actualPlugin}.cs" );
                        if ( !OsEx.File.Exists ( path ) )
                        {
                            CarbonCore.Warn ( $"Couldn't find required plugin: {actualPlugin} for {plugin}" );
                            return;
                        }

                        AddWatcher ( path );
                    }

                    _loader = new ScriptLoader ();
                    _loader.Files = Files;
                    _loader.Load ( true );
                }
                catch ( Exception ex )
                {
                    CarbonCore.Warn ( $"Failed processing {Path.GetFileNameWithoutExtension ( File )}:\n{ex}" );
                }
            }

            public bool IsDirty => _hasChanged;
            public bool IsRemoved => _hasRemoved;

            public void SetDirty ()
            {
                _hasChanged = true;
            }
            public void MarkDeleted ()
            {
                _hasRemoved = true;
            }
        }
    }
}