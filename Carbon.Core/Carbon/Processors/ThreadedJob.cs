using System;
using System.Collections;

public class ThreadedJob : IDisposable
{
    internal bool _isDone = false;
    internal object _handle = new object ();
    internal System.Threading.Thread _thread = null;

    public bool IsDone
    {
        get
        {
            bool temp;
            lock ( _handle )
            {
                temp = _isDone;
            }
            return temp;
        }
        set
        {
            lock ( _handle )
            {
                _isDone = value;
            }
        }
    }

    public virtual void Start ()
    {
        _thread = new System.Threading.Thread ( Run ) { Priority = System.Threading.ThreadPriority.AboveNormal };
        _thread.Start ();
    }
    public virtual void Abort ()
    {
        _thread.Abort ();
    }

    public virtual void ThreadFunction () { }
    public virtual void OnFinished () { }

    public virtual bool Update ()
    {
        if ( IsDone )
        {
            OnFinished ();
            return true;
        }
        return false;
    }
    public IEnumerator WaitFor ()
    {
        while ( !Update () )
        {
            yield return null;
        }
    }
    private void Run ()
    {
        ThreadFunction ();
        IsDone = true;
    }

    public virtual void Dispose () { }
}
