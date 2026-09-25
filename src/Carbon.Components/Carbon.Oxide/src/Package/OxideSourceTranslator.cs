using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Oxide.Compatibility;

/// <summary>
/// Translates Oxide plugin sources to Carbon's native API before they compile:
/// <list type="bullet">
/// <item>fully qualified Oxide names of natively provided types get rewritten (see <see cref="OxideTypeMap"/>),</item>
/// <item>'using Oxide.X;' brings in aliases for the native types that used to live in that namespace,</item>
/// <item>imports of Oxide namespaces that no longer exist get dropped.</item>
/// </list>
/// Registered through <see cref="Carbon.Jobs.ScriptCompilationThread.SourceTransformers"/>.
/// </summary>
public static class OxideSourceTranslator
{
	private const string GlobalPrefix = "global::";

	/// <summary>Oxide namespaces (and their parents) that still contain real types in this package.</summary>
	public static HashSet<string> LiveNamespaces { get; } = CollectLiveNamespaces();

	private static HashSet<string> CollectLiveNamespaces()
	{
		var result = new HashSet<string>();
		Type[] types;

		try
		{
			types = typeof(OxideSourceTranslator).Assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			// Only namespaces matter, whatever loaded is enough
			types = ex.Types.Where(x => x != null).ToArray();
		}

		foreach (var type in types)
		{
			string ns;

			try
			{
				ns = type.Namespace;
			}
			catch
			{
				continue;
			}

			while (!string.IsNullOrEmpty(ns) && (ns == "Oxide" || ns.StartsWith("Oxide.")))
			{
				result.Add(ns);
				var index = ns.LastIndexOf('.');
				ns = index < 0 ? null : ns.Substring(0, index);
			}
		}

		return result;
	}

	public static string Transform(SourceFile source, string content, CSharpParseOptions options)
	{
		// Nothing to translate without an Oxide reference
		if (content.IndexOf("Oxide", StringComparison.Ordinal) < 0)
		{
			return null;
		}

		var root = CSharpSyntaxTree.ParseText(content, options).GetCompilationUnitRoot();
		var result = new Rewriter().Visit(root);

		return result == root ? null : result.ToFullString();
	}

	private sealed class Rewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode VisitQualifiedName(QualifiedNameSyntax node)
		{
			var mapped = OxideTypeMap.Map(Normalize(node));
			return mapped != null ? SyntaxFactory.ParseName(GlobalPrefix + mapped).WithTriviaFrom(node) : base.VisitQualifiedName(node);
		}

		public override SyntaxNode VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
		{
			var mapped = OxideTypeMap.Map(Normalize(node));
			return mapped != null ? SyntaxFactory.ParseName(GlobalPrefix + mapped).WithTriviaFrom(node) : base.VisitAliasQualifiedName(node);
		}

		public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
		{
			// Only pure dotted chains (Oxide.Game.Rust.Cui.CuiHelper) can be type references
			if (IsDottedChain(node))
			{
				var mapped = OxideTypeMap.Map(Normalize(node));
				if (mapped != null)
				{
					return SyntaxFactory.ParseExpression(GlobalPrefix + mapped).WithTriviaFrom(node);
				}
			}

			return base.VisitMemberAccessExpression(node);
		}

		public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
		{
			node = (CompilationUnitSyntax)base.VisitCompilationUnit(node);
			return node.WithUsings(ProcessUsings(node.Usings, implicitNamespace: null, declaredTypes: null));
		}

		public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			var name = FullNamespaceName(node);
			node = (NamespaceDeclarationSyntax)base.VisitNamespaceDeclaration(node);
			return node.WithUsings(ProcessUsings(node.Usings, name, DeclaredTypes(node.Members)));
		}

		public override SyntaxNode VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
		{
			var name = FullNamespaceName(node);
			node = (FileScopedNamespaceDeclarationSyntax)base.VisitFileScopedNamespaceDeclaration(node);
			return node.WithUsings(ProcessUsings(node.Usings, name, DeclaredTypes(node.Members)));
		}

		/// <summary>
		/// Swaps Oxide namespace imports for aliases of the native types that lived there.
		/// Code inside an Oxide namespace implicitly sees that namespace's types, so those get aliases too.
		/// </summary>
		private static SyntaxList<UsingDirectiveSyntax> ProcessUsings(SyntaxList<UsingDirectiveSyntax> usings, string implicitNamespace, HashSet<string> declaredTypes)
		{
			var aliases = new Dictionary<string, string>();
			var existingAliases = new HashSet<string>(usings.Where(x => x.Alias != null).Select(x => x.Alias.Name.Identifier.ValueText));
			var result = new List<UsingDirectiveSyntax>(usings.Count);

			foreach (var directive in usings)
			{
				if (directive.Alias != null || directive.StaticKeyword != default || directive.Name == null)
				{
					result.Add(directive);
					continue;
				}

				var ns = Normalize(directive.Name);
				AddAliases(ns, aliases);

				if (IsOxideNamespace(ns) && !LiveNamespaces.Contains(ns))
				{
					continue;
				}

				result.Add(directive);
			}

			for (var ns = implicitNamespace; !string.IsNullOrEmpty(ns); ns = ns.IndexOf('.') >= 0 ? ns.Substring(0, ns.LastIndexOf('.')) : null)
			{
				AddAliases(ns, aliases, declaredTypes);
			}

			foreach (var alias in aliases)
			{
				if (existingAliases.Contains(alias.Key))
				{
					continue;
				}

				result.Add(SyntaxFactory.UsingDirective(
						SyntaxFactory.NameEquals(alias.Key),
						SyntaxFactory.ParseName(GlobalPrefix + alias.Value))
					.NormalizeWhitespace()
					.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed));
			}

			return SyntaxFactory.List(result);
		}

		private static void AddAliases(string ns, Dictionary<string, string> aliases, HashSet<string> declaredTypes = null)
		{
			if (!OxideTypeMap.ByNamespace.TryGetValue(ns, out var types))
			{
				return;
			}

			foreach (var type in types)
			{
				// Declaring a same-named type in that namespace would conflict with the alias
				if (declaredTypes != null && declaredTypes.Contains(type.Key))
				{
					continue;
				}

				aliases[type.Key] = type.Value;
			}
		}

		private static HashSet<string> DeclaredTypes(SyntaxList<MemberDeclarationSyntax> members)
		{
			return new HashSet<string>(members.OfType<BaseTypeDeclarationSyntax>().Select(x => x.Identifier.ValueText));
		}

		private static string FullNamespaceName(BaseNamespaceDeclarationSyntax node)
		{
			var name = Normalize(node.Name);

			for (var parent = node.Parent as BaseNamespaceDeclarationSyntax; parent != null; parent = parent.Parent as BaseNamespaceDeclarationSyntax)
			{
				name = Normalize(parent.Name) + "." + name;
			}

			return name;
		}

		private static bool IsOxideNamespace(string ns) => ns == "Oxide" || ns.StartsWith("Oxide.", StringComparison.Ordinal);

		private static bool IsDottedChain(ExpressionSyntax expression)
		{
			return expression switch
			{
				IdentifierNameSyntax => true,
				AliasQualifiedNameSyntax => true,
				MemberAccessExpressionSyntax { Name: IdentifierNameSyntax } access => IsDottedChain(access.Expression),
				_ => false
			};
		}

		private static string Normalize(SyntaxNode node)
		{
			var text = node.WithoutTrivia().ToString();
			text = new string(text.Where(c => !char.IsWhiteSpace(c)).ToArray());
			return text.StartsWith(GlobalPrefix, StringComparison.Ordinal) ? text.Substring(GlobalPrefix.Length) : text;
		}
	}
}
