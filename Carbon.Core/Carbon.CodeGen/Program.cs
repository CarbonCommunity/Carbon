using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Carbon.Core;
using Carbon.Extended;
using Carbon.Extensions;
using Carbon.Oxide.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core.Libraries;

namespace Carbon.CodeGen
{
	internal class Program
	{
		static void Main(string[] args)
		{
			DoExtendedDocs();
			DoHookDocs();

			Console.ReadLine();
		}

		public static string GetExample(Hook hook, Hook.Parameter[] parameters)
		{
			return $@"{{% code title=""Example"" %}}
```csharp
{(hook.ReturnType == typeof(void) ? "void" : "object")} {hook.Name} ( {parameters.Select(x => $"{GetType(x.Type)} {(x.Name == "this" ? GetParameterName(x.Type).Trim() : x.Name)}").ToArray().ToString(", ")} )
{{
    Puts ( ""{hook.Name} works!"" );" + (hook.ReturnType == typeof(void) ? "" : $@"
    return ({GetType(hook.ReturnType)}) null;") + $@"
}}
```
{{% endcode %}}";
		}
		public static string GetType(Type type)
		{
			if (type == null) return "null";

			if (type == typeof(void)) return "void";
			else if (type == typeof(string)) return "string";
			else if (type == typeof(uint)) return "uint";
			else if (type == typeof(int)) return "int";
			else if (type == typeof(ulong)) return "ulong";
			else if (type == typeof(object)) return "object";
			else if (type == typeof(bool)) return "bool";

			return type.FullName.Replace("+", ".");
		}
		public static string GetParameterName(Type type)
		{
			return $"{char.ToLower(type.Name[0])}{type.Name.Substring(1)}";
		}

		public static void DoExtendedDocs()
		{
			var assembly = typeof(Hammer_DoAttackShared).Assembly;
			var result = $@"---
description: >-
  This is a solution to your hook problems. Carbon.Extended provides an
  extensive amount of hooks that work with most Oxide plugins, and more!
---

# Carbon.Extended

## Download

Get the latest version of Carbon.Extended [**here**](https://github.com/Carbon-Modding/Carbon.Core/releases/latest/download/Carbon.Extended.dll)!

# Hooks
";
			var categories = new Dictionary<Hook.Category.Enum, List<Type>>();

			foreach (var type in assembly.GetTypes())
			{
				var list = (List<Type>)null;
				var category = type.GetCustomAttribute<Hook.Category>();
				if (category == null) continue;

				if (!categories.TryGetValue(category.Value, out list))
				{
					categories.Add(category.Value, list = new List<Type>());
				}

				list.Add(type);
			}

			foreach (var category in categories)
			{
				result += $"## {category.Key}\n";

				foreach (var entry in category.Value)
				{
					var hook = entry.GetCustomAttribute<Hook>();
					var info = entry.GetCustomAttributes<Hook.Info>();
					var parameters = entry.GetCustomAttributes<Hook.Parameter>();
					var require = entry.GetCustomAttribute<Hook.Require>();

					var resultInfo = new List<string>();
					foreach (var e in info) resultInfo.Add($"<li>{e.Value}</li>");
					if (!resultInfo.Any(x => x.StartsWith("<li>Return")))
					{
						if (hook.ReturnType == typeof(void)) resultInfo.Add($"<li>No return behavior.</li>");
						else resultInfo.Add($"<li>Returning a non-null value cancels default behavior.</li>");
					}

					if (hook is CarbonHook) resultInfo.Add($"<li>This is a <b>Carbon</b>-only compatible hook.</li>");
					else resultInfo.Add($"<li>This hook is compatible within <b>Oxide</b> and <b>Carbon</b>.</li>");

					Console.WriteLine($"{hook.Name} -> {GetType(hook.ReturnType)}");

					result += $@"
### {hook.Name}{(category.Value.Count(x => x.GetCustomAttribute<Hook>().Name == hook.Name) > 1 ? $" ({GetType(parameters.FirstOrDefault(x => x.Name == "this")?.Type)})" : "")}
{resultInfo.ToArray().ToString("\n\n")}

{GetExample(hook, parameters.ToArray())}

{(require == null ? "" : $"This hook requires <b>{require.Hook}</b>, which loads alongside <b>{hook.Name}</b>.")}

";
				}
			}

			OsEx.File.Create("Carbon Hooks.md", result);
		}
		public static void DoHookDocs()
		{
			new WebRequests().Enqueue("https://umod.org/documentation/hooks/rust.json", null, (error, data) =>
			{
				var jobject = JsonConvert.DeserializeObject<JObject>(data);

				var result = $@"---
description: >-
  This is a solution to your hook problems. Carbon.Extended provides an
  extensive amount of hooks that work with most Oxide plugins, and more!
---

# Carbon.Extended

## Download

Get the latest version of Carbon.Extended [**here**](https://github.com/Carbon-Modding/Carbon.Core/releases/latest/download/Carbon.Extended.dll)!

# Hooks
";

				var categories = new Dictionary<string, List<JObject>>();

				foreach (var entry in jobject["data"])
				{
					var subcategory = entry["subcategory"].ToString();
					var list = (List<JObject>)null;

					if (!categories.TryGetValue(subcategory, out list))
					{
						categories.Add(subcategory, list = new List<JObject>());
					}

					list.Add(entry.ToObject<JObject>());
				}

				foreach (var category in categories)
				{
					result += $"## {category.Key}\n";

					foreach (var entry in category.Value)
					{
						result += $@"
### {entry["name"]}
{entry["description"]}

{entry["example"]}

";
					}
				}

				OsEx.File.Create("Oxide Hooks.md", result);
			}, null);
		}
		public static void DoHooks()
		{
			var hooks = JsonConvert.DeserializeObject<HookPackage>(OsEx.File.ReadText("..\\..\\..\\..\\Tools\\Rust.opj"));
			var hookFolder = "..\\..\\..\\Carbon.Extended\\Generated Hooks";
			var rust = typeof(Bootstrap).Assembly;

			OsEx.Folder.Create(hookFolder, true);

			foreach (var manifest in hooks.Manifests)
			{
				foreach (var hook in manifest.Hooks)
				{
					try
					{
						var output = string.Empty;
						var hookName = (string.IsNullOrEmpty(hook.Hook.BaseHookName) ? hook.Hook.HookName : hook.Hook.BaseHookName).Split(' ')[0];
						if (hookName.Contains("/") /*|| !WhitelistedHooks.Contains ( hookName )*/ ) continue;

						var typeName = hook.Hook.TypeName.Replace("/", ".").Split(new string[] { ".<" }, StringSplitOptions.RemoveEmptyEntries)[0];
						var methodName = hook.Hook.Signature.Name.Contains("<Start") ? "Start" : hook.Hook.Signature.Name;
						var method = rust.GetType(typeName).GetMethod(methodName);
						var parameters = method.GetParameters().Select(x => $"{x.ParameterType.FullName.Replace("+", ".")} {x.Name}").ToArray();
						var arguments = hook.Hook.ArgumentString;

						if (hook.Hook.Instructions != null && hook.Hook.Instructions.Length > 0)
						{

						}
						else if (hook.Hook.InjectionIndex > 0 && !hookName.StartsWith("Can")) output = (method.ReturnType == typeof(void) ?
								GetTemplate_NoReturn("Postfix", parameters.ToString(", "), arguments) :
								GetTemplate_Return("Postfix", parameters.ToString(", "), method.ReturnType.FullName, arguments))
								.Replace("[TYPE]", typeName)
								.Replace("[METHOD]", methodName)
								.Replace("[HOOK]", hookName);
						else output = (method.ReturnType == typeof(void) ?
								GetTemplate_NoReturn("Prefix", parameters.ToString(", "), arguments) :
								GetTemplate_Return("Prefix", parameters.ToString(", "), method.ReturnType.FullName, arguments))
								.Replace("[TYPE]", typeName)
								.Replace("[METHOD]", methodName)
								.Replace("[HOOK]", hookName);

						Console.WriteLine($"{typeName}.{methodName} -> {hookName} {arguments}");

						if (output.Length > 0) OsEx.File.Create(Path.Combine(hookFolder, $"{hookName}.cs"), output);
					}
					catch { }
				}
			}
		}
		public static string GetTemplate_NoReturn(string method, string parameters = null, string arguments = null)
		{
			return $@"using Carbon.Core;
using HarmonyLib;

namespace Carbon.Extended
{{
    [HarmonyPatch ( typeof ( [TYPE] ), ""[METHOD]"" )]
    public class [HOOK]
    {{
        public static {(method == "Prefix" ? "bool" : "void")} {method} ( {(string.IsNullOrEmpty(parameters) ? "" : $"{parameters} ")}{(arguments != null && arguments.Contains("this") ? $", ref [TYPE] __instance " : "")})
        {{
            {(method == "Prefix" ? $"return HookExecutor.CallStaticHook ( \"[HOOK]\"{(string.IsNullOrEmpty(arguments) ? "" : $", {arguments.Replace("this", "__instance")}")} ) == null;" : "HookExecutor.CallStaticHook ( \"[HOOK]\" );")}
        }}
    }}
}}";
		}
		public static string GetTemplate_Return(string method, string parameters = null, string returnType = "object", string arguments = null)
		{
			return $@"using Carbon.Core;
using HarmonyLib;

namespace Carbon.Extended
{{
    [HarmonyPatch ( typeof ( [TYPE] ), ""[METHOD]"" )]
    public class [HOOK]
    {{
        public static bool {method} ( {(string.IsNullOrEmpty(parameters) ? "" : $"{parameters}, ")}ref {returnType} __result{(arguments != null && arguments.Contains("this") ? $", ref [TYPE] __instance" : "")} )
        {{
            CarbonCore.Log ( $""{method} [HOOK]"" );

            var result = HookExecutor.CallStaticHook ( ""[HOOK]""{(string.IsNullOrEmpty(arguments) ? "" : $", {arguments.Replace("this", "__instance")}")} );
            
            if ( result != null )
            {{
                __result = ( {returnType} ) result;
                return false;
            }}

            return true;
        }}
    }}
}}";
		}
	}
}
