using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Carbon.Test;

namespace Carbon.Plugins;

public partial class Tests
{
	public class TimerTests
	{
		private const int StartupBurstCount = 512;
		private static readonly Stopwatch StartupBurstStopwatch = new();
		private static int StartupImmediateCount;
		private static int StartupBurstCountActual;
		private static int StartupRepeatOnceCount;
		private static int StartupCallbackBeforeInitCount;
		private static bool StartupTimersQueuedBeforeInit;
		private static long StartupBurstFirstTick;
		private static long StartupBurstLastTick;
		private static Oxide.Plugins.Timer RemainingStartupTimer;
		private static Oxide.Plugins.Timer RemainingStartupEveryTimer;
		private static Oxide.Plugins.Timer RemainingStartupRepeatTimer;

		internal static void QueuePreServerInitializedTimers()
		{
			StartupTimersQueuedBeforeInit = !Community.IsServerInitialized;
			StartupBurstStopwatch.Restart();

			singleton.timer.In(0f, () =>
			{
				Interlocked.Increment(ref StartupImmediateCount);
				if (!Community.IsServerInitialized)
				{
					Interlocked.Increment(ref StartupCallbackBeforeInitCount);
				}
			});

			for (var i = 0; i < StartupBurstCount; i++)
			{
				singleton.timer.In(0f, () =>
				{
					var timestamp = Stopwatch.GetTimestamp();
					Interlocked.CompareExchange(ref StartupBurstFirstTick, timestamp, 0);
					Interlocked.Exchange(ref StartupBurstLastTick, timestamp);
					Interlocked.Increment(ref StartupBurstCountActual);

					if (!Community.IsServerInitialized)
					{
						Interlocked.Increment(ref StartupCallbackBeforeInitCount);
					}
				});
			}

			singleton.timer.Repeat(0f, 1, () =>
			{
				Interlocked.Increment(ref StartupRepeatOnceCount);
				if (!Community.IsServerInitialized)
				{
					Interlocked.Increment(ref StartupCallbackBeforeInitCount);
				}
			});

			StartupBurstStopwatch.Stop();

			RemainingStartupTimer = singleton.timer.In(60f, () =>
			{
				// This timer should be destroyed by the test before it naturally fires
			});

			RemainingStartupEveryTimer = singleton.timer.Every(60f, () =>
			{
				// This timer should be destroyed by the test before it naturally fires
			});

			RemainingStartupRepeatTimer = singleton.timer.Repeat(60f, 3, () =>
			{
				// This timer should be destroyed by the test before it naturally fires
			});
		}

		[Integrations.Test.Assert]
		public void pre_init_due_timers_fire_before_server_initialized(Integrations.Test.Assert test)
		{
			test.Log($"[tests.timer.startup] scheduled={StartupBurstCount + 2:n0} queuedBeforeInit={StartupTimersQueuedBeforeInit} immediate={StartupImmediateCount:n0} burst={StartupBurstCountActual:n0} repeatOnce={StartupRepeatOnceCount:n0} beforeInit={StartupCallbackBeforeInitCount:n0} scheduleElapsed={StartupBurstStopwatch.Elapsed.TotalMilliseconds:0.000}ms callbackSpan={GetStartupBurstCallbackSpanMilliseconds():0.000}ms");
			test.IsTrue(StartupImmediateCount == 1, "single immediate pre-init timer fired once");
			test.IsTrue(StartupBurstCountActual == StartupBurstCount, "pre-init burst timers fired once");
			test.IsTrue(StartupRepeatOnceCount == 1, "single repeat pre-init timer fired once");

			if (StartupTimersQueuedBeforeInit)
			{
				test.IsTrue(StartupCallbackBeforeInitCount == 0 || StartupCallbackBeforeInitCount == StartupBurstCount + 2, "due pre-init timers fired consistently around server initialization");
			}
			test.Complete();
		}

		[Integrations.Test.Assert]
		public void pre_init_remaining_timer_converts_to_invoke(Integrations.Test.Assert test)
		{
			test.IsNotNull(RemainingStartupTimer, "remaining pre-init timer instance");
			test.IsNotNull(RemainingStartupTimer?.Persistence, "remaining pre-init timer persistence");
			test.IsNotNull(RemainingStartupTimer?.Callback, "remaining pre-init timer callback");

			if (RemainingStartupTimer?.Persistence != null && RemainingStartupTimer.Callback != null)
			{
				var callback = RemainingStartupTimer.Callback;
				test.IsTrue(RemainingStartupTimer.Persistence.IsInvoking(callback), "remaining pre-init timer converted to invoke");
				RemainingStartupTimer.Destroy();
				test.IsFalse(RemainingStartupTimer.Persistence.IsInvoking(callback), "converted pre-init timer destroy cancels invoke");
			}

			test.IsNotNull(RemainingStartupEveryTimer, "remaining pre-init every timer instance");
			test.IsNotNull(RemainingStartupEveryTimer?.Persistence, "remaining pre-init every timer persistence");
			test.IsNotNull(RemainingStartupEveryTimer?.Callback, "remaining pre-init every timer callback");

			if (RemainingStartupEveryTimer?.Persistence != null && RemainingStartupEveryTimer.Callback != null)
			{
				var callback = RemainingStartupEveryTimer.Callback;
				test.IsTrue(RemainingStartupEveryTimer.Persistence.IsInvoking(callback), "remaining pre-init every timer converted to invoke");
				RemainingStartupEveryTimer.Destroy();
				test.IsFalse(RemainingStartupEveryTimer.Persistence.IsInvoking(callback), "converted pre-init every timer destroy cancels invoke");
			}

			test.IsNotNull(RemainingStartupRepeatTimer, "remaining pre-init repeat timer instance");
			test.IsNotNull(RemainingStartupRepeatTimer?.Persistence, "remaining pre-init repeat timer persistence");
			test.IsNotNull(RemainingStartupRepeatTimer?.Callback, "remaining pre-init repeat timer callback");

			if (RemainingStartupRepeatTimer?.Persistence != null && RemainingStartupRepeatTimer.Callback != null)
			{
				var callback = RemainingStartupRepeatTimer.Callback;
				test.IsTrue(RemainingStartupRepeatTimer.Persistence.IsInvoking(callback), "remaining pre-init repeat timer converted to invoke");
				RemainingStartupRepeatTimer.Destroy();
				test.IsFalse(RemainingStartupRepeatTimer.Persistence.IsInvoking(callback), "converted pre-init repeat timer destroy cancels invoke");
			}

			test.Complete();
		}

		[Integrations.Test.Assert]
		public void clear_destroys_existing_timers_and_keeps_timer_set_reusable(Integrations.Test.Assert test)
		{
			var timers = new Oxide.Plugins.Timers(singleton);
			var first = timers.In(60f, () => { });
			var firstCallback = first?.Callback;

			test.IsNotNull(first, "first timer instance");
			test.IsNotNull(firstCallback, "first timer callback");
			if (first == null || firstCallback == null)
			{
				test.Complete();
				return;
			}

			timers.Clear();

			test.IsTrue(first.Destroyed, "Clear destroys existing timer");
			test.IsFalse(first.Persistence.IsInvoking(firstCallback), "Clear cancels existing timer invoke");

			var second = timers.In(60f, () => { });
			var secondCallback = second?.Callback;

			test.IsNotNull(second, "timer can be created after Clear");
			test.IsNotNull(secondCallback, "new timer callback after Clear");
			if (second == null || secondCallback == null)
			{
				test.Complete();
				return;
			}

			test.IsTrue(second.Persistence.IsInvoking(secondCallback), "new timer after Clear is scheduled");

			timers.DestroyAll();

			test.IsTrue(second.Destroyed, "DestroyAll destroys timer created after Clear");
			test.IsFalse(second.Persistence.IsInvoking(secondCallback), "DestroyAll cancels timer created after Clear");
			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 5_000)]
		public void destroy_all_handles_stale_destroyed_timer_entries(Integrations.Test.Assert test)
		{
			var timers = new Oxide.Plugins.Timers(singleton);
			var timer = timers.In(60f, () => { });
			var callback = timer?.Callback;

			test.IsNotNull(timer, "timer instance");
			test.IsNotNull(callback, "timer callback");
			if (timer == null || callback == null)
			{
				test.Complete();
				return;
			}

			timer.Destroyed = true;
			timers.DestroyAll();

			test.IsTrue(timer.Destroyed, "stale destroyed timer remains destroyed");
			test.IsNull(timer.Callback, "stale destroyed timer callback is cleared");
			test.IsFalse(timer.Persistence.IsInvoking(callback), "stale destroyed timer invoke is cancelled");
			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 5_000)]
		public async Task in_timer_destroys_and_untracks_after_firing_once(Integrations.Test.Assert test)
		{
			var timers = new Oxide.Plugins.Timers(singleton);
			var firedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			var fired = 0;

			var timer = timers.In(0.05f, () =>
			{
				Interlocked.Increment(ref fired);
				firedTcs.TrySetResult(true);
			});

			var callback = timer?.Callback;
			test.IsNotNull(timer, "In timer instance");
			test.IsNotNull(callback, "In timer callback");
			if (timer == null || callback == null)
			{
				test.Complete();
				return;
			}

			var completed = await Task.WhenAny(firedTcs.Task, Task.Delay(2_000));

			test.IsTrue(completed == firedTcs.Task, "In timer fired");
			test.IsTrue(fired == 1, "In timer fired once");
			test.IsTrue(timer.Destroyed, "In timer destroyed after firing");
			test.IsNull(timer.Callback, "In timer callback cleared after firing");
			test.IsFalse(timer.Persistence.IsInvoking(callback), "In timer invoke cancelled after firing");
			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 5_000)]
		public async Task completed_in_timer_can_be_reset(Integrations.Test.Assert test)
		{
			var timers = new Oxide.Plugins.Timers(singleton);
			var firstFireTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			var secondFireTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			var fired = 0;

			var timer = timers.In(0.05f, () =>
			{
				if (Interlocked.Increment(ref fired) == 1)
				{
					firstFireTcs.TrySetResult(true);
				}
				else
				{
					secondFireTcs.TrySetResult(true);
				}
			});

			test.IsNotNull(timer, "In timer instance");
			if (timer == null)
			{
				test.Complete();
				return;
			}

			var firstCompleted = await Task.WhenAny(firstFireTcs.Task, Task.Delay(2_000));

			test.IsTrue(firstCompleted == firstFireTcs.Task, "In timer fired before reset");
			test.IsTrue(timer.Destroyed, "In timer destroyed after first fire");

			timer.Reset(0.05f);
			test.IsFalse(timer.Destroyed, "Reset revives completed In timer");

			var secondCompleted = await Task.WhenAny(secondFireTcs.Task, Task.Delay(2_000));

			test.IsTrue(secondCompleted == secondFireTcs.Task, "reset In timer fired");
			test.IsTrue(fired == 2, "reset In timer fired once more");
			test.IsTrue(timer.Destroyed, "reset In timer destroyed after firing");
			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 5_000)]
		public async Task repeat_timer_destroys_and_untracks_after_final_repetition(Integrations.Test.Assert test)
		{
			var timers = new Oxide.Plugins.Timers(singleton);
			var completedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			var fired = 0;

			var timer = timers.Repeat(0.05f, 2, () =>
			{
				if (Interlocked.Increment(ref fired) == 2)
				{
					completedTcs.TrySetResult(true);
				}
			});

			var callback = timer?.Callback;
			test.IsNotNull(timer, "Repeat timer instance");
			test.IsNotNull(callback, "Repeat timer callback");
			if (timer == null || callback == null)
			{
				test.Complete();
				return;
			}

			var completed = await Task.WhenAny(completedTcs.Task, Task.Delay(2_000));

			test.IsTrue(completed == completedTcs.Task, "Repeat timer completed expected repetitions");
			test.IsTrue(fired == 2, "Repeat timer fired twice");
			test.IsTrue(timer.Destroyed, "Repeat timer destroyed after final repetition");
			test.IsNull(timer.Callback, "Repeat timer callback cleared after final repetition");
			test.IsFalse(timer.Persistence.IsInvoking(callback), "Repeat timer invoke cancelled after final repetition");

			await Task.Delay(150);

			test.IsTrue(fired == 2, "Repeat timer did not fire after final repetition");
			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 5_000)]
		public async Task repeat_timer_reset_inside_final_callback_survives_completion(Integrations.Test.Assert test)
		{
			var timers = new Oxide.Plugins.Timers(singleton);
			var resetFireTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			var fired = 0;
			Oxide.Plugins.Timer timer = null;

			timer = timers.Repeat(0.05f, 2, () =>
			{
				var count = Interlocked.Increment(ref fired);
				if (count == 2)
				{
					timer.Reset(0.05f);
				}
				else if (count == 3)
				{
					resetFireTcs.TrySetResult(true);
				}
			});

			test.IsNotNull(timer, "Repeat timer instance");
			if (timer == null)
			{
				test.Complete();
				return;
			}

			var resetCompleted = await Task.WhenAny(resetFireTcs.Task, Task.Delay(2_000));

			test.IsTrue(resetCompleted == resetFireTcs.Task, "reset from final Repeat callback fired");
			test.IsTrue(fired == 3, "Repeat timer fired twice before reset and once after reset");
			test.IsTrue(timer.Destroyed, "reset Repeat timer destroyed after replacement fire");

			await Task.Delay(150);

			test.IsTrue(fired == 3, "Repeat timer reset did not leave original repeat running");
			test.Complete();
		}

		private static double GetStartupBurstCallbackSpanMilliseconds()
		{
			var first = Interlocked.Read(ref StartupBurstFirstTick);
			var last = Interlocked.Read(ref StartupBurstLastTick);
			if (first == 0 || last < first)
			{
				return 0;
			}

			return (last - first) * 1000d / Stopwatch.Frequency;
		}
	}
}
