using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text;
using Carbon.InternalCallHookGeneration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Carbon.SourceGenerators;

[Generator]
public sealed class InternalCallHookGenerator : IIncrementalGenerator
{
	private const string BaseModuleType = "Carbon.Base.BaseModule";
	private const string CarbonPluginType = "Carbon.Plugins.CarbonPlugin";
	private const string HookMethodAttributeName = "HookMethodAttribute";
	private const string ConditionalAttributeType = "Carbon.ConditionalAttribute";

	private static readonly SymbolDisplayFormat TypeFormat = new(
		SymbolDisplayGlobalNamespaceStyle.Included,
		SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
		SymbolDisplayGenericsOptions.IncludeTypeParameters,
		miscellaneousOptions:
		SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
		SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var targetTypes = context.SyntaxProvider
			.CreateSyntaxProvider(static (node, _) => IsCandidate(node), static (ctx, ct) => GetTargetType(ctx, ct))
			.Where(static symbol => symbol is not null)
			.Collect();

		context.RegisterSourceOutput(targetTypes, static (sourceContext, symbols) => Emit(sourceContext, symbols));
	}

	private static bool IsCandidate(SyntaxNode node)
	{
		return node is ClassDeclarationSyntax classDeclaration &&
		       classDeclaration.Modifiers.Any(static modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
	}

	private static INamedTypeSymbol? GetTargetType(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (context.Node is not ClassDeclarationSyntax classDeclaration)
		{
			return null;
		}

		if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclaration, cancellationToken) is not INamedTypeSymbol typeSymbol)
		{
			return null;
		}

		if (typeSymbol.TypeKind != TypeKind.Class)
		{
			return null;
		}

		if (!DerivesFrom(typeSymbol, BaseModuleType) && !DerivesFrom(typeSymbol, CarbonPluginType))
		{
			return null;
		}

		if (HasInternalCallHookOverride(typeSymbol))
		{
			return null;
		}

		return typeSymbol;
	}

	private static void Emit(SourceProductionContext context, ImmutableArray<INamedTypeSymbol?> symbols)
	{
		var seen = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

		foreach (var symbol in symbols)
		{
			if (symbol is null || !seen.Add(symbol))
			{
				continue;
			}

			var methods = GetHookMethods(symbol);
			if (methods.Count == 0)
			{
				continue;
			}

			var model = CreateModel(symbol, methods);
			var source = InternalCallHookEmitter.BuildSource(model);
			var hintName = $"{SanitizeHintName(symbol.ToDisplayString())}.InternalCallHook.g.cs";
			context.AddSource(hintName, SourceText.From(source, Encoding.UTF8));
		}
	}

	private static InternalCallHookTypeModel CreateModel(INamedTypeSymbol typeSymbol, List<IMethodSymbol> methods)
	{
		var isModule = DerivesFrom(typeSymbol, BaseModuleType);
		var model = new InternalCallHookTypeModel
		{
			NamespaceName = typeSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : typeSymbol.ContainingNamespace.ToDisplayString(),
			TypeName = typeSymbol.Name,
			BaseKind = isModule ? "module" : "plugin",
			VersionOwnerExpression = isModule ? "this" : "base",
		};

		foreach (var method in methods)
		{
			var hook = new InternalCallHookMethodModel
			{
				MethodName = method.Name,
				HookName = ResolveHookName(method),
				ReturnsVoid = method.ReturnsVoid,
				Score = GetMethodParameterDepthScore(method),
				ConditionalSymbol = GetConditionalSymbol(method),
			};

			hook.HookId = ComputeHookId(hook.HookName);

			foreach (var parameter in method.Parameters)
			{
				var isOut = parameter.RefKind == RefKind.Out;
				var useInlineDefaultExpression = parameter.HasExplicitDefaultValue || parameter.NullableAnnotation == NullableAnnotation.Annotated;
				hook.Parameters.Add(new InternalCallHookParameterModel
				{
					TypeName = parameter.Type.ToDisplayString(TypeFormat),
					IsOut = isOut,
					IsRef = parameter.RefKind == RefKind.Ref,
					UseInlineDefaultExpression = useInlineDefaultExpression,
					RequiresGuard = !useInlineDefaultExpression && !isOut,
				});
			}

			model.Methods.Add(hook);
		}

		return model;
	}

	private static List<IMethodSymbol> GetHookMethods(INamedTypeSymbol typeSymbol)
	{
		return typeSymbol.GetMembers()
			.OfType<IMethodSymbol>()
			.Where(static method =>
				method.MethodKind == MethodKind.Ordinary &&
				!method.IsImplicitlyDeclared &&
				!method.IsStatic &&
				method.TypeParameters.Length == 0 &&
				(method.DeclaredAccessibility != Accessibility.Public || HasHookMethodAttribute(method)) &&
				method.Name != "InternalCallHook")
			.OrderBy(static method => ResolveHookName(method), StringComparer.Ordinal)
			.ThenBy(static method => method.Name, StringComparer.Ordinal)
			.ToList();
	}

	private static bool HasInternalCallHookOverride(INamedTypeSymbol typeSymbol)
	{
		return typeSymbol.GetMembers("InternalCallHook")
			.OfType<IMethodSymbol>()
			.Any(static method =>
				method.MethodKind == MethodKind.Ordinary &&
				method.Parameters.Length == 2 &&
				method.IsOverride);
	}

	private static bool DerivesFrom(INamedTypeSymbol typeSymbol, string fullyQualifiedBaseType)
	{
		for (var current = typeSymbol; current is not null; current = current.BaseType)
		{
			if (current.ToDisplayString() == fullyQualifiedBaseType)
			{
				return true;
			}
		}

		return false;
	}

	private static bool HasAttribute(IMethodSymbol method, string fullyQualifiedAttributeType)
	{
		return method.GetAttributes().Any(attribute => attribute.AttributeClass?.ToDisplayString() == fullyQualifiedAttributeType);
	}

	private static bool HasHookMethodAttribute(IMethodSymbol method)
	{
		return method.GetAttributes().Any(static attribute => attribute.AttributeClass?.Name == HookMethodAttributeName);
	}

	private static string ResolveHookName(IMethodSymbol method)
	{
		var attribute = method.GetAttributes().FirstOrDefault(static attribute => attribute.AttributeClass?.Name == HookMethodAttributeName);
		if (attribute == null)
		{
			return method.Name;
		}

		if (attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is string constructorValue &&
		    !string.IsNullOrWhiteSpace(constructorValue))
		{
			return constructorValue;
		}

		foreach (var argument in attribute.NamedArguments)
		{
			if (argument is { Key: "Name", Value.Value: string namedValue } && !string.IsNullOrWhiteSpace(namedValue))
			{
				return namedValue;
			}
		}

		return method.Name;
	}

	private static string GetConditionalSymbol(IMethodSymbol method)
	{
		var attribute = method.GetAttributes().FirstOrDefault(current => current.AttributeClass?.ToDisplayString() == ConditionalAttributeType);
		if (attribute == null)
		{
			return string.Empty;
		}

		if (attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is string constructorValue &&
		    !string.IsNullOrWhiteSpace(constructorValue))
		{
			return constructorValue;
		}

		foreach (var argument in attribute.NamedArguments)
		{
			if (argument is { Key: "Symbol", Value.Value: string namedValue } && !string.IsNullOrWhiteSpace(namedValue))
			{
				return namedValue;
			}
		}

		return string.Empty;
	}

	private static int GetMethodParameterDepthScore(IMethodSymbol method)
	{
		var score = 0;
		foreach (var parameter in method.Parameters)
		{
			score += GetInheritanceDepth(parameter.Type);
		}

		return score;
	}

	private static int GetInheritanceDepth(ITypeSymbol typeSymbol)
	{
		var depth = 0;
		for (var current = typeSymbol.BaseType; current is not null; current = current.BaseType)
		{
			depth++;
		}

		return depth;
	}

	private static uint ComputeHookId(string hookName)
	{
		if (string.IsNullOrEmpty(hookName))
		{
			return 0;
		}

		using var md5 = MD5.Create();
		return BitConverter.ToUInt32(md5.ComputeHash(Encoding.UTF8.GetBytes(hookName)), 0);
	}

	private static string SanitizeHintName(string input)
	{
		var builder = new StringBuilder(input.Length);
		foreach (var character in input)
		{
			builder.Append(char.IsLetterOrDigit(character) ? character : '_');
		}

		return builder.ToString();
	}
}
