using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Carbon.Core;
using Carbon.Test;

namespace Carbon.Plugins;

public partial class Tests
{
	public class Logging
	{
		[Integrations.Test.Assert]
		public void validate_logger_ready(Integrations.Test.Assert test)
		{
			global::Carbon.Logger.Log("[tests.logging] validating logger state");
			test.IsNotNull(global::Carbon.Logger.CoreLog, "Carbon.Logger.CoreLog");
			test.IsTrue(global::Carbon.Logger.CoreLog.PendingCount >= 0, "PendingCount is non-negative");
			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 8_000)]
		public async Task concurrent_log_quick_stability(Integrations.Test.Assert test)
		{
			const int logsPerWorker = 24;
			var workerCount = Math.Clamp(Environment.ProcessorCount, 2, 4);
			var expected = workerCount * logsPerWorker;

			if (global::Carbon.Logger.CoreLog == null)
			{
				test.Fatal("Logger instance is null", null);
				test.Complete();
				return;
			}

			var hadConfig = Community.IsConfigReady && Community.Runtime?.Config?.Logging != null;
			var previousLogFileMode = hadConfig ? Community.Runtime.Config.Logging.LogFileMode : 0;

			var startSignal = new ManualResetEventSlim(false);
			var faults = new ConcurrentQueue<Exception>();
			var writes = 0;
			var completedWorkers = 0;

			var tasks = new Task[workerCount];

			try
			{
				if (hadConfig)
				{
					Community.Runtime.Config.Logging.LogFileMode = 2;
				}

				for (var workerIndex = 0; workerIndex < workerCount; workerIndex++)
				{
					var localWorkerIndex = workerIndex;
					tasks[workerIndex] = Task.Run(() =>
					{
						try
						{
							startSignal.Wait(5_000);

							for (var i = 0; i < logsPerWorker; i++)
							{
								var writeNumber = Interlocked.Increment(ref writes);
								var message = $"[tests.logging.quick] worker={localWorkerIndex} write={writeNumber} iter={i}";

								if (i % 10 == 0)
								{
									var ex = new InvalidOperationException($"Synthetic async fault marker [{localWorkerIndex}:{i}]");
									global::Carbon.Logger.Error(message, ex);
								}
								else
								{
									global::Carbon.Logger.Log(message);
								}
							}

							Interlocked.Increment(ref completedWorkers);
						}
						catch (Exception ex)
						{
							faults.Enqueue(ex);
						}
					});
				}

				startSignal.Set();

				var allWorkers = Task.WhenAll(tasks);
				var workersFinished = await Task.WhenAny(allWorkers, Task.Delay(6_000));

				test.IsTrue(workersFinished == allWorkers, $"workers completed within timeout ({completedWorkers}/{workerCount})");

				if (allWorkers.IsFaulted && allWorkers.Exception != null)
				{
					foreach (var ex in allWorkers.Exception.Flatten().InnerExceptions)
					{
						faults.Enqueue(ex);
					}
				}

				global::Carbon.Logger.CoreLog?.Flush();

				test.IsTrue(completedWorkers == workerCount, $"all workers completed ({completedWorkers}/{workerCount})");
				test.IsTrue(writes == expected, $"all writes were issued ({writes}/{expected})");
				test.IsTrue(faults.Count == 0, $"worker faults ({faults.Count})");
				test.IsTrue((global::Carbon.Logger.CoreLog?.PendingCount ?? 0) == 0, "flush drained queue");
				test.Complete();
			}
			finally
			{
				startSignal.Dispose();

				if (hadConfig)
				{
					Community.Runtime.Config.Logging.LogFileMode = previousLogFileMode;
				}
			}
		}

		[Integrations.Test.Assert]
		public void single_thread_basic_flush_behavior(Integrations.Test.Assert test)
		{
			const int writes = 16;

			for (var i = 0; i < writes; i++)
			{
				global::Carbon.Logger.Log($"[tests.logging.basic] write={i}");
			}

			global::Carbon.Logger.CoreLog?.Flush();

			test.IsTrue((global::Carbon.Logger.CoreLog?.PendingCount ?? 0) == 0, "single-thread flush drained queue");
			test.Complete();
		}

		/*
		[Integrations.Test.Assert(Timeout = 45_000)]
		public async Task concurrent_log_burst_with_rotation_benchmark(Integrations.Test.Assert test)
		{
			const int logsPerWorker = 120;
			const int singleThreadWrites = 320;

			var workerCount = Math.Clamp(Environment.ProcessorCount, 4, 12);
			var expectedTotalWrites = workerCount * logsPerWorker;

			var logger = global::Carbon.Logger.CoreLog;
			if (logger == null)
			{
				global::Carbon.Logger.Log("[tests.logging] priming logger instance");
				logger = global::Carbon.Logger.CoreLog;
			}

			if (logger == null)
			{
				test.Fatal("Logger instance is null", null);
				test.Complete();
				return;
			}

			var hadConfig = Community.IsConfigReady && Community.Runtime?.Config?.Logging != null;
			var previousLogFileMode = hadConfig ? Community.Runtime.Config.Logging.LogFileMode : 0;
			var previousSplitSize = logger.SplitSize;

			var startSignal = new ManualResetEventSlim(false);
			var workerThreadIds = new ConcurrentDictionary<int, byte>();
			var faults = new ConcurrentQueue<Exception>();
			var writes = 0;
			var completedWorkers = 0;

			var tasks = new Task[workerCount];
			var benchmark = Stopwatch.StartNew();

			try
			{
				if (hadConfig)
				{
					Community.Runtime.Config.Logging.LogFileMode = 2;
				}

				logger.SplitSize = 64 * 1024;

				for (var workerIndex = 0; workerIndex < workerCount; workerIndex++)
				{
					var localWorkerIndex = workerIndex;
					tasks[workerIndex] = Task.Run(() =>
					{
						try
						{
							workerThreadIds.TryAdd(Thread.CurrentThread.ManagedThreadId, 0);
							startSignal.Wait(5_000);

							for (var i = 0; i < logsPerWorker; i++)
							{
								var writeNumber = Interlocked.Increment(ref writes);
								var message = $"[tests.logging] worker={localWorkerIndex} write={writeNumber} iter={i}";

								if (i % 8 == 0)
								{
									var ex = new InvalidOperationException($"Synthetic async fault marker [{localWorkerIndex}:{i}]");
									global::Carbon.Logger.Error(message, ex);
								}
								else
								{
									global::Carbon.Logger.Log(message);
								}

								if (i % 24 == 0)
								{
									global::Carbon.Logger.CoreLog?.Flush();
								}
							}

							Interlocked.Increment(ref completedWorkers);
						}
						catch (Exception ex)
						{
							faults.Enqueue(ex);
						}
					});
				}

				startSignal.Set();

				var allWorkers = Task.WhenAll(tasks);
				var workersFinished = await Task.WhenAny(allWorkers, Task.Delay(30_000));
				benchmark.Stop();

				test.IsTrue(workersFinished == allWorkers, $"workers completed within timeout ({completedWorkers}/{workerCount})");

				if (allWorkers.IsFaulted && allWorkers.Exception != null)
				{
					foreach (var ex in allWorkers.Exception.Flatten().InnerExceptions)
					{
						faults.Enqueue(ex);
					}
				}

				global::Carbon.Logger.CoreLog?.Flush();

				var throughput = writes / Math.Max(0.001d, benchmark.Elapsed.TotalSeconds);
				test.Log($"[tests.logging] workers={workerCount} writes={writes} elapsed={benchmark.ElapsedMilliseconds}ms throughput={throughput:0} lines/sec");
				test.Log($"[tests.logging] unique worker threads={workerThreadIds.Count} faults={faults.Count}");

				test.IsTrue(completedWorkers == workerCount, $"all workers completed ({completedWorkers}/{workerCount})");
				test.IsTrue(writes == expectedTotalWrites, $"all writes were issued ({writes}/{expectedTotalWrites})");
				test.IsTrue(workerThreadIds.Count > 1, $"burst used background worker threads ({workerThreadIds.Count})");
				test.IsTrue(faults.Count == 0, $"worker faults ({faults.Count})");

				var singleThreadStartWrites = writes;
				var singleThreadBenchmark = Stopwatch.StartNew();

				for (var i = 0; i < singleThreadWrites; i++)
				{
					var writeNumber = Interlocked.Increment(ref writes);
					global::Carbon.Logger.Log($"[tests.logging.single] write={writeNumber} iter={i}");

					if (i % 64 == 0)
					{
						global::Carbon.Logger.CoreLog?.Flush();
					}
				}

				global::Carbon.Logger.CoreLog?.Flush();
				singleThreadBenchmark.Stop();

				var singleThreadDelta = writes - singleThreadStartWrites;
				var singleThreadThroughput = singleThreadDelta / Math.Max(0.001d, singleThreadBenchmark.Elapsed.TotalSeconds);
				test.Log($"[tests.logging.single] writes={singleThreadDelta} elapsed={singleThreadBenchmark.ElapsedMilliseconds}ms throughput={singleThreadThroughput:0} lines/sec");

				test.IsTrue(singleThreadDelta == singleThreadWrites, $"single-thread writes were issued ({singleThreadDelta}/{singleThreadWrites})");
				test.IsTrue((global::Carbon.Logger.CoreLog?.PendingCount ?? 0) == 0, "single-thread flush drained queue");

				test.Complete();
			}
			finally
			{
				startSignal.Dispose();

				if (hadConfig)
				{
					Community.Runtime.Config.Logging.LogFileMode = previousLogFileMode;
				}

				if (global::Carbon.Logger.CoreLog != null)
				{
					global::Carbon.Logger.CoreLog.SplitSize = previousSplitSize;
				}
			}
		}
		*/
	}
}
