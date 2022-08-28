using ConVar;
using Facepunch;
using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ConsoleSystem;
using Pool = Facepunch.Pool;

public class Command
{
    public void AddChatCommand ( string command, RustPlugin plugin, Action<BasePlayer, string, string []> callback )
    {
        if ( CarbonCore.Instance.AllChatCommands.Count ( x => x.Command == command ) == 0 )
        {
            CarbonCore.Instance.AllChatCommands.Add ( new OxideCommand
            {
                Command = command,
                Plugin = plugin,
                Callback = ( player, cmd, args ) =>
                {
                    try { callback.Invoke ( player, cmd, args ); }
                    catch ( Exception ex ) { plugin.Error ( "Error", ex ); }
                }
            } );
        }
        else CarbonCore.WarnFormat ( $"Chat command '{command}' already exists." );
    }
    public void AddChatCommand ( string command, RustPlugin plugin, string method )
    {
        AddChatCommand ( command, plugin, ( player, cmd, args ) =>
        {
            var argData = Pool.GetList<object> ();
            argData.Add ( player );
            argData.Add ( cmd );
            argData.Add ( args );
            var result = argData.ToArray ();

            try { plugin.GetType ().GetMethod ( method, BindingFlags.Instance | BindingFlags.NonPublic )?.Invoke ( plugin, result ); }
            catch ( Exception ex ) { plugin.Error ( "Error", ex ); }

            Pool.FreeList ( ref argData );
            Pool.Free ( ref result );
        } );
    }
    public void AddConsoleCommand ( string command, RustPlugin plugin, Action<BasePlayer, string, string []> callback )
    {
        if ( CarbonCore.Instance.AllConsoleCommands.Count ( x => x.Command == command ) == 0 )
        {
            CarbonCore.Instance.AllConsoleCommands.Add ( new OxideCommand
            {
                Command = command,
                Plugin = plugin,
                Callback = callback
            } );
        }
        else CarbonCore.WarnFormat ( $"Console command '{command}' already exists." );
    }
    public void AddConsoleCommand ( string command, RustPlugin plugin, string method )
    {
        AddConsoleCommand ( command, plugin, ( player, cmd, args ) =>
        {
            var arguments = Pool.GetList<object> ();
            var result = ( object [] )null;

            try
            {
                var fullString = $"{cmd} {string.Join ( " ", args )}";
                var value = new object [] { fullString };
                var client = Option.Unrestricted;
                var arg = FormatterServices.GetUninitializedObject ( typeof ( Arg ) ) as Arg;
                client = client.FromConnection ( player.net.connection );
                arg.Option = client;
                arg.FullString = fullString;
                arg.Args = args;

                arguments.Add ( arg );
                result = arguments.ToArray ();

                try { plugin.GetType ().GetMethod ( method, BindingFlags.Instance | BindingFlags.NonPublic )?.Invoke ( plugin, result ); }
                catch ( Exception ex ) { plugin.Error ( "Error", ex ); }
            }
            catch ( TargetParameterCountException ) { }
            catch ( Exception ex ) { plugin.Error ( "Error", ex ); }

            Pool.FreeList ( ref arguments );
            if ( result != null ) Pool.Free ( ref result );
        } );
    }
    public void AddConsoleCommand ( string command, RustPlugin plugin, Func<Arg, bool> callback )
    {
        AddConsoleCommand ( command, plugin, ( player, cmd, args ) =>
        {
            var arguments = Pool.GetList<object> ();
            var result = ( object [] )null;

            try
            {
                var fullString = $"{cmd} {string.Join ( " ", args )}";
                var value = new object [] { fullString };
                var client = Option.Unrestricted;
                var arg = FormatterServices.GetUninitializedObject ( typeof ( Arg ) ) as Arg;
                client = client.FromConnection ( player.net.connection );
                arg.Option = client;
                arg.FullString = fullString;
                arg.Args = args;

                arguments.Add ( arg );
                result = arguments.ToArray ();

                callback.Invoke ( arg );
            }
            catch ( TargetParameterCountException ) { }
            catch ( Exception ex ) { plugin.Error ( "Error", ex ); }

            Pool.FreeList ( ref arguments );
            if ( result != null ) Pool.Free ( ref result );
        } );
    }
}