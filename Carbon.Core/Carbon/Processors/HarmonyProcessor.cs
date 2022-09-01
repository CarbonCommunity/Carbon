using Carbon.Core;
using Facepunch;
using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Carbon.Core
{
    public class HarmonyProcessor : FacepunchBehaviour, IDisposable
    {
        public Dictionary<string, AutoUpdatePlugin> Mods { get; } = new Dictionary<string, AutoUpdatePlugin> ();

        internal FileSystemWatcher _folderWatcher { get; set; }
        internal WaitForSeconds _waitSeconds { get; set; } = new WaitForSeconds ( 0.2f );

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

            _folderWatcher = new FileSystemWatcher ( CarbonCore.GetPluginsFolder () )
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.FileName,
                Filter = "*.dll"
            };
            _folderWatcher.Created += _onCreated;
            _folderWatcher.Changed += _onChanged;
            _folderWatcher.Renamed += _onRenamed;
            _folderWatcher.Deleted += _onRemoved;

            _folderWatcher.IncludeSubdirectories = true;
            _folderWatcher.EnableRaisingEvents = true;

            CarbonCore.Log ( $" Initialized Harmony Processor" );
        }

        internal void _onCreated ( object sender, FileSystemEventArgs e )
        {
            Mods.Add ( Path.GetFileNameWithoutExtension ( e.Name ), null );
        }
        internal void _onChanged ( object sender, FileSystemEventArgs e )
        {
            Mods.TryGetValue ( Path.GetFileNameWithoutExtension ( e.Name ), out var mod );
            if ( mod != null ) mod.SetDirty ();
        }
        internal void _onRenamed ( object sender, RenamedEventArgs e )
        {
            Mods.TryGetValue ( Path.GetFileNameWithoutExtension ( e.OldName ), out var mod );
            if ( mod != null ) mod.MarkDeleted ();

            Mods.Add ( Path.GetFileNameWithoutExtension ( e.Name ), null );
        }
        internal void _onRemoved ( object sender, FileSystemEventArgs e )
        {
            Mods.TryGetValue ( Path.GetFileNameWithoutExtension ( e.Name ), out var mod );
            if ( mod != null ) mod.MarkDeleted ();
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
        public IEnumerator CompileCheck ()
        {
            var temp = new Dictionary<string, AutoUpdatePlugin> ();

            while ( true )
            {
                foreach ( var plugin in Mods ) temp.Add ( plugin.Key, plugin.Value );

                foreach ( var plugin in temp )
                {
                    if ( plugin.Value == null )
                    {
                        CarbonCore.Log ( $"New file: {plugin.Key}" );
                        var p = AutoUpdatePlugin.Create ();
                        p.File = Path.Combine ( CarbonCore.GetPluginsFolder (), $"{plugin.Key}.dll" );
                        p.Process ();
                        Mods [ plugin.Key ] = p;
                        continue;
                    }

                    if ( plugin.Value.IsRemoved )
                    {
                        CarbonCore.Log ( $"{plugin.Key} is deleted" );
                        Clear ( plugin.Key, plugin.Value );
                        yield return null;
                        break;
                    }

                    if ( plugin.Value.IsDirty )
                    {
                        CarbonCore.Log ( $"{plugin.Key} is dirty" );
                        Process ( plugin.Key, plugin.Value );
                        yield return null;
                    }
                }

                temp.Clear ();

                yield return _waitSeconds;
            }
        }

        public void Dispose ()
        {
            Clear ();
            _folderWatcher?.Dispose ();
            _folderWatcher = null;
        }

        [Serializable]
        public class AutoUpdatePlugin : IDisposable
        {
            public string File { get; set; }

            internal bool _hasChanged;
            internal bool _hasRemoved;

            public static AutoUpdatePlugin Create ()
            {
                return new AutoUpdatePlugin ();
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
                CarbonLoader.UnloadCarbonMod ( Path.GetFileNameWithoutExtension ( File ), true );
            }
            public void Process ()
            {
                try
                {
                    CarbonLoader.LoadCarbonMod ( File, true );
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