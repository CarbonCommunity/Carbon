using Carbon.Core.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Core.Processors
{
    public class ModuleProcessor : IDisposable
    {
        public List<IModule> Modules { get; set; } = new List<IModule> ( 100 );

        public void Setup ( IModule module )
        {
            module.Init ();

            Modules.Add ( module );
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