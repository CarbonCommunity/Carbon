using System.Collections.Generic;
using System.Text;
using Carbon.CompilerPolyfills;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Carbon.Jobs;

internal static class ScriptCompilerPolyfills
{
	public static void InjectMissingPolyfills(List<SyntaxTree> trees, IReadOnlyCollection<MetadataReference> references, CSharpParseOptions parseOptions)
	{
		var probeCompilation = CSharpCompilation.Create("__CarbonPolyfillProbe", references: references);
		var source = CompilerPolyfillCatalog.BuildSource(metadataName => IsPublic(probeCompilation.GetTypeByMetadataName(metadataName)), false);

		if (source.Length == 0)
		{
			return;
		}

		trees.Insert(0, CSharpSyntaxTree.ParseText(source, parseOptions, "__Carbon.CompilerPolyfills.g.cs", Encoding.UTF8));
	}

	private static bool IsPublic(INamedTypeSymbol symbol)
	{
		return symbol?.DeclaredAccessibility == Accessibility.Public;
	}
}
