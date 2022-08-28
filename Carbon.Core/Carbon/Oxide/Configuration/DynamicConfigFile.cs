using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Core.Configuration
{
    public class DynamicConfigFile
    {
        public DynamicConfigFile () { }
        public DynamicConfigFile ( string filename ) { }

        public bool Exists ()
        {
            return false;
        }

        public T ReadObject<T> ()
        {
            return default;
        }
        public void WriteObject ( object obj )
        {

        }
    }
}