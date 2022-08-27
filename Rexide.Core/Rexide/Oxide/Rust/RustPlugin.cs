using Oxide.Core.Libraries;
using System;
using UnityEngine;

namespace Oxide.Plugins
{
    public class RustPlugin : Plugin
    {
        public static Type FindType ( string withName )
        {
            withName = withName.ToLower ().Trim ();

            foreach ( var ass in AppDomain.CurrentDomain.GetAssemblies () )
            {
                if ( ass.FullName.ToLower ().Contains ( withName ) )
                {
                    return ass.GetTypes () [ 0 ];
                }
            }

            return null;
        }

        public string Name;

        public Permission permission { get; set; } = new Permission ();
        public Manager Manager { get; set; } = new Manager ();
        public Command cmd { get; set; } = new Command ();
        public Plugins plugins { get; set; } = new Plugins ();
        public Timer timer { get; set; } = new Timer ();
        public WebRequests webrequest { get; set; } = new WebRequests ();

        public RexideLoader.RexideMod mod { get; set; }

        public RustPlugin ()
        {
            Setup ( "Plugin" );
        }

        public void SetupMod ( RexideLoader.RexideMod mod )
        {
            this.mod = mod;
            Setup ( mod.Name );
        }
        public void Setup ( string name )
        {
            Name = name;
            Puts ( $"Initialized." );

            permission = new Core.Libraries.Permission ();
            cmd = new Command ();
            Manager = new Manager ();
            plugins = new Plugins ();
            timer = new Timer ();

            Type = GetType ();
        }
        public void Puts ( string message )
        {
            RexideCore.Log ( $"[{Name}] {message}" );
        }
        public void Error ( string message, Exception exception )
        {
            RexideCore.Error ( $"[{Name}] {message}", exception );
        }
    }
}