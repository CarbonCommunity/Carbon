#nullable disable

using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Carbon.Utility;
using static Carbon.Projects.Oxide.Oxide;

namespace Carbon.Projects.Oxide;

internal static partial class Helper
{
	internal static bool GenerateSimpleBodyPrepEx(ref StringBuilder instructions, int index)
	{
		AddGenericInstruction(ref instructions, "List<CodeInstruction> edit = new List<CodeInstruction>();");
		AddGenericInstruction(ref instructions, "List<CodeInstruction> original = new List<CodeInstruction>(Instructions);");
		AddGenericInstruction(ref instructions, string.Empty);

		AddGenericInstruction(ref instructions, $"Type rettype = ((MethodInfo)original[{index - 1}].operand).ReturnType;");
		AddGenericInstruction(ref instructions, "object retvar = Generator.DeclareLocal(rettype);");
		AddGenericInstruction(ref instructions, string.Empty);

		AddGenericInstruction(ref instructions, "edit.Add(new CodeInstruction(OpCodes.Stloc_S, retvar));");
		AddGenericInstruction(ref instructions, "edit.Add(new CodeInstruction(OpCodes.Ldloc_S, retvar));");
		AddGenericInstruction(ref instructions, string.Empty);

		AddGenericInstruction(ref instructions, $"original.InsertRange({index - 1}, edit);");
		AddGenericInstruction(ref instructions, "Instructions = original.AsEnumerable();");
		AddGenericInstruction(ref instructions, string.Empty);

		return true;
	}

	internal static bool GenerateSimpleBodyEx(ref StringBuilder instructions, Type hookType, MethodInfo hookMethod, HookDef.Data metadata)
	{
		Metadata = metadata;
		var targetReturnType = hookMethod.ReturnType;
		var targetReturnTypeName = Tools.TypeNameSanitizerEx(targetReturnType.ToString());

		AddGenericInstruction(ref instructions, string.Empty);
		AddGenericInstruction(ref instructions, "// hook call start");

		if (!metadata.IsInternal)
		{
			AddYieldInstructionMoveLabels(ref instructions, nameof(OpCodes.Ldc_I4),
				$"unchecked((int){HookStringPool.GetOrAdd(metadata.HookName)})", false);
		}

		try
		{
			var retval = false;
			switch (metadata.ArgumentBehavior)
			{
				case ArgumentBehavior.None:
					retval = true;
					if (metadata.IsInternal)
					{
						AddYieldInstruction(ref instructions, nameof(OpCodes.Call),
							$"AccessTools.Method(typeof(Carbon.Core.CorePlugin), \"{metadata.HookName}\")", false);
					}
					else
					{
						AddYieldInstruction(ref instructions, nameof(OpCodes.Call),
							"AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) })",
							false);
					}

					break;

				case ArgumentBehavior.All:
				case ArgumentBehavior.JustThis:
				case ArgumentBehavior.JustParams:
					retval = AutomaticArgumentLoader(ref instructions, hookMethod, metadata);
					break;

				case ArgumentBehavior.UseArgumentString:
					// TODO : OnPlayerDisconnected, OnItemCraft OnitemDeployed
					retval = CustomArgumentLoader(ref instructions, hookMethod, metadata, hookType);
					break;
			}

			AddGenericInstruction(ref instructions, "// hook call end");

			if (!retval)
			{
				throw new NotSupportedException("Hook call builder returned null");
			}
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed ({ex.Message})\n{ex.StackTrace}");
			return false;
		}

		try
		{
			AddGenericInstruction(ref instructions, string.Empty);
			AddGenericInstruction(ref instructions, "// return behaviour start");

			IsReturning = true;
			ReturnType = null;

			switch (metadata.ReturnBehavior)
			{
				case ReturnBehavior.Continue:
					AddGenericInstruction(ref instructions, "/* ReturnBehavior.Continue */");
					AddYieldInstruction(ref instructions, nameof(OpCodes.Pop));
					break;

				case ReturnBehavior.ExitWhenNonNull:
				case ReturnBehavior.ExitWhenValidType:
					// no return behavior
					if (targetReturnType == typeof(void))
					{
						AddGenericInstruction(ref instructions, "Label label = Generator.DefineLabel();");
						AddGenericInstruction(ref instructions, "instruction.labels.Add(label);"); // tag the original code starting point
						AddYieldInstruction(ref instructions, nameof(OpCodes.Ldnull));
						AddYieldInstruction(ref instructions, nameof(OpCodes.Beq_S), "label", false); // if not null ret
						AddYieldInstruction(ref instructions, nameof(OpCodes.Ret));
						ReturnType = typeof(void);
						break;
					}

					if (metadata.ReturnBehavior == ReturnBehavior.ExitWhenNonNull && targetReturnType == typeof(bool))
					{
						AddGenericInstruction(ref instructions, "Label label1 = Generator.DefineLabel();");
						AddGenericInstruction(ref instructions, "Label label2 = Generator.DefineLabel();");
						AddGenericInstruction(ref instructions, "object retvar = Generator.DeclareLocal(typeof(object));");

						AddGenericInstruction(ref instructions, "instruction.labels.Add(label1);"); // tag the orignal code starting point

						AddYieldInstruction(ref instructions, nameof(OpCodes.Stloc), "retvar", false); // store result
						AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc), "retvar", false);

						AddYieldInstruction(ref instructions, nameof(OpCodes.Brfalse_S), "label1", false); // continue if null
						AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc), "retvar", false);
						AddYieldInstruction(ref instructions, nameof(OpCodes.Isinst), "typeof(System.Boolean)", false);

						AddYieldInstruction(ref instructions, nameof(OpCodes.Brtrue_S), "label2", false); // if bool return
						AddYieldInstruction(ref instructions, nameof(OpCodes.Ldc_I4_0)); // else ret false
						AddYieldInstruction(ref instructions, nameof(OpCodes.Ret));
						AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc), "retvar).WithLabels(label2", false);

						AddYieldInstruction(ref instructions, nameof(OpCodes.Unbox_Any), $"typeof({targetReturnTypeName})", false);
						AddYieldInstruction(ref instructions, nameof(OpCodes.Ret));
						ReturnType = typeof(bool);
						break;
					}

					AddGenericInstruction(ref instructions, "Label label1 = Generator.DefineLabel();");
					AddGenericInstruction(ref instructions, "object retvar = Generator.DeclareLocal(typeof(object));");
					AddGenericInstruction(ref instructions, "instruction.labels.Add(label1);"); // tag the orignal code starting point

					AddYieldInstruction(ref instructions, nameof(OpCodes.Stloc), "retvar", false); // store result
					AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc), "retvar", false);

					var rettype = targetReturnTypeName;
					if (rettype == "System.Void")
					{
						rettype = "object";
					}

					AddYieldInstruction(ref instructions, nameof(OpCodes.Isinst), $"typeof({rettype})", false); // if matches the ret type
					AddYieldInstruction(ref instructions, nameof(OpCodes.Ldnull));
					AddYieldInstruction(ref instructions, nameof(OpCodes.Beq_S), "label1", false);

					AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc), "retvar", false);
					AddYieldInstruction(ref instructions, nameof(OpCodes.Unbox_Any), $"typeof({rettype})", false); // ret it
					AddYieldInstruction(ref instructions, nameof(OpCodes.Ret));
					ReturnType = hookMethod.ReturnType;
					break;

				case ReturnBehavior.ModifyRefArg:
					Logger.Warning($"{metadata.HookName} 'ReturnBehavior.ModifyRefArg' not implemented");
					throw new NotImplementedException();

				case ReturnBehavior.UseArgumentString:
					Tools.ArgumentParser(metadata.ArgumentString.Trim(), out var argvar, out var retvar);

					if (!string.IsNullOrEmpty(retvar))
					{
						if (retvar[0] == 'l' && retvar.Length > 1)
						{
							if (int.TryParse(retvar.Substring(1), out var index))
							{
								try
								{
									var vars = hookMethod?.GetMethodBody()?.LocalVariables ?? null;

									if (vars == null || index >= vars.Count)
									{
										// index out of bounds when the var is patched by a "modify" hook
										LoadLocalIntEx(ref instructions, index);
										return true;
									}

									var targetvar = vars[index];

									AddGenericInstruction(ref instructions, "Label label1 = Generator.DefineLabel();");
									AddGenericInstruction(ref instructions,
										"instruction.labels.Add(label1);"); // tag the original code starting point

									AddGenericInstruction(ref instructions, "object retvar = Generator.DeclareLocal(typeof(object));");

									AddYieldInstruction(ref instructions, nameof(OpCodes.Stloc), "retvar",
										false); // store hook result on new var
									AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc), "retvar", false); // load hook result

									var targetvartype = Tools.TypeNameSanitizerEx(targetvar.LocalType.ToString());
									if (targetvartype == "System.Void")
									{
										targetvartype = "object";
									}

									AddYieldInstruction(ref instructions, nameof(OpCodes.Isinst), $"typeof({targetvartype})",
										false); // check type
									AddYieldInstruction(ref instructions, nameof(OpCodes.Ldnull));
									AddYieldInstruction(ref instructions, nameof(OpCodes.Beq_S), "label1",
										false); // if type is null (ie no match) jump over

									AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc), "retvar", false); // load hook result
									AddYieldInstruction(ref instructions, nameof(OpCodes.Unbox_Any), $"typeof({targetvartype})",
										false); // type cast
									StoreLocalIntEx(ref instructions, index, targetvartype); // store result

									// Handled
									return true;
								}
								catch (Exception e)
								{
									Logger.Warning($"{metadata.HookName} 'ReturnBehavior.UseArgumentString' exception: {e.Message}");
									return false;
								}
							}
						}
						else if (retvar[0] == 'a' && retvar.Length > 1)
						{
							if (int.TryParse(retvar.Substring(1), out var index))
							{
								AddGenericInstruction(ref instructions, "Label label1 = Generator.DefineLabel();");
								AddGenericInstruction(ref instructions, "object retvar = Generator.DeclareLocal(typeof(object));");
								AddGenericInstruction(ref instructions,
									"instruction.labels.Add(label1);"); // tag the original code starting point

								var parameter = hookMethod.GetParameters()[index];
								var paramtype = parameter.ParameterType.IsByRef
									? parameter.ParameterType.GetElementType()
									: parameter.ParameterType;

								AddYieldInstruction(ref instructions, nameof(OpCodes.Stloc), "retvar", false); // store result
								AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc), "retvar", false);

								AddYieldInstruction(ref instructions, nameof(OpCodes.Isinst), $"typeof({paramtype})",
									false); // if matches the parameter type
								AddYieldInstruction(ref instructions, nameof(OpCodes.Brfalse_S), "label1", false); // continue if null

								var hasThis = hookMethod.CallingConvention.HasFlag(CallingConventions.HasThis);

								if (!parameter.ParameterType.IsValueType)
								{
									LoadArgumentEx(ref instructions, parameter, hasThis);
								}

								AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc), "retvar", false);
								AddYieldInstruction(ref instructions, nameof(OpCodes.Unbox_Any), $"typeof({paramtype})", false);

								if (!parameter.ParameterType.IsValueType)
								{
									Logger.Warning(
										$"{metadata.HookName} 'ReturnBehavior.UseArgumentString' ('IsValueType') not implemented");
									throw new NotImplementedException();
								}

								if (hasThis)
								{
									index++;
								}

								AddYieldInstruction(ref instructions, nameof(OpCodes.Starg), index, false);

								break;
							}
						}
						else if (retvar is "ret" or "return")
						{
							Logger.Warning($"{metadata.HookName} 'ReturnBehavior.UseArgumentString' ('ret') not implemented");
							throw new NotImplementedException();
						}
					}

					AddGenericInstruction(ref instructions, "/* oops */");
					AddYieldInstruction(ref instructions, nameof(OpCodes.Pop)); // oops
					break;

				default:
					throw new NotImplementedException();
			}

			IsReturning = false;
			AddGenericInstruction(ref instructions, "// return behaviour end");
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
			return false;
		}

		return true;
	}

	internal static bool GenerateModifyBodyEx(ref StringBuilder instructions, Type hookType, MethodInfo hookMethod, HookDef.Data metadata)
	{
		var labels = new Dictionary<int, string>();
		var fwdLabels = new Dictionary<int, string>();
		try
		{
			var line = 0;
			for (var instructionIndex = 0; instructionIndex < metadata.Instructions.Count; instructionIndex++)
			{
				var instruction = metadata.Instructions[instructionIndex];
				string[] operand = null;

				if (instruction.Operand != null)
				{
					operand = instruction.Operand.ToString().Split(['|'], 3);
				}

				switch (instruction.OpCode)
				{
					case "beq_s":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Beq_S), instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "bge_s":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Bge_S), instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "blt_s":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Blt_S), instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "blt_un_s":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Blt_Un_S),
							instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "ble":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Ble), instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "ble_un":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Ble_Un), instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "ble_s":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Ble_S), instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "bne_un_s":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Bne_Un_S),
							instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "box":
						if (operand is { Length: >= 2 })
						{
							AddGenericInstruction(ref instructions,
								$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Box)}, typeof({Tools.TypeNameSanitizerEx(operand[1])})));");
							break;
						}

						throw new Exception($"{metadata.Name} : box : {instruction.Operand}");

					case "br_s":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Br_S), instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "br":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Br), instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "brfalse":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Brfalse),
							instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "brfalse_s":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Brfalse_S),
							instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "brtrue_s":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Brtrue_S),
							instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "brtrue":
						AddOpCodeWithLabel(ref instructions, ref labels, ref fwdLabels, nameof(OpCodes.Brtrue),
							instruction.Operand, instruction.ReferencesNewInstruction);
						break;

					case "call":
						if (operand is { Length: >= 3 })
						{
							if (operand[2].Contains('['))
							{
								// this one is implicit so we can ignore it
								var method = operand[2].Split('[')[0];
								var generic = operand[2].Split('[')[1].TrimEnd(']');

								generic = generic.Contains('|')
									? generic.Split('|')[1]
									: generic;

								AddGenericInstruction(ref instructions,
									$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Call)}, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName(\"{operand[1]}\"), \"{method}\", generics: new System.Type[] {{ typeof({generic}) }})));");
								break;
							}

							if (operand[2].Contains('('))
							{
								var t1 = operand[2].Split('(');
								var method = t1[0];

								var t1OpenParen = t1[1];
								var t2 = t1OpenParen.Remove(t1OpenParen.Length - 1);
								var t3 = t2.Split(',');

								for (var x = 0; x < t3.Length; x++)
								{
									var piece = t3[x];
									if (piece.Contains('|'))
									{
										piece = piece.Split('|')[1];
									}

									t3[x] = $"typeof({Tools.TypeNameSanitizerEx(piece)})";
								}

								var args = string.Join(",", t3);

								AddGenericInstruction(ref instructions,
									$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Call)}, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName(\"{operand[1]}\"), \"{method}\", new System.Type[] {{ {args} }})));");
								break;
							}

							AddGenericInstruction(ref instructions,
								$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Call)}, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName(\"{operand[1]}\"), \"{operand[2]}\")));");
							break;
						}

						throw new Exception($"{metadata.Name} : call : {instruction.Operand}");

					case "callvirt":
						if (operand is { Length: >= 3 })
						{
							if (operand[2].Contains('['))
							{
								// this one is implicit so we can ignore it
								var method = operand[2].Split('[')[0];
								var generic = operand[2].Split('[')[1].TrimEnd(']');

								generic = generic.Contains('|')
									? generic.Split('|')[1]
									: generic;

								AddGenericInstruction(ref instructions,
									$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Callvirt)}, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName(\"{operand[1]}\"), \"{method}\", generics: new System.Type[] {{ typeof({generic}) }})));");
							}

							else if (operand[2].Contains('('))
							{
								var t1 = operand[2].Split('(');
								var method = t1[0];

								var t1OpenParen = t1[1];
								var t2 = t1OpenParen.Remove(t1OpenParen.Length - 1);
								var t3 = t2.Split(',');

								for (var x = 0; x < t3.Length; x++)
								{
									var piece = t3[x];
									if (piece.Contains('|'))
									{
										piece = piece.Split('|')[1];
									}

									t3[x] = $"typeof({Tools.TypeNameSanitizerEx(piece)})";
								}

								var args = string.Join(",", t3);

								AddGenericInstruction(ref instructions,
									$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Callvirt)}, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName(\"{operand[1]}\"), \"{method}\", new System.Type[] {{ {args} }})));");
							}
							else
							{
								AddGenericInstruction(ref instructions,
									$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Callvirt)}, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName(\"{operand[1]}\"), \"{operand[2]}\")));");
							}
						}

						break;

					case "castclass":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Castclass)}, typeof({operand[1]})));");
						break;

					case "ceq":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ceq)}));");
						break;

					case "conv_i8":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Conv_I8)}));");
						break;

					case "isinst":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Isinst)}, typeof({operand[1]})));");
						break;

					case "ldarg_0":
						instructions.AppendLine("// Opop");
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldarg_0)}));");
						break;

					case "ldarg_1":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldarg_1)}));");
						break;

					case "ldarg_2":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldarg_2)}));");
						break;

					case "ldarg_3":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldarg_3)}));");
						break;

					case "ldarg_s":
						var index = hookMethod.CallingConvention.HasFlag(CallingConventions.HasThis)
							? Convert.ToInt32(instruction.Operand) + 1
							: Convert.ToInt32(instruction.Operand);
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldarg_S)}, (sbyte){index}));");
						break;

					case "ldc_i4":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldc_I4)}, {instruction.Operand}));");
						break;

					case "ldc_i4_0":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldc_I4_0)}));");
						break;

					case "ldc_i4_1":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldc_I4_1)}));");
						break;

					case "ldc_i4_2":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldc_I4_2)}));");
						break;

					case "ldc_i4_3":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldc_I4_3)}));");
						break;

					case "ldc_i4_4":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldc_I4_4)}));");
						break;

					case "ldc_i4_5":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldc_I4_5)}));");
						break;

					case "ldc_i4_6":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldc_I4_6)}));");
						break;

					case "ldc_i4_7":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldc_I4_7)}));");
						break;

					case "ldc_i4_8":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldc_I4_8)}));");
						break;

					case "ldc_i4_s":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldc_I4_S)}, (sbyte){instruction.Operand}));");
						break;

					case "ldc_r4":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldc_R4)}, {instruction.Operand}f));");
						break;

					case "ldelem_ref":
						AddGenericInstruction(ref instructions, $"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldelem_Ref)}));");
						break;

					case "ldfld":
						if (operand is not { Length: 3 })
						{
							throw new Exception($"{metadata.Name} : ldfld : {instruction.Operand}");
						}

						if (hookType.IsGenericTypeDefinition)
						{
							AddGenericInstruction(ref instructions,
								$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldfld)}, AccessTools.Field(Method.DeclaringType, \"{operand[2]}\")));");
						}
						else
						{
							AddGenericInstruction(ref instructions,
								$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldfld)}, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName(\"{operand[1]}\"), \"{operand[2]}\")));");
						}

						break;

					case "ldflda":
						if (operand is not { Length: 3 })
						{
							throw new Exception($"{metadata.Name} : ldfld : {instruction.Operand}");
						}

						if (hookType.IsGenericTypeDefinition)
						{
							AddGenericInstruction(ref instructions,
								$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldflda)}, AccessTools.Field(Method.DeclaringType, \"{operand[2]}\")));");
						}
						else
						{
							AddGenericInstruction(ref instructions,
								$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldflda)}, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName(\"{operand[1]}\"), \"{operand[2]}\")));");
						}

						break;

					case "ldloc":
						AddLoadLocalInstructionEx(ref instructions, Convert.ToInt32(instruction.Operand),
							LookAheadForTypeHint(metadata.Instructions, Convert.ToInt32(instruction.Operand)));
						break;

					case "ldloc_0":
						AddLoadLocalInstructionEx(ref instructions, 0, LookAheadForTypeHint(metadata.Instructions, 0));
						break;

					case "ldloc_1":
						AddLoadLocalInstructionEx(ref instructions, 1, LookAheadForTypeHint(metadata.Instructions, 1));
						break;

					case "ldloc_2":
						AddLoadLocalInstructionEx(ref instructions, 2, LookAheadForTypeHint(metadata.Instructions, 2));
						break;

					case "ldloc_3":
						AddLoadLocalInstructionEx(ref instructions, 3, LookAheadForTypeHint(metadata.Instructions, 3));
						break;

					case "ldloc_s":
						AddLoadLocalInstructionEx(ref instructions, Convert.ToInt32(instruction.Operand),
							LookAheadForTypeHint(metadata.Instructions, Convert.ToInt32(instruction.Operand)));
						break;

					case "ldloca_s":
						AddLoadLocalAddressInstructionEx(ref instructions, Convert.ToInt32(instruction.Operand),
							LookAheadForTypeHint(metadata.Instructions, Convert.ToInt32(instruction.Operand)));
						break;

					case "ldnull":
						AddGenericInstruction(ref instructions, $"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldnull)}));");
						break;

					case "ldstr":
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ldstr)}, \"{instruction.Operand}\"));");
						break;

					case "pop":
						AddGenericInstruction(ref instructions, $"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Pop)}));");
						break;

					case "ret":
						AddGenericInstruction(ref instructions, $"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Ret)}));");
						break;

					case "leave":
						AddGenericInstruction(ref instructions, "var endLabel = Generator.DefineLabel();");
						AddGenericInstruction(ref instructions, $"original[{instruction.Operand}].labels.Add(endLabel);");
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Leave)}, endLabel));");
						break;

					case "leave_s":
						AddGenericInstruction(ref instructions, "var endLabel = Generator.DefineLabel();");
						AddGenericInstruction(ref instructions, $"original[{instruction.Operand}].labels.Add(endLabel);");
						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Leave_S)}, endLabel));");
						break;

					case "stfld":
						if (operand is not { Length: 3 })
						{
							throw new Exception($"{metadata.Name} : stfld : {instruction.Operand}");
						}

						AddGenericInstruction(ref instructions,
							$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Stfld)}, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName(\"{operand[1]}\"), \"{operand[2]}\")));");
						break;

					case "stloc":
						AddStoreLocalInstructionEx(ref instructions, Convert.ToInt32(instruction.Operand),
							LookAheadForTypeHint(metadata.Instructions, Convert.ToInt32(instruction.Operand)));
						break;

					case "stloc_0":
						AddStoreLocalInstructionEx(ref instructions, 0, LookAheadForTypeHint(metadata.Instructions, 0));
						break;

					case "stloc_1":
						AddStoreLocalInstructionEx(ref instructions, 1, LookAheadForTypeHint(metadata.Instructions, 1));
						break;

					case "stloc_2":
						AddStoreLocalInstructionEx(ref instructions, 2, LookAheadForTypeHint(metadata.Instructions, 2));
						break;

					case "stloc_3":
						AddStoreLocalInstructionEx(ref instructions, 3, LookAheadForTypeHint(metadata.Instructions, 3));
						break;

					case "stloc_s":
						AddStoreLocalInstructionEx(ref instructions, Convert.ToInt32(instruction.Operand),
							LookAheadForTypeHint(metadata.Instructions, Convert.ToInt32(instruction.Operand)));
						break;

					case "unbox_any":
						if (operand is { Length: >= 2 })
						{
							AddGenericInstruction(ref instructions,
								$"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Unbox_Any)}, typeof({Tools.TypeNameSanitizerEx(operand[1])})));");
							break;
						}

						throw new Exception($"{metadata.Name} : unbox_any : {instruction.Operand}");

					case "nop":
						AddGenericInstruction(ref instructions, $"edit.Add(new CodeInstruction(OpCodes.{nameof(OpCodes.Nop)}));");
						break;

					default:
						throw new NotImplementedException($"{metadata.HookName} opcode '{instruction.OpCode}' not implemented");
				}

				line++;
			}

			if (fwdLabels.Count > 0)
			{
				instructions.AppendLine(string.Empty);

				foreach (var foo in fwdLabels)
				{
					AddGenericInstruction(ref instructions, $"edit[{foo.Key}].labels.Add({foo.Value});");
				}
			}
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
			return false;
		}

		return true;
	}
}
