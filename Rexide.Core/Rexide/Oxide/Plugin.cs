using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Plugins
{
    public class Plugin
    {
        public T Call<T> ( string hook, params object [] args )
        {
            return default;
        }

        public object Call ( string hook, params object [] args )
        {
            return Call<object> ( hook, args );
        }

        public object CallHook ( string name )
        {
            return default;
        }

        public bool IsLoaded => true;
    }
}