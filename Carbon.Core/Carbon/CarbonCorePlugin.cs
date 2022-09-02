using Facepunch;
using Humanlights.Components;
using Humanlights.Extensions;
using Humanlights.Unity.Compiler;
using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        [ConsoleCommand ( "list" )]
        private void GetList ( ConsoleSystem.Arg arg )
        {
            if ( arg.Player () != null && !arg.Player ().IsAdmin ) return;

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
            if ( arg.Player () != null && !arg.Player ().IsAdmin ) return;

            CarbonCore.ClearPlugins ();
            CarbonCore.ReloadPlugins ();
        }

        [ConsoleCommand ( "find" )]
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

                    Puts ( $"LLoadiing" );
                    CompilerManager.AddCurrentDomainAssemblies ();
                    Puts ( $"LLoadiing2" );

                    var vs = OsEx.File.ReadText ( path );
                    Puts ( $"LLoadiing2 {vs.Length}" );

                    var test = CompilerManager.Compile ( vs );
                    Puts ( $"{test == null}" );
                    return;
                    CarbonCore.Instance.PluginProcessor.ClearIgnore ( path );
                    CarbonCore.Instance.PluginProcessor.Prepare ( path );
                    break;
            }
        }
    }
}