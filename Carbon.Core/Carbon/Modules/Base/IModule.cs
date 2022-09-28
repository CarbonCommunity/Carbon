using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Core.Modules
{
    public interface IModule
    {
        void Init ();
        void Save ();
        void Load ();
    }
}
