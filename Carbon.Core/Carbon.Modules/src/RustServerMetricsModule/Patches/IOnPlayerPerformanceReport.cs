using API.Hooks;
using Newtonsoft.Json;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;
#pragma warning disable IDE0051
#pragma warning disable IDE0060

public partial class RustServerMetricsModule
{
	[HookAttribute.Patch("IOnPlayerPerformanceReport", "IOnPlayerPerformanceReport", typeof(BasePlayer), "PerformanceReport", new System.Type[] { })]
	[HookAttribute.Identifier("8af02d123d8b47ada6099cbe8cbd882c")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class BasePlayer_PerformanceReport_8af02d123d8b47ada6099cbe8cbd882c : API.Hooks.Patch
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, BasePlayer __instance)
		{
			if (MetricsLogger.Instance == null) return true;

			var oldPosition = msg.read.Position;
			msg.read.String();
			var data = msg.read.StringRaw();

			MetricsLogger.Instance?.OnClientPerformanceReport(JsonConvert.DeserializeObject<ClientPerformanceReport>(data));

			msg.read.Position = oldPosition;
			return true;
		}
	}
}
