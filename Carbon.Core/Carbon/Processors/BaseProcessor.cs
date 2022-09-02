using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Core.Processors
{
    public class BaseProcessor : FacepunchBehaviour
    {
        public bool IsInitialized { get; set; }
    
        public virtual void Start ()
        {
            IsInitialized = true;
        }
    }
}
