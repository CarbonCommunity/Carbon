using Facepunch;
using Humanlights.Components;
using Humanlights.Extensions;
using Newtonsoft.Json;
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

        public override void Init ()
        {
            foreach ( var player in BasePlayer.activePlayerList )
            {
                permission.RefreshUser ( player );
            }
        }
        private void OnPluginLoaded ( Plugin plugin )
        {
        }
        private void OnPluginUnloaded ( Plugin plugin )
        {
        }
        private void OnPlayerConnected ( BasePlayer player )
        {
            permission.RefreshUser ( player );
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

        [ConsoleCommand ( "version", "Returns currently loaded version of Carbon." )]
        private void GetVersion ( ConsoleSystem.Arg arg )
        {
            if ( arg.Player () != null && !arg.Player ().IsAdmin ) return;

            Reply ( $"Carbon v{CarbonCore.Version}", arg );
        }

        [ConsoleCommand ( "find", "Searches through Carbon-processed console commands.", false )]
        private void Find ( ConsoleSystem.Arg arg )
        {
            var body = new StringBody ();
            var filter = arg.Args != null && arg.Args.Length > 0 ? arg.Args [ 0 ] : null;
            body.Add ( $"Console Commands:" );

            foreach ( var command in CarbonCore.Instance.AllConsoleCommands )
            {
                if ( !string.IsNullOrEmpty ( filter ) && !command.Command.Contains ( filter ) ) continue;

                body.Add ( $" {command.Command}(   )  {command.Help}" );
            }

            Reply ( body.ToNewLine (), arg );
        }

        [ConsoleCommand ( "findchat", "Searches through Carbon-processed chat commands.", false )]
        private void FindChat ( ConsoleSystem.Arg arg )
        {
            var body = new StringBody ();
            var filter = arg.Args != null && arg.Args.Length > 0 ? arg.Args [ 0 ] : null;
            body.Add ( $"Chat Commands:" );

            foreach ( var command in CarbonCore.Instance.AllChatCommands )
            {
                if ( !string.IsNullOrEmpty ( filter ) && !command.Command.Contains ( filter ) ) continue;

                body.Add ( $" {command.Command}(   )  {command.Help}" );
            }

            Reply ( body.ToNewLine (), arg );
        }

        #region Mod & Plugin Loading

        [ConsoleCommand ( "list", "Prints the list of mods and their loaded plugins." )]
        private void GetList ( ConsoleSystem.Arg arg )
        {
            if ( arg.Player () != null && !arg.Player ().IsAdmin ) return;

            var mode = arg.HasArgs ( 1 ) ? arg.Args [ 0 ] : null;

            switch ( mode )
            {
                case "-j":
                case "--j":
                case "-json":
                case "--json":
                    Reply ( JsonConvert.SerializeObject ( CarbonLoader._loadedMods, Formatting.Indented ), arg );
                    break;

                default:
                    var body = new StringTable ( "#", "Mod", "Author", "Version", "Core", "Hook Time" );
                    var count = 1;

                    foreach ( var mod in CarbonLoader._loadedMods )
                    {
                        body.AddRow ( $"{count:n0}", $"{mod.Name}{(mod.Plugins.Count > 1 ? $" ({mod.Plugins.Count:n0})" : "")}", "", "", mod.IsCoreMod ? "Yes" : "No", "" );

                        foreach ( var plugin in mod.Plugins )
                        {
                            body.AddRow ( $"", plugin.Name, plugin.Author, $"v{plugin.Version}", plugin.IsCorePlugin ? "Yes" : "No", $"{plugin.TotalHookTime:0.0}ms" );
                        }

                        count++;
                    }

                    Reply ( body.ToStringMinimal (), arg );
                    break;
            }
        }

        [ConsoleCommand ( "reload", "Reloads all or specific mods / plugins. E.g 'c.reload *' to reload everything." )]
        private void Reload ( ConsoleSystem.Arg arg )
        {
            if ( arg.Player () != null && !arg.Player ().IsAdmin || !arg.HasArgs ( 1 ) ) return;

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

        [ConsoleCommand ( "load", "Loads all mods and/or plugins. E.g 'c.load *' to load everything you've unloaded." )]
        private void Load ( ConsoleSystem.Arg arg )
        {
            if ( arg.Player () != null && !arg.Player ().IsAdmin || !arg.HasArgs ( 1 ) ) return;

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
                        tempList.AddRange ( CarbonCore.Instance.HarmonyProcessor.IgnoreList );
                        CarbonCore.Instance.HarmonyProcessor.IgnoreList.Clear ();

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
                        tempList.AddRange ( CarbonCore.Instance.PluginProcessor.IgnoreList );
                        CarbonCore.Instance.PluginProcessor.IgnoreList.Clear ();

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

        [ConsoleCommand ( "unload", "Unloads all mods and/or plugins. E.g 'c.unload *' to unload everything. They'll be marked as 'ignored'." )]
        private void Unload ( ConsoleSystem.Arg arg )
        {
            if ( arg.Player () != null && !arg.Player ().IsAdmin || !arg.HasArgs ( 1 ) ) return;

            RefreshOrderedFiles ();

            var name = arg.Args [ 0 ];
            switch ( name )
            {
                case "*":
                    //
                    // Mods
                    //
                    {
                        foreach ( var plugin in CarbonCore.Instance.HarmonyProcessor.InstanceBuffer )
                        {
                            CarbonCore.Instance.HarmonyProcessor.Ignore ( plugin.Value.File );
                        }
                        CarbonCore.Instance.HarmonyProcessor.Clear ();
                    }

                    //
                    // Plugins
                    //
                    {
                        var tempList = Pool.GetList<string> ();
                        tempList.AddRange ( CarbonCore.Instance.PluginProcessor.IgnoreList );
                        CarbonCore.Instance.PluginProcessor.IgnoreList.Clear ();

                        foreach ( var plugin in tempList )
                        {
                            CarbonCore.Instance.PluginProcessor.Ignore ( plugin );
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
                        if ( CarbonCore.Instance.HarmonyProcessor.InstanceBuffer.TryGetValue ( name, out var value ) )
                        {
                            CarbonCore.Instance.HarmonyProcessor.Ignore ( path );
                            value.Dispose ();
                        }
                    }

                    //
                    // Plugins
                    //
                    {
                        if ( CarbonCore.Instance.PluginProcessor.InstanceBuffer.TryGetValue ( name, out var value ) )
                        {
                            CarbonCore.Instance.PluginProcessor.Ignore ( path );
                            value.Dispose ();
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Permissions

        [ConsoleCommand ( "grant", "Grant one or more permissions to users or groups. Do 'c.grant' for syntax info." )]
        private void Grant ( ConsoleSystem.Arg arg )
        {
            void PrintWarn ()
            {
                Reply ( $"Syntax: c.grant <user|group> <name|id> <perm>", arg );
            }

            if ( !arg.HasArgs ( 3 ) )
            {
                PrintWarn ();
                return;
            }

            var action = arg.Args [ 0 ];
            var name = arg.Args [ 1 ];
            var perm = arg.Args [ 2 ];
            var user = permission.FindUser ( name );

            switch ( action )
            {
                case "user":
                    if ( permission.GrantUserPermission ( user.Key, perm, null ) )
                    {
                        Reply ( $"Granted user '{user.Value.LastSeenNickname}' permission '{perm}'", arg );
                    }
                    break;

                case "group":
                    if ( permission.GrantGroupPermission ( name, perm, null ) )
                    {
                        Reply ( $"Granted group '{name}' permission '{perm}'", arg );
                    }
                    break;

                default:
                    PrintWarn ();
                    break;
            }
        }

        [ConsoleCommand ( "revoke", "Revoke one or more permissions from users or groups. Do 'c.revoke' for syntax info." )]
        private void Revoke ( ConsoleSystem.Arg arg )
        {
            void PrintWarn ()
            {
                Reply ( $"Syntax: c.revoke <user|group> <name|id> <perm>", arg );
            }

            if ( !arg.HasArgs ( 3 ) )
            {
                PrintWarn ();
                return;
            }

            var action = arg.Args [ 0 ];
            var name = arg.Args [ 1 ];
            var perm = arg.Args [ 2 ];
            var user = permission.FindUser ( name );

            switch ( action )
            {
                case "user":
                    if ( permission.RevokeUserPermission ( user.Key, perm ) )
                    {
                        Reply ( $"Revoked user '{user.Value?.LastSeenNickname}' permission '{perm}'", arg );
                    }
                    break;

                case "group":
                    if ( permission.RevokeGroupPermission ( name, perm ) )
                    {
                        Reply ( $"Revoked group '{name}' permission '{perm}'", arg );
                    }
                    break;

                default:
                    PrintWarn ();
                    break;
            }
        }

        #endregion
    }
}