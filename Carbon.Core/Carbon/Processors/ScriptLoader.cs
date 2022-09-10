using Carbon.Core.Processors;
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
    public class ScriptLoader : IDisposable
    {
        public List<Script> Scripts { get; set; } = new List<Script> ();

        public List<string> Files { get; set; } = new List<string> ();
        public List<string> Sources { get; set; } = new List<string> ();
        public List<string> Namespaces { get; set; } = new List<string> ();
        public string Source { get; set; }
        public bool IsCore { get; set; }

        public BaseProcessor.Parser Parser { get; set; }

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

            if ( Parser != null )
            {
                Parser.Process ( Source, out var newSource );

                if ( !string.IsNullOrEmpty ( newSource ) )
                {
                    Source = newSource;
                }
            }

            CarbonCore.Instance.ScriptProcessor.StartCoroutine ( Compile ( target ) );
        }

        private bool ExecuteMethod ( Assembly assembly, Type type, GameObject target, string method, Script plugin, bool skipAttributes )
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
                DebugEx.Log ( $"The plugin {type.Name} is not inherited by {nameof ( RustPlugin )}, so it cannot be executed." );
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

            CarbonCore.Instance.ScriptProcessor.Clear ();
            CarbonCore.Instance.ScriptProcessor.IgnoreList.Clear ();

            foreach ( var file in files )
            {
                var plugin = new ScriptProcessor.Script ();
                plugin.File = file;
                CarbonCore.Instance.ScriptProcessor.InstanceBuffer.Add ( Path.GetFileNameWithoutExtension ( file ), plugin );
            }

            foreach ( var plugin in CarbonCore.Instance.ScriptProcessor.InstanceBuffer )
            {
                plugin.Value.SetDirty ();
            }
        }

        public void Clear ()
        {
            AsyncLoader?.Abort ();
            AsyncLoader = null;

            for ( int i = 0; i < Scripts.Count; i++ )
            {
                var plugin = Scripts [ i ];
                if ( plugin.IsCore ) continue;

                CarbonCore.Instance.Plugins.Plugins.Remove ( plugin.Instance );

                if ( plugin.Instance != null )
                {
                    try
                    {
                        HookExecutor.CallStaticHook ( "OnPluginUnloaded", plugin.Instance );
                        plugin.Instance.Unload ();
                        CarbonLoader.RemoveCommands ( plugin.Instance );
                        plugin.Instance.Dispose ();
                        CarbonCore.Log ( $"Unloaded plugin {plugin.Instance}" );
                    }
                    catch ( Exception ex ) { CarbonCore.Error ( $"Failed unloading '{plugin.Instance}'", ex ); }
                }

                plugin.Dispose ();
            }

            if ( Scripts.Count > 0 )
            {
                Scripts.RemoveAll ( x => !x.IsCore );
            }
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
                var usingLines = source.Split ( '\n' ).Where ( x => x.Trim ().ToLower ().StartsWith ( "using" ) && x.Trim ().ToLower ().EndsWith ( ";" ) ).ToArray ();
                var usingLinesString = usingLines.Length == 0 ? "" : StringArrayEx.ToString ( usingLines, "\n", "\n" );

                var fixedSource = string.IsNullOrEmpty ( usingLinesString ) ? source.Trim () : source.Replace ( usingLinesString, "" ).Trim ();
                Source += fixedSource + "\n\n";
            }
        }

        public IEnumerator Compile ( GameObject target = null )
        {
            if ( string.IsNullOrEmpty ( Source ) )
            {
                CarbonCore.Warn ( "Attempted to compile an empty string of source code." );
                yield break;
            }

            var lines = Source.Split ( '\n' );
            var resultReferences = Pool.GetList<string> ();
            foreach ( var reference in lines )
            {
                try
                {
                    if ( reference.StartsWith ( "// Reference:" ) || reference.StartsWith ( "//Reference:" ) )
                    {

                        var @ref = $"{reference.Replace ( "// Reference:", "" ).Replace ( "//Reference:", "" )}.dll".Trim ();
                        resultReferences.Add ( @ref );
                        CarbonCore.Log ( $" Added reference: {@ref}" );
                    }
                }
                catch { }
            }

            var resultRequires = Pool.GetList<string> ();
            foreach ( var require in lines )
            {
                try
                {
                    if ( require.StartsWith ( "// Requires:" ) || require.StartsWith ( "//Requires:" ) )
                    {

                        var @ref = $"{require.Replace ( "// Requires:", "" ).Replace ( "//Requires:", "" )}".Trim ();
                        resultRequires.Add ( @ref );
                        CarbonCore.Log ( $" Added required plugin: {@ref}" );
                    }
                }
                catch { }
            }

            Pool.Free ( ref lines );
            AsyncLoader.FilePath = Files [ 0 ];
            AsyncLoader.Source = Source;
            AsyncLoader.References = resultReferences?.ToArray ();
            AsyncLoader.Requires = resultRequires?.ToArray ();
            Pool.FreeList ( ref resultReferences );
            Pool.FreeList ( ref resultRequires );

            var requires = Pool.GetList<Plugin> ();
            var noRequiresFound = false;
            foreach ( var require in AsyncLoader.Requires )
            {
                var plugin = CarbonCore.Instance.CorePlugin.plugins.Find ( require );
                if ( plugin == null )
                {
                    CarbonCore.Warn ( $"Couldn't find required plugin '{require}' for '{Path.GetFileNameWithoutExtension ( Files [ 0 ] )}'" );
                    noRequiresFound = true;
                }
                else requires.Add ( plugin );
            }

            if ( noRequiresFound )
            {
                Pool.FreeList ( ref requires );
                yield break;
            }

            AsyncLoader.Start ();

            while ( AsyncLoader != null && !AsyncLoader.IsDone ) { yield return null; }

            if ( AsyncLoader == null ) yield break;

            if ( AsyncLoader.Exceptions.Count != 0 )
            {
                CarbonCore.Error ( $"Failed compiling '{AsyncLoader.FilePath}' after {AsyncLoader.Retries} retries:" );
                for ( int i = 0; i < AsyncLoader.Exceptions.Count; i++ )
                {
                    var error = AsyncLoader.Exceptions [ i ];
                    CarbonCore.Error ( $"  {i + 1:n0}. {error.Error.ErrorText}\n     ({error.Error.FileName} {error.Error.Column} line {error.Error.Line})" );
                }
                yield break;
            }

            try
            {
                CarbonLoader.AssemblyCache.Add ( AsyncLoader.Assembly );

                var assembly = AsyncLoader.Assembly;
                var pluginIndex = 0;

                foreach ( var type in assembly.GetTypes () )
                {
                    if ( !( type.Namespace.Equals ( "Oxide.Plugins" ) ||
                        type.Namespace.Equals ( "Carbon.Plugins" ) ) ) continue;

                    var info = type.GetCustomAttribute ( typeof ( InfoAttribute ), true ) as InfoAttribute;
                    var description = type.GetCustomAttribute ( typeof ( DescriptionAttribute ), true ) as DescriptionAttribute;
                    var plugin = Script.Create ( Sources [ pluginIndex ], assembly, type );

                    if ( info == null )
                    {
                        CarbonCore.Warn ( $"Failed loading '{type.Name}'. The plugin doesn't have the Info attribute." );
                        continue;
                    }

                    plugin.Name = info.Title;
                    plugin.Author = info.Author;
                    plugin.Version = info.Version;
                    plugin.Description = description?.Description;

                    plugin.Instance = Activator.CreateInstance ( type ) as RustPlugin;
                    plugin.IsCore = IsCore;
                    plugin.Instance.Requires = requires.ToArray ();
                    plugin.Instance.SetProcessor ( CarbonCore.Instance.ScriptProcessor );

                    plugin.Instance.CallHook ( "SetupMod", null, info.Title, info.Author, info.Version, plugin.Description );
                    HookExecutor.CallStaticHook ( "OnPluginLoaded", plugin );
                    plugin.Instance.Init ();
                    try { plugin.Instance.DoLoadConfig (); }
                    catch ( Exception loadException )
                    {
                        plugin.Instance.LogError ( $"Failed loading config.", loadException );
                    }

                    if ( CarbonCore.IsServerFullyInitialized )
                    {
                        try { plugin.Instance.CallHook ( "OnServerInitialized" ); }
                        catch ( Exception initException )
                        {
                            plugin.Instance.LogError ( $"Failed OnServerInitialized.", initException );
                        }
                    }

                    plugin.Loader = this;

                    CarbonLoader.AppendAssembly ( plugin.Name, AsyncLoader.Assembly );
                    CarbonCore.Instance.Plugins.Plugins.Add ( plugin.Instance );

                    if ( info != null )
                    {
                        DebugEx.Log ( $"Loaded plugin {info.Title} v{info.Version} by {info.Author}" );
                    }

                    CarbonLoader.ProcessCommands ( type, plugin.Instance );

                    Scripts.Add ( plugin );
                }
            }
            catch ( Exception exception )
            {
                CarbonCore.Error ( $"Failed to compile: ", exception );
            }

            Pool.FreeList ( ref requires );

            yield break;
        }

        public void Dispose ()
        {

        }

        [Serializable]
        public class Script : IDisposable
        {
            public Assembly Assembly { get; set; }
            public Type Type { get; set; }

            public string Name;
            public string Author;
            public VersionNumber Version;
            public string Description;
            public string Source;
            public ScriptLoader Loader;
            public RustPlugin Instance;
            public bool IsCore;

            public static Script Create ( string source, Assembly assembly, Type type )
            {
                return new Script
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

            public bool Execute ( string method, GameObject target, bool skipAttributes, ScriptLoader manager, bool non = false )
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