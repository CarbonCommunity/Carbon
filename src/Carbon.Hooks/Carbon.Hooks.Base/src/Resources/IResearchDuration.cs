#if !MINIMAL

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using API.Hooks;
using Carbon.Core;
using HarmonyLib;
using Patch = API.Hooks.Patch;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Fixes
{
	public partial class Fixes_Recycler
	{
		[HookAttribute.Patch("IResearchDuration", "IResearchDuration", typeof(ResearchTable), nameof(ResearchTable.DoResearch), new System.Type[] { typeof(BaseEntity.RPCMessage) })]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class IResearchDuration : Patch
		{
			public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
			{
				var runtime = AccessTools.PropertyGetter(typeof(Community), nameof(Community.Runtime));
				var core = AccessTools.PropertyGetter(typeof(Community), nameof(Community.Core));
				var multiplier = AccessTools.PropertyGetter(typeof(CorePlugin), nameof(CorePlugin.RuntimeResearchDurationMultiplier));

				var x = 0;
				foreach (CodeInstruction instruction in Instructions)
				{
					switch (x)
					{
						case 30:
						{
							yield return new CodeInstruction(OpCodes.Call, runtime);
							yield return new CodeInstruction(OpCodes.Callvirt, core);
							yield return new CodeInstruction(OpCodes.Callvirt, multiplier);
							yield return new CodeInstruction(OpCodes.Mul);
							break;
						}

						case 38:
						{
							yield return new CodeInstruction(OpCodes.Call, runtime);
							yield return new CodeInstruction(OpCodes.Callvirt, core);
							yield return new CodeInstruction(OpCodes.Callvirt, multiplier);
							yield return new CodeInstruction(OpCodes.Mul);
							break;
						}
					}

					yield return instruction;

					x++;
				}
			}
		}
	}
}

#endif
