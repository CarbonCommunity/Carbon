using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using API.Assembly;
using Carbon.Base;
using Carbon.Contracts;
using Carbon.Core;
using Carbon.Extensions;
using Carbon.Pooling;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community
 * All rights reserved.
 *
 */

namespace Carbon.Jobs;

public class ScriptCompilationThread : BaseThreadedJob
{
	public ISource InitialSource => Sources != null && Sources.Count > 0 ? Sources[0] : null;
	public List<ISource> Sources;
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
	internal const string _partialPattern = @" partial ";
	internal DateTime _timeSinceCompile;
	internal List<ClassDeclarationSyntax> ClassList = new();
	internal static EmitOptions _emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.Embedded);
	internal static ConcurrentDictionary<string, byte[]> _compilationCache = new();
	internal static ConcurrentDictionary<string, byte[]> _extensionCompilationCache = new();
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
		name = name.Replace(" ", string.Empty);

		foreach (var plugin in _compilationCache)
		{
			if (plugin.Key == name)
			{
				return plugin.Value;
			}
		}

		return null;
	}
	internal static byte[] _getExtensionPlugin(string name)
	{
		foreach (var extension in _extensionCompilationCache)
		{
			if (extension.Key == name)
			{
				return extension.Value;
			}
		}

		return null;
	}
	internal static void _overridePlugin(string name, byte[] pluginAssembly)
	{
		name = name.Replace(" ", "");

		if (pluginAssembly == null) return;

		var plugin = _getPlugin(name);
		if (plugin == null)
		{
			try { _compilationCache.AddOrUpdate(name, pluginAssembly, (a, v) => pluginAssembly); } catch { }
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
			try { _extensionCompilationCache.AddOrUpdate(name, pluginAssembly, (a, v) => pluginAssembly); } catch { }
			return;
		}

		Array.Clear(plugin, 0, plugin.Length);
		try { _extensionCompilationCache[name] = pluginAssembly; } catch { }
	}
	internal static void _clearExtensionPlugin(string name)
	{
		if (_extensionCompilationCache.ContainsKey(name)) _extensionCompilationCache.TryRemove(name, out _);
		if (_extensionReferenceCache.ContainsKey(name)) _extensionReferenceCache.Remove(name);
	}
	internal void _injectReference(string id, string name, List<MetadataReference> references, string[] directories, bool direct = false, bool allowCache = true)
	{
		if (allowCache && _referenceCache.TryGetValue(name, out var reference))
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
			else _referenceCache[name] = processedReference;
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
			var raw = Community.Runtime.AssemblyEx.Read(name, _libraryDirectories);
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
		var id = Path.GetFileNameWithoutExtension(InitialSource.FilePath);

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
				var name = Path.GetFileName(item.Value.Key);
				using var mem = new MemoryStream(item.Value.Value);
				var processedReference = MetadataReference.CreateFromStream(mem);

				references.Add(processedReference);
				if (!_referenceCache.ContainsKey(name)) _referenceCache.Add(name, processedReference);
				else _referenceCache[name] = processedReference;
			}
			catch (System.Exception ex)
			{
				Logger.Debug(id, $"Error loading module reference '{item}': {ex}", 4);
			}
		}

		foreach (var item in Community.Runtime.AssemblyEx.Extensions.Loaded)
		{
			try
			{
				_injectExtensionReference(id, Path.GetFileName(item.Value.Key), references);
			}
			catch (System.Exception ex)
			{
				Logger.Debug(id, $"Error loading extension reference '{item}': {ex}", 4);
			}
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
		if (Sources.TrueForAll(x => string.IsNullOrEmpty(x.Content)))
		{
			return;
		}

		try
		{
			Exceptions.Clear();
			Warnings.Clear();
			_timeSinceCompile = DateTime.Now;

			var trees = new List<SyntaxTree>();
			var conditionals = new List<string>();

			try
			{
				conditionals.AddRange(Community.Runtime.Config.ConditionalCompilationSymbols);
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed referencing conditional compilation symbols", ex);
			}
#if WIN
			conditionals.Add("WIN");
#else
			conditionals.Add("UNIX");
#endif

#if MINIMAL
			conditionals.Add("MINIMAL");
#endif

#if STAGING
			conditionals.Add("STAGING");
#elif AUX01
			conditionals.Add("AUX01");
#elif AUX02
			conditionals.Add("AUX02");
#endif

			string pdb_filename =
			#if DEBUG
				Debugger.IsAttached ? (string.IsNullOrEmpty(Community.Runtime.Config.ScriptDebuggingOrigin) ? InitialSource.ContextFilePath : Path.Combine(Community.Runtime.Config.ScriptDebuggingOrigin, InitialSource.ContextFileName)) : InitialSource.ContextFileName;
			#else
				InitialSource.ContextFileName;
			#endif

			var parseOptions = new CSharpParseOptions(LanguageVersion.Latest)
				.WithPreprocessorSymbols(conditionals);

			var containsInternalCallHookOverride = Sources.Any(x => !string.IsNullOrEmpty(x.Content) && x.Content.Contains(_internalCallHookPattern));

			foreach (var source in Sources)
			{
				var tree = CSharpSyntaxTree.ParseText(
					source.Content, options: parseOptions, source.FilePath, Encoding.UTF8);

				var root = tree.GetCompilationUnitRoot();

				if (HookCaller.FindPluginInfo(root, out var @namespace, out var namespaceIndex, out var classIndex, ClassList))
				{
					var @class = ClassList[0];

					if (!@class.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword)))
					{
						@class = @class.WithModifiers(@class.Modifiers.Add(SyntaxFactory.ParseToken(_partialPattern)));
					}

					root = root.WithMembers(root.Members.RemoveAt(namespaceIndex).Insert(namespaceIndex, @namespace.WithMembers(@namespace.Members.RemoveAt(classIndex).Insert(classIndex, @class))));
					trees.Insert(0, CSharpSyntaxTree.ParseText(
						root.ToFullString(), options: parseOptions, source.FilePath, Encoding.UTF8));
				}
				else
				{
					trees.Add(tree);
				}

				foreach (var name in root.Usings.Select(element => element.Name.ToString()).Where(name => !Usings.Contains(name)))
				{
					Usings.Add(name);
				}
			}

			if (!containsInternalCallHookOverride)
			{
				var completeBody = CSharpSyntaxTree.ParseText(
					Sources.Select(x => x.Content).ToString("\n"), options: parseOptions, pdb_filename, Encoding.UTF8);

				HookCaller.GeneratePartial(completeBody.GetCompilationUnitRoot(), out var partialTree, parseOptions, pdb_filename, ClassList);

				trees.Add(partialTree.SyntaxTree);
			}

			var options = new CSharpCompilationOptions(
				OutputKind.DynamicallyLinkedLibrary,
				optimizationLevel:
				#if DEBUG
					Debugger.IsAttached ? OptimizationLevel.Debug : OptimizationLevel.Release,
				#else
					OptimizationLevel.Release,
				#endif
				deterministic: true, warningLevel: 4
			);

			var compilation = CSharpCompilation.Create(
				$"Script.{InitialSource.FileName}.{Guid.NewGuid():N}", trees, references, options);

			using (var dllStream = new MemoryStream())
			{
				var emit = compilation.Emit(dllStream, options: _emitOptions);
				var errors = new List<string>();
				var warnings = new List<string>();

				foreach (var error in emit.Diagnostics)
				{
					if (errors.Contains(error.Id) || warnings.Contains(error.Id)) continue;

					var span = error.Location.GetMappedLineSpan().Span;

					var filePath = error?.Location?.SourceTree?.FilePath;
					var fileName = Path.GetFileNameWithoutExtension(filePath);

					switch (error.Severity)
					{
						case DiagnosticSeverity.Error:
							errors.Add(error.Id);
							Exceptions.Add(new CompilerException(filePath,
								new CompilerError(fileName, span.Start.Line + 1, span.Start.Character + 1, error.Id, error.GetMessage(CultureInfo.InvariantCulture))));

							break;

						case DiagnosticSeverity.Warning:
							if (error.GetMessage(CultureInfo.InvariantCulture).Contains("Assuming assembly reference")) continue;

							errors.Add(error.Id);
							Warnings.Add(new CompilerException(filePath,
								new CompilerError(fileName, span.Start.Line + 1, span.Start.Character + 1, error.Id, error.GetMessage(CultureInfo.InvariantCulture))));
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
						if (IsExtension) _overrideExtensionPlugin(InitialSource.ContextFilePath, assembly);
						_overridePlugin(Path.GetFileNameWithoutExtension(InitialSource.ContextFilePath), assembly);
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
		catch (Exception ex) { System.Console.WriteLine($"Threading compilation failed for '{InitialSource.ContextFilePath}': {ex}"); }
	}

	public override void Dispose()
	{
		ClassList?.Clear();

		Exceptions?.Clear();
		Warnings?.Clear();

		Hooks?.Clear();
		HookMethods?.Clear();
		PluginReferences?.Clear();

		ClassList = null;
		Hooks = null;
		HookMethods = null;
		PluginReferences = null;
		Exceptions = null;
		Warnings = null;
	}
}
