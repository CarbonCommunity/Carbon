using Oxide.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Core
{
    public class DataFileSystem
    {
        public DynamicConfigFile GetFile ( string file )
        {
            return new DynamicConfigFile ();
        }

        public bool ExistsDatafile(string name )
        {
            return false;
        }

        public void WriteObject(string name, object obj )
        {

        }
        public T ReadObject<T> (string name )
        {
            return default;
        }
    }
}