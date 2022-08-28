using Oxide.Plugins;
using Rexide.Core.Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Plugins
{
    public class Plugin
    {
        public Type Type { get; set; }

        public T Call<T> ( string hook, params object [] args )
        {
            return ( T )HookExecutor.CallHook ( this, hook, args );
        }

        public object Call ( string hook, params object [] args )
        {
            return Call<object> ( hook, args );
        }

        public object CallHook ( string name )
        {
            return Call ( name );
        }

        public bool IsLoaded => true;
    }
}