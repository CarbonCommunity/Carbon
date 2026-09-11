using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using API.Hooks;
using Carbon.Components;
using HarmonyLib;
using Network;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Static
{
	public partial class Static_ServerMgr
	{
		[HookAttribute.Patch("IServerMgrOnRPCMessage", "IServerMgrOnRPCMessage", typeof(ServerMgr), "OnRPCMessage", new System.Type[] { typeof(Message) })]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class IServerMgrOnRPCMessage : API.Hooks.Patch
		{
			public static MethodInfo Method = AccessTools.Method(typeof(ClientEntity), "ServerRPCUnknown");

			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				var il = new List<CodeInstruction>(instructions);
				var ilIndex = -1;
				var cil = (CodeInstruction)default;

				for (int index = il.Count - 1; index >= 0; index--)
				{
					cil = il[index];

					if (cil.opcode == OpCodes.Brfalse_S)
					{
						ilIndex = index;
						break;
					}
				}

				if (ilIndex == -1) throw new NullReferenceException("IServerMgrOnRPCMessage failure.");

				il.InsertRange(ilIndex + 1, new List<CodeInstruction>()
				{
					new CodeInstruction(OpCodes.Ldloc_0),
					new CodeInstruction(OpCodes.Ldloc_1),
					new CodeInstruction(OpCodes.Ldarg_1),
					new CodeInstruction(OpCodes.Call, Method),
				});

				return il;
			}
		}
	}
}
