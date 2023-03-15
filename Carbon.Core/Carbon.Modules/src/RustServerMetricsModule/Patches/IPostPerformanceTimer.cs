using API.Hooks;
using Network;
using Newtonsoft.Json;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class RustServerMetricsModule
{
	[HookAttribute.Patch("IPostPerformanceTimer", "IPostPerformanceTimer", typeof(Performance), "FPSTimer", new System.Type[] { })]
	[HookAttribute.Identifier("9c3c21e2e5bd4523aa8ba6715dbac016")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class Performance_FPSTimer_9c3c21e2e5bd4523aa8ba6715dbac016 : API.Hooks.Patch
	{
		public static void Postfix()
		{
			MetricsLogger.Instance?.OnPerformanceReportGenerated();
		}
	}
}
