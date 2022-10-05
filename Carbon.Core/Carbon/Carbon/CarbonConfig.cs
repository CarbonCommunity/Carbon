using System;

namespace Carbon.Core
{
	[Serializable]
	public class CarbonConfig
	{
		public int LogVerbosity { get; set; } = 0;
		public Logger.Severity LogSeverity { get; set; } = Logger.Severity.Notice;

		public bool CarbonTag { get; set; } = true;
		public bool IsModded { get; set; } = true;
		public bool HookTimeTracker { get; set; } = false;
		public bool HookValidation { get; set; } = true;
		public bool ScriptWatchers { get; set; } = true;
		public bool HarmonyWatchers { get; set; } = true;
		public int EntityMapBufferSize { get; set; } = 100000;
	}
}
