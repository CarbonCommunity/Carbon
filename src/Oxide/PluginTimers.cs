/*
 The MIT License (MIT)

Copyright (c) 2013-2020 Oxide Team and Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

namespace Oxide.Plugins
{
    public class Timer
    {
        private Core.Libraries.Timer.TimerInstance instance;

        public Timer(Core.Libraries.Timer.TimerInstance instance)
        {
            this.instance = instance;
        }

        /// <summary>
        /// Gets the number of repetitions left on this timer
        /// </summary>
        public int Repetitions => instance.Repetitions;

        /// <summary>
        /// Gets the delay between each repetition
        /// </summary>
        public float Delay => instance.Delay;

        /// <summary>
        /// Gets the callback delegate
        /// </summary>
        public Action Callback => instance.Callback;

        /// <summary>
        /// Gets if this timer has been destroyed
        /// </summary>
        public bool Destroyed => instance.Destroyed;

        /// <summary>
        /// Gets the plugin to which this timer belongs, if any
        /// </summary>
        public Plugin Owner => instance.Owner;

        /// <summary>
        /// Resets the timer optionally changing the delay setting a number of repetitions
        /// </summary>
        /// <param name="delay">The new delay between repetitions</param>
        /// <param name="repetitions">Number of repetitions before being destroyed</param>
        public void Reset(float delay = -1, int repetitions = 1) => instance.Reset(delay, repetitions);

        /// <summary>
        /// Destroys this timer
        /// </summary>
        public void Destroy() => instance.Destroy();

        /// <summary>
        /// Destroys this timer and returns the instance to the pool
        /// </summary>
        public void DestroyToPool() => instance.DestroyToPool();
    }

    public class PluginTimers
    {
        private Core.Libraries.Timer timer = Interface.Oxide.GetLibrary<Core.Libraries.Timer>("Timer");
        private Plugin plugin;

        public PluginTimers(Plugin plugin)
        {
            this.plugin = plugin;
        }

        /// <summary>
        /// Creates a timer which fires once after the specified delay
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="callback"></param>
        public Timer Once(float seconds, Action callback)
        {
            return new Timer(timer.Once(seconds, callback, plugin));
        }

        /// <summary>
        /// Creates a timer which fires once after the specified delay
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="callback"></param>
        public Timer In(float seconds, Action callback)
        {
            return new Timer(timer.Once(seconds, callback, plugin));
        }

        /// <summary>
        /// Creates a timer which continuously fires at the specified interval
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="callback"></param>
        public Timer Every(float interval, Action callback)
        {
            return new Timer(timer.Repeat(interval, -1, callback, plugin));
        }

        /// <summary>
        /// Creates a timer which fires a set number of times at the specified interval
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="repeats"></param>
        /// <param name="callback"></param>
        public Timer Repeat(float interval, int repeats, Action callback)
        {
            return new Timer(timer.Repeat(interval, repeats, callback, plugin));
        }

        /// <summary>
        /// Destroys a timer, returns the instance to the pool and sets the variable to null
        /// </summary>
        /// <param name="timer"></param>
        public void Destroy(ref Timer timer)
        {
            timer?.DestroyToPool();
            timer = null;
        }
    }
}
