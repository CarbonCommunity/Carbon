using Carbon.Core;
using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Timers
{
    public void In ( float time, Action action )
    {
        ServerMgr.Instance.Invoke ( action, time );
    }
    public void Once ( float time, Action action )
    {
        In ( time, action );
    }
}

namespace Oxide.Core.Libraries
{
    public class Timer
    {
        public bool IsGlobal
        {
            get
            {
                return false;
            }
        }

        public static int Count { get; private set; }

        public Timer ()
        {
            for ( int i = 0; i < 512; i++ )
            {
                timeSlots [ i ] = new TimeSlot ();
            }
        }

        public void Update ( float delta )
        {
            float now = Oxide.Now;
            TimeSlot [] array = timeSlots;
            Queue<TimerInstance> queue = expiredInstanceQueue;
            int num = 0;
            object @lock = Lock;
            lock ( @lock )
            {
                int num2 = currentSlot;
                double num3 = nextSlotAt;
                for (; ; )
                {
                    array [ num2 ].GetExpired ( ( num3 > ( double )now ) ? ( ( double )now ) : num3, queue );
                    if ( ( double )now <= num3 )
                    {
                        break;
                    }
                    num++;
                    num2 = ( ( num2 < 511 ) ? ( num2 + 1 ) : 0 );
                    num3 += nextSlotAt;
                }
                if ( num > 0 )
                {
                    currentSlot = num2;
                    nextSlotAt = num3;
                }
                int count = queue.Count;
                for ( int i = 0; i < count; i++ )
                {
                    TimerInstance timerInstance = queue.Dequeue ();
                    if ( !timerInstance.Destroyed )
                    {
                        timerInstance.Invoke ( now );
                    }
                }
            }
        }

        internal TimerInstance AddTimer ( int repetitions, float delay, Action callback, Plugin owner = null )
        {
            object @lock = Lock;
            TimerInstance result;
            lock ( @lock )
            {
                Queue<TimerInstance> pool = TimerInstance.Pool;
                TimerInstance timerInstance;
                if ( pool.Count > 0 )
                {
                    timerInstance = pool.Dequeue ();
                    timerInstance.Load ( this, repetitions, delay, callback, owner );
                }
                else
                {
                    timerInstance = new TimerInstance ( this, repetitions, delay, callback, owner );
                }
                InsertTimer ( timerInstance, timerInstance.ExpiresAt < Oxide.Now );
                result = timerInstance;
            }
            return result;
        }

        private void InsertTimer ( TimerInstance timer, bool in_past = false )
        {
            int num = in_past ? currentSlot : ( ( int )( timer.ExpiresAt / 0.01f ) & 511 );
            timeSlots [ num ].InsertTimer ( timer );
        }

        public TimerInstance Once ( float delay, Action callback, Plugin owner = null )
        {
            return AddTimer ( 1, delay, callback, owner );
        }

        public TimerInstance Repeat ( float delay, int reps, Action callback, Plugin owner = null )
        {
            return AddTimer ( reps, delay, callback, owner );
        }

        public TimerInstance NextFrame ( Action callback )
        {
            return AddTimer ( 1, 0f, callback, null );
        }

        internal static readonly object Lock = new object ();
        internal static readonly OxideMod Oxide = Interface.Oxide;

        public const int TimeSlots = 512;
        public const int LastTimeSlot = 511;
        public const float TickDuration = 0.01f;

        private readonly TimeSlot [] timeSlots = new TimeSlot [ 512 ];
        private readonly Queue<TimerInstance> expiredInstanceQueue = new Queue<TimerInstance> ();
        private int currentSlot;
        private double nextSlotAt = 0.009999999776482582;

        public class TimeSlot
        {
            // Token: 0x06000339 RID: 825 RVA: 0x0000DFA4 File Offset: 0x0000C1A4
            public void GetExpired ( double now, Queue<TimerInstance> queue )
            {
                TimerInstance timerInstance = FirstInstance;
                while ( timerInstance != null && ( double )timerInstance.ExpiresAt <= now )
                {
                    queue.Enqueue ( timerInstance );
                    timerInstance = timerInstance.NextInstance;
                }
            }

            // Token: 0x0600033A RID: 826 RVA: 0x0000DFD8 File Offset: 0x0000C1D8
            public void InsertTimer ( TimerInstance timer )
            {
                float expiresAt = timer.ExpiresAt;
                TimerInstance firstInstance = FirstInstance;
                TimerInstance lastInstance = LastInstance;
                TimerInstance timerInstance = firstInstance;
                if ( firstInstance != null )
                {
                    float expiresAt2 = firstInstance.ExpiresAt;
                    float expiresAt3 = lastInstance.ExpiresAt;
                    if ( expiresAt <= expiresAt2 )
                    {
                        timerInstance = firstInstance;
                    }
                    else if ( expiresAt >= expiresAt3 )
                    {
                        timerInstance = null;
                    }
                    else if ( expiresAt3 - expiresAt < expiresAt - expiresAt2 )
                    {
                        timerInstance = lastInstance;
                        for ( TimerInstance timerInstance2 = timerInstance; timerInstance2 != null; timerInstance2 = timerInstance2.PreviousInstance )
                        {
                            if ( timerInstance2.ExpiresAt <= expiresAt )
                            {
                                break;
                            }
                            timerInstance = timerInstance2;
                        }
                    }
                    else
                    {
                        while ( timerInstance != null && timerInstance.ExpiresAt <= expiresAt )
                        {
                            timerInstance = timerInstance.NextInstance;
                        }
                    }
                }
                if ( timerInstance == null )
                {
                    timer.NextInstance = null;
                    if ( lastInstance == null )
                    {
                        FirstInstance = timer;
                        LastInstance = timer;
                    }
                    else
                    {
                        lastInstance.NextInstance = timer;
                        timer.PreviousInstance = lastInstance;
                        LastInstance = timer;
                    }
                }
                else
                {
                    TimerInstance previousInstance = timerInstance.PreviousInstance;
                    if ( previousInstance == null )
                    {
                        FirstInstance = timer;
                    }
                    else
                    {
                        previousInstance.NextInstance = timer;
                    }
                    timerInstance.PreviousInstance = timer;
                    timer.PreviousInstance = previousInstance;
                    timer.NextInstance = timerInstance;
                }
                timer.Added ( this );
            }

            public int Count;
            public TimerInstance FirstInstance;
            public TimerInstance LastInstance;
        }

        // Token: 0x02000077 RID: 119
        public class TimerInstance
        {
            public int Repetitions { get; private set; }
            public float Delay { get; private set; }
            public Action Callback { get; private set; }
            public bool Destroyed { get; private set; }

            public Plugin Owner { get; private set; }

            // Token: 0x06000346 RID: 838 RVA: 0x0000E13C File Offset: 0x0000C33C
            internal TimerInstance ( Timer timer, int repetitions, float delay, Action callback, Plugin owner )
            {
                Load ( timer, repetitions, delay, callback, owner );
            }

            internal void Load ( Timer timer, int repetitions, float delay, Action callback, Plugin owner )
            {
                this.timer = timer;
                Repetitions = repetitions;
                Delay = delay;
                Callback = callback;
                ExpiresAt = Oxide.Now + delay;
                Owner = owner;
                Destroyed = false;
            }

            public void Reset ( float delay = -1f, int repetitions = 1 )
            {
                object @lock = Lock;
                lock ( @lock )
                {
                    if ( delay < 0f )
                    {
                        delay = Delay;
                    }
                    else
                    {
                        Delay = delay;
                    }
                    Repetitions = repetitions;
                    ExpiresAt = Oxide.Now + delay;
                    if ( Destroyed )
                    {
                        Destroyed = false;
                    }
                    else
                    {
                        Remove ();
                    }
                    timer.InsertTimer ( this, false );
                }
            }

            // Token: 0x06000349 RID: 841 RVA: 0x0000E27C File Offset: 0x0000C47C
            public bool Destroy ()
            {
                object @lock = Lock;
                lock ( @lock )
                {
                    if ( Destroyed )
                    {
                        return false;
                    }
                    Destroyed = true;
                    Remove ();
                }
                return true;
            }

            // Token: 0x0600034A RID: 842 RVA: 0x0000E2DC File Offset: 0x0000C4DC
            public bool DestroyToPool ()
            {
                object @lock = Lock;
                lock ( @lock )
                {
                    if ( Destroyed )
                    {
                        return false;
                    }
                    Destroyed = true;
                    Callback = null;
                    Remove ();
                    Queue<TimerInstance> pool = Pool;
                    if ( pool.Count < 5000 )
                    {
                        pool.Enqueue ( this );
                    }
                }
                return true;
            }

            // Token: 0x0600034B RID: 843 RVA: 0x0000E360 File Offset: 0x0000C560
            internal void Added ( TimeSlot time_slot )
            {
                time_slot.Count++;
                Count++;
                TimeSlot = time_slot;
            }

            // Token: 0x0600034C RID: 844 RVA: 0x0000E384 File Offset: 0x0000C584
            internal void Invoke ( float now )
            {
                if ( Repetitions > 0 )
                {
                    int num = Repetitions - 1;
                    Repetitions = num;
                    if ( num == 0 )
                    {
                        Destroy ();
                        FireCallback ();
                        return;
                    }
                }
                Remove ();
                float num2 = ExpiresAt + Delay;
                ExpiresAt = num2;
                timer.InsertTimer ( this, num2 < now );
                FireCallback ();
            }

            // Token: 0x0600034D RID: 845 RVA: 0x0000E3EC File Offset: 0x0000C5EC
            internal void Remove ()
            {
                TimeSlot timeSlot = TimeSlot;
                if ( timeSlot == null )
                {
                    return;
                }
                timeSlot.Count--;
                Count--;
                TimerInstance previousInstance = PreviousInstance;
                TimerInstance nextInstance = NextInstance;
                if ( nextInstance == null )
                {
                    timeSlot.LastInstance = previousInstance;
                }
                else
                {
                    nextInstance.PreviousInstance = previousInstance;
                }
                if ( previousInstance == null )
                {
                    timeSlot.FirstInstance = nextInstance;
                }
                else
                {
                    previousInstance.NextInstance = nextInstance;
                }
                TimeSlot = null;
                PreviousInstance = null;
                NextInstance = null;
            }

            // Token: 0x0600034E RID: 846 RVA: 0x0000E468 File Offset: 0x0000C668
            private void FireCallback ()
            {
                Plugin owner = Owner;
                if ( owner != null )
                {
                    owner.TrackStart ();
                }
                try
                {
                    Callback ();
                }
                catch ( Exception ex )
                {
                    Destroy ();
                    string text = string.Format ( "Failed to run a {0:0.00} timer", Delay );
                    if ( Owner != null )
                    {
                        text += string.Format ( " in '{0} v{1}'", Owner.Name, Owner.Version );
                    }
                    CarbonCore.Error ( text, ex );
                }
                finally
                {
                    Plugin owner2 = Owner;
                    if ( owner2 != null )
                    {
                        owner2.TrackEnd ();
                    }
                }
            }

            public const int MaxPooled = 5000;

            internal static Queue<TimerInstance> Pool = new Queue<TimerInstance> ();

            internal float ExpiresAt;

            internal TimeSlot TimeSlot;

            internal TimerInstance NextInstance;

            internal TimerInstance PreviousInstance;

            private Timer timer;
        }
    }
}