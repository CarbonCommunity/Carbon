using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Core
{
    public class Interface
    {
        public static OxideMod Oxide { get; set; } = new OxideMod ();

        public static object CallHook(string hook, params object [] args )
        {
            return default;
        }
    }
}
