using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using Carbon.InternalCallHookGeneration;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil;

namespace Carbon.Generator;

#pragma warning disable

public class InternalCallHook
{
	public static List<AssemblyDefinition> Assemblies = new();

	public static ConcurrentDictionary<string, int> InheritanceCache = new();

	public static TypeDefinition FindTypeInAssemblies(string fullName)
	{
		for (int i = 0; i < Assemblies.Count; i++)
		{
			var assembly = Assemblies[i];
			var type = assembly.MainModule.GetType(fullName);
			if (type != null)
			{
				return type;
			}

			for (int a = 0; a < assembly.MainModule.Types.Count; a++)
			{
				type = assembly.MainModule.Types[a];
				if (type.FullName == fullName || type.Name == fullName)
				{
					return type;
				}
			}
		}

		return null;
	}

	public static int GetInheritanceDepth(TypeDefinition type)
	{
		if (InheritanceCache.TryGetValue(type.FullName, out int depth))
		{
			return depth;
		}

		var current = type.BaseType;

		while (current != null)
		{
			var resolved = current.Resolve();
			if (resolved == null)
				break;

			depth++;
			current = resolved.BaseType;
		}
		InheritanceCache[type.FullName] = depth;
		return depth;
	}

	public static int GetMethodParameterDepthScore(MethodDeclarationSyntax method)
	{
		var totalDepth = 0;

		for (int i = 0; i < method.ParameterList.Parameters.Count; i++)
		{
			var param = method.ParameterList.Parameters[i];
			if (param.Type == null)
			{
				continue;
			}
			var type = FindTypeInAssemblies(param.Type.ToString());
			if (type != null)
			{
				totalDepth += GetInheritanceDepth(type);
			}
		}

		return totalDepth;
	}

	public static void GeneratePartial(CompilationUnitSyntax input, out CompilationUnitSyntax output, CSharpParseOptions options, string fileName, List<ClassDeclarationSyntax> classes = null, string debugOutputPath = null, List<string> usingsList = null, IEnumerable<MetadataReference> references = null)
	{
		BaseNamespaceDeclarationSyntax @namespace;

		if (classes == null)
		{
			classes = new List<ClassDeclarationSyntax>();
			FindPluginInfo(input, out @namespace, out _, out _, classes);
		}
		else
		{
			@namespace = classes[0].Parent as BaseNamespaceDeclarationSyntax;
		}

		if (classes.Count == 0)
		{
			output = null;
			return;
		}

		var model = CreateModel(input, @namespace, classes, usingsList, references, options);
		if (model.Methods.Count == 0)
		{
			output = null;
			return;
		}

		var source = InternalCallHookEmitter.BuildSource(model);
		string path;

#if DEBUG
		var isPartial = classes[0].Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword));
		if (isPartial)
		{
			var fileNameWithNewExt = $"{Path.GetFileNameWithoutExtension(fileName)}.Internal.cs";
			path = debugOutputPath != null ? Path.Combine(debugOutputPath, fileNameWithNewExt) : fileNameWithNewExt;
			output = CSharpSyntaxTree.ParseText(source, options, path, Encoding.UTF8).GetCompilationUnitRoot().NormalizeWhitespace();
			File.WriteAllText(path, output.ToFullString());
		}
		else
		{
			path = $"{fileName}/Internal";
			output = CSharpSyntaxTree.ParseText(source, options, path, Encoding.UTF8).GetCompilationUnitRoot();
		}
#else
		path = $"{fileName}/Internal";
		output = CSharpSyntaxTree.ParseText(source, options, path, Encoding.UTF8).GetCompilationUnitRoot();
#endif
	}

	private static InternalCallHookTypeModel CreateModel(
		CompilationUnitSyntax input, BaseNamespaceDeclarationSyntax @namespace, List<ClassDeclarationSyntax> classes, List<string> usingsList,
		IEnumerable<MetadataReference> references, CSharpParseOptions options
	)
	{
		var model = new InternalCallHookTypeModel
		{
			NamespaceName = @namespace?.Name.ToString() ?? string.Empty,
			TypeName = classes[0].Identifier.ValueText,
			BaseKind = "plugin",
			VersionOwnerExpression = "base"
		};

		model.GlobalUsings.AddRange(input.Usings.Select(x => x.ToString()));
		if (usingsList != null)
		{
			model.GlobalUsings.AddRange(usingsList);
		}

		if (@namespace != null)
		{
			model.NamespaceUsings.AddRange(@namespace.Usings.Select(x => x.ToString()));
		}

		var methods = classes
			.SelectMany(x => x.ChildNodes().OfType<MethodDeclarationSyntax>())
			.Where(IsHookableMethod)
			.OrderBy(x => x.Identifier.ValueText)
			.ToArray();
		var refLikeMethods = GetRefLikeMethodKeys(input, references, options, methods);

		foreach (var method in methods)
		{
			if (refLikeMethods != null && refLikeMethods.Contains(GetMethodKey(method)))
			{
				continue;
			}

			var hookName = ResolveHookName(method, classes);
			var hook = new InternalCallHookMethodModel
			{
				MethodName = method.Identifier.ValueText,
				HookName = hookName,
				HookId = HookStringPool.GetOrAdd(hookName),
				ReturnsVoid = method.ReturnType.ToString() == "void",
				Score = GetMethodParameterDepthScore(method),
				ConditionalSymbol = GetConditionalSymbol(method)
			};

			foreach (var parameter in method.ParameterList.Parameters)
			{
				if (parameter.Type == null)
				{
					continue;
				}

				var isOut = parameter.Modifiers.Any(x => x.IsKind(SyntaxKind.OutKeyword));
				var useInlineDefaultExpression = parameter.Default != null || parameter.Type is NullableTypeSyntax;
				hook.Parameters.Add(new InternalCallHookParameterModel
				{
					TypeName = (parameter.Type is TupleTypeSyntax tuple
						? $"({string.Join(", ", tuple.Elements.Select(e => e.Type.ToString()))})"
						: parameter.Type.ToString()).Replace("global::", string.Empty),
					IsOut = isOut,
					IsRef = parameter.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)),
					UseInlineDefaultExpression = useInlineDefaultExpression,
					RequiresGuard = !useInlineDefaultExpression && !isOut
				});
			}

			model.Methods.Add(hook);
		}

		return model;
	}

	private static bool IsHookableMethod(MethodDeclarationSyntax method)
	{
		return (method.Modifiers.Count == 0 ||
		        method.Modifiers.All(modifier => !modifier.IsKind(SyntaxKind.PublicKeyword) && !modifier.IsKind(SyntaxKind.StaticKeyword)) ||
		        method.AttributeLists.Any(x => x.Attributes.Any(y => y.Name.ToString() == "HookMethod"))) && method.TypeParameterList == null;
	}

	private static HashSet<string> GetRefLikeMethodKeys(CompilationUnitSyntax input, IEnumerable<MetadataReference> references, CSharpParseOptions options, IReadOnlyCollection<MethodDeclarationSyntax> methods)
	{
		if (methods.Count == 0 || !methods.Any(CanHaveRefLikeSignature) || references == null || options == null)
		{
			return null;
		}

		var methodKeys = new HashSet<string>(methods.Select(GetMethodKey));
		var tree = CSharpSyntaxTree.Create(input, options);
		var compilation = CSharpCompilation.Create(
			"Carbon.InternalCallHook.Analysis",
			new[] { tree },
			references,
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true));

		var model = compilation.GetSemanticModel(tree, true);
		HashSet<string> refLikeMethods = null;

		foreach (var method in tree.GetCompilationUnitRoot().DescendantNodes().OfType<MethodDeclarationSyntax>())
		{
			if (!methodKeys.Contains(GetMethodKey(method)))
			{
				continue;
			}

			var methodSymbol = model.GetDeclaredSymbol(method);
			if (methodSymbol == null || !HasRefLikeSignature(methodSymbol))
			{
				continue;
			}

			refLikeMethods ??= [];
			refLikeMethods.Add(GetMethodKey(method));
		}

		return refLikeMethods;
	}

	private static bool CanHaveRefLikeSignature(MethodDeclarationSyntax method)
	{
		return CanBeRefLikeType(method.ReturnType) || method.ParameterList.Parameters.Any(x => CanBeRefLikeType(x.Type));
	}

	private static bool CanBeRefLikeType(TypeSyntax type)
	{
		return type is not null and not PredefinedTypeSyntax;
	}

	private static bool HasRefLikeSignature(IMethodSymbol method)
	{
		return method.ReturnType.IsRefLikeType || method.Parameters.Any(x => x.Type.IsRefLikeType);
	}

	public static bool HasRefLikeSignature(MethodInfo method)
	{
		return IsRefLikeType(method.ReturnType) || method.GetParameters().Any(x => IsRefLikeType(x.ParameterType));
	}

	private static bool IsRefLikeType(Type type)
	{
		while (type is { HasElementType: true })
		{
			type = type.GetElementType();
		}

		if (type == null)
		{
			return false;
		}

		try
		{
			return type.GetCustomAttributesData().Any(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.IsByRefLikeAttribute");
		}
		catch
		{
			return false;
		}
	}

	private static string GetMethodKey(MethodDeclarationSyntax method)
	{
		return $"{GetContainingTypeKey(method)}|{method.Identifier.ValueText}|{method.ReturnType}|{string.Join(",", method.ParameterList.Parameters.Select(GetParameterKey))}";
	}

	private static string GetContainingTypeKey(MethodDeclarationSyntax method)
	{
		return string.Join(".", method.Ancestors().OfType<TypeDeclarationSyntax>().Reverse().Select(x => x.Identifier.ValueText));
	}

	private static string GetParameterKey(ParameterSyntax parameter)
	{
		return $"{string.Join(" ", parameter.Modifiers.Select(x => x.Text))}:{parameter.Type}";
	}

	private static string ResolveHookName(MethodDeclarationSyntax method, List<ClassDeclarationSyntax> classes)
	{
		var hookMethod = method.AttributeLists.Select(x => x.Attributes.FirstOrDefault(x => x.Name.ToString() == "HookMethod")).FirstOrDefault();
		var methodName = hookMethod != null && hookMethod.ArgumentList?.Arguments.Count > 0
			? hookMethod.ArgumentList.Arguments[0].ToString().Replace("\"", string.Empty)
			: method.Identifier.ValueText;

		if (hookMethod == null || hookMethod.ArgumentList?.Arguments.Count == 0)
		{
			return methodName;
		}

		var context = hookMethod.ArgumentList.Arguments[0];
		var contextString = context.ToString();
		if (contextString.Contains("nameof"))
		{
			methodName = contextString.Replace("nameof", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
			if (methodName.Contains("."))
			{
				var split = methodName.Split('.');
				methodName = split[^1];
			}

			return methodName;
		}

		if (contextString.Contains("."))
		{
			var argument = context.Expression as MemberAccessExpressionSyntax;
			var expression = argument?.Expression.ToString();
			var name = argument?.Name.ToString();
			var value = AccessTools.Field(AccessTools.TypeByName(expression), name)?.GetValue(null)?.ToString();
			if (!string.IsNullOrEmpty(value))
			{
				return value;
			}
		}

		if (contextString.Contains("\""))
		{
			var value = AccessTools.Field(AccessTools.TypeByName(classes[0].Identifier.Text), contextString.Replace("\"", string.Empty))?.GetValue(null)
				?.ToString();
			if (!string.IsNullOrEmpty(value))
			{
				return value;
			}
		}

		return methodName;
	}

	private static string GetConditionalSymbol(MethodDeclarationSyntax method)
	{
		var conditional = method.AttributeLists
			.SelectMany(x => x.Attributes)
			.FirstOrDefault(x => x.Name.ToString() == "Conditional")
			?.ArgumentList?.Arguments.FirstOrDefault()
			?.ToString();

		return conditional?.Replace("\"", string.Empty) ?? string.Empty;
	}

	public static bool FindPluginInfo(CompilationUnitSyntax input, out BaseNamespaceDeclarationSyntax @namespace, out int namespaceIndex, out int classIndex, List<ClassDeclarationSyntax> classes)
	{
		var @class = (ClassDeclarationSyntax)null;
		@namespace = null;
		namespaceIndex = 0;
		classIndex = 0;

		for (int n = 0; n < input.Members.Count; n++)
		{
			var memberA = input.Members[n];

			if (memberA is not BaseNamespaceDeclarationSyntax ns)
			{
				continue;
			}

			for (int c = 0; c < ns.Members.Count; c++)
			{
				var memberB = ns.Members[c];

				if (memberB is not ClassDeclarationSyntax cls)
				{
					continue;
				}

				if (cls.AttributeLists.Count > 0)
				{
					foreach (var attribute in cls.AttributeLists)
					{
						if (attribute.Attributes[0].Name is IdentifierNameSyntax nameSyntax && nameSyntax.Identifier.Text.Equals("Info"))
						{
							namespaceIndex = n;
							@namespace = ns;
							classIndex = c;
							@class = cls;
							classes?.Insert(0, @class);
						}
					}
				}
				else if (cls.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword)))
				{
					classes?.Add(cls);
				}
			}
		}

		return @class != null;
	}
}
