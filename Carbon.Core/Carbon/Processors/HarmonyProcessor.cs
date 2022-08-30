using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Facepunch;

namespace Carbon.Core.Processors
{
    public class HarmonyProcessor : FacepunchBehaviour
    {
        private Dictionary<string, AutoUpdatePlugin> Mods { get; } = new Dictionary<string, AutoUpdatePlugin> ();

        public void Prepare ( string file )
        {
            Prepare ( Path.GetFileNameWithoutExtension ( file ), file );
        }
        public void Prepare ( string id, string file )
        {
            DebugEx.Log ( $" Loading Harmony plugin '{id}'..." );

            Remove ( id );

            var plugin = AutoUpdatePlugin.Create ();
            Mods.Add ( id, plugin );

            plugin.File = file;
            plugin.Process ();
        }
        public void Remove ( string id )
        {
            var existent = !Mods.ContainsKey ( id ) ? null : Mods [ id ];
            existent?.Dispose ();

            if ( Mods.ContainsKey ( id ) ) Mods.Remove ( id );
        }
        public void Clear ()
        {
            foreach ( var mod in Mods )
            {
                try
                {
                    mod.Value?.Dispose ();
                }
                catch ( Exception ex ) { CarbonCore.Error ( $" Harmony plugin error for '{mod.Key}':", ex ); }
            }

            Mods.Clear ();
        }

        public void Start ()
        {
            StartCoroutine ( CompileCheck () );
        }
        public void Clear ( string id, AutoUpdatePlugin plugin )
        {
            plugin.Dispose ();
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
                foreach ( var plugin in Mods ) temp.Add ( plugin.Key, plugin.Value );

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
            public FileSystemWatcher Watcher { get; set; }

            internal bool HasChanged;
            internal bool HasRemoved;

            public static AutoUpdatePlugin Create ()
            {
                var plugin = new AutoUpdatePlugin
                {
                    Watcher = new FileSystemWatcher ()
                };
                return plugin;
            }

            public void AddWatcher ( string path )
            {
                Watcher = new FileSystemWatcher ( Path.GetDirectoryName ( path ) ?? throw new InvalidOperationException() )
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.LastAccess,
                    Filter = Path.GetFileName ( path )
                };
                Watcher.Changed += OnChanged;
                Watcher.Created += OnChanged;
                Watcher.Renamed += OnRemoved;
                Watcher.Deleted += OnRemoved;
                Watcher.EnableRaisingEvents = true;

                File = path;
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
                CarbonLoader.UnloadCarbonMod ( Path.GetFileNameWithoutExtension ( File ), true );
                Watcher?.Dispose ();
                Watcher = null;
            }
            public void Process ()
            {
                try
                {
                    Watcher.EnableRaisingEvents = false;
                    Watcher.Dispose ();
                    Watcher = null;

                    AddWatcher ( File );

                    HarmonyLoader.TryLoadMod ( Path.GetFileNameWithoutExtension ( File ) );
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
