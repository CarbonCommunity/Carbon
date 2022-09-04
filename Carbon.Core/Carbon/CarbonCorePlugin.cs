using Facepunch;
using Humanlights.Components;
using Humanlights.Extensions;
using Oxide.Plugins;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace Carbon.Core
{
    public class CarbonCorePlugin : RustPlugin
    {
        public static Dictionary<string, string> OrderedFiles { get; } = new Dictionary<string, string> ();

        public static void RefreshOrderedFiles ()
        {
            OrderedFiles.Clear ();

            foreach ( var file in OsEx.Folder.GetFilesWithExtension ( CarbonCore.GetPluginsFolder (), "cs" ) )
            {
                OrderedFiles.Add ( Path.GetFileNameWithoutExtension ( file ), file );
            }
        }

        public static string GetPluginPath ( string shortName )
        {
            foreach ( var file in OrderedFiles )
            {
                if ( file.Key == shortName ) return file.Value;
            }

            return null;
        }

        private void OnPluginLoaded ( Plugin plugin )
        {
            CarbonCore.Instance.RefreshConsoleInfo ();
        }
        private void OnPluginUnloaded ( Plugin plugin )
        {
            CarbonCore.Instance.RefreshConsoleInfo ();
        }

        internal static void Reply ( object message, ConsoleSystem.Arg arg )
        {
            if ( arg != null && arg.Player () != null )
            {
                arg.Player ().SendConsoleCommand ( $"echo {message}" );
                return;
            }
            CarbonCore.Log ( message );
        }

        [ConsoleCommand ( "version" )]
        private void GetVersion ( ConsoleSystem.Arg arg )
        {
            if ( arg.Player () != null && !arg.Player ().IsAdmin ) return;

            Reply ( $"Carbon v{CarbonCore.Version}", arg );
        }

        [ConsoleCommand ( "find", false )]
        private void Find ( ConsoleSystem.Arg arg )
        {
            var body = new StringBody ();
            var filter = arg.Args != null && arg.Args.Length > 0 ? arg.Args [ 0 ] : null;
            body.Add ( $"Commands:" );

            foreach ( var command in CarbonCore.Instance.AllConsoleCommands )
            {
                if ( !string.IsNullOrEmpty ( filter ) && !command.Command.Contains ( filter ) ) continue;

                body.Add ( $" {command.Command}(   )" );
            }

            Reply ( body.ToNewLine (), arg );
        }

        [ConsoleCommand ( "loadcs" )]
        private void LoadCsPlugin ( ConsoleSystem.Arg arg )
        {
            RefreshOrderedFiles ();

            var name = arg.Args [ 0 ];

            switch ( name )
            {
                case "*":
                    var tempList = Pool.GetList<string> ();
                    tempList.AddRange ( CarbonCore.Instance.PluginProcessor.IgnoredPlugins );
                    CarbonCore.Instance.PluginProcessor.IgnoredPlugins.Clear ();

                    foreach ( var plugin in tempList )
                    {
                        CarbonCore.Instance.PluginProcessor.Prepare ( plugin, plugin );
                    }
                    Pool.FreeList ( ref tempList );
                    break;

                default:
                    var path = GetPluginPath ( name );
                    if ( string.IsNullOrEmpty ( path ) )
                    {
                        CarbonCore.Warn ( $" Couldn't find plugin with name '{name}'" );
                        return;
                    }

                    var source = OsEx.File.ReadText ( path );
                    var compiler = new CSharpCompiler.CodeCompiler ();
                    var options = new CompilerParameters ()
                    {
                        GenerateInMemory = true,
                        TreatWarningsAsErrors = false,
                        GenerateExecutable = false
                    };
                    var references = new string [] { "System.dll", "mscorlib.dll" };
                    options.ReferencedAssemblies.AddRange ( references );

                    var assemblies = AppDomain.CurrentDomain.GetAssemblies ();
                    var lastCarbon = ( Assembly )null;
                    foreach ( var assembly in assemblies )
                    {
                        if ( CarbonLoader.AssemblyCache.Any ( x => x == assembly ) ) continue;

                        if ( !assembly.FullName.StartsWith ( "Carbon" ) )
                        {
                            if ( assembly.ManifestModule is ModuleBuilder builder )
                            {
                                if ( !builder.IsTransient () )
                                {
                                    options.ReferencedAssemblies.Add ( assembly.GetName ().Name );
                                }
                            }
                            else
                            {
                                options.ReferencedAssemblies.Add ( assembly.GetName ().Name );
                            }
                        }
                        else if ( assembly.FullName.StartsWith ( "Carbon" ) )
                        {
                            lastCarbon = assembly;
                        }
                    }

                    if ( lastCarbon != null )
                    {
                        options.ReferencedAssemblies.Add ( lastCarbon.GetName ().Name );
                        CarbonCore.Log ( $"  Injected {lastCarbon.GetName ().Name}" );
                    }

                    var result = compiler.CompileAssemblyFromSource ( options, source );
                    foreach ( CompilerError error in result.Errors )
                    {
                        CarbonCore.Error ( $"Eeeh: {error.ErrorText}" );
                    }
                    foreach ( var type in result.CompiledAssembly.GetTypes () )
                    {
                        CarbonCore.Log ( $"{type?.FullName}" );
                    }

                    return;
                    CarbonCore.Instance.PluginProcessor.ClearIgnore ( path );
                    CarbonCore.Instance.PluginProcessor.Prepare ( path );
                    break;
            }
        }

        #region Mod & Plugin Loading

        [ConsoleCommand ( "list" )]
        private void GetList ( ConsoleSystem.Arg arg )
        {
            if ( arg.Player () != null && !arg.Player ().IsAdmin || arg.HasArgs ( 1 ) ) return;

            var body = new StringTable ( "#", "Mod", "Author", "Version" );
            var count = 1;

            Reply ( $"Found: {CarbonLoader._loadedMods.Count} mods  with {CarbonLoader._loadedMods.Sum ( x => x.Plugins.Count )} plugins", arg );

            foreach ( var mod in CarbonLoader._loadedMods )
            {
                body.AddRow ( $"{count:n0}", mod.Name, "", "" );

                foreach ( var plugin in mod.Plugins )
                {
                    body.AddRow ( $"", plugin.Name, plugin.Author, $"v{plugin.Version}" );
                }

                count++;
            }

            Reply ( body.ToStringMinimal (), arg );
        }

        [ConsoleCommand ( "reload" )]
        private void Reload ( ConsoleSystem.Arg arg )
        {
            if ( arg.Player () != null && !arg.Player ().IsAdmin || arg.HasArgs ( 1 ) ) return;

            RefreshOrderedFiles ();

            var name = arg.Args [ 0 ];
            switch ( name )
            {
                case "*":
                    CarbonCore.ClearPlugins ();
                    CarbonCore.ReloadPlugins ();
                    break;

                default:
                    var path = GetPluginPath ( name );
                    if ( string.IsNullOrEmpty ( path ) )
                    {
                        DebugEx.Warning ( $" Couldn't find plugin or mod with name '{name}'" );
                        return;
                    }
                    CarbonCore.Instance.HarmonyProcessor.Prepare ( name, path );
                    CarbonCore.Instance.PluginProcessor.Prepare ( name, path );
                    break;
            }
        }

        [ConsoleCommand ( "load" )]
        private void Load ( ConsoleSystem.Arg arg )
        {
            if ( arg.Player () != null && !arg.Player ().IsAdmin || arg.HasArgs ( 1 ) ) return;

            RefreshOrderedFiles ();

            var name = arg.Args [ 0 ];
            switch ( name )
            {
                case "*":
                    //
                    // Mods
                    //
                    {
                        var tempList = Pool.GetList<string> ();
                        tempList.AddRange ( CarbonCore.Instance.HarmonyProcessor.IgnoredPlugins );
                        CarbonCore.Instance.HarmonyProcessor.IgnoredPlugins.Clear ();

                        foreach ( var plugin in tempList )
                        {
                            CarbonCore.Instance.HarmonyProcessor.Prepare ( plugin, plugin );
                        }
                        Pool.FreeList ( ref tempList );
                    }

                    //
                    // Plugins
                    //
                    {
                        var tempList = Pool.GetList<string> ();
                        tempList.AddRange ( CarbonCore.Instance.PluginProcessor.IgnoredPlugins );
                        CarbonCore.Instance.PluginProcessor.IgnoredPlugins.Clear ();

                        foreach ( var plugin in tempList )
                        {
                            CarbonCore.Instance.PluginProcessor.Prepare ( plugin, plugin );
                        }
                        Pool.FreeList ( ref tempList );
                    }
                    break;

                default:
                    var path = GetPluginPath ( name );
                    if ( string.IsNullOrEmpty ( path ) )
                    {
                        DebugEx.Warning ( $" Couldn't find plugin with name '{name}'" );
                        return;
                    }

                    //
                    // Mods
                    //
                    {
                        CarbonCore.Instance.HarmonyProcessor.ClearIgnore ( path );
                        CarbonCore.Instance.HarmonyProcessor.Prepare ( path );
                    }

                    //
                    // Plugins
                    //
                    {
                        CarbonCore.Instance.PluginProcessor.ClearIgnore ( path );
                        CarbonCore.Instance.PluginProcessor.Prepare ( path );
                    }
                    break;
            }
        }

        [ConsoleCommand ( "unload" )]
        private void Unload ( ConsoleSystem.Arg arg )
        {
            if ( arg.Player () != null && !arg.Player ().IsAdmin || arg.HasArgs ( 1 ) ) return;

            RefreshOrderedFiles ();

            var name = arg.Args [ 0 ];
            switch ( name )
            {
                case "*":
                    //
                    // Mods
                    //
                    {
                        foreach ( var plugin in CarbonCore.Instance.PluginProcessor.Plugins )
                        {
                            CarbonCore.Instance.PluginProcessor.Ignore ( plugin.Value.File );
                        }
                        CarbonCore.Instance.PluginProcessor.Clear ();
                    }

                    //
                    // Plugins
                    //
                    {
                        var tempList = Pool.GetList<string> ();
                        tempList.AddRange ( CarbonCore.Instance.PluginProcessor.IgnoredPlugins );
                        CarbonCore.Instance.PluginProcessor.IgnoredPlugins.Clear ();

                        foreach ( var plugin in tempList )
                        {
                            CarbonCore.Instance.PluginProcessor.Prepare ( plugin, plugin );
                        }
                        Pool.FreeList ( ref tempList );
                    }
                    break;

                default:
                    var path = GetPluginPath ( name );
                    if ( string.IsNullOrEmpty ( path ) )
                    {
                        DebugEx.Warning ( $" Couldn't find plugin with name '{name}'" );
                        return;
                    }

                    //
                    // Mods
                    //
                    {
                        CarbonCore.Instance.HarmonyProcessor.ClearIgnore ( path );
                        CarbonCore.Instance.HarmonyProcessor.Prepare ( path );
                    }

                    //
                    // Plugins
                    //
                    {
                        CarbonCore.Instance.PluginProcessor.ClearIgnore ( path );
                        CarbonCore.Instance.PluginProcessor.Prepare ( path );
                    }
                    break;
            }
        }

        #endregion
    }
}