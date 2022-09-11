using Carbon.Core.Extensions;
using Facepunch;
using Humanlights.Components;
using Humanlights.Extensions;
using Newtonsoft.Json;
using Oxide.Plugins;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Reply ( $"Carbon v{CarbonCore.Version}", arg );
        }

        [ConsoleCommand ( "list", "Prints the list of mods and their loaded plugins." )]
        private void List ( ConsoleSystem.Arg arg )
        {
            if ( !arg.IsPlayerCalledAndAdmin () ) return;

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
                        body.AddRow ( $"{count:n0}", $"{mod.Name}{( mod.Plugins.Count > 1 ? $" ({mod.Plugins.Count:n0})" : "" )}", "", "", mod.IsCoreMod ? "Yes" : "No", "" );

                        foreach ( var plugin in mod.Plugins )
                        {
                            body.AddRow ( $"", plugin.Name, plugin.Author, $"v{plugin.Version}", mod.IsAddon ? "Addon" : plugin.IsCorePlugin ? "Yes" : "No", $"{plugin.TotalHookTime:0.0}ms" );
                        }

                        count++;
                    }

                    Reply ( body.ToStringMinimal (), arg );
                    break;
            }
        }

        #region Config

        [ConsoleCommand ( "loadconfig", "Loads Carbon config from file.", false )]
        private void CarbonLoadConfig ( ConsoleSystem.Arg arg )
        {
            if ( !arg.IsPlayerCalledAndAdmin () || CarbonCore.Instance == null ) return;

            CarbonCore.Instance.LoadConfig ();

            Reply ( "Loaded Carbon config.", arg );
        }

        [ConsoleCommand ( "saveconfig", "Saves Carbon config to file.", false )]
        private void CarbonSaveConfig ( ConsoleSystem.Arg arg )
        {
            if ( !arg.IsPlayerCalledAndAdmin () || CarbonCore.Instance == null ) return;

            CarbonCore.Instance.SaveConfig ();

            Reply ( "Saved Carbon config.", arg );
        }

        [ConsoleCommand ( "modding", "Mark this server as moddable or not.", false )]
        private void CarbonModding ( ConsoleSystem.Arg arg )
        {
            if ( !arg.IsPlayerCalledAndAdmin () || CarbonCore.Instance == null || !arg.HasArgs ( 1 ) ) return;

            CarbonCore.Instance.Config.IsModded = arg.GetBool ( 0 );
            CarbonCore.Instance.SaveConfig ();

            Reply ( $"Set server as '{( CarbonCore.Instance.Config.IsModded ? "Modded" : "Community" )}'", arg );
        }

        #endregion

        #region Commands

        [ConsoleCommand ( "find", "Searches through Carbon-processed console commands.", false )]
        private void Find ( ConsoleSystem.Arg arg )
        {
            if ( !arg.IsPlayerCalledAndAdmin () ) return;

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
            if ( !arg.IsPlayerCalledAndAdmin () ) return;

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

        #endregion

        #region Mod & Plugin Loading

        [ConsoleCommand ( "reload", "Reloads all or specific mods / plugins. E.g 'c.reload *' to reload everything." )]
        private void Reload ( ConsoleSystem.Arg arg )
        {
            if ( !arg.IsPlayerCalledAndAdmin () || !arg.HasArgs ( 1 ) ) return;

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
                    CarbonCore.Instance.ScriptProcessor.Prepare ( name, path );
                    break;
            }
        }

        [ConsoleCommand ( "load", "Loads all mods and/or plugins. E.g 'c.load *' to load everything you've unloaded." )]
        private void Load ( ConsoleSystem.Arg arg )
        {
            if ( !arg.IsPlayerCalledAndAdmin () || !arg.HasArgs ( 1 ) ) return;

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
                        tempList.AddRange ( CarbonCore.Instance.ScriptProcessor.IgnoreList );
                        CarbonCore.Instance.ScriptProcessor.IgnoreList.Clear ();

                        foreach ( var plugin in tempList )
                        {
                            CarbonCore.Instance.ScriptProcessor.Prepare ( plugin, plugin );
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
                        CarbonCore.Instance.ScriptProcessor.ClearIgnore ( path );
                        CarbonCore.Instance.ScriptProcessor.Prepare ( path );
                    }
                    break;
            }
        }

        [ConsoleCommand ( "unload", "Unloads all mods and/or plugins. E.g 'c.unload *' to unload everything. They'll be marked as 'ignored'." )]
        private void Unload ( ConsoleSystem.Arg arg )
        {
            if ( !arg.IsPlayerCalledAndAdmin () || !arg.HasArgs ( 1 ) ) return;

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
                        tempList.AddRange ( CarbonCore.Instance.ScriptProcessor.IgnoreList );
                        CarbonCore.Instance.ScriptProcessor.IgnoreList.Clear ();

                        foreach ( var plugin in tempList )
                        {
                            CarbonCore.Instance.ScriptProcessor.Ignore ( plugin );
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
                        if ( CarbonCore.Instance.ScriptProcessor.InstanceBuffer.TryGetValue ( name, out var value ) )
                        {
                            CarbonCore.Instance.ScriptProcessor.Ignore ( path );
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
            if ( !arg.IsPlayerCalledAndAdmin () ) return;

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
            if ( !arg.IsPlayerCalledAndAdmin () ) return;

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

        [ConsoleCommand ( "show", "Displays information about a specific player or group (incl. permissions, groups and user list). Do 'c.show' for syntax info." )]
        private void Show ( ConsoleSystem.Arg arg )
        {
            if ( !arg.IsPlayerCalledAndAdmin () ) return;

            void PrintWarn ()
            {
                Reply ( $"Syntax: c.show <user|group> <name|id>", arg );
            }

            if ( !arg.HasArgs ( 2 ) )
            {
                PrintWarn ();
                return;
            }

            var action = arg.Args [ 0 ];
            var name = arg.Args [ 1 ];

            switch ( action )
            {
                case "user":
                    var user = permission.FindUser ( name );
                    if ( user.Value == null )
                    {
                        Reply ( $"Couldn't find that user.", arg );
                        return;
                    }

                    Reply ( $"User {user.Value.LastSeenNickname}[{user.Key}] found in {user.Value.Groups.Count:n0} groups:\n  {user.Value.Groups.Select ( x => x ).ToArray ().ToString ( ", ", " and " )}", arg );
                    Reply ( $"and has {user.Value.Perms.Count:n0} permissions:\n  {user.Value.Perms.Select ( x => x ).ToArray ().ToString ( ", ", " and " )}", arg );
                    break;

                case "group":
                    if ( !permission.GroupExists ( name ) )
                    {
                        Reply ( $"Couldn't find that group.", arg );
                        return;
                    }

                    var users = permission.GetUsersInGroup ( name );
                    var permissions = permission.GetGroupPermissions ( name, false );
                    Reply ( $"Group {name} has {users.Length:n0} users:\n  {users.Select ( x => x ).ToArray ().ToString ( ", ", " and " )}", arg );
                    Reply ( $"and has {permissions.Length:n0} permissions:\n  {permissions.Select ( x => x ).ToArray ().ToString ( ", ", " and " )}", arg );
                    break;

                default:
                    PrintWarn ();
                    break;
            }
        }

        [ConsoleCommand ( "usergroup", "Adds or removes a player from a group. Do 'c.usergroup' for syntax info." )]
        private void UserGroup ( ConsoleSystem.Arg arg )
        {
            if ( !arg.IsPlayerCalledAndAdmin () ) return;

            void PrintWarn ()
            {
                Reply ( $"Syntax: c.usergroup <add|remove> <player> <group>", arg );
            }

            if ( !arg.HasArgs ( 3 ) )
            {
                PrintWarn ();
                return;
            }

            var action = arg.Args [ 0 ];
            var player = arg.Args [ 1 ];
            var group = arg.Args [ 2 ];

            var user = permission.FindUser ( player );

            if ( user.Value == null )
            {
                Reply ( $"Couldn't find that player.", arg );
                return;
            }

            if ( !permission.GroupExists ( group ) )
            {
                Reply ( $"Group '{group}' could not be found.", arg );
                return;
            }

            switch ( action )
            {
                case "add":
                    if ( permission.UserHasGroup ( user.Key, group ) )
                    {
                        Reply ( $"{user.Value.LastSeenNickname}[{user.Key}] is already in '{group}' group.", arg );
                        return;
                    }

                    permission.AddUserGroup ( user.Key, group );
                    Reply ( $"Added {user.Value.LastSeenNickname}[{user.Key}] to '{group}' group.", arg );
                    break;

                case "remove":
                    if ( !permission.UserHasGroup ( user.Key, group ) )
                    {
                        Reply ( $"{user.Value.LastSeenNickname}[{user.Key}] isn't in '{group}' group.", arg );
                        return;
                    }

                    permission.RemoveUserGroup ( user.Key, group );
                    Reply ( $"Removed {user.Value.LastSeenNickname}[{user.Key}] from '{group}' group.", arg );
                    break;

                default:
                    PrintWarn ();
                    break;
            }
        }

        [ConsoleCommand ( "group", "Adds or removes a group. Do 'c.group' for syntax info." )]
        private void Group ( ConsoleSystem.Arg arg )
        {
            if ( !arg.IsPlayerCalledAndAdmin () ) return;

            void PrintWarn ()
            {
                Reply ( $"Syntax: c.group add <group> [<displayName>] [<rank>]", arg );
                Reply ( $"Syntax: c.group remove <group>", arg );
                Reply ( $"Syntax: c.group set <group> <title|rank> <value>", arg );
                Reply ( $"Syntax: c.group parent <group> [<parent>]", arg );
            }

            if ( !arg.HasArgs ( 1 ) ) { PrintWarn (); return; }

            var action = arg.Args [ 0 ];

            switch ( action )
            {
                case "add":
                    {
                        if ( !arg.HasArgs ( 2 ) ) { PrintWarn (); return; }

                        var group = arg.Args [ 1 ];

                        if ( permission.GroupExists ( group ) )
                        {
                            Reply ( $"Group '{group}' already exists. To set any values for this group, use 'c.group set'.", arg );
                            return;
                        }

                        if ( permission.CreateGroup ( group, arg.HasArgs ( 3 ) ? arg.Args [ 2 ] : group, arg.HasArgs ( 4 ) ? arg.Args [ 3 ].ToInt () : 0 ) )
                        {
                            Reply ( $"Created '{group}' group.", arg );
                        }
                    }
                    break;

                case "set":
                    {
                        if ( !arg.HasArgs ( 2 ) ) { PrintWarn (); return; }

                        var group = arg.Args [ 1 ];

                        if ( !permission.GroupExists ( group ) )
                        {
                            Reply ( $"Group '{group}' does not exists.", arg );
                            return;
                        }

                        if ( arg.HasArgs ( 3 ) ) permission.SetGroupTitle ( group, arg.Args [ 2 ] );
                        if ( arg.HasArgs ( 4 ) ) permission.SetGroupTitle ( group, arg.Args [ 3 ] );

                        Reply ( $"Set '{group}' group.", arg );
                    }
                    break;
                case "remove":
                    {
                        if ( !arg.HasArgs ( 2 ) ) { PrintWarn (); return; }

                        var group = arg.Args [ 1 ];

                        if ( permission.RemoveGroup ( group ) ) Reply ( $"Removed '{group}' group.", arg );
                        else Reply ( $"Couldn't remove '{group}' group.", arg );
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