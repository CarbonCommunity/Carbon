using Facepunch;
using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Plugins
{
    public class Plugins
    {
        public Plugin Find ( string name )
        {
            return default;
        }

        public Plugin [] GetAll ()
        {
            var list = Pool.GetList<Plugin>();
            foreach(var mod in CarbonLoader._loadedMods )
            {
                list.AddRange ( mod.Plugins );
            }

            var result = list.ToArray ();
            Pool.FreeList ( ref list );
            return result;
        }
    }
}