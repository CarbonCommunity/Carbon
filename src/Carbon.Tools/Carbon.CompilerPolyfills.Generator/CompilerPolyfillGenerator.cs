using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Carbon.CompilerPolyfills;

[Generator]
public sealed class CompilerPolyfillGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{
	}

	public void Execute(GeneratorExecutionContext context)
	{
		var designTime = context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.DesignTimeBuild", out var value) &&
			bool.TryParse(value, out var parsed) && parsed;
		var accessibility = context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.CarbonCompilerPolyfillsAccessibility", out var accessibilityValue) &&
			string.Equals(accessibilityValue, "internal", StringComparison.OrdinalIgnoreCase)
				? "internal"
				: "public";

		var source = CompilerPolyfillCatalog.BuildSource(
			metadataName => IsPublic(context.Compilation.GetTypeByMetadataName(metadataName)),
			designTime,
			accessibility);

		if (source.Length == 0)
		{
			return;
		}

		context.AddSource("Carbon.CompilerPolyfills.g.cs", SourceText.From(source, Encoding.UTF8));
	}

	private static bool IsPublic(INamedTypeSymbol? symbol)
	{
		return symbol?.DeclaredAccessibility == Accessibility.Public;
	}
}
