///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Oxide.Plugins;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using static ConsoleSystem;
using Pool = Facepunch.Pool;

public class Command
{
    public void AddChatCommand ( string command, RustPlugin plugin, Action<BasePlayer, string, string []> callback, bool skipOriginal = true, string help = null )
    {
        if ( CarbonCore.Instance.AllChatCommands.Count ( x => x.Command == command ) == 0 )
        {
            CarbonCore.Instance.AllChatCommands.Add ( new OxideCommand
            {
                Command = command,
                Plugin = plugin,
                SkipOriginal = skipOriginal,
                Callback = ( player, cmd, args ) =>
                {
                    try { callback.Invoke ( player, cmd, args ); }
                    catch ( Exception ex ) { plugin.LogError ( "Error", ex ); }
                },
                Help = help
            } );
        }
        else CarbonCore.WarnFormat ( $"Chat command '{command}' already exists." );
    }
    public void AddChatCommand ( string command, RustPlugin plugin, string method, bool skipOriginal = true, string help = null )
    {
        AddChatCommand ( command, plugin, ( player, cmd, args ) =>
        {
            var argData = Pool.GetList<object> ();
            var result = ( object [] )null;
            try
            {
                var m = plugin.GetType ().GetMethod ( method, BindingFlags.Instance | BindingFlags.NonPublic );
                switch ( m.GetParameters ().Length )
                {
                    case 1:
                        {
                            argData.Add ( player );
                            result = argData.ToArray ();
                            break;
                        }

                    case 2:
                        {
                            argData.Add ( player );
                            argData.Add ( cmd );
                            result = argData.ToArray ();
                            break;
                        }

                    case 3:
                        {
                            argData.Add ( player );
                            argData.Add ( cmd );
                            argData.Add ( args );
                            result = argData.ToArray ();
                            break;
                        }
                }

                m?.Invoke ( plugin, result );
            }
            catch ( Exception ex ) { plugin.LogError ( "Error", ex ); }

            if ( argData != null ) Pool.FreeList ( ref argData );
            if ( result != null ) Pool.Free ( ref result );
        }, skipOriginal, help );
    }
    public void AddConsoleCommand ( string command, RustPlugin plugin, Action<BasePlayer, string, string []> callback, bool skipOriginal = true, string help = null )
    {
        if ( CarbonCore.Instance.AllConsoleCommands.Count ( x => x.Command == command ) == 0 )
        {
            CarbonCore.Instance.AllConsoleCommands.Add ( new OxideCommand
            {
                Command = command,
                Plugin = plugin,
                SkipOriginal = skipOriginal,
                Callback = callback,
                Help = help
            } );
        }
        else CarbonCore.WarnFormat ( $"Console command '{command}' already exists." );
    }
    public void AddConsoleCommand ( string command, RustPlugin plugin, string method, bool skipOriginal = true, string help = null )
    {
        AddConsoleCommand ( command, plugin, ( player, cmd, args ) =>
        {
            var arguments = Pool.GetList<object> ();
            var result = ( object [] )null;

            try
            {
                var fullString = args == null || args.Length == 0 ? cmd : $"{cmd} {string.Join ( " ", args )}";
                var value = new object [] { fullString };
                var client = player == null ? Option.Unrestricted : Option.Client;
                var arg = FormatterServices.GetUninitializedObject ( typeof ( Arg ) ) as Arg;
                if ( player != null ) client = client.FromConnection ( player.net.connection );
                arg.Option = client;
                arg.FullString = fullString;
                arg.Args = args;

                arguments.Add ( arg );
                result = arguments.ToArray ();

                try { plugin.GetType ().GetMethod ( method, BindingFlags.Instance | BindingFlags.NonPublic )?.Invoke ( plugin, result ); }
                catch ( Exception ex ) { plugin.LogError ( "Error", ex ); }
            }
            catch ( TargetParameterCountException ) { }
            catch ( Exception ex ) { plugin.LogError ( "Error", ex ); }

            Pool.FreeList ( ref arguments );
            if ( result != null ) Pool.Free ( ref result );
        }, skipOriginal, help );
    }
    public void AddConsoleCommand ( string command, RustPlugin plugin, Func<Arg, bool> callback, bool skipOriginal = true, string help = null )
    {
        AddConsoleCommand ( command, plugin, ( player, cmd, args ) =>
        {
            var arguments = Pool.GetList<object> ();
            var result = ( object [] )null;

            try
            {
                var fullString = args == null || args.Length == 0 ? cmd : $"{cmd} {string.Join ( " ", args )}";
                var value = new object [] { fullString };
                var client = player == null ? Option.Unrestricted : Option.Client;
                var arg = FormatterServices.GetUninitializedObject ( typeof ( Arg ) ) as Arg;
                if ( player != null ) client = client.FromConnection ( player.net.connection );
                arg.Option = client;
                arg.FullString = fullString;
                arg.Args = args;

                arguments.Add ( arg );
                result = arguments.ToArray ();

                callback.Invoke ( arg );
            }
            catch ( TargetParameterCountException ) { }
            catch ( Exception ex ) { plugin.LogError ( "Error", ex ); }

            Pool.FreeList ( ref arguments );
            if ( result != null ) Pool.Free ( ref result );
        }, skipOriginal, help );
    }
    public void AddCovalenceCommand ( string command, RustPlugin plugin, string method, bool skipOriginal = true, string help = null )
    {
        AddChatCommand ( command, plugin, method, skipOriginal, help );
        AddConsoleCommand ( command, plugin, method, skipOriginal, help );
    }
    public void AddCovalenceCommand ( string command, RustPlugin plugin, Action<BasePlayer, string, string []> callback, bool skipOriginal = true, string help = null )
    {
        AddChatCommand ( command, plugin, callback, skipOriginal, help );
        AddConsoleCommand ( command, plugin, callback, skipOriginal, help );
    }
}