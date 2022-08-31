﻿using Carbon.Core;
using Facepunch;

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