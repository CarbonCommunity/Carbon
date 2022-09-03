using Carbon.Core;
using Humanlights.Extensions;
using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries;
using System;
using System.IO;
using UnityEngine;

namespace Oxide.Plugins
{
    public class RustPlugin : Plugin
    {
        public Permission permission { get; set; } = new Permission ();
        public Language lang { get; set; } = new Language ();
        public PluginManager Manager { get; set; } = new PluginManager ();
        public Command cmd { get; set; } = new Command ();
        public Plugins plugins { get; set; } = new Plugins ();
        public Timers timer { get; set; } = new Timers ();
        public OxideMod mod { get; set; } = new OxideMod ();
        public WebRequests webrequest { get; set; } = new WebRequests ();

        public CarbonLoader.CarbonMod carbon { get; set; }

        public DynamicConfigFile Config { get; private set; }

        public RustPlugin ()
        {
            Setup ( $"Core Plugin {RandomEx.GetRandomString ( 5 )}", "Carbon Community", new VersionNumber ( 1, 0, 0 ), string.Empty );
        }

        public void SetupMod ( CarbonLoader.CarbonMod mod, string name, string author, VersionNumber version, string description )
        {
            carbon = mod;
            Setup ( name, author, version, description );
        }
        public void Setup ( string name, string author, VersionNumber version, string description )
        {
            Name = name;
            Version = version;
            Author = author;
            Description = description;

            permission = new Permission ();
            cmd = new Command ();
            Manager = new PluginManager ();
            plugins = new Plugins ();
            timer = new Timers ();
            lang = new Language ();
            mod = new OxideMod ();
            webrequest = new WebRequests ();

            Type = GetType ();

            mod.Load ();
        }
        public void Puts ( string message )
        {
            CarbonCore.Format ( $"[{Name}] {message}" );
        }
        public void Puts ( string message, params object [] args )
        {
            Puts ( string.Format ( message, args ) );
        }
        public void Error ( string message, Exception exception )
        {
            CarbonCore.Error ( $"[{Name}] {message}", exception );
        }
        protected void PrintWarning ( string format, params object [] args )
        {
            CarbonCore.WarnFormat ( "[{0}] {1}", Title, ( args.Length != 0 ) ? string.Format ( format, args ) : format );
        }

        protected void PrintError ( string format, params object [] args )
        {
            CarbonCore.ErrorFormat ( "[{0}] {1}", null, Title, ( args.Length != 0 ) ? string.Format ( format, args ) : format );
        }

        public void DoLoadConfig ()
        {
            LoadConfig ();
        }

        protected virtual void LoadConfig ()
        {
            Config = new DynamicConfigFile ( Path.Combine ( Manager.ConfigPath, Name + ".json" ) );

            if ( !Config.Exists ( null ) )
            {
                LoadDefaultConfig ();
                SaveConfig ();
            }
            try
            {
                Config.Load ( null );
            }
            catch ( Exception ex )
            {
                CarbonCore.Error ( "Failed to load config file (is the config file corrupt?) (" + ex.Message + ")" );
            }
        }
        protected virtual void LoadDefaultConfig ()
        {
            // CallHook ( "LoadDefaultConfig" );
        }
        protected virtual void SaveConfig ()
        {
            if ( Config == null )
            {
                return;
            }
            try
            {
                Config.Save ( null );
            }
            catch ( Exception ex )
            {
                CarbonCore.Error ( "Failed to save config file (does the config have illegal objects in it?) (" + ex.Message + ")", ex );
            }
        }

        public void Unsubscribe ( string hook )
        {

        }
        public void Subscribe ( string hook )
        {

        }

        public override string ToString ()
        {
            return $"{Name} v{Version} by {Author}";
        }

        protected void PrintToConsole ( BasePlayer player, string format, params object [] args )
        {
            if ( ( ( player != null ) ? player.net : null ) != null )
            {
                player.SendConsoleCommand ( "echo " + ( ( args.Length != 0 ) ? string.Format ( format, args ) : format ) );
            }
        }
        protected void PrintToConsole ( string format, params object [] args )
        {
            if ( BasePlayer.activePlayerList.Count >= 1 )
            {
                ConsoleNetwork.BroadcastToAllClients ( "echo " + ( ( args.Length != 0 ) ? string.Format ( format, args ) : format ) );
            }
        }
        protected void PrintToChat ( BasePlayer player, string format, params object [] args )
        {
            if ( ( ( player != null ) ? player.net : null ) != null )
            {
                player.SendConsoleCommand ( "chat.add", 2, 0, ( args.Length != 0 ) ? string.Format ( format, args ) : format );
            }
        }
        protected void PrintToChat ( string format, params object [] args )
        {
            if ( BasePlayer.activePlayerList.Count >= 1 )
            {
                ConsoleNetwork.BroadcastToAllClients ( "chat.add", 2, 0, ( args.Length != 0 ) ? string.Format ( format, args ) : format );
            }
        }

        protected void SendReply ( ConsoleSystem.Arg arg, string format, params object [] args )
        {
            var connection = arg.Connection;
            var basePlayer = connection?.player as BasePlayer;
            var text = ( args.Length != 0 ) ? string.Format ( format, args ) : format;

            if ( ( ( basePlayer != null ) ? basePlayer.net : null ) != null )
            {
                basePlayer.SendConsoleCommand ( $"echo {text}" );
                return;
            }

            Puts ( text, null );
        }
        protected void SendReply ( BasePlayer player, string format, params object [] args )
        {
            PrintToChat ( player, format, args );
        }
        protected void SendWarning ( ConsoleSystem.Arg arg, string format, params object [] args )
        {
            var connection = arg.Connection;
            var basePlayer = connection?.player as BasePlayer;
            var text = ( args.Length != 0 ) ? string.Format ( format, args ) : format;

            if ( ( ( basePlayer != null ) ? basePlayer.net : null ) != null )
            {
                basePlayer.SendConsoleCommand ( $"echo {text}" );
                return;
            }

            Debug.LogWarning ( text );
        }
        protected void SendError ( ConsoleSystem.Arg arg, string format, params object [] args )
        {
            var connection = arg.Connection;
            var basePlayer = connection?.player as BasePlayer;
            var text = ( args.Length != 0 ) ? string.Format ( format, args ) : format;

            if ( ( ( basePlayer != null ) ? basePlayer.net : null ) != null )
            {
                basePlayer.SendConsoleCommand ( $"echo {text}" );
                return;
            }

            Debug.LogError ( text );
        }

        protected void ForcePlayerPosition ( BasePlayer player, Vector3 destination )
        {
            player.MovePosition ( destination );

            if ( !player.IsSpectating () || ( double )Vector3.Distance ( player.transform.position, destination ) > 25.0 )
            {
                player.ClientRPCPlayer ( null, player, "ForcePositionTo", destination );
                return;
            }

            player.SendNetworkUpdate ( BasePlayer.NetworkQueue.UpdateDistance );
        }
    }
}