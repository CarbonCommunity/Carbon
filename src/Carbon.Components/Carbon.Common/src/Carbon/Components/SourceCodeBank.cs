using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using Unity.Collections.LowLevel.Unsafe;

namespace Carbon.Components;

/// <summary>
/// To access the text source code of an assembly, type or method, this is an useful component to do just that.
/// </summary>
public class SourceCodeBank
{
	public static CachedAssemblyBank AssemblyBank { get; } = new();

	public class CachedAssemblyBank : Dictionary<string, SourceCode>;

	public static SourceCode Parse(string assemblyPath)
	{
		if (!AssemblyBank.TryGetValue(assemblyPath, out var source))
		{
			AssemblyBank[assemblyPath] = source = SourceCode.Get(assemblyPath);
		}

		return source;
	}
	public static SourceCode Parse(string name, PEFile file)
	{
		if (!AssemblyBank.TryGetValue(name, out var source))
		{
			AssemblyBank[name] = source = SourceCode.Get(file);
		}

		return source;
	}
	public static unsafe SourceCode Parse(string name, ModuleHandle handle)
	{
		if (!AssemblyBank.TryGetValue(name, out var source))
		{
			var image = MonoProfiler.MonoImage.handle_to_image(handle);

			var reader = new PEReader(image->raw_data, (int)image->raw_data_len);
			var pefile = new PEFile(name, reader);

			AssemblyBank[name] = source = SourceCode.Get(pefile);
		}

		return source;
	}

	public struct SourceCode
	{
		public Dictionary<string, string> Types;
		public Dictionary<string, string> Methods;

		public CSharpDecompiler Decompiler;
		public DecompilerSettings Settings;

		public static SourceCode Get(string assemblyPath)
		{
			SourceCode sourceCode = default;

			sourceCode.Types = new();
			sourceCode.Methods = new();
			sourceCode.Settings = new()
			{
				ThrowOnAssemblyResolveErrors = false
			};
			sourceCode.Decompiler = new CSharpDecompiler(assemblyPath, sourceCode.Settings);

			return sourceCode;
		}
		public static SourceCode Get(PEFile file)
		{
			SourceCode sourceCode = default;

			sourceCode.Types = new();
			sourceCode.Methods = new();
			sourceCode.Settings = new()
			{
				ThrowOnAssemblyResolveErrors = false
			};
			sourceCode.Decompiler = new CSharpDecompiler(file, new UniversalAssemblyResolver(null, false,
				file.DetectTargetFrameworkId(), file.DetectRuntimePack(),
				sourceCode.Settings.LoadInMemory ? PEStreamOptions.PrefetchMetadata : PEStreamOptions.Default,
				sourceCode.Settings.ApplyWindowsRuntimeProjections ? MetadataReaderOptions.ApplyWindowsRuntimeProjections : MetadataReaderOptions.None), sourceCode.Settings);

			return sourceCode;
		}

		public string ParseType(string type)
		{
			if (Types.TryGetValue(type, out var source)) return source;

			var typeCode = Decompiler.TypeSystem.MainModule.TypeDefinitions.FirstOrDefault(x => x.FullName.Equals(type, StringComparison.OrdinalIgnoreCase));

			if (typeCode == null)
			{
				return string.Empty;
			}

			Settings.UsingDeclarations = true;

			return Types[type] = Decompiler.DecompileTypeAsString(typeCode.FullTypeName);
		}
		public string ParseMethod(string type, string method)
		{
			var id = $"{type}:{method}";

			if (Methods.TryGetValue(id, out var source)) return source;

			var typeCode = Decompiler.TypeSystem.MainModule.TypeDefinitions.FirstOrDefault(x => x.FullName.Equals(type, StringComparison.OrdinalIgnoreCase));

			if (typeCode == null)
			{
				return string.Empty;
			}

			var methodCode = typeCode.Methods.FirstOrDefault(x => x.Name.Equals(method, StringComparison.OrdinalIgnoreCase));

			if (methodCode == null)
			{
				return string.Empty;
			}

			Settings.UsingDeclarations = false;

			return Methods[id] = Decompiler.DecompileAsString(methodCode.MetadataToken);
		}
		public string ParseMethod(uint token)
		{
			var id = token.ToString();
			if (Methods.TryGetValue(id, out var source)) return source;

			Settings.UsingDeclarations = false;

			return Methods[id] = Decompiler.DecompileAsString(Decompiler.TypeSystem.MainModule.GetDefinition(UnsafeUtility.As<uint, MethodDefinitionHandle>(ref token)).MetadataToken);
		}
		public unsafe string ParseMethod(MonoProfiler.MonoMethod* methodInfo, out string type, out string method)
		{
			var token = methodInfo->token;
			var id = token.ToString();
			type = null;
			method = null;

			if (Methods.TryGetValue(id, out var source)) return source;

			Settings.UsingDeclarations = false;

			var iMethod = Decompiler.TypeSystem.MainModule.GetDefinition(UnsafeUtility.As<uint, MethodDefinitionHandle>(ref token));

			if (iMethod == null)
			{
				return "No method body available";
			}

			type = iMethod.DeclaringType.FullName;
			method = iMethod.Name;

			return Methods[id] = Decompiler.DecompileAsString(iMethod.MetadataToken);
		}
	}
}
