using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace Carbon.Validation.Metadata;

/// <summary>
///     Generic parameter names in scope while decoding a signature. Definitions decode with the
///     declaring member's parameter names (Cecil prints "T"); references decode without a context
///     and fall back to positional names (Cecil prints "!0" / "!!0").
/// </summary>
internal sealed record MetadataGenericContext(ImmutableArray<string> TypeParameters, ImmutableArray<string> MethodParameters)
{
	public static readonly MetadataGenericContext Empty = new([], []);
}

/// <summary>
///     Decodes metadata signatures into Mono.Cecil-style full name strings ("Ns.Outer/Inner",
///     "System.Collections.Generic.List`1&lt;System.String&gt;", "System.Single&amp;"), the format the
///     Oxide patcher writes into OPJ files and hashes.
/// </summary>
internal sealed class PatcherSignatureProvider : ISignatureTypeProvider<string, MetadataGenericContext?>
{
	public string GetPrimitiveType(PrimitiveTypeCode typeCode)
	{
		return typeCode switch
		{
			PrimitiveTypeCode.Void => "System.Void",
			PrimitiveTypeCode.Boolean => "System.Boolean",
			PrimitiveTypeCode.Char => "System.Char",
			PrimitiveTypeCode.SByte => "System.SByte",
			PrimitiveTypeCode.Byte => "System.Byte",
			PrimitiveTypeCode.Int16 => "System.Int16",
			PrimitiveTypeCode.UInt16 => "System.UInt16",
			PrimitiveTypeCode.Int32 => "System.Int32",
			PrimitiveTypeCode.UInt32 => "System.UInt32",
			PrimitiveTypeCode.Int64 => "System.Int64",
			PrimitiveTypeCode.UInt64 => "System.UInt64",
			PrimitiveTypeCode.Single => "System.Single",
			PrimitiveTypeCode.Double => "System.Double",
			PrimitiveTypeCode.String => "System.String",
			PrimitiveTypeCode.TypedReference => "System.TypedReference",
			PrimitiveTypeCode.IntPtr => "System.IntPtr",
			PrimitiveTypeCode.UIntPtr => "System.UIntPtr",
			PrimitiveTypeCode.Object => "System.Object",
			_ => "System.Object",
		};
	}

	public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
	{
		return GetTypeDefinitionName(reader, handle);
	}

	public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
	{
		return GetTypeReferenceName(reader, handle);
	}

	public string GetTypeFromSpecification(MetadataReader reader, MetadataGenericContext? genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
	{
		return reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
	}

	public string GetSZArrayType(string elementType)
	{
		return elementType + "[]";
	}

	public string GetArrayType(string elementType, ArrayShape shape)
	{
		// Cecil prints sized/bounded dimensions ("[0...,0...]"), which is how compilers emit
		// multi-dimensional arrays.
		var dimensions = new string[Math.Max(shape.Rank, 1)];
		for (var i = 0; i < dimensions.Length; i++)
		{
			int? lower = i < shape.LowerBounds.Length ? shape.LowerBounds[i] : null;
			int? upper = i < shape.Sizes.Length ? (lower ?? 0) + shape.Sizes[i] - 1 : null;
			dimensions[i] = lower == null && upper == null ? string.Empty : $"{lower}...{upper}";
		}

		return elementType + "[" + string.Join(",", dimensions) + "]";
	}

	public string GetByReferenceType(string elementType)
	{
		return elementType + "&";
	}

	public string GetPointerType(string elementType)
	{
		return elementType + "*";
	}

	public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
	{
		return genericType + "<" + string.Join(",", typeArguments) + ">";
	}

	public string GetGenericTypeParameter(MetadataGenericContext? genericContext, int index)
	{
		if (genericContext != null && index < genericContext.TypeParameters.Length)
		{
			return genericContext.TypeParameters[index];
		}

		return "!" + index;
	}

	public string GetGenericMethodParameter(MetadataGenericContext? genericContext, int index)
	{
		if (genericContext != null && index < genericContext.MethodParameters.Length)
		{
			return genericContext.MethodParameters[index];
		}

		return "!!" + index;
	}

	public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired)
	{
		return unmodifiedType + (isRequired ? " modreq(" : " modopt(") + modifier + ")";
	}

	public string GetPinnedType(string elementType)
	{
		return elementType;
	}

	public string GetFunctionPointerType(MethodSignature<string> signature)
	{
		return signature.ReturnType + " *(" + string.Join(",", signature.ParameterTypes) + ")";
	}

	/// <summary>
	///     Builds the Cecil-style full name of a type definition ("Ns.Outer/Inner").
	/// </summary>
	public static string GetTypeDefinitionName(MetadataReader reader, TypeDefinitionHandle handle)
	{
		var definition = reader.GetTypeDefinition(handle);
		var name = reader.GetString(definition.Name);
		var declaring = definition.GetDeclaringType();
		if (!declaring.IsNil)
		{
			return GetTypeDefinitionName(reader, declaring) + "/" + name;
		}

		var ns = definition.Namespace.IsNil ? string.Empty : reader.GetString(definition.Namespace);
		return string.IsNullOrEmpty(ns) ? name : ns + "." + name;
	}

	/// <summary>
	///     Builds the Cecil-style full name of a type reference ("Ns.Outer/Inner").
	/// </summary>
	public static string GetTypeReferenceName(MetadataReader reader, TypeReferenceHandle handle)
	{
		var reference = reader.GetTypeReference(handle);
		var name = reader.GetString(reference.Name);
		if (reference.ResolutionScope.Kind == HandleKind.TypeReference)
		{
			return GetTypeReferenceName(reader, (TypeReferenceHandle)reference.ResolutionScope) + "/" + name;
		}

		var ns = reference.Namespace.IsNil ? string.Empty : reader.GetString(reference.Namespace);
		return string.IsNullOrEmpty(ns) ? name : ns + "." + name;
	}
}
