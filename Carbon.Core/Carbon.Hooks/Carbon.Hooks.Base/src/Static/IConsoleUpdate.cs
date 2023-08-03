using System.Collections.Generic;
using System;
using System.Reflection.Emit;
using System.Reflection;
using API.Hooks;
using Windows;
using HarmonyLib;
using Patch = API.Hooks.Patch;

/*
 *
 * Copyright (c) 2023 Carbon Community 
 * Copyright (c) 2023 turner
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

#if WIN

public partial class Category_Static
{
	public partial class Static_Console
	{
		[HookAttribute.Patch("IConsoleUpdate", "IConsoleUpdate", typeof(ConsoleInput), "Update", new System.Type[] { })]
		[HookAttribute.Identifier("0e2b1792048b482387730fa17ea46f1f")]
		[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden | HookFlags.IgnoreChecksum)]
		public class Static_Console_Update_0e2b1792048b482387730fa17ea46f1f : Patch
		{
			public static void UpHook(ConsoleInput _this)
			{
				var up = Static_Console_Enter_7aec275cf9ec428baa152b4108fcd390.GetUp();
				if (string.IsNullOrEmpty(up)) return;

				_this.inputString = up;
				_this.RedrawInputLine();
			}

			public static void DownHook(ConsoleInput _this)
			{
				var down = Static_Console_Enter_7aec275cf9ec428baa152b4108fcd390.GetDown();

				_this.inputString = string.IsNullOrEmpty(down) ? string.Empty : down;
				_this.RedrawInputLine();

			}

			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
			{
				var flag = false;
				var flag2 = false;
				var label = generator.DefineLabel();
				var label2 = generator.DefineLabel();
				var list = new List<CodeInstruction>();

				foreach (var codeInstruction in instructions)
				{
					if (flag && !flag2)
					{
						list.Add(new CodeInstruction(OpCodes.Stloc_0, null));
						list.Add(new CodeInstruction(OpCodes.Ldloca, 0));
						list.Add(new CodeInstruction(OpCodes.Call, typeof(ConsoleKeyInfo).GetProperty("Key", BindingFlags.Instance | BindingFlags.Public).GetMethod));
						list.Add(new CodeInstruction(OpCodes.Ldc_I4, 38));
						list.Add(new CodeInstruction(OpCodes.Sub, null));
						list.Add(new CodeInstruction(OpCodes.Brtrue, label));
						list.Add(new CodeInstruction(OpCodes.Ldarg_0, null));
						list.Add(new CodeInstruction(OpCodes.Call, typeof(Static_Console_Update_0e2b1792048b482387730fa17ea46f1f).GetMethod("UpHook", BindingFlags.Static | BindingFlags.Public)));
						list.Add(new CodeInstruction(OpCodes.Ret, null));
						list.Add(new CodeInstruction(OpCodes.Nop, null)
						{
							labels = new List<Label>
							{
								label
							}
						});
						list.Add(new CodeInstruction(OpCodes.Ldloca, 0));
						list.Add(new CodeInstruction(OpCodes.Call, typeof(ConsoleKeyInfo).GetProperty("Key", BindingFlags.Instance | BindingFlags.Public).GetMethod));
						list.Add(new CodeInstruction(OpCodes.Ldc_I4, 40));
						list.Add(new CodeInstruction(OpCodes.Sub, null));
						list.Add(new CodeInstruction(OpCodes.Brtrue, label2));
						list.Add(new CodeInstruction(OpCodes.Ldarg_0, null));
						list.Add(new CodeInstruction(OpCodes.Call, typeof(Static_Console_Update_0e2b1792048b482387730fa17ea46f1f).GetMethod("DownHook", BindingFlags.Static | BindingFlags.Public)));
						list.Add(new CodeInstruction(OpCodes.Ret, null));
						list.Add(new CodeInstruction(OpCodes.Nop, null)
						{
							labels = new List<Label>
							{
								label2
							}
						});
						flag2 = true;
					}
					else
					{
						if (codeInstruction.opcode == OpCodes.Call && (MethodInfo)codeInstruction.operand == typeof(Console).GetMethod("ReadKey", BindingFlags.Static | BindingFlags.Public, null, new Type[0], null) && !flag2)
						{
							flag = true;
						}

						list.Add(codeInstruction);
					}
				}
				return list;
			}
		}
	}
}

#endif
