using System.Diagnostics;
using System.Threading;
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
