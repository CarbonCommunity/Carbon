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
		private static int StartupCallbackBeforeInitCount;
		private static long StartupBurstFirstTick;
		private static long StartupBurstLastTick;
		private static Oxide.Plugins.Timer RemainingStartupTimer;

		internal static void QueuePreServerInitializedTimers()
		{
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

			StartupBurstStopwatch.Stop();

			RemainingStartupTimer = singleton.timer.In(60f, () =>
			{
				// This timer should be destroyed by the test before it naturally fires
			});
		}

		[Integrations.Test.Assert]
		public void pre_init_due_timers_fire_before_server_initialized(Integrations.Test.Assert test)
		{
			test.Log($"[tests.timer.startup] scheduled={StartupBurstCount + 1:n0} immediate={StartupImmediateCount:n0} burst={StartupBurstCountActual:n0} beforeInit={StartupCallbackBeforeInitCount:n0} scheduleElapsed={StartupBurstStopwatch.Elapsed.TotalMilliseconds:0.000}ms callbackSpan={GetStartupBurstCallbackSpanMilliseconds():0.000}ms");
			test.IsTrue(StartupImmediateCount == 1, "single immediate pre-init timer fired once");
			test.IsTrue(StartupBurstCountActual == StartupBurstCount, "pre-init burst timers fired once");
			test.IsTrue(StartupCallbackBeforeInitCount == StartupBurstCount + 1, "due pre-init timers fired before server initialized");
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
