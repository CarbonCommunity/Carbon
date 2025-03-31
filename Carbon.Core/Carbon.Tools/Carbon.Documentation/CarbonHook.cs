using System.Reflection;
using System.Runtime.InteropServices;
using Carbon.Components;
using Carbon.Documentation.Generators;
using Newtonsoft.Json;

namespace Carbon.Documentation;

public struct CarbonHook
{
	public static Dictionary<string, int> iterations = new();

	[Flags]
	public enum HookFlags
	{
		None = 0,
		Static = 1,
		Patch = 2,
		Hidden = 4,
		IgnoreChecksum = 8,
		MetadataOnly = 16
	}

	public string name;
	public string fullName;
	public string category;
	public Parameter[] parameters;
	public HookFlags flags;
	[JsonIgnore]
	public string[] descriptions;
	[JsonIgnore]
	public Type target;
	[JsonIgnore]
	public MethodInfo method;
	[JsonIgnore]
	public Assembly assembly;
	[JsonIgnore]
	public Assembly hooksAssembly;
	[JsonIgnore]
	public Type returnType;
	public bool carbonCompatible;
	public bool oxideCompatible;
	public string methodSource;
	[JsonIgnore]
	public int iteration;

	public string targetName => target?.FullName;
	public string methodName => method?.Name;
	public string assemblyName => assembly?.GetName().Name;
	public string hooksAssemblyName => hooksAssembly?.GetName().Name;
	public string returnTypeName => returnType?.FullName ?? "void";

	[JsonIgnore]
	public readonly bool IsValid => !string.IsNullOrEmpty(name);

	public struct Parameter
	{
		public string name;
		public Type type;
		public bool optional;
	}

	public static CarbonHook Parse(IEnumerable<Attribute> attributes, Assembly referenceAssembly, bool isOxideHooks)
	{
		var patch = attributes.FirstOrDefault(x => x.GetType().Name.Equals("Patch"));
		if (patch == null)
		{
			return default;
		}

		var category = attributes.FirstOrDefault(x => x.GetType().Name.Equals("Category"));
		var returnType = attributes.FirstOrDefault(x => x.GetType().Name.Equals("Return"));
		var optionsType = attributes.FirstOrDefault(x => x.GetType().Name.Equals("Options"));
		var parameters = attributes.Where(x => x.GetType().Name.Equals("Parameter"));
		var isOxideCompatbile = attributes.FirstOrDefault(x => x.GetType().Name.Equals("OxideCompatible")) != null;

		Type patchType = patch.GetType();
		Type parametersType = parameters.FirstOrDefault()?.GetType();
		CarbonHook hook = default;
		string methodName = patchType?.GetProperty("Method").GetValue(patch) as string;
		Type[] methodArgs = patchType?.GetProperty("MethodArgs").GetValue(patch) as Type[];
		hook.name = patchType.GetProperty("Name").GetValue(patch) as string;
		hook.fullName = patchType.GetProperty("FullName").GetValue(patch) as string;
		if (iterations.TryGetValue(hook.fullName, out var iteration))
		{
			hook.fullName += $" [{iteration:n0}]";
			iterations[hook.fullName] = iteration + 1;
		}
		else iterations[hook.fullName] = 1;
		hook.target = patchType.GetProperty("Target").GetValue(patch) as Type;
		hook.assembly = hook.target?.Assembly;
		hook.returnType = returnType?.GetType().GetProperty("Type").GetValue(returnType) as Type;
		hook.carbonCompatible = true;
		hook.oxideCompatible = isOxideHooks || isOxideCompatbile;
		hook.hooksAssembly = referenceAssembly;
		if (optionsType != null)
		{
			hook.flags = (HookFlags)optionsType.GetType().GetProperty("Value").GetValue(optionsType);
		}
		hook.parameters = parameters.Select(x =>
		{
			var name = parametersType.GetProperty("Name").GetValue(x) as string;
			var type = parametersType.GetProperty("Type").GetValue(x) as Type;
			if (name.Equals("self", StringComparison.CurrentCultureIgnoreCase))
			{
				name = char.ToLower(type.Name[0]) + type.Name.Substring(1, type.Name.Length - 1);
			}

			return new Parameter
			{
				name = name,
				type = type,
				optional = (bool)parametersType.GetProperty("Optional").GetValue(x)
			};
		}).ToArray();
		var researchedHook = HooksAIResearch.hooks.FirstOrDefault(x => x.hook.Equals(hook.name));
		if (!string.IsNullOrEmpty(researchedHook.hook))
		{
			hook.descriptions = researchedHook.descriptions;
		}
		if (!string.IsNullOrEmpty(methodName))
		{
			if (methodArgs == null)
			{
				hook.method = hook.target?.GetMethod(methodName, 0) ?? hook.target?.GetMethods().FirstOrDefault(x => x.Name.Equals(methodName));
			}
			else
			{
				hook.method = hook.target?.GetMethod(methodName, 0, methodArgs) ?? hook.target?.GetMethods().FirstOrDefault(x => x.Name.Equals(methodName));
			}
		}
		hook.category = category?.GetType().GetProperty("Name").GetValue(category) as string ?? "Global";
		if (hook.assembly != null && hook.target != null && hook.method != null)
		{
			hook.methodSource = SourceCodeBank.Parse(hook.assembly.Location).ParseMethod(hook.target.FullName, hook.method.Name);
		}

		return hook;
	}
}
