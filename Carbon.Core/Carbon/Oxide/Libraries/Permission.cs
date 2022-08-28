using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Core.Libraries
{
    public class Permission
    {
        public void RegisterPermission ( string perm, RustPlugin plugin )
        {
            //kekw2
        }

        public bool UserHasPermission ( string steamId, string perm )
        {
            return true;
        }

        public bool UserHasGroup ( string steamId, string group )
        {
            return true;
        }
    }
}