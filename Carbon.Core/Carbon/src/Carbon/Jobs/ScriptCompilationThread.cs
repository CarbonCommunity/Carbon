using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Carbon.Base;
using Carbon.Core;
using Carbon.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProtoBuf;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Jobs;

public class ScriptCompilationThread : BaseThreadedJob
{
	public string FilePath;
	public string FileName;
	public string Source;
	public string[] References;
	public string[] Requires;
	public bool IsExtension;
	public List<string> Usings = new();
	public Dictionary<Type, Dictionary<uint, Priorities>> Hooks = new();
	public Dictionary<Type, List<uint>> UnsupportedHooks = new();
	public Dictionary<Type, List<HookMethodAttribute>> HookMethods = new();
	public Dictionary<Type, List<PluginReferenceAttribute>> PluginReferences = new();
	public float CompileTime;
	public Assembly Assembly;
	public List<CompilerException> Exceptions = new();
	public List<CompilerException> Warnings = new();

	#region Internals

	internal DateTime TimeSinceCompile;
	internal static Dictionary<string, byte[]> _compilationCache = new();
	internal static Dictionary<string, byte[]> _extensionCompilationCache = new();
	internal static Dictionary<string, PortableExecutableReference> _referenceCache = new();
	internal static Dictionary<string, PortableExecutableReference> _extensionReferenceCache = new();

	internal static byte[] _getPlugin(string name)
	{
		name = name.Replace(" ", "");

		if (!_compilationCache.TryGetValue(name, out var result)) return null;

		return result;
	}
	internal static byte[] _getExtensionPlugin(string name)
	{
		if (!_extensionCompilationCache.TryGetValue(name, out var result)) return null;

		return result;
	}
	internal static void _overridePlugin(string name, byte[] pluginAssembly)
	{
		name = name.Replace(" ", "");

		if (pluginAssembly == null) return;

		var plugin = _getPlugin(name);
		if (plugin == null)
		{
			try { _compilationCache.Add(name, pluginAssembly); } catch { }
			return;
		}

		Array.Clear(plugin, 0, plugin.Length);
		try { _compilationCache[name] = pluginAssembly; } catch { }
	}
	internal static void _overrideExtensionPlugin(string name, byte[] pluginAssembly)
	{
		if (pluginAssembly == null) return;

		var plugin = _getExtensionPlugin(name);
		if (plugin == null)
		{
			try { _extensionCompilationCache.Add(name, pluginAssembly); } catch { }
			return;
		}

		Array.Clear(plugin, 0, plugin.Length);
		try { _extensionCompilationCache[name] = pluginAssembly; } catch { }
	}
	internal static void _clearExtensionPlugin(string name)
	{
		if (_extensionCompilationCache.ContainsKey(name)) _extensionCompilationCache.Remove(name);
		if (_extensionReferenceCache.ContainsKey(name)) _extensionReferenceCache.Remove(name);
	}
	internal void _injectReference(string id, string name, List<MetadataReference> references)
	{
		if (_referenceCache.TryGetValue(name, out var reference))
		{
			Logger.Debug(id, $"Added common references from cache '{name}'", 4);
			references.Add(reference);
		}
		else
		{
			var raw = Community.Runtime.AssemblyEx.Read(name);
			if (raw == null) return;

			using var mem = new MemoryStream(raw);
			var processedReference = MetadataReference.CreateFromStream(mem);

			references.Add(processedReference);
			if (!_referenceCache.ContainsKey(name)) _referenceCache.Add(name, processedReference);
			Logger.Debug(id, $"Added common reference '{name}'", 4);
		}
	}
	internal void _injectExtensionReference(string id, string name, List<MetadataReference> references)
	{
		if (_extensionReferenceCache.TryGetValue(name, out var reference))
		{
			references.Add(reference);
		}
		else
		{
			var raw = Community.Runtime.AssemblyEx.Read(name);
			if (raw == null) return;

			using var mem = new MemoryStream(raw);
			var processedReference = MetadataReference.CreateFromStream(mem);

			references.Add(processedReference);
			_extensionReferenceCache.Add(name, processedReference);
		}
	}
	internal List<MetadataReference> _addReferences()
	{
		var references = new List<MetadataReference>();
		var id = Path.GetFileNameWithoutExtension(FilePath);

		if (Community.Runtime.Config.HarmonyReference)
		{
			_injectReference(id, "0Harmony", references);
		}

		foreach (var item in Community.Runtime.AssemblyEx.RefWhitelist)
		{
			try
			{
				_injectReference(id, item, references);
			}
			catch (System.Exception)
			{
				Logger.Debug(id, $"Error loading common reference '{item}'", 4);
			}
		}

		foreach (var item in Community.Runtime.AssemblyEx.Extensions.Loaded)
		{
			try { _injectExtensionReference(id, item, references); }
			catch { }
		}

		return references;
	}

	#endregion

	public class CompilerException : Exception
	{
		public string FilePath;
		public CompilerError Error;
		public CompilerException(string filePath, CompilerError error) { FilePath = filePath; Error = error; }

		public override string ToString()
		{
			return $"{Error.ErrorText}\n ({FilePath} {Error.Column} line {Error.Line})";
		}
	}

	private List<MetadataReference> references;

	public override void Start()
	{
		references = _addReferences();

		foreach (var require in Requires)
		{
			try
			{
				var requiredPlugin = _getPlugin(require);

				using var dllStream = new MemoryStream(requiredPlugin);
				references.Add(MetadataReference.CreateFromStream(dllStream));
			}
			catch { /* do nothing */ }
		}

		foreach (var reference in References)
		{
			try
			{
				var extensionFile = Path.Combine(Defines.GetExtensionsFolder(), $"{reference}.dll");
				if (OsEx.File.Exists(extensionFile))
				{
					_injectExtensionReference(reference, extensionFile, references);
					continue;
				}

				var managedFile = Path.Combine(Defines.GetRustManagedFolder(), $"{reference}.dll");
				if (OsEx.File.Exists(managedFile))
				{
					_injectReference(reference, managedFile, references);
					continue;
				}
			}
			catch { /* do nothing */ }
		}

		base.Start();
	}

	public override void ThreadFunction()
	{
		try
		{
			Exceptions.Clear();
			Warnings.Clear();
			TimeSinceCompile = DateTime.Now;
			FileName = Path.GetFileNameWithoutExtension(FilePath);

			var trees = new List<SyntaxTree>();

			var parseOptions = new CSharpParseOptions(LanguageVersion.Latest)
				.WithPreprocessorSymbols(Community.Runtime.Config.ConditionalCompilationSymbols);
			var tree = CSharpSyntaxTree.ParseText(
				Source, options: parseOptions);

			GenerateInternalCallHook(tree.GetCompilationUnitRoot(), out var root);

			Source = root.ToFullString();

			trees.Add(CSharpSyntaxTree.ParseText(Source, options: parseOptions));

			foreach (var element in root.Usings)
				Usings.Add($"{element.Name}");

			var options = new CSharpCompilationOptions(
				OutputKind.DynamicallyLinkedLibrary,
				optimizationLevel: OptimizationLevel.Release,
				deterministic: true, warningLevel: 4
			);

			var compilation = CSharpCompilation.Create(
				$"Script.{FileName}.{Guid.NewGuid():N}", trees, references, options);

			using (var dllStream = new MemoryStream())
			{
				var emit = compilation.Emit(dllStream);
				var errors = new List<string>();
				var warnings = new List<string>();

				foreach (var error in emit.Diagnostics)
				{
					if (errors.Contains(error.Id) || warnings.Contains(error.Id)) continue;

					var span = error.Location.GetMappedLineSpan().Span;

					switch (error.Severity)
					{
						case DiagnosticSeverity.Error:
							errors.Add(error.Id);
							Exceptions.Add(new CompilerException(FilePath,
								new CompilerError(FileName, span.Start.Line + 1, span.Start.Character + 1, error.Id, error.GetMessage(CultureInfo.InvariantCulture))));

							break;

						case DiagnosticSeverity.Warning:
							if (error.GetMessage(CultureInfo.InvariantCulture).Contains("Assuming assembly reference")) continue;

							errors.Add(error.Id);
							Warnings.Add(new CompilerException(FilePath,
								new CompilerError(FileName, span.Start.Line + 1, span.Start.Character + 1, error.Id, error.GetMessage(CultureInfo.InvariantCulture))));
							break;
					}
				}

				errors.Clear();
				warnings.Clear();
				errors = warnings = null;

				if (emit.Success)
				{
					var assembly = dllStream.ToArray();
					if (assembly != null)
					{
						if (IsExtension) _overrideExtensionPlugin(FilePath, assembly);
						_overridePlugin(FileName, assembly);
						Assembly = Assembly.Load(assembly);
					}
				}
			}

			if (Assembly == null) return;

			CompileTime = (float)(DateTime.Now - TimeSinceCompile).Milliseconds;

			references.Clear();
			references = null;
			trees.Clear();
			trees = null;

			foreach (var type in Assembly.GetTypes())
			{
				var hooks = new Dictionary<uint, Priorities>();
				var unsupportedHooks = new List<uint>();
				var hookMethods = new List<HookMethodAttribute>();
				var pluginReferences = new List<PluginReferenceAttribute>();
				Hooks.Add(type, hooks);
				UnsupportedHooks.Add(type, unsupportedHooks);
				HookMethods.Add(type, hookMethods);
				PluginReferences.Add(type, pluginReferences);

				foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
				{
					var hash = HookCallerCommon.StringPool.GetOrAdd(method.Name);

					if (HookValidator.IsIncompatibleOxideHook(method.Name))
					{
						unsupportedHooks.Add(hash);
					}

					if (Community.Runtime.HookManager.IsHookLoaded(method.Name))
					{
						var priority = method.GetCustomAttribute<HookPriority>();
						if (!hooks.ContainsKey(hash)) hooks.Add(hash, priority == null ? Priorities.Normal : priority.Priority);
					}
					else
					{
						var attribute = method.GetCustomAttribute<HookMethodAttribute>();
						if (attribute == null) continue;

						attribute.Method = method;
						hookMethods.Add(attribute);
					}
				}

				foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
				{
					var attribute = field.GetCustomAttribute<PluginReferenceAttribute>();
					if (attribute == null) continue;

					attribute.Field = field;
					pluginReferences.Add(attribute);
				}
			}

			if (Exceptions.Count > 0) throw null;
		}
		catch (Exception ex) { System.Console.WriteLine($"Threading compilation failed for '{FileName}': {ex}"); }
	}

	public override void Dispose()
	{
		Exceptions.Clear();
		Warnings.Clear();

		Hooks.Clear();
		UnsupportedHooks.Clear();
		HookMethods.Clear();
		PluginReferences.Clear();

		Hooks = null;
		UnsupportedHooks = null;
		HookMethods = null;
		PluginReferences = null;
		Exceptions = null;
		Warnings = null;
	}

	#region Code Generator

	public static void GenerateInternalCallHook(CompilationUnitSyntax input, out CompilationUnitSyntax output)
	{
		var methodContents =
@"
	var result = (object)null;

	switch(hook)
	{
";
		var @namespace = input.Members[0] as BaseNamespaceDeclarationSyntax;
		var @class = @namespace.Members[0] as ClassDeclarationSyntax;
		var methodDeclarations = @class.ChildNodes().OfType<MethodDeclarationSyntax>();
		var hookableMethods = new Dictionary<uint, List<MethodDeclarationSyntax>>();
		var privateMethods = methodDeclarations.Where(md => md.Modifiers.Any(SyntaxKind.PrivateKeyword) && md.TypeParameterList == null).OrderBy(x => x.Identifier.ValueText);

		foreach (var method in privateMethods)
		{
			if (method.ParameterList.Parameters.Any(x => x.Modifiers.Any(y => y.IsKind(SyntaxKind.OutKeyword)))) continue;

			var id = HookCallerCommon.StringPool.GetOrAdd(method.Identifier.ValueText);

			if (!hookableMethods.TryGetValue(id, out var list))
			{
				hookableMethods[id] = list = new();
			}

			list.Add(method);
		}

		foreach (var group in hookableMethods)
		{
			if (group.Value.Count > 0)
			{
				for (int i = 0; i < group.Value.Count; i++)
				{
					var parameterIndex = -1;
					var method = group.Value[i];
					var parameters = method.ParameterList.Parameters.Select(x =>
					{
						parameterIndex++;
						return $"{(x.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)) ? "ref " : "")}arg{parameterIndex}";
					}).ToArray();

					parameterIndex = -1;
					methodContents += $"\t\tcase {group.Key} when {(method.ParameterList.Parameters.Count > 0 ? $"{method.ParameterList.Parameters.Select(x =>
					{
						parameterIndex++;
						return $"args[{parameterIndex}] is {x.Type} arg{parameterIndex}";
					}).ToArray().ToString(" && ")}" : "args == null || args.Length == 0")}:\n" +
						$"\t\t{{";

					methodContents += $"\t\t\t{(method.ReturnType.ToString() != "void" ? "result = " : string.Empty)}{method.Identifier.ValueText}({string.Join(", ", parameters)});\n";

					parameterIndex = 0;
					foreach (var @ref in method.ParameterList.Parameters)
					{
						if (@ref.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)))
						{
							methodContents += $"\t\targs[{parameterIndex}] = arg{parameterIndex};\n";
						}

						parameterIndex++;
					}

					methodContents +=
						"\tbreak;\n" +
						"\t}\n" +
						"\n";
				}
			}
			else
			{
				methodContents += $"\t\tcase \"{group.Key}\":\n" +
					$"\t\t{{";

				for (int i = 0; i < group.Value.Count; i++)
				{
					PopulateCase(group.Value[i], i);
				}

				methodContents +=
					"\tbreak;\n" +
					"\t}\n" +
					"\n";
			}

			void PopulateCase(MethodDeclarationSyntax method, int methodIndex)
			{
				var parameterIndex = -1;
				var parameters = method.ParameterList.Parameters.Select(x =>
				{
					parameterIndex++;

					if (x.Modifiers.Any(y => y.IsKind(SyntaxKind.RefKeyword)))
					{
						return $"ref ref{parameterIndex}_{methodIndex}";
					}

					return $"({x.Type})args[{parameterIndex}]";
				}).ToArray();

				var refId = 0;
				foreach (var @ref in method.ParameterList.Parameters)
				{
					if (@ref.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)))
					{
						methodContents += $"\t\tvar ref{refId}_{methodIndex} = ({@ref.Type})args[{refId}];\n";
					}

					refId++;
				}

				methodContents += $"\t\t\t{(method.ReturnType.ToString() != "void" ? "result = " : string.Empty)}{method.Identifier.ValueText}({string.Join(", ", parameters)});\n";

				refId = 0;
				foreach (var @ref in method.ParameterList.Parameters)
				{
					if (@ref.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)))
					{
						methodContents += $"\t\targs[{refId}] = ref{refId}_{methodIndex};\n";
					}

					refId++;
				}
			}
		}

		methodContents += "\t}\n\n\treturn result;\n";

		var generatedMethod = SyntaxFactory.MethodDeclaration(
			SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword).WithTrailingTrivia(SyntaxFactory.Space)),
			"InternalCallHook").AddParameterListParameters(
				SyntaxFactory.Parameter(SyntaxFactory.Identifier("hook")).WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntKeyword)).WithTrailingTrivia(SyntaxFactory.Space)),
				SyntaxFactory.Parameter(SyntaxFactory.Identifier("args")).WithType(SyntaxFactory.ArrayType(
				SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
				SyntaxFactory.SingletonList(
						SyntaxFactory.ArrayRankSpecifier(
							SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
								SyntaxFactory.OmittedArraySizeExpression()
							)
						)
					)
				).WithTrailingTrivia(SyntaxFactory.Space)))
				.WithTrailingTrivia(SyntaxFactory.LineFeed)
			.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(SyntaxFactory.Space), SyntaxFactory.Token(SyntaxKind.OverrideKeyword).WithTrailingTrivia(SyntaxFactory.Space))
			.AddBodyStatements(SyntaxFactory.ParseStatement(methodContents)).WithTrailingTrivia(SyntaxFactory.LineFeed);

		output = input.WithMembers(input.Members.RemoveAt(0).Insert(0, @namespace.WithMembers(@namespace.Members.RemoveAt(0).Insert(0, @class.WithMembers(@class.Members.Insert(0, generatedMethod))))));
	}

	#endregion
}
