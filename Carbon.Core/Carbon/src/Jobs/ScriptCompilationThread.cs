///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Carbon.Base;
using Carbon.Common;
using Carbon.Components;
using Carbon.Core;
using Carbon.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Carbon.Jobs
{
	public class ScriptCompilationThread : BaseThreadedJob
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

		internal static bool _hasInit { get; set; }
		internal static void _doInit()
		{
			if (_hasInit) return;
			_hasInit = true;
		}

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

		private float lastReferenceUpdate;
		private static HashSet<MetadataReference> cachedReferences = new HashSet<MetadataReference>();

		internal HashSet<MetadataReference> _addReferences()
		{
			if (AssemblyResolver.LastCacheUpdate > lastReferenceUpdate)
			{
				lastReferenceUpdate = AssemblyResolver.LastCacheUpdate;
				cachedReferences.Clear();

				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					try
					{
						if (!AssemblyResolver.IsReferenceAllowed(assembly) || assembly.IsDynamic) continue;
						CarbonReference asm = AssemblyResolver.GetAssembly(assembly.GetName().Name);
						if (asm == null || asm.assembly.IsDynamic) continue;

						using (MemoryStream mem = new MemoryStream(asm.raw))
							cachedReferences.Add(MetadataReference.CreateFromStream(mem));
					}
					catch { }
				}

				foreach (string item in References)
				{
					try
					{
						if (string.IsNullOrEmpty(item) || !AssemblyResolver.IsReferenceAllowed(item)) continue;
						CarbonReference asm = AssemblyResolver.GetAssembly(item);
						if (asm == null || asm.assembly.IsDynamic) continue;

						using (MemoryStream mem = new MemoryStream(asm.raw))
							cachedReferences.Add(MetadataReference.CreateFromStream(mem));
					}
					catch { }
				}
			}

			return cachedReferences;
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
			catch (Exception ex) { Logger.Error($"Couldn't compile '{FileName}'", ex); }

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
					try
					{
						var requiredPlugin = _getPlugin(require);

						using (var dllStream = new MemoryStream(requiredPlugin))
						{
							references.Add(MetadataReference.CreateFromStream(dllStream));
						}
					}
					catch { }
				}

				var options = new CSharpCompilationOptions(
					OutputKind.DynamicallyLinkedLibrary,
					optimizationLevel: OptimizationLevel.Release,
					warningLevel: 4, deterministic: true
				);

				var compilation = CSharpCompilation.Create(
					$"{FileName}_{RandomEx.GetRandomInteger()}", trees, references, options);

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
						if (HookValidator.IsIncompatibleOxideHook(method.Name))
						{
							unsupportedHooks.Add(method.Name);
						}

						if (Community.Runtime.HookProcessor.DoesHookExist(method.Name))
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
			catch (Exception ex) { Logger.Error($"Threading compilation failed ({ex.Message})", ex); }
		}
	}
}
