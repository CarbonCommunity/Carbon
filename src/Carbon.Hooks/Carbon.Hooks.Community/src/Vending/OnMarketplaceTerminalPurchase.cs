using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System;
using API.Hooks;
using HarmonyLib;
using Patch = API.Hooks.Patch;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Vending
{
	public partial class Vending_MarketTerminal
	{
		[HookAttribute.Patch("OnMarketplaceTerminalPurchase", "OnMarketplaceTerminalPurchase", typeof(MarketTerminal), "Server_Purchase", new System.Type[] { typeof(BaseEntity.RPCMessage) })]

		[MetadataAttribute.Category("Vending")]
		[MetadataAttribute.Parameter("terminal", typeof(MarketTerminal))]
		[MetadataAttribute.Parameter("vending", typeof(VendingMachine))]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("sellOrderIndex", typeof(int))]
		[MetadataAttribute.Parameter("amount", typeof(int))]
		[MetadataAttribute.Info("Called before making a purchase at the Marketplace terminal.")]
		[MetadataAttribute.Return(typeof(void))]

		public class OnMarketplaceTerminalPurchase : Patch
		{
			public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
			{
				var slotsRequired = AccessTools.Method(typeof(VendingMachine), nameof(VendingMachine.GetSlotsRequiredForTransaction),
					new Type[] { typeof(int), typeof(int) });
				var transactionActive = AccessTools.Field(typeof(MarketTerminal), "_transactionActive");
				var player = AccessTools.Field(typeof(BaseEntity.RPCMessage), "player");
				var callStaticHook = AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook),
					new Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object) });

				var instructions = new List<CodeInstruction>(Instructions);

				if (slotsRequired == null || transactionActive == null || player == null || callStaticHook == null)
				{
					return Unpatched(instructions);
				}

				var vending = -1;
				var sellOrderIndex = -1;
				var amount = -1;
				var index = -1;

				for (var i = 2; i < instructions.Count; i++)
				{
					var instruction = instructions[i];

					// Rust keeps reshuffling the locals of this method between updates, so read them off
					// the 'vending.GetSlotsRequiredForTransaction(sellOrderIndex, amount)' call, which loads
					// all three of them back to back, instead of hardcoding their indices.
					if (vending == -1 && instruction.Calls(slotsRequired) && i >= 3
						&& instructions[i - 3].IsLdloc() && instructions[i - 2].IsLdloc() && instructions[i - 1].IsLdloc())
					{
						vending = instructions[i - 3].LocalIndex();
						sellOrderIndex = instructions[i - 2].LocalIndex();
						amount = instructions[i - 1].LocalIndex();
						continue;
					}

					// 'this._transactionActive = true' is the first instruction of the try/finally the
					// transaction runs in, so the hook goes right in front of it: every check has passed
					// and nothing has been taken off the player yet, and a 'ret' there is still outside
					// of the protected region.
					if (index == -1 && instruction.StoresField(transactionActive)
						&& instructions[i - 1].opcode == OpCodes.Ldc_I4_1 && instructions[i - 2].opcode == OpCodes.Ldarg_0)
					{
						index = i - 2;
					}
				}

				if (vending == -1 || index == -1)
				{
					return Unpatched(instructions);
				}

				var anchor = instructions[index];
				var resume = Generator.DefineLabel();

				var hook = new List<CodeInstruction>
				{
					// hook call start
					new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2145652880)).MoveLabelsFrom(anchor),
					new CodeInstruction(OpCodes.Ldarg_0),
					CodeInstruction.LoadLocal(vending),
					new CodeInstruction(OpCodes.Ldarg_1),
					new CodeInstruction(OpCodes.Ldfld, player),
					CodeInstruction.LoadLocal(sellOrderIndex),
					new CodeInstruction(OpCodes.Box, typeof(int)),
					CodeInstruction.LoadLocal(amount),
					new CodeInstruction(OpCodes.Box, typeof(int)),
					new CodeInstruction(OpCodes.Call, callStaticHook),
					// hook call end

					// return behaviour start
					new CodeInstruction(OpCodes.Brfalse, resume),
					new CodeInstruction(OpCodes.Ret)
					// return behaviour end
				};

				anchor.labels.Add(resume);
				instructions.InsertRange(index, hook);

				return instructions;
			}

			private static List<CodeInstruction> Unpatched(List<CodeInstruction> instructions)
			{
				Logger.Warn($"Failed patching '{nameof(OnMarketplaceTerminalPurchase)}', {nameof(MarketTerminal)}.Server_Purchase no longer matches the expected IL");

				return instructions;
			}
		}
	}
}
