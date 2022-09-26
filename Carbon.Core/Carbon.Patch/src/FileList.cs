using System.Collections.Generic;

namespace Carbon.Patch
{
	internal partial class Program
	{
		private static Dictionary<string, string> commonList = new Dictionary<string, string>
		{
			{ "%BASE%/Carbon.Core/Carbon/bin/%TARGET%/net48/Carbon.dll", "%BASE%/Release/Carbon.dll" },
			{ "%BASE%/Carbon.Core/Carbon/bin/%TARGET%Unix/net48/Carbon.dll", "%BASE%/Release/Carbon-Unix.dll" },
		};

		private static Dictionary<string, string> windowsList = new Dictionary<string, string>
		{
			// carbon
			{ "%BASE%/Carbon.Core/Carbon/bin/%TARGET%/net48/Carbon.dll", "HarmonyMods/Carbon.dll" },
			{ "%BASE%/Carbon.Core/Carbon.Doorstop/bin/%TARGET%/net48/Carbon.Doorstop.dll", "RustDedicated_Data/Managed/Carbon.Doorstop.dll" },

			// tools
			{ "%BASE%/Tools/Helpers/doorstop_config.ini", "doorstop_config.ini" },
			{ "%BASE%/Tools/UnityDoorstop/windows/x64/doorstop.dll", "winhttp.dll" },
			{ "%BASE%/Tools/NStrip/NStrip/bin/Release/net452/NStrip.exe", "carbon/tools/NStrip.exe" },
		};

		private static Dictionary<string, string> unixList = new Dictionary<string, string>
		{
			// carbon
			{ "%BASE%/Tools/Helpers/linux_prepatch.sh", "carbon_prepatch.sh" },
			{ "%BASE%/Carbon.Core/Carbon/bin/%TARGET%Unix/net48/Carbon.dll", "HarmonyMods/Carbon.dll" },

			// tools
			{ "%BASE%/Tools/NStrip/NStrip/bin/Release/net452/NStrip.exe", "carbon/tools/NStrip.exe" },
		};
	}
}