using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Carbon.CompilerPolyfills;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Carbon.Jobs;

internal static class ScriptCompilerPolyfills
{
	private const int MaxCachedProbes = 8;

	private static readonly List<(HashSet<MetadataReference> references, string source)> _probeCache = new();

	public static void InjectMissingPolyfills(List<SyntaxTree> trees, IReadOnlyCollection<MetadataReference> references, CSharpParseOptions parseOptions)
	{
		var source = GetPolyfillSource(references);

		if (source.Length == 0)
		{
			return;
		}

		trees.Insert(0, CSharpSyntaxTree.ParseText(source, parseOptions, "__Carbon.CompilerPolyfills.g.cs", Encoding.UTF8));
	}

	private static string GetPolyfillSource(IReadOnlyCollection<MetadataReference> references)
	{
		var key = new HashSet<MetadataReference>(references, ReferenceIdentityComparer.Instance);

		lock (_probeCache)
		{
			foreach (var entry in _probeCache)
			{
				if (entry.references.SetEquals(key))
				{
					return entry.source;
				}
			}
		}

		var probeCompilation = CSharpCompilation.Create("__CarbonPolyfillProbe", references: references);
		var source = CompilerPolyfillCatalog.BuildSource(metadataName => IsPublic(probeCompilation.GetTypeByMetadataName(metadataName)), false);

		lock (_probeCache)
		{
			if (_probeCache.Count >= MaxCachedProbes)
			{
				_probeCache.RemoveAt(0);
			}

			_probeCache.Add((key, source));
		}

		return source;
	}

	private static bool IsPublic(INamedTypeSymbol symbol)
	{
		return symbol?.DeclaredAccessibility == Accessibility.Public;
	}

	private sealed class ReferenceIdentityComparer : IEqualityComparer<MetadataReference>
	{
		public static readonly ReferenceIdentityComparer Instance = new();

		public bool Equals(MetadataReference x, MetadataReference y)
		{
			return ReferenceEquals(x, y);
		}

		public int GetHashCode(MetadataReference obj)
		{
			return RuntimeHelpers.GetHashCode(obj);
		}
	}
}
