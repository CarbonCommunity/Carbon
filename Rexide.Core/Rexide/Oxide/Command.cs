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

public class Command
{
    public void AddChatCommand ( string command, RustPlugin plugin, Action<BasePlayer, string, string []> callback )
    {
        if ( Rexide.Instance.AllChatCommands.Count ( x => x.Command == command ) == 0 )
        {
            Rexide.Instance.AllChatCommands.Add ( new OxideCommand
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
        else Rexide.Warn ( $"Chat command '{command}' already exists." );
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
    public void AddConsoleCommand ( string command, RustPlugin plugin, Func<ConsoleSystem.Arg, bool> callback )
    {
        if ( Rexide.Instance.AllConsoleCommands.Count ( x => x.Plugin == plugin && x.Command == command ) == 0 )
        {
            Rexide.Instance.AllConsoleCommands.Add ( new OxideCommand
            {
                Command = command,
                Plugin = plugin,
                Callback = ( player, cmd, args ) =>
                {
                    var arguments2 = Pool.GetList<object> ();
                    var result2 = ( object [] )null;

                    try
                    {
                        var fullString = $"{cmd} {string.Join ( " ", args )}";
                        var value = new object [] { fullString };
                        var client = ConsoleSystem.Option.Unrestricted;
                        var arg = FormatterServices.GetUninitializedObject ( typeof ( ConsoleSystem.Arg ) ) as ConsoleSystem.Arg;
                        client = client.FromConnection ( player.net.connection );
                        arg.Option = client;
                        arg.FullString = fullString;
                        arg.Args = args;

                        arguments2.Add ( arg );
                        result2 = arguments2.ToArray ();

                        callback.Invoke ( arg );
                    }
                    catch ( TargetParameterCountException ) { }
                    catch ( Exception ex ) { plugin.Error ( "Error", ex ); }

                    Pool.FreeList ( ref arguments2 );
                    if ( result2 != null ) Pool.Free ( ref result2 );
                }
            } );

            plugin.Puts ( $" Added console command: '{command}'" );
        }
        else Rexide.Warn ( $"Console command '{command}' already exists." );
    }
}
}