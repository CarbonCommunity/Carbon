#nullable disable

using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Carbon.Utility;
using static Carbon.Projects.Oxide.Oxide;

namespace Carbon.Projects.Oxide;

internal static partial class Helper
{
	internal static bool AutomaticArgumentLoader(ref StringBuilder instructions, MethodInfo hookMethod, HookDef.Data metadata)
	{
		Metadata = metadata;

		var includeargs = metadata.ArgumentBehavior is ArgumentBehavior.All or ArgumentBehavior.JustParams;
		var includethis = metadata.ArgumentBehavior is ArgumentBehavior.All or ArgumentBehavior.JustThis;

		var argc = 0;
		var hasThis = hookMethod.CallingConvention.HasFlag(CallingConventions.HasThis);

		if (includethis && hasThis)
		{
			argc++;
			LoadArgumentEx(ref instructions, null);
			Parameters.Add(("self", hookMethod.DeclaringType));
		}

		if (includeargs)
		{
			var hookParameters = hookMethod.GetParameters();
			for (var parameterIndex = 0; parameterIndex < hookParameters.Length; parameterIndex++)
			{
				var parameter = hookParameters[parameterIndex];
				if (parameter.IsOut)
				{
					continue;
				}

				argc++;

				if (!metadata.IsInternal && parameter.ParameterType is { IsValueType: true, IsByRefLike: true })
				{
					var argIndex = parameter.Position + (hasThis ? 1 : 0);
					AddYieldInstruction(ref instructions, nameof(OpCodes.Ldarga_S), $"(byte){argIndex}", false);
					AddYieldInstruction(ref instructions, nameof(OpCodes.Call),
						$"AccessTools.Method(typeof({Tools.TypeNameSanitizerEx(parameter.ParameterType.FullName)}), \"ToArray\")",
						false);
				}
				else
				{
					LoadArgumentEx(ref instructions, parameter, hasThis);

					if (parameter.ParameterType.IsByRef)
					{
						AddYieldInstruction(ref instructions, nameof(OpCodes.Ldobj),
							$"typeof({Tools.TypeNameSanitizerEx(parameter.ParameterType.FullName)})", false);

						if (!metadata.IsInternal)
						{
							AddYieldInstruction(ref instructions, nameof(OpCodes.Box),
								$"typeof({(Nullable.GetUnderlyingType(parameter.ParameterType) == null ? Tools.TypeNameSanitizerEx(parameter.ParameterType.FullName) : $"{parameter.ParameterType.FullName}?")})",
								false);
						}
					}
					else if (!metadata.IsInternal && parameter.ParameterType.IsValueType)
					{
						AddYieldInstruction(ref instructions, nameof(OpCodes.Box),
							$"typeof({(Nullable.GetUnderlyingType(parameter.ParameterType) == null ? Tools.TypeNameSanitizerEx(parameter.ParameterType.FullName) : $"{parameter.ParameterType.FullName}?")})",
							false);
					}
				}
			}
		}

		if (metadata.IsInternal)
		{
			AddYieldInstruction(ref instructions, nameof(OpCodes.Call),
				$"AccessTools.Method(typeof(Carbon.Core.CorePlugin), \"{metadata.HookName}\")", false);
		}
		else
		{
			var arr = string.Concat(Enumerable.Repeat("typeof(object),", argc));

			AddYieldInstruction(ref instructions, nameof(OpCodes.Call),
				$"AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] {{ typeof(uint), {arr} }})",
				false);
		}

		return true;
	}

	internal static bool CustomArgumentLoader(ref StringBuilder instructions, MethodInfo hookMethod, HookDef.Data metadata, Type type)
	{
		Metadata = metadata;

		var argc = Tools.ArgumentParser(metadata.ArgumentString.Trim(), out var argvar, out _);
		var hasThis = hookMethod.CallingConvention.HasFlag(CallingConventions.HasThis);

		if (argc == 0)
		{
			Logger.Warning($"{metadata.HookName} ArgumentBehavior.UseArgumentString is set but argc count is zero");
			return false;
		}

		for (var i = 0; i < argc; i++)
		{
			var argument = argvar[i];
			string[] target = null;

			if (argument.Contains('.'))
			{
				var split = argument.Split('.');
				argument = split[0];
				target = new string[split.Length - 1];
				Array.Copy(split, 1, target, 0, target.Length);
			}

			if (string.IsNullOrEmpty(argument))
			{
				AddYieldInstruction(ref instructions, nameof(OpCodes.Ldnull));
			}

			else if (argument == "this")
			{
				if (hookMethod.IsStatic)
				{
					AddYieldInstruction(ref instructions, nameof(OpCodes.Ldnull));
				}
				else
				{
					LoadArgumentEx(ref instructions, null, type: type, target: target);
				}

				try
				{
					LoadMember(ref instructions, hookMethod.DeclaringType.GetTypeInfo(), hookMethod, target, metadata);
				}
				catch (Exception)
				{
					return false;
				}

				Parameters.Add(("self", type));
			}

			// method's arguments
			else if (argument[0] == 'p' || argument[0] == 'a')
			{
				if (int.TryParse(argument.AsSpan(1), out var index))
				{
					var hookMethodParameters = hookMethod.GetParameters();
					try
					{
						if (index >= hookMethodParameters.Length)
						{
							throw new ArgumentException();
						}

						var parameter = hookMethodParameters[index];

						Type TypeLookup()
						{
							const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
							                         BindingFlags.Static;

							var currentType = parameter.ParameterType;

							if (target != null)
							{
								for (var targetIndex = 0; targetIndex < target.Length; targetIndex++)
								{
									var targetValue = target[targetIndex];
									var field = currentType.GetField(targetValue, all);

									if (field == null)
									{
										var property = currentType.GetProperty(targetValue, all);

										if (property != null)
										{
											currentType = property.PropertyType;
										}
									}
									else
									{
										currentType = field.FieldType;
									}
								}
							}

							return currentType;
						}

						LoadArgumentEx(ref instructions, parameter, hasThis, parameter.ParameterType, target);

						try
						{
							var processType = TypeLookup();

							if (!LoadMember(ref instructions, parameter.ParameterType.GetTypeInfo(), hookMethod, target, metadata))
							{
								var typeref = parameter.ParameterType.IsByRef
									? parameter.ParameterType.GetElementType()
									: parameter.ParameterType;

								if (processType.IsByRef)
								{
									AddYieldInstruction(ref instructions, nameof(OpCodes.Ldobj),
										$"typeof({Tools.TypeNameSanitizerEx(typeref.FullName)})", false);
								}

								if (!metadata.IsInternal && processType.IsValueType)
								{
									AddYieldInstruction(ref instructions, nameof(OpCodes.Box),
										$"typeof({(Nullable.GetUnderlyingType(typeref) == null ? Tools.TypeNameSanitizerEx(typeref.FullName) : $"{typeref.FullName}?")})",
										false);
								}
							}

							Parameters.Add((target == null ? parameter.Name : target[^1], processType));
						}
						catch (Exception)
						{
							return false;
						}
					}
					catch
					{
						Logger.Warning(
							$"{metadata.HookName} out of bounds arg:{argument} index:{index} count:{hookMethodParameters.Length}");
						return false;
					}
				}
			}

			// method's local vars
			else if (argument[0] == 'l' || argument[0] == 'v')
			{
				if (int.TryParse(argument.Substring(1), out var index))
				{
					try
					{
						var vars = hookMethod?.GetMethodBody()?.LocalVariables ?? null;

						if (vars == null || index >= vars.Count)
						{
							// index out of bounds when the var is patched by a "modify" hook
							AddGenericInstruction(ref instructions, $"/* out of bounds {index}/{vars.Count} */");
							LoadLocalIntEx(ref instructions, index);
						}
						else
						{
							var var = vars[index];

							LoadLocalEx(ref instructions, var, target);

							if (var.LocalType is null)
							{
								throw new Exception("var.LocalType is null");
							}

							var typeref = var.LocalType.IsByRef
								? var.LocalType.GetElementType()
								: var.LocalType;

							if (var.LocalType.IsByRef)
							{
								AddYieldInstruction(ref instructions, nameof(OpCodes.Ldobj),
									$"typeof({(Nullable.GetUnderlyingType(typeref) == null ? Tools.TypeNameSanitizerEx(typeref.FullName) : $"{typeref.FullName}?")})",
									false);
							}

							if (!metadata.IsInternal && var.LocalType.IsValueType)
							{
								var genericTypes = typeref.GenericTypeArguments;

								if (genericTypes is { Length: > 0 })
								{
									AddYieldInstruction(ref instructions, nameof(OpCodes.Box), $"Carbon.Extensions.AccessToolsEx.TypeByName(\"{typeref.FullName.Split('[')[0]}\").MakeGenericType({string.Join(", ", genericTypes.Select(x => $"AccessTools.TypeByName(\"{x.FullName}\")"))})",
										false);
								}
								else
								{
									AddYieldInstruction(ref instructions, nameof(OpCodes.Box), $"Carbon.Extensions.AccessToolsEx.TypeByName(\"{(RuntimeType ?? typeref).FullName}\")", false);
								}
							}

							Parameters.Add((target == null ? $"local{var.LocalIndex}" : target[^1], RuntimeType ?? typeref));
						}
					}
					catch (Exception e)
					{
						Logger.Warning($"{metadata.HookName} 'l' and 'v' exception: ({e.Message})\n{e.StackTrace}");
						return false;
					}
				}
				else
				{
					AddYieldInstruction(ref instructions, nameof(OpCodes.Ldnull));
				}
			}

			// method's introduced local vars
			else if (argument[0] == 'r')
			{
				AddGenericInstruction(ref instructions, "yield return new CodeInstruction(OpCodes.Ldloc_S, retvar);");
			}

			else
			{
				AddYieldInstruction(ref instructions, nameof(OpCodes.Ldnull));
			}
		}

		if (metadata.IsInternal)
		{
			AddYieldInstruction(ref instructions, nameof(OpCodes.Call), $"AccessTools.Method(typeof(Carbon.Core.CorePlugin), \"{metadata.HookName}\")", false);
		}
		else
		{
			var arr = string.Concat(Enumerable.Repeat("typeof(object),", argc));

			instructions.AppendLine($"yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] {{ typeof(uint), {arr} }}));");
		}

		return true;
	}
}
