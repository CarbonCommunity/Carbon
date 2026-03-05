using Carbon.Compat.Lib;
using JetBrains.Annotations;
using Oxide.Core;

namespace Carbon.Compat.Legacy.EventCompat;

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

public static class OxideEvents
{
	[Obsolete(OxideCompat.LEGACY_MSG)]
	[UsedImplicitly]
	public class Event
	{
		public delegate void Action<in T1, in T2, in T3, in T4, in T5>(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4);

		public class Callback
		{
			public Action Invoke;

			internal Callback Previous;

			internal Callback Next;

			internal Event Handler;

			public Callback(Action callback)
			{
				Invoke = callback;
			}

			public void Call()
			{
				Action invoke = Invoke;
				if (invoke == null)
				{
					return;
				}

				try
				{
					invoke();
				}
				catch (Exception ex)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", ex);
				}
			}

			public void Remove()
			{
				Event handler = Handler;
				Callback next = Next;
				Callback previous = Previous;
				if (previous == null)
				{
					handler.First = next;
				}
				else
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}

				if (next == null)
				{
					handler.Last = previous;
				}
				else
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}

				if (handler.Invoking)
				{
					handler.RemovedQueue.Enqueue(this);
				}
				else
				{
					Previous = null;
					Next = null;
				}

				Invoke = null;
				Handler = null;
			}
		}

		public class Callback<T>
		{
			public Action<T> Invoke;

			internal Callback<T> Previous;

			internal Callback<T> Next;

			internal Event<T> Handler;

			public Callback(Action<T> callback)
			{
				Invoke = callback;
			}

			public void Call(T arg0)
			{
				Action<T> invoke = Invoke;
				if (invoke == null)
				{
					return;
				}

				try
				{
					invoke(arg0);
				}
				catch (Exception ex)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", ex);
				}
			}

			public void Remove()
			{
				Event<T> handler = Handler;
				Callback<T> next = Next;
				Callback<T> previous = Previous;
				if (previous == null)
				{
					handler.First = next;
				}
				else
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}

				if (next == null)
				{
					handler.Last = previous;
				}
				else
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}

				if (handler.Invoking)
				{
					handler.RemovedQueue.Enqueue(this);
				}
				else
				{
					Previous = null;
					Next = null;
				}

				Invoke = null;
				Handler = null;
			}
		}

		public class Callback<T1, T2>
		{
			public Action<T1, T2> Invoke;

			internal Callback<T1, T2> Previous;

			internal Callback<T1, T2> Next;

			internal Event<T1, T2> Handler;

			public Callback(Action<T1, T2> callback)
			{
				Invoke = callback;
			}

			public void Call(T1 arg0, T2 arg1)
			{
				Action<T1, T2> invoke = Invoke;
				if (invoke == null)
				{
					return;
				}

				try
				{
					invoke(arg0, arg1);
				}
				catch (Exception ex)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", ex);
				}
			}

			public void Remove()
			{
				Event<T1, T2> handler = Handler;
				Callback<T1, T2> next = Next;
				Callback<T1, T2> previous = Previous;
				if (previous == null)
				{
					handler.First = next;
				}
				else
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}

				if (next == null)
				{
					handler.Last = previous;
				}
				else
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}

				if (handler.Invoking)
				{
					handler.RemovedQueue.Enqueue(this);
				}
				else
				{
					Previous = null;
					Next = null;
				}

				Invoke = null;
				Handler = null;
			}
		}

		public class Callback<T1, T2, T3>
		{
			public Action<T1, T2, T3> Invoke;

			internal Callback<T1, T2, T3> Previous;

			internal Callback<T1, T2, T3> Next;

			internal Event<T1, T2, T3> Handler;

			public Callback(Action<T1, T2, T3> callback)
			{
				Invoke = callback;
			}

			public void Call(T1 arg0, T2 arg1, T3 arg2)
			{
				Action<T1, T2, T3> invoke = Invoke;
				if (invoke == null)
				{
					return;
				}

				try
				{
					invoke(arg0, arg1, arg2);
				}
				catch (Exception ex)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", ex);
				}
			}

			public void Remove()
			{
				Event<T1, T2, T3> handler = Handler;
				Callback<T1, T2, T3> next = Next;
				Callback<T1, T2, T3> previous = Previous;
				if (previous == null)
				{
					handler.First = next;
				}
				else
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}

				if (next == null)
				{
					handler.Last = previous;
				}
				else
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}

				if (handler.Invoking)
				{
					handler.RemovedQueue.Enqueue(this);
				}
				else
				{
					Previous = null;
					Next = null;
				}

				Invoke = null;
				Handler = null;
			}
		}

		public class Callback<T1, T2, T3, T4>
		{
			public Action<T1, T2, T3, T4> Invoke;

			internal Callback<T1, T2, T3, T4> Previous;

			internal Callback<T1, T2, T3, T4> Next;

			internal Event<T1, T2, T3, T4> Handler;

			public Callback(Action<T1, T2, T3, T4> callback)
			{
				Invoke = callback;
			}

			public void Call(T1 arg0, T2 arg1, T3 arg2, T4 arg3)
			{
				Action<T1, T2, T3, T4> invoke = Invoke;
				if (invoke == null)
				{
					return;
				}

				try
				{
					invoke(arg0, arg1, arg2, arg3);
				}
				catch (Exception ex)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", ex);
				}
			}

			public void Remove()
			{
				Event<T1, T2, T3, T4> handler = Handler;
				Callback<T1, T2, T3, T4> next = Next;
				Callback<T1, T2, T3, T4> previous = Previous;
				if (previous == null)
				{
					handler.First = next;
				}
				else
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}

				if (next == null)
				{
					handler.Last = previous;
				}
				else
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}

				if (handler.Invoking)
				{
					handler.RemovedQueue.Enqueue(this);
				}
				else
				{
					Previous = null;
					Next = null;
				}

				Invoke = null;
				Handler = null;
			}
		}

		public class Callback<T1, T2, T3, T4, T5>
		{
			public Action<T1, T2, T3, T4, T5> Invoke;

			internal Callback<T1, T2, T3, T4, T5> Previous;

			internal Callback<T1, T2, T3, T4, T5> Next;

			internal Event<T1, T2, T3, T4, T5> Handler;

			public Callback(Action<T1, T2, T3, T4, T5> callback)
			{
				Invoke = callback;
			}

			public void Call(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4)
			{
				Action<T1, T2, T3, T4, T5> invoke = Invoke;
				if (invoke == null)
				{
					return;
				}

				try
				{
					invoke(arg0, arg1, arg2, arg3, arg4);
				}
				catch (Exception ex)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", ex);
				}
			}

			public void Remove()
			{
				Event<T1, T2, T3, T4, T5> handler = Handler;
				Callback<T1, T2, T3, T4, T5> next = Next;
				Callback<T1, T2, T3, T4, T5> previous = Previous;
				if (previous == null)
				{
					handler.First = next;
				}
				else
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}

				if (next == null)
				{
					handler.Last = previous;
				}
				else
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}

				if (handler.Invoking)
				{
					handler.RemovedQueue.Enqueue(this);
				}
				else
				{
					Previous = null;
					Next = null;
				}

				Invoke = null;
				Handler = null;
			}
		}

		public Callback First;

		public Callback Last;

		internal object Lock = new object();

		internal bool Invoking;

		internal Queue<Callback> RemovedQueue = new Queue<Callback>();

		public static void Remove(ref Callback callback)
		{
			if (callback != null)
			{
				callback.Remove();
				callback = null;
			}
		}

		public static void Remove<T1>(ref Callback<T1> callback)
		{
			if (callback != null)
			{
				callback.Remove();
				callback = null;
			}
		}

		public static void Remove<T1, T2>(ref Callback<T1, T2> callback)
		{
			if (callback != null)
			{
				callback.Remove();
				callback = null;
			}
		}

		public static void Remove<T1, T2, T3>(ref Callback<T1, T2, T3> callback)
		{
			if (callback != null)
			{
				callback.Remove();
				callback = null;
			}
		}

		public static void Remove<T1, T2, T3, T4>(ref Callback<T1, T2, T3, T4> callback)
		{
			if (callback != null)
			{
				callback.Remove();
				callback = null;
			}
		}

		public static void Remove<T1, T2, T3, T4, T5>(ref Callback<T1, T2, T3, T4, T5> callback)
		{
			if (callback != null)
			{
				callback.Remove();
				callback = null;
			}
		}

		public void Add(Callback callback)
		{
			callback.Handler = this;
			lock (Lock)
			{
				Callback last = Last;
				if (last == null)
				{
					First = callback;
					Last = callback;
				}
				else
				{
					last.Next = callback;
					callback.Previous = last;
					Last = callback;
				}
			}
		}

		public Callback Add(Action callback)
		{
			Callback callback2 = new Callback(callback);
			Add(callback2);
			return callback2;
		}

		public void Invoke()
		{
			lock (Lock)
			{
				Invoking = true;
				for (Callback callback = First; callback != null; callback = callback.Next)
				{
					callback.Call();
				}

				Invoking = false;
				Queue<Callback> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Callback callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}
	}

	[Obsolete(OxideCompat.LEGACY_MSG)]
	[UsedImplicitly]
	public class Event<T>
	{
		public Event.Callback<T> First;

		public Event.Callback<T> Last;

		internal object Lock = new object();

		internal bool Invoking;

		internal Queue<Event.Callback<T>> RemovedQueue = new Queue<Event.Callback<T>>();

		public void Add(Event.Callback<T> callback)
		{
			callback.Handler = this;
			lock (Lock)
			{
				Event.Callback<T> last = Last;
				if (last == null)
				{
					First = callback;
					Last = callback;
				}
				else
				{
					last.Next = callback;
					callback.Previous = last;
					Last = callback;
				}
			}
		}

		public Event.Callback<T> Add(Action<T> callback)
		{
			Event.Callback<T> callback2 = new Event.Callback<T>(callback);
			Add(callback2);
			return callback2;
		}

		public void Invoke(T arg0)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0);
				}

				Invoking = false;
				Queue<Event.Callback<T>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}
	}

	[Obsolete(OxideCompat.LEGACY_MSG)]
	[UsedImplicitly]
	public class Event<T1, T2>
	{
		public Event.Callback<T1, T2> First;

		public Event.Callback<T1, T2> Last;

		internal object Lock = new object();

		internal bool Invoking;

		internal Queue<Event.Callback<T1, T2>> RemovedQueue = new Queue<Event.Callback<T1, T2>>();

		public void Add(Event.Callback<T1, T2> callback)
		{
			callback.Handler = this;
			lock (Lock)
			{
				Event.Callback<T1, T2> last = Last;
				if (last == null)
				{
					First = callback;
					Last = callback;
				}
				else
				{
					last.Next = callback;
					callback.Previous = last;
					Last = callback;
				}
			}
		}

		public Event.Callback<T1, T2> Add(Action<T1, T2> callback)
		{
			Event.Callback<T1, T2> callback2 = new Event.Callback<T1, T2>(callback);
			Add(callback2);
			return callback2;
		}

		public void Invoke()
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(default(T1), default(T2));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, default(T2));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1);
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}
	}

	[Obsolete(OxideCompat.LEGACY_MSG)]
	[UsedImplicitly]
	public class Event<T1, T2, T3>
	{
		public Event.Callback<T1, T2, T3> First;

		public Event.Callback<T1, T2, T3> Last;

		internal object Lock = new object();

		internal bool Invoking;

		internal Queue<Event.Callback<T1, T2, T3>> RemovedQueue = new Queue<Event.Callback<T1, T2, T3>>();

		public void Add(Event.Callback<T1, T2, T3> callback)
		{
			callback.Handler = this;
			lock (Lock)
			{
				Event.Callback<T1, T2, T3> last = Last;
				if (last == null)
				{
					First = callback;
					Last = callback;
				}
				else
				{
					last.Next = callback;
					callback.Previous = last;
					Last = callback;
				}
			}
		}

		public Event.Callback<T1, T2, T3> Add(Action<T1, T2, T3> callback)
		{
			Event.Callback<T1, T2, T3> callback2 = new Event.Callback<T1, T2, T3>(callback);
			Add(callback2);
			return callback2;
		}

		public void Invoke()
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3> callback = First; callback != null; callback = callback.Next)
				{
					callback.Invoke(default(T1), default(T2), default(T3));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, default(T2), default(T3));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, default(T3));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1, T3 arg2)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, arg2);
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}
	}

	[Obsolete(OxideCompat.LEGACY_MSG)]
	[UsedImplicitly]
	public class Event<T1, T2, T3, T4>
	{
		public Event.Callback<T1, T2, T3, T4> First;

		public Event.Callback<T1, T2, T3, T4> Last;

		internal object Lock = new object();

		internal bool Invoking;

		internal Queue<Event.Callback<T1, T2, T3, T4>> RemovedQueue = new Queue<Event.Callback<T1, T2, T3, T4>>();

		public void Add(Event.Callback<T1, T2, T3, T4> callback)
		{
			callback.Handler = this;
			lock (Lock)
			{
				Event.Callback<T1, T2, T3, T4> last = Last;
				if (last == null)
				{
					First = callback;
					Last = callback;
				}
				else
				{
					last.Next = callback;
					callback.Previous = last;
					Last = callback;
				}
			}
		}

		public Event.Callback<T1, T2, T3, T4> Add(Action<T1, T2, T3, T4> callback)
		{
			Event.Callback<T1, T2, T3, T4> callback2 = new Event.Callback<T1, T2, T3, T4>(callback);
			Add(callback2);
			return callback2;
		}

		public void Invoke()
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3, T4> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(default(T1), default(T2), default(T3), default(T4));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3, T4> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, default(T2), default(T3), default(T4));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3, T4> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, default(T3), default(T4));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1, T3 arg2)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3, T4> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, arg2, default(T4));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1, T3 arg2, T4 arg3)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3, T4> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, arg2, arg3);
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}
	}

	[Obsolete(OxideCompat.LEGACY_MSG)]
	[UsedImplicitly]
	public class Event<T1, T2, T3, T4, T5>
	{
		public Event.Callback<T1, T2, T3, T4, T5> First;

		public Event.Callback<T1, T2, T3, T4, T5> Last;

		internal object Lock = new object();

		internal bool Invoking;

		internal Queue<Event.Callback<T1, T2, T3, T4, T5>> RemovedQueue =
			new Queue<Event.Callback<T1, T2, T3, T4, T5>>();

		public void Add(Event.Callback<T1, T2, T3, T4, T5> callback)
		{
			callback.Handler = this;
			lock (Lock)
			{
				Event.Callback<T1, T2, T3, T4, T5> last = Last;
				if (last == null)
				{
					First = callback;
					Last = callback;
				}
				else
				{
					last.Next = callback;
					callback.Previous = last;
					Last = callback;
				}
			}
		}

		public Event.Callback<T1, T2, T3, T4, T5> Add(Event.Action<T1, T2, T3, T4, T5> callback)
		{
			Event.Callback<T1, T2, T3, T4, T5> callback2 = new Event.Callback<T1, T2, T3, T4, T5>(callback);
			Add(callback2);
			return callback2;
		}

		public void Invoke()
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3, T4, T5> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(default(T1), default(T2), default(T3), default(T4), default(T5));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4, T5> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3, T4, T5> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, default(T2), default(T3), default(T4), default(T5));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4, T5> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3, T4, T5> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, default(T3), default(T4), default(T5));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4, T5> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1, T3 arg2)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3, T4, T5> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, arg2, default(T4), default(T5));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4, T5> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1, T3 arg2, T4 arg3)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3, T4, T5> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, arg2, arg3, default(T5));
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4, T5> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		public void Invoke(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4)
		{
			lock (Lock)
			{
				Invoking = true;
				for (Event.Callback<T1, T2, T3, T4, T5> callback = First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, arg2, arg3, arg4);
				}

				Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4, T5> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}
	}
}
