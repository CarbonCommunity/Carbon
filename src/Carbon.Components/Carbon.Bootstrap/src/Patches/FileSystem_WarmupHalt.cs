using System;
using System.Collections;
using System.Threading;
using Carbon;
using Carbon.Core;
using HarmonyLib;

namespace Patches;

[HarmonyPatch(typeof(FileSystem_Warmup), nameof(FileSystem_Warmup.Run), typeof(Action<string>), typeof(string), typeof(CancellationToken))]
internal static class FileSystem_WarmupHalt
{
	internal static bool IsReady = false;
	internal static bool AllowNative = false;

	public static IEnumerator Process(Action<string> statusFunction = null, string format = null)
	{
		while (!IsReady || !ModLoader.IsBatchComplete)
		{
			yield return null;
		}

		AllowNative = true;

		yield return FileSystem_Warmup.Run(statusFunction, format);
	}

	public static bool Prefix(Action<string> statusFunction, string format, ref IEnumerator __result)
	{
		if (AllowNative || (IsReady && ModLoader.IsBatchComplete))
		{
			return true;
		}

		__result = Process(statusFunction, format);
		return false;
	}
}
