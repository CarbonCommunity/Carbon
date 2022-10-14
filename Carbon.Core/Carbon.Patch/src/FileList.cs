using System.Collections.Generic;

namespace Carbon.Patch
{
	internal partial class Program
	{
		private static Dictionary<string, string> commonList = new Dictionary<string, string>
		{
			{ "%BASE%/Carbon.Core/Carbon/bin/%TARGET%/Carbon.dll", "%BASE%/Release/Carbon.dll" },
			{ "%BASE%/Carbon.Core/Carbon/bin/%TARGET%Unix/Carbon.dll", "%BASE%/Release/Carbon-Unix.dll" },
		};

		private static Dictionary<string, string> windowsList = new Dictionary<string, string>
		{
			// carbon
			{ "%BASE%/Carbon.Core/Carbon/bin/%TARGET%/Carbon.dll", "HarmonyMods/Carbon.dll" },
			{ "%BASE%/Carbon.Core/Carbon.Doorstop/bin/%TARGET%/Carbon.Doorstop.dll", "RustDedicated_Data/Managed/Carbon.Doorstop.dll" },

			// tools
			{ "%BASE%/Tools/Helpers/doorstop_config.ini", "doorstop_config.ini" },
			{ "%BASE%/Tools/UnityDoorstop/windows/x64/doorstop.dll", "winhttp.dll" }
		};

		private static Dictionary<string, string> unixList = new Dictionary<string, string>
		{
			// carbon
			{ "%BASE%/Tools/Helpers/publicizer.sh", "carbon/tools/publicizer.sh" },
			{ "%BASE%/Carbon.Core/Carbon/bin/%TARGET%Unix/Carbon.dll", "HarmonyMods/Carbon-Unix.dll" },
			{ "%BASE%/Carbon.Core/Carbon.Doorstop/bin/%TARGET%Unix/Carbon.Doorstop.dll", "HarmonyMods/Carbon.Doorstop.dll" },
		};
	}
}
