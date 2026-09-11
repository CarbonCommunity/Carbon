using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace Carbon.Validation.Metadata;

/// <summary>
///     A single game assembly opened for metadata inspection: type lookup by patcher-style name,
///     method lookup, and Cecil-compatible rendering of IL operand tokens.
/// </summary>
internal sealed class MetadataAssembly : IDisposable
{
	private readonly PEReader _pe;
	private readonly PatcherSignatureProvider _provider;
	private readonly Dictionary<string, TypeDefinitionHandle> _typesByName = new(StringComparer.Ordinal);
	private readonly Dictionary<TypeDefinitionHandle, MetadataType> _types = [];
	private readonly Dictionary<MethodDefinitionHandle, MetadataMethod> _methods = [];

	private bool? _isLikelyPublicized;

	public MetadataReader Reader { get; }

	public string FilePath { get; }

	/// <summary>
	///     True when virtually every type is public, which means the assembly went through the
	///     Carbon publicizer and its accessibility metadata no longer reflects the vanilla game.
	/// </summary>
	public bool IsLikelyPublicized => _isLikelyPublicized ??= ComputeIsLikelyPublicized();

	public MetadataAssembly(string filePath)
	{
		FilePath = filePath;
		_pe = new PEReader(File.OpenRead(filePath));
		Reader = _pe.GetMetadataReader();
		_provider = new PatcherSignatureProvider();

		foreach (var handle in Reader.TypeDefinitions)
		{
			_typesByName.TryAdd(PatcherSignatureProvider.GetTypeDefinitionName(Reader, handle), handle);
		}
	}

	private bool ComputeIsLikelyPublicized()
	{
		var total = 0;
		var visible = 0;
		var compilerGenerated = 0;
		var compilerGeneratedVisible = 0;
		foreach (var handle in Reader.TypeDefinitions)
		{
			var definition = Reader.GetTypeDefinition(handle);
			var visibility = definition.Attributes & System.Reflection.TypeAttributes.VisibilityMask;
			var isVisible = visibility is not System.Reflection.TypeAttributes.NotPublic
				and not System.Reflection.TypeAttributes.NestedPrivate
				and not System.Reflection.TypeAttributes.NestedAssembly
				and not System.Reflection.TypeAttributes.NestedFamANDAssem;

			total++;
			if (isVisible)
			{
				visible++;
			}

			// Compiler-generated nested types ("<>c__DisplayClass...", "<Foo>d__12") are always
			// private in vanilla assemblies; seeing them public is the publicizer's signature.
			var name = Reader.GetString(definition.Name);
			if (name.StartsWith('<'))
			{
				compilerGenerated++;
				if (isVisible)
				{
					compilerGeneratedVisible++;
				}
			}
		}

		if (compilerGenerated > 0 && compilerGeneratedVisible == compilerGenerated && total >= 10)
		{
			return true;
		}

		return total > 100 && visible >= total * 0.999;
	}

	/// <summary>
	///     Finds a type by its patcher-style full name ("Ns.Outer/Inner").
	/// </summary>
	public MetadataType? FindType(string patcherTypeName)
	{
		return _typesByName.TryGetValue(patcherTypeName, out var handle) ? GetType(handle) : null;
	}

	public MetadataType GetType(TypeDefinitionHandle handle)
	{
		if (_types.TryGetValue(handle, out var type))
		{
			return type;
		}

		type = new MetadataType(this, handle);
		_types.Add(handle, type);
		return type;
	}

	/// <summary>
	///     Finds a method by patcher-style type name, method name and (optionally) parameter type
	///     names, compared through <see cref="Utility.Tools.TypeNameSanitizerEx" /> so patcher and
	///     reflection spellings of the same type match.
	/// </summary>
	public MetadataMethod? FindMethod(string patcherTypeName, string methodName, string[]? parameters)
	{
		var type = FindType(patcherTypeName);
		if (type == null)
		{
			return null;
		}

		MetadataMethod? loose = null;
		var looseCount = 0;
		foreach (var method in type.Methods)
		{
			if (!string.Equals(method.Name, methodName, StringComparison.Ordinal))
			{
				continue;
			}

			loose = method;
			looseCount++;
			if (parameters != null && SignatureComparer.ParametersMatch(method.ParameterTypes, parameters))
			{
				return method;
			}
		}

		// Mirror the generator's fallback: a unique name match wins even when the recorded
		// signature no longer lines up.
		return looseCount == 1 ? loose : null;
	}

	public MetadataMethod? GetMethodByToken(int token)
	{
		var handle = MetadataTokens.EntityHandle(token);
		return handle.Kind == HandleKind.MethodDefinition ? GetMethod((MethodDefinitionHandle)handle) : null;
	}

	public MetadataMethod GetMethod(MethodDefinitionHandle handle)
	{
		if (_methods.TryGetValue(handle, out var method))
		{
			return method;
		}

		method = new MetadataMethod(this, handle);
		_methods.Add(handle, method);
		return method;
	}

	public MethodBodyBlock? GetMethodBody(int relativeVirtualAddress)
	{
		return relativeVirtualAddress == 0 ? null : _pe.GetMethodBody(relativeVirtualAddress);
	}

	public MethodSignature<string> DecodeMethodSignature(MethodDefinitionHandle handle, MetadataGenericContext context)
	{
		return Reader.GetMethodDefinition(handle).DecodeSignature(_provider, context);
	}

	public ImmutableArray<string> DecodeLocalSignature(StandaloneSignatureHandle handle, MetadataGenericContext context)
	{
		return Reader.GetStandaloneSignature(handle).DecodeLocalSignature(_provider, context);
	}

	public MetadataGenericContext GetGenericContext(MethodDefinitionHandle handle)
	{
		var definition = Reader.GetMethodDefinition(handle);
		return new MetadataGenericContext(
			GetGenericParameterNames(Reader.GetTypeDefinition(definition.GetDeclaringType()).GetGenericParameters()),
			GetGenericParameterNames(definition.GetGenericParameters()));
	}

	private ImmutableArray<string> GetGenericParameterNames(GenericParameterHandleCollection handles)
	{
		var builder = ImmutableArray.CreateBuilder<string>(handles.Count);
		foreach (var handle in handles)
		{
			builder.Add(Reader.GetString(Reader.GetGenericParameter(handle).Name));
		}

		return builder.MoveToImmutable();
	}

	/// <summary>
	///     Renders an IL operand token the way Mono.Cecil 0.9.5's Instruction.ToString() does, which
	///     is what the Oxide patcher feeds into the MSIL hash (e.g.
	///     "System.Void Bootstrap::Init_Tier0()").
	/// </summary>
	public string RenderToken(int token, MetadataGenericContext context)
	{
		try
		{
			var handle = MetadataTokens.EntityHandle(token);
			return handle.Kind switch
			{
				HandleKind.TypeDefinition => PatcherSignatureProvider.GetTypeDefinitionName(Reader, (TypeDefinitionHandle)handle),
				HandleKind.TypeReference => PatcherSignatureProvider.GetTypeReferenceName(Reader, (TypeReferenceHandle)handle),
				HandleKind.TypeSpecification => Reader.GetTypeSpecification((TypeSpecificationHandle)handle)
					.DecodeSignature(_provider, context),
				HandleKind.MethodDefinition => RenderMethodDefinition((MethodDefinitionHandle)handle),
				HandleKind.FieldDefinition => RenderFieldDefinition((FieldDefinitionHandle)handle),
				HandleKind.MemberReference => RenderMemberReference((MemberReferenceHandle)handle, context),
				HandleKind.MethodSpecification => RenderMethodSpecification((MethodSpecificationHandle)handle, context),
				HandleKind.StandaloneSignature => RenderStandaloneSignature((StandaloneSignatureHandle)handle, context),
				_ => $"/* 0x{token:x8} */",
			};
		}
		catch (Exception)
		{
			return $"/* 0x{token:x8} */";
		}
	}

	private string RenderMethodDefinition(MethodDefinitionHandle handle)
	{
		var definition = Reader.GetMethodDefinition(handle);
		var declaringType = PatcherSignatureProvider.GetTypeDefinitionName(Reader, definition.GetDeclaringType());
		var signature = definition.DecodeSignature(_provider, GetGenericContext(handle));
		var name = Reader.GetString(definition.Name);
		return $"{signature.ReturnType} {declaringType}::{name}({string.Join(",", signature.ParameterTypes)})";
	}

	private string RenderFieldDefinition(FieldDefinitionHandle handle)
	{
		var definition = Reader.GetFieldDefinition(handle);
		var declaringHandle = definition.GetDeclaringType();
		var declaringType = PatcherSignatureProvider.GetTypeDefinitionName(Reader, declaringHandle);
		var context = new MetadataGenericContext(
			GetGenericParameterNames(Reader.GetTypeDefinition(declaringHandle).GetGenericParameters()), []);
		var fieldType = definition.DecodeSignature(_provider, context);
		return $"{fieldType} {declaringType}::{Reader.GetString(definition.Name)}";
	}

	private string RenderMemberReference(MemberReferenceHandle handle, MetadataGenericContext context)
	{
		var reference = Reader.GetMemberReference(handle);
		var parent = RenderMemberReferenceParent(reference.Parent, context);
		var name = Reader.GetString(reference.Name);

		// Cecil resolves generic type parameters in reference blobs against the parent's element
		// type when that is a definition in this module ("TInner"); otherwise they stay positional
		// ("!0"). Method generic parameters in reference blobs are always positional ("!!0").
		var memberContext = GetMemberReferenceContext(reference.Parent);

		if (reference.GetKind() == MemberReferenceKind.Field)
		{
			var fieldType = reference.DecodeFieldSignature(_provider, memberContext);
			return $"{fieldType} {parent}::{name}";
		}

		var signature = reference.DecodeMethodSignature(_provider, memberContext);
		return $"{signature.ReturnType} {parent}::{name}({string.Join(",", signature.ParameterTypes)})";
	}

	/// <summary>
	///     Resolves the generic parameter names visible inside a member reference's signature blob:
	///     when the reference's parent is a generic instantiation of a type defined in this module,
	///     Cecil names the type parameters after the definition's.
	/// </summary>
	private MetadataGenericContext? GetMemberReferenceContext(EntityHandle parent)
	{
		if (parent.Kind != HandleKind.TypeSpecification)
		{
			return null;
		}

		try
		{
			var blob = Reader.GetBlobReader(Reader.GetTypeSpecification((TypeSpecificationHandle)parent).Signature);
			if (blob.ReadSignatureTypeCode() != SignatureTypeCode.GenericTypeInstance)
			{
				return null;
			}

			blob.ReadSignatureTypeCode();
			var element = blob.ReadTypeHandle();
			if (element.Kind != HandleKind.TypeDefinition)
			{
				return null;
			}

			var names = GetGenericParameterNames(Reader.GetTypeDefinition((TypeDefinitionHandle)element).GetGenericParameters());
			return new MetadataGenericContext(names, []);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private string RenderMemberReferenceParent(EntityHandle parent, MetadataGenericContext context)
	{
		return parent.Kind switch
		{
			HandleKind.TypeDefinition => PatcherSignatureProvider.GetTypeDefinitionName(Reader, (TypeDefinitionHandle)parent),
			HandleKind.TypeReference => PatcherSignatureProvider.GetTypeReferenceName(Reader, (TypeReferenceHandle)parent),
			HandleKind.TypeSpecification => Reader.GetTypeSpecification((TypeSpecificationHandle)parent)
				.DecodeSignature(_provider, context),
			HandleKind.MethodDefinition => RenderMethodDefinition((MethodDefinitionHandle)parent),
			_ => "?",
		};
	}

	private string RenderMethodSpecification(MethodSpecificationHandle handle, MetadataGenericContext context)
	{
		var specification = Reader.GetMethodSpecification(handle);
		var arguments = specification.DecodeSignature(_provider, context);
		string declaringType;
		string name;
		MethodSignature<string> signature;

		if (specification.Method.Kind == HandleKind.MethodDefinition)
		{
			var definitionHandle = (MethodDefinitionHandle)specification.Method;
			var definition = Reader.GetMethodDefinition(definitionHandle);
			declaringType = PatcherSignatureProvider.GetTypeDefinitionName(Reader, definition.GetDeclaringType());
			name = Reader.GetString(definition.Name);
			signature = definition.DecodeSignature(_provider, GetGenericContext(definitionHandle));
		}
		else
		{
			var reference = Reader.GetMemberReference((MemberReferenceHandle)specification.Method);
			declaringType = RenderMemberReferenceParent(reference.Parent, context);
			name = Reader.GetString(reference.Name);
			signature = reference.DecodeMethodSignature(_provider, GetMemberReferenceContext(reference.Parent));
		}

		return $"{signature.ReturnType} {declaringType}::{name}<{string.Join(",", arguments)}>({string.Join(",", signature.ParameterTypes)})";
	}

	private string RenderStandaloneSignature(StandaloneSignatureHandle handle, MetadataGenericContext context)
	{
		var signature = Reader.GetStandaloneSignature(handle);
		if (signature.GetKind() != StandaloneSignatureKind.Method)
		{
			return "/* localsig */";
		}

		var method = signature.DecodeMethodSignature(_provider, context);
		return $"{method.ReturnType}({string.Join(",", method.ParameterTypes)})";
	}

	public void Dispose()
	{
		_pe.Dispose();
	}
}

/// <summary>
///     A type definition with its declared methods and nested types, addressed by patcher-style names.
/// </summary>
internal sealed class MetadataType
{
	private IReadOnlyList<MetadataMethod>? _methods;

	public MetadataType(MetadataAssembly assembly, TypeDefinitionHandle handle)
	{
		Assembly = assembly;
		Handle = handle;
		FullName = PatcherSignatureProvider.GetTypeDefinitionName(assembly.Reader, handle);
	}

	public MetadataAssembly Assembly { get; }

	public TypeDefinitionHandle Handle { get; }

	public string FullName { get; }

	public IReadOnlyList<MetadataMethod> Methods => _methods ??= Assembly.Reader.GetTypeDefinition(Handle)
		.GetMethods()
		.Select(Assembly.GetMethod)
		.ToList();
}
