///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Facepunch;
using Carbon.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Application = UnityEngine.Application;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Carbon.Core
{
	public class AsyncPluginLoader : ThreadedJob
	{
		public string FilePath;
		public string FileName;
		public string Source;
		public string[] References;
		public string[] Requires;
		public Dictionary<Type, List<string>> Hooks = new Dictionary<Type, List<string>>();
		public Dictionary<Type, List<string>> UnsupportedHooks = new Dictionary<Type, List<string>>();
		public Dictionary<Type, List<HookMethodAttribute>> HookMethods = new Dictionary<Type, List<HookMethodAttribute>>();
		public Dictionary<Type, List<PluginReferenceAttribute>> PluginReferences = new Dictionary<Type, List<PluginReferenceAttribute>>();
		public float CompileTime;
		public Assembly Assembly;
		public List<CompilerException> Exceptions = new List<CompilerException>();
		internal RealTimeSince TimeSinceCompile;

		#region Fody

		internal static Type _fodyAssemblyLoader { get; set; }
		internal static FieldInfo _fodyAssemblyNames { get; set; }
		internal static MethodInfo _fodyLoadStream { get; set; }

		#endregion

		internal static bool _hasInit { get; set; }
		internal static void _doInit()
		{
			if (_hasInit) return;
			_hasInit = true;

			_fodyAssemblyLoader = CarbonDefines.Carbon.GetType("Costura.AssemblyLoader");
			_fodyAssemblyNames = _fodyAssemblyLoader.GetField("assemblyNames", BindingFlags.NonPublic | BindingFlags.Static);
			_fodyLoadStream = _fodyAssemblyLoader.GetMethod("LoadStream", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(string) }, default);

			var fodyNames = _fodyAssemblyNames.GetValue(null) as Dictionary<string, string>;

			_metadataReferences.Add(MetadataReference.CreateFromStream(_fodyLoadStream.Invoke(null, new object[] { fodyNames["protobuf-net.core"] }) as Stream));
			_metadataReferences.Add(MetadataReference.CreateFromStream(_fodyLoadStream.Invoke(null, new object[] { fodyNames["protobuf-net"] }) as Stream));
			_metadataReferences.Add(MetadataReference.CreateFromStream(_fodyLoadStream.Invoke(null, new object[] { fodyNames["1harmony"] }) as Stream));
			_metadataReferences.Add(MetadataReference.CreateFromStream(new MemoryStream(OsEx.File.ReadBytes(CarbonDefines.DllPath))));

			var managedFolder = Path.Combine(Application.dataPath, "..", "RustDedicated_Data", "Managed");
			_metadataReferences.Add(MetadataReference.CreateFromStream(new MemoryStream(OsEx.File.ReadBytes(Path.Combine(managedFolder, "System.Drawing.dll")))));

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (var assembly in assemblies)
			{
				if (assembly.IsDynamic || !OsEx.File.Exists(assembly.Location) || CarbonLoader.AssemblyCache.Contains(assembly)) continue;

				_metadataReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
			}
		}

		internal static List<MetadataReference> _metadataReferences = new List<MetadataReference>();
		internal static Dictionary<string, MetadataReference> _referenceCache = new Dictionary<string, MetadataReference>();
		internal static Dictionary<string, byte[]> _compilationCache = new Dictionary<string, byte[]>();

		internal static byte[] _getPlugin(string name)
		{
			if (!_compilationCache.TryGetValue(name, out var result)) return null;

			return result;
		}
		internal static void _overridePlugin(string name, byte[] pluginAssembly)
		{
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

		internal static MetadataReference _getReferenceFromCache(string reference)
		{
			if (!_referenceCache.TryGetValue(reference, out var metaReference))
			{
				var referencePath = Path.Combine(Application.dataPath, "..", "RustDedicated_Data", "Managed", $"{reference}.dll");
				var outReference = MetadataReference.CreateFromFile(referencePath);
				if (outReference == null) return null;

				_referenceCache.Add(reference, outReference);
			}

			return metaReference;
		}

		internal List<MetadataReference> _addReferences()
		{
			var references = new List<MetadataReference>();
			references.AddRange(_metadataReferences);

			foreach (var reference in References)
			{
				if (string.IsNullOrEmpty(reference) || _metadataReferences.Any(x => x.Display.Contains(reference))) continue;

				try
				{
					var outReference = _getReferenceFromCache(reference);
					if (outReference != null && !references.Contains(outReference)) references.Add(outReference);
				}
				catch { }
			}

			return references;
		}

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

		public override void Start()
		{
			try
			{

				FileName = Path.GetFileNameWithoutExtension(FilePath);

				_doInit();
			}
			catch (Exception ex) { Console.WriteLine($"Couldn't compile '{FileName}'\n{ex}"); }

			base.Start();
		}

		public override void ThreadFunction()
		{
			try
			{
				Exceptions.Clear();

				TimeSinceCompile = 0;

				var references = _addReferences();
				var trees = new List<SyntaxTree>();
				trees.Add(CSharpSyntaxTree.ParseText(Source));

				foreach (var require in Requires)
				{
					var requiredPlugin = _getPlugin(require);

					using (var dllStream = new MemoryStream(requiredPlugin))
					{
						references.Add(MetadataReference.CreateFromStream(dllStream));
					}
				}

				var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release, warningLevel: 4);
				var compilation = CSharpCompilation.Create($"{FileName}_{RandomEx.GetRandomInteger()}", trees, references, options);

				using (var dllStream = new MemoryStream())
				{
					var emit = compilation.Emit(dllStream);

					foreach (var error in emit.Diagnostics)
					{
						var span = error.Location.GetMappedLineSpan().Span;
						switch (error.Severity)
						{
							case DiagnosticSeverity.Error:
								Exceptions.Add(new CompilerException(FilePath, new CompilerError(FileName, span.Start.Line + 1, span.Start.Character + 1, error.Id, error.GetMessage(CultureInfo.InvariantCulture))));
								break;
						}
					}

					if (emit.Success)
					{
						var assembly = dllStream.ToArray();
						if (assembly != null)
						{
							_overridePlugin(FileName, assembly);
							Assembly = Assembly.Load(assembly);
						}
					}
				}

				if (Assembly == null)
				{
					throw null;
				}

				CompileTime = TimeSinceCompile;

				references.Clear();
				references = null;
				trees.Clear();
				trees = null;

				foreach (var type in Assembly.GetTypes())
				{
					var hooks = new List<string>();
					var unsupportedHooks = new List<string>();
					var hookMethods = new List<HookMethodAttribute>();
					var pluginReferences = new List<PluginReferenceAttribute>();
					Hooks.Add(type, hooks);
					UnsupportedHooks.Add(type, unsupportedHooks);
					HookMethods.Add(type, hookMethods);
					PluginReferences.Add(type, pluginReferences);

					foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
					{
						if (CarbonHookValidator.IsIncompatibleOxideHook(method.Name))
						{
							unsupportedHooks.Add(method.Name);
						}

						if (CarbonCore.Instance.HookProcessor.DoesHookExist(method.Name))
						{
							if (!hooks.Contains(method.Name)) hooks.Add(method.Name);
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
			catch (Exception ex) { Console.WriteLine($"Threading compilation failed ({ex.Message})\n{ex.StackTrace}"); }
		}
	}
}
