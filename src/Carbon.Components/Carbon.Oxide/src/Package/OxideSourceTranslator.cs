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

	/// <summary>Every Oxide namespace a plugin may name: the live ones plus those the renamed types came from.</summary>
	public static HashSet<string> KnownNamespaces { get; } = CollectKnownNamespaces();

	private static HashSet<string> CollectKnownNamespaces()
	{
		var result = new HashSet<string>(LiveNamespaces);

		foreach (var type in OxideTypeMap.Types.Keys)
		{
			for (var index = type.LastIndexOf('.'); index > 0; index = type.LastIndexOf('.', index - 1))
			{
				result.Add(type.Substring(0, index));
			}
		}

		return result;
	}

	/// <summary>Native types the renamed Oxide types map to, as (namespace, name).</summary>
	private static readonly HashSet<(string, string)> NativeTypes = new(OxideTypeMap.Types.Values.Select(SplitTypeName));

	/// <summary>Segments a name relative to an Oxide namespace can start with ('Core' in 'Core.Libraries.Permission').</summary>
	private static readonly HashSet<string> RelativeHeads = new(KnownNamespaces.SelectMany(x => x.Split('.').Skip(1)));

	// Types declared across a multi-file plugin, as (namespace, name). Every file of a batch is transformed in order
	// on the compiling thread, so the set is built for its first file and handed back to the pool after its last one.
	[ThreadStatic] private static IReadOnlyList<SourceFile> _batch;
	[ThreadStatic] private static PooledHashSet<(string, string)> _batchTypes;

	public static string Transform(SourceFile source, string content, CSharpParseOptions options)
	{
		var batch = source?.Batch is { Count: > 1 } files ? files : null;
		Dictionary<string, string> namespaceAliases = null;

		try
		{
			// Nothing to translate without an Oxide reference
			if (content.IndexOf("Oxide", StringComparison.Ordinal) < 0)
			{
				return null;
			}

			var root = CSharpSyntaxTree.ParseText(content, options).GetCompilationUnitRoot();
			CollectNamespaceAliases(root.Usings, root.Members, ref namespaceAliases);

			if (batch != null)
			{
				return Rewrite(root, namespaceAliases, GetBatchTypes(batch, options));
			}

			using var declaredTypes = Facepunch.Pool.Get<PooledHashSet<(string, string)>>();
			CollectDeclaredTypes(root.Members, null, declaredTypes);
			return Rewrite(root, namespaceAliases, declaredTypes);
		}
		finally
		{
			if (namespaceAliases != null)
			{
				Facepunch.Pool.FreeUnmanaged(ref namespaceAliases);
			}

			if (batch != null && ReferenceEquals(source, batch[batch.Count - 1]))
			{
				ReleaseBatch();
			}
		}
	}

	private static string Rewrite(CompilationUnitSyntax root, Dictionary<string, string> namespaceAliases, HashSet<(string, string)> declaredTypes)
	{
		var result = new Rewriter(namespaceAliases, declaredTypes).Visit(root);
		return result == root ? null : result.ToFullString();
	}

	private static HashSet<(string, string)> GetBatchTypes(IReadOnlyList<SourceFile> batch, CSharpParseOptions options)
	{
		if (ReferenceEquals(_batch, batch))
		{
			return _batchTypes;
		}

		// A batch whose last file never reached the translator (empty, or a transformer before failed)
		ReleaseBatch();

		_batch = batch;
		_batchTypes = Facepunch.Pool.Get<PooledHashSet<(string, string)>>();

		foreach (var file in batch)
		{
			if (!string.IsNullOrEmpty(file.Content))
			{
				CollectDeclaredTypes(CSharpSyntaxTree.ParseText(file.Content, options).GetCompilationUnitRoot().Members, null, _batchTypes);
			}
		}

		return _batchTypes;
	}

	private static void ReleaseBatch()
	{
		((IDisposable)_batchTypes)?.Dispose();
		_batchTypes = null;
		_batch = null;
	}

	private static void CollectDeclaredTypes(SyntaxList<MemberDeclarationSyntax> members, string ns, HashSet<(string, string)> output)
	{
		foreach (var member in members)
		{
			switch (member)
			{
				case BaseNamespaceDeclarationSyntax nested:
					CollectDeclaredTypes(nested.Members, Rewriter.FullNamespaceName(nested), output);
					break;

				case BaseTypeDeclarationSyntax type when ns != null:
					output.Add((ns, type.Identifier.ValueText));
					break;

				case DelegateDeclarationSyntax @delegate when ns != null:
					output.Add((ns, @delegate.Identifier.ValueText));
					break;
			}
		}
	}

	/// <summary>Aliases of Oxide namespaces ('using Cui = Oxide.Game.Rust.Cui;'), alias -> namespace. Left null when there are none.</summary>
	private static void CollectNamespaceAliases(SyntaxList<UsingDirectiveSyntax> usings, SyntaxList<MemberDeclarationSyntax> members, ref Dictionary<string, string> output)
	{
		foreach (var directive in usings)
		{
			if (directive.Alias == null || directive.Name == null || !StartsWithOxide(directive.Name))
			{
				continue;
			}

			var target = Rewriter.Normalize(directive.Name);

			if (KnownNamespaces.Contains(target))
			{
				output ??= Facepunch.Pool.Get<Dictionary<string, string>>();
				output[directive.Alias.Name.Identifier.ValueText] = target;
			}
		}

		foreach (var member in members)
		{
			if (member is BaseNamespaceDeclarationSyntax nested)
			{
				CollectNamespaceAliases(nested.Usings, nested.Members, ref output);
			}
		}
	}

	private static (string, string) SplitTypeName(string fullName)
	{
		var index = fullName.LastIndexOf('.');
		return index < 0 ? (string.Empty, fullName) : (fullName.Substring(0, index), fullName.Substring(index + 1));
	}

	/// <summary>The leftmost identifier of a dotted name, without allocating.</summary>
	private static SimpleNameSyntax Head(SyntaxNode node)
	{
		while (true)
		{
			switch (node)
			{
				case QualifiedNameSyntax qualified:
					node = qualified.Left;
					continue;

				case MemberAccessExpressionSyntax access:
					node = access.Expression;
					continue;

				case AliasQualifiedNameSyntax aliased:
					return aliased.Alias.Identifier.ValueText == "global" ? aliased.Name : aliased.Alias;

				case SimpleNameSyntax name:
					return name;

				default:
					return null;
			}
		}
	}

	private static bool StartsWithOxide(SyntaxNode node) => Head(node)?.Identifier.ValueText == "Oxide";

	private sealed class Rewriter(Dictionary<string, string> namespaceAliases, HashSet<(string, string)> declaredTypes) : CSharpSyntaxRewriter
	{
		/// <summary>The namespace being visited, which relative names are looked up from.</summary>
		private string _namespace;

		/// <summary>Whether <see cref="_namespace"/> is an Oxide namespace, the only ones relative names resolve from.</summary>
		private bool _inOxideNamespace;

		public override SyntaxNode VisitQualifiedName(QualifiedNameSyntax node)
		{
			var mapped = CanMap(node) ? Resolve(Normalize(node)) : null;
			return mapped != null ? SyntaxFactory.ParseName(GlobalPrefix + mapped).WithTriviaFrom(node) : base.VisitQualifiedName(node);
		}

		public override SyntaxNode VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
		{
			var mapped = CanMap(node) ? Resolve(Normalize(node)) : null;
			return mapped != null ? SyntaxFactory.ParseName(GlobalPrefix + mapped).WithTriviaFrom(node) : base.VisitAliasQualifiedName(node);
		}

		public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
		{
			// Only pure dotted chains (Oxide.Game.Rust.Cui.CuiHelper) can be type references
			if (CanMap(node) && IsDottedChain(node))
			{
				var mapped = Resolve(Normalize(node));
				if (mapped != null)
				{
					return SyntaxFactory.ParseExpression(GlobalPrefix + mapped).WithTriviaFrom(node);
				}
			}

			return base.VisitMemberAccessExpression(node);
		}

		public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			node = (FieldDeclarationSyntax)base.VisitFieldDeclaration(node);

			// Native plugins aren't RustPlugins anymore (RustPlugin derives from CarbonPlugin), so a reference typed
			// RustPlugin couldn't bind to them like it used to. Plugin is the common base of both.
			if (IsRustPlugin(node.Declaration.Type) && HasPluginReference(node.AttributeLists))
			{
				node = node.WithDeclaration(node.Declaration.WithType(SyntaxFactory.ParseTypeName(GlobalPrefix + "Carbon.Plugins.Plugin").WithTriviaFrom(node.Declaration.Type)));
			}

			return node;
		}

		private static bool IsRustPlugin(TypeSyntax type)
		{
			return type switch
			{
				IdentifierNameSyntax name => name.Identifier.ValueText == "RustPlugin",
				QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText == "RustPlugin" && Normalize(qualified) == "Oxide.Plugins.RustPlugin",
				_ => false
			};
		}

		private static bool HasPluginReference(SyntaxList<AttributeListSyntax> lists)
		{
			foreach (var list in lists)
			{
				foreach (var attribute in list.Attributes)
				{
					var name = attribute.Name switch
					{
						QualifiedNameSyntax qualified => qualified.Right,
						AliasQualifiedNameSyntax aliased => aliased.Name,
						SimpleNameSyntax simple => simple,
						_ => null
					};

					if (name?.Identifier.ValueText is "PluginReference" or "PluginReferenceAttribute")
					{
						return true;
					}
				}
			}

			return false;
		}

		public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
		{
			node = (CompilationUnitSyntax)base.VisitCompilationUnit(node);
			return node.WithUsings(ProcessUsings(node.Usings, scope: null));
		}

		public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			var name = FullNamespaceName(node);
			var previous = _namespace;
			EnterNamespace(name);
			node = (NamespaceDeclarationSyntax)base.VisitNamespaceDeclaration(node);
			EnterNamespace(previous);
			return node.WithUsings(ProcessUsings(node.Usings, name));
		}

		public override SyntaxNode VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
		{
			var name = FullNamespaceName(node);
			var previous = _namespace;
			EnterNamespace(name);
			node = (FileScopedNamespaceDeclarationSyntax)base.VisitFileScopedNamespaceDeclaration(node);
			EnterNamespace(previous);
			return node.WithUsings(ProcessUsings(node.Usings, name));
		}

		private void EnterNamespace(string name)
		{
			_namespace = name;
			_inOxideNamespace = name != null && IsOxideNamespace(name);
		}

		/// <summary>Cheap check ahead of <see cref="Resolve"/>: only names starting at Oxide, an Oxide namespace alias or a relative Oxide segment can map.</summary>
		private bool CanMap(SyntaxNode node)
		{
			var head = Head(node)?.Identifier.ValueText;

			return head != null && (head == "Oxide" || (namespaceAliases?.ContainsKey(head) ?? false) || (_inOxideNamespace && RelativeHeads.Contains(head)));
		}

		/// <summary>
		/// Maps a name the way C# binds it: fully qualified, through an alias of an Oxide namespace,
		/// or relative to the enclosing Oxide namespaces ('Core.Libraries.Permission' inside 'Oxide.Plugins').
		/// </summary>
		private string Resolve(string name)
		{
			var mapped = OxideTypeMap.Map(name);

			if (mapped != null)
			{
				return mapped;
			}

			var aliasSplit = name.IndexOf("::", StringComparison.Ordinal);
			var dot = name.IndexOf('.');
			var head = aliasSplit >= 0 ? name.Substring(0, aliasSplit) : dot >= 0 ? name.Substring(0, dot) : null;

			if (head == null)
			{
				return null;
			}

			if (namespaceAliases != null && namespaceAliases.TryGetValue(head, out var target))
			{
				return OxideTypeMap.Map($"{target}.{name.Substring(head.Length + (aliasSplit >= 0 ? 2 : 1))}");
			}

			if (aliasSplit >= 0 || !_inOxideNamespace)
			{
				return null;
			}

			for (var scope = _namespace; !string.IsNullOrEmpty(scope) && IsOxideNamespace(scope); scope = ParentNamespace(scope))
			{
				if (KnownNamespaces.Contains($"{scope}.{head}"))
				{
					return OxideTypeMap.Map($"{scope}.{name}");
				}
			}

			return null;
		}

		/// <summary>
		/// Swaps Oxide namespace imports for aliases of the native types that lived there.
		/// Code inside an Oxide namespace implicitly sees that namespace's types, so those get aliases too.
		/// Returns <paramref name="usings"/> itself when there's nothing to change.
		/// </summary>
		private SyntaxList<UsingDirectiveSyntax> ProcessUsings(SyntaxList<UsingDirectiveSyntax> usings, string scope)
		{
			var aliases = Facepunch.Pool.Get<Dictionary<string, string>>();
			using var result = Facepunch.Pool.Get<PooledList<UsingDirectiveSyntax>>();
			var changed = false;

			try
			{
				foreach (var directive in usings)
				{
					if (directive.Alias != null && directive.Name != null && IsRemovedNamespace(directive.Name))
					{
						// Aliases of removed namespaces only reach renamed types, which got rewritten above
						changed = true;
						continue;
					}

					if (directive.Alias != null || directive.StaticKeyword != default || directive.Name == null)
					{
						result.Add(directive);
						continue;
					}

					if (StartsWithOxide(directive.Name))
					{
						AddAliases(Normalize(directive.Name), aliases, scope);

						if (IsRemovedNamespace(directive.Name))
						{
							changed = true;
							continue;
						}
					}

					result.Add(directive);
				}

				for (var ns = scope; !string.IsNullOrEmpty(ns); ns = ParentNamespace(ns))
				{
					AddAliases(ns, aliases, scope);
				}

				foreach (var alias in aliases)
				{
					if (HasAlias(usings, alias.Key))
					{
						continue;
					}

					changed = true;
					result.Add(SyntaxFactory.UsingDirective(
							SyntaxFactory.NameEquals(alias.Key),
							SyntaxFactory.ParseName(GlobalPrefix + alias.Value))
						.NormalizeWhitespace()
						.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed));
				}

				return changed ? SyntaxFactory.List(result) : usings;
			}
			finally
			{
				Facepunch.Pool.FreeUnmanaged(ref aliases);
			}
		}

		private static bool HasAlias(SyntaxList<UsingDirectiveSyntax> usings, string alias)
		{
			foreach (var directive in usings)
			{
				if (directive.Alias != null && directive.Alias.Name.Identifier.ValueText == alias)
				{
					return true;
				}
			}

			return false;
		}

		private void AddAliases(string ns, Dictionary<string, string> aliases, string scope)
		{
			if (!OxideTypeMap.ByNamespace.TryGetValue(ns, out var types))
			{
				return;
			}

			foreach (var type in types)
			{
				// A same-named type in the namespace holding the alias conflicts with it (CS0576), whichever file
				// of the plugin declares it. A native type already living there takes precedence anyway.
				if (scope != null && (declaredTypes.Contains((scope, type.Key)) || NativeTypes.Contains((scope, type.Key))))
				{
					continue;
				}

				aliases[type.Key] = type.Value;
			}
		}

		private static bool IsRemovedNamespace(NameSyntax name) => StartsWithOxide(name) && IsRemovedNamespace(Normalize(name));

		private static bool IsRemovedNamespace(string ns) => IsOxideNamespace(ns) && KnownNamespaces.Contains(ns) && !LiveNamespaces.Contains(ns);

		private static string ParentNamespace(string ns) => ns.IndexOf('.') >= 0 ? ns.Substring(0, ns.LastIndexOf('.')) : null;

		internal static string FullNamespaceName(BaseNamespaceDeclarationSyntax node)
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

		internal static string Normalize(SyntaxNode node)
		{
			// ToString leaves out the node's own leading and trailing trivia
			var text = node.ToString();

			for (var i = 0; i < text.Length; i++)
			{
				if (char.IsWhiteSpace(text[i]))
				{
					text = new string(text.Where(c => !char.IsWhiteSpace(c)).ToArray());
					break;
				}
			}

			return text.StartsWith(GlobalPrefix, StringComparison.Ordinal) ? text.Substring(GlobalPrefix.Length) : text;
		}
	}
}
