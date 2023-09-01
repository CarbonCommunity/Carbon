using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System;
using API.Hooks;
using HarmonyLib;
using Patch = API.Hooks.Patch;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Vending
{
	public partial class Vending_MarketTerminal
	{
		[HookAttribute.Patch("OnMarketplaceTerminalPurchase", "OnMarketplaceTerminalPurchase", typeof(MarketTerminal), "Server_Purchase", new System.Type[] { typeof(BaseEntity.RPCMessage) })]
		[HookAttribute.Identifier("72eec86b418f48f18d2cdd57785bc9ab")]

		[MetadataAttribute.Parameter("terminal", typeof(MarketTerminal))]
		[MetadataAttribute.Parameter("vending", typeof(VendingMachine))]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("sellOrderIndex", typeof(int))]
		[MetadataAttribute.Parameter("amount", typeof(int))]
		[MetadataAttribute.Info("Called before making a purchase at the Marketplace terminal.")]
		[MetadataAttribute.Return(typeof(bool))]

		public class Vending_MarketTerminal_72eec86b418f48f18d2cdd57785bc9ab : Patch
		{
			public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
			{
				var x = 0;
				foreach (CodeInstruction instruction in Instructions)
				{
					if (x++ != 66)
					{
						yield return instruction;
						continue;
					}

					// hook call start
					yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3542372208)).MoveLabelsFrom(instruction);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldloc_3);
					yield return new CodeInstruction(OpCodes.Ldarg_1);
					yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(BaseEntity.RPCMessage), "player"));
					yield return new CodeInstruction(OpCodes.Ldloc_1);
					yield return new CodeInstruction(OpCodes.Box, typeof(int));
					yield return new CodeInstruction(OpCodes.Ldloc_2);
					yield return new CodeInstruction(OpCodes.Box, typeof(int));
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook),
						new Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object) }));
					// hook call end

					// return behaviour start
					var label = Generator.DefineLabel();
					instruction.labels.Add(label);
					yield return new CodeInstruction(OpCodes.Ldnull);
					yield return new CodeInstruction(OpCodes.Beq_S, label);
					yield return new CodeInstruction(OpCodes.Ret);
					// return behaviour end

					yield return instruction;
				}
			}
		}
	}
}
