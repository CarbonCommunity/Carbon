using System;
using System.Collections;

namespace Carbon.Core.Processors
{
    public class ThreadedJob : IDisposable
    {
        private bool _isDone;
        private readonly object _handle = new object ();
        private System.Threading.Thread _thread;

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
            private set
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

        protected virtual void ThreadFunction () { }
        protected virtual void OnFinished () { }

        protected virtual bool Update ()
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
}
