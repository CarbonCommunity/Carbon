using System.Reflection;
using System.Text;
using Carbon.Components;

namespace Carbon.Documentation.Generators;

public static class Hooks
{
	public static List<CarbonHook> Oxide = new();
	public static List<CarbonHook> Community = new();
	public static List<CarbonHook> Base = new();

	public static void CollectHooks(Assembly hookAssembly, List<CarbonHook> hooks, bool isOxideHooks)
	{
		foreach (var type in hookAssembly.GetTypes())
		{
			var hookAttributes = type.GetCustomAttributes();
			if (!hookAttributes.Any())
			{
				continue;
			}

			var hook = CarbonHook.Parse(hookAttributes, hookAssembly, isOxideHooks);
			if (!hook.IsValid)
			{
				continue;
			}

			Console.WriteLine($" Processing hook: {hook.name} [{hook.category}] @ {hook.method?.DeclaringType.FullName}:{hook.method?.Name}");
			hooks.Add(hook);
		}

		Console.WriteLine($"Processed {hooks.Count:n0} hooks for {hookAssembly.GetName().Name}..\n");
	}

	public static void GenerateHooks()
	{
		var builder = new StringBuilder();
		foreach (var hook in Oxide.Concat(Community).Concat(Base))
		{
			var categoryDirectory = Path.Combine(Generator.Arguments.Docs, "hooks", hook.category);
			if (!Directory.Exists(categoryDirectory))
			{
				Directory.CreateDirectory(categoryDirectory);
			}

			var hookFile = Path.Combine(categoryDirectory, $"{hook.name}{(hook.iteration is 0 ? string.Empty : $" [{hook.iteration}]")}.md");
			{
				if (hook.carbonCompatible)
				{
					builder.Append("<Badge type=\"danger\" text=\"Carbon Compatible\"/>");
				}

				if (hook.oxideCompatible)
				{
					builder.Append("<Badge type=\"warning\" text=\"Oxide Compatible\"/>");
				}
				builder.AppendLine();
				builder.AppendLine($"# {hook.name}");
				if (hook.descriptions != null && hook.descriptions.Any())
				{
					var moreThanOne = hook.descriptions.Length > 1;
					foreach (var description in hook.descriptions)
					{
						builder.AppendLine($"{(moreThanOne ? "- " : string.Empty)}{description}");
					}
				}
				else builder.AppendLine($"No description.");
				builder.AppendLine($"### Return");
				builder.AppendLine($"Returning a non-null value cancels default behavior.");
				builder.AppendLine();
				builder.AppendLine($"### Usage");
				builder.AppendLine($"::: code-group");
				builder.AppendLine($"```csharp [Example]");
				builder.AppendLine($"private {(hook.returnType == null ? "void" : GetType(hook.returnType.FullName))} {hook.name}()");
				builder.AppendLine($"{{");
				builder.AppendLine($"	Puts(\"{hook.name} has been fired!\");");
				if (hook.returnType != null && hook.returnType.GetType() != typeof(void))
				{
					builder.AppendLine($"	return ({GetPrettyTypeName(hook.returnType)})default;");
				}
				builder.AppendLine($"}}");
				builder.AppendLine($"```");

				if (!string.IsNullOrEmpty(hook.methodSource))
				{
					builder.AppendLine($"```csharp [Source — {hook.assembly.GetName().Name} @ {hook.target.FullName}]");
					builder.AppendLine(hook.methodSource);
					builder.AppendLine($"```");
				}
				builder.AppendLine($":::");
			}
			File.WriteAllText(hookFile, builder.ToString());
			builder.Clear();
		}
	}

	#region Helpers

	private static string GetType(string type, string empty = "null")
	{
		if (type == typeof(void).FullName) return "void";
		if (type == typeof(string).FullName) return "string";
		if (type == typeof(uint).FullName) return "uint";
		if (type == typeof(int).FullName) return "int";
		if (type == typeof(double).FullName) return "double";
		if (type == typeof(float).FullName) return "float";
		if (type == typeof(ulong).FullName) return "ulong";
		if (type == typeof(object).FullName) return "object";
		return type == typeof(bool).FullName ? "bool" : type.Replace("+", ".");
	}
	private static string GetParameterName(Type type)
	{
		return $"{char.ToLower(type.Name[0])}{type.Name.Substring(1)}";
	}
	private static string GetPrettyTypeName(Type type, bool fullName = true)
	{
		var name = (fullName ? (type.FullName ?? type.Name) : type.Name).Replace("+", ".");

		if (!type.IsGenericType)
			return name;

		var typeName = new StringBuilder();
		typeName.Append(name.Substring(0, name.IndexOf('`')));
		typeName.Append('<');

		var genericArguments = type.GetGenericArguments();
		for (int i = 0; i < genericArguments.Length; i++)
		{
			if (i > 0)
				typeName.Append(", ");

			typeName.Append(GetType(GetPrettyTypeName(genericArguments[i]), "T"));
		}

		typeName.Append(">");
		return typeName.ToString();
	}

	#endregion
}
