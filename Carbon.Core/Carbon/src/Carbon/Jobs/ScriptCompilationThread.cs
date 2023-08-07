using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Carbon.Base;
using Carbon.Core;
using Carbon.Extensions;
using Carbon.Pooling;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
	public Dictionary<Type, List<uint>> Hooks = new();
	public Dictionary<Type, List<HookMethodAttribute>> HookMethods = new();
	public Dictionary<Type, List<PluginReferenceAttribute>> PluginReferences = new();
	public float CompileTime;
	public Assembly Assembly;
	public List<CompilerException> Exceptions = new();
	public List<CompilerException> Warnings = new();

	#region Internals

	internal const string _internalCallHookPattern = @"override object InternalCallHook";
	internal DateTime _timeSinceCompile;
	internal static Dictionary<string, byte[]> _compilationCache = new();
	internal static Dictionary<string, byte[]> _extensionCompilationCache = new();
	internal static Dictionary<string, PortableExecutableReference> _referenceCache = new();
	internal static Dictionary<string, PortableExecutableReference> _extensionReferenceCache = new();
	internal static readonly string[] _libraryDirectories = new[]
	{
		Defines.GetLibFolder(),
		Defines.GetManagedFolder(),
		Defines.GetRustManagedFolder(),
		Defines.GetManagedModulesFolder(),
		Defines.GetExtensionsFolder()
	};

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
	internal void _injectReference(string id, string name, List<MetadataReference> references, string[] directories, bool direct = false)
	{
		if (_referenceCache.TryGetValue(name, out var reference))
		{
			Logger.Debug(id, $"Added common references from cache '{name}'", 4);
			references.Add(reference);
		}
		else
		{
			var raw = (byte[])null;

			if (direct)
			{
				var found = false;
				foreach (var directory in directories)
				{
					foreach (var file in OsEx.Folder.GetFilesWithExtension(directory, "dll"))
					{
						if (file.Contains(name))
						{
							raw = OsEx.File.ReadBytes(file);
							found = true;
							break;
						}
					}

					if (found) break;
				}
			}
			else
			{
				raw = Community.Runtime.AssemblyEx.Read(name, directories);
			}

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

		_injectReference(id, "0Harmony", references, _libraryDirectories);
		_injectReference(id, "System.Memory", references, _libraryDirectories, direct: true);

		foreach (var item in Community.Runtime.AssemblyEx.RefWhitelist)
		{
			try
			{
				_injectReference(id, item, references, _libraryDirectories);
			}
			catch (System.Exception ex)
			{
				Logger.Debug(id, $"Error loading common reference '{item}': {ex}", 4);
			}
		}

		foreach (var item in Community.Runtime.AssemblyEx.Modules.Loaded)
		{
			try
			{
				_injectReference(id, item, references, _libraryDirectories);
			}
			catch (System.Exception ex)
			{
				Logger.Debug(id, $"Error loading common reference '{item}': {ex}", 4);
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

				var libFile = Path.Combine(Defines.GetLibFolder(), $"{reference}.dll");
				if (OsEx.File.Exists(libFile))
				{
					_injectReference(reference, libFile, references, _libraryDirectories);
					continue;
				}

				var managedFile = Path.Combine(Defines.GetRustManagedFolder(), $"{reference}.dll");
				if (OsEx.File.Exists(managedFile))
				{
					_injectReference(reference, managedFile, references, _libraryDirectories );
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
			_timeSinceCompile = DateTime.Now;
			FileName = Path.GetFileNameWithoutExtension(FilePath);

			var trees = new List<SyntaxTree>();
			var conditionals = new List<string>();

			conditionals.AddRange(Community.Runtime.Config.ConditionalCompilationSymbols);

#if WIN
			conditionals.Add("WIN");
#else
			conditionals.Add("UNIX");
#endif

#if MINIMAL
			conditionals.Add("MINIMAL");
#endif

			var parseOptions = new CSharpParseOptions(LanguageVersion.Latest)
				.WithPreprocessorSymbols(conditionals);
			var tree = CSharpSyntaxTree.ParseText(
				Source, options: parseOptions);

			var root = tree.GetCompilationUnitRoot();

			if (!Source.Contains(_internalCallHookPattern))
			{
				HookCaller.GenerateInternalCallHook(root, out root, out _, publicize: false);

				Source = root.ToFullString();
				trees.Add(CSharpSyntaxTree.ParseText(Source, options: parseOptions));
			}
			else
			{
				trees.Add(tree);
			}

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

			conditionals.Clear();
			conditionals = null;
			references.Clear();
			references = null;
			trees.Clear();
			trees = null;

			if (Assembly == null) return;

			CompileTime = (float)(DateTime.Now - _timeSinceCompile).Milliseconds;

			foreach (var type in Assembly.GetTypes())
			{
				var hooks = new List<uint>();
				var hookMethods = new List<HookMethodAttribute>();
				var pluginReferences = new List<PluginReferenceAttribute>();
				Hooks.Add(type, hooks);
				HookMethods.Add(type, hookMethods);
				PluginReferences.Add(type, pluginReferences);

				foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
				{
					var hash = HookStringPool.GetOrAdd(method.Name);

					if (Community.Runtime.HookManager.IsHookLoaded(method.Name))
					{
						if (!hooks.Contains(hash)) hooks.Add(hash);
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
		HookMethods.Clear();
		PluginReferences.Clear();

		Hooks = null;
		HookMethods = null;
		PluginReferences = null;
		Exceptions = null;
		Warnings = null;
	}
}
