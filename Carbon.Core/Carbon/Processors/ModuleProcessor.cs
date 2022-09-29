using Carbon.Core.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Core.Processors
{
    public class ModuleProcessor : IDisposable
    {
        public List<IModule> Modules { get; set; } = new List<IModule> ( 100 );

        public void Init ()
        {
            foreach ( var type in typeof ( ModuleProcessor ).Assembly.GetTypes () )
            {
                if ( type.BaseType == null || !type.BaseType.Name.Contains ( "BaseModule" ) ) continue;

                Setup ( Activator.CreateInstance ( type ) as IModule );
            }
        }
        public void Setup ( IModule module )
        {
            module.Init ();
            Modules.Add ( module );
            module.InitEnd ();
        }
        public void OnServerInitialized ()
        {
            foreach ( var module in Modules )
            {
                module.OnEnableStatus ();
            }
        }

        public void Save ()
        {
            foreach ( var module in Modules )
            {
                module.Save ();
            }
        }
        public void Load ()
        {
            foreach ( var module in Modules )
            {
                module.Load ();
                module.OnEnableStatus ();
            }
        }

        public void Dispose ()
        {
            foreach ( var module in Modules )
            {
                module.Dispose ();
            }

            Modules.Clear ();
        }
    }
}