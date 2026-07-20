#nullable disable

using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Carbon.Utility;

namespace Carbon.Projects.Oxide;

internal static partial class Helper
{
	internal static bool LoadMember(
		ref StringBuilder instructions, TypeInfo declaringType, MethodInfo hookMethod, string[] targets, Oxide.HookDef.Data metadata
	)
	{
		if (declaringType == null || targets == null || targets.Length == 0)
		{
			return false;
		}

		Type type = declaringType;
		for (var i = 0; i < targets.Length; i++)
		{
			var target = targets[i];
			try
			{
				if (LoadMember(ref instructions, declaringType, hookMethod, target, metadata))
				{
					continue;
				}

				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		if (type.IsValueType || type.IsByRef)
		{
			//AddInstruction(ref Instructions, nameof(OpCodes.Box),
			//	$"typeof({Tools.TypeNameSanitizerEx(type.FullName)})", false);

			// example hit: OnTeamUpdate
			//throw new NotImplementedException();
		}

		return true;
	}

	internal static bool LoadMember(
		ref StringBuilder instructions, TypeInfo declaringType, MethodInfo hookMethod, string target, Oxide.HookDef.Data metadata
	)
	{
		if (declaringType == null || string.IsNullOrEmpty(target))
		{
			return false;
		}

		while (declaringType != null)
		{
			if (target.Contains('('))
			{
				// TODO : only one hit
				throw new NotImplementedException();
			}

			// check if type is a class or a structure
			if (declaringType.IsClass || declaringType is { IsValueType: true, IsEnum: false })
			{
				instructions.AppendLine($"// Read {CurrentField} : {declaringType}");

				if (declaringType.DeclaredFields.Any() && CurrentField != declaringType)
				{
					foreach (var field in declaringType.DeclaredFields)
					{
						if (!string.Equals(field.Name, target, StringComparison.CurrentCultureIgnoreCase))
						{
							continue;
						}

						instructions.AppendLine(
							$"// Been here done that: {field.FieldType} | {declaringType} | {declaringType.DeclaredFields.Count()} | {target} | {hookMethod.ReturnType} {hookMethod.Name}");
						AddYieldInstruction(ref instructions, nameof(OpCodes.Ldfld),
							$"AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName(\"{declaringType.FullName}\"), \"{field.Name}\")",
							false);

						if (!metadata.IsInternal && (field.FieldType.IsValueType || field.FieldType.IsByRef))
						{
							instructions.AppendLine("// WAAAAAAA 2");
							AddYieldInstruction(ref instructions, nameof(OpCodes.Box),
								$"typeof({Tools.TypeNameSanitizerEx(field.FieldType.FullName)})", false);
						}

						return true;
					}
				}
				// throw new NotImplementedException("Code path never reached");
			}

			if (declaringType.DeclaredProperties.Any())
			{
				foreach (var property in declaringType.DeclaredProperties)
				{
					if (!string.Equals(property.Name, target, StringComparison.CurrentCultureIgnoreCase))
					{
						continue;
					}

					// instructions.AppendLine($"// This is it fellas");
					// AddYieldInstruction(ref instructions, nameof(OpCodes.Callvirt),
					// 	$"AccessTools.Method(typeof({Tools.TypeNameSanitizerEx(declaringType.FullName)}), \"{property.GetGetMethod().Name}\")", false);

					return true;
				}
			}

			if (declaringType.ImplementedInterfaces.Any())
			{
				throw new NotImplementedException();
			}

			if (declaringType.BaseType != null && hookMethod.Module.Assembly != declaringType.BaseType.Module.Assembly)
			{
				throw new NotImplementedException("Code path never reached");
			}

			throw new NotImplementedException("Code path never reached");
		}

		return false;
	}
}
