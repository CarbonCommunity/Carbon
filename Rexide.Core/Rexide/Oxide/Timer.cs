using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Timer
{
    public void In ( float time, Action action )
    {
        ServerMgr.Instance.Invoke ( action, time );
    }
}